using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateRound : NetworkBehaviour, IGameState
	{
		public const int IDX = 1;

		public bool timeoutEnabled = true;

		public void Enter(GameContext ctx) {
			print("Entering round state");

			FindPlayers(ctx);

			print("Round starting");
			print(ctx.localPlayer);
			print(ctx.remotePlayer);
		}

		public void Exit(GameContext ctx) {
			print("Exiting round state");
		}

		public void LocalPlayerSelectedLocation(GameContext ctx, Vector3 selectedLocation) {

		}

		public void RemotePlayerSelectedLocation(GameContext ctx, Vector3 selectedLocation) {

		}

		private void FindPlayers(GameContext ctx) {
			foreach(var playerGO in GameObject.FindGameObjectsWithTag("Player")) {
				Player player = playerGO.GetComponent<Player> ();
				if(player.isLocalPlayer) {
					ctx.localPlayer = player;
				} else {
					ctx.remotePlayer = player;
				}
			}
		}
	}
}

