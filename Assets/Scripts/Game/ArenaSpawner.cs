using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawner : MonoBehaviour {

	// CAMERA AND BOUNDS
	private Camera theCam;
	private Bounds vision;

	// PREFABS
	public GameObject arenaWallPrefab;
	public GameObject triggerPrefab;
	public GameObject cornerCannonPrefab;

	// CONSTANTS
	private const float ZONE_THICKNESS = 3.0f;
	private const float ZONE_DISTANCE_FROM_BOUND = 1.5f;
	private const float TRIGGER_THICKNESS = 0.2f;

	// Use this for initialization
	void Start () {
//		Debug.Log(Screen.height);
		theCam = GetComponent<Camera>();
//		float screenRatio = Screen.width / Screen.height;
		float screenRatio = theCam.aspect;
		float visionHeight = theCam.orthographicSize * 2;
		vision = new Bounds(
			new Vector3(theCam.transform.position.x, theCam.transform.position.y, 0),
			new Vector3(visionHeight * screenRatio, visionHeight, 0)
		);
		SpawnZones();
	}

	private void SpawnZones() {
		GameObject leftZone, rightZone, topZone, botZone;
		GameObject leftTrigger, rightTrigger, topTrigger, botTrigger;

		leftZone = Object.Instantiate(
//			wrapZonePrefab,
			arenaWallPrefab,
			new Vector3(
//				vision.center.x - vision.extents.x - ZONE_DISTANCE_FROM_BOUND,
				vision.center.x - vision.extents.x,
				vision.center.y,
				0
			),
			theCam.transform.rotation
		);
		leftTrigger = Object.Instantiate(
			triggerPrefab,
			leftZone.transform
		);
		leftTrigger.transform.localScale = new Vector3(TRIGGER_THICKNESS, 1, 1);
		leftTrigger.transform.position = new Vector3(
			leftZone.transform.position.x + 
				(leftZone.transform.localScale.x / 2) +
				(leftTrigger.transform.localScale.x / 2),
			leftZone.transform.position.y,
			leftZone.transform.position.z
		);

		rightZone = Object.Instantiate(
//			wrapZonePrefab,
			arenaWallPrefab,
			new Vector3(
//				vision.center.x + vision.extents.x + ZONE_DISTANCE_FROM_BOUND,
				vision.center.x + vision.extents.x,
				vision.center.y,
				0
			),
			theCam.transform.rotation
		);
		rightTrigger = Object.Instantiate(
			triggerPrefab,
			rightZone.transform
		);
		rightTrigger.transform.localScale = new Vector3(TRIGGER_THICKNESS, 1, 1);
		rightTrigger.transform.position = new Vector3(
			rightZone.transform.position.x - 
				(rightZone.transform.localScale.x / 2) -
				(rightTrigger.transform.localScale.x / 2),
			rightZone.transform.position.y,
			rightZone.transform.position.z
		);

		topZone = Object.Instantiate(
			//wrapZonePrefab,
			arenaWallPrefab,
			new Vector3(
				vision.center.x,
				vision.extents.y + vision.center.y,// - ZONE_DISTANCE_FROM_BOUND,
				0
			),
			theCam.transform.rotation
		);
		topTrigger = Object.Instantiate(
			triggerPrefab,
			topZone.transform
		);
		topTrigger.transform.localScale = new Vector3(1, TRIGGER_THICKNESS, 1);
		topTrigger.transform.position = new Vector3(
			topZone.transform.position.x,
			topZone.transform.position.y - 
				(topZone.transform.localScale.y / 2) -
				(topTrigger.transform.localScale.y / 2),
			topZone.transform.position.z
		);

		botZone = Object.Instantiate(
			//wrapZonePrefab,
			arenaWallPrefab,
			new Vector3(
				vision.center.x,
				vision.center.y - vision.extents.y,// - ZONE_DISTANCE_FROM_BOUND,
				0
			),
			theCam.transform.rotation
		);
		botTrigger = Object.Instantiate(
			triggerPrefab,
			botZone.transform
		);
		botTrigger.transform.localScale = new Vector3(1, TRIGGER_THICKNESS, 1);
		botTrigger.transform.position = new Vector3(
			botZone.transform.position.x,
			botZone.transform.position.y + 
				(botZone.transform.localScale.y / 2) +
				(botTrigger.transform.localScale.y / 2),
			botZone.transform.position.z
		);

		leftZone.transform.localScale = new Vector3(
			ZONE_THICKNESS,
			(vision.extents.y * 2) + (2 * ZONE_THICKNESS),//ZONE_DISTANCE_FROM_BOUND),
			1
		);
		rightZone.transform.localScale = new Vector3(
			ZONE_THICKNESS,
			(vision.extents.y * 2) + (2 * ZONE_THICKNESS),//ZONE_DISTANCE_FROM_BOUND),
			1
		);
		topZone.transform.localScale = new Vector3(
			(vision.extents.x * 2) + (2 * ZONE_THICKNESS),//ZONE_DISTANCE_FROM_BOUND),
			ZONE_THICKNESS,
			1
		);
		botZone.transform.localScale = new Vector3(
			(vision.extents.x * 2) + (2 * ZONE_THICKNESS),//ZONE_DISTANCE_FROM_BOUND),
			ZONE_THICKNESS,
			1
		);

		Object.Instantiate(
			cornerCannonPrefab,
			new Vector3(
				transform.position.x - vision.extents.x,// - ZONE_DISTANCE_FROM_BOUND,
				transform.position.y - vision.extents.y,// - ZONE_DISTANCE_FROM_BOUND,
				0
			),
			Quaternion.Euler(
				0, 0, 0
			)
		);
		Object.Instantiate(
			cornerCannonPrefab,
			new Vector3(
				transform.position.x - vision.extents.x,// - ZONE_DISTANCE_FROM_BOUND,
				transform.position.y + vision.extents.y,// + ZONE_DISTANCE_FROM_BOUND,
				0
			),
			Quaternion.Euler(
				0, 0, 270
			)
		);
		Object.Instantiate(
			cornerCannonPrefab,
			new Vector3(
				transform.position.x + vision.extents.x,// + ZONE_DISTANCE_FROM_BOUND,
				transform.position.y + vision.extents.y,// + ZONE_DISTANCE_FROM_BOUND,
				0
			),
			Quaternion.Euler(
				0, 0, 180
			)
		);
		Object.Instantiate(
			cornerCannonPrefab,
			new Vector3(
				transform.position.x + vision.extents.x,// + ZONE_DISTANCE_FROM_BOUND,
				transform.position.y - vision.extents.y,// - ZONE_DISTANCE_FROM_BOUND,
				0
			),
			Quaternion.Euler(
				0, 0, 90
			)
		);


		/*leftZone.GetComponent<WrapZone>().counterpart = rightZone;
		rightZone.GetComponent<WrapZone>().counterpart = leftZone;*/
		/*topZone.GetComponent<WrapZone>().counterpart = botZone;
		botZone.GetComponent<WrapZone>().counterpart = topZone;*/


	}
	
}
