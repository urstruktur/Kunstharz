using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler  {

	string originalText = "";
	string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	int currentAlphabetIndex;
	bool firstClick = true;
	public GameObject startGameStartGameButton;
	public Menu menu;
	public GameObject placeholder;

	void Start () {
	}

	void ChangeLastLetter() {
		if (originalText.Length > 0) {
			gameObject.transform.GetChild(0).GetComponent<Text>().text = originalText + " + " + alphabet[currentAlphabetIndex];
		} else {
			gameObject.transform.GetChild(0).GetComponent<Text>().text = "" + alphabet[currentAlphabetIndex];
		}

		currentAlphabetIndex += 1;
		if (currentAlphabetIndex == 26) {
			currentAlphabetIndex = 0;
		}
	}

    public void OnPointerClick(PointerEventData eventData) {
		if (firstClick) {
			gameObject.transform.GetChild(0).GetComponent<Text>().text = originalText;
			placeholder.SetActive(true);
			InvokeRepeating("ChangeLastLetter", 0f, 0.1f);
		} else {
			originalText += alphabet[currentAlphabetIndex];

			if (originalText=="SSS") {
				originalText = "ARNOLD SCHWARZENEGGER";
			}

			if (originalText.Length > 0) {
				menu.SetPlayerName(originalText);
				startGameStartGameButton.SetActive(true);
			}
		}

		firstClick = false;
    }

	public void ResetNameChanger() {
		gameObject.transform.GetChild(0).GetComponent<Text>().text = "CLICK TO TYPE YOUR NAME";
		firstClick = true;
		originalText = "";
		placeholder.SetActive(false);
		startGameStartGameButton.SetActive(false);
		CancelInvoke("ChangeLastLetter");
	}

    public void OnPointerExit(PointerEventData eventData) {
		if (!firstClick) {
			gameObject.transform.GetChild(0).GetComponent<Text>().text = originalText;
        	CancelInvoke("ChangeLastLetter");
			placeholder.SetActive(false);
		}
    }

    public void OnPointerEnter(PointerEventData eventData) {
		if (!firstClick) {
			InvokeRepeating("ChangeLastLetter", 0f, 0.1f);
			placeholder.SetActive(true);
		}
    }
}
