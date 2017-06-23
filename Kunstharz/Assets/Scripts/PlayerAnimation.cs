using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public GameObject diamond;
	public GameObject tank;

	private const float D_TIMER = 8f;
	private const float T_TIMER = 12f;

	private float diamondTimer = D_TIMER;
	private float tankTimer = T_TIMER;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		diamondTimer -= Time.deltaTime;
		tankTimer -= Time.deltaTime;

		if(diamondTimer <= 0) {
			PerformPulseAnimation();
			diamondTimer = D_TIMER;
		}

		if(tankTimer <= 0) {
			PerformTankMechanicAnimation();
			tankTimer = T_TIMER;
		}
	}

	private void PerformPulseAnimation () {
		LeanTween.rotateZ(diamond, 5f, 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
				LeanTween.rotateZ(diamond, -5f, 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
					LeanTween.rotateZ(diamond, 0f, 0.05f).setEase(LeanTweenType.easeInOutQuad);
				}).setLoopPingPong(7);
			});
	}

	private void PerformTankMechanicAnimation () {
		LeanTween.scale(tank, new Vector3(1.2f, 1f, 1.2f), 0.1f).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
			LeanTween.rotateAround(tank, Vector3.up, 360f, 0.75f).setEase(LeanTweenType.easeInOutBack).setOnComplete(() => {
				LeanTween.scale(tank, new Vector3(1f, 1f, 1f), 0.1f).setEase(LeanTweenType.easeOutBack);
			});
		});
	}
}
