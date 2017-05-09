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

			if (delta.magnitude > 0.0) {
				Vector3 newUiPosition = panels.transform.localPosition + new Vector3 (-delta.x * 2f, -delta.y * 2f, 0f);

				float x = Mathf.Clamp (newUiPosition.x, -150f, 150f);
				float y = Mathf.Clamp (newUiPosition.y, -30f, 30f);

				panels.transform.localPosition = new Vector3 (x, y, 0f);

				/*ChromaticAberrationModel.Settings chromaticAbberation = mainCamera.GetComponent<PostProcessingBehaviour> ().profile.chromaticAberration.settings;

				chromaticAbberation.intensity = Mathf.Clamp (delta.magnitude * 0.5f, 0.25f, 0.8f);

				Debug.Log (Mathf.Clamp (delta.magnitude * 0.5f, 0f, 0.8f));

				mainCamera.GetComponent<PostProcessingBehaviour> ().profile.chromaticAberration.settings = chromaticAbberation;*/


			} else if (panels.transform.localPosition.x > 0.01f || panels.transform.localPosition.y > 0.01f) {
				
				Vector3 damp = new Vector3 (0.8f, 0.8f, 0.8f);
				panels.transform.localPosition = Vector3.Scale (panels.transform.localPosition, damp);
			} else {
				panels.transform.localPosition = Vector3.zero;
			}

		}

	}
}
