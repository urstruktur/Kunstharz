using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Responsible for preparing a level to be played.
/// </summary>

namespace Kunstharz {
	public class LevelLoader : MonoBehaviour {
	    Matrix4x4 orthographic;
	    Matrix4x4 perspective;
	    private Camera cam;

	    public GameObject gameEssentialsPrefab;
	    private GameObject currentLevel;

	    public GameObject[] toDeactivate;

		public string[] levelSceneNames;

		private bool canLoad = true;

	    void Start()
	    {
	        cam = Camera.main;

	        Debug.Log("amen");

	        // initialize projection matrices
	        orthographic = Matrix4x4.Ortho(-cam.orthographicSize * cam.aspect, cam.orthographicSize * cam.aspect,
	            -cam.orthographicSize, cam.orthographicSize, -15, cam.farClipPlane);
	        perspective = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, 0, cam.farClipPlane);
	    }

	    /// <summary>
	    /// Parameter being the level geometry to be prepared for playing.
	    /// </summary>
	    public void LoadLevel(GameObject level) {
	        foreach(GameObject o in toDeactivate){
	            o.SetActive(false);
	        }
	        currentLevel = Instantiate(level, level.transform.position, level.transform.rotation);
	        currentLevel.SetActive(true);
	        currentLevel.transform.parent = null; // set to top hierarchy
	        Instantiate(gameEssentialsPrefab);

	        Camera.main.transform.position = cam.transform.position;
	        Camera.main.transform.rotation = cam.transform.rotation;
	        //Camera.main.orthographic = true;
	        //Camera.main.projectionMatrix = orthographic;

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
	            LeanTween.move(cam.gameObject, spawnLocation.transform.position, 1f);
	        }

	        // tweens from orthographic to perspective
	        LeanTween.value(gameObject, matrixLerpCallback, 0f, 1f, 1f).setEase(LeanTweenType.easeOutElastic);
	    }

	    void matrixLerpCallback(float val)
	    {
	        cam.projectionMatrix = MatrixLerp(orthographic, perspective, val);
	    }

	    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float amount)
	    {
	        Matrix4x4 ret = new Matrix4x4();
	        for (int i = 0; i < 16; i++)
	            ret[i] = Mathf.Lerp(from[i], to[i], amount);
	        return ret;
	    }

		public void StartUgly(int selectedLevelIdx) {
			StartUgly (levelSceneNames [selectedLevelIdx]);
		}

		public void StartUgly(string levelSceneName) {
			if (canLoad && !NetworkManager.singleton.isNetworkActive) {
				canLoad = false;
				NetworkManager.singleton.onlineScene = levelSceneName;
				NetworkManager.singleton.networkPort = Kunstharz.NetworkSpecs.GAME_HOST_PORT;
				DontDestroyOnLoad(GameObject.Find("NetworkDiscovery"));
				NetworkManager.singleton.StartHost ();
			}
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