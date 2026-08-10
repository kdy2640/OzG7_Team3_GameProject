using UnityEngine;

public class FacilityController : MonoBehaviour
{
    [Header("Facility Info")]
    [SerializeField] private string facilityName;

    [SerializeField] private int currentLevel = 0;

    [SerializeField] private int maxLevel = 3;

    [SerializeField] private bool isPurchased = false;


    [Header("Level Models")]
    [SerializeField] private GameObject lockedModel;

    [SerializeField] private GameObject[] levelModels;


    [Header("Effects")]
    [SerializeField] private ParticleSystem upgradeEffect;


    [Header("Effect Text")]
    [SerializeField] private string[] levelEffects;


    public string FacilityName => facilityName;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public bool IsPurchased => isPurchased;


    private void Start()
    {
        RefreshModel();
    }


    public void Purchase()
    {
        if (isPurchased) return;

        isPurchased = true;
        currentLevel = 1;

        RefreshModel();
        PlayUpgradeEffect();
    }


    public bool CanUpgrade()
    {
        return isPurchased && currentLevel < maxLevel;
    }


    public void Upgrade()
    {
        if (!CanUpgrade()) return;

        currentLevel++;

        RefreshModel();
        PlayUpgradeEffect();
    }


    private void RefreshModel()
    {
        // 모든 레벨 모델 끄기
        if (lockedModel != null) lockedModel.SetActive(false);

        for (int i = 0; i < levelModels.Length; i++)
        {
            if (levelModels[i] != null) levelModels[i].SetActive(false);
        }


        // 구매 전
        if (!isPurchased)
        {
            if (lockedModel != null) lockedModel.SetActive(true);

            return;
        }


        // 구매 후
        int index = currentLevel - 1;

        if (index >= 0 && index < levelModels.Length)
        {
            if (levelModels[index] != null) levelModels[index].SetActive(true);
        }
    }


    private void PlayUpgradeEffect()
    {
        if (upgradeEffect == null) return;

        upgradeEffect.Play();
    }


    public string GetCurrentEffect()
    {
        if (!isPurchased) return "Not Purchased";

        int index = currentLevel - 1;

        if (index < 0 || index >= levelEffects.Length) return "";

        return levelEffects[index];
    }


    public string GetNextEffect()
    {
        if (!isPurchased) return levelEffects.Length > 0 ? levelEffects[0] : "";

        if (currentLevel >= maxLevel) return "Max Level";
        int index = currentLevel;

        if (index < 0 || index >= levelEffects.Length) return "";

        return levelEffects[index];
    }
}