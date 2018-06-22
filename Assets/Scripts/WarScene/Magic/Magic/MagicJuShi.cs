using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicJuShi : MonoBehaviour {

	public int level;

	private bool hitGeneral;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private Vector3 enemyCenterPoint;

	private List<GameObject> listJuShi = new List<GameObject>();
	private List<GameObject> listJuShiDown = new List<GameObject>();

	private Vector3 cameraPosLeft = new Vector3(-940, -180, 0);
	private Vector3 cameraPosRight = new Vector3(940, -180, 0);

	private Vector3 juShiDownOffLeft = new Vector3(-50, 0, -150);
	private Vector3 juShiDownOffRight = new Vector3(50, 0, -150);

	private Vector3 juShiBasePosLeft = new Vector3(-1080, 130, 0);
	private Vector3 juShiBasePosRight = new Vector3(1080, 130, 0);

	private float juShiYStep = -50;
	private float juShiXStepLeft = -80;
	private float juShiXStepRight = 80;

	private int[] levelNum = new int[]{3, 6, 12};

	private string juShiName = "Magic/JuShi/JuShi";
	private string juShiDownName = "Magic/JuShi/JuShiDown";
	private string juShiParticalName = "Magic/JuShi/JuShiParticle";
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
		enemyCenterPoint = warCtrl.GetArmyCentrePoint(enemySide);

		Vector3 scale;
		Vector3 juShiDownOff;
		Vector3 juShiBasePos;
		float juShiXStep;
		if (side == WarSceneController.WhichSide.Left) {
			scale = WarSceneController.manScaleLeft;
			juShiDownOff = juShiDownOffLeft;
			juShiBasePos = juShiBasePosLeft;
			juShiXStep = juShiXStepLeft;
		} else {
			scale = WarSceneController.manScaleRight;
			juShiDownOff = juShiDownOffRight;
			juShiBasePos = juShiBasePosRight;
			juShiXStep = juShiXStepRight;
		}

		for (int i=0; i<levelNum[level-1]; i++) {

			int col = i / 6;
			int row = i % 6;
			Vector3 pos = new Vector3(juShiBasePos.x + juShiXStep * col,
			                          juShiBasePos.y + juShiYStep * row,
			                          juShiBasePos.z);
			GameObject go = InstantiateOneGO(juShiName, pos, scale, WarSceneController.manEulerAngles);
			listJuShi.Add(go);

			List<SolidersController> listSolidersAdded = new List<SolidersController>();

			SolidersController sCtrl = MagicController.Instance.warCtrl.GetRandomSolider(enemySide);
			if (sCtrl != null && !listSolidersAdded.Contains(sCtrl)) {
				listSolidersAdded.Add(sCtrl);

				pos = sCtrl.transform.localPosition;
				pos.z = 0;
				pos += juShiDownOff;
			} else {
				if (!hitGeneral && Random.Range(0, 100) < 40) {
					hitGeneral = true;
					GeneralController gCtrl;
					if (enemySide == WarSceneController.WhichSide.Left) {
						gCtrl = MagicController.Instance.warCtrl.leftGeneral;
					} else {
						gCtrl = MagicController.Instance.warCtrl.rightGeneral;
					}
					pos = gCtrl.transform.localPosition;
					pos.z = 0;
					pos += juShiDownOff;
				} else {
					pos = new Vector3(enemyCenterPoint.x + Random.Range(-150, 150), 
					                  enemyCenterPoint.y + Random.Range(-80, 80),
					                  -150);
				}
			}

			go = InstantiateOneGO(juShiDownName, pos, scale, WarSceneController.manEulerAngles);
			go.SetActive(false);
			go.GetComponent<MagicJuShiDown>().SetInfo(pos, pos - juShiDownOff, this);
			listJuShiDown.Add(go);
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

		if (side == WarSceneController.WhichSide.Left) {
			warCtrl.SetCameraMoveTo(cameraPosLeft);
		} else {
			warCtrl.SetCameraMoveTo(cameraPosRight);
		}

		Invoke("WaitForMoveToJuShi", 0.3f);
	}

	void WaitForMoveToJuShi() {

		StartCoroutine(ShowJuShiAnim());
		SoundController.Instance.PlaySound("00039");
	}

	IEnumerator ShowJuShiAnim() {

		int count = listJuShi.Count;
		int[] index = new int[count];
		for (int i=0; i<count; i++) {
			index[i] = i;
		}
		for (int i=0; i<count; i++) {
			int p = Random.Range(0, count);
			int q = Random.Range(0, count);
			int t = index[p];
			index[p] = index[q];
			index[q] = t;
		}

		for (int i=0; i<count; i++) {
			listJuShi[index[i]].GetComponent<exSpriteAnimation>().PlayDefault();
			yield return new WaitForSeconds(0.1f);
		}

		Invoke("WaitForShowJuShiDown", 0.3f);
	}

	void WaitForShowJuShiDown() {

		warCtrl.SetCameraMoveTo(new Vector3(enemyCenterPoint.x, -180, 0));
		StartCoroutine(ShowJuShiDown());
	}

	IEnumerator ShowJuShiDown() {

		yield return new WaitForSeconds(0.2f);

		for (int i=0; i<listJuShiDown.Count; i++) {
			listJuShiDown[i].SetActive(true);

			yield return new WaitForSeconds(0.1f);
		}

		Invoke("WaitForMagicOver", 1);
	}

	void WaitForMagicOver() {
		Destroy(gameObject);
		MagicController.Instance.OnMagicOver();
	}

	public void OnJuShiDown(Vector3 pos) {

		GameObject go = (GameObject)Instantiate(Resources.Load(juShiParticalName));
		go.transform.parent = transform;
		go.transform.localPosition = pos;

		go = (GameObject)Instantiate(Resources.Load(explodeName));
		go.transform.parent = transform;
		go.transform.localPosition = pos;

		Rect region = new Rect();
		region.x = pos.x - 50;
		region.y = pos.y - 50;
		region.width = 100;
		region.height = 100;

		warCtrl.OnMagicHitChecking(side, MagicManager.Instance.GetMagicDataInfo(38).ATTACK, region, true);
	}
}
