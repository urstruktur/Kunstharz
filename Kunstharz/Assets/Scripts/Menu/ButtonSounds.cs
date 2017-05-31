using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {

	public GameObject menuScripts;

		public void OnPointerEnter(PointerEventData eventData) {
			menuScripts.GetComponent<MenuSounds>().PlayHoverSound();
		}

    public void OnPointerClick (PointerEventData eventData) {
			menuScripts.GetComponent<MenuSounds>().PlayClickSound();
    }

}
