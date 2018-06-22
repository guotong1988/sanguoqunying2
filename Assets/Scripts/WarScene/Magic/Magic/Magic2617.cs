using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Magic2617 : MonoBehaviour {

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private bool hitGeneral;
	private Vector3 enemyCenterPos;
	private int downIndex;

	private GameObject yellowDragon;
	private GameObject particalGO;
	private List<GameObject> listFireUp = new List<GameObject>();
	private List<GameObject> listFireDown = new List<GameObject>();
	private List<GameObject> listFireExplode = new List<GameObject>();
	
	private string yellowDragonName = "Magic/YellowDragon/YellowDragon";
	private string fireUpName = "Magic/YellowDragon/FireUp";
	private string fireDownName = "Magic/YellowDragon/FireDown";
	private string fireExplodeName = "Magic/YellowDragon/FireExplode";
	private string particalName1 = "Magic/YellowDragon/YellowDragonParticle1";
	private string explodeName = "Magic/Misc/Explode";

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
		side = MagicController.Instance.GetMagicSide();
		enemySide = MagicController.Instance.GetMagicEnemySide();
		enemyCenterPos = warCtrl.GetArmyCentrePoint(enemySide);

		Vector3 posGeneral;
		if (side == WarSceneController.WhichSide.Left) {
			posGeneral = warCtrl.leftGeneral.transform.localPosition;
		} else {
			posGeneral = warCtrl.rightGeneral.transform.localPosition;
		}
		yellowDragon = InstantiateOneGO(yellowDragonName, posGeneral, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
		yellowDragon.SetActive(false);

		particalGO = (GameObject)Instantiate(Resources.Load(particalName1));
		particalGO.transform.parent = transform;
		particalGO.transform.localPosition = posGeneral;

		for (int i=0; i<10; i++) {

			Vector3 posUp = posGeneral;
			posUp.x += Random.Range(-30, 30);
			GameObject go = InstantiateOneGO(fireUpName, posUp, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listFireUp.Add(go);

			go = InstantiateOneGO(fireDownName, Vector3.zero, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listFireDown.Add(go);
			MagicFireDown fireDownScript = go.GetComponent<MagicFireDown>();
			SolidersController sCtrl = MagicController.Instance.warCtrl.GetRandomSolider(enemySide);
			Vector3 pos;
			if (sCtrl != null) {
				pos = sCtrl.transform.localPosition;
				pos.z = -150;
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
					pos.z = -150;
				} else {
					pos = new Vector3(enemyCenterPos.x + Random.Range(-150, 150), 
					                  enemyCenterPos.y + Random.Range(-80, 80),
					                  -150);
				}
			}

			fireDownScript.SetInfo(pos, this);

			go = InstantiateOneGO(fireExplodeName, Vector3.zero, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listFireExplode.Add(go);
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

	void WaitForCameraMoveToGeneral() {

		yellowDragon.SetActive(true);

		Invoke("WaitForYellowDragonFly", 1);

		SoundController.Instance.PlaySound("00020");
	}

	void WaitForYellowDragonFly() {

		StartCoroutine(ShowFireUp());

		Invoke("WaitForShowFireDown", 1);
	}

	IEnumerator ShowFireUp() {

		for (int i=0; i<10; i++) {

			listFireUp[i].SetActive(true);
			yield return new WaitForSeconds(0.1f);
		}

		Destroy(particalGO);

		warCtrl.SetCameraMoveTo(new Vector3(enemyCenterPos.x, -90, 0));
	}

	void WaitForShowFireDown() {

		StartCoroutine(ShowFireDown());
	}

	IEnumerator ShowFireDown() {

		for (int i=0; i<10; i++) {
			
			listFireDown[i].SetActive(true);
			yield return new WaitForSeconds(0.1f);
		}
	}

	void WaitForMagicOver() {
		Destroy(gameObject);
		MagicController.Instance.OnMagicOver();
	}

	public void OnFireDownOver(Vector3 pos) {

		GameObject go = listFireExplode[downIndex++];
		go.SetActive(true);
		go.transform.localPosition = pos;

		go = (GameObject)Instantiate(Resources.Load(explodeName));
		go.transform.parent = transform;
		go.transform.localPosition = pos;

		Rect region = new Rect();
		region.x = pos.x - 40;
		region.y = pos.y - 40;
		region.width = 80;
		region.height = 80;

		warCtrl.OnMagicHitChecking(side, MagicManager.Instance.GetMagicDataInfo(14).ATTACK, region, true);

		if (downIndex == 10) {
			Invoke("WaitForMagicOver", 0.5f);
		}
	}
}
