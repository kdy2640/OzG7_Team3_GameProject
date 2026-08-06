using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

// [주의] 이 스크립트는 Editor 폴더 안에 있어야 합니다.
public class SOCsvExporterWindow : EditorWindow
{
    // 사용자가 에디터 창에 끌어다 놓을 SO 스크립트 파일
    private MonoScript targetScript;
    private Type targetType;
    private readonly List<MonoScript> availableScripts = new List<MonoScript>();
    private string[] availableScriptNames = Array.Empty<string>();

    // CSV 파일들이 기본적으로 저장/로드될 루트 폴더
    private string baseCsvDirectory = "Assets/CSV_Exports";

    // 스크롤을 위한 변수
    private Vector2 soScrollPos;
    private Vector2 csvScrollPos;

    // 데이터 리스트 및 리플렉션 캐싱
    private List<ScriptableObject> foundSOs = new List<ScriptableObject>();
    private string targetCsvPath = ""; // [변경] 단일 CSV 경로
    private string[] csvPreviewHeaders = Array.Empty<string>();
    private List<string[]> csvPreviewRows = new List<string[]>(); // [변경] CSV 내부 줄(Row) 데이터
    private System.Reflection.FieldInfo[] targetFields;

    [MenuItem("Tools/SO Data Sync Dashboard")]
    public static void ShowWindow()
    {
        GetWindow<SOCsvExporterWindow>("데이터 동기화 대시보드");
    }

