using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicChongChe : MonoBehaviour {

	public int level;

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private Vector3 enemyCenterPoint;

	private List<GameObject> listCar = new List<GameObject>();

	private Vector3 upSpace = new Vector3(0, 60, 0);
	private Vector3 downSpace = new Vector3(0, -60, 0);
	private Vector3 upOffsetLeft = new Vector3(-300, 100, 0);
	private Vector3 downOffsetLeft = new Vector3(-300, -100, 0);
	private Vector3 upOffsetRight = new Vector3(300, 100, 0);
	private Vector3 downOffsetRight = new Vector3(300, -100, 0);

	private string carName = "Magic/ChongChe/ChongChe";

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

		Vector3 upOffset;
		Vector3 downOffset;
		Vector3 scale;
		if (side == WarSceneController.WhichSide.Left) {
			upOffset = upOffsetLeft;
			downOffset = downOffsetLeft;
			scale = WarSceneController.manScaleLeft;
		} else {
			upOffset = upOffsetRight;
			downOffset = downOffsetRight;
			scale = WarSceneController.manScaleRight;
		}

		Vector3[] carPositionFrom = new Vector3[]{
			enemyCenterPoint + upOffset,
			enemyCenterPoint + downOffset,
			enemyCenterPoint + upOffset + upSpace,
			enemyCenterPoint + downOffset + downSpace
		};

		upOffset.x = -upOffset.x;
		downOffset.x = -downOffset.x;

		Vector3[] carPositionTo = new Vector3[]{
			enemyCenterPoint + upOffset,
			enemyCenterPoint + downOffset,
			enemyCenterPoint + upOffset + upSpace,
			enemyCenterPoint + downOffset + downSpace
		};

		for (int i=0; i<level * 2; i++) {
			GameObject go = InstantiateOneGO(carName, carPositionFrom[i], scale, WarSceneController.manEulerAngles);
			go.SetActive(false);
			go.GetComponent<MagicChongCheAct>().SetInfo(carPositionFrom[i], carPositionTo[i]);
			listCar.Add(go);
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

		warCtrl.SetCameraMoveTo(new Vector3(enemyCenterPoint.x, -180, 0));

		StartCoroutine(ShowCars());
	}

	IEnumerator ShowCars() {

		yield return new WaitForSeconds(0.3f);

		for (int i=0; i<listCar.Count; i++) {
			listCar[i].SetActive(true);

			yield return new WaitForSeconds(0.2f);
		}

		Invoke("WaitForMagicOver", 3);

		SoundController.Instance.PlaySound("00037");
	}

	void WaitForMagicOver() {
		Destroy(gameObject);
		MagicController.Instance.OnMagicOver();
	}
}
