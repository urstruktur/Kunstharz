using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBlurBackground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	GameObject content;
	GameObject effectCanvas;
	GameObject blur;
	public int blurSpace = 80;

	void Start() {
		blur = GameObject.Find("Blur");
		effectCanvas = GameObject.Find("Effect Canvas");
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

		blur.GetComponent<RectTransform>().sizeDelta = new Vector2(0, blur.GetComponent<RectTransform>().rect.height);

		Vector2 size = new Vector2(
			GetContent().GetComponent<RectTransform>().rect.size.x + blurSpace,
			blur.GetComponent<RectTransform>().rect.height
		);

		Menu.tweenBlurId = LeanTween.size (blur.GetComponent<RectTransform>(), size, 0.25f).setEase(LeanTweenType.easeInOutQuint).id;
	}

	private GameObject GetContent() {
		if (content == null) {
			return transform.GetChild(0).gameObject;
		}
		return content;
	}

}
