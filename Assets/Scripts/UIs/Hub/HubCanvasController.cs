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
        StaffManagement
    }


    [Serializable]
    private sealed class ViewEntry
    {
        [SerializeField] private HubCanvasState state;
        [SerializeField] private UI_Base prefab;

        [NonSerialized] private UI_Base instance;

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
        }
    }

    [SerializeField] private HubCanvasState initialState = HubCanvasState.None;
    [SerializeField] private List<ViewEntry> views = new();

    private readonly Dictionary<HubCanvasState, ViewEntry> viewByState = new();

    private UI_Base currentView;
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
        UI_Base nextView = null;

        if (nextState != HubCanvasState.None)
        {
            if (!viewByState.TryGetValue(nextState, out ViewEntry entry))
            {
                Debug.LogError($"[HubCanvasController] {nextState} 상태에 연결된 UI 프리팹이 없습니다.", this);
                yield break;
            }

            nextView = entry.Instance;
        }

        if (currentView != null)
        {
            yield return currentView.Hide();
        }

        currentView = nextView;
        CurrentState = nextState;

        if (currentView != null)
        {
            yield return currentView.Show();
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
