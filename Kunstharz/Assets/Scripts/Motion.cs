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
		//public float flyVelocity = 3.0f;
		public float afterFlyIdleTime = 0.2f;
		public bool allowMoveDebug;
		public static bool allowMoveDebugStatic;

		private Vector3 flyStartPosition;
		private Vector3 flyTargetPosition;
		private Quaternion flyStartOrientation;
		private Quaternion flyTargetOrientation;
		private float flyDuration = 1.5f;
		private float remainingFlyDuration = float.MinValue;

		void Start() {
			enabled = false;
			allowMoveDebugStatic = allowMoveDebug;
		}
        /*
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
				}
			}
            
    }
    */
   public float FlightDuration(Target target) {
		float flyDistance = Vector3.Distance(transform.position, target.position);
		return flyDuration;
	}
    
        public void DoSetFlyTarget(Target target) {
			flyStartPosition = transform.position;
			flyTargetPosition = target.position;
  
			flyStartOrientation = transform.rotation;
			flyTargetOrientation = Quaternion.FromToRotation(Vector3.forward, target.normal);

			float flyDistance = Vector3.Distance(flyTargetPosition, flyStartPosition);
		}
        
        [ClientRpc]
		public void RpcSetFlyTarget(Target target) {
			print("Setting fly target in clientrpc");
			DoSetFlyTarget (target);
		}

		[ClientRpc]
		public void RpcLaunch() {
			print("Launched");
			enabled = true;
            LeanTween.move(this.gameObject, flyTargetPosition, 1.5f).setEase(LeanTweenType.easeOutQuart);
            LeanTween.rotate(this.gameObject, flyTargetOrientation.eulerAngles, 1.5f).setEase(LeanTweenType.easeOutQuart);
		}

	}
}

