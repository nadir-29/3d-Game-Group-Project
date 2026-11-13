using UnityEngine;
using UnityEngine.InputSystem;
 
// Top-down shooter movement (uses the new Input System).
//   WASD   move in screen directions: W = up, S = down, A = left, D = right
//   Mouse  the player turns to face the cursor (aim)
// Moving and aiming are separate, so you can strafe and backpedal while aiming.
// No gravity (flat ground only). Attach to the player (needs a CharacterController).
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Tooltip("The camera. Leave empty to use the Main Camera.")]
    public Camera cam;
 
    public float moveSpeed = 5f;
    [Tooltip("How fast the player turns to face the mouse, in degrees per second.")]
    public float turnSpeed = 1080f;
 
    CharacterController controller;
 
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null)
            cam = Camera.main;
    }
 
    void Update()
    {
        Move();
        FaceMouse();
    }
 
    void Move()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
 
        // Read the WASD keys.
        Vector3 input = Vector3.zero;
        if (keyboard.wKey.isPressed) input.z += 1f;
        if (keyboard.sKey.isPressed) input.z -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;
 
        // Stops diagonal movement from being faster.
        input = input.normalized;
 
        // Match the camera's left/right rotation so W is always "up" on screen.
        if (cam != null)
            input = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * input;
 
        controller.Move(input * moveSpeed * Time.deltaTime);
    }
 
    void FaceMouse()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || cam == null) return;
 
        // Shoot a ray from the camera through the mouse cursor and see where it
        // hits a flat plane at the player's height.
        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        Plane ground = new Plane(Vector3.up, transform.position);
 
        if (ground.Raycast(ray, out float hitDistance))
        {
            Vector3 aimPoint = ray.GetPoint(hitDistance);
            Vector3 lookDirection = aimPoint - transform.position;
            lookDirection.y = 0f;
 
            // Don't turn if the cursor is right on top of the player.
            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                              turnSpeed * Time.deltaTime);
            }
        }
    }
}