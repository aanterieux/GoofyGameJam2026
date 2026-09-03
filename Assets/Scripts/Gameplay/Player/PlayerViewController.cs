using UnityEngine;

public class PlayerViewController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] private bool invertVAxis = true;

    private float verticalRotation = 0f;

    private void Awake()
    {
        verticalRotation = transform.localEulerAngles.x;

        if (verticalRotation > 180f)
        {
            verticalRotation -= 360f;
        }
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        if (!InputManager.MouseConnected)
        {
            return;
        }

        float verticalAngle =
            InputManager.MouseDelta.y *
            (invertVAxis
                ? 1f
                : -1f
            );

        verticalRotation += Time.deltaTime * verticalAngle * rotationSpeed;
        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -maxRotationAngle,
            maxRotationAngle
        );

        transform.localRotation = Quaternion.Euler(
            verticalRotation,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z
        );
    }
}
