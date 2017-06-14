using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class InputPlaceholderBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject placeholder;

	public void OnPointerEnter(PointerEventData eventData) {
		LeanTween.move(placeholder.GetComponent<RectTransform>(), new Vector3(0,140f,0f), 0.25f).setEase(LeanTweenType.easeInOutQuint);
		//LeanTween.value(gameObject, updateColorOpacity, 0f, 1f, 0.25f).setEase(LeanTweenType.easeInOutQuint);
		LeanTween.value(gameObject, updateFontSize, 20f, 45f, 0.25f).setEase(LeanTweenType.easeInOutQuint);
	}

	public void OnPointerExit(PointerEventData eventData) {
		LeanTween.move(placeholder.GetComponent<RectTransform>(), new Vector3(0,70,0f), 0.25f).setEase(LeanTweenType.easeInOutQuint);
		//LeanTween.value(gameObject, updateColorOpacity, 1f, 0f, 0.25f).setEase(LeanTweenType.easeInOutQuint);
		LeanTween.value(gameObject, updateFontSize, 45f, 20f, 0.25f).setEase(LeanTweenType.easeInOutQuint);
	}

	void updateColorOpacity(float val){
		placeholder.GetComponent<Text>().color = new Color(1f, 1f, 1f, val);
	}	

	void updateFontSize(float val) {
		placeholder.GetComponent<Text>().fontSize = (int) val;
	}

}
