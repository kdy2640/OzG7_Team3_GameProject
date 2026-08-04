using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RestaurantMenuUI : MonoBehaviour
{

    [Header("UI Reference")]

    [SerializeField]
    private Transform menuCardContainer;


    [SerializeField]
    private Transform menuDetailPanel;



    [Header("Test Menu Data")]

    [SerializeField]
    private List<RecipeData> recipes =
        new List<RecipeData>();



    private RecipeDetailUI detailUI;



    private void Awake()
    {

        if (menuDetailPanel != null)
        {

            detailUI =
                menuDetailPanel
                .GetComponent<RecipeDetailUI>();

        }

    }




    private void Start()
    {

        CreateTestMenuCards();

    }





    public void CreateTestMenuCards()
    {

        ClearCards();



        foreach (RecipeData recipe in recipes)
        {

            if (recipe == null)
                continue;



            CreateCard(recipe);

        }

    }





    private void CreateCard(
        RecipeData data)
    {

        GameObject card =
            new GameObject(
                "MenuCard_" + data.recipeName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));



        card.transform.SetParent(
            menuCardContainer,
            false);



        RectTransform rt =
            card.GetComponent<RectTransform>();


        rt.sizeDelta =
            new Vector2(
                450,
                100);



        Image image =
            card.GetComponent<Image>();


        image.color =
            new Color(
                0.25f,
                0.2f,
                0.15f,
                1);



        Button button =
            card.GetComponent<Button>();


        button.onClick.AddListener(
            () =>
            {
                SelectMenu(data);
            });



        CreateCardText(
            data.recipeName,
            card.transform,
            28,
            new Vector2(0, 20));



        CreateCardText(
            "Lv."
            + data.level
            +
            "   "
            +
            data.price
            +
            "G",
            card.transform,
            20,
            new Vector2(0, -20));


    }





    private void CreateCardText(
        string text,
        Transform parent,
        int size,
        Vector2 position)
    {

        GameObject obj =
            new GameObject(
                "TMP_" + text,
                typeof(RectTransform),
                typeof(TMPro.TextMeshProUGUI));


        obj.transform.SetParent(
            parent,
            false);



        RectTransform rt =
            obj.GetComponent<RectTransform>();


        rt.sizeDelta =
            new Vector2(
                400,
                40);



        rt.anchoredPosition =
            position;



        TMPro.TMP_Text tmp =
            obj.GetComponent<TMPro.TMP_Text>();


        tmp.text =
            text;


        tmp.fontSize =
            size;


        tmp.alignment =
            TMPro.TextAlignmentOptions.Center;

    }





    private void SelectMenu(
        RecipeData recipe)
    {

        if (detailUI == null)
            return;


        detailUI.Show(recipe);

    }





    private void ClearCards()
    {

        if (menuCardContainer == null)
            return;



        for (int i =
            menuCardContainer.childCount - 1;
            i >= 0;
            i--)
        {

            Destroy(
                menuCardContainer
                .GetChild(i)
                .gameObject);

        }

    }

}