using System;
using UnityEngine;

namespace Kunstharz
{
	[Serializable]
	public class Challenge
	{
		public String gameName;
		public String playerName;
		public int selectedLevelIdx;

		public override string ToString () {
			return JsonUtility.ToJson (this);
		}
	}
}

