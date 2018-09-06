using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

	// COMPONENTS
	private BoxCollider2D BC;
	private Rigidbody2D RB;

	// MOVEMENT VARIABLES
	public Vector2 movementDirection;

	// CONSTANTS
	private const float LIFESPAN = 0.8f;
	private const float THICKNESS = 0.2f;
	private const float PUSH_FORCE_DIVIDER = 3.0f;
	private const float DEBRIS_FORCE_DIVIDER = 25.0f;

	// Use this for initialization
	void Start () {
		BC = GetComponent<BoxCollider2D>();
		RB = GetComponent<Rigidbody2D>();

		if (ExpandHorizontally()) {
			transform.localScale = new Vector3(1, THICKNESS, 1);
		} else {
			transform.localScale = new Vector3(THICKNESS, 1, 1);
		}

		RB.velocity = movementDirection;
		StartCoroutine(Elongate());
		StartCoroutine(Decay());
	}

	private IEnumerator Elongate() {
		bool horizontal = ExpandHorizontally();
		Vector3 tempScale;
		while(true) {
			tempScale = transform.localScale;
			if (horizontal) {
				tempScale.x += Mathf.Abs(movementDirection.y) * Time.deltaTime;
			} else {
				tempScale.y += Mathf.Abs(movementDirection.x) * Time.deltaTime;
			}
			transform.localScale = tempScale;
			yield return null;
		}
	}

	private bool ExpandHorizontally() {
		return movementDirection.y != 0;
	}

	private IEnumerator Decay() {
		for(float i = 0; i < LIFESPAN; i += Time.deltaTime) {
			yield return null;
		}
		Object.Destroy(this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" && other.gameObject != transform.parent.gameObject) {
			other.GetComponent<Player>().EnterStunned();
			other.GetComponent<Rigidbody2D>().velocity += movementDirection / PUSH_FORCE_DIVIDER;
		} else if (other.tag == "Debris") {
			other.GetComponent<Rigidbody2D>().velocity += movementDirection / DEBRIS_FORCE_DIVIDER;
		}
	}
	
}
