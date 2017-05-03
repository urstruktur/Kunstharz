using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{

    public class Soundsystem : MonoBehaviour {

        FMODUnity.StudioEventEmitter eventEmitterRef;
        FMOD.Studio.EventInstance[] playerState;

        [FMODUnity.EventRef]
        public string landing = "event:/click";
        [FMODUnity.EventRef]
        public string victory = "event:/win";
        [FMODUnity.EventRef]
        public string defeat = "event:/lose";
        [FMODUnity.EventRef]
        public string flying = "event:/fly";

        // Use this for initialization
        void Start () {
            eventEmitterRef = GetComponent<FMODUnity.StudioEventEmitter>();
        }

        void PlayerStateChanged(Player changedPlayer)
        {
                if (changedPlayer.isLocalPlayer)
                {
                    switch (changedPlayer.state)
                    {
                        case PlayerState.SelectedMotion:
                            FMODUnity.RuntimeManager.PlayOneShot(landing);
                            break;
                        case PlayerState.Victorious:
                            FMODUnity.RuntimeManager.PlayOneShot(victory);
                            break;
                        case PlayerState.Dead:
                            FMODUnity.RuntimeManager.PlayOneShot(defeat);
                            break;
                        case PlayerState.ExecutingMotion:
                            FMODUnity.RuntimeManager.PlayOneShot(flying);
                            break;
                    }
                    Debug.Log("eventually playing sound");
                }
        }
    
        // Update is called once per frame
        void Update () {
		
	    }
    }

}