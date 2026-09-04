using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class TutorialButton : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private TutorialManager.TutorialType tutorialType;
    [SerializeField] private TutorialPopup tutorialPopupPrefab;

    private TutorialPopup tutorialPopup;

    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenTutorial);
    }
    private void OnEnable()
    {
        StartCoroutine(OpenTutorialOnFirstEnable());
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OpenTutorial);
    }
    private IEnumerator OpenTutorialOnFirstEnable()
    {
        yield return null;

        TutorialManager tutorialManager = GameManager.Instance.Utility.Tutorial;

        if (tutorialManager.GetTutorialProgressed(tutorialType))
            yield break;

        tutorialManager.ResolveTutorial(tutorialType);
        OpenTutorial();
    }
    public void OpenTutorial()
    {
        if (tutorialPopup == null)
        {
            tutorialPopup = FindFirstObjectByType<TutorialPopup>(
                FindObjectsInactive.Include);
        }

        if (tutorialPopup == null)
            tutorialPopup = Instantiate(tutorialPopupPrefab);

        tutorialPopup.Open(tutorialType);
    }
}
