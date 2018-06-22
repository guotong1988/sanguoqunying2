using UnityEngine;
using System.Collections;

public class WSSpeedUpController : MonoBehaviour {

	bool isSpeedUp = false;

	// Use this for initialization
	void Start () {
		GetComponent<Button>().SetButtonClickHandler(OnClick);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {

		if (isSpeedUp) {
			isSpeedUp = false;
			Time.timeScale = 1;
			GetComponent<exSpriteFont>().text = ZhongWen.Instance.speedUp;
		} else {
			isSpeedUp = true;
			Time.timeScale = 2;
			GetComponent<exSpriteFont>().text = ZhongWen.Instance.normalSpeed;
		}
	}
}
