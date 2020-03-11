using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Tooltip("The GameObject this camera should follow")]
	[SerializeField]
	private GameObject parent = default;

	[Tooltip("Position offset from parent")]
	[SerializeField]
	private Vector3 offset = default;

	[Tooltip("Position that the camera lerps towards each frame")]
	private Vector3 targetPosition;

	[Range(0.0F, 1.0F)]
	[SerializeField]
	private float lerpSpeed = 0.1f;
	private float maxDistanceToTarget;


	void Start()
	{
		maxDistanceToTarget = Vector3.Distance(Vector3.zero, offset);
		transform.position = parent.transform.position + offset;
		transform.rotation = parent.transform.rotation;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		transform.rotation = Quaternion.LookRotation(parent.transform.position - transform.position, Vector3.up);

		targetPosition = parent.transform.position + (Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f) * offset);

		RaycastHit hit;
		if (Physics.Raycast(parent.transform.position, targetPosition - parent.transform.position,out hit, maxDistanceToTarget))
		{
			targetPosition = hit.point;
		}

		transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);
	}
}
