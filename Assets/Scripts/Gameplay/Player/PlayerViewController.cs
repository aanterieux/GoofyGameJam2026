using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerViewController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private bool invertVAxis = true;

    private Mouse mouse = null;

    private void Awake()
    {
        mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        Vector2 screenCenter = 0.5f * new Vector2(
            Screen.width,
            Screen.height
        );

        mouse.WarpCursorPosition(screenCenter);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Rotate();
    }


    private void Rotate()
    {
        if (mouse == null)
        {
            return;
        }

        Vector2 rotationAxes = mouse.delta.ReadValue();
        float vertical = rotationAxes.y * ((invertVAxis) ? 1f : -1f);

        rotationAxes.x = vertical;
        rotationAxes.y = 0f;

        transform.Rotate(Time.deltaTime * rotationSpeed * rotationAxes);
    }
}
