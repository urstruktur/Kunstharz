using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineMotion : MonoBehaviour {

	public float rotationSensitivity = 10.0f;

	private float leftRightAngle = 0;
	private float topDownAngle = 0;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		HandleRotationInput ();
	}

	void HandleRotationInput () {
		float horizontal = Input.GetAxis ("Mouse X");
		float vertical = Input.GetAxis ("Mouse Y");

		leftRightAngle = Mathf.MoveTowardsAngle (leftRightAngle, leftRightAngle + horizontal * rotationSensitivity, 361.0f);
		topDownAngle = Mathf.Clamp (topDownAngle - vertical * rotationSensitivity, -10.0f, 170.0f);

		Quaternion leftRightRotation = Quaternion.AngleAxis (leftRightAngle, Vector3.forward);
		Quaternion topDownRotation = Quaternion.AngleAxis (topDownAngle, Vector3.right);

		transform.rotation = leftRightRotation * topDownRotation;
	}
}
