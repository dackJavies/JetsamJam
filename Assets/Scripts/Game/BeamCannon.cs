using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCannon : MonoBehaviour {

	private SpriteRenderer SR;

	// Use this for initialization
	void Start () {
		SR = GetComponent<SpriteRenderer>();
	}

	public void Hide() {
		SR.enabled = false;
	}

	public void Show() {
		SR.enabled = true;
	}
	
}
