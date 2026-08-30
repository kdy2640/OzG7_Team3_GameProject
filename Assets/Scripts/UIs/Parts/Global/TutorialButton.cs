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
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OpenTutorial);
    }
    public void OpenTutorial()
    {
        TutorialDataSO tutorialData = TutorialDataDB.GetData(tutorialType);

        if (tutorialData == null)
            return;

        if (tutorialPopup == null)
        {
            tutorialPopup = FindFirstObjectByType<TutorialPopup>(
                FindObjectsInactive.Include);
        }

        if (tutorialPopup == null)
            tutorialPopup = Instantiate(tutorialPopupPrefab);

        tutorialPopup.Open(tutorialData);
    }
}
