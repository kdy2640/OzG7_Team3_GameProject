using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityWorldUI : MonoBehaviour
{
    [Header("Purchase State")]
    [SerializeField] private GameObject purchaseStatusRoot;

    [Header("Purchased State")]
    [SerializeField] private GameObject purchasedStatusRoot;

    [Header("Level")]
    [SerializeField] private TMP_Text levelText;

    [Header("Upgrade Icon")]
    [SerializeField] private Image upgradeIcon;

    [Header("Upgrade Sprites")]
    [SerializeField] private Sprite upgradeAvailableSprite;
    [SerializeField] private Sprite upgradeUnavailableSprite;

    public void Refresh(bool isPurchased, int currentLevel, bool canUpgrade)
    {
        if (!isPurchased)
        {
            ShowNotPurchased();
            return;
        }
        ShowPurchased(currentLevel, canUpgrade);
    }
    public void ShowNotPurchased()
    {
        if (purchaseStatusRoot != null) purchaseStatusRoot.SetActive(true);

        if (purchasedStatusRoot != null) purchasedStatusRoot.SetActive(false);
    }


    public void ShowPurchased(int currentLevel, bool canUpgrade)
    {
        if (purchaseStatusRoot != null) purchaseStatusRoot.SetActive(false);

        if (purchasedStatusRoot != null) purchasedStatusRoot.SetActive(true);

        if (levelText != null) levelText.text = $"Lv.{currentLevel}";

        if (upgradeIcon != null)
        {
            upgradeIcon.sprite = canUpgrade
                ? upgradeAvailableSprite : upgradeUnavailableSprite;
        }
    }
}