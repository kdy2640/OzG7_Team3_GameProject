using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TempSceneChangeButton : MonoBehaviour
{
    [SerializeField] private SceneType sceneType;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    public void OnButtonClicked()
    {
        GameManager.Instance.Scene.ChangeScene(sceneType);
    }
    private void ChangeText()
    {
        text.text = "To" + sceneType.ToString();
    }
    private void OnValidate()
    {
        ChangeText();
    }
    private void Awake()
    {
        if(button == null) button = gameObject.GetOrAddComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
