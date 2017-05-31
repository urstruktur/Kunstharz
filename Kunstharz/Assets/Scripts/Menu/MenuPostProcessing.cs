using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MenuPostProcessing : MonoBehaviour {

	private Vector3 oldCameraRotation;
	PostProcessingProfile profile;

	private float oldValue = 0.0f;

	void OnEnable() {
		PostProcessingBehaviour behaviour = GetComponent<PostProcessingBehaviour> ();
		profile = Instantiate(behaviour.profile);
        behaviour.profile = profile;
	}

	public void SetSaturation(float value) {
		if (value != oldValue) {
			LeanTween.value (gameObject, UpdateSaturation, oldValue, value, 0.5f).setEaseInOutSine();
			oldValue = value;
		}
	}

	private void UpdateSaturation(float value) {
		ColorGradingModel.Settings colorGrading = profile.colorGrading.settings;
		colorGrading.basic.saturation = value;
		profile.colorGrading.settings = colorGrading;
	}

}