    private void OnEnable()
    {
        RefreshAvailableScripts();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("ScriptableObject <-> CSV 동기화 관리", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("1. 기준 타입 설정", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        int currentIndex = availableScripts.IndexOf(targetScript) + 1;

        EditorGUI.BeginChangeCheck();
        int selectedIndex = EditorGUILayout.Popup("SO 스크립트 (Type):", currentIndex, availableScriptNames);
        if (EditorGUI.EndChangeCheck())
        {
            targetScript = selectedIndex > 0 ? availableScripts[selectedIndex - 1] : null;
            RefreshDataTracking();
        }

        if (GUILayout.Button("목록 갱신", GUILayout.Width(80)))
        {
            RefreshAvailableScripts();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("CSV 저장 위치:", GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();
        string changedDirectory = GUILayout.TextField(baseCsvDirectory);
        if (EditorGUI.EndChangeCheck())
        {
            baseCsvDirectory = changedDirectory;
            RefreshDataTracking();
        }
        GUILayout.EndHorizontal();

        if (targetType != null)
        {
            GUILayout.Label($"현재 타겟 경로: {baseCsvDirectory}/{targetType.Name}.csv", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // 2. SO 현황 테이블 그리기
        DrawSOTable();

        GUILayout.Space(10);

        // 3. CSV 현황 테이블 그리기
        DrawCSVTable();

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        // 버튼 1: CSV -> SO
        GUI.enabled = targetType != null && File.Exists(targetCsvPath);
        if (GUILayout.Button("CSV -> SO 덮어쓰기 (Import)", GUILayout.Height(40)))
        {
            bool success = SOCsvConverter.ImportFromCSV(targetCsvPath, targetType);
            ShowResult(success, "Import");
        }

        GUILayout.Space(10);

        // 버튼 2: SO -> CSV
        GUI.enabled = targetType != null;
        if (GUILayout.Button("SO -> CSV 추출하기 (Export)", GUILayout.Height(40)))
        {
            bool success = SOCsvConverter.ExportToCSV(foundSOs, targetType, baseCsvDirectory);
            ShowResult(success, "Export");
        }

        GUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    private void DrawSOTable()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        GUILayout.Label("2. SO 현황", EditorStyles.boldLabel);
        if (GUILayout.Button("새로고침", GUILayout.Width(80))) RefreshDataTracking();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        DrawTableHeader();

        soScrollPos = GUILayout.BeginScrollView(soScrollPos, GUILayout.Height(150));
        if (foundSOs.Count == 0)
        {
            GUILayout.Label("추적된 SO 데이터가 없습니다.", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            for (int i = 0; i < foundSOs.Count; i++)
            {
                var so = foundSOs[i];
                GUILayout.BeginHorizontal("Label");
                GUILayout.Label(i.ToString(), GUILayout.Width(40));
                GUILayout.Label(so.name, GUILayout.Width(150));

                // 리플렉션을 통해 SO의 실제 내부 필드값 가져오기
                if (targetFields != null)
                {
                    foreach (var field in targetFields)
                    {
                        object val = field.GetValue(so);
                        GUILayout.Label(
                            SOCsvConverter.SerializeValueForCsv(val, field.FieldType),
                            GUILayout.Width(100));
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawCSVTable()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        GUILayout.Label("3. CSV 현황 (단일 파일)", EditorStyles.boldLabel);
        if (!string.IsNullOrEmpty(targetCsvPath) && File.Exists(targetCsvPath))
        {
            GUILayout.Label($"(파일: {Path.GetFileName(targetCsvPath)})", EditorStyles.miniLabel);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        DrawTableHeader();

        csvScrollPos = GUILayout.BeginScrollView(csvScrollPos, GUILayout.Height(150));
        if (csvPreviewRows.Count == 0)
        {
            GUILayout.Label("저장된 CSV 파일이 없거나 데이터가 비어있습니다.", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            for (int i = 0; i < csvPreviewRows.Count; i++)
            {
                string[] rowValues = csvPreviewRows[i];

                GUILayout.BeginHorizontal("Label");
                GUILayout.Label(i.ToString(), GUILayout.Width(40));

                int objectNameIndex = Array.IndexOf(csvPreviewHeaders, "ObjectName");
                string rowName = objectNameIndex >= 0 && objectNameIndex < rowValues.Length
                    ? rowValues[objectNameIndex]
                    : "Unknown";
                GUILayout.Label(rowName, GUILayout.Width(150));

                if (targetFields != null)
                {
                    for (int j = 0; j < targetFields.Length; j++)
                    {
                        int columnIndex = Array.IndexOf(csvPreviewHeaders, targetFields[j].Name);
                        string displayVal = columnIndex >= 0 && columnIndex < rowValues.Length
                            ? rowValues[columnIndex]
                            : "";
                        GUILayout.Label(displayVal, GUILayout.Width(100));
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    // 두 테이블의 헤더(열 이름)를 그리는 공통 함수
    private void DrawTableHeader()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Index", GUILayout.Width(40));
        GUILayout.Label("ObjectName (Key)", GUILayout.Width(150)); // [변경] SO 이름 기준
        if (targetFields != null)
        {
            foreach (var field in targetFields)
            {
                GUILayout.Label(field.Name, GUILayout.Width(100));
            }
        }
        GUILayout.EndHorizontal();
    }

    private void RefreshDataTracking()
    {
        foundSOs.Clear();
        targetFields = null;
        targetCsvPath = "";
        csvPreviewHeaders = Array.Empty<string>();
        csvPreviewRows.Clear();

        if (targetScript == null)
        {
            targetType = null;
            return;
        }

        targetType = targetScript.GetClass();
        if (targetType == null || !targetType.IsSubclassOf(typeof(ScriptableObject)))
        {
            EditorUtility.DisplayDialog("오류", "ScriptableObject를 상속받은 클래스만 등록 가능합니다.", "확인");
            targetScript = null;
            targetType = null;
            return;
        }

        // 1. 상속 계층을 포함해 CSV에서 지원하는 단순 타입 필드만 수집합니다.
        targetFields = SOCsvConverter.GetSupportedFields(targetType);

        // 2. SO 데이터 찾기
        string[] guids = AssetDatabase.FindAssets($"t:{targetType.Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so != null && so.GetType() == targetType)
            {
                foundSOs.Add(so);
            }
        }
        foundSOs = foundSOs.OrderBy(x => x.name).ToList();

        // 3. 단일 CSV 파일 찾기 및 텍스트 파싱 프리뷰 갱신
        targetCsvPath = $"{baseCsvDirectory}/{targetType.Name}.csv";
        if (File.Exists(targetCsvPath))
        {
            try
            {
                List<string[]> rows = SOCsvConverter.ParseCsv(File.ReadAllText(targetCsvPath));
                if (rows.Count > 0)
                {
                    csvPreviewHeaders = rows[0];
                    for (int i = 1; i < rows.Count; i++)
                    {
                        if (rows[i].Any(value => !string.IsNullOrWhiteSpace(value)))
                        {
                            csvPreviewRows.Add(rows[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV 프리뷰 읽기 실패: {e.Message}");
            }
        }
    }

    private void RefreshAvailableScripts()
    {
        MonoScript previousScript = targetScript;
        availableScripts.Clear();

        string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
            if (path.Contains("/Editor/"))
            {
                continue;
            }

            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            Type scriptType = script != null ? script.GetClass() : null;
            if (scriptType == null ||
                !scriptType.IsSubclassOf(typeof(ScriptableObject)) ||
                scriptType.IsAbstract ||
                scriptType.ContainsGenericParameters)
            {
                continue;
            }

            availableScripts.Add(script);
        }

        availableScripts.Sort((left, right) => string.Compare(
            left.GetClass().FullName,
            right.GetClass().FullName,
            StringComparison.Ordinal));

        availableScriptNames = new string[availableScripts.Count + 1];
        availableScriptNames[0] = "선택하세요";
        for (int i = 0; i < availableScripts.Count; i++)
        {
            availableScriptNames[i + 1] = availableScripts[i].GetClass().FullName;
        }

        if (!availableScripts.Contains(previousScript))
        {
            targetScript = null;
            RefreshDataTracking();
        }

        Repaint();
    }

    private void ShowResult(bool success, string operation)
    {
        if (success)
        {
            EditorUtility.DisplayDialog("성공", $"{operation} 작업이 완료되었습니다.", "확인");
            AssetDatabase.Refresh();
            RefreshDataTracking();
        }
        else
        {
            EditorUtility.DisplayDialog("실패", $"{operation} 중 오류가 발생했습니다. 콘솔 확인.", "확인");
        }
    }
}
