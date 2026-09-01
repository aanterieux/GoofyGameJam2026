using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

// Needs an instance for some features
public class InputManager : MonoBehaviour
{
    public static Keyboard CurrentKeyboard
    {
        get => Keyboard.current;
    }
    public static Mouse CurrentMouse
    {
        get => Mouse.current;
    }
    public static Gamepad CurrentGamepad
    {
        get => Gamepad.current;
    }

    public static StickControl GamepadStick_Left
    {
        get => CurrentGamepad.leftStick;
    }
    public static StickControl GamepadStick_Right
    {
        get => CurrentGamepad.rightStick;
    }

    public static Vector2 MouseDelta
    {
        get => CurrentMouse.delta.ReadValue();
    }
    public static Vector2 GamepadDelta_Left
    {
        get => GamepadStick_Left.ReadValue();
    }
    public static Vector2 GamepadDelta_Right
    {
        get => GamepadStick_Right.ReadValue();
    }

    public static int GameDeviceCount
    {
        get =>
            InputSystem.devices.Count(
                d =>
                    d is Keyboard ||
                    d is Mouse    ||
                    d is Gamepad
            );
    }

    public static bool KeyboardConnected
    {
        get => (CurrentKeyboard != null);
    }
    public static bool MouseConnected
    {
        get => (CurrentMouse != null);
    }
    public static bool GamepadConnected
    {
        get => (CurrentGamepad != null);
    }
    public static bool NoGameDeviceConnected
    {
        get => (GameDeviceCount == 0);
    }

    public static bool KeyboardPress
    {
        get => CurrentKeyboard.anyKey.IsPressed();
    }
    public static bool MousePress
    {
        get =>
            (CurrentMouse.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public static bool GamepadPress
    {
        get =>
            (CurrentGamepad.allControls.Count(
                x =>
                    x is ButtonControl &&
                    x.IsPressed()
            ) > 0);
    }
    public static bool GameDevicePress
    {
        get => (KeyboardPress || MousePress || GamepadPress);
    }

    [SerializeField] private bool dontDestroyOnLoad = true;

    private void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (!MouseConnected)
        {
            return;
        }

        Vector2 screenCenter = 0.5f * new Vector2(
            Screen.width,
            Screen.height
        );

        CurrentMouse.WarpCursorPosition(screenCenter);

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
