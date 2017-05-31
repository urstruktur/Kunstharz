using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {

	public int world;
	public string gameName;

	public Game(string gameName, int world) {
		this.world = world;
		this.gameName = gameName;
	}
}
