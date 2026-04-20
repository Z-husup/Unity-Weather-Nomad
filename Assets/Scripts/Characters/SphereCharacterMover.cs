using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SphereCharacterMover : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private Transform planetCenter;
    [SerializeField] private float planetRadius = 27.5f;
    [SerializeField] private float surfaceOffset = 0.2f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float rotationSmooth = 12f;

    private CharacterController controller;

    // Это наше стабильное направление движения по поверхности.
    private Vector3 surfaceForward;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (planetCenter == null)
        {
            Debug.LogError($"{name}: SphereCharacterMover requires planetCenter to be assigned.");
            enabled = false;
            return;
        }

        Vector3 upDirection = (transform.position - planetCenter.position).normalized;

        // Берем начальное направление по поверхности.
        surfaceForward = Vector3.ProjectOnPlane(transform.forward, upDirection).normalized;

        // Если модель смотрит криво, даем запасной вариант.
        if (surfaceForward.sqrMagnitude < 0.001f)
        {
            surfaceForward = Vector3.ProjectOnPlane(Vector3.forward, upDirection).normalized;
        }

        SnapToSurface();
        AlignToPlanet(upDirection);
    }

    private void Update()
    {
        if (planetCenter == null)
            return;

        Vector3 upDirection = (transform.position - planetCenter.position).normalized;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Поворачиваем направление по поверхности.
        if (Mathf.Abs(horizontal) > 0.001f)
        {
            Quaternion turn = Quaternion.AngleAxis(horizontal * turnSpeed * Time.deltaTime, upDirection);
            surfaceForward = turn * surfaceForward;
            surfaceForward = Vector3.ProjectOnPlane(surfaceForward, upDirection).normalized;
        }

        // Двигаемся вперед/назад по поверхности.
        if (Mathf.Abs(vertical) > 0.001f)
        {
            Vector3 moveDirection = surfaceForward * vertical;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        SnapToSurface();
        AlignToPlanet((transform.position - planetCenter.position).normalized);
    }
    private void SnapToSurface()
    {
        Vector3 upDirection = (transform.position - planetCenter.position).normalized;
        transform.position = planetCenter.position + upDirection * (planetRadius + surfaceOffset);
    }

    private void AlignToPlanet(Vector3 upDirection)
    {
        // Обновляем направление еще раз после движения, чтобы оно не "сползало".
        surfaceForward = Vector3.ProjectOnPlane(surfaceForward, upDirection).normalized;

        if (surfaceForward.sqrMagnitude < 0.001f)
        {
            surfaceForward = Vector3.ProjectOnPlane(transform.forward, upDirection).normalized;
        }

        Quaternion targetRotation = Quaternion.LookRotation(surfaceForward, upDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}
