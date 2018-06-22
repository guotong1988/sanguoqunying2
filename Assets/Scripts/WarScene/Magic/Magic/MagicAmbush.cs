using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicAmbush : MonoBehaviour {

	public int level;

	public Transform mapPointPrefabLeft;
	public Transform horsePrefabLeft;
	public Transform mapPointPrefabRight;
	public Transform horsePrefabRight;

	private WarSceneController.WhichSide side;
	private Transform soldierRoot;

	private List<SolidersController> listSolider = new List<SolidersController>();

	private Vector3 cameraPosLeft = new Vector3(-940, -180, 0);
	private Vector3 cameraPosRight = new Vector3(940, -180, 0);

	private Vector3 manPosBaseLeft = new Vector3(-1000, 180, 0);
	private Vector3 manPosBaseRight = new Vector3(1000, 180, 0);

	private float manYStep = -36;
	private float manXStepLeft = 36;
	private float manXStepRight = -36;

	private int[] levelNum = new int[]{4, 8, 16, 30};

	// Use this for initialization
	void Start () {

		Init();
		Invoke("WaitForCameraMoveToGeneral", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Init() {

		side = MagicController.Instance.GetMagicSide();

		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;

		int soldierCur = 0;
		int soldierMax = 0;
		int addNum = 0;
		float manXStep = 0;
		Vector3 pos = Vector3.zero;
		GeneralInfo gInfo;
		if (side == WarSceneController.WhichSide.Left) {
			manXStep = manXStepLeft;
			pos = manPosBaseLeft;
			gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.leftGeneralIdx);

			soldierRoot = GameObject.Find("LeftSoliders").transform;
		} else {
			manXStep = manXStepRight;
			pos = manPosBaseRight;
			gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);

			soldierRoot = GameObject.Find("RightSoliders").transform;
		}

		soldierCur = gInfo.soldierCur + gInfo.knightCur;
		soldierMax = gInfo.soldierMax + gInfo.knightMax;
		soldierCur += levelNum[level];
		if (soldierCur > soldierMax)	
			soldierCur = soldierMax;
		addNum = soldierCur - (gInfo.soldierCur + gInfo.knightCur);

		if (addNum == 0) 	return;

		int addIdx = 0;
		int soliderType = Misc.GetArmsIdx(gInfo.armsCur);

		int knightNum = gInfo.knightMax - gInfo.knightCur;
		for (int i=0; i<knightNum; i++) {

			GenerateOneSoldier(soliderType, pos, true);
			gInfo.knightCur++;

			addNum--;
			if (addNum == 0) 	return;

			addIdx++;
			if (addIdx == 10) {
				addIdx = 0;
				pos.x += manXStep;
				pos.y = 180;
			} else {
				pos.y += manYStep;
			}
		}

		int footSoldierNum = gInfo.soldierMax - gInfo.soldierCur;
		for (int i=0; i<footSoldierNum; i++) {
			GenerateOneSoldier(soliderType, pos, false);
			gInfo.soldierCur++;
			
			addNum--;
			if (addNum == 0) 	return;

			addIdx++;
			if (addIdx == 10) {
				addIdx = 0;
				pos.x += manXStep;
				pos.y = 180;
			} else {
				pos.y += manYStep;
			}
		}
	}

	void GenerateOneSoldier(int soliderType, Vector3 pos, bool isKnight) {

		string sName = "Soliders/";
		if (side == WarSceneController.WhichSide.Left) {
			sName += "Red/";
		} else {
			sName += "Green/";
		}
		
		if (!isKnight) {
			if (soliderType < 9) {
				sName += "Solider00" + (soliderType+1);
			} else {
				sName += "Solider0" + (soliderType+1);
			}
		} else {
			if (soliderType < 9) {
				sName += "Knight00" + (soliderType+1);
			} else {
				sName += "Knight0" + (soliderType+1);
			}
		}
		
		GameObject go = (GameObject)Instantiate(Resources.Load(sName));
		go.transform.parent = soldierRoot;
		
		if (isKnight) {
			Transform horsePrefab;
			if (side == WarSceneController.WhichSide.Left) {
				horsePrefab = horsePrefabLeft;
			} else {
				horsePrefab = horsePrefabRight;
			}
			Transform horse = (Transform)Instantiate(horsePrefab);
			horse.gameObject.AddComponent<SolidersHorse>();
			horse.gameObject.SetActive(false);
			horse.parent = go.transform;
			horse.localPosition = Vector3.zero;
			horse.localScale = Vector3.one;
		}
		
		go.transform.localPosition = pos;
		go.transform.eulerAngles = WarSceneController.manEulerAngles;
		
		if (side == WarSceneController.WhichSide.Left) {
			go.transform.localScale = WarSceneController.manScaleLeft;
		} else {
			go.transform.localScale = WarSceneController.manScaleRight;
		}
		
		SolidersController soliderCtrl = go.AddComponent<SolidersController>();
		
		Transform mapFrame = GameObject.Find("Map").transform;
		Transform mapPointPrefab;
		if (side == WarSceneController.WhichSide.Left) {
			mapPointPrefab = mapPointPrefabLeft;
		} else {
			mapPointPrefab = mapPointPrefabRight;
		}
		Transform mapPoint = (Transform)Instantiate(mapPointPrefab);
		mapPoint.parent = mapFrame;
		mapPoint.localPosition = Vector3.zero;
		
		soliderCtrl.SetMapPoint(mapPoint);
		soliderCtrl.SetSide(side);
		soliderCtrl.SetType(soliderType);
		soliderCtrl.SetIsKnight(isKnight);
		
		if (side == WarSceneController.WhichSide.Left) {
			soliderCtrl.SetIndex(-Random.Range(100, 10000));
		} else {
			soliderCtrl.SetIndex(Random.Range(100, 10000));
		}

		listSolider.Add(soliderCtrl);
		MagicController.Instance.warCtrl.AddSolider(side, soliderCtrl);
		MagicController.Instance.warCtrl.AddBackSolider(side, soliderCtrl);
	}

	void WaitForCameraMoveToGeneral() {

		GeneralController gCtrl;
		if (side == WarSceneController.WhichSide.Left) {
			MagicController.Instance.warCtrl.SetCameraMoveTo(cameraPosLeft);
			gCtrl = MagicController.Instance.warCtrl.leftGeneral;
		} else {
			MagicController.Instance.warCtrl.SetCameraMoveTo(cameraPosRight);
			gCtrl = MagicController.Instance.warCtrl.rightGeneral;
		}

		if (gCtrl.GetArmyAssaultFlag()) {
			for (int i=0; i<listSolider.Count; i++) {
				listSolider[i].SetRun();
			}
		}

		Invoke("WaitForMoveBackToGeneral", 1);

		SoundController.Instance.PlaySound("00014");
	}

	void WaitForMoveBackToGeneral() {

		Vector3 pos;
		if (side == WarSceneController.WhichSide.Left) {
			pos = MagicController.Instance.warCtrl.leftGeneral.transform.localPosition;
		} else {
			pos = MagicController.Instance.warCtrl.rightGeneral.transform.localPosition;
		}
		pos.y = -30;
		MagicController.Instance.warCtrl.SetCameraMoveTo(pos);

		Invoke("WaitForMagicOver", 0.3f);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
