using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Game/TutorialDataSO")]
public sealed class TutorialDataSO : ScriptableObject
{
    [Header("Tutorial Info")]
    public TutorialManager.TutorialType tutorialType;

    public string title;

    public Sprite image;

    [TextArea(5,12)] 
    public string description;
}
