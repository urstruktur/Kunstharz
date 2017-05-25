using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonImageScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public Vector3 smallScale = new Vector3(0.3f, 0.3f, 0.3f);
	public Vector3 bigScale = new Vector3(0.7f, 0.7f, 0.7f);

	public void OnPointerEnter(PointerEventData eventData) {
		ScaleBig();
	}

	public void OnPointerExit(PointerEventData eventData) {
		ScaleSmall();
	}

	private void ScaleBig() {
		transform.Find("Content").GetComponent<RectTransform>().localScale = bigScale;
	}

	private void ScaleSmall() {
		transform.Find("Content").GetComponent<RectTransform>().localScale = smallScale;
	}

}