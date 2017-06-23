using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public GameObject diamond;

	public GameObject tank;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		LeanTween.move(diamond, new Vector3(1,1,1), 5.0f);
	}
}
