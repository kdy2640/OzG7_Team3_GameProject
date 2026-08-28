using UnityEngine;

public class FacilityModelView : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private FacilityController facility;

    [Header("Model")]
    [Tooltip("현재 레벨 모델이 생성될 위치")]
    [SerializeField] private Transform facilityModelRoot;

    private GameObject currentModelInstance;
    private int shownLevel = -1;

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (facility == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityController가 연결되지 않았습니다: {name}",
                this );
            return;
        }

        ShowLevel(facility.CurrentLevel);
    }

    public void ShowLocked()
    {
        ShowLevel(0);
    }

    public void ShowLevel(int level)
    {
        if (facility == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityController가 연결되지 않았습니다: {name}",
                this );
            return;
        }

        if (facilityModelRoot == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityModelRoot가 연결되지 않았습니다: {name}",
                this );
            return;
        }

        FacilityDataSO facilityData = FacilityDataDB.GetData(facility.FacilityType);

        if (facilityData == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] FacilityData가 없습니다: {facility.FacilityType}",
                this );
            return;
        }

        // 같은 레벨의 모델이 이미 존재하면 다시 생성하지 않음
        if (shownLevel == level && currentModelInstance != null)
        {
            return;
        }

        GameObject modelPrefab = facilityData.GetSolidPrefabForLevel(level);

        if (modelPrefab == null)
        {
            Debug.LogWarning(
                $"[FacilityModelView] " +
                $"{facilityData.DisplayName}의 Lv.{level} 모델이 없습니다.",
                this
            );
            return;
        }

        ClearCurrentModel();

        

        currentModelInstance =
            Instantiate( modelPrefab, facilityModelRoot, false );

        currentModelInstance.transform.localPosition = Vector3.zero;

        currentModelInstance.transform.localRotation = Quaternion.identity;

        currentModelInstance.transform.localScale = Vector3.one;

        shownLevel = level;

        // 서비스씬 0렙 제외
        if (GameManager.Instance.Scene.CurrentSceneType == SceneType.Service)
        {
            if(level <= 0)
            {
                ClearCurrentModel();
            }
        }

        Debug.Log(
            $"[FacilityModelView] " +
            $"{facility.FacilityType} → Lv.{level} 모델 생성",
            this
        );
    }

    public void PlayUpgradeEffect()
    {
        if (facility != null)
        {
            ShowLevel(facility.CurrentLevel);
        }
    }

    private void ClearCurrentModel()
    {
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        currentModelInstance = null;
        shownLevel = -1;
    }
}