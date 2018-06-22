using UnityEngine;
using System.Collections;

public class MagicSwordUp : MonoBehaviour {

	private int state;
	private float timeTick;

	private float speed = 500;

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {
		case 0:
			timeTick += Time.deltaTime;
			if (timeTick < 0.5f) {
				transform.localScale = new Vector3(timeTick, timeTick, 1);
			} else {
				state = 1;
				timeTick = 0;
				transform.localScale = new Vector3(0.5f, 0.5f, 1);
			}
			break;
		case 1:
			timeTick += Time.deltaTime;
			if (timeTick >= 0.5f) {
				timeTick = 0;
				state = 2;
			}
			break;
		case 2:
			Vector3 pos = transform.localPosition;
			pos.z -= speed * Time.deltaTime;
			transform.localPosition = pos;
			if (pos.z < -200) {
				Destroy(gameObject);
			}
			break;
		}
	}

}
