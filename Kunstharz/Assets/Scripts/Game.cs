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
			
		void PlayerStateChanged(Player changedPlayer) {
			if(changedPlayer.isLocalPlayer) {
				if(changedPlayer.state == PlayerState.SelectingShot) {
					Camera.main.GetComponent<ImageEffectShockwave> ().actionMode = true;
				} else if(changedPlayer.state == PlayerState.SelectingMotion) {

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

