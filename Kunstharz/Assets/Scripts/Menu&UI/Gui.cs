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

		Vector2 delta = Vector2.zero;

		public Camera mainCamera;
		public GameObject panels;

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
		}

		void Update() {
			UISlug ();
		}

		private void UISlug() {

			delta = new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));

			panels.transform.localPosition += new Vector3(- (delta.x * delta.magnitude * 30f), -(delta.y * delta.magnitude * 30f), 0f);

			panels.transform.localPosition = Vector3.ClampMagnitude (panels.transform.localPosition, 30f);

			Vector3 damp = new Vector3 (0.5f, 0.5f, 0.5f);

			panels.transform.localPosition = Vector3.Scale (panels.transform.localPosition, damp);

		}

	}
}
