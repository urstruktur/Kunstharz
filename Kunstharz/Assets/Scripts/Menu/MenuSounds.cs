using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSounds : MonoBehaviour {

	[Header("FMOD Events")]

    [FMODUnity.EventRef]
    public string hover = "event:/ui/hover";

    [FMODUnity.EventRef]
    public string click = "event:/ui/click";

    [FMODUnity.EventRef]
    public string select = "event:/ui/click";

    [FMODUnity.EventRef]
    public string typing = "event:/ui/type";

    [FMODUnity.EventRef]
    public string zoomIn = "event:/ui/click";

    [FMODUnity.EventRef]
    public string zoomOut = "event:/ui/click";

    [FMODUnity.EventRef]
    public string hostGame = "event:/ui/gameHosted";

    void Start () {
		LevelSwitcherLogic.OnLevelSwitch += PlayClickSound;
	}

    public void PlayHostingSound()
    {
        if (Menu.canTurn)
        {
            FMODUnity.RuntimeManager.PlayOneShot(hostGame, Camera.main.transform.position);
        }
    }

    public void PlayHoverSound() {
        if (Menu.canTurn) {
            FMODUnity.RuntimeManager.PlayOneShot(hover, Camera.main.transform.position);
        }   
    }

	public void PlayClickSound() {
        if (Menu.canTurn) {
            FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
        } 
	}

    public void PlayTypeSound()
    {
        if (Menu.canTurn)
        {
            FMODUnity.RuntimeManager.PlayOneShot(typing);
        }
    }

    void OnDestroy() {
		LevelSwitcherLogic.OnLevelSwitch -= PlayClickSound;
	}

}
