using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Switcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject switcherLogic;
	public bool left = false;
	public bool right = false;
	private bool canSwitch = false;
	
	void Update () {
		ChangeGameWorlds();
	}

	private void ChangeGameWorlds() {
		if (canSwitch) {
			if (left) {
				ISwitcherLogic sli = switcherLogic.GetComponent(typeof(ISwitcherLogic)) as ISwitcherLogic;
				sli.PrevLevel();
			} 
			if (right) {
				ISwitcherLogic sli = switcherLogic.GetComponent(typeof(ISwitcherLogic)) as ISwitcherLogic;
				sli.NextLevel();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		canSwitch = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		canSwitch = false;
	}
}
