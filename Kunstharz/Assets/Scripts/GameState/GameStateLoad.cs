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

		private Camera gameCamera;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;
			print("Entering load state");

			localPlayerName = GetLocalPlayerNameFromMenu();

			GameObject.Find("Background Camera").GetComponent<MenuPostProcessing>().SetFadeOut(1f);

			if(isServer) {
				StartCoroutine(SetupGame());
			}
		}

		IEnumerator SetupGame() {
			yield return new WaitForSeconds(1.5f);
			//RegisterSpawnPrefabs();
			SpawnEnvironment();
			SpawnPlayers();
			// Wait for a frame to ensure players are synced first
			yield return new WaitForEndOfFrame();
			// Then hide the main menu, and set up context variables on server and client
			RpcLocalSetup();
			// Wait again to ensure this is finished
			yield return new WaitForEndOfFrame();
			// We are ready to start the game…
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

			// Instantiate away from the menu, so it is only visible when the game camera takes over
			var level = Instantiate (levelPrefab, Vector3.back * 1000, Quaternion.identity);
			NetworkServer.Spawn (level);

			startPositions = level.GetComponentsInChildren<NetworkStartPosition> ();
			if(startPositions.Length < 2) {
				Debug.LogError("A level needs at two instances of NetworkStartPosition, but " + level + " has only " + startPositions.Length);
			}
		}

		void SpawnPlayers() {
			int idx = 0;
			foreach(NetworkConnection conn in NetworkServer.connections) {
				var playerGO = Instantiate (playerPrefab);
				NetworkServer.AddPlayerForConnection(conn, playerGO, 0);
				Transform spawn = startPositions[idx].transform;
				var player = playerGO.GetComponent<Player> ();
				player.RpcInitPlayer (spawn.position, spawn.rotation);
				++idx;
			}
		}

		[ClientRpc]
		private void RpcLocalSetup() {
			FindGameCamera();

			// Set localPlayer and remotePlayer for client and server respectively
			FindPlayers(ctx);

			// Set the game camera locally as a child of the local player and then enable it
			GiveGameCameraToPlayer(ctx.localPlayer);
			
			// Make sure the prepare overlay is immediately visible when the
			// game camera takes over and presents its first frame from the perspecive
			// of the local player
			ShowPrepareGUI();

			// Delete menu now that the level can be shown
			DestroyMainMenu();

			// Camera is initially deactivated, enable it
			ActivateGameCamera();

			// Let server know what the local player name is
			ctx.localPlayer.CmdSetPlayerName(localPlayerName);
		}

        private void ShowPrepareGUI() {
			var gui = GameObject.Find ("GUI").GetComponent<Gui> ();
			gui.ShowInstruction(Gui.InstructionType.PrepareGame, GameContext.instance.GetComponent<GameStateKickoff>().kickoffDuration, true);
			Canvas.ForceUpdateCanvases();
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

		private void FindGameCamera() {
			gameCamera = GameObject.Find("Game Camera").GetComponent<Camera> ();
        }

		private void GiveGameCameraToPlayer(Player camOwner) {
			Transform camTransform = gameCamera.transform;

			camTransform.parent = camOwner.transform;
			camTransform.localPosition = GetComponent<GameStateRound> ().camLocalPosition;
			camTransform.GetComponent<Controls> ().enabled = true;
		}

		private void ActivateGameCamera() {
			gameCamera.enabled = true;
        }

		string GetLocalPlayerNameFromMenu() {
			var menu = GameObject.Find("Menu Script").GetComponent<Menu> ();
			return menu.playerName;
		}

		void DestroyMainMenu() {
			GameObject.Destroy(GameObject.Find("General"));
			GameObject.Destroy(GameObject.Find("Cameras"));
			GameObject.Destroy(GameObject.Find("General Canvas"));
			GameObject.Destroy(GameObject.Find("Effect Canvas"));
			GameObject.Destroy(GameObject.Find("NetworkDiscovery"));
			Cursor.lockState = CursorLockMode.Locked;
		}

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
			throw new NotImplementedException("When loading, did not expect to see action");
		}
	}
}

