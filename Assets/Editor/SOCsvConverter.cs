using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

// Assets/Editor/SOCsvConverter.cs에 넣는 Editor 전용 코드입니다.
public static class SOCsvConverter
{
    private const string ObjectNameHeader = "ObjectName";

    // 검사가 끝났지만 아직 SO에는 적용하지 않은 CSV 한 행입니다.
    // 먼저 이 목록을 모두 만들고, 성공했을 때만 실제 SO를 수정합니다.
    private class PendingRow
    {
        public string objectName;
        public Dictionary<FieldInfo, object> values = new Dictionary<FieldInfo, object>();
        public string newAssetPath;
    }

    // -------------------------------------------------------------------------
    // 1. Export : SO 여러 개를 CSV 하나로 저장
    // -------------------------------------------------------------------------
    public static bool ExportToCSV(List<ScriptableObject> soList, Type targetType, string saveDirectory)
    {
        try
        {
            FieldInfo[] fields = GetSupportedFields(targetType);
            Directory.CreateDirectory(saveDirectory);

            StringBuilder csv = new StringBuilder();

            // 첫 행은 헤더입니다. ObjectName은 SO를 찾는 고유 키입니다.
            WriteCsvCell(csv, ObjectNameHeader);
            for (int i = 0; i < fields.Length; i++)
            {
                csv.Append(',');
                WriteCsvCell(csv, fields[i].Name);
            }

            HashSet<string> usedNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (ScriptableObject so in soList)
            {
                if (so == null || so.GetType() != targetType) continue;

                // 같은 이름은 Import 때 어느 SO인지 판단할 수 없으므로 막습니다.
                if (!usedNames.Add(so.name))
                    throw new Exception("같은 이름의 SO가 둘 이상입니다: " + so.name);

                csv.Append('\n');
                WriteCsvCell(csv, so.name);

                for (int i = 0; i < fields.Length; i++)
                {
                    csv.Append(',');
                    object fieldValue = fields[i].GetValue(so);
                    string text = SerializeValueForCsv(fieldValue, fields[i].FieldType);
                    WriteCsvCell(csv, text);
                }
            }

            string path = Path.Combine(saveDirectory, targetType.Name + ".csv").Replace('\\', '/');
            File.WriteAllText(path, csv.ToString(), new UTF8Encoding(false));
            Debug.Log("CSV Export 완료: " + path);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("CSV Export 실패: " + e.Message);
            return false;
        }
    }

