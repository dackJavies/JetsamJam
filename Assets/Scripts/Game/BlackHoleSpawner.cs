using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSpawner : MonoBehaviour {

	public GameObject blackHolePrefab;
	private Bounds spawnBox;

	private const float EXTENDS_X = 50.0f;
	private const float EXTENDS_Y = 25.0f;
	public int NUM_BLACK_HOLES;
	private Vector3[] positions;

	void Start() {
		spawnBox = new Bounds(
			new Vector3(
				Camera.main.transform.position.x,
				Camera.main.transform.position.y,
				0
			),
			new Vector3(EXTENDS_X, EXTENDS_Y, 0)
		);
		positions = new Vector3[NUM_BLACK_HOLES + 4];
		AddPlayerTransformsToPositionList();
		SpawnBlackHoles();
	}

	private void AddPlayerTransformsToPositionList() {
		GameObject current;
		for(int i = 0; i < 4; i++) {
			current = GameObject.Find("P" + (i + 1).ToString());
			if (current != null) {
				positions[i] = current.transform.position;
			}
		}
	}

	private void SpawnBlackHoles() {
		Object.Instantiate(blackHolePrefab, Vector3.zero, Quaternion.identity);
		GameObject current;
		for(int i = 4; i < positions.Length - 1; i++) {
			current = Object.Instantiate(blackHolePrefab);
			current.transform.position =
				new Vector3(
					Random.Range(
						spawnBox.center.x - spawnBox.extents.x,
						spawnBox.center.x + spawnBox.extents.x
					),
					Random.Range(
						spawnBox.center.y - spawnBox.extents.y,
						spawnBox.center.y + spawnBox.extents.y
					),
					0
				);
			/*Collider2D other = Physics2D.OverlapBox(
				current.transform.position,
				current.transform.localScale / 2,
				0f
			);*/
			/*Collider2D other = Physics2D.OverlapCircle(
				current.transform.position,
				Mathf.Sqrt(Mathf.Pow(transform.localScale.x, 2) + Mathf.Pow(transform.localScale.y, 2))
			);*/
			/*if (other != null && other.tag != "Debris") {
				Debug.Log("something's there");
			//	Object.Destroy(current);
				i -= 1;
			}*/
			/*if (Physics.OverlapBox(
				current.transform.position,
				new Vector3(
					Random.Range(
						spawnBox.center.x - spawnBox.extents.x,
						spawnBox.center.x + spawnBox.extents.x
					),
					Random.Range(
						spawnBox.center.y - spawnBox.extents.y,
						spawnBox.center.y + spawnBox.extents.y
					),
					0.5f
				)
			).Length != 0) */
			if (NearAnotherBlackHole(i, current.transform.position)){
				Object.Destroy(current);
				i -= 1;
			} else {
				positions[i] = current.transform.position;
			}
		}
	}

	private bool NearAnotherBlackHole(int spawnProgress, Vector3 spawnPoint) {
		for(int i = 0; i < spawnProgress; i++) {
			if (Vector3.Distance(spawnPoint, positions[i]) < 5.0f) {
				return true;
			}
		}
		return false;
	}

}
