using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

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

	[Range(5f, 100f)]
	[SerializeField]
	private float lerpSpeed = 10f;

	private CursorLockMode oldLockMode;
	private Vector2 oldMousePos;
	private Vector3 offset;
	private Quaternion cameraRot;
	private Quaternion focusRot;
	private bool useTargetRotation = true;
	private Coroutine resetRotationRoutine;



	void Start()
	{
		focus = new GameObject("CameraFocus").transform;
		focus.position = objectToFollow.transform.position;
		offset = new Vector3();

		currentRange = maxRange;
		cameraRot = transform.localRotation;
		focusRot = focus.localRotation;
	}
	void LateUpdate()
	{
		focus.position = objectToFollow.transform.position;

		if (Input.mouseScrollDelta.y != 0.0f)
		{
			currentRange -= Input.mouseScrollDelta.y;
			currentRange = Mathf.Clamp(currentRange, 0.1F, maxRange);
		}


		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			useTargetRotation = false;
			oldMousePos = Input.mousePosition;
			oldLockMode = Cursor.lockState;
			Cursor.lockState = CursorLockMode.Locked;
			if (resetRotationRoutine != null)
			{
				StopCoroutine(resetRotationRoutine);
				resetRotationRoutine = null;
			}
		}

		if (Input.GetKey(KeyCode.Mouse0))
		{
			float xRot = Input.GetAxis("Mouse Y");
			float yRot = Input.GetAxis("Mouse X");

			cameraRot *= Quaternion.Euler(-xRot, 0F, 0F);
			cameraRot = ClampRotationAroundXAxis(cameraRot);
			focusRot *= Quaternion.Euler(0F, yRot, 0F);
			focus.rotation = focusRot;
		}

		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			resetRotationRoutine = StartCoroutine(EnableCameraFollowRotation());
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
			focus.rotation = focusRot;
			objectToFollow.transform.rotation = Quaternion.Euler(0, focus.rotation.eulerAngles.y, 0);
		}

		if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			Cursor.lockState = oldLockMode;
		}

		transform.rotation = cameraRot;

		if (useTargetRotation)
		{
			focus.rotation = objectToFollow.transform.rotation;
		}
		// Calculate offset from focus to camera. Set position and rotation
		offset = new Vector3(0, 0, -currentRange);
		offset = transform.rotation * offset;
		offset = focus.rotation * offset;


		if (Physics.Linecast(focus.position, offset + focus.position, out RaycastHit hit))
		{
			transform.position = hit.point;
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, offset + focus.position, lerpSpeed * Time.deltaTime);
		}

		transform.rotation = Quaternion.LookRotation(focus.position - transform.position);
		focusRot = focus.rotation;
	}

	public float MinimumX = -0;
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

	private void OnDrawGizmos()
	{
		if (focus != null)
		{
			Gizmos.DrawLine(focus.position, focus.position + offset);
			Gizmos.DrawWireSphere(focus.position, 0.5F);
		}
	}
	IEnumerator EnableCameraFollowRotation()
	{
		yield return new WaitForSeconds(2F);
		useTargetRotation = true;
		resetRotationRoutine = null;
	}
}
