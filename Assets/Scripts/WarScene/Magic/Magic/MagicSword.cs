using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicSword : MonoBehaviour {

	public int level;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private List<GameObject> listSwordUp = new List<GameObject>();
	private List<GameObject> listSwordDown = new List<GameObject>();
	private List<GameObject> listSwordDownMain = new List<GameObject>();

	private float radius = 50;

	private string swordUpName = "Magic/MagicSword/MagicSwordUp";
	private string swordDownName = "Magic/MagicSword/MagicSwordDown";
	private string swordDownMainName = "Magic/MagicSword/MagicSwordDownMain";
	private string swordParticalName = "Magic/MagicSword/MagicSwordPartical";

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

		Vector3 posGeneral;
		if (side == WarSceneController.WhichSide.Left) {
			posGeneral = warCtrl.leftGeneral.transform.localPosition;
		} else {
			posGeneral = warCtrl.rightGeneral.transform.localPosition;
		}

		float angle = 2 * Mathf.PI / (level * 4);
		for (int i=0; i<level * 4; i++) {
			Vector3 pos = posGeneral;
			pos.x += radius * Mathf.Cos(angle * (i + 0.5f));
			pos.y += radius * Mathf.Sin(angle * (i + 0.5f));
			pos.z = -50;
			GameObject go = InstantiateOneGO(swordUpName, pos, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listSwordUp.Add(go);
		}

		List<SolidersController> listSoldier = new List<SolidersController>();
		for (int i=0; i<level; i++) {

			SolidersController sCtrl = MagicController.Instance.warCtrl.GetRandomSolider(enemySide);
			Vector3 pos;
			if (sCtrl != null && !listSoldier.Contains(sCtrl)) {
				pos = sCtrl.transform.localPosition;
				pos.z = -150;
				listSoldier.Add(sCtrl);
			} else {

				GeneralController gCtrl;
				if (enemySide == WarSceneController.WhichSide.Left) {
					gCtrl = MagicController.Instance.warCtrl.leftGeneral;
				} else {
					gCtrl = MagicController.Instance.warCtrl.rightGeneral;
				}
				pos = gCtrl.transform.localPosition;
				pos.z = -150;
			}
			GameObject go = InstantiateOneGO(swordDownMainName, pos, WarSceneController.manScaleRight * 3f, WarSceneController.manEulerAngles);
			go.SetActive(false);
			go.GetComponent<MagicSwordDonwMain>().SetInfo(this);
			listSwordDownMain.Add(go);

			for (int j=0; j<8; j++) {
				Vector3 posDown = pos;
				posDown.x += Random.Range(-50, 50);
				posDown.y += Random.Range(-50, 50);
				go = InstantiateOneGO(swordDownName, posDown, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
				go.SetActive(false);
				listSwordDown.Add(go);
			}
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

		StartCoroutine(ShowSwordUp());
		SoundController.Instance.PlaySound("00010");
	}

	IEnumerator ShowSwordUp() {

		for (int i=0; i<listSwordUp.Count; i++) {
			listSwordUp[i].SetActive(true);

			if (i % 2 == 0) {
				yield return new WaitForSeconds(0.05f);
			}
		}

		Invoke("WaitForShowDownSword", 1.5f);
	}

	void WaitForShowDownSword() {
		StartCoroutine(ShowSwordDown());
	}

	IEnumerator ShowSwordDown() {

		for (int i=0; i<level; i++) {

			Vector3 pos = listSwordDownMain[i].transform.localPosition;
			pos.y -= 30;
			pos.y = Mathf.Clamp(pos.y, -WarSceneController.cameraPosMaxY, WarSceneController.cameraPosMaxY);
			pos.z = 0;
			warCtrl.SetCameraMoveTo(pos);

			for (int j=0; j<8; j++) {
				listSwordDown[i * 8 + j].SetActive(true);
				if (j % 2 == 0) {
					yield return new WaitForSeconds(0.05f);
				}
				if (j == 4) {
					listSwordDownMain[i].SetActive(true);
				}
			}

			yield return new WaitForSeconds(1f);
		}

		Invoke("WaitForMagicOver", 1);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}

	public void OnSwordDownOver(Vector3 pos) {

		GameObject go = (GameObject)Instantiate(Resources.Load(swordParticalName));
		go.transform.parent = transform;
		go.transform.localPosition = pos;

		Rect region = new Rect();
		region.x = pos.x - 60;
		region.y = pos.y - 60;
		region.width = 120;
		region.height = 120;
		
		warCtrl.OnMagicHitChecking(side, MagicController.Instance.GetMagicAttack(), region, false);
	}
}
