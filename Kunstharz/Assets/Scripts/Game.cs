using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		void PlayerJoined(Player player) {
			GiveCameraToPlayer (player);
		}

		void GiveCameraToPlayer(Player activePlayer) {
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

