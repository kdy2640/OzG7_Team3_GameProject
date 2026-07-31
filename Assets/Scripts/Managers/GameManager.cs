using UnityEngine;


// 전역으로 접근 가능한 게임매니저.
// Start에서 GameManager.Instance를 통해 접근해주세요. <- Awake에서 초기화하기 때문에 보수적인 체킹
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public StockManager StockManager { get; private set; }
    public CookingManager CookingManager { get; private set; }
    public SceneController Scene { get; private set; }
    public UpgradeManager Upgrade { get; private set; }
    public HarvestManager Harvest { get; private set; } 
    public AudioManager AudioManager { get; private set; }
    public TutorialManager Tutorial { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StockManager = GetComponent<StockManager>();
        CookingManager = StockManager.CookingManager;
        Scene = GetComponent<SceneController>();
        Upgrade = GetComponent<UpgradeManager>();
        Harvest = GetComponent<HarvestManager>(); 
        AudioManager = GetComponentInChildren<AudioManager>();
        Tutorial = GetComponentInChildren<TutorialManager>();
    }
}
