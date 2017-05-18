using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcher : MonoBehaviour {

	public int selectedLevelIdx = 0;
	public float switchLevelOffset = 10.0f;
	public float levelSwitchTime = 0.8f;

	private float remainingLevelSwitchTime = 0.0f;

	private int futureSelectedLevelIdx;

	private bool isSwitchingLevel {
		get {
			return futureSelectedLevelIdx != selectedLevelIdx;
		}
	}

	// Use this for initialization
	void Start () {
		futureSelectedLevelIdx = selectedLevelIdx;
		for (int i = 0; i < transform.childCount; ++i) {
			transform.GetChild (i).gameObject.SetActive (i == selectedLevelIdx);
		}
	}

	void StartLevelSwitchTo(int idx) {
		if (!isSwitchingLevel && idx != selectedLevelIdx) {
			futureSelectedLevelIdx = idx;

			var oldLevel = transform.GetChild (selectedLevelIdx);
			var newLevel = transform.GetChild (futureSelectedLevelIdx);

			newLevel.gameObject.SetActive (true);

			Vector3 offset = Vector3.up * switchLevelOffset;

			Vector3 oldLevelStartPos = transform.GetChild (selectedLevelIdx).position;
			Vector3 oldLevelEndPos = oldLevelStartPos + offset;
			Vector3 newLevelStartPos = oldLevelStartPos - offset;
			Vector3 newLevelEndPos = oldLevelStartPos;


			newLevel.position = newLevelStartPos;
			LeanTween.move (newLevel.gameObject, newLevelEndPos, levelSwitchTime);
			LeanTween.move (oldLevel.gameObject, oldLevelEndPos, levelSwitchTime).setOnComplete(() => {
				selectedLevelIdx = idx;
				oldLevel.gameObject.SetActive(false);
			});
		}
	}

	void NextLevel() {
		int nextLevelIdx = (selectedLevelIdx + 1) % transform.childCount;
		StartLevelSwitchTo (nextLevelIdx);
	}

	void PrevLevel() {
		int prevLevelIdx = selectedLevelIdx - 1;

		if (prevLevelIdx < 0) {
			prevLevelIdx = transform.childCount - 1;
		}

		StartLevelSwitchTo (prevLevelIdx);
	}
	
	// Update is called once per frame
	void Update () {
		if (isSwitchingLevel) {

		} else {
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				PrevLevel ();
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				NextLevel ();
			}
		}
	}
}
