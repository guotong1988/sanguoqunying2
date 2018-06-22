using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicArcher : MonoBehaviour {

	public int level;
	public bool isWithFire;

	private Vector3 enemyCenterPoint;
	private int arrowDownIndex;

	private List<GameObject> listArrowUp = new List<GameObject>();
	private List<GameObject> listArrowDown = new List<GameObject>();

	private bool hitGeneral;

	private Vector3 cameraPosLeft = new Vector3(-940, -180, 0);
	private Vector3 cameraPosRight = new Vector3(940, -180, 0);

	private Vector3 arrowOffLeft = new Vector3(30, 0, -48);
	private Vector3 arrowOffRight = new Vector3(-30, 0, -48);
	private Vector3 arrowDownOffLeft = new Vector3(-150, 0, -150);
	private Vector3 arrowDownOffRight = new Vector3(150, 0, -150);
	private Vector3 manBasePosLeft = new Vector3(-1080, 180, 0);
	private Vector3 manBasePosRight = new Vector3(1080, 180, 0);
	private float manYStep = -36;
	private float manXStepLeft = -36;
	private float manXStepRight = 36;

	private string archerNameLeft = "Magic/Archer/ArcherRed";
	private string archerNameRight = "Magic/Archer/ArcherGreen";
	private string arrowUpName = "Magic/Archer/ArrowUp";
	private string arrowDownName = "Magic/Archer/ArrowDown";
	private string fireArcherNameLeft = "Magic/Archer/FireArcherRed";
	private string fireArcherNameRight = "Magic/Archer/FireArcherGreen";
	private string fireArrowUpName = "Magic/Archer/FireArrowUp";
	private string fireArrowDownName = "Magic/Archer/FireArrowDown";


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

		float xStep = 0;
		Vector3 arrowOff = Vector3.zero;
		Vector3 arrowDownOff = Vector3.zero;
		Vector3 posBase = Vector3.zero;
		Vector3 scale = Vector3.zero;
		string archerName = "";
		string arrowUpName = "";
		string arrowDownName = "";

		WarSceneController.WhichSide side = MagicController.Instance.GetMagicSide();
		WarSceneController.WhichSide enemySide = 
			(side == WarSceneController.WhichSide.Left) ? WarSceneController.WhichSide.Right : WarSceneController.WhichSide.Left;

		if (side == WarSceneController.WhichSide.Left) {
			xStep 			= this.manXStepLeft;
			arrowOff 		= this.arrowOffLeft;
			arrowDownOff	= this.arrowDownOffLeft;
			posBase 		= this.manBasePosLeft;
			scale			= WarSceneController.manScaleLeft;

			if (!isWithFire) {
				archerName = this.archerNameLeft;
			} else {
				archerName = this.fireArcherNameLeft;
			}
		} else {
			xStep 			= this.manXStepRight;
			arrowOff 		= this.arrowOffRight;
			arrowDownOff	= this.arrowDownOffRight;
			posBase 		= this.manBasePosRight;
			scale			= WarSceneController.manScaleRight;

			if (!isWithFire) {
				archerName	= this.archerNameRight;
			} else {
				archerName = this.fireArcherNameRight;
			}
		}

		if (!isWithFire) {
			arrowUpName = this.arrowUpName;
			arrowDownName = this.arrowDownName;
		} else {
			arrowUpName = this.fireArrowUpName;
			arrowDownName = this.fireArrowDownName;
		}

		this.enemyCenterPoint = MagicController.Instance.warCtrl.GetArmyCentrePoint(enemySide);
		List<SolidersController> listSolidersAdded = new List<SolidersController>();

		for (int i=0; i<level; i++) {
			for (int j=0; j<10; j++) {

				GameObject archerGO = InstantiateOneGO(archerName, posBase, scale);
				archerGO.GetComponent<MGArcherCtrl>().SetInfo(i*10+j, isWithFire, Random.Range(1f, 1.5f), this);

				GameObject arrowUpGO = InstantiateOneGO(arrowUpName, posBase + arrowOff, scale);
				arrowUpGO.SetActive(false);
				listArrowUp.Add(arrowUpGO);
				arrowUpGO.GetComponent<MGArrowUpCtrl>().SetInfo(side);

				GameObject arrowDownGO = InstantiateOneGO(arrowDownName, Vector3.zero, scale);
				arrowDownGO.SetActive(false);
				listArrowDown.Add(arrowDownGO);

				Vector3 pos;
				SolidersController sCtrl = MagicController.Instance.warCtrl.GetRandomSolider(enemySide);
				if (sCtrl != null && !listSolidersAdded.Contains(sCtrl)) {
					listSolidersAdded.Add(sCtrl);

					pos = sCtrl.transform.localPosition;
					pos.z = 0;
					pos += arrowDownOff;
					arrowDownGO.GetComponent<MGArrowDownCtrl>().SetInfo(isWithFire, side, pos, sCtrl, null);
				} else {
					if (!hitGeneral && Random.Range(0, 100) < 20) {
						hitGeneral = true;
						GeneralController gCtrl;
						if (enemySide == WarSceneController.WhichSide.Left) {
							gCtrl = MagicController.Instance.warCtrl.leftGeneral;
						} else {
							gCtrl = MagicController.Instance.warCtrl.rightGeneral;
						}
						pos = gCtrl.transform.localPosition;
						pos.z = 0;
						pos += arrowDownOff;
						arrowDownGO.GetComponent<MGArrowDownCtrl>().SetInfo(isWithFire, side, pos, null, gCtrl);
					} else {
						pos = new Vector3(enemyCenterPoint.x + Random.Range(-150, 150), 
						                  enemyCenterPoint.y + Random.Range(-80, 80),
						                  -150);
						arrowDownGO.GetComponent<MGArrowDownCtrl>().SetInfo(isWithFire, side, pos, null, null);
					}
				}

				posBase.y += this.manYStep;
			}
			posBase.x += xStep;
			posBase.y -= this.manYStep * 10;
		}
	}

	GameObject InstantiateOneGO(string goName, Vector3 pos, Vector3 scale) {

		GameObject go = (GameObject)Instantiate(Resources.Load(goName));
		go.transform.parent = this.transform;
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.transform.eulerAngles = WarSceneController.manEulerAngles;

		return go;
	}

	void WaitForCameraMoveToGeneral() {
		
		WarSceneController.WhichSide side = MagicController.Instance.GetMagicSide();
		if (side == WarSceneController.WhichSide.Left) {
			MagicController.Instance.warCtrl.SetCameraMoveTo(cameraPosLeft);
		} else {
			MagicController.Instance.warCtrl.SetCameraMoveTo(cameraPosRight);
		}

		Invoke("WaitForMoveCameraToDownArrow", 1.5f);
	}

	void WaitForMoveCameraToDownArrow() {
		MagicController.Instance.warCtrl.SetCameraMoveTo(new Vector3(enemyCenterPoint.x, -180, 0));

		for (int i=0; i<listArrowDown.Count; i++) {
			Invoke("ShowArrowDown", Random.Range(0.5f, 1f));
		}

		Invoke("WaitForMagicOver", 1.5f);
	}

	void ShowArrowDown() {
		listArrowDown[arrowDownIndex++].SetActive(true);
	}

	void WaitForMagicOver() {
		Destroy(gameObject);
		MagicController.Instance.OnMagicOver();
	}

	public void ShowArrowUp(int index) {
		listArrowUp[index].SetActive(true);
	}
}
