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

		/*public void PlayerSelectedTarget(Player selectingPlayer, Target target) {
			current.PlayerSelectedTarget(this, selectingPlayer, target);
		}

		public void PlayerSelectedPlayer(Player selectingPlayer, Player selectedPlayer) {
			current.PlayerSelectedPlayer(this, selectingPlayer, selectedPlayer);
		}

		public void PlayerFinishedMotion(Player player) {
			current.PlayerFinishedMotion(this, player);
		}*/

		void Update() {
			if(isClient) {
				UpdateClient();
			} else {
				UpdateServer();
			}
			UpdateBothServerAndClient();
		}

		/// <summary>
		/// Called each frame for both server and client.
		/// </summary>
		private void UpdateBothServerAndClient() {
			//current.Update (this);
		}

		/// <summary>
		/// Called each frame when not running on a client.
		/// </summary>
        private void UpdateServer() {
            //throw new NotImplementedException();
        }

		/// <summary>
		/// Called each frame when running on a client and the context
		/// was spawned by the server.
		/// </summary>
        private void UpdateClient() {
			//throw new NotImplementedException();
		}
	}
}

