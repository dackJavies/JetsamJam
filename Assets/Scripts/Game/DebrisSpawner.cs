using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour {

	private const float EXTENT_X = 50.0f;
	private const float EXTENT_Y = 30.0f;

	private const float SMALL_LOW = 0.1f;
	private const float SMALL_HIGH = 0.3f;
	private const float MED_LOW = 0.5f;
	private const float MED_HIGH = 0.7f;
	private const float LRG_LOW = 0.8f;
	private const float LRG_HIGH = 1.2f;
	private const float GIANT_LOW = 1.3f;
	private const float GIANT_HIGH = 1.8f;

	private const int NUM_SMALL = 0;
	private const int NUM_MED = 0;
	private const int NUM_LRG = 0;
	private const int NUM_GIANT = 0;

	private const float MASS_MULTIPLIER = 1.0f;

	public GameObject debrisPrefab;
	private Bounds spawnBox;

	// Use this for initialization
	void Start () {
		spawnBox = new Bounds(
			new Vector3(
				Camera.main.transform.position.x,
				Camera.main.transform.position.y,
				0
			),
			//new Vector3(EXTENT_X, EXTENT_Y, 0)
			new Vector3(
				Camera.main.orthographicSize * 2 * Camera.main.aspect - 4,
				Camera.main.orthographicSize * 2 - 4,
				0.5f
			)
		);
		SpawnDebris();
	}

	private void SpawnDebris() {
		SpawnCategory(GIANT_LOW, GIANT_HIGH, NUM_GIANT);
		SpawnCategory(LRG_LOW, LRG_HIGH, NUM_LRG);
		SpawnCategory(MED_LOW, MED_HIGH, NUM_MED);
		SpawnCategory(SMALL_LOW, SMALL_HIGH, NUM_SMALL);
	}

	private void SpawnCategory(float low, float high, int num) {
		GameObject current;
		for(int i = 0; i < num; i++) {
			current = Object.Instantiate(
				debrisPrefab,
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
				),
				new Quaternion(0, 0, 0, 0)
			);
			current.transform.localScale = new Vector3(
				Random.Range(low, high),
				Random.Range(low, high),
				1
			);
			current.GetComponent<Rigidbody2D>().mass =
				current.transform.localScale.x *
				current.transform.localScale.y *
				MASS_MULTIPLIER;
			if (Physics.OverlapBox(
				current.transform.position,
				new Vector3(
					current.transform.localScale.x / 2,
					current.transform.localScale.y / 2,
					current.transform.localScale.z / 2
				)
			).Length != 0) {
				Object.Destroy(current);
				i -= 1;
			}
		}
	}
	
}
