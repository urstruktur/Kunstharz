using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

namespace Kunstharz
{
	public class Gui : MonoBehaviour {

		public Text p1ScoreText;
		public Text p2ScoreText;

		public Text p1Action;
		public Text p2Action;

		public GameObject GUIAnchor;

		public float speed = 5f;

		private Vector3 currentAngle;

		private float velocity = 0f;

		public void UpdateScore(Game game) {
			Player local = game.localPlayer;
			Player other = game.nonLocalPlayer;

			p1ScoreText.text = ""+local.wins;
			p2ScoreText.text = ""+other.wins;
		}

		public void UpdatePlayerStates(Game game) {
			p1Action.text = ""+game.localPlayer.state;
			p2Action.text = ""+game.nonLocalPlayer.state;
		}

		void Start() {
			currentAngle = GUIAnchor.transform.eulerAngles;

			GUIAnchor.transform.position = Camera.main.transform.position;
			GUIAnchor.transform.eulerAngles = Camera.main.transform.eulerAngles;
		}

		void Update() {
			UISlug();
		}

		private void UISlug() {

			GUIAnchor.transform.position = Camera.main.transform.position;

			currentAngle = new Vector3(

				Mathf.LerpAngle(currentAngle.x, Camera.main.transform.eulerAngles.x, Time.deltaTime * speed),
				Mathf.LerpAngle(currentAngle.y, Camera.main.transform.eulerAngles.y, Time.deltaTime * speed),
				Mathf.LerpAngle(currentAngle.z, Camera.main.transform.eulerAngles.z, Time.deltaTime * speed));

			GUIAnchor.transform.eulerAngles = currentAngle;

		}

	}
}
