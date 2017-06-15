using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Controls : MonoBehaviour {

		Vector2 mouseAbsolute;
		Vector2 smoothMouse;

		public Vector2 sensitivity = new Vector2(3, 3);
		public Vector2 smoothing = new Vector2(2, 2);

		private float leftRightAngle = 0;
		private float topDownAngle = 0;

		private GameContext ctx;

		void Start () {
			Cursor.lockState = CursorLockMode.Locked;
			ctx = GameContext.instance;
		}

		void Update () {
			SmoothMove ();
			HandleTargetInput ();
		}

		void HandleTargetInput () {
			if (Input.GetMouseButtonDown (0)) {
				transform.parent.GetComponent<Player> ().CmdSelected(transform.forward);
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