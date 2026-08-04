using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RecipeDetailUI : MonoBehaviour
{

    [Header("UI References")]

    [SerializeField]
    private Image foodIcon;


    [SerializeField]
    private TMP_Text nameText;


    [SerializeField]
    private TMP_Text levelText;


    [SerializeField]
    private TMP_Text ingredientText;


    [SerializeField]
    private TMP_Text priceText;


    [SerializeField]
    private TMP_Text descriptionText;



    private RecipeData currentRecipe;




    public void Show(
        RecipeData data)
    {

        if (data == null)
            return;



        currentRecipe = data;



        if (foodIcon != null)
        {

            foodIcon.sprite =
                data.icon;

        }



        if (nameText != null)
        {

            nameText.text =
                data.recipeName;

        }



        if (levelText != null)
        {

            levelText.text =
                "LEVEL " +
                data.level;

        }



        if (ingredientText != null)
        {

            string ingredientA =
                data.ingredientA != null ?
                data.ingredientA.ingredientName :
                "None";


            string ingredientB =
                data.ingredientB != null ?
                data.ingredientB.ingredientName :
                "None";



            ingredientText.text =
                ingredientA +
                " + " +
                ingredientB;

        }



        if (priceText != null)
        {

            priceText.text =
                "PRICE : " +
                data.price +
                " G";

        }



        if (descriptionText != null)
        {

            descriptionText.text =
                data.description;

        }

    }



    public RecipeData GetCurrentRecipe()
    {

        return currentRecipe;

    }


}