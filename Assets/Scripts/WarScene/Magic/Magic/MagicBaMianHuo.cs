using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBaMianHuo : MonoBehaviour {

	public int level;
	
	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;

	private List<GameObject> listFire = new List<GameObject>();

	private Vector3 posGeneral;
	private float radius = 100;

	private string fireName = "Magic/BaMianHuo/BaMianHuo";

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

		if (side == WarSceneController.WhichSide.Left) {
			posGeneral = warCtrl.leftGeneral.transform.localPosition;
		} else {
			posGeneral = warCtrl.rightGeneral.transform.localPosition;
		}

		float angle = 2 * Mathf.PI / 8;
		for (int i=0; i<level; i++) {
			for (int j=0; j<8; j++) {
				Vector3 pos = posGeneral;
				pos.x += radius * level * Mathf.Cos(angle * (j + 0.5f));
				pos.y += radius * level * Mathf.Sin(angle * (j + 0.5f));

				GameObject go = InstantiateOneGO(fireName, posGeneral, WarSceneController.manScaleRight, WarSceneController.manEulerAngles);
				go.GetComponent<MagicBaMianHuoAct>().SetInfo(posGeneral, pos);
				go.SetActive(false);
				listFire.Add(go);
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

		Rect region = new Rect();
		region.x = posGeneral.x - radius * level;
		region.y = posGeneral.y - radius * level;
		region.width = radius * level * 2;
		region.height = radius * level * 2;

		warCtrl.OnMagicHitChecking(side, MagicController.Instance.GetMagicAttack(), region, true);

		StartCoroutine(ShowFire());

		SoundController.Instance.PlaySound("00019");
	}

	IEnumerator ShowFire() {

		for (int i=0; i<level; i++) {
			for (int j=0; j<8; j++) {
				listFire[i * 8 + j].SetActive(true);
			}
			yield return new WaitForSeconds(0.1f);
		}

		Invoke("WaitForMagicOver", 0.5f);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
