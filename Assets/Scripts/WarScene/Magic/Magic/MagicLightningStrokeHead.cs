using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicLightningStrokeHead : MonoBehaviour {

	private List<GameObject> listLightning = new List<GameObject>();

	private GeneralController gCtrl;

	private string prefabName = "Magic/LightningStroke/LightningStrokeHead";
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
		
		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	void GenerateLightning() {

		for (int i=0; i<2; i++) {
			
			GameObject go = (GameObject)Instantiate(Resources.Load(prefabName));
			go.transform.parent = transform;
			go.transform.localEulerAngles = WarSceneController.manEulerAngles;
			go.transform.localScale = WarSceneController.manScaleRight;
			if (MagicController.Instance.GetMagicEnemySide() == WarSceneController.WhichSide.Left) {
				gCtrl = MagicController.Instance.warCtrl.leftGeneral;
			} else {
				gCtrl = MagicController.Instance.warCtrl.rightGeneral;
			}
			go.transform.localPosition = new Vector3(gCtrl.transform.localPosition.x,
			                                         gCtrl.transform.localPosition.y - i * 20,
			                                         0);
			
			go.SetActive(false);
			listLightning.Add(go);
		}
	}

	void WaitForCameraMoveToGeneral() {
		MagicController.Instance.warCtrl.SetCameraMoveTo(new Vector3(gCtrl.transform.localPosition.x, -30, 0));
		
		Invoke("WaitForShowLightningStroke", 0.3f);
	}

	void WaitForShowLightningStroke() {
		StartCoroutine(ShowLightningStroke());
	}
	
	IEnumerator ShowLightningStroke() {

		gCtrl.OnDamage(MagicManager.Instance.GetMagicDataInfo(15).ATTACK, -1, false);
		
		for (int i=0; i<listLightning.Count; i++) {
			listLightning[i].SetActive(true);
			yield return new WaitForSeconds(0.1f);
		}
		for (int i=0; i<5; i++) {
			GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
			go.transform.parent = transform;
			Vector3 pos = gCtrl.transform.localPosition;
			pos.x += Random.Range(-20, 20);
			pos.z += Random.Range(-20, 20);
			go.transform.localPosition = pos;
		}
		
		Invoke("WaitForMagicOver", 1f);
	}
	
	void WaitForMagicOver() {
		Destroy(gameObject);
		MagicController.Instance.OnMagicOver();
	}
}
