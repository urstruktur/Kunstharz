using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRotate : MonoBehaviour {

	public GameObject foregroundCamera;
	public GameObject generalCanvas;

	private float rotationDuration = 0.5f;
	private float oldValue = 0f;

	private bool rotateOut = false;

	private Vector3 originalPosition;
	private Vector3 originalRotation;

	void OnEnable() {
		oldValue = 0f;
		Menu.canTurn = false;
		rotateOut = false;
		generalCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		LeanTween.value (gameObject, UpdateRotation, 120 * Menu.menuRotationDirection, 0, rotationDuration).setOnComplete(RotateComplete).setEaseInOutSine();
	}

	private void UpdateRotation(float value) {
		float differenceValue = value - oldValue;
		transform.RotateAround(foregroundCamera.transform.position, Vector3.up, differenceValue);
		oldValue = value;
	}

	private void RotateComplete() {
		if (rotateOut) {
			transform.localEulerAngles = Vector3.zero;
			transform.localPosition = new Vector3 (0f, 0f, -300f);
			gameObject.SetActive(false);
		}
		Menu.canTurn = true;
		oldValue = 0;
		generalCanvas.GetComponent<GraphicRaycaster>().enabled = true;
	}

	public void RotateOut() {
		generalCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		oldValue = 0f;
		Menu.canTurn = false;
		rotateOut = true;
		LeanTween.value (gameObject, UpdateRotation, 0f, -120f * Menu.menuRotationDirection, rotationDuration).setOnComplete(RotateComplete).setEaseInOutSine();
	}
}
