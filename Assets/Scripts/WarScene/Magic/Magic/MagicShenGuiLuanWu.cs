using UnityEngine;
using System.Collections;

public class MagicShenGuiLuanWu : MonoBehaviour {

	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private GeneralController general;
	private GeneralController enemyGeneral;

	private int step;
	private Vector3 scale;

	private float speed;
	private float speedLeft = 50;
	private float speedRight = -50;

	private Vector3 posOffset;
	private Vector3 posOffsetLeft = new Vector3(30, -5, -30);
	private Vector3 posOffsetRight = new Vector3(-30, -5, -30);

	private string shenGuiLuanWuName1 = "Magic/ShenGuiLuanWu/ShenGuiLuanWu1";
	private string shenGuiLuanWuName2 = "Magic/ShenGuiLuanWu/ShenGuiLuanWu2";
	private string shenGuiLuanWuName3 = "Magic/ShenGuiLuanWu/ShenGuiLuanWu3";
	private string shenGuiLuanWuName4 = "Magic/ShenGuiLuanWu/ShenGuiLuanWu4";
	private string explodeName = "Magic/Misc/Explode";

	// Use this for initialization
	void Start () {

		Init();
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
			general = warCtrl.leftGeneral;
			enemyGeneral = warCtrl.rightGeneral;
			scale = WarSceneController.manScaleLeft;
			speed = speedLeft;
			posOffset = posOffsetLeft;
		} else {
			general = warCtrl.rightGeneral;
			enemyGeneral = warCtrl.leftGeneral;
			scale = WarSceneController.manScaleRight;
			speed = speedRight;
			posOffset = posOffsetRight;
		}
	}

	void WaitForArrowOut() {

		Vector3 pos = general.transform.localPosition + posOffset;
		string shenGuiLuanWuName = "";

		switch (step) {
		case 0:
			pos -= posOffset;
			shenGuiLuanWuName = shenGuiLuanWuName1;
			if (Mathf.Abs(general.transform.localPosition.x - enemyGeneral.transform.localPosition.x) < 200) {
				general.SetOnMagic(5);

				enemyGeneral.OnDamage(20, -1, false);
			} else {
				Invoke("WaitForMagicOver", 0.5f);
			}
			break;
		case 1:
			shenGuiLuanWuName = shenGuiLuanWuName2;
			general.SetOnMagic(4);
			enemyGeneral.OnDamage(10, -1, false);
			break;
		case 2:
			shenGuiLuanWuName = shenGuiLuanWuName3;
			general.SetOnMagic(5);
			enemyGeneral.OnDamage(10, -1, false);
			break;
		case 3:
			shenGuiLuanWuName = shenGuiLuanWuName2;
			general.SetOnMagic(4);
			enemyGeneral.OnDamage(10, -1, false);
			break;
		case 4:
			shenGuiLuanWuName = shenGuiLuanWuName4;

			Invoke("WaitForMagicOver", 0.5f);
			break;
		}

		GameObject go = InstantiateOneGO(shenGuiLuanWuName, pos, scale, WarSceneController.manEulerAngles);
		go.GetComponent<MagicShenGuiLuanWuAct>().SetInfo(speed);

		InstantiateOneGO(explodeName, enemyGeneral.transform.localPosition, Vector3.one, Vector3.zero);

		step++;
	}

	GameObject InstantiateOneGO(string goName, Vector3 pos, Vector3 scale, Vector3 eulerAngles) {
		
		GameObject go = (GameObject)Instantiate(Resources.Load(goName));
		go.transform.parent = this.transform;
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.transform.eulerAngles = eulerAngles;
		
		return go;
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
