using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectCursor : MonoBehaviour {

	private const float MOVE_SPEED = 5.0f;
	private Vector2 velocity;
	private Vector3 delta;

	private float horizontal;
	private float vertical;
	private float selector;

	private List<MapSelectPicture> hovering;
	private bool voted;

	public int playerNum;
	public Sprite selectedSprite;
	private Color myColor;

	void Start() {
		hovering = new List<MapSelectPicture>();
		myColor = GetComponent<SpriteRenderer>().color;
		voted = false;
	}

	void Update() {
		horizontal = Input.GetAxis("Left Horizontal " + playerNum.ToString());
		vertical = Input.GetAxis("Left Vertical " + playerNum.ToString());
		selector = Input.GetAxis("Select " + playerNum);

		velocity = new Vector2(horizontal, vertical).normalized * MOVE_SPEED;
		delta = velocity * Time.deltaTime;

		if (selector > 0.3f && hovering.Count == 1) {
			if (playerNum == 1) {
				MapMenu.SetP1Vote(hovering[0].name);
			} else {
				MapMenu.SetP2Vote(hovering[0].name);
			}
			GetComponent<SpriteRenderer>().sprite = selectedSprite;
			GetComponent<SpriteRenderer>().color = myColor;
			voted = true;
		}

		if (!voted) {
			transform.position += delta;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "MapImage") {
			hovering.Add(other.transform.parent.GetComponent<MapSelectPicture>());
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "MapImage") {
			hovering.Remove(other.transform.parent.GetComponent<MapSelectPicture>());
		}
	}

}