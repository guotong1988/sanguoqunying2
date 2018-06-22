#pragma strict

var screenWidth : int = 640;
var screenHeight : int = 480;

function Start () {
	
	Application.targetFrameRate = -1;
	//Input.multiTouchEnabled = false;

	if (Application.platform == RuntimePlatform.Android) {
		GetComponent.<Camera>().orthographicSize = screenHeight / 2;
		Screen.SetResolution (screenWidth, screenHeight, true);
	}
}