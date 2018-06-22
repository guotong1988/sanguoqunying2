using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicFenShenZhan : MonoBehaviour {

	private int state;
	
	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	
	private GeneralController general;
	private GeneralController enemyGeneral;
	
	private List<GameObject> listMan = new List<GameObject>();
	
	private Vector3 cameraPos;
	
	private float speed;
	private float speedLeft = 500;
	private float speedRight = -500;

	private float zOff = 0;
	private Vector3 bottomOffset = new Vector3(-90, -5, 0);
	
	private string manName = "Magic/FenShenZhan/FenShenZhan";
	private string bottomName = "Magic/FenShenZhan/FenShenZhanBottom";
	private string explodeName = "Magic/Misc/Explode";
	
	// Use this for initialization
	void Start () {
		
		Init();
		GenerateMan();
	}
	
	// Update is called once per frame
	void Update () {
		
		float deltaPos = speed * Time.deltaTime;
		if (state == 1) {
			
			for (int i=0; i<listMan.Count; i++) {
				if (!listMan[i].activeSelf) break;

				Vector3 pos = listMan[i].transform.localPosition;
				pos.x += deltaPos;
				listMan[i].transform.localPosition = pos;
			}

			cameraPos = new Vector3(listMan[4].transform.localPosition.x, -30, 0);
			
			if ((side == WarSceneController.WhichSide.Left && cameraPos.x > enemyGeneral.transform.localPosition.x)
			    || (side == WarSceneController.WhichSide.Right && cameraPos.x < enemyGeneral.transform.localPosition.x)) {
				
				state = 2;
				Invoke("WaitForMagicOver", 1);

				listMan[0].SetActive(false);
				enemyGeneral.OnDamage(MagicManager.Instance.GetMagicDataInfo(40).ATTACK, -1, false);

				Rect region = new Rect();
				if (side == WarSceneController.WhichSide.Left) {
					region.x = listMan[0].transform.localPosition.x - 20;
				} else {
					region.x = listMan[0].transform.localPosition.x - 180;
				}
				region.y = listMan[0].transform.localPosition.y - 20;
				region.width = 200;
				region.height = 40;
				warCtrl.OnMagicHitChecking(side, 0, region, false);
				
				StartCoroutine(ShowArrowExplode());
			} else {
				warCtrl.SetCameraPosition(cameraPos);

				Rect region = new Rect();
				region.x = listMan[0].transform.localPosition.x;
				region.y = listMan[0].transform.localPosition.y - 20;
				region.width = 20;
				region.height = 40;
				warCtrl.OnMagicHitChecking(side, 0, region, false);
			}
		} else if (state == 2) {
			
			for (int i=0; i<listMan.Count; i++) {
				Vector3 pos = listMan[i].transform.localPosition;
				pos.x += deltaPos;
				listMan[i].transform.localPosition = pos;
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
		cameraPos.y = -30;
	}
	
	void GenerateMan() {
		
		Vector3 scale;
		if (side == WarSceneController.WhichSide.Left) {
			scale = WarSceneController.manScaleLeft;
		} else {
			scale = WarSceneController.manScaleRight;
		}

		for (int i=0; i<10; i++) {
			Vector3 pos = general.transform.localPosition;
			pos.y = 0.3f * i;
			pos.z += zOff;
			GameObject go = InstantiateOneGO(manName, pos, scale, WarSceneController.manEulerAngles);
			go.SetActive(false);
			go.GetComponent<exSprite>().color = new Color(1, 1, 1, 1 - 0.08f * i);
			listMan.Add(go);
		}

		GameObject bottomGo = InstantiateOneGO(bottomName, general.transform.localPosition, scale, WarSceneController.manEulerAngles);
		bottomGo.transform.parent = listMan[0].transform;
		bottomGo.transform.localPosition = bottomOffset;
		bottomGo.transform.localScale = Vector3.one;
		bottomGo.transform.localEulerAngles = Vector3.zero;
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

			Vector3 pos = enemyGeneral.transform.localPosition;
			pos.x += Random.Range(-30, 30);
			pos.y = -5;
			pos.z += Random.Range(-30, 0);

			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			go.transform.localPosition = pos;
			
			yield return new WaitForSeconds(0.1f);
		}
		
		Invoke("WaitForMagicOver", 0.5f);
	}
	
	void WaitForArrowOut() {
		state = 1;

		StartCoroutine(ShowMan());

		SoundController.Instance.PlaySound("00019");
	}

	IEnumerator ShowMan() {
		for (int i=0; i<listMan.Count; i++) {
			listMan[i].SetActive(true);
			yield return new WaitForSeconds(0.03f);
		}
	}
	
	void WaitForMagicOver() {
		Destroy(gameObject);
		MagicController.Instance.OnMagicOver();
	}
}
