using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public GameObject diamond;
	public GameObject tank;

	private const float D_TIMER = 6f;
	private const float T_TIMER = 12f;

	private float diamondTimer;
	private float tankTimer;

	// Use this for initialization
	void Start () {
		diamondTimer = Random.Range(D_TIMER, T_TIMER);
		tankTimer = Random.Range(D_TIMER, T_TIMER);
	}
	
	// Update is called once per frame
	void Update () {
		diamondTimer -= Time.deltaTime;
		tankTimer -= Time.deltaTime;

		if(diamondTimer <= 0) {
			PerformPulseAnimation();
			diamondTimer = Random.Range(D_TIMER, T_TIMER);
		}

		if(tankTimer <= 0) {
			PerformTankMechanicAnimation();
			tankTimer = Random.Range(D_TIMER, T_TIMER);
		}
	}

	private void PerformPulseAnimation () {
		LeanTween.rotate(diamond, new Vector3(7f, 15f, 7f), 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
			LeanTween.rotate(diamond, new Vector3(-7f, -15f, -7f), 0.05f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
				LeanTween.rotate(diamond, new Vector3(0f, 0f, 0f), 0.05f).setEase(LeanTweenType.easeInOutQuad);
			}).setLoopPingPong(7);
		});
	}

	private void PerformTankMechanicAnimation () {
		LeanTween.scale(tank, new Vector3(1.2f, 1f, 1.2f), 0.1f).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
			LeanTween.rotateAround(tank, Vector3.up, 360f, 0.6f).setEase(LeanTweenType.easeInOutBack).setOnComplete(() => {
				LeanTween.scale(tank, new Vector3(1f, 1f, 1f), 0.05f).setEase(LeanTweenType.easeOutBack);
			});
		});
	}
}
