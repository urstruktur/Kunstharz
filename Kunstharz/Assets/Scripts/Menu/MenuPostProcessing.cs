using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MenuPostProcessing : MonoBehaviour {

	private Vector3 oldCameraRotation;
	PostProcessingProfile profile;

	private float oldValue = 0.0f;

	private int saturationID;

	private Colorful.Glitch glitch;

	void OnEnable() {
		PostProcessingBehaviour behaviour = GetComponent<PostProcessingBehaviour> ();
		profile = Instantiate(behaviour.profile);
        behaviour.profile = profile;
		glitch = GetComponent<Colorful.Glitch>();
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

	public void SetFadeOut(float duration) {
		glitch.RandomActivation = false;
		glitch.SettingsTearing.MaxDisplacement = 0.5f;
		LeanTween.value (gameObject, UpdateExposure, 0f, 14f, duration/2).setEaseInOutSine().setDelay(duration/2);
		LeanTween.value (gameObject, UpdateGlitch, 0f, 1f, duration).setEaseInOutSine();
	}

	private void UpdateExposure(float value) {
		ColorGradingModel.Settings colorGrading = profile.colorGrading.settings;
		colorGrading.basic.postExposure = value;
		profile.colorGrading.settings = colorGrading;
	}

	private void UpdateGlitch(float value) {
		glitch.SettingsTearing.Intensity = value;
	}

}
