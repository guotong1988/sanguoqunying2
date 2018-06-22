using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicTuShi : MonoBehaviour {

	public int level;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private GeneralController gCtrl;

	private List<GameObject> listStones = new List<GameObject>();

	private int[] levelNum = new int[]{1, 3, 6};
	private string stoneName = "Magic/TuShi/TuShi";
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

		Vector3 posGeneral;
		if (enemySide == WarSceneController.WhichSide.Left) {
			gCtrl = warCtrl.leftGeneral;
			posGeneral = warCtrl.leftGeneral.transform.localPosition;
		} else {
			gCtrl = warCtrl.rightGeneral;
			posGeneral = warCtrl.rightGeneral.transform.localPosition;
		}

		Vector3 pos = posGeneral;
		GameObject go = InstantiateOneGO(stoneName, pos, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
		go.SetActive(false);
		listStones.Add(go);

		for (int i=1; i<levelNum[level-1]; i++) {
			pos = posGeneral;
			pos.x += Random.Range(-100, 100);
			pos.y += Random.Range(-100, 100);
			go = InstantiateOneGO(stoneName, pos, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
			go.SetActive(false);
			listStones.Add(go);
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

		Vector3 pos = listStones[0].transform.localPosition;
		pos.y = -30;
		warCtrl.SetCameraMoveTo(pos);

		StartCoroutine(ShowStones());
	}

	IEnumerator ShowStones() {

		yield return new WaitForSeconds(0.3f);

		gCtrl.OnDamage(MagicController.Instance.GetMagicAttack(), -1, false);

		for (int i=0; i<listStones.Count; i++) {
			listStones[i].SetActive(true);

			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			go.transform.localPosition = listStones[i].transform.localPosition;

			Rect region = new Rect();
			region.x = listStones[i].transform.localPosition.x - 60;
			region.y = listStones[i].transform.localPosition.y - 60;
			region.width = 120;
			region.height = 120;
			
			warCtrl.OnMagicHitChecking(side, 0, region, false);

			yield return new WaitForSeconds(0.2f);
		}

		Invoke("WaitForMagicOver", 1);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
