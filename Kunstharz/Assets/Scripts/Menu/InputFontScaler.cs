using System.Collections;
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
		content.GetComponent<TextMeshProUGUI>().fontSize = Menu.textSizeSmallTextMeshPro;
		placeholder.GetComponent<TextMeshProUGUI>().fontSize = Menu.textSizeSmallTextMeshPro;
		Canvas.ForceUpdateCanvases();
	}

	private void TextBig() {
		content.GetComponent<TextMeshProUGUI>().fontSize = Menu.textSizeBigTextMeshPro;
		placeholder.GetComponent<TextMeshProUGUI>().fontSize = Menu.textSizeBigTextMeshPro;
		Canvas.ForceUpdateCanvases();
	}

}