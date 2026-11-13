using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    [Tooltip("How fast the player turns to face its movement direction.")]
    public float turnSpeed = 720f;   // degrees per second
 
    CharacterController controller;
 
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
 
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;   // no keyboard connected
 
        // Read the WASD keys.
        Vector3 input = Vector3.zero;
        if (keyboard.wKey.isPressed) input.z += 1f;
        if (keyboard.sKey.isPressed) input.z -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;
 
        // Stops diagonal movement from being faster.
        input = input.normalized;
 
        // Turn to face the direction of movement.
        if (input != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                          turnSpeed * Time.deltaTime);
        }
 
        controller.Move(input * moveSpeed * Time.deltaTime);
    }
}