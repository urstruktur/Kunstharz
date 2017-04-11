using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

		private float leftRightAngle = 0;
		private float topDownAngle = 0;

		void Start () {
			Cursor.lockState = CursorLockMode.Locked;
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
				if (-remainingFlyDuration > afterFlyIdleTime) {
					state = flyTargetState;
					SendMessageUpwards ("TurnFinished");
				}
			}
		}

		void HandleInput () {

			if (state == ControlState.DecidingTurn ||
			    state == ControlState.Twitch ||
			    state == ControlState.FinishedTurn) {
				HandleRotationInput ();
			}

			if (state == ControlState.DecidingTurn || state == ControlState.Twitch) {
				HandleFlyInput ();
			}
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
                    flyStartPosition = transform.parent.position;
                    flyTargetPosition = hit.point;

                    flyStartOrientation = transform.parent.rotation;
                    flyTargetOrientation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

                    flyStartOrientationCam = transform.localRotation;

                    //leftRightAngle = -leftRightAngle;
                    topDownAngle = 10;

                    Quaternion leftRightRotation = Quaternion.AngleAxis(leftRightAngle, Vector3.forward);
                    Quaternion topDownRotation = Quaternion.AngleAxis(topDownAngle, Vector3.right);

                    flyTargetOrientationCam = leftRightRotation * topDownRotation;

                    float flyDistance = Vector3.Distance(flyTargetPosition, flyStartPosition);
                    remainingFlyDuration = flyDuration = flyDistance / flyVelocity;

                    flyTargetState = (state == ControlState.DecidingTurn) ? ControlState.FinishedTurn : state;

                    state = ControlState.ExecutingTurn;
                }
			}
		}

		public Color c1 = Color.yellow;
		public Color c2 = Color.red;

		void CalculateHit() {
			
			RaycastHit hit;

			if (Physics.Raycast (transform.position + 0.1f*transform.forward, transform.forward, out hit)) {
				if (hit.collider.transform.parent.CompareTag ("Player")) {
					Player player = transform.parent.GetComponent(typeof(Player)) as Player;
					player.CmdShot (hit.collider.transform.parent.name);
				}
			}

			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();

			if (lineRenderer == null) {
				lineRenderer = gameObject.AddComponent<LineRenderer>();
			}

			lineRenderer.material = Resources.Load("Materials/TestMaterial.mat", typeof(Material)) as Material;
			lineRenderer.widthMultiplier = 0.2f;
			lineRenderer.positionCount = 2;

			lineRenderer.SetPosition(0, (transform.up * - 0.5f) + transform.position);

			if (hit.collider != null) {
				lineRenderer.SetPosition(1, hit.point);
			} else {
				lineRenderer.SetPosition(1, transform.forward * 20 + transform.position);
			}	

		}

	}
}
