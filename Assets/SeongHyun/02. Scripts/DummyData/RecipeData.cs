using UnityEngine;


[CreateAssetMenu(
    fileName = "New Recipe Data",
    menuName = "Restaurant/Recipe Data")]
public class RecipeData : ScriptableObject
{

    [Header("Basic Info")]

    public string recipeName;


    public Sprite icon;


    [Header("Level / Economy")]

    public int level = 1;


    public int price = 100;



    [Header("Ingredients")]

    public IngredientData ingredientA;


    public IngredientData ingredientB;



    [Header("Description")]

    [TextArea(3, 5)]
    public string description;



    [Header("State")]

    public bool isUnlocked = true;


}