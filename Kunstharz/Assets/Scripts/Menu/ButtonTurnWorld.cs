using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTurnWorld : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public bool saturation = false;
	private bool pointerInside = false;
	private GameObject geometry;
	private GameObject activeLevel;

	private int activeLevelIndex;
	private GameObject instructions;
	private Vector3 initialRotation = Vector3.zero;

	private Vector3 currentRotation = Vector3.zero;

	private bool first = true;
	private float firstX = 0;
	private float firstY = 0;

	private int rotateID;
	private int fadeID;

	private bool animationRunning = false;

	private bool canTurnLevel = false;

	public GameObject [] buttons;
	private GameObject backgroundCamera;

	private bool showInstructions = true;

	void Start() {
		geometry = GameObject.Find("Geometry");
		activeLevelIndex = geometry.GetComponent<LevelSwitcherLogic>().selectedLevelIdx;
		activeLevel = geometry.transform.GetChild (activeLevelIndex).gameObject;
		initialRotation = activeLevel.transform.localEulerAngles;
		instructions = gameObject.transform.GetChild(0).gameObject;
		backgroundCamera = GameObject.Find("Background Camera");
	}

	void Update() {
		if (pointerInside && Input.GetMouseButtonDown(0)) {
			canTurnLevel = true;
			first = true;
		}

		if (Input.GetMouseButtonUp(0)) {
			canTurnLevel = false;
			TurnLevelBack();
		} else {
			if (canTurnLevel) {
				TurnLevel();
			}
		}

		if (activeLevelIndex != geometry.GetComponent<LevelSwitcherLogic>().selectedLevelIdx) {
			activeLevel.transform.localEulerAngles = initialRotation;
			activeLevelIndex = geometry.GetComponent<LevelSwitcherLogic>().selectedLevelIdx;
			activeLevel = geometry.transform.GetChild (activeLevelIndex).gameObject;
			initialRotation = activeLevel.transform.localEulerAngles;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		pointerInside = true;
		
	}

	public void OnPointerExit(PointerEventData eventData) {
		pointerInside = false;
	}

	public void TurnLevel() {

		if (first) {

			LeanTween.cancel(fadeID);
			LeanTween.cancel(rotateID);

			first = false;
			firstX = Input.mousePosition.x;
			firstY = Input.mousePosition.y;

			if (animationRunning) {
				currentRotation = activeLevel.transform.localEulerAngles;
			} else {
				currentRotation = initialRotation;
			}

			fadeID = LeanTween.value(gameObject, updateColorOpacity, 1f, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuint).setOnComplete(OpacityStop).id;

			SetButtonActive(false);

			if (saturation) backgroundCamera.GetComponent<MenuPostProcessing>().SetSaturation(1f, 0.25f);

		}

		activeLevel.transform.localEulerAngles = currentRotation + new Vector3((Input.mousePosition.y - firstY) * 0.5f, (Input.mousePosition.x - firstX) * 0.5f,0);
	}

	private void TurnLevelBack() {
		LeanTween.cancel(fadeID);
		animationRunning = true;
		fadeID = LeanTween.value(gameObject, updateColorOpacity, 0f, 1f, 0.5f).setEase(LeanTweenType.easeInOutQuint).id;
		rotateID = LeanTween.rotate(activeLevel, initialRotation, .5f).setEaseOutBounce().setOnComplete(AnimationStop).id;
		SetButtonActive(true);
		if (saturation) backgroundCamera.GetComponent<MenuPostProcessing>().SetSaturation(0f, 0.25f);
	}

	private void AnimationStop() {
		animationRunning = false;
	}

	private void OpacityStop() {
		showInstructions = false;
	}

	void updateColorOpacity(float val){
		if (showInstructions) {
			instructions.GetComponent<Text>().color = new Color(1f, 1f, 1f, val);
		}
	}

	private void SetButtonActive(bool active) {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].SetActive(active);
		}
	}

}