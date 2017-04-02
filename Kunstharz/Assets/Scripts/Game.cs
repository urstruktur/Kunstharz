using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
	public class Game : MonoBehaviour
	{
		public bool playBothPlayers = false;
		public bool skipSecondPlayer = true;

		private Controls[] playerControls;

		private int lastActivePlayerIdx;
		private int activePlayerIdx {
			get {
				int idx = 0;
				foreach (Controls controls in playerControls) {
					if (controls.state == ControlState.DecidingTurn) {
						return idx;
					}
					++idx;
				}
				return -1;
			}

			set {
				foreach (Controls controls in playerControls) {
					controls.state = ControlState.FinishedTurn;
					controls.transform.Find ("Mesh").GetComponent<Renderer> ().enabled = true;
					controls.transform.Find ("Crosshair").GetComponent<Renderer> ().enabled = false;
				}

				var activePlayer = playerControls [value];

				activePlayer.state = ControlState.DecidingTurn;
				activePlayer.transform.Find ("Mesh").GetComponent<Renderer> ().enabled = false;
				activePlayer.transform.Find ("Crosshair").GetComponent<Renderer> ().enabled = true;

				if (playBothPlayers) {
					GiveCameraToPlayer (value);
				}

				lastActivePlayerIdx = value;
			}
		}

		void Start () {
			playerControls = GetComponentsInChildren<Controls> ();
			// Its the turn of player 1
			activePlayerIdx = 0;

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
		}

		void TurnFinished () {
			if (skipSecondPlayer) {
				activePlayerIdx = 0;
			} else {
				activePlayerIdx = (lastActivePlayerIdx + 1) % playerControls.Length;
			}
		}
	}
}

