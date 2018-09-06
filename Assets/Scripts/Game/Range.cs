using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour {

	private Transform center;
	private const float PULL_DAMPENER = 3f; //15
	private const float BASE_PULL_FORCE = 0.05f;//0.5f;

	// Use this for initialization
	void Start () {
		center = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			GetComponent<AudioSource>().Play();
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		// if (other.tag != "Hitbox" && other.tag != "BlackHole" && other.tag != "Range" && other.tag != "BlastZone") {
		// 	Vector2 difference = other.transform.position - center.position;
		// 	other.GetComponent<Rigidbody2D>().velocity += 
		// 		difference /
		// 		((Vector2.Distance(other.transform.position, center.position) /
		// 			(transform.localScale.x / 2)) * center.localScale.x) *
		// 		Time.deltaTime *
		// 		-1 / PULL_DAMPENER;
		// }
		if (other.tag != "Hitbox" && other.tag != "BlackHole" && other.tag != "Range" && other.tag != "BlastZone" && other.tag != "LaserBeam") {
			Vector2 difference = other.transform.position - center.position;
			other.GetComponent<Rigidbody2D>().velocity += 
				((difference.normalized * (BASE_PULL_FORCE * (center.localScale.x / 2))) *
				(-1 * (Vector2.Distance(center.transform.position, other.transform.position) / PULL_DAMPENER)));
		}
		if (other.tag == "Player") {
			other.GetComponent<JoystickPlayer>().Recharge();
		}
	}
}
