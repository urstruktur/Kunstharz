using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kunstharz
{
	public class Gui : MonoBehaviour {

		public Text p1ScoreText;
		public Text p2ScoreText;

		public Text p1Action;
		public Text p2Action;

		public Text p1Name;
		public Text p2Name;

		public GameObject GUIAnchor;
		public float speed = 5f;

		private Vector3 currentAngle;
		private float currentVelocity;

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

	}
}
