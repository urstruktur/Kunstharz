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

	}
}
