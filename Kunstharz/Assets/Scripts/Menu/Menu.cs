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

	public List<Game> games = new List<Game> {
		new Game("PHIL'S GAME", 1),
		new Game("THOMAS' GAME", 2),
		new Game("SARAH'S GAME", 0),
		new Game("ANDREAS' GAME", 1)
	};

	void Start() {
		blur = GameObject.Find("Blur");
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
			publisher.beacon = playerName;
		}
	}

	private void TurnGameWorld() {
		if (canTurn) {
			float turnX = mouseXN () - turnXOld;
			float turnY = mouseYN () - turnYOld;

			menuTransform.transform.eulerAngles += new Vector3(turnY * 8, turnX * 8, 0);

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
<<<<<<< HEAD
	
=======

	private void SetMenusInactive() {
		for (int i = 0; i < menus.Length; i++) {
			menus[i].SetActive(false);
		}
		blur.GetComponent<RawImage>().enabled = false;
	}

	public void TopLevelMenu() {
		publisher.enabled = false;
		finder.enabled = false;

		SetMenusInactive();
		menus[0].SetActive (true);
	}

	public void JoinGame () {
		finder.enabled = true;
		publisher.enabled = false;
    }

	public void StartGameChooseLevelMenu () {
		SetMenusInactive();
		menus[1].SetActive (true);
    }

	public void ExitGame () {
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
    }

	public void StartGameChooseNameMenu() {
		publisher.enabled = true;
		finder.enabled = false;
		SetMenusInactive();
		menus[2].SetActive (true);
	}

    public void StartGameWaiting()
    {
        SetMenusInactive();
        menus[3].SetActive(true);
		loader.StartUgly (selectedLevelIdx);
    }
>>>>>>> 2b2472212575a602208693fa948a223f023938f0
}