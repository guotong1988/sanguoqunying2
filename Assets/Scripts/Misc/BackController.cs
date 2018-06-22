using UnityEngine;
using System.Collections;

public class BackController : MonoBehaviour {
	
	private Button bottonCtrl;
	
	// Use this for initialization
	void Start () {
		bottonCtrl = GetComponent<Button>();
		Misc.backButton = gameObject;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Misc.isBack) {
			Misc.isBack = false;
			gameObject.SetActive(false);
		}
		
		if (bottonCtrl.GetButtonState() == Button.ButtonState.Clicked) {
			Misc.isBack = true;
		}

		if (Misc.isNeedBack) {
			Misc.isNeedBack = false;
		} else {
			Misc.isBack = false;
			gameObject.SetActive(false);
		}
	}

	void OnDestroy() {
		Misc.isBack = false;
	}
}
