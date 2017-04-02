using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

	public float rotationSensitivity = 10.0f;

	private Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponentInChildren<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Mouse X");
		float vertical = Input.GetAxis ("Mouse Y");

		print (horizontal);

		Vector3 angles = transform.rotation.eulerAngles;
		angles.x += -vertical * rotationSensitivity;
		angles.y += horizontal * rotationSensitivity;
		angles.z = 0.0f;
		transform.rotation = Quaternion.Euler(angles);

		//cam.transform.Translate (new Vector3 (0f, 0f, -0.01f));
	}
}
