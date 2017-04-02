using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz {
	public class Controls : MonoBehaviour {

		public float flyVelocity = 50.0f;
		public float rotationSensitivity = 10.0f;
		public ControlState state = ControlState.DecidingTurn;

		private ControlState flyTargetState;
		private Vector3 flyStartPosition;
		private Vector3 flyTargetPosition;
		private Quaternion flyStartOrientation;
		private Quaternion flyTargetOrientation;
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

			if (remainingFlyDuration < 0.02) {
				remainingFlyDuration = 0;
				state = flyTargetState;
			}

			float alpha = (flyDuration - remainingFlyDuration) / flyDuration;
			alpha = Mathf.SmoothStep (0f, 1f, alpha);

			transform.position = Vector3.Lerp (flyStartPosition, flyTargetPosition, alpha);
			transform.rotation = Quaternion.Lerp (flyStartOrientation, flyTargetOrientation, alpha);
		}

		void HandleInput () {
			HandleRotationInput ();

			if (state == ControlState.DecidingTurn || state == ControlState.Twitch) {
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
			transform.rotation = Quaternion.Euler(angles);
		}

		void HandleFlyInput () {
			if (Input.GetMouseButton (0)) {
				Fly ();
			}
		}
			

		void Fly () {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit)) {
				flyStartPosition = transform.position;
				flyTargetPosition = hit.point;

				flyStartOrientation = transform.rotation;
				flyTargetOrientation = Quaternion.FromToRotation (Vector3.forward, hit.normal);

				float flyDistance = Vector3.Distance (flyTargetPosition, flyStartPosition);
				remainingFlyDuration = flyDuration = flyDistance / flyVelocity;

				flyTargetState = (state == ControlState.DecidingTurn) ? ControlState.FinishedTurn : state;

				state = ControlState.ExecutingTurn;
			}
		}
	}
}
