using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetRegion : MonoBehaviour
{
    public static event Action OnAnyRegionStateChanged;

    [Header("Region Info")]
    [SerializeField] private RegionKind regionKind = RegionKind.Grassland;
    [SerializeField] private RegionState currentState = RegionState.Normal;

    [Header("References")]
    [SerializeField] private Transform triggerPoint;

    [Header("Visual Roots")]
    [SerializeField] private GameObject droughtRoot;
    [SerializeField] private GameObject grassRoot;
    [SerializeField] private GameObject cityRoot;
    [SerializeField] private GameObject floodRoot;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1.2f;
    [SerializeField] private float outgoingMinScaleFactor = 0.15f;
    [SerializeField] private float incomingDelay = 0.15f;

    [Header("Reaction")]
    [SerializeField] private float reactionCooldown = 1f;

    [Header("Messenger")]
    [SerializeField] private bool sendMentorMessages = true;
    [SerializeField] private string mentorName = "Mentor";

    private bool isTransitioning;
    private float lastReactionTime = -999f;

    private Vector3 droughtOriginalScale = Vector3.one;
    private Vector3 grassOriginalScale = Vector3.one;
    private Vector3 cityOriginalScale = Vector3.one;
    private Vector3 floodOriginalScale = Vector3.one;

    public RegionKind RegionKind => regionKind;
    public RegionState CurrentState => currentState;
    public Vector3 TriggerPosition => triggerPoint != null ? triggerPoint.position : transform.position;

    public bool IsDrought => regionKind == RegionKind.Grassland && currentState == RegionState.Drought;
    public bool IsFlood => regionKind == RegionKind.City && currentState == RegionState.Flood;
    public bool IsNormalGrass => regionKind == RegionKind.Grassland && currentState == RegionState.Normal;
    public bool IsNormalCity => regionKind == RegionKind.City && currentState == RegionState.Normal;

    public bool IsInDanger
    {
        get
        {
            if (regionKind == RegionKind.Grassland && currentState == RegionState.Drought)
                return true;

            if (regionKind == RegionKind.City && currentState == RegionState.Flood)
                return true;

            return false;
        }
    }

    public bool IsStable => !IsInDanger;

    private void Awake()
    {
        CacheOriginalScales();
    }

    private void Start()
    {
        ApplyVisualStateInstant();
        NotifyStateChanged();
    }

    public void Initialize(RegionKind kind, RegionState state)
    {
        regionKind = kind;
        currentState = state;
        ApplyVisualStateInstant();
        NotifyStateChanged();
    }

    public void SetState(RegionState newState)
    {
        if (currentState == newState)
            return;

        StartStateTransition(newState);
    }

    public void ReactToAura(AuraType auraType)
    {
        if (isTransitioning)
            return;

        if (Time.time < lastReactionTime + reactionCooldown)
            return;

        RegionState oldState = currentState;
        RegionState newState = currentState;
        bool isPositiveAction = false;
        bool changed = false;

        switch (regionKind)
        {
            case RegionKind.Grassland:
                ResolveGrasslandReaction(auraType, ref newState, ref isPositiveAction, ref changed);
                break;

            case RegionKind.City:
                ResolveCityReaction(auraType, ref newState, ref isPositiveAction, ref changed);
                break;
        }

        if (!changed || newState == oldState)
            return;

        lastReactionTime = Time.time;

        if (sendMentorMessages && PriestMessengerSystem.Instance != null)
        {
            string mentorMessage = BuildMentorMessage(auraType, oldState, newState, isPositiveAction);
            PriestMessengerSystem.Instance.SendMessage(mentorName, mentorMessage);
        }

        StartStateTransition(newState);
    }

    private void ResolveGrasslandReaction(AuraType auraType, ref RegionState newState, ref bool isPositiveAction, ref bool changed)
    {
        if (auraType == AuraType.Fire && currentState == RegionState.Normal)
        {
            newState = RegionState.Drought;
            isPositiveAction = false;
            changed = true;
        }
        else if (auraType == AuraType.Rain && currentState == RegionState.Drought)
        {
            newState = RegionState.Normal;
            isPositiveAction = true;
            changed = true;
        }
    }

    private void ResolveCityReaction(AuraType auraType, ref RegionState newState, ref bool isPositiveAction, ref bool changed)
    {
        if (auraType == AuraType.Rain && currentState == RegionState.Normal)
        {
            newState = RegionState.Flood;
            isPositiveAction = false;
            changed = true;
        }
        else if (auraType == AuraType.Fire && currentState == RegionState.Flood)
        {
            newState = RegionState.Normal;
            isPositiveAction = true;
            changed = true;
        }
    }

    private void StartStateTransition(RegionState newState)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToState(newState));
    }

    private IEnumerator TransitionToState(RegionState newState)
    {
        isTransitioning = true;

        GameObject oldRoot = GetActiveRootForCurrentState();
        GameObject newRoot = GetRootForState(regionKind, newState);

        Vector3 oldScale = GetOriginalScale(oldRoot);
        Vector3 newScale = GetOriginalScale(newRoot);

        if (newRoot != null)
        {
            newRoot.SetActive(true);
            newRoot.transform.localScale = Vector3.zero;
        }

        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;

            float outT = Mathf.Clamp01(time / transitionDuration);
            outT = Mathf.SmoothStep(0f, 1f, outT);

            if (oldRoot != null)
            {
                Vector3 targetOldScale = Vector3.Lerp(oldScale, oldScale * outgoingMinScaleFactor, outT);
                oldRoot.transform.localScale = targetOldScale;
            }

            if (newRoot != null && time >= incomingDelay)
            {
                float inT = Mathf.Clamp01((time - incomingDelay) / Mathf.Max(0.01f, transitionDuration - incomingDelay));
                inT = Mathf.SmoothStep(0f, 1f, inT);

                Vector3 targetNewScale = Vector3.Lerp(Vector3.zero, newScale, inT);
                newRoot.transform.localScale = targetNewScale;
            }

            yield return null;
        }

        currentState = newState;
        ApplyVisualStateInstant();
        NotifyStateChanged();

        isTransitioning = false;
    }

    private void ApplyVisualStateInstant()
    {
        ResetAllRoots();

        GameObject targetRoot = GetRootForState(regionKind, currentState);
        if (targetRoot != null)
        {
            targetRoot.SetActive(true);
            targetRoot.transform.localScale = GetOriginalScale(targetRoot);
        }
    }

    private void ResetAllRoots()
    {
        ResetRoot(droughtRoot, droughtOriginalScale);
        ResetRoot(grassRoot, grassOriginalScale);
        ResetRoot(cityRoot, cityOriginalScale);
        ResetRoot(floodRoot, floodOriginalScale);
    }

    private void ResetRoot(GameObject root, Vector3 originalScale)
    {
        if (root == null)
            return;

        root.SetActive(false);
        root.transform.localScale = originalScale;
    }

    private GameObject GetActiveRootForCurrentState()
    {
        return GetRootForState(regionKind, currentState);
    }

    private GameObject GetRootForState(RegionKind kind, RegionState state)
    {
        switch (kind)
        {
            case RegionKind.Grassland:
                return state == RegionState.Drought ? droughtRoot : grassRoot;

            case RegionKind.City:
                return state == RegionState.Flood ? floodRoot : cityRoot;

            default:
                return null;
        }
    }

    private void CacheOriginalScales()
    {
        if (droughtRoot != null) droughtOriginalScale = droughtRoot.transform.localScale;
        if (grassRoot != null) grassOriginalScale = grassRoot.transform.localScale;
        if (cityRoot != null) cityOriginalScale = cityRoot.transform.localScale;
        if (floodRoot != null) floodOriginalScale = floodRoot.transform.localScale;
    }

    private Vector3 GetOriginalScale(GameObject root)
    {
        if (root == droughtRoot) return droughtOriginalScale;
        if (root == grassRoot) return grassOriginalScale;
        if (root == cityRoot) return cityOriginalScale;
        if (root == floodRoot) return floodOriginalScale;

        return Vector3.one;
    }

    private void NotifyStateChanged()
    {
        OnAnyRegionStateChanged?.Invoke();
    }


   private string BuildMentorMessage(AuraType auraType, RegionState oldState, RegionState newState, bool isPositiveAction)
{
    string regionLabel = GetRegionLabel();

    if (isPositiveAction)
    {
        switch (regionKind)
        {
            case RegionKind.Grassland:
                string[] grassPositive =
                {
                    $"Nice work. You brought life back to {regionLabel} — the rain ended the drought and the land is breathing again.",
                    $"Well done. {regionLabel} is recovering — the drought is gone and the grass is coming back.",
                    $"Good job. You restored balance in {regionLabel}. The soil is alive again.",
                    $"Solid move. {regionLabel} is no longer dry — the grass is starting to grow again."
                };
                return grassPositive[Random.Range(0, grassPositive.Length)];

            case RegionKind.City:
                string[] cityPositive =
                {
                    $"Nice work. You stabilized {regionLabel} — the water is gone and the city is recovering.",
                    $"Good job. {regionLabel} is back under control. The flooding has subsided.",
                    $"Well handled. You brought balance back to {regionLabel}, the city is functioning again.",
                    $"Clean recovery. {regionLabel} is no longer flooded — things are returning to normal."
                };
                return cityPositive[Random.Range(0, cityPositive.Length)];

            default:
                string[] defaultPositive =
                {
                    $"Good job. You restored balance in the region.",
                    $"Well done. The situation is under control again.",
                    $"Nice work. The region is stable now.",
                    $"Solid move. Balance has been restored."
                };
                return defaultPositive[Random.Range(0, defaultPositive.Length)];
        }
    }
    else
    {
        switch (regionKind)
        {
            case RegionKind.Grassland:
                string[] grassNegative =
                {
                    $"You dried out {regionLabel}. The land is scorched — you'll need rain to fix this.",
                    $"That wasn't great. {regionLabel} is now in drought — the soil is burning.",
                    $"You pushed {regionLabel} into drought. The grass is gone, the land is suffering.",
                    $"Bad move. {regionLabel} is dried out — you'll have to restore it with rain."
                };
                return grassNegative[Random.Range(0, grassNegative.Length)];

            case RegionKind.City:
                string[] cityNegative =
                {
                    $"You flooded {regionLabel}. The city is overwhelmed — you need to fix this.",
                    $"That caused a flood in {regionLabel}. The area is unstable now.",
                    $"Bad call. {regionLabel} is underwater — restore balance quickly.",
                    $"You overloaded {regionLabel} with water. The city can't handle it."
                };
                return cityNegative[Random.Range(0, cityNegative.Length)];

            default:
                string[] defaultNegative =
                {
                    $"You disrupted the balance in the region.",
                    $"That made things worse. The region is unstable now.",
                    $"Careful. You've thrown the region out of balance.",
                    $"Not good. The situation has deteriorated."
                };
                return defaultNegative[Random.Range(0, defaultNegative.Length)];
        }
    }
}

    private string GetRegionLabel()
    {
        switch (regionKind)
        {
            case RegionKind.Grassland:
                return "Fields Region";

            case RegionKind.City:
                return "City Region";

            default:
                return "Region";
        }
    }
}