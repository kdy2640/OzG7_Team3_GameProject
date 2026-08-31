using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public sealed class GoldenPigRadarIndicator : MonoBehaviour
{
    [SerializeField, Min(0f)] private float indicatorHeight = 2f;

    private Canvas indicatorCanvas;
    private AnimalStateController animalStateController;
    private Camera targetCamera;
    private float detectionRadius;
    private bool hasPlayedDetectedSFX;

    private void Start()
    {
        indicatorCanvas = GetComponent<Canvas>();
        animalStateController = GetComponentInParent<AnimalStateController>();
        targetCamera = Camera.main;
        detectionRadius = GameManager.Instance.Upgrade.RuntimeStat.Harvest.Get(
            HarvestStatType.GoldenPigDetectionRadius);

        transform.localPosition = Vector3.up * indicatorHeight;
        indicatorCanvas.enabled = false;
        hasPlayedDetectedSFX = false;
    }

    private void LateUpdate()
    {
        bool isDetected = detectionRadius > 0f
            && animalStateController.IsPlayerWithin(detectionRadius);

        indicatorCanvas.enabled = isDetected;

        if (isDetected && !hasPlayedDetectedSFX)
        {
            hasPlayedDetectedSFX = true;
            GameManager.Instance.Utility.Audio.PlaySFX(
                SFXType.Harvest_GoldenPigDetected);
        }

        transform.rotation = targetCamera.transform.rotation;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        transform.localPosition = Vector3.up * indicatorHeight;
    }
#endif
}
