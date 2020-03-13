using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6.0F;

    [SerializeField] private float jumpSpeed = 8.0F;

    [SerializeField] private float gravity = 20.0F;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            moveDirection = Input.GetMouseButton(1)
                ? new Vector3(Input.GetAxisRaw("Turn"), 0, Input.GetAxisRaw("Forward"))
                : new Vector3(Input.GetAxisRaw("Strafe"), 0, Input.GetAxisRaw("Forward"));

            if (moveDirection.sqrMagnitude > 1f)
            {
                moveDirection.Normalize();
            }
            moveDirection = transform.rotation * moveDirection;
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        if (!Input.GetMouseButton(1))
        {
            transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Turn"), 0);
        }

        moveDirection.y -= gravity * Time.deltaTime;

        
        controller.Move(moveDirection * Time.deltaTime);
    }
}