using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		private Player[] playerControls;

		void Start () {
			playerControls = GetComponentsInChildren<Player> ();
			GiveCameraToPlayer (0);
		}

		void GiveCameraToPlayer(int idx) {
			var activePlayer = playerControls [idx];

			Transform camTransform = Camera.main.transform;
			Vector3 pos = camTransform.localPosition;
			Quaternion orientation = camTransform.localRotation;

			camTransform.parent = activePlayer.transform;
			camTransform.localPosition = pos;
			camTransform.localRotation = orientation;

			camTransform.GetComponent<Controls> ().state = ControlState.Twitch;
		}

		void TurnFinished () {
		}
	}
}

