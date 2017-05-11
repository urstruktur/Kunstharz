using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class PostProcessingCamera : MonoBehaviour {

	private Vector3 oldCameraRotation;
	PostProcessingProfile profile;

	void OnEnable() {
		PostProcessingBehaviour behaviour = GetComponent<PostProcessingBehaviour> ();
		profile = Instantiate(behaviour.profile);
        behaviour.profile = profile;
	}
	
	void Update () {
		SetChromaticAberration(GetCameraRotationVelocity()/10f);
	}

	private void SetChromaticAberration(float value) {
		ChromaticAberrationModel.Settings chromaticAbberation = profile.chromaticAberration.settings;
		chromaticAbberation.intensity = Mathf.Clamp (value, 0.2f, 0.6f);
		Debug.Log(chromaticAbberation.intensity);
		profile.chromaticAberration.settings = chromaticAbberation;
	}

	private float GetCameraRotationVelocity() {
		float distance = Vector3.Distance(transform.eulerAngles, oldCameraRotation);
		oldCameraRotation = transform.eulerAngles;
		return distance;
	}
}
