using UnityEngine;

public class FacilityModelView : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private GameObject lockedModel;
    [SerializeField] private GameObject[] levelModels;

    [Header("Upgrade Effect")]
    [SerializeField] private ParticleSystem upgradeEffect;


    public void ShowLocked()
    {
        HideAll();

        if (lockedModel != null) lockedModel.SetActive(true);
    }


    public void ShowLevel(int level)
    {
        HideAll();

        int index = level - 1;

        if (index < 0 || index >= levelModels.Length) return;

        if (levelModels[index] != null) levelModels[index].SetActive(true);
    }
    public void PlayUpgradeEffect()
    {
        if (upgradeEffect != null) upgradeEffect.Play();
    }

    private void HideAll()
    {
        if (lockedModel != null) lockedModel.SetActive(false);

        if (levelModels == null) return;

        foreach (GameObject model in levelModels)
        {
            if (model != null) model.SetActive(false);
        }
    }


    
}