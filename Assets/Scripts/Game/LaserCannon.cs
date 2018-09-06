using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannon : MonoBehaviour {

	private SpriteRenderer SR;

	private int playerNum;
	private bool disabled;
	private bool lagging;

	public Color defaultColor;
	public Color activeColor;
	public Color disabledColor;
	public GameObject laserBeamPrefab;

	private const float LAG_DURATION = 2.0f;
	

	void Start() {
		SR = GetComponent<SpriteRenderer>();
		SR.color = defaultColor;

		playerNum = transform.parent.parent.gameObject
			.GetComponent<JoystickPlayer>().playerNum;

	}

	void Update() {

	}

	public bool Fire(Vector2 direction, float power) {
		if (lagging) {
			SR.color = disabledColor;
			return false;
		}
		GameObject beam = Object.Instantiate(laserBeamPrefab, transform);
		beam.transform.localEulerAngles = Vector3.zero;
		beam.GetComponent<LaserBeam>().direction = direction;
		beam.GetComponent<LaserBeam>().power = power;
		beam.GetComponent<LaserBeam>().originPlayer = transform.parent.parent.name;
		beam.transform.parent = null;
		StartCoroutine(Lag());
		return true;
	}

	public void SetDisabled(bool setting) {
		disabled = setting;
		if (disabled) {
			SR.color = disabledColor;
		} else {
			SR.color = defaultColor;
		}
	}

	public bool GetDisabled() {
		return disabled;
	}

	public void ResetLag() {
		lagging = false;
	}

	private IEnumerator Lag() {
		lagging = true;
		SR.color = activeColor;
		for(float i = 0; i < LAG_DURATION; i += Time.deltaTime) {
			yield return null;
		}
		lagging = false;
		if (disabled) {
			SR.color = disabledColor;
		} else {
			SR.color = defaultColor;
		}
	}
}
