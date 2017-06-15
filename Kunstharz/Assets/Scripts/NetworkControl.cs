using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Kunstharz
{
	public class NetworkControl : NetworkManager
	{
		public GameObject contextPrefab;
		
		// <summary>
		// Spawns a game context when two connections are ready and sets it to
		// the loading state
		// </summary>
		public override void OnServerReady(NetworkConnection conn) {
			base.OnServerReady(conn);
			
			if(NetworkServer.connections.Count == 2) {
				var context = Instantiate(contextPrefab);
				NetworkServer.Spawn(context);
				context.GetComponent<GameContext> ().CmdSetStateIdx(GameStateLoad.IDX);
			}
		}
	}
}

