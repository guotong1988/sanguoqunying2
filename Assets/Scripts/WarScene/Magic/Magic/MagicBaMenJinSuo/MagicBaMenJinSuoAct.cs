using UnityEngine;
using System.Collections;

public class MagicBaMenJinSuoAct : MonoBehaviour {

	private int state;
	private float timeTick;

	private float yUpPos = -200;
	private float yDownPos = -50;

	// Use this for initialization
	void Start () {

		Vector3 pos = transform.localPosition;
		pos.z = yUpPos;
		transform.localPosition = pos;
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {
		case 0:
		{
			timeTick += Time.deltaTime;
			Vector3 pos = transform.localPosition;
			if (timeTick < 1f) {
				pos.z = Mathf.Lerp(yUpPos, yDownPos, timeTick);
			} else {
				timeTick = 0;
				pos.z = yDownPos;
				state = 1;
			}
			transform.localPosition = pos;
		}
			break;
		case 1:
			timeTick += Time.deltaTime;
			if (timeTick >= 2) {
				timeTick = 0;
				state = 2;
			}
			break;
		case 2:
		{
			timeTick += Time.deltaTime;
			Vector3 pos = transform.localPosition;
			Vector3 scale = transform.localScale;
			if (timeTick < 0.5f) {
				pos.z = Mathf.Lerp(yDownPos, yUpPos, timeTick * 2);
				scale.x = Mathf.Lerp(0.5f, 1, timeTick * 2);
				scale.y = Mathf.Lerp(0.5f, 3, timeTick * 2);
				transform.localPosition = pos;
				transform.localScale = scale;
			} else {
				Destroy(gameObject);
			}
		}
			break;
		}
	}
}
