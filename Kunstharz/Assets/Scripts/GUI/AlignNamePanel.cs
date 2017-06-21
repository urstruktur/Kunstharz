using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignNamePanel : MonoBehaviour {

	public GameObject canvas;

	[ExecuteInEditMode]
	void Start() {
		GetComponent<RectTransform>().localPosition = new Vector2(0, canvas.GetComponent<RectTransform>().sizeDelta.y / 2.0f);
	}
	
}
