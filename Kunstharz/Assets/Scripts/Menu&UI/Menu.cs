using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colorful;
using TMPro;

public class Menu : MonoBehaviour {

	[Header("Menu Text Objects")]
	public GameObject [] textObjects;
	public GameObject [] imageObjects;
	GameObject [] blurObjects;
	[Header("Other Parameters")]
	public GameObject generalCanvas;
	public GameObject effectCanvas;
	public GameObject [] menus;
	public GameObject startGameInputFieldBlur;
	public GameObject startGameInputFieldText;
	public GameObject startGameInputFieldPlaceholder;
	public GameObject startGameStartGameButton;
	public Kunstharz.Publisher publisher;
	public Kunstharz.Finder finder;


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

	void Start() {
		blurObjects = new GameObject [effectCanvas.transform.childCount];
		for (int i = 0; i < effectCanvas.transform.childCount; i++) {
			blurObjects[i] = effectCanvas.transform.GetChild(i).gameObject;
		}

	}

	void Update() {
		turnGameWorld ();
		UpdateBeacon ();
	}

	private void UpdateBeacon() {
		// Updates the beacon to be sent when publishing game on the network
		if (publisher.enabled) {
			string beacon = menus[2].GetComponentInChildren<TMP_InputField> ().text;
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

	public void _EnterGeneral() {
		menus[2].GetComponentInChildren<TMP_InputField> ().DeactivateInputField();
        FMODUnity.RuntimeManager.PlayOneShot(hover, Camera.main.transform.position);
    }

	public void _EnterText(GameObject obj) {
		for (int j = 0; j < textObjects.Length; j++) {
			if (textObjects [j].GetComponent<Text> () == null) {
				textObjects [j].GetComponent<TextMeshProUGUI>().fontSize = 52;
				startGameInputFieldPlaceholder.GetComponent<TextMeshProUGUI>().fontSize = 52;
			} else {
				textObjects [j].GetComponent<Text> ().fontSize = 43;
			}
		}
		
		if (obj.GetComponent<Text> () == null) {
			obj.GetComponent<TextMeshProUGUI>().fontSize = 130;
			startGameInputFieldPlaceholder.GetComponent<TextMeshProUGUI>().fontSize = 130;
		} else {
			obj.GetComponent<Text> ().fontSize = 122;
		}
			
	}

	public void _EnterText() {
		for (int j = 0; j < textObjects.Length; j++) {
			if (textObjects [j].GetComponent<Text> () == null) {
				textObjects [j].GetComponent<TextMeshProUGUI>().fontSize = 52;
				startGameInputFieldPlaceholder.GetComponent<TextMeshProUGUI>().fontSize = 52;
			} else {
				textObjects [j].GetComponent<Text> ().fontSize = 43;
			}
		}	
	}

	public void _EnterBlur(GameObject obj) {
		for (int j = 0; j < blurObjects.Length; j++) {
			blurObjects [j].GetComponent<RawImage>().enabled = false;
		}

		obj.GetComponent<RawImage>().enabled = true;
		float height = obj.GetComponent<RectTransform> ().rect.height;
		float width = obj.GetComponent<RectTransform>().rect.width;
		obj.GetComponent<RectTransform> ().sizeDelta = new Vector2(0, height);
		LeanTween.size (obj.GetComponent<RectTransform> (), new Vector2(width,height), 0.25f).setEase(LeanTweenType.easeInOutQuint);
	}

	public void _EnterBlur() {
		for (int j = 0; j < blurObjects.Length; j++) {
			blurObjects [j].GetComponent<RawImage>().enabled = false;
		}
	}

	public void _EnterImage(GameObject obj) {
		for (int j = 0; j < imageObjects.Length; j++) {
			imageObjects [j].GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
		}
		
		obj.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
	}

	public void _EnterImage() {
		for (int j = 0; j < imageObjects.Length; j++) {
			imageObjects [j].GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
		}
	}

	private float mouseXN() {
		return Input.mousePosition.x / (float) Screen.width * 2f - 1f;
	}

	private float mouseYN() {
		return Input.mousePosition.y / (float) Screen.height * 2f - 1f;
	}

	public void OnValueChangeInputFieldGameStart(string s) {

		if (s != "") {
			startGameStartGameButton.SetActive(true);
		} else {
			startGameStartGameButton.SetActive(false);
			s = "TYPE YOUR NAME";
		}

		float width = startGameInputFieldText.GetComponent<TextMeshProUGUI>().GetPreferredValues(s).x + 100;
		float height = startGameInputFieldBlur.GetComponent<RectTransform> ().rect.height;

		startGameInputFieldBlur.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		LeanTween.size (startGameInputFieldBlur.GetComponent<RectTransform> (), new Vector2(width,height), 0.25f).setEase(LeanTweenType.easeInOutQuint);
	}

	private void SetMenusInactive() {
		for (int i = 0; i < menus.Length; i++) {
			menus[i].SetActive(false);
		}

		for (int i = 0; i < blurObjects.Length; i++) {
			blurObjects [i].GetComponent<RawImage>().enabled = false;
		}
	}

	public void TopLevelMenu() {
		publisher.enabled = false;
		finder.enabled = false;

		SetMenusInactive();
		menus[0].SetActive (true);
	}

	public void ExitGame () {
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif

        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void JoinGame () {
		finder.enabled = true;
		publisher.enabled = false;
        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void StartGameChooseLevelMenu () {
		SetMenusInactive();
		menus[1].SetActive (true);
        FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
    }

	public void StartGameChooseNameMenu() {
		publisher.enabled = true;
		finder.enabled = false;
		SetMenusInactive();
		menus[2].SetActive (true);
		FMODUnity.RuntimeManager.PlayOneShot(click, Camera.main.transform.position);
	}

}