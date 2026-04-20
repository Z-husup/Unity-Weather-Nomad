using UnityEngine;

public class PriestController : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private PriestType priestType;

    [Header("Controlled Scripts")]
    [SerializeField] private SphereCharacterMover movementScript;
    [SerializeField] private MonoBehaviour abilityScript;

    public PriestType PriestType => priestType;
    public Transform PriestTransform => transform;

    public void SetControlled(bool isControlled)
    {
        if (movementScript != null)
            movementScript.enabled = isControlled;

        if (abilityScript != null)
            abilityScript.enabled = isControlled;
    }
}