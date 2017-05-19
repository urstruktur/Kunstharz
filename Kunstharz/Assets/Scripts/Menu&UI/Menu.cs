using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colorful;

public class Menu : MonoBehaviour {

	[Header("Menu Text Objects")]
	public GameObject [] textObjects = new GameObject[3];

	[Header("Menu Blur Objects")]
	public GameObject [] blurObjects = new GameObject[3];

	[Header("Other Parameters")]
	public GameObject generalCanvas;
	public GameObject topLevelMenu;
	public GameObject pickNameMenu;

	float [] blurObjectLength = {232f, 499f, 219f};
	float turnXOld = 0;
	float turnYOld = 0;

	[ContextMenu("Reset The Value")]
	private void resetTheValue()  
	{
		turnXOld = 42;
	}

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

	public Kunstharz.Publisher publisher;
	public Kunstharz.Finder finder;

	void Update() {
		turnGameWorld ();
		UpdateBeacon ();
	}

	private void UpdateBeacon() {
		// Updates the beacon to be sent when publishing game on the network
		if (publisher.enabled) {
			string beacon = pickNameMenu.GetComponentInChildren<InputField> ().text;
			publisher.beacon = beacon;
		}
	}

	private void turnGameWorld() {
		float turnX = mouseXN () - turnXOld;
		float turnY = mouseYN () - turnYOld;

		generalCanvas.transform.eulerAngles += new Vector3(turnY * 8, turnX * 8, 0);

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

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void JoinGame () {
		//TODO: Go to join scene
		Debug.Log("TODO: Start Join Game Menu");

		finder.enabled = true;
		publisher.enabled = false;

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void StartGame () {
		//TODO: Go to start scene
		Debug.Log("TODO: Start Start Game Menu");

		topLevelMenu.SetActive (false);
		pickNameMenu.SetActive (true);

		publisher.enabled = true;
		finder.enabled = false;

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	private float mouseXN() {
		return Input.mousePosition.x / (float) Screen.width * 2f - 1f;
	}

	private float mouseYN() {
		return Input.mousePosition.y / (float) Screen.height * 2f - 1f;
	}

}