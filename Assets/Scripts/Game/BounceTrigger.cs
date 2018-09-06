using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTrigger : MonoBehaviour {

	ArenaWall wall;

	void Start() {
		wall = transform.parent.gameObject.GetComponent<ArenaWall>();
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("hello " + other.name);
		Vector2 temp = other.gameObject.GetComponent<Rigidbody2D>().velocity;
//		if (temp.x * temp.y != 0) {
			LogObject(other.gameObject);
//		}
	}

	void OnTriggerStay2D(Collider2D other) {
		Vector2 temp = other.gameObject.GetComponent<Rigidbody2D>().velocity;
//		if (temp.x * temp.y != 0) {
//			LogObject(other.gameObject);
//		}
	}

	private void LogObject(GameObject incoming) {
		wall.Anticipate(
			incoming.GetInstanceID(),
			incoming.GetComponent<Rigidbody2D>().velocity
		);
	}

}
