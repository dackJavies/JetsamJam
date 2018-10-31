using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayer : MonoBehaviour {

	// COLORS
	public Color defaultColor;
	public Color lagColor;
	public Color stunnedColor;

	// HITBOX
	public GameObject hitbox;
	private const float HITBOX_SPEED = 13.0f;

	// COMPONENTS
	private Rigidbody2D RB;
	private SpriteRenderer SR;
	private TrailRenderer TR;
	private ParticleSystem PS;
	private AudioSource AS;

	// SOUNDS
	public AudioClip pulseSound;
	public AudioClip hitSound;
	public AudioClip laserSound;
	public AudioClip dieSound;
	public AudioClip bounceSound;
	public AudioClip respawnSound;

	// SCORE
	public GameObject myScoreKeeper;

	// TRAIL
	private Gradient defaultGradient;
	private Gradient stunnedGradient;
	private Gradient deathGradient;
	private Gradient chargeGradient;

	// LASER CANNON
	private TextMesh FuelReader;
	private GameObject myAnchor;
	private LaserCannon myLaserCannon;
	private bool aiming;
	private const int FIRE_COST = 0;
	private const float MAX_FIRE_KNOCKBACK = 35.0f;//25.0f;
	private const float MAX_FIRE_STUN = 1.2f;


	// MOVEMENT CONSTANTS
//	private const float BLAST_FORCE = 1.5f;
	private const float BLAST_FORCE = 2.0f;//1.2f;
	private const float TERMINAL_VELOCITY = 15.0f;//13.0f;//10.0f;
	private const float HIT_LAG = 0.04f;
	private const float JOYSTICK_THRESHOLD = 0.3f;
	private const float FIRE_THRESHOLD = 0.0f;

	public int playerNum;


	// MOVEMENT VARIABLES
	private Vector2 temp;
	private bool stunned;
	private bool lagging;
	private float horizontal;
	private float vertical;
	private float hypotenuse;
	private float fire;
	private float angle;
	private bool holdingFire;

	// FUEL
	private int fuel;
	private bool charging;
	private const float CHARGE_LAG = 0.02f;
//	private const float MAX_FUEL = 50;
	private float maxFuel;
	private const float STARTING_MAX_FUEL = 50;

	// STOCKS
	private Vector3 spawnPoint;
	private int lives;
	private const int STARTING_NUM_LIVES = 3;
	private const float DEAD_TIME = 1.0f;
	private const float INVINCIBLE_TIME = 1.0f;
	public bool invincible;
	private bool dead;
	private TextMesh stunCounter;
	private const float MAX_RUBBERBAND = 0.6f;


	// Use this for initialization
	void Start () {
		spawnPoint = transform.position;
		lives = STARTING_NUM_LIVES;
		invincible = false;
		dead = false;
		stunned = false;
		lagging = false;

		// Identify player number and prepare lives
		if (name == "P1") {
			ScoreKeeper.p1Lives = lives;
		} else {
			ScoreKeeper.p2Lives = lives;
		}
		myScoreKeeper.GetComponent<ScoreKeeper>().RefreshScore();

		// Set Components
		RB = GetComponent<Rigidbody2D>();
		SR = GetComponent<SpriteRenderer>();
		SR.material.color = defaultColor;
		TR = GetComponent<TrailRenderer>();
		PS = GetComponent<ParticleSystem>();
		AS = GetComponent<AudioSource>();

		// Instantiate gradients
		defaultGradient = GetFadingGradient(defaultColor);
		stunnedGradient = GetFadingGradient(stunnedColor);
		deathGradient = GetFadingGradient(MyColors.red);
		chargeGradient = GetFadingGradient(MyColors.green);

		// Get references to children
		FuelReader = transform.GetChild(0).GetComponent<TextMesh>();
		FuelReader.GetComponent<MeshRenderer>().sortingLayerName =
			SR.sortingLayerName;
		maxFuel = STARTING_MAX_FUEL;

		fuel = Mathf.FloorToInt(maxFuel);

		stunCounter = transform.GetChild(2).GetComponent<TextMesh>();
		stunCounter.GetComponent<MeshRenderer>().sortingLayerName =
			SR.sortingLayerName;
		stunCounter.text = "";

		// Set some information on children
		myAnchor = transform.GetChild(1).gameObject;
		myLaserCannon = transform.GetChild(1).GetChild(0).gameObject.GetComponent<LaserCannon>();

		// Make sure camera gets refreshed
		FollowCamera.Restart();
	}

	// Update is called once per frame
	void Update () {
		temp = RB.velocity;
		if (!stunned && !dead) {
			CheckForInput();
		}
		EnforceTerminalVelocity();
		EnforceFuelConstraints();
		RB.velocity = temp;

		UpdateFuelReader();
	}

	private Gradient GetFadingGradient(Color color) {
		Gradient result = new Gradient();
		result.SetKeys(
			new GradientColorKey[2] {
				new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f)
			},
			new GradientAlphaKey[2] {
				new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)
			}
		);
		return result;
	}

	private void UpdateFuelReader() {
		if (invincible) {
			FuelReader.color = defaultColor;
			FuelReader.text = lives.ToString() + " lives left.";
		} else {
			FuelReader.color = new Color(
				MyColors.red.r + ((MyColors.green.r - MyColors.red.r) * (fuel / maxFuel)),
				MyColors.red.g + ((MyColors.green.g - MyColors.red.g) * (fuel / maxFuel)),
				MyColors.red.b + ((MyColors.green.b - MyColors.red.b) * (fuel / maxFuel)),
				1.0f
			);
			FuelReader.text = fuel.ToString();
		}
	}

	private void CheckForInput() {
		// Moving
		if (!lagging && fuel > 0) {
			SR.material.color = defaultColor;
			TR.colorGradient = defaultGradient;
			horizontal = Input.GetAxis("Left Horizontal " + playerNum.ToString());
			vertical = Input.GetAxis("Left Vertical " + playerNum.ToString());
			if (Mathf.Abs(horizontal) > JOYSTICK_THRESHOLD || Mathf.Abs(vertical) > JOYSTICK_THRESHOLD) {
				hypotenuse = Mathf.Sqrt(Mathf.Pow(horizontal, 2) + Mathf.Pow(vertical, 2));
				temp.x += horizontal * BLAST_FORCE;
				temp.y += vertical * BLAST_FORCE;
				SpawnHitbox(new Vector2(-1 * horizontal * BLAST_FORCE, -1 * vertical * BLAST_FORCE));
				AS.PlayOneShot(pulseSound);
				if (!invincible) {
					fuel -= 1;
				}
				StartCoroutine(Lag());
			}
		} else if (fuel <= 0) {
			SR.material.color = stunnedColor;
			TR.colorGradient = stunnedGradient;
		}

		// Aiming
		horizontal = Input.GetAxis("Right Horizontal " + playerNum.ToString());
		vertical = Input.GetAxis("Right Vertical " + playerNum.ToString());
		if (Mathf.Abs(horizontal) > JOYSTICK_THRESHOLD || Mathf.Abs(vertical) > JOYSTICK_THRESHOLD) {
			myLaserCannon.GetComponent<SpriteRenderer>().enabled = true;
			angle = Mathf.Atan(vertical / horizontal) * Mathf.Rad2Deg;
			if (horizontal < 0){
				angle += 180;
			}
			Vector3 anchorAngle = myAnchor.transform.localEulerAngles;
			anchorAngle.z = angle;
			myAnchor.transform.localEulerAngles = anchorAngle;
			aiming = true;
		} else {
			myLaserCannon.GetComponent<SpriteRenderer>().enabled = false;
			aiming = false;
		}

		// Firing
		if (fuel >= FIRE_COST && aiming) {
			if (myLaserCannon.GetDisabled()) {
				myLaserCannon.SetDisabled(false);
			}
			fire = Input.GetAxis("Fire " + playerNum.ToString());
			if (fire > FIRE_THRESHOLD && !holdingFire && fuel > 0) {
				holdingFire = true;
				if (myLaserCannon.Fire(new Vector2(
					horizontal,
					vertical
//				), fuel / MAX_FUEL);
				), fuel / maxFuel)) {
					TakeLaserKnockback(new Vector2(
						horizontal, vertical
	//				).normalized * -1, fuel / MAX_FUEL);
					).normalized * -1, fuel / maxFuel, false);
					EnterStunned(MAX_FIRE_STUN * (fuel / maxFuel));
					fuel = 0;
					AS.PlayOneShot(laserSound);
//					fuel -= FIRE_COST;
				}
			} else if (fire <= FIRE_THRESHOLD) {
				holdingFire = false;
			}
		} else {
			if (!myLaserCannon.GetDisabled()) {
				myLaserCannon.SetDisabled(true);
			}
		}

	}

	private void SpawnHitbox(Vector2 direction) {
		GameObject hb;
		hb = Object.Instantiate(hitbox, transform);
		hb.transform.localPosition = Vector2.zero;
		Vector2 movementDir = (direction * HITBOX_SPEED) + (RB.velocity);
		hb.GetComponent<JoystickHitbox>().movementDirection = movementDir;
	}

	private IEnumerator Lag() {
		SR.material.color = lagColor;
		lagging = true;
		for(float i = 0; i < HIT_LAG; i += Time.deltaTime) {
			lagging = true;
			yield return null;
		}
		SR.material.color = defaultColor;
		lagging = false;
		if (fuel <= 0) {
			StartCoroutine(Stunned(0));
		}
	}

	private void EnforceTerminalVelocity() {
		if (temp.x > TERMINAL_VELOCITY) {
			temp.x = TERMINAL_VELOCITY;
		} else if (temp.x < -1 * TERMINAL_VELOCITY) {
			temp.x = -1 * TERMINAL_VELOCITY;
		}
		if (temp.y > TERMINAL_VELOCITY) {
			temp.y = TERMINAL_VELOCITY;
		} else if (temp.y < -1 * TERMINAL_VELOCITY) {
			temp.y = -1 * TERMINAL_VELOCITY;
		}
	}

	private void EnforceFuelConstraints() {
		if (fuel < 0) {
			fuel = 0;
		} else if (fuel > maxFuel) {
			fuel = Mathf.FloorToInt(maxFuel);
		}
		if (maxFuel <= 0 && !dead) {
			Die();
		}
	}

	public void EnterStunned(float duration) {
		if (!stunned) {
			AS.PlayOneShot(hitSound);
			StartCoroutine(Stunned(duration));
		}
	}

	public void TakeLaserKnockback(Vector2 direction, float power, bool receiver) {
		Vector2 test;
		if (receiver) {
			test = direction.normalized * power * MAX_FIRE_KNOCKBACK;
			temp = direction.normalized
				* ((test.magnitude > temp.magnitude) ? power : (temp.magnitude / TERMINAL_VELOCITY))
				* MAX_FIRE_KNOCKBACK;
			if (test.magnitude < temp.magnitude) {
				temp += test;
			} else {
				temp = test;
			}
		} else {
			temp += direction.normalized * power * MAX_FIRE_KNOCKBACK;
		}
//		temp += direction.normalized * power * MAX_FIRE_KNOCKBACK;
//		temp = direction.normalized * power * MAX_FIRE_KNOCKBACK;
		RB.velocity = temp;
	}

	public void LoseFuelCapacity(int amount) {
		if (!stunned) {
			maxFuel -= amount;
		}
	}

	public void Die() {
		AS.PlayOneShot(dieSound);
		TR.enabled = false;
		if (lives > 0) {
			PlayParticleSystem(deathGradient, 25.0f);
			StartCoroutine(Dead());
		} else {
			if (!Manager.gameOver) {
				Manager.GameOver();
				Manager.ShowWinner((name == "P1") ? "P2" : "P1");
			}
			Object.Destroy(this.gameObject);
		}
	}

	private IEnumerator Dead() {
		Hide();
		dead = true;
		for(float i = 0; i < DEAD_TIME; i += Time.deltaTime) {
			yield return null;
		}
		transform.position = spawnPoint;
		maxFuel = STARTING_MAX_FUEL;
		fuel = Mathf.FloorToInt(maxFuel);
		AS.PlayOneShot(respawnSound);
		Show();
		StartCoroutine(Invincible());
	}

	private void Hide() {
		SR.enabled = false;
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(false);
		transform.GetChild(2).gameObject.SetActive(false);
		RB.constraints = RigidbodyConstraints2D.FreezeAll;
		GetComponent<CircleCollider2D>().enabled = false;
	}

	private void Show() {
		SR.enabled = true;
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(1).gameObject.SetActive(true);
		transform.GetChild(2).gameObject.SetActive(true);
		RB.constraints = RigidbodyConstraints2D.FreezeRotation;
		GetComponent<CircleCollider2D>().enabled = true;
		myLaserCannon.ResetLag();
	}

	private IEnumerator Invincible() {
		Color temp;
		bool switcher = false;
		int switchFrequency = 7;
		int counter = 0;
		float transparentAlpha = 0.0f;
		float visibleAlpha = 0.3f;
		invincible = true;
		lives -= 1;
		TR.enabled = true;
		if (name == "P1") {
			ScoreKeeper.p1Lives = lives;
		} else {
			ScoreKeeper.p2Lives = lives;
		}
		myScoreKeeper.GetComponent<ScoreKeeper>().RefreshScore();

		dead = false;
		for(float i = 0; i < INVINCIBLE_TIME; i += Time.deltaTime) {
			counter += 1;
			if (counter >= switchFrequency) {
				switcher = !switcher;
				temp = SR.material.color;
				temp.a = (switcher) ? visibleAlpha : transparentAlpha;
				SR.material.color = temp;
				counter = 0;
			}
			yield return null;
		}
		temp = SR.material.color;
		temp.a = 1.0f;
		SR.material.color = temp;
		invincible = false;
	}

	private IEnumerator Stunned(float stunTime) {
		SR.material.color = stunnedColor;
		TR.colorGradient = stunnedGradient;
		PlayParticleSystem(stunnedGradient, 10.0f);
		stunned = true;
		myLaserCannon.SetDisabled(true);
		stunTime -= (MAX_RUBBERBAND - ((maxFuel / STARTING_MAX_FUEL) * MAX_RUBBERBAND));
		for(float i = 0; i < stunTime; i+= Time.deltaTime) {
			if (invincible) {
				stunned = false;
				stunCounter.text = "";
				myLaserCannon.SetDisabled(false);
				yield break;
			}
			stunned = true;
			if (SR.material.color != stunnedColor) {
				SR.material.color = stunnedColor;
				TR.colorGradient = stunnedGradient;
			}
			stunCounter.text = (stunTime - i).ToString("F2");
			yield return null;
		}
		SR.material.color = defaultColor;
		stunned = false;
		myLaserCannon.SetDisabled(false);
		stunCounter.text = "";
		if (fuel <= 0) {
			SR.material.color = stunnedColor;
			TR.colorGradient = stunnedGradient;
		}
	}

	private void PlayParticleSystem(Gradient gradient, float speed) {
		var particleGradient = PS.colorOverLifetime;
		particleGradient.color = gradient;
		var main = PS.main;
		main.startSpeed = speed;
		PS.Play();
	}

	public void Recharge() {
//		if (!charging && fuel < MAX_FUEL) {
		if (!charging && fuel < maxFuel) {
			StartCoroutine(RegainFuel());
		}
	}

	private IEnumerator RegainFuel() {
		charging = true;
		if (!PS.isPlaying) {
			PlayParticleSystem(chargeGradient, 1.0f);
		}
		for(float i = 0; i < CHARGE_LAG; i += Time.deltaTime) {
			yield return null;
		}
		fuel += 1;
		if (stunned) {
			fuel += 1;
		}
		charging = false;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "ArenaWall") {
			AS.PlayOneShot(bounceSound);
			PlayParticleSystem(defaultGradient, 10);
			if (fuel <= 0) {
				fuel = 10;
			}
		}
	}

}
