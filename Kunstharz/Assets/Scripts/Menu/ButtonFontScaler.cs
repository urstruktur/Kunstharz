using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonFontScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	GameObject content;

	public void OnPointerEnter(PointerEventData eventData) {
		TextBig();
	}

	public void OnPointerExit(PointerEventData eventData) {
		TextSmall();
	}

	private void TextSmall() {
		GetContent().GetComponent<Text>().fontSize = Menu.textSizeSmall;
		Canvas.ForceUpdateCanvases();
	}

	private void TextBig() {
		GetContent().GetComponent<Text>().fontSize = Menu.textSizeBig;
		Canvas.ForceUpdateCanvases();
	}

	private GameObject GetContent() {
		if (content == null) {
			return transform.GetChild(0).gameObject;
		}
		return content;
	}

	void OnEnable() {
		TextSmall();
	}

}
