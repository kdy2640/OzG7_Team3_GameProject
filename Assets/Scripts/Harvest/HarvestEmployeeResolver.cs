using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class HarvestEmployeeResolver : MonoBehaviour
{
    #region Fields

    private const float CutterRangeMultiplier = 1.5f;
    private const float CutterRangeBuffDuration = 5f;
    private const float ChargeDuration = 1.5f;
    private const float ChargeDamageMultiplier = 100f;

    [SerializeField] private GameObject harvester1Sidecar;
    [SerializeField] private GameObject harvester2Sidecar;

    private readonly SkillBase[] skills = new SkillBase[2];
    private CropCutter harvester1Cutter;
    private CropCutter harvester2Cutter;
    private float harvester1OriginalRange;
    private float harvester2OriginalRange;
    private TractorController tractorController;
    private CropCutter tractorCutter;
    private Coroutine chargeCoroutine;

    #endregion

    #region Skill Lifecycle

    private void Awake()
    {
        tractorController = GetComponent<TractorController>();
        tractorCutter = tractorController?.Cutter;

        skills[0] = new SkillBase(5f);
        skills[1] = new SkillBase(8f);

        skills[0].SetExecute(ExecuteCutterRangeBuff);
        skills[1].SetExecute(ExecuteCharge);
    }

    private void Start()
    { 

        harvester1Cutter = ResolveSidecar(
            EmployeeType.Harvester_1,
            harvester1Sidecar);
        harvester2Cutter = ResolveSidecar(
            EmployeeType.Harvester_2,
            harvester2Sidecar);

        if (harvester1Cutter != null)
        {
            harvester1OriginalRange = harvester1Cutter.Range;
        }

        if (harvester2Cutter != null)
        {
            harvester2OriginalRange = harvester2Cutter.Range;
        }

    }

    private void Update()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Tick();
        }
    }

    public SkillBase GetSkill(int index)
    {
        if (index < 0 || index >= skills.Length)
        {
            return null;
        }

        return skills[index];
    }

    #endregion

    #region Cutter Range Skill

    private void ExecuteCutterRangeBuff()
    {
        StartCoroutine(CutterRangeBuff());
    }

    private IEnumerator CutterRangeBuff()
    {
        SetCutterTargetRanges(CutterRangeMultiplier);

        yield return new WaitForSeconds(CutterRangeBuffDuration);

        SetCutterTargetRanges(1f);
    }

    private void SetCutterTargetRanges(float multiplier)
    {
        if (harvester1Cutter != null
            && harvester1Cutter.gameObject.activeInHierarchy)
        {
            harvester1Cutter.SetTargetRange(
                harvester1OriginalRange * multiplier);
        }

        if (harvester2Cutter != null
            && harvester2Cutter.gameObject.activeInHierarchy)
        {
            harvester2Cutter.SetTargetRange(
                harvester2OriginalRange * multiplier);
        }
    }

    #endregion

    #region Charge Skill

    private void ExecuteCharge()
    {
        if (tractorController == null || tractorCutter == null)
        {
            return;
        }

        chargeCoroutine = StartCoroutine(Charge());
    }

    private IEnumerator Charge()
    {
        tractorController.SetCharging(true);
        tractorCutter.SetDamageMultiplier(ChargeDamageMultiplier);
        harvester1Cutter?.SetDamageMultiplier(ChargeDamageMultiplier);
        harvester2Cutter?.SetDamageMultiplier(ChargeDamageMultiplier);

        yield return new WaitForSeconds(ChargeDuration);

        RestoreChargeState();
        chargeCoroutine = null;
    }

    private void OnDisable()
    {
        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
        }

        RestoreChargeState();
    }

    private void RestoreChargeState()
    {
        tractorController?.SetCharging(false);
        tractorCutter?.SetDamageMultiplier(1f);
        harvester1Cutter?.SetDamageMultiplier(1f);
        harvester2Cutter?.SetDamageMultiplier(1f);
    }

    #endregion

    #region Sidecar

    private CropCutter ResolveSidecar(
        EmployeeType employeeType,
        GameObject sidecar)
    { 

        int level = GameManager.Instance.Upgrade.RuntimeLevel.Get(employeeType);
        bool isUnlocked = level > 1;
        sidecar.SetActive(isUnlocked);

        if (!isUnlocked)
        {
            return null;
        }

        CropCutter cutter = sidecar.GetComponentInChildren<CropCutter>(true);
         

        cutter.Initialize(tractorController.GridChunkHandler);
        tractorController.GridChunkHandler.Streamer.AddLoadingTarget(
            sidecar.transform);

        return cutter;
    }

    #endregion
}
