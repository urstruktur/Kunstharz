using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularCrosshair : MonoBehaviour {

	[Header("Crosshair Parts")]
	public GameObject [] crosshairParts = new GameObject[5];

	public GameObject prepareTimer;

	public GameObject finishedTimer;

	public enum CrosshairModes {Move, Shoot, MoveIdle, ShootIdle, MoveDenied, ShotFired};

	[Header("General")]
	public CrosshairModes crosshairMode = CrosshairModes.Move;

	CrosshairModes currentCrosshairMode = CrosshairModes.Move;

	void Start() {
		if (GameObject.Find("Level01_Geometry(Clone)") != null) {
			prepareTimer.GetComponent<Text>().color = CreateColor(201, 38, 38);
			finishedTimer.GetComponent<Text>().color = CreateColor(201, 38, 38);
		} else if (GameObject.Find("Level02_Geometry(Clone)") != null) {
			prepareTimer.GetComponent<Text>().color = CreateColor(133, 66, 191);
			finishedTimer.GetComponent<Text>().color = CreateColor(133, 66, 191);
		} else if (GameObject.Find("Level03_Geometry(Clone)") != null) {
			prepareTimer.GetComponent<Text>().color = CreateColor(255, 119, 45);
			finishedTimer.GetComponent<Text>().color = CreateColor(255, 119, 45);
		} else if (GameObject.Find("Level04_Geometry(Clone)") != null) {
			prepareTimer.GetComponent<Text>().color = CreateColor(26, 135, 163);
			finishedTimer.GetComponent<Text>().color = CreateColor(26, 135, 163);
		} else if (GameObject.Find("Level05_Geometry(Clone)") != null) {
			prepareTimer.GetComponent<Text>().color = CreateColor(232, 90, 116);
			finishedTimer.GetComponent<Text>().color = CreateColor(232, 90, 116);
		}
	}

	private Color CreateColor(int r, int g, int b) {
		return new Color(r/255.0f, g/255.0f, b/255.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (crosshairMode != currentCrosshairMode) {
			currentCrosshairMode = crosshairMode;
			if (crosshairMode == CrosshairModes.Move) {
				ShowMoveCrosshair ();
			} else if (crosshairMode == CrosshairModes.Shoot) {
				ShowShootCrosshair ();
			} else if (crosshairMode == CrosshairModes.MoveIdle) {
				ShowMoveIdleCrosshair ();
			} else if (crosshairMode == CrosshairModes.ShootIdle) {
				ShowShootIdleCrosshair ();
			} else if (crosshairMode == CrosshairModes.MoveDenied) {
				ShowMoveDeniedCrosshair ();
			}  else if (crosshairMode == CrosshairModes.ShotFired) {
				ShowShotFiredCrosshair ();
			}
		}
	}

	public void ShowShootCrosshair() {
		ChangeCrosshairMode(CrosshairModes.Shoot);
		gameObject.GetComponent<Animator> ().Play ("CrosshairShootAnimation");
	}

	public void ShowMoveCrosshair() {
		ChangeCrosshairMode(CrosshairModes.Move);
		gameObject.GetComponent<Animator> ().Play ("CrosshairMoveAnimation");
	}

	public void ShowMoveIdleCrosshair() {
		ChangeCrosshairMode(CrosshairModes.MoveIdle);
		gameObject.GetComponent<Animator> ().Play ("CrosshairMoveIdleAnimation");
	}

	public void ShowShootIdleCrosshair() {
		ChangeCrosshairMode(CrosshairModes.ShootIdle);
		gameObject.GetComponent<Animator> ().Play ("CrosshairShootIdleAnimation");
	}

	public void ShowMoveDeniedCrosshair() {
		ChangeCrosshairMode(CrosshairModes.MoveDenied);
		gameObject.GetComponent<Animator> ().Play ("CrosshairMoveDeniedAnimation");
	}

	public void ShowShotFiredCrosshair() {
		ChangeCrosshairMode(CrosshairModes.ShotFired);
		gameObject.GetComponent<Animator> ().Play ("CrosshairShootFiredAnimation");
	}

	public void ChangeCrosshairMode (CrosshairModes mode) {
		currentCrosshairMode = mode;
		crosshairMode = mode;
	}

	public void ShowCrosshair() {
		for (int i = 0; i < crosshairParts.Length; i++) {
			crosshairParts[i].GetComponent<Image>().enabled = true;
		}
	} 

	public void HideCrosshair() {
		for (int i = 0; i < crosshairParts.Length; i++) {
			crosshairParts[i].GetComponent<Image>().enabled = false;
		}
	} 

	public void ShowPrepareTimer(float duration) {
		prepareTimer.GetComponent<Text>().enabled = true;
		prepareTimer.GetComponent<Text>().text = duration + "";
		HideCrosshair();
		LeanTween.value(gameObject, UpdateTime, duration, 0, duration).setOnComplete(TimeComplete);
	} 

	public void HidePrepareTimer() {
		prepareTimer.GetComponent<Text>().enabled = false;
	} 

	public void TimeComplete() {
		HidePrepareTimer();
		ShowCrosshair();
	}

	void UpdateTime(float value) {
		prepareTimer.GetComponent<Text>().text = (int)value + "";
	}

	public void ShowFinishedTimer(float duration) {
		finishedTimer.GetComponent<Text>().enabled = true;
		finishedTimer.GetComponent<Text>().text = duration + "";
		HideCrosshair();
		LeanTween.value(gameObject, UpdateFinished, duration, 0, duration).setOnComplete(FinishedComplete);
	} 

	public void HideFinishedTimer() {
		finishedTimer.GetComponent<Text>().enabled = false;
	} 

	void UpdateFinished(float value) {
		finishedTimer.GetComponent<Text>().text = (int)value + "";
	}

	public void FinishedComplete() {
		HideFinishedTimer();
	}
	
}
