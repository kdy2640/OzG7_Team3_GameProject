using UnityEngine;

public class FacilityModelView : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private FacilityController facility;

    [Header("Model")]
    [Tooltip("런타임에 현재 레벨 모델 프리팹이 생성될 빈 오브젝트 컨테이너")]
    [SerializeField] private Transform facilityModelRoot;

    private GameObject currentModelInstance;
    private int shownLevel = -1;

    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (facility != null)
            ShowLevel(facility.CurrentLevel);
    }

    public void ShowLocked()
    {
        ShowLevel(0);
    }

    // Lv.0 = 미구매 모델, Lv.1 이상 = 해당 레벨 모델
    public void ShowLevel(int level)
    {
        if (facility == null)
        {
            Debug.LogWarning($"{name}: FacilityController가 할당되지 않았습니다.",this);
            return;
        }
        if(facilityModelRoot == null)
        {
            Debug.LogWarning
                ($"[FacilityModelView] FacilityModelRoot가 없습니다: {name}",this);
            return;
        }
        FacilityDataSO facilityData =
            FacilityDataDB.GetData(facility.FacilityType);

        if (facilityData == null)
        {
            Debug.LogWarning
                ($"[FacilityModelView] FacilityData가 없습니다: {facility.FacilityType}",
                this);
            return;
        }
        GameObject modelPrefab = facilityData.GetSolidPrefabForLevel(level);

        if (modelPrefab == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] " +
                $"{facilityData.DisplayName} Lv.{level} 모델이 없습니다.");
            return;
        }
        if (shownLevel == level && currentModelInstance != null) return;

        ClearCurrentModel();

        currentModelInstance = Instantiate(modelPrefab, facilityModelRoot);
        currentModelInstance.transform.localPosition = Vector3.zero;
        currentModelInstance.transform.localRotation = Quaternion.identity;
        currentModelInstance.transform.localScale = Vector3.one;

        shownLevel = level;

        Debug.Log(
            $"[FacilityModelView] {facility.FacilityType} " +
            $"→ Lv.{level} 모델 생성: {modelPrefab.name}",
            this);
    }

    public void PlayUpgradeEffect()
    {
        // 나중에 파티클/애니메이션을 추가할 위치입니다.
        if (facility != null) ShowLevel(facility.CurrentLevel);
    }

    private void ClearCurrentModel()
    {
        if (currentModelInstance != null) Destroy(currentModelInstance);

        currentModelInstance = null;
        shownLevel = -1;
    }
}