using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Controls : MonoBehaviour {
		
		public float rotationSensitivity = 10.0f;
		// If true, will send SetShootTarget instead of SetFlyTarget
		public bool isShootingMode = false;

		private float leftRightAngle = 0;
		private float topDownAngle = 0;

		void Start () {
			Cursor.lockState = CursorLockMode.Locked;
		}

		void Update () {
			HandleRotationInput ();
			HandleFlyInput ();
		}

		void HandleRotationInput () {
			float horizontal = Input.GetAxis ("Mouse X");
			float vertical = Input.GetAxis ("Mouse Y");

			leftRightAngle = Mathf.MoveTowardsAngle (leftRightAngle, leftRightAngle + horizontal * rotationSensitivity, 361.0f);
			topDownAngle = Mathf.Clamp (topDownAngle - vertical * rotationSensitivity, -10.0f, 90.0f + 45.0f);

			Quaternion leftRightRotation = Quaternion.AngleAxis (leftRightAngle, Vector3.forward);
			Quaternion topDownRotation = Quaternion.AngleAxis (topDownAngle, Vector3.right);

			transform.localRotation = leftRightRotation * topDownRotation;
		}

		void HandleFlyInput () {
			if (Input.GetMouseButton (0)) {
				Fly ();
			}

			if (Input.GetMouseButtonDown (1)) {
				Debug.Log ("Shot fired");
				CalculateHit ();
			}
		}

		void Fly () {
			RaycastHit hit;
			if (Physics.Raycast (transform.position + 0.1f*transform.forward, transform.forward, out hit)) {
				if (!hit.collider.transform.parent.CompareTag("Player")) // cant fly onto another player
				{
					Target target;
					target.position = hit.point;
					target.normal = hit.normal;

					SendMessageUpwards (isShootingMode ? "SetShootTarget" : "SetFlyTarget", target);
					enabled = false;
				}
			}
		}

		void CalculateHit() {

			RaycastHit hit;
			Player player = transform.parent.GetComponent(typeof(Player)) as Player;

			if (Physics.Raycast (transform.position + 0.1f*transform.forward, transform.forward, out hit)) {
				if (hit.collider.transform.parent.CompareTag ("Player")) {
					player.CmdShot (hit.collider.transform.parent.name);
				}
			}

			/*GameObject destroy = GameObject.Find ("Shot(Clone)");

			if (destroy != null) {
				player.CmdDestroy (destroy);
			}*/
				
			/*GameObject go = Instantiate(Resources.Load("Prefabs/Shot")) as GameObject;

			LineRenderer lineRenderer = go.GetComponent<LineRenderer> ();

			lineRenderer.SetPosition(0, (transform.up * - 0.5f) + transform.position);

			if (hit.collider != null) {
				lineRenderer.SetPosition(1, hit.point);
			} else {
				lineRenderer.SetPosition(1, transform.forward * 20 + transform.position);
			}

			player.CmdInstantiate (go);*/

		}

	}
}
