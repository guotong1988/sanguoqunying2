using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicLightningStroke : MonoBehaviour {

	public int level;

	private List<GameObject> listLightning = new List<GameObject>();
	private List<SolidersController> soliderHit = new List<SolidersController>();

	private GeneralController gCtrl;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide enemySide;
	private Vector3 armyCenterPoint;
	private bool hitGeneral;

	private string prefabName = "Magic/LightningStroke/LightningStroke";
	private string explodeName = "Magic/Misc/Explode";

	// Use this for initialization
	void Start () {

		Init();
		GenerateLightning();
		Invoke("WaitForCameraMoveToGeneral", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Init() {

		warCtrl = MagicController.Instance.warCtrl;
		enemySide = MagicController.Instance.GetMagicEnemySide();
		armyCenterPoint = warCtrl.GetArmyCentrePoint(enemySide);
		
		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	void GenerateLightning() {

		for (int i=0; i<level*10; i++) {

			GameObject go = (GameObject)Instantiate(Resources.Load(prefabName));
			go.transform.parent = transform;
			go.transform.localEulerAngles = WarSceneController.manEulerAngles;
			go.transform.localScale = WarSceneController.manScaleRight;

			Vector3 pos = Vector3.zero;
			SolidersController sCtrl = MagicController.Instance.warCtrl.GetRandomSolider(enemySide);
			if (sCtrl != null && !soliderHit.Contains(sCtrl)) {
				pos = sCtrl.transform.localPosition;
				soliderHit.Add(sCtrl);
			} else {
				if (!hitGeneral && Random.Range(0, 100) < 20) {
					hitGeneral = true;

					if (MagicController.Instance.GetMagicEnemySide() == WarSceneController.WhichSide.Left) {
						gCtrl = MagicController.Instance.warCtrl.leftGeneral;
					} else {
						gCtrl = MagicController.Instance.warCtrl.rightGeneral;
					}
					pos = gCtrl.transform.localPosition;
				} else {
					pos = new Vector3(armyCenterPoint.x + Random.Range(-150, 150), 
					                  armyCenterPoint.y + Random.Range(-80, 80),
					                  5);
				}
			}
			pos.z = 5;
			go.transform.localPosition = pos;

			go.SetActive(false);
			listLightning.Add(go);
		}
	}

	void WaitForCameraMoveToGeneral() {
		warCtrl.SetCameraMoveTo(new Vector3(armyCenterPoint.x, -30, 0));
		
		Invoke("WaitForShowLightningStroke", 0.3f);
	}

	void WaitForShowLightningStroke() {
		StartCoroutine(ShowLightningStroke());
	}

	IEnumerator ShowLightningStroke() {

		for (int i=0; i<soliderHit.Count; i++) {
			soliderHit[i].OnDamage(-1, (WarSceneController.Direction)Random.Range(0, 2));

			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			go.transform.localPosition = soliderHit[i].transform.localPosition;
		}

		if (hitGeneral) {
			gCtrl.OnDamage(MagicController.Instance.GetMagicAttack(), -1, false);

			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			go.transform.localPosition = gCtrl.transform.localPosition;
		}

		for (int i=0; i<listLightning.Count; i++) {
			listLightning[i].SetActive(true);
			if (i % 3 == 0) {
				yield return new WaitForSeconds(0.1f);
			}
		}

		Invoke("WaitForMagicOver", 1f);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
