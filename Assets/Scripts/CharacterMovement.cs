using UnityEngine.Assertions;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField]
	private GameObject cameraFocus = null;
	private CharacterController controller = null;
	private float gravity = 20.0f;
	private float walkSpeed = 5.0f;
	private float jumpSpeed = 8.0f;
	Vector3 movement = Vector3.zero;

	//Cursor lock
	private CursorLockMode previousState;
	private bool rotateByMouse = false;
	void Start()
	{
		controller = GetComponent<CharacterController>();
		Assert.IsNotNull(controller);
		Assert.IsNotNull(cameraFocus);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			previousState = Cursor.lockState;
			Cursor.lockState = CursorLockMode.Locked;
			rotateByMouse = true;
		}

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
			Cursor.lockState = previousState;
			rotateByMouse = false;
		}

		if(rotateByMouse)
		{
			Quaternion pitch = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up);
			transform.rotation *= pitch;

            Quaternion yaw = Quaternion.AngleAxis(Input.GetAxis("Mouse Y"), -Vector3.right);
			cameraFocus.transform.rotation *= yaw;
        }


		if (controller.isGrounded)
		{
			movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			movement = transform.TransformDirection(movement);
			movement *= walkSpeed;
			if (Input.GetButton("Jump"))
				movement.y = jumpSpeed;

		}
		movement.y -= gravity * Time.deltaTime;
		controller.Move(movement * Time.deltaTime);
	}
}
