using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapZone : MonoBehaviour {

	public GameObject counterpart;
	private bool horizontal;

	void Start() {
		horizontal = transform.localScale.x < transform.localScale.y;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Debris" || other.tag == "Player") {
			Vector3 tempPos = other.transform.position;
			if (horizontal) {
				tempPos.x = counterpart.transform.position.x + 
					(((transform.position.x < counterpart.transform.position.x) ?
						-1 : 1) * (other.transform.localScale.x / 2)) +
					((transform.position.x < counterpart.transform.position.x) ?
						-1 : 1) * counterpart.transform.localScale.x;
			} else {
				tempPos.y = counterpart.transform.position.y + 
					(((transform.position.y < counterpart.transform.position.y) ?
						-1 : 1) * (other.transform.localScale.y / 2)) +
					((transform.position.y < counterpart.transform.position.y) ?
						-1 : 1) * counterpart.transform.localScale.y;
			}
			other.transform.position = tempPos;
		}
	}

}
