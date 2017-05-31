using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonImageScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData eventData) {
		ScaleBig();
	}

	public void OnPointerExit(PointerEventData eventData) {
		ScaleSmall();
	}

	private void ScaleBig() {
		transform.Find("Content").GetComponent<RectTransform>().localScale = Menu.imageBigScale;
	}

	private void ScaleSmall() {
		transform.Find("Content").GetComponent<RectTransform>().localScale = Menu.imageSmallScale;
	}

}