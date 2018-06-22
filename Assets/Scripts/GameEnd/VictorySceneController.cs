using UnityEngine;
using System.Collections;

public class VictorySceneController : MonoBehaviour {

	private float orgY = -250;
	private float maxY = 1700;
	private float speed = 50;
	private int state = 0;
	private float timeTick = 0;
	private float waitTime = 10;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3(0, orgY, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (state == 0) {
			transform.position = new Vector3(0, transform.position.y + speed * Time.deltaTime, 0);
			if (transform.position.y >= maxY) {
				state = 1;
			}
		} else {
			timeTick += Time.deltaTime;
			if (timeTick >= waitTime) {
				Application.LoadLevel("StartScene");
				GameObject.Destroy(GameObject.Find("MouseTrack"));
			} else if (timeTick > 2) {
				if (Input.GetMouseButtonDown(0)) {
					Application.LoadLevel("StartScene");
					GameObject.Destroy(GameObject.Find("MouseTrack"));
				}
			}
		}
	}
}
