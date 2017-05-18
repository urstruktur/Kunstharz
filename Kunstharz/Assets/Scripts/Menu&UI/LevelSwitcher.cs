using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcher : MonoBehaviour {

	public int selectedLevelIdx = 0;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; ++i) {
			transform.GetChild (i).gameObject.SetActive (i == selectedLevelIdx);
		}
	}

	void SelectLevel(int idx) {
		transform.GetChild (selectedLevelIdx).gameObject.SetActive (false);
		transform.GetChild (idx).gameObject.SetActive (true);
		selectedLevelIdx = idx;
	}

	void NextLevel() {
		int nextLevelIdx = (selectedLevelIdx + 1) % transform.childCount;
		SelectLevel (nextLevelIdx);
	}

	void PrevLevel() {
		int prevLevelIdx = selectedLevelIdx - 1;

		if (prevLevelIdx < 0) {
			prevLevelIdx = transform.childCount - 1;
		}

		SelectLevel (prevLevelIdx);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			PrevLevel ();
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			NextLevel ();
		}
	}
}
