using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public GameObject diamond;
	public GameObject tank;

	private const float D_TIMER = 4f;
	private const float T_TIMER = 6f;

	private float diamondTimer = D_TIMER;
	private float tankTimer = T_TIMER;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		diamondTimer -= Time.deltaTime;

		if(diamondTimer <= 0) {
			PerformPulseAnimation();
			diamondTimer = D_TIMER;
		}
	}

	private void PerformPulseAnimation () {
		LeanTween.rotateAround(diamond, Vector3.up, 360f, 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
			LeanTween.rotateZ(diamond, 1f, 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
				LeanTween.rotateZ(diamond, -5f, 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
					LeanTween.rotateZ(diamond, 0f, 0.05f).setEase(LeanTweenType.easeInOutQuad);
				}).setLoopPingPong(7);
			});
		});
	}
}
