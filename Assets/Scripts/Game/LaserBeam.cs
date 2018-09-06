using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

	private Rigidbody2D myRigidbody;
	private const float LASER_SPEED = 40.0f;
	private const float MAX_STUN = 1.7f;
	private const int LASER_DAMAGE = 5;
	public Vector2 direction;
	public string originPlayer = "";
	public float power;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D>();
		myRigidbody.velocity = direction.normalized * LASER_SPEED;
	}

	void OnTriggerEnter2D(Collider2D other) {
		// if (other.tag == "Player" && originPlayer == "") {
		// 	originPlayer = other.gameObject.name;
		// }
		if (other.tag == "Player" && other.gameObject.name != originPlayer) {
			if (!other.GetComponent<JoystickPlayer>().invincible) {
				other.GetComponent<JoystickPlayer>().LoseFuelCapacity(LASER_DAMAGE);
				other.GetComponent<JoystickPlayer>().EnterStunned(
					((power > 0.6f) ? power : 0.6f) * MAX_STUN);
				other.GetComponent<JoystickPlayer>().TakeLaserKnockback(
					direction.normalized, power, true
				);
			}
			Object.Destroy(this.gameObject);
		}
	}

	// void OnBecameInvisible() {
	// 	Object.Destroy(this.gameObject);
	// }

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "WrapZone" || other.tag == "ArenaWall") {
			if (Vector2.Distance(transform.position, Vector2.zero)
				> Vector2.Distance(other.transform.position, Vector2.zero)) {
					Object.Destroy(this.gameObject);
				}
		}
	}
	
}
