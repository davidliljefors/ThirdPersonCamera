using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField]
	private float speed = 6.0F;

	[SerializeField]
	private float jumpSpeed = 8.0F;

	[SerializeField]
	private float gravity = 20.0F;

	private CharacterController controller;
	private Transform cameraTransform;
	private Vector3 moveDirection = Vector3.zero;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		cameraTransform = Camera.main.transform;
	}
	void Update()
	{
		if (controller.isGrounded)
		{
			if (Input.GetMouseButton(1))
			{ moveDirection = new Vector3(Input.GetAxisRaw("Turn"), 0, Input.GetAxisRaw("Forward")); }
			else
			{ moveDirection = new Vector3(Input.GetAxisRaw("Strafe"), 0, Input.GetAxisRaw("Forward")); }
			
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			if (Input.GetButton("Jump"))
			{ moveDirection.y = jumpSpeed; }

		}

		if (!Input.GetMouseButton(1))
		{ transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Turn"), 0); }

		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}
