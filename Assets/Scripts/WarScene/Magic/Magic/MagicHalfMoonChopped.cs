using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicHalfMoonChopped : MonoBehaviour {

	public int level;

	private int state;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	
	private GeneralController general;
	private GeneralController enemyGeneral;

	private List<GameObject> listMoon = new List<GameObject>();
	private List<GameObject> listArrowExplode = new List<GameObject>();

	private Vector3 cameraPos;

	private float speed;
	private float speedLeft = 500;
	private float speedRight = -500;

	private float xStepLeft = -30;
	private float xStepRight = 30;
	private float yStep = 60;

	private string halfMoonChoppedName = "Magic/HalfMoonChopped/HalfMoonChopped";
	private string explodeName = "Magic/Misc/Explode";
	private string explodeName2 = "Magic/SunArrow/SunArrowExplode";

	// Use this for initialization
	void Start () {

		Init();
		GenerateMoon();
	}
	
	// Update is called once per frame
	void Update () {

		float deltaPos = speed * Time.deltaTime;
		if (state == 1) {

			cameraPos = new Vector3(cameraPos.x + deltaPos, cameraPos.y, 0);

			for (int i=0; i<listMoon.Count; i++) {
				Vector3 pos = listMoon[i].transform.localPosition;
				pos.x += deltaPos;
				listMoon[i].transform.localPosition = pos;
			}
			
			if ((side == WarSceneController.WhichSide.Left && cameraPos.x > enemyGeneral.transform.localPosition.x)
			    || (side == WarSceneController.WhichSide.Right && cameraPos.x < enemyGeneral.transform.localPosition.x)) {
				
				state = 2;
				Invoke("WaitForMagicOver", 1);

				enemyGeneral.OnDamage(MagicController.Instance.GetMagicAttack(), -1, false);
				listMoon[0].SetActive(false);

				for (int i=1; i<listMoon.Count; i++) {
					Rect region = new Rect();
					if (side == WarSceneController.WhichSide.Left) {
						region.x = listMoon[i].transform.localPosition.x - 20;
					} else {
						region.x = listMoon[i].transform.localPosition.x - 180;
					}
					region.y = listMoon[i].transform.localPosition.y - 15;
					region.width = 200;
					region.height = 30;
					warCtrl.OnMagicHitChecking(side, 0, region, false);
				}

				StartCoroutine(ShowArrowExplode());
			} else {
				warCtrl.SetCameraPosition(cameraPos);

				for (int i=0; i<listMoon.Count; i++) {
					Rect region = new Rect();
					region.x = listMoon[i].transform.localPosition.x;
					region.y = listMoon[i].transform.localPosition.y - 15;
					region.width = 20;
					region.height = 30;
					warCtrl.OnMagicHitChecking(side, 0, region, false);
				}
			}
		} else if (state == 2) {

			for (int i=0; i<listMoon.Count; i++) {
				Vector3 pos = listMoon[i].transform.localPosition;
				pos.x += deltaPos;
				listMoon[i].transform.localPosition = pos;
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

		cameraPos = general.transform.localPosition;
		cameraPos.y = -30 * level;
	}

	void GenerateMoon() {

		Vector3 scale;
		float xStep;
		Vector3 posBase = general.transform.localPosition;
		if (side == WarSceneController.WhichSide.Left) {
			scale = WarSceneController.manScaleLeft;
			xStep = xStepLeft;
		} else {
			scale = WarSceneController.manScaleRight;
			xStep = xStepRight;
		}
		posBase.x -= xStep;

		GameObject go = InstantiateOneGO(halfMoonChoppedName, posBase, scale, WarSceneController.manEulerAngles);
		go.SetActive(false);
		listMoon.Add(go);

		for (int i=1; i<level; i++) {
			Vector3 pos = new Vector3(posBase.x + xStep * i, posBase.y + yStep * i, 0);
			go = InstantiateOneGO(halfMoonChoppedName, pos, scale, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listMoon.Add(go);

			pos.y = posBase.y - yStep * i;
			go = InstantiateOneGO(halfMoonChoppedName, pos, scale, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listMoon.Add(go);
		}

		for (int i=0; i<5; i++) {
			go = InstantiateOneGO(explodeName2, Vector3.zero, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listArrowExplode.Add(go);
		}
	}

	GameObject InstantiateOneGO(string goName, Vector3 pos, Vector3 scale, Vector3 eulerAngles) {
		
		GameObject go = (GameObject)Instantiate(Resources.Load(goName));
		go.transform.parent = this.transform;
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.transform.eulerAngles = eulerAngles;
		
		return go;
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
			
			yield return new WaitForSeconds(0.1f);
		}
		
		Invoke("WaitForMagicOver", 0.5f);
	}

	void WaitForArrowOut() {
		state = 1;
		for (int i=0; i<listMoon.Count; i++) {
			listMoon[i].SetActive(true);
		}

		SoundController.Instance.PlaySound("00040");
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
