using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

	private static Text T;
	public static bool gameOver;

	void Start() {
		T = GetComponent<Text>();
		T.text = "";
		gameOver = false;
	}

	void Update() {
		if (Input.GetAxis("Start") > 0 && gameOver) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			FollowCamera.Restart();
		} else if (Input.GetAxis("Back") > 0 && gameOver) {
			SceneManager.LoadScene("Map Select");
		}
	}

	public static void ShowWinner(string name) {
		T.text = name + " wins!\nStart button to restart\nBack button to select map.";
	}

	public static void GameOver() {
		gameOver = true;
		FollowCamera.Stop();
	}



}
