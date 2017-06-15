using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		}

		void PlayerWon() {
			gui.UpdateScore (GameContext.instance);
		}
	}
}

