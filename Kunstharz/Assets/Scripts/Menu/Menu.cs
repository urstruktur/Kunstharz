using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Colorful;

public class Menu : MonoBehaviour {

	public string playerName = "";
	public int selectedLevelIdx = 0;
	public GameObject menuTransform;
	public GameObject [] menus;
	public Kunstharz.Publisher publisher;
	public Kunstharz.Finder finder;
	public Kunstharz.LevelLoader loader;

	private float turnXOld = 0;
	private float turnYOld = 0;

	[ContextMenu("Reset The Value")]
	private void resetTheValue()  { turnXOld = 42; }

	private GameObject blur;

	public static bool canTurn = false;
	public static int menuRotationDirection = 1;

	public static int blurSpace = 80;

	public static int textSizeSmall = 43;

	public static int textSizeBig = 122;

	public static int textSizeSmallTextMeshPro = 43;

	public static int textSizeBigTextMeshPro = 122;

	public static Vector3 imageSmallScale = new Vector3(0.3f, 0.3f, 0.3f);

	public static Vector3 imageBigScale = new Vector3(0.7f, 0.7f, 0.7f);

	public static int tweenBlurId = -1;

	void Start() {
		blur = GameObject.Find("Blur");
		Cursor.lockState = CursorLockMode.None;
	}

	public void SetPlayerName(string gameName) {
		this.playerName = gameName.ToUpper();
	}

	void Update() {
		TurnGameWorld ();
		UpdateBeacon ();
	}

	private void UpdateBeacon() {
		// Updates the beacon to be sent when publishing game on the network
		if (publisher.enabled) {
			if (publisher.challenge == null) {
				publisher.challenge = new Kunstharz.Challenge ();
			}
			publisher.challenge.playerName = playerName;
			publisher.challenge.selectedLevelIdx = selectedLevelIdx;
		}
	}

	private void TurnGameWorld() {
		if (canTurn) {
			float turnX = mouseXN () - turnXOld;
			float turnY = mouseYN () - turnYOld;

			menuTransform.transform.eulerAngles += new Vector3(turnY * 8, turnX * 14, 0);

			turnXOld = mouseXN ();
			turnYOld = mouseYN ();
		}
	}

	private float mouseXN() {
		return Input.mousePosition.x / (float) Screen.width * 2f - 1f;
	}

	private float mouseYN() {
		return Input.mousePosition.y / (float) Screen.height * 2f - 1f;
	}
	
}