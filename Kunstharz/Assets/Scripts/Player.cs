using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz {
	public class Player : MonoBehaviour {
		void Start() {
			var game = GameObject.Find ("Game").transform;
			transform.parent = game;
			SendMessageUpwards ("PlayerJoined", this);
		}
	}
}
