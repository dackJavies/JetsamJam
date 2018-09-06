using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	private Vector2 lastPos;
	private Vector2 currentPos;
	private float distanceFromCam;
	private Vector2 difference;
	private const float DISTANCE_PROPORTION = 0.2f;
	private const float DISTANCE_THRESOLD = 0.1f;

	void Start() {
		distanceFromCam = Vector3.Distance(transform.position,
			Camera.main.transform.position);
		lastPos = Camera.main.transform.position;
	}

	void Update() {
		currentPos = Camera.main.transform.position;
		if (Vector2.Distance(lastPos, currentPos) > DISTANCE_THRESOLD) {
			difference = currentPos - lastPos;//(Vector2)transform.position;
			difference /= -1 * (distanceFromCam * DISTANCE_PROPORTION);
			transform.position += new Vector3(
				difference.x,
				difference.y,
				0
			);
		}
		lastPos = Camera.main.transform.position;
	}

}
