using TMPro;
using UnityEngine;

public sealed class UI_NewFunctionList : MonoBehaviour
{
    private const int SlotCount = 5;

    private readonly GameObject[] slots = new GameObject[SlotCount];
    private readonly TextMeshProUGUI[] descriptionTexts =
        new TextMeshProUGUI[SlotCount];

    private bool areSlotsCached;

    public void Refresh()
    {
        CacheSlots();
        ClearSlots();

        MarketManager marketManager = GameManager.Instance?.Market;

        if (marketManager == null)
        {
            Debug.LogError($"[{nameof(UI_NewFunctionList)}] MarketManager를 찾을 수 없습니다.", this);
            return;
        }

        int currentLevel = marketManager.MarketData.CurrentLevel;
        int currentCustomerCount = marketManager.LevelData.BaseCustomerCount;
        int previousCustomerCount = 0;

        if (currentLevel > 0
            && LevelDataDB.TryGetData(currentLevel - 1, out LevelData previousLevelData)
            && previousLevelData != null)
        {
            previousCustomerCount = previousLevelData.BaseCustomerCount;
        }

        int increasedCustomerCount =
            Mathf.Max(0, currentCustomerCount - previousCustomerCount);

        int slotIndex = 0;

        if (increasedCustomerCount > 0 && slotIndex < SlotCount)
        {
            slots[slotIndex]?.SetActive(true);

            if (descriptionTexts[slotIndex] != null)
            {
                descriptionTexts[slotIndex].text =
                    $"- 기본 손님 수 + {increasedCustomerCount}";
            }

            slotIndex++;
        }

        int newFacilityCount = 0;

        for (int i = 0; i < (int)FacilityType.Count; i++)
        {
            FacilityUpgradeDataSO facilityData =
                UpgradeDataDB.GetData((FacilityType)i);

            if (facilityData != null
                && facilityData.TryGetRequiredMarketLevel(
                    1,
                    out int requiredMarketLevel)
                && requiredMarketLevel == currentLevel)
            {
                newFacilityCount++;
            }
        }

        if (newFacilityCount > 0 && slotIndex < SlotCount)
        {
            slots[slotIndex]?.SetActive(true);

            if (descriptionTexts[slotIndex] != null)
            {
                descriptionTexts[slotIndex].text =
                    $"- 구매 가능한 신규 시설 + {newFacilityCount}";
            }

            slotIndex++;
        }

        int newEmployeeCount = 0;

        for (int i = 0; i < (int)EmployeeType.Count; i++)
        {
            EmployeeUpgradeDataSO employeeData =
                UpgradeDataDB.GetData((EmployeeType)i);

            if (employeeData != null
                && employeeData.TryGetRequiredMarketLevel(
                    1,
                    out int requiredMarketLevel)
                && requiredMarketLevel == currentLevel)
            {
                newEmployeeCount++;
            }
        }

        if (newEmployeeCount > 0 && slotIndex < SlotCount)
        {
            slots[slotIndex]?.SetActive(true);

            if (descriptionTexts[slotIndex] != null)
            {
                descriptionTexts[slotIndex].text =
                    $"- 구매 가능한 직원 + {newEmployeeCount}";
            }

            slotIndex++;
        }

        HarvestUpgradeDataSO stageLevelData =
            UpgradeDataDB.GetData(HarvestUpgradeType.StageLevel);
        int unlockedStage = 0;

        if (stageLevelData != null)
        {
            int stageCount = Mathf.Min(
                stageLevelData.MaxLevel,
                stageLevelData.RequiredMarketLevel.Count);

            for (int i = 0; i < stageCount; i++)
            {
                if (stageLevelData.RequiredMarketLevel[i] != currentLevel)
                    continue;

                unlockedStage = i + 1;
                break;
            }
        }

        if (unlockedStage > 0 && slotIndex < SlotCount)
        {
            slots[slotIndex]?.SetActive(true);

            if (descriptionTexts[slotIndex] != null)
            {
                descriptionTexts[slotIndex].text =
                    $"- 신규 농장 {unlockedStage}스테이지 개방";
            }
        }
    }

    private void CacheSlots()
    {
        if (areSlotsCached)
            return;

        areSlotsCached = true;

        for (int i = 0; i < SlotCount; i++)
        {
            Transform slot = transform.Find($"NewFuctionSlot{i + 1:00}");
            slots[i] = slot?.gameObject;
            descriptionTexts[i] = slot?
                .Find("FunctionDescriptionText")?
                .GetComponent<TextMeshProUGUI>();
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            slots[i]?.SetActive(false);

            if (descriptionTexts[i] != null)
                descriptionTexts[i].text = string.Empty;
        }
    }
}
