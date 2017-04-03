using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz {
	public class Controls : MonoBehaviour {

		public float flyVelocity = 50.0f;
		public float rotationSensitivity = 10.0f;
		public float afterFlyIdleTime = 0.2f;
		public ControlState state = ControlState.DecidingTurn;

		private ControlState flyTargetState;
		private Vector3 flyStartPosition;
		private Vector3 flyTargetPosition;
		private Quaternion flyStartOrientation;
		private Quaternion flyTargetOrientation;
		private Quaternion flyStartOrientationCam;
		private Quaternion flyTargetOrientationCam;
		private float flyDuration;
		private float remainingFlyDuration;

		void Start () {
			Cursor.visible = false;
		}

		void Update () {
			if (state == ControlState.ExecutingTurn) {
				ExecuteTurn ();
			} else {
				HandleInput ();
			}
		}

		void ExecuteTurn () {
			remainingFlyDuration -= Time.deltaTime;

			if (remainingFlyDuration > 0.02) {
				float alpha = (flyDuration - remainingFlyDuration) / flyDuration;
				alpha = Mathf.SmoothStep (0f, 1f, alpha);

				transform.parent.position = Vector3.Lerp (flyStartPosition, flyTargetPosition, alpha);
				transform.parent.rotation = Quaternion.Lerp (flyStartOrientation, flyTargetOrientation, alpha);
				transform.localRotation = Quaternion.Lerp (flyStartOrientationCam, flyTargetOrientationCam, alpha);

				Input.GetAxis ("Mouse X");
				Input.GetAxis ("Mouse Y");
			} else {
				//Debug.Break ();
				//transform.parent.position = flyTargetPosition;
				//transform.parent.rotation = flyTargetOrientation;

				if (-remainingFlyDuration > afterFlyIdleTime) {
					state = flyTargetState;
					//SendMessageUpwards ("TurnFinished");
				}
			}
		}

		void HandleInput () {
			if (state == ControlState.DecidingTurn || state == ControlState.Twitch) {
				HandleRotationInput ();
				HandleFlyInput ();
			}
		}

		void HandleRotationInput () {
			float horizontal = Input.GetAxis ("Mouse X");
			float vertical = Input.GetAxis ("Mouse Y");

			Vector3 angles = transform.rotation.eulerAngles;
			angles.x += -vertical * rotationSensitivity;
			angles.y += horizontal * rotationSensitivity;
			angles.z = 0.0f;

			//angles.x = Mathf.Clamp (angles.x, -92, 0); 
			//angles.y = Mathf.Clamp (angles.y, -90, 90); 

			transform.rotation = Quaternion.Euler(angles);
			//GetComponent<Rigidbody> ().MoveRotation(Quaternion.Euler(angles));

			//rotation = Quaternion.Euler(angles);
		}

		void HandleFlyInput () {
			if (Input.GetMouseButton (0)) {
				Fly ();
			}
		}
			

		void Fly () {
			RaycastHit hit;
			if (Physics.Raycast (transform.position + 0.1f*transform.forward, transform.forward, out hit)) {
				flyStartPosition = transform.parent.position;
				flyTargetPosition = hit.point;

				flyStartOrientation = transform.parent.rotation;
				flyTargetOrientation = Quaternion.FromToRotation (Vector3.forward, hit.normal);

				flyStartOrientationCam = transform.localRotation;
				flyTargetOrientationCam = Quaternion.identity;

				float flyDistance = Vector3.Distance (flyTargetPosition, flyStartPosition);
				remainingFlyDuration = flyDuration = flyDistance / flyVelocity;

				flyTargetState = (state == ControlState.DecidingTurn) ? ControlState.FinishedTurn : state;

				state = ControlState.ExecutingTurn;
			}
		}
	}
}
