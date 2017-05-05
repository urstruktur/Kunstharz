using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Controls : MonoBehaviour {
		
		public float rotationSensitivity = 10.0f;
		public float shotCooldown = 0.5f;

		private float leftRightAngle = 0;
		private float topDownAngle = 0;
		private float remainingShotCooldown = 0.0f;

		private Transform ghostTransform;
		private PlayerState state;

		void Start () {
			Cursor.lockState = CursorLockMode.Locked;
			ghostTransform = GameObject.Find ("GhostPlayer").transform;
			ghostTransform.gameObject.SetActive (false);

			enabled = false;
		}

		void Update () {
			if (remainingShotCooldown > 0) {
				remainingShotCooldown -= Time.deltaTime;
			}


			Player player = transform.parent.GetComponent<Player> ();
			state = player.state;

			bool canChooseRotation = state == PlayerState.SelectingMotion ||
			                         state == PlayerState.SelectedMotion ||
			                         state == PlayerState.SelectingShot;

			bool canSelectTarget = canChooseRotation && remainingShotCooldown <= 0;

			if (canChooseRotation) {
				HandleRotationInput ();
			}

			if (canSelectTarget) {
				HandleTargetInput ();
			}

			ghostTransform.gameObject.SetActive (state == PlayerState.SelectedMotion ||
			                                     state == PlayerState.ExecutingMotion);
		}

		void HandleRotationInput () {
			float horizontal = Input.GetAxis ("Mouse X");
			float vertical = Input.GetAxis ("Mouse Y");

			leftRightAngle = Mathf.MoveTowardsAngle (leftRightAngle, leftRightAngle + horizontal * rotationSensitivity, 361.0f);
			topDownAngle = Mathf.Clamp (topDownAngle - vertical * rotationSensitivity, -10.0f, 170.0f);

			Quaternion leftRightRotation = Quaternion.AngleAxis (leftRightAngle, Vector3.forward);
			Quaternion topDownRotation = Quaternion.AngleAxis (topDownAngle, Vector3.right);

			transform.localRotation = leftRightRotation * topDownRotation;
		}

		void HandleTargetInput () {
			if (Input.GetMouseButtonDown (0)) {
				TrySelectTarget ();
			}
		}

		void TrySelectTarget () {
			if (state == PlayerState.SelectingShot) {
				remainingShotCooldown = shotCooldown;
			}

			RaycastHit hit;
			if (Physics.Raycast (transform.position + 0.1f*transform.forward, transform.forward, out hit)) {
				Target target;
				target.position = hit.point;
				target.normal = hit.normal;

				SendMessageUpwards ("SelectedTarget", target);

				if (hit.collider.CompareTag ("Player")) {
					SendMessageUpwards ("HitPlayer", hit.collider.GetComponent<Player> ());
				} else {
					if (state == PlayerState.SelectingMotion || state == PlayerState.SelectedMotion) {
						ghostTransform.transform.position = hit.point;
						ghostTransform.transform.up = hit.normal;
					}
				}
			}
		}

	}
}
