using UnityEngine;
using UnityEngine.InputSystem;
 
// Top-down shooter camera that follows the player.
//   Q / E         rotate the camera around the player
//   Scroll wheel  tilt up / down (between minPitch and maxPitch)
// The mouse cursor is left free for aiming.
// Uses the new Input System. Attach to the Main Camera and drag the player into Player.
public class CameraScript : MonoBehaviour
{
    public Transform player;
 
    [Header("Position")]
    [Tooltip("How far the camera stays from the player.")]
    public float distance = 22f;
    [Tooltip("Starting angle (read once when Play starts). X = tilt down toward the ground, Y = facing direction.")]
    public Vector3 rotation = new Vector3(60f, 0f, 0f);
    [Tooltip("How far ahead of the player the camera aims. Higher = player sits lower on screen. 0 = centered.")]
    public float lookAhead = 6f;
 
    [Header("Rotation")]
    [Tooltip("Q/E rotation speed in degrees per second.")]
    public float rotateSpeed = 90f;
 
    [Header("Tilt")]
    [Tooltip("How much one scroll-wheel notch tilts the camera, in degrees.")]
    public float scrollTiltAmount = 5f;
    [Tooltip("Floor: the lowest the camera can go (smaller = closer to the ground, looking ahead).")]
    public float minPitch = 25f;
    [Tooltip("Upper limit: the highest the camera can go (90 = straight down).")]
    public float maxPitch = 85f;
 
    float yaw;     // left/right angle
    float pitch;   // up/down angle
 
    void Start()
    {
        yaw = rotation.y;
        pitch = Mathf.Clamp(rotation.x, minPitch, maxPitch);
    }
 
    void LateUpdate()
    {
        if (player == null) return;
 
        // ---------- Q / E rotation ----------
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.qKey.isPressed) yaw -= rotateSpeed * Time.deltaTime;   // turn left
            if (keyboard.eKey.isPressed) yaw += rotateSpeed * Time.deltaTime;   // turn right
        }
 
        // ---------- Scroll wheel tilt ----------
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll != 0f)
            {
                // Scroll up lowers the camera to look ahead; scroll down raises it overhead.
                pitch -= Mathf.Sign(scroll) * scrollTiltAmount;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
        }
 
        // ---------- Follow ----------
        Quaternion camRotation = Quaternion.Euler(pitch, yaw, 0f);
 
        // Aim at a point in front of the player so the player sits lower on screen.
        Vector3 aheadDirection = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
        Vector3 focusPoint = player.position + aheadDirection * lookAhead;
 
        transform.position = focusPoint - camRotation * Vector3.forward * distance;
        transform.rotation = camRotation;
    }
}