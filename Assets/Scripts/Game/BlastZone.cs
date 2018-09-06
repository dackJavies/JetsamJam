using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZone : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			Object.Destroy(other.gameObject);
		}
	}

}
