using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
	public struct Target {
		public Vector3 position;
		public Vector3 normal;
	}

	public class Motion : MonoBehaviour
	{
		public float flyVelocity = 3.0f;
		public float afterFlyIdleTime = 0.2f;

		private Vector3 flyStartPosition;
		private Vector3 flyTargetPosition;
		private Quaternion flyStartOrientation;
		private Quaternion flyTargetOrientation;
		private float flyDuration;
		private float remainingFlyDuration;

		void Start() {
			enabled = false;
		}

		void Update () {
			remainingFlyDuration -= Time.deltaTime;

			if (remainingFlyDuration > 0.02) {
				float alpha = (flyDuration - remainingFlyDuration) / flyDuration;
				alpha = Mathf.SmoothStep (0f, 1f, alpha);

				transform.position = Vector3.Lerp (flyStartPosition, flyTargetPosition, alpha);
				transform.rotation = Quaternion.Lerp (flyStartOrientation, flyTargetOrientation, alpha);
			} else {
				transform.position = flyTargetPosition;
				transform.rotation = flyTargetOrientation;

				if (-remainingFlyDuration > afterFlyIdleTime) {
					enabled = false;
					SendMessageUpwards ("MotionFinished");
				}
			}
		}

		void SetFlyTarget(Target target) {
			flyStartPosition = transform.position;
			flyTargetPosition = target.position;

			flyStartOrientation = transform.rotation;
			flyTargetOrientation = Quaternion.FromToRotation(Vector3.forward, target.normal);

			float flyDistance = Vector3.Distance(flyTargetPosition, flyStartPosition);
			remainingFlyDuration = flyDuration = flyDistance / flyVelocity;

			enabled = true;
		}

	}
}

