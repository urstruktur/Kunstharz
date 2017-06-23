using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kunstharz
{
	public class Gui : MonoBehaviour {

		public enum InstructionType {Move, Shoot, Scored, Erased, TimeOut, TimeOutNonLocal, TimeOutBoth, Win, Lose, PrepareGame}

		public Camera gameCamera;

		public Text p1ScoreText;
		public Text p2ScoreText;
		public Text generalScore;

		public Text p1Action;
		public Text p2Action;

		public Text p1Name;
		public Text p2Name;

		public Image[] scoreImages = new Image [6];

		public GameObject GUIAnchor;
		private float speed = 1f;

		public GameObject moveImage;

		public GameObject shootImage;

		public GameObject erasedImage;

		public GameObject scoredImage;

		public GameObject[] instructions = new GameObject [6];

		private Quaternion currentAngle;
		private Quaternion currentVelocity;

		bool slug = true;

		public GameObject time;

		private int timeID;

		private bool moveInstructionShown = false;

		private bool timeHidden = false;

        private bool warningPlayed = false;

		public ModularCrosshair crosshair;

		IEnumerator currentCR;

		void Start() {
			currentAngle = GUIAnchor.transform.rotation;
			GUIAnchor.transform.rotation = gameCamera.transform.rotation;
		}

		public void UpdateScore(GameContext game) {
			Player local = game.localPlayer;
			Player other = game.remotePlayer;

			/*p1ScoreText.text = ""+local.wins;
			p2ScoreText.text = ""+other.wins;*/

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

			Canvas.ForceUpdateCanvases();
			
		}

		public void UpdatePlayerStates(GameContext game) {
			p1Action.text = ""+game.localPlayer.state;
			p2Action.text = ""+game.remotePlayer.state;
		}

		public void UpdatePlayerNames(GameContext game) {
			if (game != null) {
				p1Name.text = ""+game.localPlayer.playerName;
				p2Name.text = ""+game.remotePlayer.playerName;
			}
		}

		void LateUpdate() {
			UISlug();
		}

		private void UISlug() {
			if (gameCamera != null) {
				GUIAnchor.transform.rotation = Quaternion.Slerp(GUIAnchor.transform.rotation, gameCamera.transform.rotation, 10.0f * Time.deltaTime);
			}
		}

		public void ShowInstruction (InstructionType it, float duration, bool hideCrosshair) {
			if (currentCR != null) StopCoroutine(currentCR);
			StartCoroutine(SetInstructionsInactive(0));
			instructions[(int)it].SetActive(true);
			if (hideCrosshair) {crosshair.HideCrosshair();}
			currentCR = SetInstructionsInactive(duration);
			StartCoroutine(currentCR);
			moveInstructionShown = true;
		}

		IEnumerator SetInstructionsInactive(float duration) {
			if (duration > 0) yield return new WaitForSeconds(duration);
			for (int i = 0; i < instructions.Length; i++) { instructions[i].SetActive(false); }
			moveInstructionShown = false;
			crosshair.ShowCrosshair();
    	}

		public void ShowTime(float duration) {
			LeanTween.cancel(timeID);
			timeHidden = false;
			timeID = LeanTween.value(gameObject, UpdateTime, 1f, 0f, duration).setOnComplete(TimeComplete).id;
		}


		void UpdateTime(float value) {
			if (!moveInstructionShown && !time.activeInHierarchy && !timeHidden) {
				time.SetActive (true);
			}
			time.GetComponent<Image>().fillAmount = value;

            // play warning sound when round time is at 90%
            if (!timeHidden && value < 0.25 && !warningPlayed)
            {
                FindObjectOfType<Soundsystem>().playWarning();
                warningPlayed = true;
            }
		}

		void TimeComplete () {
			time.SetActive (false);
            warningPlayed = false;
		}

		public void HideTime() {
			time.SetActive (false);
			timeHidden = true;
		}
	}
}
