using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSwitcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

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
				LevelSwitcherLogic sli = switcherLogic.GetComponent<LevelSwitcherLogic>();
				sli.PrevLevel();
			} 
			if (right) {
				LevelSwitcherLogic sli = switcherLogic.GetComponent<LevelSwitcherLogic>();
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
