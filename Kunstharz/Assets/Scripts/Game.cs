using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		private List<Player> players = new List<Player> ();

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

		private Player nonLocalPlayer {
			get {
				foreach (var player in players) {
					if (!player.isLocalPlayer) {
						return player;
					}
				}
				return null;
			}
		}
			
		void PlayerStateChanged(Player changedPlayer) {
			if (localPlayer.state == PlayerState.SelectedMotion &&
			    (nonLocalPlayer.state == PlayerState.SelectedMotion || nonLocalPlayer.state == PlayerState.ExecutingMotion)) {

				localPlayer.CmdSetState (PlayerState.ExecutingMotion);

				players [0].GetComponent<Motion> ().enabled = true;
				players [1].GetComponent<Motion> ().enabled = true;

			} else if (localPlayer.state == PlayerState.ExecutedMotion && nonLocalPlayer.state != PlayerState.ExecutingMotion) {
				if (LineOfSightExists ()) {
					// Action mode!
					localPlayer.CmdSetState (PlayerState.SelectingShot);
					GameObject.Find ("Crosshair").GetComponent<Crosshair> ().mode = Crosshair.CrosshairMode.ShotSelection;
				} else {
					// Next turn!
					localPlayer.CmdSetState (PlayerState.SelectingMotion);
				}
			} else if (nonLocalPlayer.state == PlayerState.Victorious) {
				localPlayer.CmdSetState (PlayerState.Dead);
			} else if (nonLocalPlayer.state == PlayerState.Dead) {
				localPlayer.CmdSetState (PlayerState.Victorious);
			}

			if (changedPlayer.isLocalPlayer) {
				var go = GameObject.Find ("OwnState");
				if (go) {
					go.GetComponent<Text> ().text = ""+changedPlayer.state;
				}
			}
		}

		void Start() {
			enabled = false;
		}

		IEnumerator StartNextRoundLater() {
			yield return new WaitForSeconds (5.0f);
			localPlayer.CmdRespawn ();
			GameObject.Find ("Crosshair").GetComponent<Crosshair> ().mode = Crosshair.CrosshairMode.MotionSelection;
		}

		void Update() {
			if (localPlayer.state == PlayerState.ExecutingShot) {
				localPlayer.CmdSetState (PlayerState.SelectingShot);
			}

			if ((localPlayer.state == PlayerState.Victorious || localPlayer.state == PlayerState.Dead) &&
				(nonLocalPlayer.state == PlayerState.Victorious || nonLocalPlayer.state == PlayerState.Dead)) {

				StartCoroutine ("StartNextRoundLater");
			}
		}

		// Called once when all players first in scene together
		void StartGame() {
			localPlayer.CmdSetState (PlayerState.SelectingMotion);
			enabled = true;
		}

		void PlayerJoined(Player player) {
			int newPlayerIdx = players.Count;
			players.Add (player);

			if (player.isLocalPlayer) {
				GiveCameraToPlayer (player);
			}

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

		/*void MotionFinished (Player movedPlayer) {
			if (players.Count == 1) {
				// If only one player yet, move about as you like and skip round logic
				localPlayer.GetComponentInChildren<Controls> ().enabled = true;
				return;
			}

			if (movedPlayer.isLocalPlayer) {
				movedPlayer.CmdSetState(PlayerState.ExecutedMotion);
			}
		}*/

		bool LineOfSightExists() {
			if (players.Count != 2) { return false; }

			Player p1 = players [0];
			Player p2 = players [1];

			Vector3 p1Pos = p1.transform.position + p1.transform.forward * p1.GetComponent<BoxCollider> ().size.z * 0.7f;
			Vector3 p2Pos = p2.transform.position + p2.transform.forward * p2.GetComponent<BoxCollider> ().size.z * 0.7f;

			Vector3 dir = p2Pos - p1Pos;
			Vector3 start = p1Pos;
			RaycastHit hit;

			if (Physics.Raycast (start, dir, out hit, dir.magnitude) && hit.collider.GetComponent<Player> () == p2) {
				print ("Other player is in line of sight");
				Debug.DrawRay (start, dir, Color.red);
				return true;
			} else {
				Debug.DrawRay (start, dir, Color.blue);
				return false;
			}
		}
	}
}

