using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Controls : MonoBehaviour {
		
		public float shotCooldown = 0.5f;

		Vector2 mouseAbsolute;
		Vector2 smoothMouse;

		public Vector2 sensitivity = new Vector2(3, 3);
		public Vector2 smoothing = new Vector2(2, 2);

		private float leftRightAngle = 0;
		private float topDownAngle = 0;
		private float remainingShotCooldown = 0.0f;

		private Transform ghostTransform;
		private PlayerState state;

		void Start () {
			Cursor.lockState = CursorLockMode.Locked;
			ghostTransform = GameObject.Find ("GhostPlayer").transform;
			ghostTransform.gameObject.SetActive (false);

			//enabled = false;

		}

		void Update () {
			if (remainingShotCooldown > 0) {
				remainingShotCooldown -= Time.deltaTime;
			}


			Player player = (transform.parent != null) ? transform.parent.GetComponent<Player> () : null;
			state = (player != null) ? player.state : PlayerState.SelectingMotion;

			bool canChooseRotation = state == PlayerState.SelectingMotion ||
			                         state == PlayerState.SelectedMotion ||
			                         state == PlayerState.SelectingShot;

			bool canSelectTarget = canChooseRotation && remainingShotCooldown <= 0;


			//canChooseRotation

			if (true) {
				SmoothMove ();
			}

			if (canSelectTarget) {
				HandleTargetInput ();
			}

			ghostTransform.gameObject.SetActive (state == PlayerState.SelectedMotion ||
			                                     state == PlayerState.ExecutingMotion);
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

		void SmoothMove () {

			Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

			// Scale input against the sensitivity setting and multiply that against the smoothing value.
			mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

			// Interpolate mouse movement over time to apply smoothing delta.
			smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
			smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

			mouseAbsolute += smoothMouse;

			mouseAbsolute.y = Mathf.Clamp (-mouseAbsolute.y, -20.0f, 170.0f);

			Quaternion leftRightRotation = Quaternion.AngleAxis (mouseAbsolute.x, Vector3.forward);
			Quaternion topDownRotation = Quaternion.AngleAxis (mouseAbsolute.y, Vector3.right);

			transform.localRotation = leftRightRotation * topDownRotation;

			mouseAbsolute.y = -mouseAbsolute.y;

		}

	}
}