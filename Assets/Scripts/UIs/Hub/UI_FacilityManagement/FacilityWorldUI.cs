using TMPro;
using UnityEngine;

public class FacilityWorldUI : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Purchase State")]
    [SerializeField] private GameObject purchaseStatusRoot;
    [SerializeField] private GameObject purchaseIcon;

    [Header("Purchased State")]
    [SerializeField] private GameObject purchasedStatusRoot;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject upgradeArrow;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }
    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        // 월드 UI가 항상 카메라 정면을 향하도록 처리
        transform.rotation = targetCamera.transform.rotation;
    }

    public void Refresh(bool isPurchased, int currentLevel, bool canUpgrade)
    {
        if (purchaseStatusRoot != null)
            purchaseStatusRoot.SetActive(!isPurchased);

        if (purchaseIcon != null)
            purchaseIcon.SetActive(!isPurchased);

        if (purchasedStatusRoot != null)
            purchasedStatusRoot.SetActive(isPurchased);

        if (!isPurchased)
            return;

        if (levelText != null)
            levelText.text = $"Lv.{currentLevel}";

        if (upgradeArrow != null)
            upgradeArrow.SetActive(canUpgrade);
    }
}