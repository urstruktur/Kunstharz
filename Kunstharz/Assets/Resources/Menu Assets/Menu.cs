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

	public GameObject EffectsCamera;
	public GameObject gameWorld;
	public GameObject canvas;

	Vector3 [] textPosition = new Vector3 [3];
	GameObject [] textObjects = new GameObject[3];
	GameObject [] blurObjects = new GameObject[3];
	float [] textWidth = new float[3];

	float turnXOld = 0;
	float turnYOld = 0;

	float phasePolarity = 1; 

	Vector4 [] cameras = new Vector4[3];

	void Start () {
		
		textPosition[0] = textJoin.GetComponent<RectTransform>().localPosition;
		textPosition[1] = textStart.GetComponent<RectTransform>().localPosition;
		textPosition[2] = textExit.GetComponent<RectTransform>().localPosition;

		textObjects [0] = textJoin;
		textObjects [1] = textStart;
		textObjects [2] = textExit;

		blurObjects [0] = blurJoin;
		blurObjects [1] = blurStart;
		blurObjects [2] = blurExit;

		textWidth[0] = textJoin.GetComponent<RectTransform> ().rect.width;
		textWidth[1] = textJoin.GetComponent<RectTransform> ().rect.width;
		textWidth[2] = textJoin.GetComponent<RectTransform> ().rect.width;

		cameras[0] = new Vector4(0.055f, 0.405f, 0.2f, 0.19f);
		cameras[1] = new Vector4(0.26f, 0.405f, 0.48f, 0.19f);
		cameras[2] = new Vector4(0.74f, 0.405f, 0.2f, 0.19f);

	}

	void Update() {
		turnGameWorld ();
		EffectsCamera.GetComponent<AnalogTV> ().Phase += 0.01f * phasePolarity;
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
			
		EffectsCamera.GetComponent<Camera> ().rect = new Rect (cameras[i].x, cameras[i].y, cameras[i].z, cameras[i].w);

		textObjects [i].GetComponent<Text> ().fontSize = 122;
		blurObjects [i].GetComponent<RawImage>().enabled = true;

		float width = blurObjects [i].GetComponent<RectTransform> ().rect.width;
		float height = blurObjects [i].GetComponent<RectTransform> ().rect.height;

		blurObjects [i].GetComponent<RectTransform> ().sizeDelta = new Vector2(0, height);

		LeanTween.size (
			blurObjects [i].GetComponent<RectTransform> (),
			new Vector2(width,height),
			0.25f).setEase(LeanTweenType.easeInOutQuint);

		phasePolarity *= -1;
	}

	public void ExitGame () {
		Application.Quit();
		UnityEditor.EditorApplication.isPlaying = false;
	}

	public void JoinGame () {
		//TODO: Go to join scene
		Debug.Log("TODO: Start Join Game Menu");
	}

	public void StartGame () {
		//TODO: Go to start scene
		Debug.Log("TODO: Start Start Game Menu");
	}

	private float mouseXN() {
		return Input.mousePosition.x / (float) Screen.width * 2f - 1f;
	}

	private float mouseYN() {
		return Input.mousePosition.y / (float) Screen.height * 2f - 1f;
	}

}