using System.Collections;
using UnityEngine;

public class TPCamera : MonoBehaviour
{
	#region Serialized Variables
	[Tooltip("The GameObject this camera should follow")]
	[SerializeField]
	private GameObject objectToFollow = default;

	[Range(5f, 100f)]
	[SerializeField]
	private float maxRange = 10.0f;

	[Range(0.01f, 10f)]
	[SerializeField]
	private float minRange = 0.1f;

	[Range(5f, 50f)]
	[SerializeField]
	private float lerpSpeed = 10f;

	[Range(0f, -90f)]
	[SerializeField]
	private float minX = -90f;

	[Range(0f, 90f)]
	[SerializeField]
	private float maxX = 90f;

	[Tooltip("The time until camera resets to behind after user let go of LMB")]
	[SerializeField]
	private float resetTime = 2f;

	[Tooltip("Size of the sphere for collision check with obstacles")]
	[SerializeField]
	private float sphereSize = 0.25f;

	[SerializeField]
	private string axisYName = "Mouse Y";
	[SerializeField]
	private string axisXName = "Mouse X";
	#endregion Serialized Variables

	#region Private Variables
	private float currentRange;
	private Transform focus = default;
	private Quaternion cameraRot;
	private Quaternion focusRot;
	private Vector3 offset;
	private CursorLockMode oldLockMode;
	private bool useTargetRotation = true;

	private Coroutine resetRotationRoutine;
	#endregion Private Variables

	private void Start()
	{
		focus = new GameObject("CameraFocus").transform;
		focus.position = objectToFollow.transform.position;
		offset = new Vector3();

		currentRange = maxRange;
		cameraRot = transform.rotation;
		focusRot = focus.rotation;
	}

	private void LateUpdate()
	{
		focus.position = objectToFollow.transform.position;

		HandleInput();
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		transform.rotation = cameraRot;

		if (useTargetRotation)
		{
			focus.rotation = Quaternion.Lerp(focus.rotation, objectToFollow.transform.rotation, Time.deltaTime * lerpSpeed);
		}

		offset.x = 0;
		offset.y = 0;
		offset.z = -currentRange;

		offset = transform.rotation * offset;
		offset = focus.rotation * offset;


		// Cast a sphere towards camera position. If a hit occurs, 
		// calculate length of that vector in direction of camera and change offset length to that instead

		if (Physics.SphereCast(focus.position, sphereSize, transform.position - focus.position, out RaycastHit hit, currentRange))
		{
			float vectorLength = Mathf.Sqrt(Vector3.Dot((hit.point - focus.position), (transform.position - focus.position)));
			offset = offset.normalized * vectorLength;
		}


		transform.position = offset + focus.position;
		transform.rotation = Quaternion.LookRotation(focus.position - transform.position);
		focusRot = focus.rotation;
	}

	private void HandleInput()
	{
		currentRange -= Input.mouseScrollDelta.y;
		currentRange = Mathf.Clamp(currentRange, minRange, maxRange);

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			useTargetRotation = false;
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
			float xRot = Input.GetAxis(axisYName);
			float yRot = Input.GetAxis(axisXName);

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
			useTargetRotation = true;
			oldLockMode = Cursor.lockState;
			Cursor.lockState = CursorLockMode.Locked;
			if (resetRotationRoutine != null)
			{
				StopCoroutine(resetRotationRoutine);
				resetRotationRoutine = null;
			}
		}

		if (Input.GetKey(KeyCode.Mouse1))
		{
			float xRot = Input.GetAxis(axisYName);
			float yRot = Input.GetAxis(axisXName);

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
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
		angleX = Mathf.Clamp(angleX, minX, maxX);
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
		yield return new WaitForSeconds(resetTime);
		useTargetRotation = true;
		resetRotationRoutine = null;
	}
}
