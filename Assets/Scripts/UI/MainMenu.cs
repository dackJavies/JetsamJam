using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public GameObject warningText;
	public GameObject playButton;
	private bool notReady;

	void Update() {
		notReady = Input.GetJoystickNames().Length < 2;
		warningText.SetActive(notReady);
		playButton.GetComponent<Button>().enabled = !notReady;
	}

	public void PlayGame() {
		SceneManager.LoadScene("Map Select");
	}

	public void QuitGame() {
		Application.Quit();
	}

}
