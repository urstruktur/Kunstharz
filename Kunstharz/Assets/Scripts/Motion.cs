using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public struct Target {
		public Vector3 position;
		public Vector3 normal;
	}

	public class Motion : NetworkBehaviour
	{
		public float flyVelocity = 3.0f;
		public float afterFlyIdleTime = 0.2f;
		public bool allowMoveDebug;
		public static bool allowMoveDebugStatic;

		[SyncVar]
		private Vector3 flyStartPosition;
		[SyncVar]
		private Vector3 flyTargetPosition;
		[SyncVar]
		private Quaternion flyStartOrientation;
		[SyncVar]
		private Quaternion flyTargetOrientation;
		[SyncVar]
		private float flyDuration;
		private float remainingFlyDuration = float.MinValue;

		void Start() {

			enabled = false;
			allowMoveDebugStatic = allowMoveDebug;
		}

		void Update () {
			if (remainingFlyDuration == float.MinValue) {
				remainingFlyDuration = flyDuration;
			}

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
					remainingFlyDuration = float.MinValue;
					enabled = false;
					SendMessageUpwards ("MotionFinished", GetComponent<Player> ());
				}
			}
		}

		void DoSetFlyTarget(Target target) {
			flyStartPosition = transform.position;
			flyTargetPosition = target.position;

			flyStartOrientation = transform.rotation;
			flyTargetOrientation = Quaternion.FromToRotation(Vector3.forward, target.normal);

			float flyDistance = Vector3.Distance(flyTargetPosition, flyStartPosition);
			flyDuration = flyDistance / flyVelocity;
		}

		[Command]
		void CmdSetFlyTarget(Target target) {
			DoSetFlyTarget (target);
		}

		void SetFlyTarget(Target target) {
			if (allowMoveDebugStatic) {
				DoSetFlyTarget (target);
			} else {
				CmdSetFlyTarget (target);
			}
		}

	}
}

