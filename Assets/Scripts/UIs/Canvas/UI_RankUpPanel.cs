using System.Collections;
using UnityEngine;

public sealed class UI_RankUpPanel : UI_Base
{
    private UI_RastaurantLevel rastaurantLevel;
    private UI_NewDishList newDishList;
    private UI_NewIngredientList newIngredientList;
    private UI_NewFunctionList newFunctionList;
    private UI_HubStateButton exitButton;

    [Header("Panel Animations")]
    [SerializeField] private PanelAnimator[] entranceAnimators;

    private enum GameObjects
    {
        UI_RastaurantLevel,
        UI_NewDishList,
        UI_NewIngredientList,
        UI_NewFunctionList
    }

    private enum HubStateButtons
    {
        ExitButton
    }

    protected override void OnInit()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_HubStateButton>(typeof(HubStateButtons));

        rastaurantLevel = GetGameObject((int)GameObjects.UI_RastaurantLevel)?
            .GetComponent<UI_RastaurantLevel>();
        newDishList = GetGameObject((int)GameObjects.UI_NewDishList)?
            .GetComponent<UI_NewDishList>();
        newIngredientList = GetGameObject((int)GameObjects.UI_NewIngredientList)?
            .GetComponent<UI_NewIngredientList>();
        newFunctionList = GetGameObject((int)GameObjects.UI_NewFunctionList)?
            .GetComponent<UI_NewFunctionList>();

        exitButton = GetUI<UI_HubStateButton>((int)HubStateButtons.ExitButton);
        exitButton?.Init(Owner);
    }

    protected override IEnumerator OnShow()
    {
        rastaurantLevel?.Refresh();
        newDishList?.Refresh();
        newIngredientList?.Refresh();
        newFunctionList?.Refresh();

        if (entranceAnimators != null)
        {
            for (int i = 0; i < entranceAnimators.Length; i++)
            {
                if (entranceAnimators[i] == null) continue;

                entranceAnimators[i].SetDelay(i * 0.06f);
                entranceAnimators[i].Show();
            }
        }


        yield break;
    }
}
