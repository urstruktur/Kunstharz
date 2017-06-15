using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularCrosshair : MonoBehaviour {

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
	
}
