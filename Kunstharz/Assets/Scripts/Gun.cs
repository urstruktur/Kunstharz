using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kunstharz {
	public class Gun : MonoBehaviour {

		void SetShootTarget(Target target) {
			print ("Shooting at " + target.position);
		}

	}

}
