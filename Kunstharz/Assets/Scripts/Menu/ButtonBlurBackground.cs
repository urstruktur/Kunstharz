using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBlurBackground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	GameObject content;
	GameObject effectCanvas;
	GameObject blur;
	GameObject textChild;

	string text = "";
	int textLength;

	bool pointerInside = false;
	
	void Start() {
		blur = GameObject.Find("Blur");
		effectCanvas = GameObject.Find("Effect Canvas");
		textChild = transform.GetChild(0).gameObject;

		if (textChild.transform.GetComponent<Text>() != null) {
			text = textChild.transform.GetComponent<Text>().text;
			textLength = textChild.transform.GetComponent<Text>().text.Length;
		} else {
			text = "";
		}
	}

	void Update() {
		if (textChild.transform.GetComponent<Text>() != null) {
			if (textLength != textChild.transform.GetComponent<Text>().text.Length && pointerInside) {
				Debug.Log("Setting blur size");
				textLength = textChild.transform.GetComponent<Text>().text.Length;
				SetBlurSize(false);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		pointerInside = true;
		SetBlurPosition();
		SetBlurSize(true);
		blur.GetComponent<RawImage>().enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		pointerInside = false;
		blur.GetComponent<RawImage>().enabled = false;
	}

	void OnDisable() {
		if (pointerInside) {
			blur.GetComponent<RawImage>().enabled = false;
		}
	}


	private void SetBlurPosition() {

		RectTransform rtButton = transform.GetComponent<RectTransform>();

		Vector2 position = new Vector2(
			effectCanvas.GetComponent<CanvasScaler>().referenceResolution.x * (-0.5f + rtButton.pivot.x) + rtButton.rect.width * (0.5f - rtButton.pivot.x), 
			effectCanvas.GetComponent<CanvasScaler>().referenceResolution.y * (-0.5f + rtButton.pivot.y) + rtButton.rect.height * (0.5f - rtButton.pivot.y)
		);

		blur.GetComponent<RectTransform>().anchoredPosition = position;

	}

	private void SetBlurSize(bool enter) {

		if (enter) {
			LeanTween.cancel(Menu.tweenBlurId);
		}

		float width = GetContent().GetComponent<RectTransform>().rect.size.x + Menu.blurSpace;
		float height = GetContent().GetComponent<RectTransform>().rect.size.y + Menu.blurSpace / 4.0f;

		if (enter) {
			blur.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
		}

		Vector2 size = new Vector2(width, height);

		Menu.tweenBlurId = LeanTween.size (blur.GetComponent<RectTransform>(), size, 0.25f).setEase(LeanTweenType.easeInOutQuint).id;
	}

	private GameObject GetContent() {
		if (content == null) {
			return transform.GetChild(0).gameObject;
		}
		return content;
	}

}
