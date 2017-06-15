using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameContext : NetworkBehaviour
	{
		public static GameContext instance {
			get {
				var go = GameObject.FindGameObjectWithTag("Context");
				return (go == null) ? null : go.GetComponent<GameContext> ();
			}
		}

		[SyncVar(hook="ChangeCurrentStateIdx")]
		public int currentStateIdx = -1;

		public Player localPlayer;
		public Player remotePlayer;

		public string localPlayerName;

		private IGameState current {
			get {
				return (currentStateIdx == -1)
				            ? GameStateNull.instance
							: GetComponents<IGameState> ()[currentStateIdx];
			}
		}

		[Command]
		public void  CmdSetStateIdx(int newIdx) {
			print ("Got command to set to: " + newIdx);
			currentStateIdx = newIdx;
		}

		private void ChangeCurrentStateIdx(int newIdx) {
			print("Changing idx to " + newIdx);
			current.Exit(this);
			currentStateIdx = newIdx;
			current.Enter(this);
		}

		public void Selected(Player player, Vector3 direction) {
			current.Selected(this, player, direction);
		}
	}
}

