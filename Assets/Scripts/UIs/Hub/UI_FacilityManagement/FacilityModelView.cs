using System.Collections.Generic;
using UnityEngine;

public class FacilityModelView : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private FacilityController facility;
    [SerializeField] private FacilityDataSO facilityData;

    [Header("Model")]
    [SerializeField] private Transform facilityModelRoot;

    private readonly List<GameObject> registeredModels = new();

    private void Awake()
    {
        RegisterModelsFromRoot();
    }

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

    private void RegisterModelsFromRoot()
    {
        registeredModels.Clear();

        if (facilityModelRoot == null)
        {
            Debug.LogError($"{name}: Facility Model Root가 지정되지 않았습니다.");
            return;
        }

        if (facilityData != null &&
            facilityData.SolidPrefabs.Count != facilityModelRoot.childCount)
        {
            Debug.LogWarning(
                $"{name}: SolidPrefabs 수({facilityData.SolidPrefabs.Count})와 " +
                $"Model Root 자식 수({facilityModelRoot.childCount})가 다릅니다.");
        }

        // Model Root의 직계 자식만 자동 등록.
        // 자식 순서는 SolidPrefabs의 순서와 동일해야 합니다.
        for (int i = 0; i < facilityModelRoot.childCount; i++)
        {
            registeredModels.Add(facilityModelRoot.GetChild(i).gameObject);
        }
    }

    private void OnFacilityStateChanged(FacilityController changedFacility)
    {
        if (changedFacility == facility)
            Refresh();
    }

    private void Refresh()
    {
        if (facility == null) return;

        ShowLevel(facility.CurrentLevel);
    }
    public void ShowLocked()
    {
        ShowLevel(0);
    }

    public void ShowLevel(int level)
    {
        if (registeredModels.Count == 0) return;

        int modelIndex = Mathf.Clamp( level, 0, registeredModels.Count - 1);

        for (int i = 0; i < registeredModels.Count; i++)
        {
            registeredModels[i].SetActive(i == modelIndex);
        }
    }

    public void PlayUpgradeEffect()
    {
        // 기존에 파티클/애니메이션 효과가 있었다면
        // 그 코드를 이 메서드 안에 복원하면 됩니다.
        // 모델 표시는 Controller가 호출하는 ShowLevel()에서 처리됩니다.

        if (facility != null) ShowLevel(facility.CurrentLevel);
    }
}