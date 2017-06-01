using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz {
	public class ConnectWithFirstFoundHost : MonoBehaviour {
		public LevelLoader loader;
		private bool loaded = false;

		void FinderEntriesChanged(ICollection<FinderEntry> found) {
			if (!loaded && found.Count > 0) {
				loaded = true;
				var enumerator = found.GetEnumerator ();
				enumerator.MoveNext ();
				FinderEntry first = enumerator.Current;
				loader.JoinUgly (first.hostname);
				Destroy (this);
			}
		}
	}
}
