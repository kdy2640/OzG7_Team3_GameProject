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
        if (facility == null) return;

        facility.StateChanged += OnFacilityStateChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (facility != null)
            facility.StateChanged -= OnFacilityStateChanged;
    }

    private void OnFacilityStateChanged(FacilityController changedFacility)
    {
        if (changedFacility == facility)
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
        if (facility == null || facilityModelRoot == null) return;

        FacilityDataSO facilityData =
            FacilityDataDB.GetData(facility.FacilityType);

        if (facilityData == null) return;

        if (shownLevel == level && currentModelInstance != null) return;

        GameObject modelPrefab =
            facilityData.GetSolidPrefabForLevel(level);

        ClearCurrentModel();

        if (modelPrefab == null)
        {
            Debug.LogWarning
                ($"{name}: {facilityData.DisplayName}의 Lv.{level} 모델이 없습니다.");
            return;
        }

        currentModelInstance = Instantiate(modelPrefab, facilityModelRoot);
        currentModelInstance.transform.localPosition = Vector3.zero;
        currentModelInstance.transform.localRotation = Quaternion.identity;
        currentModelInstance.transform.localScale = Vector3.one;

        shownLevel = level;
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