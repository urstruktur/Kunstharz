using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICrosshair : MonoBehaviour {

	[Header("Crosshair Parts")]
	public GameObject [] crosshairParts = new GameObject[5];


	public enum CrosshairModes {Move, Shoot};

	[Header("General")]
	public CrosshairModes crosshairMode = CrosshairModes.Move;

	CrosshairModes currentCrosshairMode = CrosshairModes.Move;
	
	// Update is called once per frame
	void Update () {
		if (crosshairMode != currentCrosshairMode) {
			currentCrosshairMode = crosshairMode;
			if (crosshairMode == CrosshairModes.Move) {
				showMoveCrosshair ();
			} else {
				showShootCrosshair ();
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
}
