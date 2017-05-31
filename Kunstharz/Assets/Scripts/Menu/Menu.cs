using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Colorful;

public class Menu : MonoBehaviour {

	public string gameName = "";
	public int selectedLevelIdx = 0;
	public GameObject generalCanvas;
	public GameObject [] menus;
	public Kunstharz.Publisher publisher;
	public Kunstharz.Finder finder;
	public Kunstharz.LevelLoader loader;

	private float turnXOld = 0;
	private float turnYOld = 0;

	[ContextMenu("Reset The Value")]
	private void resetTheValue()  { turnXOld = 42; }

	private GameObject blur;
	void Start() {
		blur = GameObject.Find("Blur");
	}

	public void SetGameName(string gameName) {
		this.gameName = gameName.ToUpper();
	}

	public static int tweenBlurId = -1;

	void Update() {
		TurnGameWorld ();
		UpdateBeacon ();
	}

	private void UpdateBeacon() {
		// Updates the beacon to be sent when publishing game on the network
		if (publisher.enabled) {
			publisher.beacon = gameName;
		}
	}

	private void TurnGameWorld() {
		float turnX = mouseXN () - turnXOld;
		float turnY = mouseYN () - turnYOld;

		generalCanvas.transform.eulerAngles += new Vector3(turnY * 8, turnX * 8, 0);

		turnXOld = mouseXN ();
		turnYOld = mouseYN ();
	}

	private float mouseXN() {
		return Input.mousePosition.x / (float) Screen.width * 2f - 1f;
	}

	private float mouseYN() {
		return Input.mousePosition.y / (float) Screen.height * 2f - 1f;
	}

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
}