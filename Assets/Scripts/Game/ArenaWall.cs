using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWall : MonoBehaviour {

	private const float BOUNCE_VELOCITY = 10.0f;
	private const float ENCROACHMENT_SPEED = 0.3f;
	private const float ENCROACHMENT_DELAY = 1.0f;
	private const float ENCROACHMENT_ACTIVE_DURATION = 0.6f;
	private const float END_ENCROACHMENT = 6.0f;
	private Dictionary<int, Vector2> incoming;

	// Use this for initialization
	void Start () {
		incoming = new Dictionary<int, Vector2>();
		//StartCoroutine(Delay());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Anticipate(int id, Vector2 trajectory) {
		if (incoming.ContainsKey(id)) {
			incoming.Remove(id);
		}
		incoming.Add(id, trajectory);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Vector2 temp;
		if (incoming.ContainsKey(collision.gameObject.GetInstanceID())
			) {//&& incoming[collision.gameObject.GetInstanceID()].magnitude > 0.1f) {
			Debug.Log("anticipated");
			temp = incoming[collision.gameObject.GetInstanceID()];
			if (transform.localScale.x > transform.localScale.y) {
				temp.y *= -1;
			} else {
				temp.x *= -1;
			}
		} else {
			if (!incoming.ContainsKey(collision.gameObject.GetInstanceID())) {
				Debug.Log("not anticipated");
			} else {
				Debug.Log("too slow");
			}
			temp = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
			if (transform.localScale.x > transform.localScale.y) {
				temp.y = ((temp.y < 0) ? 1 : -1) * BOUNCE_VELOCITY;
			} else {
				temp.x = ((temp.x < 0) ? 1 : -1) * BOUNCE_VELOCITY;
			}
		}

	//	Vector2 temp = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
/*		if (transform.localScale.x > transform.localScale.y) {
			temp.y *= -1;
			temp.y += (collision.gameObject.transform.position.y > transform.position.y)
				? BOUNCE_VELOCITY : -1 * BOUNCE_VELOCITY;
		} else {
			temp.x *= -1;
			temp.x += (collision.gameObject.transform.position.x > transform.position.x)
				? BOUNCE_VELOCITY : -1 * BOUNCE_VELOCITY;
		}*/
		collision.gameObject.GetComponent<Rigidbody2D>().velocity = temp;
	}

	void OnCollisionStay2D(Collision2D collision) {
		// Vector2 temp;
		// temp = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
		// if (transform.localScale.x > transform.localScale.y) {
		// 	if (temp.y == 0) {
		// 		temp.y = ((transform.position.y > 0) ? -1 : 1) * BOUNCE_VELOCITY;
		// 	} else {
		// 		temp.y = ((temp.y > 0) ? 1 : -1) * BOUNCE_VELOCITY;
		// 	}
		// } else {
		// 	if (temp.x == 0) {
		// 		temp.x = ((transform.position.x > 0) ? -1 : 1) * BOUNCE_VELOCITY;
		// 	} else {
		// 		temp.x = ((temp.x > 0) ? 1 : -1) * BOUNCE_VELOCITY;
		// 	}
		// }
		// // if (collision.gameObject.tag == "Player") {
		// // 	collision.gameObject.GetComponent<JoystickPlayer>()
		// // 		.ForceChange(temp);
		// // }
		// collision.gameObject.GetComponent<Rigidbody2D>().velocity = temp;
	}

	private IEnumerator Encroach() {
		Vector3 tempPos, tempScale;
		float delta;
		for(float i = 0; i < ENCROACHMENT_ACTIVE_DURATION; i += Time.deltaTime) {
			tempPos = transform.position;
			tempScale = transform.localScale;
			if (transform.localScale.x > transform.localScale.y) {
				delta = ((tempPos.y > 0) ? -1 : 1) * ENCROACHMENT_SPEED * Time.deltaTime;
				if (Mathf.Abs(tempPos.y) > END_ENCROACHMENT) {
					tempPos.y += delta;
				}
				if (tempScale.x > ((2 * END_ENCROACHMENT) - 1)) {
					tempScale.x -= 2 * Mathf.Abs(delta);
				}
			} else {
				delta = ((tempPos.x > 0) ? -1 : 1) * ENCROACHMENT_SPEED * Time.deltaTime;
				if (Mathf.Abs(tempPos.x) > END_ENCROACHMENT) {
					tempPos.x += delta;
				}
				if (tempScale.y > ((2 * END_ENCROACHMENT) - 1)) {
					tempScale.y -= 2 * Mathf.Abs(delta);
				}
			}
			transform.position = tempPos;
			transform.localScale = tempScale;
			yield return null;
		}
		StartCoroutine(Delay());
	}

	private IEnumerator Delay() {
		for(float i = 0; i < ENCROACHMENT_DELAY; i += Time.deltaTime) {
			yield return null;
		}
		/*if (transform.localScale.x > transform.localScale.y) {
			if (Mathf.Abs(transform.position.y) > END_ENCROACHMENT) {
				StartCoroutine(Encroach());
			}
		} else {
			if (Mathf.Abs(transform.position.x) > END_ENCROACHMENT) {
				StartCoroutine(Encroach());
			}
		}*/
		StartCoroutine(Encroach());
	}
}
