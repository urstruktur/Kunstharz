using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICrosshair : MonoBehaviour {

	[Header("Crosshair Parts")]
	public GameObject [] crosshairParts = new GameObject[5];


	public enum CrosshairModes {Move, Shoot, MoveIdle, ShootIdle, MoveDenied, ShotFired};

	[Header("General")]
	public CrosshairModes crosshairMode = CrosshairModes.Move;

	CrosshairModes currentCrosshairMode = CrosshairModes.Move;
	
	// Update is called once per frame
	void Update () {
		if (crosshairMode != currentCrosshairMode) {
			currentCrosshairMode = crosshairMode;
			if (crosshairMode == CrosshairModes.Move) {
				showMoveCrosshair ();
			} else if (crosshairMode == CrosshairModes.Shoot) {
				showShootCrosshair ();
			} else if (crosshairMode == CrosshairModes.MoveIdle) {
				showMoveIdleCrosshair ();
			} else if (crosshairMode == CrosshairModes.ShootIdle) {
				showShootIdleCrosshair ();
			} else if (crosshairMode == CrosshairModes.MoveDenied) {
				showMoveDeniedCrosshair ();
			}  else if (crosshairMode == CrosshairModes.ShotFired) {
				showShotFiredCrosshair ();
			}
		}
	}

	void showShootCrosshair() {
		Debug.Log ("Shoot Crosshair Activated");
		gameObject.GetComponent<Animator> ().Play ("CrosshairShootAnimation");
	}

	void showMoveCrosshair() {
		Debug.Log ("Move Crosshair Activated");
		gameObject.GetComponent<Animator> ().Play ("CrosshairMoveAnimation");
	}

	void showMoveIdleCrosshair() {
		Debug.Log ("Move Idle Crosshair Activated");
		gameObject.GetComponent<Animator> ().Play ("CrosshairMoveIdleAnimation");
	}

	void showShootIdleCrosshair() {
		Debug.Log ("Shoot Idle Crosshair Activated");
		gameObject.GetComponent<Animator> ().Play ("CrosshairShootIdleAnimation");
	}

	void showMoveDeniedCrosshair() {
		Debug.Log ("Move Denied Crosshair Activated");
		gameObject.GetComponent<Animator> ().Play ("CrosshairMoveDeniedAnimation");
	}

	void showShotFiredCrosshair() {
		Debug.Log ("Shot Fired Crosshair Activated");
		gameObject.GetComponent<Animator> ().Play ("CrosshairShootFiredAnimation");
	}
}
