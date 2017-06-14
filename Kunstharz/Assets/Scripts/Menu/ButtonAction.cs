﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour, IPointerClickHandler {

	public enum Actions { MoveTo, Exit, None }

	public Actions action = Actions.MoveTo;

	public GameObject moveToMenu;

	private GameObject blur;

	private GameObject backgroundCamera;

	private GameObject levelLoader;

	private GameObject levelSwitcherLogic;

	private GameObject menu;

	public bool reverseMenuRotation = false;

	public float menuSaturation = 0.0f;

	public bool enablePublisher = false;

	public bool enableFinder = false;

	public bool disableFinderAndPublisher = false;

	public bool enableStartHost = false;

	public bool joinGame = false;

	void Start() {
		blur = GameObject.Find("Blur");
		backgroundCamera = GameObject.Find("Background Camera");
		menu = GameObject.Find("Menu Script");
		levelLoader = GameObject.Find("LevelLoader");
		levelSwitcherLogic = GameObject.Find("Geometry");
	}

	public void OnPointerClick (PointerEventData eventData) {
		Clicked();
    }

	public void Clicked() {
		if (action == Actions.MoveTo) {
			blur.GetComponent<RawImage>().enabled = false;

			if (reverseMenuRotation) {
				Menu.menuRotationDirection = -1;
			} else {
				Menu.menuRotationDirection = 1;
			}

			backgroundCamera.GetComponent<MenuPostProcessing>().SetSaturation(menuSaturation);

			moveToMenu.SetActive (true);
			transform.parent.gameObject.GetComponent<MenuRotate>().RotateOut();
		}

		if (action == Actions.Exit) {
			Application.Quit();

			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#endif
		}

		if (enableFinder) {
			menu.GetComponent<Menu>().finder.enabled = true;
			menu.GetComponent<Menu>().publisher.enabled = false;
		}

		if (enablePublisher) {
			menu.GetComponent<Menu>().finder.enabled = false;
			menu.GetComponent<Menu>().publisher.enabled = true;
		}

		if (disableFinderAndPublisher) {
			menu.GetComponent<Menu>().finder.enabled = false;
			menu.GetComponent<Menu>().publisher.enabled = false;
		}

		if (enableStartHost) {
			var loader = GameObject.Find ("LevelLoader").GetComponent<Kunstharz.LevelLoader> ();
			var idx = menu.GetComponent<Menu> ().selectedLevelIdx;
			loader.StartUgly (idx);
		}

		if (joinGame) {
			LevelSwitcherLogic lsw = levelSwitcherLogic.GetComponent<LevelSwitcherLogic>();
			levelLoader.GetComponent<Kunstharz.LevelLoader>().JoinUgly(lsw.entries[lsw.selectedGameIdx].hostname);
		}
	}

}