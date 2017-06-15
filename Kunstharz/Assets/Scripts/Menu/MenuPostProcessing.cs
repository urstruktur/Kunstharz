using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MenuPostProcessing : MonoBehaviour {

	private Vector3 oldCameraRotation;
	PostProcessingProfile profile;

	private float oldValue = 0.0f;

	private int saturationID;

	void OnEnable() {
		PostProcessingBehaviour behaviour = GetComponent<PostProcessingBehaviour> ();
		profile = Instantiate(behaviour.profile);
        behaviour.profile = profile;
	}

	public void SetSaturation(float value, float duration) {
		if (value != oldValue) {
			LeanTween.cancel(saturationID);
			saturationID = LeanTween.value (gameObject, UpdateSaturation, oldValue, value, duration).setEaseInOutSine().id;
			oldValue = value;
		}
	}

	private void UpdateSaturation(float value) {
		ColorGradingModel.Settings colorGrading = profile.colorGrading.settings;
		colorGrading.basic.saturation = value;
		profile.colorGrading.settings = colorGrading;
	}

}
