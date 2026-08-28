using UnityEngine;

public sealed class TutorialButton : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private TutorialPopup tutorialPopup;
    [SerializeField] private TutorialDataSO tutorialData;

    public void OpenTutorial()
    {
        if (tutorialPopup == null)
        {
            Debug.LogWarning("[TutorialButton] TutorialPopup이 연결되지 않았습니다.");
            return;
        }

        if (tutorialData == null)
        {
            Debug.LogWarning("[TutorialButton] TutorialDataSO가 연결되지 않았습니다.");
            return;
        }

        tutorialPopup.Open(tutorialData);
    }
}