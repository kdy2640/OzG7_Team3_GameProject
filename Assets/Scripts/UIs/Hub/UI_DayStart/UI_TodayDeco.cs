using DG.Tweening;
using UnityEngine;

public sealed class UI_TodayDeco : MonoBehaviour
{
    private const float ShowDuration = 0.25f;

    private readonly GameObject[] tasteDecos = new GameObject[(int)TasteType.Count];
    private readonly GameObject[] categoryDecos = new GameObject[(int)CategoryType.Count];
    private readonly Vector3[] tasteDecoScales = new Vector3[(int)TasteType.Count];
    private readonly Vector3[] categoryDecoScales = new Vector3[(int)CategoryType.Count];

    private Vector3 visibleScale;
    private Tween showTween;
    private bool isInitialized;

    public void Init()
    {
        if (isInitialized)
            return;

        tasteDecos[(int)TasteType.Salty] = transform.Find("Flavor_Salty").gameObject;
        tasteDecos[(int)TasteType.Clean] = transform.Find("Flavor_Dambaek").gameObject;
        tasteDecos[(int)TasteType.SpicyAndSour] = transform.Find("Flavor_SpicyAndSour").gameObject;

        categoryDecos[(int)CategoryType.WesternDine] = transform.Find("Theme_WesternDine").gameObject;
        categoryDecos[(int)CategoryType.AsianFood] = transform.Find("Theme_AsianFood").gameObject;
        categoryDecos[(int)CategoryType.StreetSnack] = transform.Find("Theme_StreetSnack").gameObject;

        for (int i = 0; i < tasteDecos.Length; i++)
            tasteDecoScales[i] = tasteDecos[i].transform.localScale;

        for (int i = 0; i < categoryDecos.Length; i++)
            categoryDecoScales[i] = categoryDecos[i].transform.localScale;

        visibleScale = transform.localScale;
        isInitialized = true;
        Hide();
    }

    public void Show()
    {
        MarketManager market = GameManager.Instance.Market;
        int currentBusinessDay = market.MarketData.CurrentBusinessDay;
        FestivalCalendar festivalCalendar = market.FestivalCalendar;
        TasteType currentTaste = festivalCalendar.GetNowTaste(currentBusinessDay);
        CategoryType currentCategory = festivalCalendar.GetNowCategory(currentBusinessDay);
        bool hasTasteFestival = currentTaste != TasteType.Count;
        bool hasCategoryFestival = currentCategory != CategoryType.Count;

        for (int i = 0; i < tasteDecos.Length; i++)
        {
            tasteDecos[i].transform.localScale = tasteDecoScales[i];
            tasteDecos[i].SetActive(hasTasteFestival && i == (int)currentTaste);
        }

        for (int i = 0; i < categoryDecos.Length; i++)
        {
            categoryDecos[i].transform.localScale = categoryDecoScales[i];
            categoryDecos[i].SetActive(hasCategoryFestival && i == (int)currentCategory);
        }

        if (!hasTasteFestival && !hasCategoryFestival)
        {
            Hide();
            return;
        }

        showTween?.Kill();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        showTween = transform.DOScale(visibleScale, ShowDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() => showTween = null);
    }

    public void ShowTasteFestival(TasteType tasteType)
    {
        int tasteIndex = (int)tasteType;
        ShowFestivalDeco(tasteDecos[tasteIndex], tasteDecoScales[tasteIndex]);
    }

    public void ShowCategoryFestival(CategoryType categoryType)
    {
        int categoryIndex = (int)categoryType;
        ShowFestivalDeco(categoryDecos[categoryIndex], categoryDecoScales[categoryIndex]);
    }

    public void Hide()
    {
        showTween?.Kill();
        showTween = null;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        showTween?.Kill();
        showTween = null;
    }

    private void ShowFestivalDeco(GameObject festivalDeco, Vector3 targetScale)
    {
        showTween?.Kill();
        transform.localScale = visibleScale;
        festivalDeco.transform.localScale = Vector3.zero;
        festivalDeco.SetActive(true);
        gameObject.SetActive(true);
        showTween = festivalDeco.transform.DOScale(targetScale, ShowDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() => showTween = null);
    }
}
