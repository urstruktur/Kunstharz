using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		public Camera camera;
		public Gui gui;
		public ModularCrosshair crosshair;
		public GameState state = GameState.Preparing;
		public int numRounds = 3;

		private List<Player> players = new List<Player> ();

		public Player localPlayer {
			get {
				foreach (var player in players) {
					if (player.isLocalPlayer) {
						return player;
					}
				}
				return null;
			}
		}

		public Player nonLocalPlayer {
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
				crosshair.ShowMoveIdleCrosshair();

				players [0].GetComponent<Motion> ().enabled = true;
				players [1].GetComponent<Motion> ().enabled = true;

			} else if (localPlayer.state == PlayerState.ExecutedMotion && nonLocalPlayer.state != PlayerState.ExecutingMotion) {
				if (LineOfSightExists ()) {
					// Action mode!
					localPlayer.CmdSetState (PlayerState.SelectingShot);
					crosshair.ShowShootCrosshair();
				} else {
					// Next turn!
					localPlayer.CmdSetState (PlayerState.SelectingMotion);
				}
			} else if (nonLocalPlayer.state == PlayerState.Victorious) {
				localPlayer.CmdSetState (PlayerState.Dead);
			} else if (nonLocalPlayer.state == PlayerState.TimedOut) {
				if (localPlayer.state == PlayerState.SelectingMotion || localPlayer.state == PlayerState.TimedOut) {
					// Both timed out
					print("Both timed out");
					StartCoroutine ("StartNextRoundLater");
				} else {
					// Only opponent timed out
					print("Only opponent timed out");

					if (localPlayer.state != PlayerState.Victorious) {
						localPlayer.CmdSetState (PlayerState.Victorious);
						localPlayer.CmdWon ();
						StartCoroutine ("StartNextRoundLater");
					}
				}
			}

			gui.UpdatePlayerStates (this);
		}

		void Start() {
			enabled = false;

			// DEBUG
			Destroy(GameObject.Find("General"));
			Destroy(GameObject.Find("Cameras"));
			Destroy(GameObject.Find("General Canvas"));
			Destroy(GameObject.Find("Effect Canvas"));
			Destroy(GameObject.Find ("NetworkDiscovery"));
		}

		IEnumerator StartNextRoundLater() {
			state = GameState.BetweenRounds;

			yield return new WaitForSeconds (5.0f);

			enabled = true;
			localPlayer.CmdSetState (PlayerState.SelectingMotion);
			localPlayer.CmdRespawn ();

			crosshair.ShowMoveCrosshair();

			state = GameState.PlayingRound;
		}

		void Update() {
			if (localPlayer.state == PlayerState.ExecutingShot) {
				localPlayer.CmdSetState (PlayerState.SelectingShot);
			}
		}

		// Called once when all players first in scene together
		void StartGame() {
			print ("Game starting");
			localPlayer.CmdSetState (PlayerState.SelectingMotion);
			state = GameState.PlayingRound;
			gui.UpdatePlayerNames(this);
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

		void PlayerWon() {
			int playedRounds = localPlayer.wins + nonLocalPlayer.wins;

			if (playedRounds < numRounds) {
				StartCoroutine ("StartNextRoundLater");
			} else {
				state = GameState.Finished;
			}
				
			gui.UpdateScore (this);
		}

		void GiveCameraToPlayer(Player activePlayer) {
			Transform camTransform = camera.transform;
			Vector3 pos = camTransform.localPosition;
			Quaternion orientation = camTransform.localRotation;

			camTransform.parent = activePlayer.transform;
			camTransform.localPosition = pos;
			camTransform.localRotation = orientation;

			camTransform.GetComponent<Controls> ().enabled = true;
		}

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
				Debug.DrawRay (start, dir, Color.red);
				return true;
			} else {
				Debug.DrawRay (start, dir, Color.blue);
				return false;
			}
		}
	}
}

