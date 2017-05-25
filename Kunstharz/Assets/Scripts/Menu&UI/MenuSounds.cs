using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSounds : MonoBehaviour {

	[Header("FMOD Events")]

    [FMODUnity.EventRef]
    public string hover = "event:/ui/hover";

    [FMODUnity.EventRef]
    public string click = "event:/ui/click";

    [FMODUnity.EventRef]
    public string select = "event:/ui/click";

    [FMODUnity.EventRef]
    public string typing = "event:/ui/click";

    [FMODUnity.EventRef]
    public string zoomIn = "event:/ui/click";

    [FMODUnity.EventRef]
    public string zoomOut = "event:/ui/click";

	void Start () {
		LevelSwitcherLogic.OnLevelSwitch += PlayClickSound;
	}

	public void PlayHoverSound() {
        FMODUnity.RuntimeManager.PlayOneShot(hover, Camera.main.transform.position);
    }

	private void PlayClickSound() {
		FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
	}

	void OnDestroy() {
		LevelSwitcherLogic.OnLevelSwitch -= PlayClickSound;
	}

}
