using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
public sealed class HubLightingController : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] private MarketPhase previewPhase = MarketPhase.Morning;

    [Header("Sky")]
    [SerializeField] private Image hubViewSky;

    [Header("Morning")]
    [SerializeField] private Color morningColor = new(1f, 0.95f, 0.82f, 1f);
    [SerializeField] private float morningIntensity = 0.65f;
    [SerializeField] private Vector3 morningRotation = new(65f, 20f, 0f);
    [SerializeField] private Color morningSkyColor = new(0.45f, 0.7f, 0.92f, 1f);

    [Header("Afternoon")]
    [SerializeField] private Color afternoonColor = new(1f, 0.78f, 0.58f, 1f);
    [SerializeField] private float afternoonIntensity = 0.5f;
    [SerializeField] private Vector3 afternoonRotation = new(35f, -60f, 0f);
    [SerializeField] private Color afternoonSkyColor = new(0.96f, 0.78f, 0.58f, 1f);

    [Header("Night")]
    [SerializeField] private Color nightColor = new(0.48f, 0.6f, 1f, 1f);
    [SerializeField] private float nightIntensity = 0.15f;
    [SerializeField] private Vector3 nightRotation = new(25f, 140f, 0f);
    [SerializeField] private Color nightSkyColor = new(0.07f, 0.1f, 0.22f, 1f);

    private Light hubLight;
    private MarketManager marketManager;

    private void Awake()
    {
        marketManager = GameManager.Instance.Market;
    }

    private void OnEnable()
    {
        marketManager.SubscribeMarketDataChanged(ApplyCurrentPhaseLighting);
        ApplyCurrentPhaseLighting();
    }

    private void OnDisable()
    {
        marketManager.UnsubscribeMarketDataChanged(ApplyCurrentPhaseLighting);
    }

    [ContextMenu("Refresh Lighting")]
    public void RefreshLighting()
    {
        ApplyLighting(previewPhase);
    }

    private void ApplyCurrentPhaseLighting()
    {
        ApplyLighting(marketManager.MarketData.CurrentPhase);
    }

    private void ApplyLighting(MarketPhase phase)
    {
        hubLight = GetComponent<Light>();

        switch (phase)
        {
            case MarketPhase.Morning:
                SetLighting(morningColor, morningIntensity, morningRotation, morningSkyColor);
                break;
            case MarketPhase.Afternoon:
                SetLighting(afternoonColor, afternoonIntensity, afternoonRotation, afternoonSkyColor);
                break;
            case MarketPhase.Night:
                SetLighting(nightColor, nightIntensity, nightRotation, nightSkyColor);
                break;
        }
    }

    private void SetLighting(
        Color color,
        float intensity,
        Vector3 rotation,
        Color skyColor)
    {
        hubLight.color = color;
        hubLight.intensity = intensity;
        transform.rotation = Quaternion.Euler(rotation);
        hubViewSky.color = skyColor;
    }
}
