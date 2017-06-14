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

	void Start() {
		blur = GameObject.Find("Blur");
		effectCanvas = GameObject.Find("Effect Canvas");
		textChild = transform.GetChild(0).gameObject;

		if (textChild.transform.GetComponent<Text>() != null) {
			text = textChild.transform.GetComponent<Text>().text;
		} else {
			text = "";
		}

	}

	void Update() {
		if (textChild.transform.GetComponent<Text>() != null) {
			if (text != textChild.transform.GetComponent<Text>().text) {
				Debug.Log("Setting blur size");
				SetBlurSize();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		SetBlurPosition();
		SetBlurSize();
		blur.GetComponent<RawImage>().enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
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

	private void SetBlurSize() {
		LeanTween.cancel(Menu.tweenBlurId);

		float width = GetContent().GetComponent<RectTransform>().rect.size.x + Menu.blurSpace;
		float height = GetContent().GetComponent<RectTransform>().rect.size.y + Menu.blurSpace / 4.0f;

		blur.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);

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
