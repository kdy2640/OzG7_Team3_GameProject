using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainUIEditorBuilder
{
    [MenuItem("Tools/Create Main UI")]
    static void CreateMainUI()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        Transform old = canvas.transform.Find("UI_Main");

        if (old != null)
        {
            Object.DestroyImmediate(old.gameObject);
        }

        GameObject root = CreatePanel("UI_Main",
            canvas.transform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero
);
        // 배경
        CreatePanel(
            "Background",
            root.transform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero);

        // 왼쪽 메뉴
        CreatePanel(
            "LeftMenu",
            root.transform,
            new Vector2(0f, 0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(220, 520),
            new Vector2(140, 0));

        // 상단 가운데
        CreatePanel(
            "TopCenter",
            root.transform,
            new Vector2(0.5f, 1),
            new Vector2(0.5f, 1),
            new Vector2(350, 120),
            new Vector2(0, -70));

        // 우측 상단
        CreatePanel(
            "TopRight",
            root.transform,
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(300, 60),
            new Vector2(-170, -40));

        // 우측 정보창
        CreatePanel(
            "RightPanel",
            root.transform,
            new Vector2(1, 0.5f),
            new Vector2(1, 0.5f),
            new Vector2(300, 420),
            new Vector2(-180, 20));

        // 우측 하단 버튼
        CreatePanel(
            "BottomRight",
            root.transform,
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(340, 80),
            new Vector2(-180, 60));

        // 팝업
        CreatePanel(
            "PopupRoot",
            root.transform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero);

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("Main UI 생성 완료");
    }

    static GameObject CreatePanel(
    string name,
    Transform parent,
    Vector2 anchorMin,
    Vector2 anchorMax,
    Vector2 size,
    Vector2 position)
    {
        GameObject obj = new GameObject(name);

        Undo.RegisterCreatedObjectUndo(obj, "Create UI Object");

        obj.transform.SetParent(parent, false);

        RectTransform rt = obj.AddComponent<RectTransform>();

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.sizeDelta = size;
        rt.anchoredPosition = position;

        Image image = obj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.1f);

        return obj;
    }
}