    // -------------------------------------------------------------------------
    // 2. Import : CSV를 먼저 전부 검사하고, 이상이 없을 때 SO를 수정
    // -------------------------------------------------------------------------
    public static bool ImportFromCSV(string csvPath, Type targetType)
    {
        try
        {
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("CSV 파일을 찾을 수 없습니다.", csvPath);

            // [중요] 여기까지는 CSV를 읽고 검사만 합니다. 아직 SO를 수정하지 않습니다.
            List<PendingRow> pendingRows = ReadAndValidateCsv(csvPath, targetType);
            Dictionary<string, ScriptableObject> existingSOs = FindExistingSOs(targetType);

            // 새 SO가 필요하다면 만들 위치와 파일명을 미리 검사합니다.
            string newAssetFolder = GetNewAssetFolder(csvPath, targetType);
            for (int i = 0; i < pendingRows.Count; i++)
            {
                if (!existingSOs.ContainsKey(pendingRows[i].objectName))
                    pendingRows[i].newAssetPath = MakeNewAssetPath(newAssetFolder, pendingRows[i].objectName);
            }

            // 이제부터 실제 수정 단계입니다.
            Directory.CreateDirectory(newAssetFolder);
            for (int i = 0; i < pendingRows.Count; i++)
            {
                PendingRow row = pendingRows[i];
                ScriptableObject so;

                if (!existingSOs.TryGetValue(row.objectName, out so))
                {
                    so = ScriptableObject.CreateInstance(targetType);
                    so.name = row.objectName;
                    AssetDatabase.CreateAsset(so, row.newAssetPath);
                    existingSOs.Add(row.objectName, so);
                }

                // CSV에 존재하는 지원 필드만 넣습니다.
                // CSV에 없는 필드는 이 반복문에 오지 않으므로 기존 값을 유지합니다.
                foreach (KeyValuePair<FieldInfo, object> pair in row.values)
                    pair.Key.SetValue(so, pair.Value);

                EditorUtility.SetDirty(so);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("CSV Import 완료: " + csvPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("CSV Import 실패: " + e.Message);
            return false;
        }
    }

    // -------------------------------------------------------------------------
    // 3. Window가 표에 표시할 '지원 필드' 목록 만들기
    // -------------------------------------------------------------------------
    internal static FieldInfo[] GetSupportedFields(Type targetType)
    {
        if (targetType == null || !targetType.IsSubclassOf(typeof(ScriptableObject)))
            return new FieldInfo[0];

        // 부모 클래스의 필드도 포함하려고, 부모부터 자식 순서로 조사합니다.
        List<Type> types = new List<Type>();
        for (Type type = targetType; type != null && type != typeof(ScriptableObject); type = type.BaseType)
            types.Add(type);
        types.Reverse();

        List<FieldInfo> supportedFields = new List<FieldInfo>();
        HashSet<string> fieldNames = new HashSet<string>(StringComparer.Ordinal);
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public |
                             BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        foreach (Type type in types)
        {
            FieldInfo[] fields = type.GetFields(flags);
            foreach (FieldInfo field in fields)
            {
                bool visibleToUnity = (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField))) &&
                                      !field.IsNotSerialized;

                if (field.IsStatic || !visibleToUnity || !IsSupportedType(field.FieldType))
                    continue;

                if (!fieldNames.Add(field.Name))
                    throw new Exception("상속 클래스에 같은 이름의 필드가 있습니다: " + field.Name);

                supportedFields.Add(field);
            }
        }
        return supportedFields.ToArray();
    }

    // SO 값 하나를 CSV의 텍스트 한 칸으로 바꿉니다.
    internal static string SerializeValueForCsv(object value, Type valueType)
    {
        if (value == null) return "";
        if (valueType == typeof(string) || valueType == typeof(char)) return value.ToString();
        if (valueType == typeof(bool)) return (bool)value ? "true" : "false";
        if (valueType.IsEnum) return value.ToString();

        // 숫자는 현재 PC의 언어 설정과 상관없이 항상 . 을 소수점으로 사용합니다.
        if (IsNumberType(valueType)) return Convert.ToString(value, CultureInfo.InvariantCulture);

        throw new NotSupportedException("지원하지 않는 타입입니다: " + valueType.Name);
    }

