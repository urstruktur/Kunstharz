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

		bool canMove = false;

		void Start () {
			#if UNITY_EDITOR
			Cursor.lockState = CursorLockMode.Locked;
			#endif
			mouseAbsolute = ToNegativeAngle (new Vector2(transform.eulerAngles.y, transform.eulerAngles.x));
			LeanTween.delayedCall(1f, CanMove);
		}

		Vector2 ToNegativeAngle(Vector2 angleTotal) {
			return new Vector3(ToPositive(angleTotal.x), ToPositive(angleTotal.y));
		}

		private void CanMove(){
			canMove = true;
		}

		float ToPositive(float number) {
			if (number > 180) return number - 360f;
			return number;
		}

		void Update () {
			if (canMove) SmoothMove ();
			HandleTargetInput ();
		}

		void HandleTargetInput () {
			if (Input.GetMouseButtonDown (0)) {
				transform.parent.GetComponent<Player> ().CmdSelected(transform.forward);
			}
		}

		void SmoothMove () {

			Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

			mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

			smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
			smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

			mouseAbsolute.x += smoothMouse.x;
			mouseAbsolute.y -= smoothMouse.y;

			mouseAbsolute.y = Mathf.Clamp (mouseAbsolute.y, -80.0f, 80.0f);

			Quaternion leftRightRotation = Quaternion.AngleAxis (mouseAbsolute.x, Vector3.up);
			Quaternion topDownRotation = Quaternion.AngleAxis (mouseAbsolute.y, Vector3.right);

			transform.localRotation = leftRightRotation * topDownRotation;

		}

	}
}