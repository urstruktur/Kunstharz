using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputBehaviour : MonoBehaviour {

	public GameObject button;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Return) && !GetComponent<TMP_InputField>().text.Equals("")) {
			button.GetComponent<ButtonAction>().Clicked();
		}
	}
}
