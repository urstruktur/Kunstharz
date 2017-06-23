using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputBehaviour : MonoBehaviour {

	public GameObject button;

	void Update() {

		if (Input.anyKeyDown && !GetComponent<TMP_InputField>().isFocused) {
			if (HasALetterBeenPressed()) {
				string newText = GetComponent<TMP_InputField>().text + Input.inputString;
				if (newText.Length <= 10) GetComponent<TMP_InputField>().text += Input.inputString;
			}

			if (Input.GetKeyDown(KeyCode.Backspace) && GetComponent<TMP_InputField>().text.Length >= 1) {
				string newText = GetComponent<TMP_InputField>().text;
				newText = newText.Remove(newText.Length - 1);
				GetComponent<TMP_InputField>().text = newText;		
			}			
		}
		
		if (Input.GetKeyDown(KeyCode.Return) && !GetComponent<TMP_InputField>().text.Equals("")) {
			button.GetComponent<ButtonAction>().Clicked();
		}
	}

	private KeyCode[] desiredKeys = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z};
 
 	public bool HasALetterBeenPressed () {
    	foreach (KeyCode keyToCheck in desiredKeys) {
			if (Input.GetKeyDown (keyToCheck)) return true;
    	}
    	return false;
 	}
}
