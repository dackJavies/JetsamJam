using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// CONTROLS
	public string left;
	public string right;
	public string up;
	public string down;

	// COLORS
	public Color defaultColor;
	public Color lagColor;
	public Color stunnedColor;

	// HITBOX
	public GameObject hitbox;
	private const float HITBOX_SPEED = 10.0f;

	// COMPONENTS
	private Rigidbody2D RB;
	private SpriteRenderer SR;

	// MOVEMENT CONSTANTS
	private const float BLAST_FORCE = 0.4f;
	private const float GRAVITY_ACCEL = 20.0f;
	private const float TERMINAL_VELOCITY = 10.0f;
	private const float HIT_LAG = 0.02f;
	private const float STUN_DURATION = 0.55f;


	// MOVEMENT VARIABLES
	private Vector2 temp;
	private bool stunned;
	private bool lagging;


	// Use this for initialization
	void Start () {
		RB = GetComponent<Rigidbody2D>();
		SR = GetComponent<SpriteRenderer>();
		SR.color = defaultColor;

		stunned = false;
		lagging = false;
	}
	
	// Update is called once per frame
	void Update () {
		temp = RB.velocity;
		if (!(stunned || lagging) && !Manager.gameOver) {
			CheckForInput();
		}
		EnforceTerminalVelocity();
		RB.velocity = temp;
	}

	private void CheckForInput() {
//		if (Input.GetKey(left)) {
		if (Input.GetKey(right)) {
			temp.x += BLAST_FORCE;
			SpawnHitbox(Vector2.left);
			StartCoroutine(Lag());
//		} else if (Input.GetKey(right)) {
		} else if (Input.GetKey(left)) {
			temp.x += -1 * BLAST_FORCE;
			SpawnHitbox(Vector2.right);
			StartCoroutine(Lag());
//		} else if (Input.GetKey(up)) {
		} else if (Input.GetKey(down)) {
			temp.y += -1 * BLAST_FORCE;
			SpawnHitbox(Vector2.up);
			StartCoroutine(Lag());
//		} else  if (Input.GetKey(down)) {
		} else if (Input.GetKey(up)) {
			temp.y += BLAST_FORCE;
			SpawnHitbox(Vector2.down);
			StartCoroutine(Lag());
		}
	}

	private void SpawnHitbox(Vector2 direction) {
		GameObject hb;
		hb = Object.Instantiate(hitbox, transform);
		hb.transform.localPosition = Vector2.zero;
		hb.GetComponent<Hitbox>().movementDirection = direction * HITBOX_SPEED;
	}

	private IEnumerator Lag() {
		SR.color = lagColor;
		lagging = true;
		for(float i = 0; i < HIT_LAG; i += Time.deltaTime) {
			lagging = true;
			yield return null;
		}
		SR.color = defaultColor;
		lagging = false;
	}

	private void Fall() {
		temp.y -= GRAVITY_ACCEL * Time.deltaTime;
	}

	private void EnforceTerminalVelocity() {
		if (temp.x > TERMINAL_VELOCITY) {
			temp.x = TERMINAL_VELOCITY;
		} else if (temp.x < -1 * TERMINAL_VELOCITY) {
			temp.x = -1 * TERMINAL_VELOCITY;
		}
		if (temp.y > TERMINAL_VELOCITY) {
			temp.y = TERMINAL_VELOCITY;
		} else if (temp.x < -1 * TERMINAL_VELOCITY) {
			temp.y = -1 * TERMINAL_VELOCITY;
		}
	}

	public void EnterStunned() {
		if (!stunned) {
			StartCoroutine(Stunned());
		}
	}

	private IEnumerator Stunned() {
		SR.color = stunnedColor;
		stunned = true;
		for(float i = 0; i < STUN_DURATION; i+= Time.deltaTime) {
			stunned = true;
			yield return null;
		}
		SR.color = defaultColor;
		stunned = false;
	}

}
