using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickHitbox : MonoBehaviour {

	// COMPONENTS
	private BoxCollider2D BC;
	private Rigidbody2D RB;
	private SpriteRenderer SR;

	// MOVEMENT VARIABLES
	public Vector2 movementDirection;

	// CONSTANTS
	private const float LIFESPAN = 0.17f;//0.2f;//0.3f;//0.5f;
	private const float LIFESPAN_EXTENSION_DAMPENER = 120.0f;
	private const float THICKNESS = 0.4f;
	private const float PUSH_FORCE_DIVIDER = 2.0f;
	private const float DEBRIS_FORCE_DIVIDER = 13.0f;
	private const float COLOR_ALPHA = 0.8f;
	private const float MIN_STUN_DURATION = 0.1f;
	private const float STUN_DURATION_DAMPENER = 25;//18;
	private const int WAKE_DAMAGE = 2;

	// Use this for initialization
	void Start () {
		BC = GetComponent<BoxCollider2D>();
		RB = GetComponent<Rigidbody2D>();
		SR = GetComponent<SpriteRenderer>();

		// SR.color = 
		// 	transform.parent.GetComponent<JoystickPlayer>().defaultColor;
		Color temp = 
			transform.parent.GetComponent<JoystickPlayer>().defaultColor;
		temp.a = COLOR_ALPHA;
		SR.material.color = temp;

		Vector3 rot = transform.localEulerAngles;
		float foo = Mathf.Acos(movementDirection.x / Mathf.Sqrt(Mathf.Pow(movementDirection.x, 2) + Mathf.Pow(movementDirection.y, 2))) * Mathf.Rad2Deg;
		if (movementDirection.y > 0) {
			rot.z = foo - 90;
		} else if (movementDirection.y < 0) {
			rot.z = foo + (2 * (180 - foo)) + 90;
		} else {
			rot.z = 90;
		}
		transform.localEulerAngles = rot;

//		if (ExpandHorizontally()) {
		transform.localScale = new Vector3(1, THICKNESS, 1);
//		} else {
//			transform.localScale = new Vector3(THICKNESS, 1, 1);
//		}

		RB.velocity = movementDirection;
		StartCoroutine(Elongate());
		StartCoroutine(Decay());
	}

	private IEnumerator Elongate() {
		Vector3 tempScale;
		while(true) {
			tempScale = transform.localScale;
			tempScale.x += Mathf.Sqrt(Mathf.Pow(movementDirection.x, 2) + Mathf.Pow(movementDirection.y, 2)) / 2 * Time.deltaTime;
			transform.localScale = tempScale;
			yield return null;
		}
	}

	private bool ExpandHorizontally() {
		return movementDirection.y != 0;
	}

	private IEnumerator Decay() {
		float cap = LIFESPAN + (movementDirection.magnitude / LIFESPAN_EXTENSION_DAMPENER);
		Color temp;
		for(float i = 0; i < cap; i += Time.deltaTime) {
			temp = SR.material.color;
			temp.a -= COLOR_ALPHA * Time.deltaTime;
			SR.material.color = temp;
			yield return null;
		}
		Object.Destroy(this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" && other.gameObject != transform.parent.gameObject) {
			if (!other.GetComponent<JoystickPlayer>().invincible) {
				other.GetComponent<JoystickPlayer>().LoseFuelCapacity(WAKE_DAMAGE);
				other.GetComponent<JoystickPlayer>().EnterStunned(MIN_STUN_DURATION + (RB.velocity.magnitude / STUN_DURATION_DAMPENER));
				other.GetComponent<Rigidbody2D>().velocity += movementDirection / PUSH_FORCE_DIVIDER;
			}
		} else if (other.tag == "Debris") {
			other.GetComponent<Rigidbody2D>().velocity += movementDirection /
				(DEBRIS_FORCE_DIVIDER);// * other.GetComponent<Rigidbody2D>().mass);
			Object.Destroy(this.gameObject);
		} else if (other.tag == "Hitbox") {
			Object.Destroy(other.gameObject);
			Object.Destroy(this.gameObject);
		}
	}
	
}
