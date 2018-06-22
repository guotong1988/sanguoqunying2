using UnityEngine;
using System.Collections;

public class OverSceneController : MonoBehaviour {

	private float timeTick = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeTick += Time.deltaTime;
		if (timeTick >= 10) {
			timeTick = 0;
			Misc.LoadLevel("StartScene");
			GameObject.Destroy(GameObject.Find("MouseTrack"));
		} else if (timeTick > 2) {
			if (Input.GetMouseButtonDown(0)) {
				timeTick = 0;
				Misc.LoadLevel("StartScene");
				GameObject.Destroy(GameObject.Find("MouseTrack"));
			}
		}
	}
}
