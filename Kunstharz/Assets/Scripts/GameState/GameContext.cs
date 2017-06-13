using System;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameContext : NetworkBehaviour
	{
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

