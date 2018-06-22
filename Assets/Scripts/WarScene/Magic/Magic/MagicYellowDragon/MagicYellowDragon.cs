using UnityEngine;
using System.Collections;

public class MagicYellowDragon : MonoBehaviour {

	private int state;
	private float timeTick;

	private exSprite sprite;

	private float speed = 500;

	// Use this for initialization
	void Start () {

		sprite = GetComponent<exSprite>();
		sprite.color = new Color(1, 1, 1, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
		switch (state) {
		case 0:
			timeTick += Time.deltaTime;
			if (timeTick <= 0.5f) {
				sprite.color = new Color(1, 1, 1, timeTick * 2);
			} else {
				timeTick = 0;
				state = 1;
				sprite.color = new Color(1, 1, 1, 1);
			}
			break;
		case 1:
			timeTick += Time.deltaTime;
			if (timeTick >= 0.5f) {
				state = 2;
			}
			break;
		case 2:
			Vector3 pos = transform.localPosition;
			pos.z -= speed * Time.deltaTime;
			transform.localPosition = pos;
			if (pos.z < -300) {
				Destroy(gameObject);
			}
			break;
		}
	}
}
