using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public sealed class UI_HarvestSkillButton : MonoBehaviour
{
    [SerializeField, Min(0)] private int skillIndex;
    [SerializeField] private HarvestEmployeeResolver resolver;
    [SerializeField] private Button button;
    [SerializeField] private Image coolDownOverlay;
    [SerializeField] private TMP_Text coolDownText;

    private SkillBase skill;
    private bool wasCoolingDown;

    private void Start()
    {
        if (resolver == null)
        {
            Debug.LogError(
                "[UI_HarvestSkillButton] Resolver is not assigned.",
                this);
            return;
        }

        skill = resolver.GetSkill(skillIndex);

        if (skill == null)
        {
            Debug.LogError(
                $"[UI_HarvestSkillButton] Skill was not found. "
                + $"index: {skillIndex}",
                this);
            return;
        }

        if (!skill.IsUnlocked)
        {
            gameObject.SetActive(false);
            return;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(OnClick);
        skill.OnTick += SetCoolDown;
        SetCoolDown(0f);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }

        if (skill != null)
        {
            skill.OnTick -= SetCoolDown;
        }
    }

    private void OnClick()
    {
        if (!skill.CanExecute())
            return;

        skill.Execute();
        GameManager.Instance.Utility.Audio.PlaySFX(
            SFXType.Harvest_SkillActivate);
    }

    private void SetCoolDown(float remainingTime)
    {
        bool isCoolingDown = remainingTime > 0f;

        if (wasCoolingDown && !isCoolingDown && skill.CanExecute())
        {
            GameManager.Instance.Utility.Audio.PlaySFX(
                SFXType.Harvest_SkillReady);
        }

        wasCoolingDown = isCoolingDown;

        if (coolDownOverlay != null)
        {
            coolDownOverlay.fillAmount = skill.CoolDownTime > 0f
                ? remainingTime / skill.CoolDownTime
                : 0f;
        }

        if (coolDownText != null)
        {
            coolDownText.text = remainingTime > 0f
                ? Mathf.CeilToInt(remainingTime).ToString()
                : string.Empty;
        }

        if (button != null)
        {
            button.interactable = skill.CanExecute();
        }
    }
}
