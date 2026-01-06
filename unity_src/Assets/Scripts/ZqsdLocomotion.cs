using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Simple ZQSD keyboard locomotion that can be toggled on/off from the inspector.
/// Attach to the camera rig root; set directionSource to the head/camera so movement follows yaw.
/// </summary>
public class ZqsdLocomotion : MonoBehaviour
{
    [Header("Desktop mode")]
    [SerializeField] private bool keyboardModeEnabled = true;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fastMultiplier = 2f;
    [SerializeField] private Transform directionSource;

    /// <summary>
    /// Exposed for UI/events to toggle desktop mode at runtime.
    /// </summary>
    /// <param name="enabled">Enable or disable keyboard locomotion.</param>
    public void EnableKeyboardMode(bool enabled) => keyboardModeEnabled = enabled;

    private void Update()
    {
        if (!keyboardModeEnabled)
        {
            return;
        }

        Vector2 input = ReadMovementInput();
        if (input.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float speed = moveSpeed;
        if (IsFastModifierPressed())
        {
            speed *= fastMultiplier;
        }

        Transform reference = directionSource != null ? directionSource : transform;
        Vector3 forward = reference.forward;
        Vector3 right = reference.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * input.y + right * input.x;
        transform.position += move * (speed * Time.deltaTime);
    }

    private Vector2 ReadMovementInput()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        if (kb == null)
        {
            return Vector2.zero;
        }

        // Use physical WASD keys so AZERTY users naturally get ZQSD.
        float x = (IsPressed(kb, Key.D) ? 1f : 0f) - (IsPressed(kb, Key.A) ? 1f : 0f);
        float y = (IsPressed(kb, Key.W) ? 1f : 0f) - (IsPressed(kb, Key.S) ? 1f : 0f);
        return new Vector2(x, y).normalized;
#else
        float x = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        float y = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        return new Vector2(x, y).normalized;
#endif
    }

    private bool IsFastModifierPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        return kb != null && (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed);
#else
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
    }

#if ENABLE_INPUT_SYSTEM
    private static bool IsPressed(Keyboard kb, Key key) => kb != null && kb[key].isPressed;
#endif
}
