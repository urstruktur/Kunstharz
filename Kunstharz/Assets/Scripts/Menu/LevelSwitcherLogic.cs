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

	public int selectedGameIdx = 0;
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

	public List<Kunstharz.FinderEntry> entries = new List<Kunstharz.FinderEntry> ();

	void Start () {

		futureSelectedLevelIdx = selectedLevelIdx;
		levelPosition = new Vector3 [transform.childCount];

		for (int i = 0; i < transform.childCount; ++i) {
			transform.GetChild (i).gameObject.SetActive (i == selectedLevelIdx);
			levelPosition[i] = transform.GetChild (i).position;
		}

		Kunstharz.Finder.ChallengeExpired += ChallengeExpired;
		Kunstharz.Finder.ChallengeDiscovered += ChallengeDiscovered;

	}

	void OnDestroy() {
		Kunstharz.Finder.ChallengeExpired -= ChallengeExpired;
		Kunstharz.Finder.ChallengeDiscovered -= ChallengeDiscovered;
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
			selectedGameIdx = (selectedGameIdx + 1) % entries.Count;
			StartLevelSwitchTo (entries[selectedGameIdx].challenge.selectedLevelIdx, false);
			gameName.GetComponent<Text>().text = entries[selectedGameIdx].challenge.playerName;
		}
	}

	public void PreviousGame() {
		if (!isSwitchingLevel) {
			selectedGameIdx = selectedGameIdx - 1;
			if (selectedGameIdx < 0) selectedGameIdx = entries.Count - 1;
			StartLevelSwitchTo (entries[selectedGameIdx].challenge.selectedLevelIdx, false);
			gameName.GetComponent<Text>().text = entries[selectedGameIdx].challenge.playerName;
		}
	}
	
	void Update () {
		if (!isSwitchingLevel) {
			if (Input.GetKey (KeyCode.UpArrow)) {
				PrevLevel ();
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				NextLevel ();
			}
		}
	}

	private void GameAdded() {
		if (entries.Count == 1) {
			buttonVisibility(false, true);
			NextGame();
		} else if (entries.Count > 1) {
			buttonVisibility(true, true);
		}
	}

	private void GameRemoved(int removeIdx) {
		if (removeIdx < selectedGameIdx) {
			selectedGameIdx -= 1;
		} else if (removeIdx == selectedGameIdx) {
			PreviousGame();
		}

		if (entries.Count == 1) {
			buttonVisibility(false, true);
			NextGame();
		} else if (entries.Count == 0) {
			buttonVisibility(false, false);
			gameName.GetComponent<Text>().text = "SEARCHING FOR GAMES...";
			StartLevelSwitchTo(0, false);
		}

	}

	private void ChallengeExpired(int idx) {
		GameRemoved(idx);
		entries.RemoveAt(idx);
	}

	private void ChallengeDiscovered(Kunstharz.FinderEntry entry) {
		entries.Add(entry);
		GameAdded();
	}

	private void buttonVisibility(bool toggle, bool choose) {
		leftArrow.SetActive(toggle);
		rightArrow.SetActive(toggle);
		chooseGameButton.SetActive(choose);
	}

}