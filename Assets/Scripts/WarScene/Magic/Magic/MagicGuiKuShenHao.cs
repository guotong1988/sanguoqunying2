using UnityEngine;
using System.Collections;

public class MagicGuiKuShenHao : MonoBehaviour {

	private GeneralController general;
	private GameObject fenShenZhanGO;

	private string[] group = new string[]{"2604", "2622", "2646"};

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

		GameObject.Instantiate(Resources.Load("Magic/" + group[0]));
		GameObject.Instantiate(Resources.Load("Magic/" + group[1]));

		Invoke("SetMagic", 1f);
	}

	void SetMagic() {

		if (MagicController.Instance.GetMagicSide() == WarSceneController.WhichSide.Left) {
			general = MagicController.Instance.warCtrl.leftGeneral;
		} else {
			general = MagicController.Instance.warCtrl.rightGeneral;
		}

		Vector3 pos = general.transform.localPosition;
		pos.y = -30;
		MagicController.Instance.warCtrl.SetCameraMoveTo(pos);

		Invoke("WaitForCameraMoveToGeneral", 0.5f);
	}

	void WaitForCameraMoveToGeneral() {

		fenShenZhanGO = (GameObject)GameObject.Instantiate(Resources.Load("Magic/" + group[2]));
		general.SetOnMagic(5);

		Invoke("WaitForArrowOut", 0.3f);
	}

	void WaitForArrowOut() {
		fenShenZhanGO.SendMessage("WaitForArrowOut", SendMessageOptions.DontRequireReceiver);
	}
}
