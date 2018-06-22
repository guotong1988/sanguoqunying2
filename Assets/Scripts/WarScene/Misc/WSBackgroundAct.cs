using UnityEngine;
using System.Collections;

public class WSBackgroundAct : MonoBehaviour {
	
	public Transform sky;
	public Transform background;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		sky.localPosition = new Vector3(WarSceneController.cameraPosCur.x / WarSceneController.cameraPosMaxX * (-45f),
			sky.localPosition.y, sky.localPosition.z);
		background.localPosition = new Vector3(WarSceneController.cameraPosCur.x / WarSceneController.cameraPosMaxX * (-135f),
			background.localPosition.y, background.localPosition.z);
	}
}
