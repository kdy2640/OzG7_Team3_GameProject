using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HubScene의 UI 상태와 전환을 제어합니다.
/// 각 UI 프리팹은 최초 요청 시 생성하고 이후 비활성화 상태로 재사용합니다.
/// </summary> 
public sealed class HubCanvasController : MonoBehaviour
{
    public enum HubCanvasState
    {   
        None = -1,
        HubView,
        FacilityManagement,
        MenuManagement,
        HarvestUpgrade,
        HarvestSelection,
        ServiceSelection,
        StaffManagement,
        DayStart,
        RankUpPanel
    }


    [Serializable]
    private sealed class ViewEntry
    {
        [Header("UI")]
        [SerializeField] private HubCanvasState state;
        [SerializeField] private UI_Base prefab;

        [Header("World Visual")]
        [SerializeField] private GameObject worldVisualPrefab;
        [SerializeField] private Transform worldVisualAnchor;

        [NonSerialized] private UI_Base instance;
        [NonSerialized] private GameObject worldVisualInstance;

        public HubCanvasState State => state;
        public UI_Base Prefab => prefab;
        public UI_Base Instance => instance;

        public void Initialize(HubCanvasController owner)
        {
            if (instance != null)
            {
                return;
            }

            instance = Instantiate(prefab, owner.transform, false);
            instance.name = prefab.name;
            instance.gameObject.SetActive(false);
            instance.Init(owner);

            InitializeWorldVisual(owner);
        }

        public void ShowWorldVisual()
        {
            if (worldVisualInstance != null)
            {
                worldVisualInstance.SetActive(true);
            }
        }

        public void HideWorldVisual()
        {
            if (worldVisualInstance != null)
            {
                worldVisualInstance.SetActive(false);
            }
        }

        private void InitializeWorldVisual(HubCanvasController owner)
        {
            if (worldVisualPrefab == null && worldVisualAnchor == null)
            {
                return;
            }

            if (worldVisualPrefab == null || worldVisualAnchor == null)
            {
                Debug.LogWarning(
                    $"[HubCanvasController] {state} requires both a world visual prefab and anchor.",
                    owner);
                return;
            }

            worldVisualInstance = Instantiate(worldVisualPrefab, worldVisualAnchor, false);
            worldVisualInstance.name = worldVisualPrefab.name;

            Transform visualTransform = worldVisualInstance.transform;
            visualTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            visualTransform.localScale = Vector3.one;

            worldVisualInstance.SetActive(false);
        }
    }

    [SerializeField] private HubCanvasState initialState = HubCanvasState.None;
    [SerializeField] private List<ViewEntry> views = new();

    private readonly Dictionary<HubCanvasState, ViewEntry> viewByState = new();

    private ViewEntry currentEntry;
    private Coroutine transitionCoroutine;
    private HubCanvasState? pendingState;

    public HubCanvasState CurrentState { get; private set; } = HubCanvasState.None;

    private void Awake()
    {
        BuildViewLookup();
    }

    private IEnumerator Start()
    {
        if (initialState == HubCanvasState.None)
        {
            yield break;
        }

        RequestStateChange(initialState);

        while (transitionCoroutine != null)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 현재 전환이 끝난 뒤 요청한 상태로 이동합니다.
    /// 전환 중 요청이 여러 번 들어오면 가장 최근 요청을 처리합니다.
    /// </summary>
    public void RequestStateChange(HubCanvasState nextState)
    {
        if (CurrentState == nextState && pendingState == null)
        {
            return;
        }

        pendingState = nextState;

        if (transitionCoroutine == null)
        {
            transitionCoroutine = StartCoroutine(ProcessStateRequests());
        }
    }

    private IEnumerator ProcessStateRequests()
    {
        while (pendingState.HasValue)
        {
            HubCanvasState nextState = pendingState.Value;
            pendingState = null;

            yield return ChangeState(nextState);
        }

        transitionCoroutine = null;
    }

    private IEnumerator ChangeState(HubCanvasState nextState)
    {
        ViewEntry nextEntry = null;

        if (nextState != HubCanvasState.None)
        {
            if (!viewByState.TryGetValue(nextState, out ViewEntry entry))
            {
                Debug.LogError($"[HubCanvasController] {nextState} 상태에 연결된 UI 프리팹이 없습니다.", this);
                yield break;
            }

            nextEntry = entry;
        }

        if (currentEntry != null)
        {
            yield return currentEntry.Instance.Hide();
            currentEntry.HideWorldVisual();
        }

        currentEntry = nextEntry;
        CurrentState = nextState;

        if (currentEntry != null)
        {
            currentEntry.ShowWorldVisual();
            yield return currentEntry.Instance.Show();
        }
    }

    private void BuildViewLookup()
    {
        viewByState.Clear();

        for (int i = 0; i < views.Count; i++)
        {
            ViewEntry entry = views[i];

            if (entry.State == HubCanvasState.None)
            {
                Debug.LogWarning("[HubCanvasController] None 상태에는 UI 프리팹을 연결하지 않습니다.", this);
                continue;
            }

            if (entry.Prefab == null)
            {
                Debug.LogWarning($"[HubCanvasController] {entry.State} 상태의 UI 프리팹이 비어 있습니다.", this);
                continue;
            }

            if (!viewByState.TryAdd(entry.State, entry))
            {
                Debug.LogError($"[HubCanvasController] {entry.State} 상태가 중복 등록되어 있습니다.", this);
                continue;
            }

            entry.Initialize(this);
        }
    }

}
