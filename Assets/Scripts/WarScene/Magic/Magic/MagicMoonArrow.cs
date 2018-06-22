using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicMoonArrow : MonoBehaviour {

	public int type;

	private int state;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;

	private GeneralController general;
	private GeneralController enemyGeneral;

	private GameObject arrow;
	private List<GameObject> listArrowExplode = new List<GameObject>();

	private float speed;
	private float speedLeft = 500;
	private float speedRight = -500;

	private string arrowName;
	private string explodeName;
	private string arrowName1 = "Magic/MoonArrow/MoonArrow";
	private string explodeName1 = "Magic/MoonArrow/MoonArrowExplode";
	private string arrowName2 = "Magic/SunArrow/SunArrow";
	private string explodeName2 = "Magic/SunArrow/SunArrowExplode";
	private string arrowName3 = "Magic/Dart/Dart";
	private string explodeName3 = "Magic/SunArrow/SunArrowExplode";
	private string explodeName4 = "Magic/Misc/Explode";

	// Use this for initialization
	void Start () {
	
		Init();
		GenerateArrow();
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 1) {
			Vector3 pos = arrow.transform.localPosition;
			pos.x += speed * Time.deltaTime;
			arrow.transform.localPosition = pos;

			if ((side == WarSceneController.WhichSide.Left && pos.x > enemyGeneral.transform.localPosition.x)
			    || (side == WarSceneController.WhichSide.Right && pos.x < enemyGeneral.transform.localPosition.x)) {

				state = 0;
				Destroy(arrow);
				StartCoroutine(ShowArrowExplode());

				enemyGeneral.OnDamage(MagicController.Instance.GetMagicAttack(), -1, false);
			} else {
				warCtrl.SetCameraPosition(new Vector3(pos.x, -30, 0));
			}
		}
	}

	void Init() {

		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;

		warCtrl = MagicController.Instance.warCtrl;
		side = MagicController.Instance.GetMagicSide();
		if (side == WarSceneController.WhichSide.Left) {
			speed = speedLeft;
			general = MagicController.Instance.warCtrl.leftGeneral;
			enemyGeneral = MagicController.Instance.warCtrl.rightGeneral;
		} else {
			speed = speedRight;
			general = MagicController.Instance.warCtrl.rightGeneral;
			enemyGeneral = MagicController.Instance.warCtrl.leftGeneral;
		}

		if (type == 0) {
			arrowName = arrowName1;
			explodeName = explodeName1;
		} else if (type == 1) {
			arrowName = arrowName2;
			explodeName = explodeName2;
		} else if (type == 2) {
			arrowName = arrowName3;
			explodeName = explodeName3;
		}
	}

	void GenerateArrow() {

		arrow = (GameObject)Instantiate(Resources.Load(arrowName));
		arrow.transform.parent = transform;

		float off = 0;
		Vector3 scale = Vector3.zero;
		if (side == WarSceneController.WhichSide.Left) {
			off = 30;
			scale = WarSceneController.manScaleLeft;

		} else {
			off = -30;
			scale = WarSceneController.manScaleRight;
		}
		arrow.transform.localPosition = new Vector3(general.transform.localPosition.x + off,
		                                            general.transform.localPosition.y,
		                                            -40);
		arrow.transform.localEulerAngles = WarSceneController.manEulerAngles;
		arrow.transform.localScale = scale;
		arrow.SetActive(false);

		for (int i=0; i<5; i++) {
			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			go.transform.localEulerAngles = WarSceneController.manEulerAngles;
			go.transform.localScale = scale;
			go.SetActive(false);

			listArrowExplode.Add(go);
		}
	}

	void WaitForArrowOut() {
		state = 1;
		arrow.SetActive(true);

		if (type == 0 || type == 1) {
			SoundController.Instance.PlaySound("00004");
		} else {
			SoundController.Instance.PlaySound("00005");
		}
	}

	IEnumerator ShowArrowExplode() {

		for (int i=0; i<5; i++) {
			listArrowExplode[i].SetActive(true);

			Vector3 pos = enemyGeneral.transform.localPosition;
			pos.x += Random.Range(-30, 30);
			pos.y = -5;
			pos.z += Random.Range(-30, 0);
			listArrowExplode[i].transform.localPosition = pos;

			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			go.transform.localPosition = pos;

			go = (GameObject)Instantiate(Resources.Load(explodeName4));
			go.transform.parent = transform;
			go.transform.localPosition = pos;

			yield return new WaitForSeconds(0.1f);
		}

		Invoke("WaitForMagicOver", 0.5f);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
