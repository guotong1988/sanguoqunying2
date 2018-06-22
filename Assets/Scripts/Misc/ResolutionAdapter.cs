using UnityEngine;
using System.Collections;

public class ResolutionAdapter : MonoBehaviour {

	// Use this for initialization
	void Start () {

		float scaleX = Screen.width / 640f;
		float scaleY = Screen.height / 480f;
		float factor = 1;
		if (scaleX > scaleY)
			factor = scaleX / scaleY;
		transform.localScale = new Vector3(factor, 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
