using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapMenu : MonoBehaviour {

	public GameObject warningText;
	public GameObject selectedText;

	private bool notReady;

	private static string p1Vote;
	private static string p2Vote;

	private const float SHOW_MAP_CHOICE_TIME = 2.0f;

	void Start() {
		p1Vote = "";
		p2Vote = "";
	}

	void Update() {
		notReady = Input.GetJoystickNames().Length < 2;
		warningText.SetActive(notReady);

		if (p1Vote != "" && p2Vote != "") {
			if (p1Vote != p2Vote && !notReady) {
				StartCoroutine(ShowFlippingCoin());
			} else if (!notReady) {
				StartCoroutine(ShowResult(p1Vote));
			}
		}
	}

	private IEnumerator ShowFlippingCoin() {
		selectedText.GetComponent<Text>().text = "Flipping a coin...";
		for(float i = 0; i < SHOW_MAP_CHOICE_TIME / 2; i += Time.deltaTime) {
			yield return null;
		}
		PickRandom();
	}

	private IEnumerator ShowResult(string result) {
		selectedText.GetComponent<Text>().text = "Going to " + result;
		for(float i = 0; i < SHOW_MAP_CHOICE_TIME; i += Time.deltaTime) {
			if (notReady) {
				selectedText.GetComponent<Text>().text = "Waiting for two players...";
				yield break;
			}
			yield return null;
		}
		SceneManager.LoadScene(result);
	}

	private void PickRandom() {
		float random = Random.Range(0, 1);
		StartCoroutine(ShowResult((random <= 0.5f) ? p1Vote : p2Vote));
	}

	public static void SetP1Vote(string vote) {
		p1Vote = vote;
	}

	public static void SetP2Vote(string vote) {
		p2Vote = vote;
	}

}
