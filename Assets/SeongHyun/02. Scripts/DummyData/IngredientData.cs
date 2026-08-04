using UnityEngine;


[CreateAssetMenu(
    fileName = "New Ingredient Data",
    menuName = "Restaurant/Ingredient Data")]
public class IngredientData : ScriptableObject
{

    [Header("Ingredient Info")]

    public string ingredientName;


    public Sprite icon;


    [TextArea(2, 4)]
    public string description;


    [Header("Value")]

    public int value = 10;

}