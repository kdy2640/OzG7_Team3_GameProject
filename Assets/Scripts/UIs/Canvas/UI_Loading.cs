using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class UI_Loading : MonoBehaviour
{
    [SerializeField] private LoopPresenter loopPresenter;

    [Header("Loading Tween")]
    [SerializeField, Min(0f)] private float openDuration = 0.5f;
    [SerializeField, Min(0f)] private float closeDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutCubic;
    [SerializeField] private Ease closeEase = Ease.InCubic;

    private Tween currentTween;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        loopPresenter.gameObject.SetActive(false);
        loopPresenter.transform.localScale = Vector3.zero;
    }

    public IEnumerator OpenLoading()
    {
        KillCurrentTween();

        GameManager.Instance.Utility.Audio.PlaySFX(
            SFXType.Global_SceneChangeStart);
        loopPresenter.gameObject.SetActive(true);
        loopPresenter.transform.localScale = Vector3.zero;

        currentTween = loopPresenter.transform
            .DOScale(Vector3.one, openDuration)
            .SetEase(openEase)
            .SetUpdate(true);

        yield return currentTween.WaitForCompletion();

        currentTween = null;
    }

    public IEnumerator CloseLoading()
    {
        KillCurrentTween();

        GameManager.Instance.Utility.Audio.PlaySFX(
            SFXType.Global_SceneChangeEnd);
        loopPresenter.transform.localScale = Vector3.one;

        currentTween = loopPresenter.transform
            .DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .SetUpdate(true);

        yield return currentTween.WaitForCompletion();

        loopPresenter.gameObject.SetActive(false);
        currentTween = null;
    }

    private void KillCurrentTween()
    {
        currentTween?.Kill();
        currentTween = null;
    }

    private void OnDestroy()
    {
        KillCurrentTween();
    }
}
