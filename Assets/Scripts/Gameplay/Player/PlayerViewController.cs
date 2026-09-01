using UnityEngine;

public class PlayerViewController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private bool invertVAxis = true;

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

        Vector2 rotationAxes = InputManager.MouseDelta;
        float vertical = rotationAxes.y * ((invertVAxis) ? 1f : -1f);

        rotationAxes.x = vertical;
        rotationAxes.y = 0f;

        transform.Rotate(Time.deltaTime * rotationSpeed * rotationAxes);
    }
}
