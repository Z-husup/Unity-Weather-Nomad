using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform planetCenter;

    [SerializeField] private float distance = 7f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float smoothSpeed = 6f;

    private void LateUpdate()
    {
        if (player == null || planetCenter == null)
            return;

        Vector3 upDirection = (player.position - planetCenter.position).normalized;
        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(player.forward, upDirection).normalized;

        if (forwardOnSurface.sqrMagnitude < 0.001f)
        {
            forwardOnSurface = Vector3.ProjectOnPlane(Vector3.forward, upDirection).normalized;
        }

        Vector3 desiredPosition =
            player.position
            - forwardOnSurface * distance
            + upDirection * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(player.position, upDirection);
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}