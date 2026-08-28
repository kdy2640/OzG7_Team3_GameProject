using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TempButtonInvoker : MonoBehaviour
{
    [SerializeField] private SFXType sfxType = SFXType.Global_ButtonClick;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlaySFX);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(PlaySFX);
    }

    private void PlaySFX()
    {
        GameManager.Instance.Utility.Audio.PlaySFX(sfxType);
    }
}
