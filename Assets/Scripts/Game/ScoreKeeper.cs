using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour {

	private Text myText;

	public static int p1Lives;
	public static int p2Lives;
	public int playerNum;

	void Awake() {
		myText = GetComponent<Text>();
	}

	// Use this for initialization
	void Start () {
//		myText = GetComponent<Text>();
	}

	public void RefreshScore() {
		myText.text = (playerNum == 1) ? 
			"P1: " + p1Lives.ToString() + " extra lives" :
			"P2: " + p2Lives.ToString() + " extra lives";
	}
	
}
