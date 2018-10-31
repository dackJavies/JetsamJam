using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

	// COMPONENTS
	private Camera theCamera;
	private AudioSource myAudioSource;
	private Rigidbody2D myRigidbody2D;

	// SONGS
	public AudioClip[] songs;

	// TRACKING SIZE OF CAMERA VIEW IN WORLD UNITS
	private Vector2 view;

	// MOVEMENT AND ZOOM PROPORTIONS
	private const float ZOOM_PROPORTION = 3.8f;
	private const float MOVE_PROPORTION = 1.1f;
	private const float ZOOM_DEFAULT_SPEED = 15.0f;

	// HOW OFTEN DO WE CHECK FOR MOVEMENT/ZOOM NEED?
	private const float CHECK_FREQUENCY = 0.2f;
	private const float MOVE_THRESHOLD = 0.01f;
	private const float ZOOM_OUT_FRACTION = 0.36f;
	private const float ZOOM_IN_FRACTION = 0.35f;

	// VARIABLE TO CHANGE VELOCITY
	private Vector3 tempVelocity;

	// PLAYERS
	public GameObject p1;
	public GameObject p2;

	// TARGETS
	Vector2 targetPosition;
	float targedSize;

	// LIMITS
	private const float MIN_SIZE = 15;
	private const float MAX_SIZE = 40;

	// STATIC FLAGS
	public static bool restart;
	public static bool stop;

	void Start() {
		theCamera = GetComponent<Camera>();
		myAudioSource = GetComponent<AudioSource>();
		myRigidbody2D = GetComponent<Rigidbody2D>();

		ChooseRandomSong();

		StartCoroutine(Move());
		StartCoroutine(Zoom());
	}

	void Update() {
		view = GetCameraViewSize();
		if (restart) {
			StartCoroutine(Move());
			StartCoroutine(Zoom());
			restart = false;
		}
		if (stop) {
			myRigidbody2D.velocity = Vector2.zero;
			stop = false;
		}
	}

	private void ChooseRandomSong() {
		if (songs == null || songs.Length <= 0) {
			return;
		} else {
			myAudioSource.clip = songs[Random.Range((int)0, songs.Length)];
			myAudioSource.loop = true;
			myAudioSource.Play();
		}
	}

	public static void Restart() {
		restart = true;
	}

	public static void Stop() {
		stop = true;
	}

	private Vector2 GetMidpoint(Vector2 a, Vector2 b) {
		return b + ((a - b) / 2);
	}

	private Vector2 GetCameraViewSize() {
		float height = theCamera.orthographicSize * 2;
		return new Vector2(height * theCamera.aspect, height);
	}

	private bool NeedToMove() {
		Vector2 midPoint = GetMidpoint(p1.transform.position, p2.transform.position);
		return Vector2.Distance(midPoint, transform.position) > MOVE_THRESHOLD;
	}

	private bool NeedToZoom() {
		float distance = Vector2.Distance(p1.transform.position, p2.transform.position);
		float proportion = distance / view.y;
		bool bothOnScreen = WithinView(p1) && WithinView(p2);
		return (proportion > ZOOM_OUT_FRACTION || proportion < ZOOM_IN_FRACTION)
			|| !bothOnScreen;
	}

	private bool WithinView(GameObject player) {
		float playerX, playerY;
		float myX, myY;
		playerX = player.transform.position.x;
		playerY = player.transform.position.y;
		myX = transform.position.x;
		myY = transform.position.y;
		return (playerX >= myX - view.x && playerX <= myX + view.x)
			&& (playerY >= myY - view.y && playerY <= myY + view.y);
	}

	private IEnumerator Move() {
		Vector2 midPoint;
		while(true) {
			if (Manager.gameOver) {
				yield break;
			}
			if (CanCheck() && NeedToMove()) {
				midPoint = GetMidpoint(p1.transform.position, p2.transform.position);
				tempVelocity = ((midPoint - new Vector2(transform.position.x, transform.position.y))
					* (Vector2.Distance(transform.position, midPoint) / MOVE_PROPORTION)
					* Time.deltaTime);
				tempVelocity.z = 0;
				myRigidbody2D.velocity = tempVelocity * 6.0f;
			}
			yield return null;
		}
	}

	private bool CanCheck() {
		return p1 != null && p2 != null;
	}

	private IEnumerator Zoom() {
		float distance;
		float proportion;
		float p1Magnitude;
		float p2Magnitude;
		float speed;
		while(true) {
			if (Manager.gameOver) {
				yield break;
			}
			if (CanCheck() && NeedToZoom()) {
				distance = Vector2.Distance(p1.transform.position, p2.transform.position);
				proportion = distance / view.y;
				p1Magnitude = p1.GetComponent<Rigidbody2D>().velocity.magnitude;
				p2Magnitude = p2.GetComponent<Rigidbody2D>().velocity.magnitude;
				if (p1Magnitude != 0 || p2Magnitude != 0) {
					speed = ((p1Magnitude > p2Magnitude) ? p1Magnitude : p2Magnitude);
				} else {
					speed = ZOOM_DEFAULT_SPEED;
				}
				if (proportion > ZOOM_OUT_FRACTION && theCamera.orthographicSize < MAX_SIZE) {
					theCamera.orthographicSize +=
						(speed
						/ ZOOM_PROPORTION)
						* Time.deltaTime;
				} else if (proportion < ZOOM_IN_FRACTION && theCamera.orthographicSize > MIN_SIZE) {
					theCamera.orthographicSize -=
						speed
						/ ZOOM_PROPORTION
						* Time.deltaTime;
				}
			}
			yield return null;
		}
	}

}
