using UnityEngine;
using TMPro;

public class RegionStatsManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text dangerText;
    [SerializeField] private TMP_Text stableText;
    [SerializeField] private TMP_Text totalText;

    private void OnEnable()
    {
        PlanetRegion.OnAnyRegionStateChanged += RefreshStats;
    }

    private void OnDisable()
    {
        PlanetRegion.OnAnyRegionStateChanged -= RefreshStats;
    }

    private void Start()
    {
        RefreshStats();
    }

    public void RefreshStats()
    {
        PlanetRegion[] regions = FindObjectsByType<PlanetRegion>(FindObjectsSortMode.None);

        int total = regions.Length;
        int danger = 0;
        int stable = 0;

        foreach (PlanetRegion region in regions)
        {
            if (region.IsInDanger)
                danger++;
            else
                stable++;
        }

        if (dangerText != null)
            dangerText.text = $"⚠ In danger: {danger}";

        if (stableText != null)
            stableText.text = $"✔ Stable: {stable}";

        if (totalText != null)
            totalText.text = $"Total Regions: {total}";
    }
}