using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	public Color motionSelectionColor = Color.cyan;
	public Color shotSelectionColor = Color.yellow;
	public Color shotCooldownColor = Color.gray;

	public enum CrosshairMode {
		None,
		MotionSelection,
		ShotSelection,
		ShotCooldown
	}

	public CrosshairMode mode {
		set {
			switch (value) {
			case CrosshairMode.None:
				HideCrosshair ();
				break;

			case CrosshairMode.MotionSelection:
				ShowCrosshair ();
				color = motionSelectionColor;
				break;

			case CrosshairMode.ShotSelection:
				ShowCrosshair ();
				color = shotSelectionColor;
				break;

			case CrosshairMode.ShotCooldown:
				ShowCrosshair ();
				color = shotCooldownColor;
				break;
			}
		}
	}

	private Color color {
		set {
			GetComponent<MeshRenderer> ().material.color = value;
		}

		get {
			return GetComponent<MeshRenderer> ().material.color;
		}
	}

	// Use this for initialization
	void Start () {
		mode = CrosshairMode.MotionSelection;	
	}

	void HideCrosshair() {
		GetComponent<MeshRenderer> ().enabled = false;
	}

	void ShowCrosshair() {
		GetComponent<MeshRenderer> ().enabled = true;
	}

}
