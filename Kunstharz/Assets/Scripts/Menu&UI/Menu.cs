using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colorful;

public class Menu : MonoBehaviour {

	public GameObject textJoin;
	public GameObject textStart;
	public GameObject textExit;

	public GameObject blurJoin;
	public GameObject blurStart;
	public GameObject blurExit;

	public GameObject gameWorld;
	public GameObject canvas;

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

	GameObject [] textObjects = new GameObject[3];
	GameObject [] blurObjects = new GameObject[3];
	float [] blurObjectLength = {232f, 499f, 219f};

	float turnXOld = 0;
	float turnYOld = 0;

	void Start () {

		textObjects [0] = textJoin;
		textObjects [1] = textStart;
		textObjects [2] = textExit;

		blurObjects [0] = blurJoin;
		blurObjects [1] = blurStart;
		blurObjects [2] = blurExit;

    }

	void Update() {
		turnGameWorld ();
	}

	private void turnGameWorld() {
		float turnX = mouseXN () - turnXOld;
		float turnY = mouseYN () - turnYOld;

		//gameWorld.transform.eulerAngles += new Vector3(turnY * 8, turnX * 8, 0);
		canvas.transform.eulerAngles += new Vector3(turnY * 8, turnX * 8, 0);

		turnXOld = mouseXN ();
		turnYOld = mouseYN ();
	}

	public void Enter(int i) {
        
		for (int j = 0; j < 3; j++) {
			textObjects [j].GetComponent<Text> ().fontSize = 43;
			blurObjects [j].GetComponent<RawImage>().enabled = false;
		}
			
		textObjects [i].GetComponent<Text> ().fontSize = 122;

		//Set blur
		blurObjects [i].GetComponent<RawImage>().enabled = true;
		float height = blurObjects [i].GetComponent<RectTransform> ().rect.height;
		blurObjects [i].GetComponent<RectTransform> ().sizeDelta = new Vector2(0, height);
		LeanTween.size (blurObjects [i].GetComponent<RectTransform> (), new Vector2(blurObjectLength [i],height), 0.25f).setEase(LeanTweenType.easeInOutQuint);

        FMODUnity.RuntimeManager.PlayOneShot(hover, Camera.main.transform.position);
    }

	public void ExitGame () {
		Application.Quit();
		UnityEditor.EditorApplication.isPlaying = false;

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void JoinGame () {
		//TODO: Go to join scene
		Debug.Log("TODO: Start Join Game Menu");

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void StartGame () {
		//TODO: Go to start scene
		Debug.Log("TODO: Start Start Game Menu");

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	private float mouseXN() {
		return Input.mousePosition.x / (float) Screen.width * 2f - 1f;
	}

	private float mouseYN() {
		return Input.mousePosition.y / (float) Screen.height * 2f - 1f;
	}

}