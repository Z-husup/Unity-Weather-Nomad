using UnityEngine;

public class PriestSwitcher : MonoBehaviour
{
    [Header("Priests")]
    [SerializeField] private PriestController[] priests;

    [Header("Input")]
    [SerializeField] private KeyCode switchKey = KeyCode.Tab;

    [Header("Start Settings")]
    [SerializeField] private int startingPriestIndex = 0;

    [Header("Camera")]
    [SerializeField] private FollowPlayerCamera followPlayerCamera;

    private int currentPriestIndex;

    private void Start()
    {
        if (priests == null || priests.Length == 0)
        {
            Debug.LogWarning("PriestSwitcher: no priests assigned.");
            return;
        }

        currentPriestIndex = Mathf.Clamp(startingPriestIndex, 0, priests.Length - 1);
        ApplyCurrentPriest();
    }

    private void Update()
    {
        if (priests == null || priests.Length == 0)
            return;

        if (Input.GetKeyDown(switchKey))
        {
            SwitchToNextPriest();
        }
    }

    private void SwitchToNextPriest()
    {
        currentPriestIndex++;

        if (currentPriestIndex >= priests.Length)
            currentPriestIndex = 0;

        ApplyCurrentPriest();
    }

    private void ApplyCurrentPriest()
    {
        for (int i = 0; i < priests.Length; i++)
        {
            bool isActive = i == currentPriestIndex;
            priests[i].SetControlled(isActive);
        }

        if (followPlayerCamera != null)
        {
            followPlayerCamera.SetPlayer(priests[currentPriestIndex].PriestTransform);
        }
    }
}