using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	private SpriteRenderer SR;

	private int playerNum;
	private bool disabled;

	public Color defaultColor;
	public Color activeColor;
	public Color disabledColor;
	

	void Start() {
		SR = GetComponent<SpriteRenderer>();

		playerNum = transform.parent.parent.gameObject
			.GetComponent<JoystickPlayer>().playerNum;
	}

	void Update() {
		if (disabled) {
			TryColorSwitch(disabledColor);
		} else if (Input.GetAxis("Fire " + playerNum.ToString()) > 0) {
			TryColorSwitch(activeColor);
		} else {
			TryColorSwitch(defaultColor);
		}
	}

	private void TryColorSwitch(Color candidate) {
		if (SR.color != candidate) {
			SR.color = candidate;
		}
	}

	public void SetDisabled(bool setting) {
		disabled = setting;
	}

}
