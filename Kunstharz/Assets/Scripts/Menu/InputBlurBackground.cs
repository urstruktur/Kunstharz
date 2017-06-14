using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InputBlurBackground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	static int id = -1;
	GameObject effectCanvas;
	GameObject blur;
	public GameObject content;
	public GameObject startGameStartGameButton;
	public GameObject placeholderText;

	public string currentText = "";

	void Start() {
		blur = GameObject.Find("Blur");
		effectCanvas = GameObject.Find("Effect Canvas");
	}

	public void OnPointerEnter(PointerEventData eventData) {
		SetBlurPosition();
		SetBlurSize(currentText, true);
		blur.GetComponent<RawImage>().enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		GetComponentInChildren<TMP_InputField> ().DeactivateInputField();
		blur.GetComponent<RawImage>().enabled = false;
	}


	private void SetBlurPosition() {

		RectTransform rtButton = transform.GetComponent<RectTransform>();

		Vector2 position = new Vector2(
			effectCanvas.GetComponent<CanvasScaler>().referenceResolution.x * (-0.5f + rtButton.pivot.x) + rtButton.rect.width * (0.5f - rtButton.pivot.x), 
			effectCanvas.GetComponent<CanvasScaler>().referenceResolution.y * (-0.5f + rtButton.pivot.y) + rtButton.rect.height * (0.5f - rtButton.pivot.y)
		);

		blur.GetComponent<RectTransform>().anchoredPosition = position;

	}


	public void OnValueChangeInputFieldGameStart(string s) {
		SetBlurSize(s, false);
	}

	private void SetBlurSize(string s, bool enter) {

		if (enter) {
			LeanTween.cancel(Menu.tweenBlurId);
		}

		currentText = s.ToUpper();

		if (s == "") {
			startGameStartGameButton.SetActive(false);
			placeholderText.SetActive(false);
			s = "TYPE YOUR NAME";
		} else {
			startGameStartGameButton.SetActive(true);
			placeholderText.SetActive(true);
		}

		float width = content.GetComponent<TextMeshProUGUI>().GetPreferredValues(s).x + Menu.blurSpace;
		float height = content.GetComponent<TextMeshProUGUI>().GetPreferredValues(s).y + Menu.blurSpace / 4.0f;

		if (enter) {
			blur.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, height);
		}
		
		Menu.tweenBlurId = LeanTween.size (blur.GetComponent<RectTransform> (), new Vector2(width,height), 0.25f).setEase(LeanTweenType.easeInOutQuint).id;
	
	}

}
