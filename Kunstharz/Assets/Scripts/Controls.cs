using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

	public float rotationSensitivity = 10.0f;
	public float flyCooldownTime = 0.2f;

	private Camera cam;
	private float remainingFlyCooldown;

	// Use this for initialization
	void Start () {
		cam = GetComponentInChildren<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		HandleRotationInput ();
		HandleFlyInput ();
		Debug.DrawRay (transform.position, transform.forward*1000, Color.green);
	}

	void HandleRotationInput() {
		float horizontal = Input.GetAxis ("Mouse X");
		float vertical = Input.GetAxis ("Mouse Y");

		Vector3 angles = transform.rotation.eulerAngles;
		angles.x += -vertical * rotationSensitivity;
		angles.y += horizontal * rotationSensitivity;
		angles.z = 0.0f;
		transform.rotation = Quaternion.Euler(angles);
	}

	void HandleFlyInput() {
		if (remainingFlyCooldown > 0) {
			remainingFlyCooldown -= Time.deltaTime;
		} else if (Input.GetMouseButton (0)) {
			Fly ();
		}
	}
		

	void Fly() {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			transform.position = hit.point + hit.normal * 2;
			transform.forward = hit.normal;

			remainingFlyCooldown = flyCooldownTime;
		}
	}
}
