using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		public Gui gui;
		public ModularCrosshair crosshair;

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
			if(changedPlayer.isLocalPlayer) {
				if(changedPlayer.state == PlayerState.SelectingShot) {
					crosshair.ShowShootCrosshair();
					gui.ShowShootInstruction(0.5f);
					Camera.main.GetComponent<ImageEffectShockwave> ().actionMode = true;
				} else if(changedPlayer.state == PlayerState.SelectingMotion) {
					gui.ShowMoveInstruction(0.5f);
					crosshair.ShowMoveIdleCrosshair();
				}
			}

			gui.UpdatePlayerStates (GameContext.instance);
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

		void Update() {
			if (localPlayer.state == PlayerState.ExecutingShot) {
				localPlayer.CmdSetState (PlayerState.SelectingShot);
			}
		}

		void PlayerWon() {
			gui.UpdateScore (GameContext.instance);
		}
	}
}

