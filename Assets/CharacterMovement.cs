using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public Transform camera;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	CharacterController controller;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		camera = Camera.main.transform;
	}
	void Update()
	{
		if (controller.isGrounded)
		{
			moveDirection = new Vector3(Input.GetAxisRaw("Strafe"), 0, Input.GetAxisRaw("Forward"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;

		}
		transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Turn"), 0);
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}
