using UnityEngine;

public class AuraApplier : MonoBehaviour
{
    [Header("Aura")]
    [SerializeField] private AuraType auraType = AuraType.Rain;

    [Header("References")]
    [SerializeField] private Transform auraSphere;

    [Header("Reaction")]
    [SerializeField] private float affectDistance = 2.5f;
    [SerializeField] private float checkInterval = 0.2f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            ApplyAuraToNearbyRegions();
        }
    }

    private void ApplyAuraToNearbyRegions()
    {
        if (auraSphere == null)
        {
            Debug.LogWarning($"{name}: auraSphere is not assigned.");
            return;
        }

        PlanetRegion[] regions = FindObjectsByType<PlanetRegion>(FindObjectsSortMode.None);

        foreach (PlanetRegion region in regions)
        {
            float distance = Vector3.Distance(auraSphere.position, region.TriggerPosition);

            if (distance <= affectDistance)
            {
                region.ReactToAura(auraType);
            }
        }
    }
}