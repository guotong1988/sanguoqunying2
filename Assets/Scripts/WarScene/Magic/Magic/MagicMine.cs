using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicMine : MonoBehaviour {

	public int level;

	private WarSceneController warCtrl;

	private List<GameObject> listMine = new List<GameObject>();

	private Transform generalPos;
	private Rect region;
	private Vector2 rangeBase = new Vector2(200, 200);

	// Use this for initialization
	void Start () {

		Init();
		GenerateMine();
		Invoke("WaitForCameraMoveToGeneral", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Init() {

		warCtrl = MagicController.Instance.warCtrl;
		
		if (MagicController.Instance.GetMagicSide() == WarSceneController.WhichSide.Left) {
			generalPos = MagicController.Instance.warCtrl.leftGeneral.transform;
		} else {
			generalPos = MagicController.Instance.warCtrl.rightGeneral.transform;
		}
		
		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	void GenerateMine() {

		string mineName = "Magic/Mine/Mine";
		Vector2 range = rangeBase * level;
		region = new Rect(
			generalPos.localPosition.x - range.x / 2,
			generalPos.localPosition.y - range.y / 2,
			range.x,
			range.y);
		for (int i=0; i<level*10; i++) {
			GameObject go = (GameObject)Instantiate(Resources.Load(mineName));
			go.SetActive(false);
			go.transform.parent = this.transform;
			go.transform.localPosition = new Vector3(
				Random.Range(generalPos.localPosition.x - region.width / 2, generalPos.localPosition.x + region.width / 2),
				Random.Range(generalPos.localPosition.y - region.height / 2, generalPos.localPosition.y + region.height / 2),
				0);
			go.transform.localEulerAngles = WarSceneController.manEulerAngles;
			go.transform.localScale = WarSceneController.manScaleRight;

			listMine.Add(go);
		}
	}

	void WaitForCameraMoveToGeneral() {

		warCtrl.OnMagicHitChecking(MagicController.Instance.GetMagicSide(), 
		                           MagicController.Instance.GetMagicAttack(),
		                           region,
		                           true);

		StartCoroutine(ShowMine());

		SoundController.Instance.PlaySound("00055");
	}

	IEnumerator ShowMine() {
		for (int i=0; i<listMine.Count; i++) {
			listMine[i].SetActive(true);
			if (i % 3 == 0) {
				yield return new WaitForSeconds(0.1f);
			}
		}

		Invoke("WaitForMagicOver", 0.5f);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