    // -------------------------------------------------------------------------
    // 4. CSV 파서: 쉼표 / 큰따옴표 / 줄바꿈이 셀 안에 있어도 읽기
    // -------------------------------------------------------------------------
    internal static List<string[]> ParseCsv(string csvText)
    {
        List<string[]> result = new List<string[]>();
        if (string.IsNullOrEmpty(csvText)) return result;

        List<string> currentRow = new List<string>();
        StringBuilder currentCell = new StringBuilder();
        bool inQuotes = false;       // 현재 "..." 안에 있는가?
        bool quoteWasClosed = false; // 닫힌 따옴표 뒤에 이상한 글자가 오지 않았는가?
        bool lastWasNewline = false;

        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // CSV에서 ""는 실제 큰따옴표 한 글자입니다.
                    if (i + 1 < csvText.Length && csvText[i + 1] == '"')
                    {
                        currentCell.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                        quoteWasClosed = true;
                    }
                }
                else
                {
                    currentCell.Append(c);
                }
                lastWasNewline = false;
                continue;
            }

            if (c == '"')
            {
                if (currentCell.Length != 0 || quoteWasClosed)
                    throw new FormatException("큰따옴표 위치가 올바르지 않습니다. (문자 " + (i + 1) + ")");
                inQuotes = true;
                lastWasNewline = false;
            }
            else if (c == ',')
            {
                currentRow.Add(currentCell.ToString());
                currentCell.Length = 0;
                quoteWasClosed = false;
                lastWasNewline = false;
            }
            else if (c == '\r' || c == '\n')
            {
                currentRow.Add(currentCell.ToString());
                result.Add(currentRow.ToArray());
                currentRow.Clear();
                currentCell.Length = 0;
                quoteWasClosed = false;
                if (c == '\r' && i + 1 < csvText.Length && csvText[i + 1] == '\n') i++;
                lastWasNewline = true;
            }
            else
            {
                if (quoteWasClosed)
                    throw new FormatException("닫힌 큰따옴표 뒤에는 쉼표 또는 줄바꿈만 올 수 있습니다.");
                currentCell.Append(c);
                lastWasNewline = false;
            }
        }

        if (inQuotes) throw new FormatException("닫히지 않은 큰따옴표가 있습니다.");

        // 파일 끝이 줄바꿈이 아니면 마지막 행을 추가합니다.
        if (!lastWasNewline)
        {
            currentRow.Add(currentCell.ToString());
            result.Add(currentRow.ToArray());
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // Import 전에 CSV 전체를 검사하고, 변환된 값들을 PendingRow에 담기
    // -------------------------------------------------------------------------
    private static List<PendingRow> ReadAndValidateCsv(string csvPath, Type targetType)
    {
        List<string[]> csvRows = ParseCsv(File.ReadAllText(csvPath));
        if (csvRows.Count == 0) throw new FormatException("CSV가 비어 있습니다.");

        string[] headers = csvRows[0];
        Dictionary<string, int> headerIndexes = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < headers.Length; i++)
        {
            if (string.IsNullOrEmpty(headers[i])) throw new FormatException("빈 헤더가 있습니다.");
            if (headerIndexes.ContainsKey(headers[i])) throw new FormatException("중복 헤더: " + headers[i]);
            headerIndexes.Add(headers[i], i);
        }

        int objectNameIndex;
        if (!headerIndexes.TryGetValue(ObjectNameHeader, out objectNameIndex))
            throw new FormatException("필수 헤더 ObjectName이 없습니다.");

        // "필드 이름 -> 실제 FieldInfo" 사전입니다. 헤더 순서가 바뀌어도 동작하게 해줍니다.
        Dictionary<string, FieldInfo> fieldsByName = new Dictionary<string, FieldInfo>(StringComparer.Ordinal);
        foreach (FieldInfo field in GetSupportedFields(targetType))
            fieldsByName.Add(field.Name, field);

        List<PendingRow> pendingRows = new List<PendingRow>();
        HashSet<string> objectNames = new HashSet<string>(StringComparer.Ordinal);

        for (int rowIndex = 1; rowIndex < csvRows.Count; rowIndex++)
        {
            string[] cells = csvRows[rowIndex];
            if (IsEmptyRow(cells)) continue;
            if (cells.Length != headers.Length)
                throw new FormatException((rowIndex + 1) + "행의 칸 수가 헤더와 다릅니다.");

            string objectName = cells[objectNameIndex];
            if (string.IsNullOrWhiteSpace(objectName))
                throw new FormatException((rowIndex + 1) + "행의 ObjectName이 비어 있습니다.");
            if (!objectNames.Add(objectName))
                throw new FormatException("중복 ObjectName: " + objectName);

            PendingRow pending = new PendingRow();
            pending.objectName = objectName;

            for (int column = 0; column < headers.Length; column++)
            {
                FieldInfo field;
                if (column == objectNameIndex || !fieldsByName.TryGetValue(headers[column], out field))
                    continue; // 지원하지 않거나 CSV에만 있는 열은 무시합니다.

                pending.values.Add(field, ConvertCsvText(cells[column], field.FieldType,
                    rowIndex + 1, headers[column]));
            }
            pendingRows.Add(pending);
        }
        return pendingRows;
    }

    // CSV 텍스트를 실제 필드 타입으로 되돌립니다.
    private static object ConvertCsvText(string text, Type type, int rowNumber, string columnName)
    {
        try
        {
            if (type == typeof(string)) return text;

            if (type == typeof(char))
            {
                if (text.Length != 1) throw new FormatException("char는 한 글자여야 합니다.");
                return text[0];
            }

            if (type == typeof(bool))
            {
                bool value;
                if (!bool.TryParse(text, out value))
                    throw new FormatException("bool은 true 또는 false여야 합니다.");
                return value;
            }

            if (type.IsEnum)
            {
                // 숫자(예: 1)가 아니라 enum 멤버 이름(예: Rare)만 받습니다.
                if (Array.IndexOf(Enum.GetNames(type), text) < 0)
                    throw new FormatException("정의되지 않은 enum 이름입니다.");
                object value = Enum.Parse(type, text, false);
                return value;
            }

            if (IsNumberType(type))
                return Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
        }
        catch (Exception e) when (e is FormatException || e is OverflowException || e is ArgumentException)
        {
            throw new FormatException(rowNumber + "행 " + columnName + " 열의 값 '" + text + "'이(가) 올바르지 않습니다: " + e.Message);
        }

        throw new NotSupportedException("지원하지 않는 타입입니다: " + type.Name);
    }

    // -------------------------------------------------------------------------
    // AssetDatabase 관련 작은 도우미 메서드
    // -------------------------------------------------------------------------
    private static Dictionary<string, ScriptableObject> FindExistingSOs(Type targetType)
    {
        Dictionary<string, ScriptableObject> result = new Dictionary<string, ScriptableObject>(StringComparer.Ordinal);
        string[] guids = AssetDatabase.FindAssets("t:" + targetType.Name);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so == null || so.GetType() != targetType) continue;

            if (result.ContainsKey(so.name))
                throw new Exception("프로젝트에 같은 이름의 SO가 둘 이상입니다: " + so.name);
            result.Add(so.name, so);
        }
        return result;
    }

    private static string GetNewAssetFolder(string csvPath, Type targetType)
    {
        string csvFolder = Path.GetDirectoryName(csvPath);
        if (csvFolder != null) csvFolder = csvFolder.Replace('\\', '/');

        if (string.IsNullOrEmpty(csvFolder) ||
            (csvFolder != "Assets" && !csvFolder.StartsWith("Assets/", StringComparison.Ordinal)))
            throw new Exception("새 SO는 Assets 폴더 안에만 만들 수 있습니다.");

        return csvFolder + "/" + targetType.Name + "_Assets";
    }

    private static string MakeNewAssetPath(string folder, string objectName)
    {
        if (objectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            objectName.Contains("/") || objectName.Contains("\\"))
            throw new FormatException("ObjectName에 파일명 불가 문자가 있습니다: " + objectName);

        return AssetDatabase.GenerateUniqueAssetPath(folder + "/" + objectName + ".asset");
    }

    // -------------------------------------------------------------------------
    // 아주 작은 공통 도우미
    // -------------------------------------------------------------------------
    private static void WriteCsvCell(StringBuilder csv, string value)
    {
        value = value ?? "";
        bool needQuotes = value.IndexOfAny(new char[] { ',', '"', '\r', '\n' }) >= 0;
        if (!needQuotes)
        {
            csv.Append(value);
            return;
        }

        csv.Append('"');
        csv.Append(value.Replace("\"", "\"\""));
        csv.Append('"');
    }

    private static bool IsEmptyRow(string[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
            if (!string.IsNullOrWhiteSpace(cells[i])) return false;
        return true;
    }

    private static bool IsSupportedType(Type type)
    {
        return type == typeof(string) || type == typeof(char) || type == typeof(bool) ||
               type.IsEnum || IsNumberType(type);
    }

    private static bool IsNumberType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) || type == typeof(decimal);
    }
}
