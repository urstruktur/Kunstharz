﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InputFontScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject placeholder;
	public GameObject content;

	public void OnPointerEnter(PointerEventData eventData) {
		TextBig();
	}

	public void OnPointerExit(PointerEventData eventData) {
		TextSmall();
	}

	private void TextSmall() {
		content.GetComponent<TextMeshProUGUI>().fontSize = 52;
		placeholder.GetComponent<TextMeshProUGUI>().fontSize = 52;
		Canvas.ForceUpdateCanvases();
	}

	private void TextBig() {
		content.GetComponent<TextMeshProUGUI>().fontSize = 130;
		placeholder.GetComponent<TextMeshProUGUI>().fontSize = 130;
		Canvas.ForceUpdateCanvases();
	}

}