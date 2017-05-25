using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonFontScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	GameObject content;

	public int textSizeSmall = 43;

	public int textSizeBig = 122;

	public void OnPointerEnter(PointerEventData eventData) {
		TextBig();
	}

	public void OnPointerExit(PointerEventData eventData) {
		TextSmall();
	}

	private void TextSmall() {
		GetContent().GetComponent<Text>().fontSize = textSizeSmall;
		Canvas.ForceUpdateCanvases();
	}

	private void TextBig() {
		GetContent().GetComponent<Text>().fontSize = textSizeBig;
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
