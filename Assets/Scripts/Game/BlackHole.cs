using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

	private GameObject range;

	// CONSTANTS
	private const float GRACE_PERIOD = 2.0f;
	private const float GROWTH_TIME = 0.5f;
	private const float GROWTH_MAGNITUDE = 0.07f;
	private const float GROWTH_DAMPENER = 6.0f;
	private const float SIZE_CAP = 3f;

	// COMPONENTS
	private CircleCollider2D CC;

	// Use this for initialization
	void Start () {
		CC = GetComponent<CircleCollider2D>();

		range = transform.Find("Range").gameObject;
		//StartCoroutine(GracePeriod());
	}

	private IEnumerator GracePeriod() {
		for(float i = 0; i < GRACE_PERIOD; i += Time.deltaTime) {
			yield return null;
		}
		if (transform.localScale.x < SIZE_CAP) {
			StartCoroutine(Grow());
		}
	}

	private IEnumerator Grow() {
		for(float i = 0; i < GROWTH_TIME; i += Time.deltaTime) {
			transform.localScale *= (1 + GROWTH_MAGNITUDE * Time.deltaTime);
			range.transform.localScale *= (1 + GROWTH_MAGNITUDE * 3.5f * Time.deltaTime);
			yield return null;
		}
		if (transform.localScale.x < SIZE_CAP) {
			StartCoroutine(GracePeriod());
		}
	}

	private IEnumerator Consume(float size) {
		for(float i = 0; i < GROWTH_TIME; i += Time.deltaTime) {
			if (transform.localScale.x > SIZE_CAP) {
				yield break;
			}
			transform.localScale += (Vector3.one * (size * Time.deltaTime));
			yield return null;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" /*|| other.tag == "Debris"*/) {
			if(!other.GetComponent<JoystickPlayer>().invincible) {
				other.GetComponent<JoystickPlayer>().Die();
			}
			// if (!Manager.gameOver) {
			// 	Manager.GameOver();
			// 	Manager.ShowWinner((other.name == "P1") ? "P2" : "P1");
			// }
		} else if (other.tag == "BlackHole") {
//			CombineWithOtherBlackHole(other.gameObject);
		} else if (other.tag == "Debris") {
/*			if (transform.localScale.x < SIZE_CAP) {
				StartCoroutine(Consume(other.transform.localScale.x * other.transform.localScale.y / GROWTH_DAMPENER));
			}
			Object.Destroy(other.gameObject);*/
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		// Debug.Log("bye");
		// if (other.tag == "ArenaWall") {
		// 	Debug.Log("bye bye arena wall!" + transform.position);
		// 	Object.Destroy(this.gameObject);
		// }
	}

	private void CombineWithOtherBlackHole(GameObject other) {

	}
	
}
