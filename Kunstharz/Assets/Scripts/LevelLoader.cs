using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Responsible for preparing a level to be played.
/// </summary>

namespace Kunstharz {
	public class LevelLoader : MonoBehaviour {
	    public GameObject gameEssentialsPrefab;
	    private GameObject currentLevel;
        public GameObject testLevelGeometry;

	    public GameObject[] toDeactivate;

		public string[] levelSceneNames;

		private bool canLoad = true;
        
	    /// <summary>
	    /// Parameter being the level geometry to be prepared for playing.
	    /// </summary>
	    public void LoadLevel(GameObject level) {
	        foreach(GameObject o in toDeactivate){
	            //o.SetActive(false);
	        }
            currentLevel = level;
	        //currentLevel = Instantiate(level, level.transform.position, level.transform.rotation);
	        currentLevel.SetActive(true);
	        //currentLevel.transform.parent = null; // set to top hierarchy
	        //Instantiate(gameEssentialsPrefab);

	        StartLevel(level);
	    }

	    /// <summary>
	    /// initializes game and tweens camera to spawn point
	    /// </summary>
	    void StartLevel(GameObject level)
	    {
	        // !!! TODO: get genuine spawn location !!!

	        // get spawn points
	        GameObject parent = level.transform.Find("SpawnLocations").gameObject;
	        GameObject spawnLocation = null;
	        if (parent != null)
	        {
	            spawnLocation = parent.transform.GetChild(0).gameObject;
            }
	        
	        if (spawnLocation != null)
	        {
	            // tweens camera to spawnLocation
	            LeanTween.move(Camera.main.gameObject, spawnLocation.transform.position, 3f).setEase(LeanTweenType.easeOutExpo);
            }else
            {
                Debug.Log("no spawn point found");
            }

            ProjectionController projection = Camera.main.GetComponent<ProjectionController>();
            if (projection == null)
            {
                Debug.LogError("No ProjectionController on camera!");
            }
            projection.lerp = true;
        }

		public void StartUgly(int selectedLevelIdx) {
            StartUgly(levelSceneNames[selectedLevelIdx]);


		}

		public void StartUgly(string levelSceneName) {
            /*LoadLevel(testLevelGeometry);
            LeanTween.delayedCall(3f, () => {*/

                if (canLoad && !NetworkManager.singleton.isNetworkActive)
                {
                    canLoad = false;
					NetworkControl control = (NetworkControl) NetworkManager.singleton;
					//control.onlineScene = levelSceneName;
					control.networkPort = Kunstharz.NetworkSpecs.GAME_HOST_PORT;
					control.maxConnections = 2;
					control.StartHost();
                    //DontDestroyOnLoad(GameObject.Find("NetworkDiscovery"));
                }

            //});
		}

		public void JoinUgly(string hostname) {
			if (canLoad && !NetworkManager.singleton.isNetworkActive) {
				canLoad = false;
				print ("Connecting with " + hostname);
				NetworkManager.singleton.networkAddress = hostname;
				NetworkManager.singleton.networkPort = Kunstharz.NetworkSpecs.GAME_HOST_PORT;
				NetworkManager.singleton.StartClient ();
			}
		}
	}
}