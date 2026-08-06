using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection; // 뉴비용 리플렉션 힌트

// [주의] 이 스크립트도 Editor 폴더 안에 있어야 합니다.
// ※ UI 스크립트가 개편되어 SO 리스트와 CSV 경로 리스트를 직접 받도록 변경되었습니다.
public static class SOCsvConverter
{
    /// <summary>
    /// 전달받은 SO 리스트의 데이터들을 1개의 CSV 파일로 모아서 추출합니다.
    /// </summary>
    public static bool ExportToCSV(List<ScriptableObject> soList, Type targetType, string saveDirectory)
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        string filePath = $"{saveDirectory}/{targetType.Name}.csv";

        try
        {
            /* ==========================================
             * [과제 1: SO 리스트 -> 1개의 CSV 파일로 변환기 구현]
             * 
             * 1. targetType.GetFields(...) 를 이용해 [SerializeField] 속성이 있거나 Public인 필드들을 가져오세요.
             * 2. 첫 번째 줄(헤더) 문자열을 만드세요. (예: "ObjectName,id,dish,cost...") 
             *    ※ 표의 첫 컬럼은 SO 파일의 이름(so.name)을 키값으로 기록하는 것이 좋습니다.
             * 3. foreach (var so in soList) 를 돌면서 SO 1개당 1줄(Row)씩 데이터를 연결하세요.
             * 4. 줄바꿈(\n)으로 각 줄을 합쳐서 File.WriteAllText(filePath, 전체문자열) 로 한 번에 저장하세요.
             * ========================================== */

            Debug.Log($"[작업중] {targetType.Name}.csv 단일 파일 추출 로직 구현 필요...");

            // 뉴비가 지우고 제대로 짤 부분 (임시 데이터)
            File.WriteAllText(filePath, "ObjectName,TestHeader1,TestHeader2\nTempSO1,10,20\nTempSO2,30,40");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV Export 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 단일 CSV 파일을 읽어와서 각 줄(Row)의 데이터를 찾아 동명의 SO에 덮어씌우거나 새로 생성합니다.
    /// </summary>
    public static bool ImportFromCSV(string csvPath, Type targetType)
    {
        try
        {
            if (!File.Exists(csvPath))
            {
                Debug.LogWarning("CSV 파일을 찾을 수 없습니다.");
                return false;
            }

            /* ==========================================
             * [과제 2: 1개의 CSV 파일 -> SO 리스트 덮어쓰기 구현]
             * 
             * 1. File.ReadAllLines(csvPath) 로 CSV 텍스트를 줄 단위로 읽으세요.
             * 2. 첫 번째 줄(헤더)을 쪼개서 필드 이름 배열을 만드세요.
             * 3. 2번째 줄(데이터)부터 반복문을 돕니다.
             * 4. 데이터의 첫 번째 열(ObjectName)을 기준으로 에셋을 찾습니다.
             *    ( AssetDatabase.FindAssets($"{오브젝트이름} t:{targetType.Name}") 활용 )
             * 5. 에셋이 없다면 ScriptableObject.CreateInstance(targetType) 로 새로 만들고 AssetDatabase.CreateAsset()으로 파일 생성!
             * 6. 리플렉션을 이용해 문자열을 원래 타입으로 캐스팅해서 SO에 밀어 넣으세요.
             * 7. 값이 변경된 SO는 EditorUtility.SetDirty(so) 를 호출해야 유니티가 변경을 감지하고 저장합니다.
             * ========================================== */

            Debug.Log($"[작업중] {Path.GetFileName(csvPath)} 파일 읽어서 SO 업데이트 구현 필요...");

            // 모든 변경사항을 디스크에 일괄 저장
            AssetDatabase.SaveAssets();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV Import 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// CSV에서 지원할 SO 필드 목록을 반환합니다.
    /// TODO: public 또는 [SerializeField] 필드 중 단순 타입만 선별하세요.
    /// </summary>
    internal static FieldInfo[] GetSupportedFields(Type targetType)
    {
        return Array.Empty<FieldInfo>();
    }

    /// <summary>
    /// SO 필드 값을 CSV 셀에 표시할 문자열로 변환합니다.
    /// TODO: 문자열, 숫자, bool, enum 변환을 구현하세요.
    /// </summary>
    internal static string SerializeValueForCsv(object value, Type valueType)
    {
        return string.Empty;
    }

    /// <summary>
    /// CSV 전체 문자열을 행과 셀 단위로 파싱합니다.
    /// TODO: 쉼표, 큰따옴표, 줄바꿈 규칙을 처리하세요.
    /// </summary>
    internal static List<string[]> ParseCsv(string csvText)
    {
        return new List<string[]>();
    }
}
