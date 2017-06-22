using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateLoad : NetworkBehaviour, IGameState
	{
		public const int IDX = 0;

		public GameObject essentialsPrefab;
		public GameObject playerPrefab;
		public GameObject[] levelPrefabs;

		private NetworkStartPosition[] startPositions;

		private GameContext ctx;

		private string localPlayerName;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;
			print("Entering load state");

			localPlayerName = GetLocalPlayerNameFromMenu();

			HideMainMenu(); 

			if(isServer) {
				//RegisterSpawnPrefabs();
				SpawnEnvironment();
				SpawnPlayers();
				StartCoroutine(ChangeToKickoffNextFrame());
			}
		}

		IEnumerator ChangeToKickoffNextFrame() {
			yield return new WaitForEndOfFrame();
			RpcLocalSetup();
			yield return new WaitForEndOfFrame();
			ctx.currentStateIdx = GameStateKickoff.IDX;
		}

		public void Exit(GameContext ctx) {
			print("Exiting load state");
		}

		void RegisterSpawnPrefabs() {
			ClientScene.RegisterPrefab(essentialsPrefab);
			ClientScene.RegisterPrefab(playerPrefab);
			foreach(var level in levelPrefabs) {
				ClientScene.RegisterPrefab(level);
			}
		}

		void SpawnEnvironment() {
			NetworkServer.Spawn (Instantiate (essentialsPrefab));

			var levelIdx = GameObject.Find("Menu Script").GetComponent<Menu> ().selectedLevelIdx;
			var levelPrefab = levelPrefabs[levelIdx];

			var level = Instantiate (levelPrefab);
			NetworkServer.Spawn (level);

			startPositions = level.GetComponentsInChildren<NetworkStartPosition> ();
		}

		void SpawnPlayers() {
			int idx = 0;
			foreach(NetworkConnection conn in NetworkServer.connections) {
				var playerGO = Instantiate (playerPrefab);
				NetworkServer.AddPlayerForConnection(conn, playerGO, 0);
				Transform spawn = startPositions[idx].transform;
				var player = playerGO.GetComponent<Player> ();
				player.RpcInitPlayer (spawn.position, spawn.rotation);
				player.RpcResetPosition();
				++idx;
			}
		}

		[ClientRpc]
		private void RpcLocalSetup() {
			print("Doing local setup");
			FindPlayers(ctx);
			GiveCameraToPlayer(ctx.localPlayer);
			ctx.localPlayer.CmdSetPlayerName(localPlayerName);
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

		private void GiveCameraToPlayer(Player camOwner) {
			Transform camTransform = Camera.main.transform;

			camTransform.parent = camOwner.transform;
			camTransform.localPosition = GetComponent<GameStateRound> ().camLocalPosition;
			camTransform.GetComponent<Controls> ().enabled = true;
		}

		string GetLocalPlayerNameFromMenu() {
			var menu = GameObject.Find("Menu Script").GetComponent<Menu> ();
			return menu.playerName;
		}

		void HideMainMenu() {
			// DEBUG
			GameObject.Destroy(GameObject.Find("General"));
			GameObject.Destroy(GameObject.Find("Cameras"));
			GameObject.Destroy(GameObject.Find("General Canvas"));
			GameObject.Destroy(GameObject.Find("Effect Canvas"));
			GameObject.Destroy(GameObject.Find("NetworkDiscovery"));
		}

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
			throw new NotImplementedException("When loading, did not expect to see action");
		}
	}
}

