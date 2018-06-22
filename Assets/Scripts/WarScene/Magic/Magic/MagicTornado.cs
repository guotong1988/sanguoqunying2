using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicTornado : MonoBehaviour {

	public int level;

	private bool isHitGeneral;

	private Vector3 enemyCenterPos;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private List<GameObject> listTornado = new List<GameObject>();

	private string tornadoName = "Magic/Tornado/Tornado";

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

		List<SolidersController> listSoldier = new List<SolidersController>();
		for (int i=0; i<level; i++) {

			SolidersController sCtrl = MagicController.Instance.warCtrl.GetRandomSolider(enemySide);
			Vector3 pos;
			if (sCtrl != null && !listSoldier.Contains(sCtrl)) {
				pos = sCtrl.transform.localPosition;
				listSoldier.Add(sCtrl);
			} else {
				if (!isHitGeneral && Random.Range(0, 100) < 40) {
					isHitGeneral = true;
					GeneralController gCtrl;
					if (enemySide == WarSceneController.WhichSide.Left) {
						gCtrl = MagicController.Instance.warCtrl.leftGeneral;
					} else {
						gCtrl = MagicController.Instance.warCtrl.rightGeneral;
					}
					pos = gCtrl.transform.localPosition;
				} else {
					pos = enemyCenterPos;
					pos.x += Random.Range(-180, 180);
					pos.y += Random.Range(-100, 100);
				}
			}

			GameObject go = InstantiateOneGO(tornadoName, pos, Vector3.one, Vector3.zero);
			go.SetActive(false);
			listTornado.Add(go);
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

		StartCoroutine(ShowTornado());
	}

	IEnumerator ShowTornado() {

		for (int i=0; i<listTornado.Count; i++) {

			Vector3 pos = listTornado[i].transform.localPosition;
			pos.y -= 30;
			warCtrl.SetCameraMoveTo(pos);

			yield return new WaitForSeconds(0.3f);

			listTornado[i].SetActive(true);

			Rect region = new Rect();
			region.x = listTornado[i].transform.localPosition.x - 50;
			region.y = listTornado[i].transform.localPosition.y - 50;
			region.width = 100;
			region.height = 100;

			warCtrl.OnMagicHitChecking(side, MagicController.Instance.GetMagicAttack(), region, false);

			yield return new WaitForSeconds(0.3f);
		}

		Invoke("WaitForMagicOver", 1.5f);

		if (listTornado.Count < 3) {
			SoundController.Instance.PlaySound("00007");
		} else {
			SoundController.Instance.PlaySound("00008");
		}
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
