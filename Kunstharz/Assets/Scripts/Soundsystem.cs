﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz
{
    public class Soundsystem : MonoBehaviour {

        FMODUnity.StudioEventEmitter eventEmitterRef;

        [FMODUnity.EventRef]
        private string ambient = "event:/ambient";
        [FMODUnity.EventRef]
        private string landing = "event:/ui/click";
        [FMODUnity.EventRef]
        private string victory = "event:/ui/win";
        [FMODUnity.EventRef]
        private string defeat = "event:/ui/lose";
        [FMODUnity.EventRef]
        private string flying = "event:/fly";
        [FMODUnity.EventRef]
        private string shoot = "event:/shoot";
        [FMODUnity.EventRef]
        private string reject = "event:/reject";
        [FMODUnity.EventRef]
        private string newGameInLobby = "event:/newGameInLobby";

        // Use this for initialization
        void Start () {
            eventEmitterRef = GetComponent<FMODUnity.StudioEventEmitter>();
            FMODUnity.RuntimeManager.PlayOneShot(ambient);

            Finder.ChallengeDiscovered += ChallangeDiscovered;
        }

        void OnDestroy()
        {
            Finder.ChallengeDiscovered -= ChallangeDiscovered;
        }

        void ChallangeDiscovered(FinderEntry entry)
        {
            FMODUnity.RuntimeManager.PlayOneShot(newGameInLobby);
        }

        void PlayerStateChanged(Player changedPlayer)
        {
                if (changedPlayer.isLocalPlayer)
                {
                    switch (changedPlayer.state)
                    {
                        case PlayerState.SelectedMotion:
                            FMODUnity.RuntimeManager.PlayOneShot(landing, changedPlayer.transform.position);
                            break;
                        case PlayerState.Victorious:
                            FMODUnity.RuntimeManager.PlayOneShot(victory, changedPlayer.transform.position);
                            break;
                        case PlayerState.Dead:
                            FMODUnity.RuntimeManager.PlayOneShot(defeat, changedPlayer.transform.position);
                            break;
                        case PlayerState.ExecutingMotion:
                            FMODUnity.RuntimeManager.PlayOneShot(flying, changedPlayer.transform.position);
                            break;
                        case PlayerState.ExecutingShot:
                            FMODUnity.RuntimeManager.PlayOneShot(shoot, changedPlayer.transform.position);
                            break;
                }
                }
        }

        public void playMotionMissed()
        {
            FMODUnity.RuntimeManager.PlayOneShot(reject);
        }
    
        // Update is called once per frame
        void Update () {
		
	    }
    }

}