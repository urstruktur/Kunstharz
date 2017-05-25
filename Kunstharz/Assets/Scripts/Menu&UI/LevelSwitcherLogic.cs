using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcherLogic : MonoBehaviour, ISwitcherLogic {

	public int selectedLevelIdx = 0;
	public float switchLevelOffset = 10.0f;
	public float levelSwitchTime = 0.6f;
	private float remainingLevelSwitchTime = 0.0f;

	private int futureSelectedLevelIdx;

	private bool isSwitchingLevel {
		get {
			return futureSelectedLevelIdx != selectedLevelIdx;
		}
	}

	public delegate void LevelSwitchDelegate();
	public static LevelSwitchDelegate OnLevelSwitch;

	void Start () {
		futureSelectedLevelIdx = selectedLevelIdx;
		for (int i = 0; i < transform.childCount; ++i) {
			transform.GetChild (i).gameObject.SetActive (i == selectedLevelIdx);
		}
	}

	void StartLevelSwitchTo(int idx, bool moveUp) {
		if (!isSwitchingLevel && idx != selectedLevelIdx) {
			futureSelectedLevelIdx = idx;

			var oldLevel = transform.GetChild (selectedLevelIdx);
			var newLevel = transform.GetChild (futureSelectedLevelIdx);

			newLevel.gameObject.SetActive (true);

			Vector3 offset = (moveUp ? Vector3.left : Vector3.right) * switchLevelOffset;

			Vector3 oldLevelStartPos = transform.GetChild (selectedLevelIdx).position;
			Vector3 oldLevelEndPos = oldLevelStartPos + offset;
			Vector3 newLevelStartPos = oldLevelStartPos - offset;
			Vector3 newLevelEndPos = oldLevelStartPos;


			newLevel.position = newLevelStartPos;
			LeanTween.move (newLevel.gameObject, newLevelEndPos, levelSwitchTime).setEaseOutExpo();
			LeanTween.move (oldLevel.gameObject, oldLevelEndPos, levelSwitchTime).setEaseOutExpo().setOnComplete(() => {
				selectedLevelIdx = idx;
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
}
