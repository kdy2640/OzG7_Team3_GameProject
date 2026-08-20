using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DailySalesPanel : MonoBehaviour
{
    [Header("Today's Performance")]
    [SerializeField] private TMP_Text todaySalesText;
    [SerializeField] private TMP_Text customerCountText;

    [Header("Sales Difference")]
    [SerializeField] private TMP_Text salesDifferenceText;
    [SerializeField] private GameObject increaseIcon;
    [SerializeField] private GameObject decreaseIcon;

    [Header("Menu Sales")]
    [SerializeField] private UI_MenuSalesRow[] menuSalesRows = new UI_MenuSalesRow[3];

    [Header("Tip")]
    [SerializeField] private TMP_Text tipSalesText;

    [Header("Exit")]
    [SerializeField] private Button exitButton;

    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

    private SalesResultData data;

    public bool IsExitRequested { get; private set; }

    private void Awake()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("[UI_DailySalesPanel] CanvasGroup이 연결되지 않았습니다.", this);
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public void SetData(SalesResultData data)
    {
        if (data == null)
            return;

        this.data = data;
        Refresh();
    }

    public void Refresh()
    {
        if (data == null)
            return;

        if (todaySalesText != null)
            todaySalesText.text = $"{data.todaySales:N0}코인";

        if (customerCountText != null)
        {
            customerCountText.text =
                $"{data.customerReceived:N0} / {data.customerMax:N0}명";
        }

        int difference = data.todaySales - data.yesterdaySales;

        if (salesDifferenceText != null)
        {
            salesDifferenceText.text =
                difference > 0 ? $"+{difference:N0}코인" : $"{difference:N0}코인";
        }

        if (increaseIcon != null)
            increaseIcon.SetActive(difference > 0);

        if (decreaseIcon != null)
            decreaseIcon.SetActive(difference < 0);

        RefreshMenuSales();

        if (tipSalesText != null)
            tipSalesText.text = $"{data.tipSales:N0}코인";
    }

    public IEnumerator Show()
    {
        if (canvasGroup == null)
            yield break;

        IsExitRequested = false;
        gameObject.SetActive(true);
        Refresh();

        if (exitButton != null)
            exitButton.interactable = true;

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return canvasGroup
            .DOFade(1f, fadeDuration)
            .WaitForCompletion();
    }

    public IEnumerator Hide()
    {
        if (canvasGroup == null || !gameObject.activeSelf)
            yield break;

        if (exitButton != null)
            exitButton.interactable = false;

        canvasGroup.DOKill();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return canvasGroup
            .DOFade(0f, fadeDuration)
            .WaitForCompletion();

        gameObject.SetActive(false);
    }

    public void RequestExit()
    {
        IsExitRequested = true;

        if (exitButton != null)
            exitButton.interactable = false;
    }

    private void RefreshMenuSales()
    {
        for (int i = 0; i < menuSalesRows.Length; i++)
        {
            UI_MenuSalesRow row = menuSalesRows[i];

            if (row == null)
                continue;

            if (data.menuSales != null && i < data.menuSales.Count)
            {
                row.SetData(data.menuSales[i]);
            }
            else
            {
                row.Clear();
            }
        }
    }
}
