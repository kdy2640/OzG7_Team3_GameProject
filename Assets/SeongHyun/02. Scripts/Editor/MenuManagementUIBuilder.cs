using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;


public class MenuManagementUIBuilder
{

    static Transform root;


    [MenuItem("Tools/Restaurant/Create Menu Management UI")]
    public static void Build()
    {

        Canvas canvas =
            Object.FindFirstObjectByType<Canvas>();


        if (canvas == null)
        {
            Debug.LogError("Canvas not found");
            return;
        }



        Transform old =
            canvas.transform.Find(
                "AUTO_MenuManagementUI");


        if (old != null)
        {
            Object.DestroyImmediate(
                old.gameObject);
        }



        GameObject rootObj =
            new GameObject(
                "AUTO_MenuManagementUI",
                typeof(RectTransform));


        root = rootObj.transform;


        root.SetParent(
            canvas.transform,
            false);



        CreateMainPanel();



        Debug.Log(
            "Menu Management UI Created");

    }



    static void CreateMainPanel()
    {

        GameObject panel =
            CreatePanel(
                "UI_MenuManagement",
                root,
                new Color(
                    0.12f,
                    0.09f,
                    0.06f,
                    0.95f));



        SetRect(
            panel,
            1500,
            850,
            new Vector2(.5f, .5f),
            Vector2.zero);



        CreateHeader(panel.transform);


        CreateMenuList(panel.transform);


        CreateDetail(panel.transform);


        CreateCloseButton(panel.transform);

    }



    static void CreateHeader(
        Transform parent)
    {

        GameObject header =
            CreatePanel(
                "Header",
                parent,
                new Color(
                    .25f,
                    .18f,
                    .1f,
                    1));



        SetRect(
            header,
            1500,
            100,
            new Vector2(.5f, 1),
            new Vector2(0, -50));



        CreateText(
            "MENU MANAGEMENT",
            header.transform,
            36,
            Vector2.zero);

    }




    static void CreateMenuList(
        Transform parent)
    {


        GameObject list =
            CreatePanel(
                "PANEL_MenuList",
                parent,
                new Color(
                    .18f,
                    .14f,
                    .1f,
                    1));



        SetRect(
            list,
            550,
            650,
            new Vector2(0, 0.5f),
            new Vector2(320, -20));



        CreateText(
            "MENU LIST",
            list.transform,
            28,
            new Vector2(0, 250));



        GameObject container =
            new GameObject(
                "MenuCardContainer",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup));



        container.transform.SetParent(
            list.transform,
            false);



        RectTransform rt =
            container.GetComponent
            <RectTransform>();


        rt.sizeDelta =
            new Vector2(
                480,
                500);


        rt.anchoredPosition =
            new Vector2(
                0,
                -50);



        VerticalLayoutGroup layout =
            container.GetComponent
            <VerticalLayoutGroup>();


        layout.spacing = 15;


        layout.childAlignment =
            TextAnchor.UpperCenter;

    }





    static void CreateDetail(
        Transform parent)
    {


        GameObject detail =
            CreatePanel(
                "PANEL_MenuDetail",
                parent,
                new Color(
                    .2f,
                    .17f,
                    .13f,
                    1));



        SetRect(
            detail,
            750,
            650,
            new Vector2(1, .5f),
            new Vector2(-400, -20));



        CreateText(
            "SELECTED MENU",
            detail.transform,
            28,
            new Vector2(0, 250));



        CreateImage(
            "IMG_Food",
            detail.transform);



        CreateText(
            "MENU NAME",
            detail.transform,
            30,
            new Vector2(100, 80));



        CreateText(
            "LEVEL",
            detail.transform,
            22,
            new Vector2(100, 30));



        CreateText(
            "INGREDIENTS",
            detail.transform,
            22,
            new Vector2(100, -40));



        CreateText(
            "PRICE",
            detail.transform,
            22,
            new Vector2(100, -100));



        CreateText(
            "DESCRIPTION",
            detail.transform,
            18,
            new Vector2(100, -170));



        CreateButton(
            "REGISTER",
            detail.transform,
            new Vector2(150, -260));

    }





    static void CreateCloseButton(
        Transform parent)
    {

        CreateButton(
            "CLOSE",
            parent,
            new Vector2(650, -380));

    }





    static GameObject CreatePanel(
        string name,
        Transform parent,
        Color color)
    {

        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform),
                typeof(Image));


        obj.transform.SetParent(
            parent,
            false);



        obj.GetComponent<Image>()
            .color = color;



        return obj;

    }





    static void CreateImage(
        string name,
        Transform parent)
    {


        GameObject obj =
            CreatePanel(
                name,
                parent,
                new Color(
                    .35f,
                    .3f,
                    .25f));


        SetRect(
            obj,
            220,
            220,
            new Vector2(0, 1),
            new Vector2(120, -170));

    }





    static void CreateText(
        string text,
        Transform parent,
        int size,
        Vector2 pos)
    {


        GameObject obj =
            new GameObject(
                "TMP_" + text,
                typeof(RectTransform),
                typeof(TextMeshProUGUI));



        obj.transform.SetParent(
            parent,
            false);



        RectTransform rt =
            obj.GetComponent
            <RectTransform>();


        rt.sizeDelta =
            new Vector2(
                400,
                50);



        rt.anchoredPosition =
            pos;



        TMP_Text tmp =
            obj.GetComponent
            <TMP_Text>();


        tmp.text = text;


        tmp.fontSize = size;


        tmp.alignment =
            TextAlignmentOptions.Center;


    }





    static void CreateButton(
        string text,
        Transform parent,
        Vector2 pos)
    {


        GameObject obj =
            CreatePanel(
                "BTN_" + text,
                parent,
                new Color(
                    .5f,
                    .3f,
                    .1f));



        SetRect(
            obj,
            220,
            70,
            new Vector2(.5f, .5f),
            pos);



        CreateText(
            text,
            obj.transform,
            24,
            Vector2.zero);

    }





    static void SetRect(
        GameObject obj,
        float width,
        float height,
        Vector2 anchor,
        Vector2 pos)
    {

        RectTransform rt =
            obj.GetComponent
            <RectTransform>();


        rt.sizeDelta =
            new Vector2(
                width,
                height);


        rt.anchorMin =
            anchor;


        rt.anchorMax =
            anchor;


        rt.anchoredPosition =
            pos;

    }

}