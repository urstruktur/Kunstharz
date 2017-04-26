using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		private List<Player> players = new List<Player> ();
		private List<Transform> availableSpawnLocs = new List<Transform>();

		private Player localPlayer {
			get {
				foreach (var player in players) {
					if (player.isLocalPlayer) {
						return player;
					}
				}
				return null;
			}
		}

		void Start() {
			var spawnLocs = GameObject.Find ("SpawnLocations").transform;
			foreach (Transform spawnLocTransform in spawnLocs) {
				availableSpawnLocs.Add (spawnLocTransform);
			}
				
			availableSpawnLocs.Shuffle ();
			enabled = false;
		}

		void Update() {
			if (players [0].state == PlayerState.SelectedMotion &&
			    players [1].state == PlayerState.SelectedMotion) {

				players [0].state = PlayerState.ExecutingMotion;
				players [1].state = PlayerState.ExecutingMotion;

				players [0].GetComponent<Motion> ().enabled = true;
				players [1].GetComponent<Motion> ().enabled = true;
			}
		}

		void StartGame() {
			print ("All players here, starting game.");
			enabled = true;
		}

		void PlayerJoined(Player player) {
			int newPlayerIdx = players.Count;
			players.Add (player);

			if (player.isLocalPlayer) {
				GiveCameraToPlayer (player);
			}

			var spawnLoc = availableSpawnLocs [0].position;
			var spawnRot = availableSpawnLocs [0].rotation;
			availableSpawnLocs.RemoveAt (0);

			player.transform.position = spawnLoc;
			player.transform.rotation = spawnRot;

			if (players.Count == 2) {
				// All players are spawned, start the actual game
				StartGame ();
			}
		}

		void GiveCameraToPlayer(Player activePlayer) {
			Transform camTransform = Camera.main.transform;
			Vector3 pos = camTransform.localPosition;
			Quaternion orientation = camTransform.localRotation;

			camTransform.parent = activePlayer.transform;
			camTransform.localPosition = pos;
			camTransform.localRotation = orientation;

			camTransform.GetComponent<Controls> ().enabled = true;
		}

		void MotionFinished (Player movedPlayer) {
			if (players.Count == 1) {
				// If only one player yet, move about as you like and skip round logic
				localPlayer.GetComponentInChildren<Controls> ().enabled = true;
				return;
			}

			movedPlayer.state = PlayerState.ExecutedMotion;

			if (players [0].state == PlayerState.ExecutedMotion && players [1].state == PlayerState.ExecutedMotion) {
				if (LineOfSightExists ()) {
					// Action mode!
					players [0].state = PlayerState.SelectingShot;
					players [1].state = PlayerState.SelectingShot;
				} else {
					// Next turn!
					players [0].state = PlayerState.SelectingMotion;
					players [1].state = PlayerState.SelectingMotion;
				}
			}
		}

		bool LineOfSightExists() {
			if (players.Count != 2) { return false; }

			Player p1 = players [0];
			Player p2 = players [1];
			Vector3 dir = p2.transform.position - p1.transform.position;
			RaycastHit hit;

			if (Physics.Raycast (p1.transform.position, dir, out hit, dir.magnitude)) {
				if (hit.collider.GetComponent<Player> () == p2) {
					print ("Other player is in line of sight");
					return true;
				}
			}

			return false;
		}
	}
}

