using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSwitcherLogic : MonoBehaviour {

	public GameObject menu;
	public GameObject gameName;
	public GameObject leftArrow;
	public GameObject rightArrow;
	public GameObject chooseGameButton;
	public int selectedLevelIdx = 0;
	public float switchLevelOffset = 10.0f;
	public float levelSwitchTime = 0.6f;
	private float remainingLevelSwitchTime = 0.0f;
	private int futureSelectedLevelIdx;
	Vector3 [] levelPosition;
	private bool isSwitchingLevel {
		get {
			return futureSelectedLevelIdx != selectedLevelIdx;
		}
	}

	public delegate void LevelSwitchDelegate();
	public static LevelSwitchDelegate OnLevelSwitch;
	private int currentGameIndex = 0;
	
	private bool _gamesAvailable = false;

	public bool gamesAvailable {
    	get { return this._gamesAvailable; }
		set { if(_gamesAvailable == value) return; this._gamesAvailable = value; OnGamesAvailable(); }
	}

	void Start () {

		futureSelectedLevelIdx = selectedLevelIdx;
		levelPosition = new Vector3 [transform.childCount];

		for (int i = 0; i < transform.childCount; ++i) {
			transform.GetChild (i).gameObject.SetActive (i == selectedLevelIdx);
			levelPosition[i] = transform.GetChild (i).position;
		}
		

	}

	void StartLevelSwitchTo(int idx, bool moveUp) {
		if (!isSwitchingLevel && idx != selectedLevelIdx) {
			futureSelectedLevelIdx = idx;

			var oldLevel = transform.GetChild (selectedLevelIdx);
			var newLevel = transform.GetChild (futureSelectedLevelIdx);

			newLevel.gameObject.SetActive (true);

			Vector3 offset = (moveUp ? Vector3.left : Vector3.right) * switchLevelOffset;

			Vector3 oldLevelStartPos = levelPosition[selectedLevelIdx];
			Vector3 oldLevelEndPos = levelPosition[selectedLevelIdx] + offset;
			Vector3 newLevelStartPos = levelPosition[futureSelectedLevelIdx] - offset;
			Vector3 newLevelEndPos = levelPosition[futureSelectedLevelIdx];


			newLevel.position = newLevelStartPos;
			LeanTween.move (newLevel.gameObject, newLevelEndPos, levelSwitchTime).setEaseOutExpo();
			LeanTween.move (oldLevel.gameObject, oldLevelEndPos, levelSwitchTime).setEaseOutExpo().setOnComplete(() => {
				selectedLevelIdx = idx;
				menu.GetComponent<Menu>().selectedLevelIdx = selectedLevelIdx;
				oldLevel.gameObject.SetActive(false);
			});

			if(OnLevelSwitch != null)
			OnLevelSwitch();

		}
	}

	public void NextLevel() {
		int nextLevelIdx = (selectedLevelIdx + 1) % transform.childCount;
		StartLevelSwitchTo (nextLevelIdx, true);
	}

	public void PrevLevel() {
		int prevLevelIdx = selectedLevelIdx - 1;

		if (prevLevelIdx < 0) {
			prevLevelIdx = transform.childCount - 1;
		}

		StartLevelSwitchTo (prevLevelIdx, false);
	}

	public void NextGame () {
		if (!isSwitchingLevel) {
			currentGameIndex = (currentGameIndex + 1) % menu.GetComponent<Menu>().games.Count;
			StartLevelSwitchTo (menu.GetComponent<Menu>().games[currentGameIndex].world, false);
			gameName.GetComponent<Text>().text = menu.GetComponent<Menu>().games[currentGameIndex].gameName;
		}
	}

	public void PreviousGame() {
		if (!isSwitchingLevel) {
			currentGameIndex = currentGameIndex - 1;
			if (currentGameIndex < 0) currentGameIndex = menu.GetComponent<Menu>().games.Count - 1;
			StartLevelSwitchTo (menu.GetComponent<Menu>().games[currentGameIndex].world, false);
			gameName.GetComponent<Text>().text = menu.GetComponent<Menu>().games[currentGameIndex].gameName;
		}
	}
	
	void Update () {
		if (isSwitchingLevel) {

		} else {
			if (Input.GetKey (KeyCode.UpArrow)) {
				PrevLevel ();
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				NextLevel ();
			}
		}
	}

	private void OnGamesAvailable() {
		if (_gamesAvailable) {
			leftArrow.SetActive(true);
			rightArrow.SetActive(true);
			chooseGameButton.SetActive(true);
			NextGame();
		}
	}

	void OnValidate() {
		OnGamesAvailable();
	}

}
