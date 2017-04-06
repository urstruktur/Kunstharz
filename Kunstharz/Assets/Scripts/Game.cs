using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		private List<Transform> availableSpawnLocs = new List<Transform>();

		void Start() {
			var spawnLocs = GameObject.Find ("SpawnLocations").transform;
			foreach (Transform spawnLocTransform in spawnLocs) {
				availableSpawnLocs.Add (spawnLocTransform);
			}
				
			availableSpawnLocs.Shuffle ();
		}

		void PlayerJoined(Player player) {
			if (player.isLocalPlayer) {
				GiveCameraToPlayer (player);
			}

			var spawnLoc = availableSpawnLocs [0].position;
			var spawnRot = availableSpawnLocs [0].rotation;
			availableSpawnLocs.RemoveAt (0);

			player.transform.position = spawnLoc;
			player.transform.rotation = spawnRot;
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

