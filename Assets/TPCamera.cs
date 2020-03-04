using UnityEngine;
using System.Runtime.InteropServices;

public class TPCamera : MonoBehaviour
{
	[Tooltip("The GameObject this camera should follow")]
	[SerializeField]
	private GameObject objectToFollow;
	private Transform focus;

	[Range(5.0f, 100.0f)]
	[SerializeField]
	private float maxRange = 10.0f;
	private float currentRange;

	private CursorLockMode oldLockMode;
	private Vector2 oldMousePos;
	private Vector3 offset;
	private Quaternion cameraRot;
	private Quaternion focusRot;

	void Start()
	{
		focus = new GameObject("CameraFocus").transform;
		focus.position = objectToFollow.transform.position;
		focus.parent = objectToFollow.transform;

		offset = new Vector3();
		currentRange = maxRange;
		cameraRot = transform.localRotation;
		focusRot = focus.localRotation;
	}

	void LateUpdate()
	{
		if (Input.mouseScrollDelta.y != 0.0f)
		{
			currentRange -= Input.mouseScrollDelta.y;
			currentRange = Mathf.Clamp(currentRange, 0, maxRange);
		}


		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			oldMousePos = Input.mousePosition;
			oldLockMode = Cursor.lockState;
			Cursor.lockState = CursorLockMode.Locked;
		}

		if (Input.GetKey(KeyCode.Mouse0))
		{
			float xRot = Input.GetAxis("Mouse Y");
			float yRot = Input.GetAxis("Mouse X");

			cameraRot *= Quaternion.Euler(-xRot, 0F, 0F);
			cameraRot = ClampRotationAroundXAxis(cameraRot);
			focusRot *= Quaternion.Euler(0F, yRot, 0F);

		}

		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			Cursor.lockState = oldLockMode;
		}

		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			oldMousePos = Input.mousePosition;
			oldLockMode = Cursor.lockState;
			Cursor.lockState = CursorLockMode.Locked;
		}

		if (Input.GetKey(KeyCode.Mouse1))
		{
			float xRot = Input.GetAxis("Mouse Y");
			float yRot = Input.GetAxis("Mouse X");

			cameraRot *= Quaternion.Euler(-xRot, 0F, 0F);
			cameraRot = ClampRotationAroundXAxis(cameraRot);
			focusRot *= Quaternion.Euler(0F, yRot, 0F);
		}

		if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			Cursor.lockState = oldLockMode;
		}

		transform.localRotation = cameraRot;
		focus.localRotation = focusRot;

		// Calculate offset from focus to camera. Set position and rotation
		offset = new Vector3(0, 0, -currentRange);
		offset = transform.rotation * offset;
		offset = focus.rotation * offset;
		transform.position = offset + focus.position;
		transform.rotation = Quaternion.LookRotation(focus.position - transform.position);


		if (Input.GetKey(KeyCode.Mouse1))
		{
			objectToFollow.transform.rotation = Quaternion.Euler(0,focus.rotation.eulerAngles.y, 0);
			focus.localRotation = Quaternion.identity;
			focusRot = Quaternion.identity;
		}
	}

	public float MinimumX = -80;
	public float MaximumX = 80;
	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
		angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
}
