using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kunstharz
{
	public class Gui : MonoBehaviour {

		public Text p1ScoreText;
		public Text p2ScoreText;
		public Text generalScore;

		public Text p1Action;
		public Text p2Action;

		public Text p1Name;
		public Text p2Name;

		public Image[] scoreImages = new Image [6];

		public GameObject GUIAnchor;
		public float speed = 5f;

		public GameObject moveImage;

		public GameObject shootImage;

		private Vector3 currentAngle;
		private float currentVelocity;


		bool slug = true;

		void Start() {
			currentAngle = GUIAnchor.transform.eulerAngles;

			GUIAnchor.transform.position = Camera.main.transform.position;
			GUIAnchor.transform.eulerAngles = Camera.main.transform.eulerAngles;
		}

		public void UpdateScore(Game game) {
			Player local = game.localPlayer;
			Player other = game.nonLocalPlayer;

			p1ScoreText.text = ""+local.wins;
			p2ScoreText.text = ""+other.wins;

			generalScore.text = local.wins + ":" + other.wins;

			for (int i = 0; i < 3; i++) {
				if (i < local.wins) {
					scoreImages[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
				} else {
					scoreImages[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
				}
			}

			for (int i = 0; i < 3; i++) {
				if (i < other.wins) {
					scoreImages[i + 3].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
				} else {
					scoreImages[i + 3].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
				}
			}
			
		}

		public void UpdatePlayerStates(Game game) {
			p1Action.text = ""+game.localPlayer.state;
			p2Action.text = ""+game.nonLocalPlayer.state;
		}

		public void UpdatePlayerNames(Game game) {
			p1Action.text = ""+game.localPlayer.name;
			p2Action.text = ""+game.nonLocalPlayer.name;	
		}

		void LateUpdate() {
			UISlug();

			if (Input.GetKeyDown(KeyCode.O)) {
				ShowMoveInstruction(0.5f);
			}

			if (Input.GetKeyDown(KeyCode.P)) {
				ShowShootInstruction(0.5f);
			}

		}

		private void UISlug() {

			Vector3 currentCameraAngle = Camera.main.transform.eulerAngles;

			GUIAnchor.transform.position = Camera.main.transform.position;

			currentAngle = new Vector3 (
				Mathf.SmoothDampAngle (currentAngle.x, currentCameraAngle.x, ref currentVelocity, Time.deltaTime * speed),
				Mathf.SmoothDampAngle (currentAngle.y, currentCameraAngle.y, ref currentVelocity, Time.deltaTime * speed * 1.5f),
				Mathf.SmoothDampAngle (currentAngle.z, currentCameraAngle.z, ref currentVelocity, Time.deltaTime * speed));
	
			GUIAnchor.transform.eulerAngles = currentAngle;

		}

		public void ShowMoveInstruction(float duration) {
			moveImage.SetActive(true);
			StartCoroutine(SetInstructionsInactive(duration));
		}

		public void ShowShootInstruction(float duration) {
			shootImage.SetActive(true);
			StartCoroutine(SetInstructionsInactive(duration));
		}

		IEnumerator SetInstructionsInactive(float duration) {
         	yield return new WaitForSeconds(duration);
			 shootImage.SetActive(false);
			 moveImage.SetActive(false);
    	}
	}
}
