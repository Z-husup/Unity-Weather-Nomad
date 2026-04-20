using UnityEngine;

public class PlanetRegionGenerator : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private Transform planetCenter;
    [SerializeField] private float planetRadius = 26.5f;
    [SerializeField] private float surfaceOffset = -2f;

    [Header("Region Prefab")]
    [SerializeField] private PlanetRegion regionPrefab;
    [SerializeField] private Transform regionsParent;

    [Header("Generation")]
    [SerializeField] private int regionCount = 40;

    [Header("Type Chances")]
    [Range(0f, 1f)]
    [SerializeField] private float grasslandChance = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float cityChance = 0.3f;

    [Header("State Chances")]
    [Range(0f, 1f)]
    [SerializeField] private float droughtChanceForGrassland = 0.4f;

    [Range(0f, 1f)]
    [SerializeField] private float floodChanceForCity = 0.35f;

    [Header("Rotation")]
    [SerializeField] private bool randomYawRotation = true;

    private void Start()
    {
        GenerateRegions();
    }

    public void GenerateRegions()
    {
        if (planetCenter == null)
        {
            Debug.LogError("PlanetRegionGenerator: planetCenter is not assigned.");
            return;
        }

        if (regionPrefab == null)
        {
            Debug.LogError("PlanetRegionGenerator: regionPrefab is not assigned.");
            return;
        }

        ClearOldRegions();

        if (regionCount <= 0)
            return;

        float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < regionCount; i++)
        {
            Vector3 direction = GetFibonacciSphereDirection(i, regionCount, goldenAngle);

            Vector3 spawnPosition =
                planetCenter.position +
                direction * (planetRadius + surfaceOffset);

            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, direction);

            if (randomYawRotation)
            {
                surfaceRotation = surfaceRotation * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }

            PlanetRegion newRegion = Instantiate(
                regionPrefab,
                spawnPosition,
                surfaceRotation,
                regionsParent
            );

            newRegion.name = $"Region_{i:000}";

            RegionKind kind = GetRandomRegionKind();
            RegionState state = GetRandomStateForKind(kind);

            newRegion.Initialize(kind, state);
        }
    }

    private void ClearOldRegions()
    {
        if (regionsParent == null)
            return;

        for (int i = regionsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(regionsParent.GetChild(i).gameObject);
        }
    }

    private Vector3 GetFibonacciSphereDirection(int index, int total, float goldenAngle)
    {
        if (total == 1)
            return Vector3.up;

        float y = 1f - (2f * index) / (total - 1f);
        float radius = Mathf.Sqrt(1f - y * y);
        float theta = goldenAngle * index;

        float x = Mathf.Cos(theta) * radius;
        float z = Mathf.Sin(theta) * radius;

        return new Vector3(x, y, z).normalized;
    }

    private RegionKind GetRandomRegionKind()
    {
        float total = grasslandChance + cityChance;

        if (total <= 0f)
            return RegionKind.Grassland;

        float roll = Random.value * total;

        if (roll < grasslandChance)
            return RegionKind.Grassland;

        return RegionKind.City;
    }

    private RegionState GetRandomStateForKind(RegionKind kind)
    {
        switch (kind)
        {
            case RegionKind.Grassland:
                return Random.value < droughtChanceForGrassland
                    ? RegionState.Drought
                    : RegionState.Normal;

            case RegionKind.City:
                return Random.value < floodChanceForCity
                    ? RegionState.Flood
                    : RegionState.Normal;

            default:
                return RegionState.Normal;
        }
    }
}