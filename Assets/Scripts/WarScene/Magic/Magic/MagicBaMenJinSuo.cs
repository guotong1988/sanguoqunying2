using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBaMenJinSuo : MonoBehaviour {

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide enemySide;

	private GameObject go;
	private Vector3 enemyCenterPoint;

	private string wordName = "Magic/BaMenJinSuo/BaMenJinSuo";

	// Use this for initialization
	void Start () {
		Init();
		Invoke("WaitForCameraMoveToGeneral", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Init() {

		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;

		warCtrl = MagicController.Instance.warCtrl;
		enemySide = MagicController.Instance.GetMagicEnemySide();
		enemyCenterPoint = warCtrl.GetArmyCentrePoint(enemySide);

		go = (GameObject)Instantiate(Resources.Load(wordName));
		go.transform.parent = transform;
		go.transform.localPosition = enemyCenterPoint;
		go.SetActive(false);
	}

	void WaitForCameraMoveToGeneral() {

		warCtrl.SetCameraMoveTo(new Vector3(enemyCenterPoint.x, -180, 0));

		Invoke("WaitForShowWord", 0.2f);
	}

	void WaitForShowWord() {

		go.SetActive(true);

		warCtrl.SetSoldierMagicLock(enemySide);

		Invoke("WaitForMagicOver", 4f);

		SoundController.Instance.PlaySound("00009");
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
