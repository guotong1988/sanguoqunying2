using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicFireDragon : MonoBehaviour {

	public int level;

	private WarSceneController warCtrl;
	
	private Vector3 centerPoint;
	private WarSceneController.WhichSide side;

	private List<GameObject> listBody = new List<GameObject>();

	private float speedLeft = 100;
	private float speedRight = -100;

	private float ySpace = 30;

	private string bodyName1 = "Magic/FireDragon/FireDragon1";
	private string bodyName2 = "Magic/FireDragon/FireDragon2";
	private string bodyName3 = "Magic/FireDragon/FireDragon3";

	// Use this for initialization
	void Start () {

		Init();
		GenerateBody();
		Invoke("WaitForCameraMoveToGeneral", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Init() {

		warCtrl = MagicController.Instance.warCtrl;
		centerPoint = warCtrl.GetArmyCentrePoint(MagicController.Instance.GetMagicEnemySide());

		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = centerPoint;
		side = MagicController.Instance.GetMagicSide();
		if (side == WarSceneController.WhichSide.Left) {
			transform.localPosition = new Vector3(transform.localPosition.x - 100,
			                                      transform.localPosition.y,
			                                      transform.localPosition.z);
		} else {
			transform.localPosition = new Vector3(transform.localPosition.x + 100,
			                                      transform.localPosition.y,
			                                      transform.localPosition.z);
		}
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	void GenerateBody() {

		float yPos = ySpace * (level - 1);
		float speed = 0;
		if (side == WarSceneController.WhichSide.Left) {
			speed = this.speedLeft;
		} else {
			speed = this.speedRight;
		}
		for (int i=0; i<level; i++) {
			string bodyName = this.bodyName1;
			float disappearTime = 2;
			LoadOneBody(bodyName, yPos, speed, disappearTime);

			bodyName = this.bodyName2;
			for (int j=0; j<8; j++) {
				disappearTime -= 0.15f;
				LoadOneBody(bodyName, yPos, speed, disappearTime);
			}

			bodyName = this.bodyName3;
			disappearTime -= 0.15f;
			LoadOneBody(bodyName, yPos, speed, disappearTime);

			yPos -= 2 * ySpace;
		}
	}

	void LoadOneBody(string bodyName, float yPos, float speed, float disappearTime) {
		GameObject go = (GameObject)Instantiate(Resources.Load(bodyName));
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(0, yPos, -30);
		go.transform.localEulerAngles = WarSceneController.manEulerAngles;

		if (side == WarSceneController.WhichSide.Left) {
			go.transform.localScale = WarSceneController.manScaleLeft;
		} else {
			go.transform.localScale = WarSceneController.manScaleRight;
		}

		go.GetComponent<MagicFireDragonBody>().SetInfo(speed, disappearTime);

		go.SetActive(false);
		listBody.Add(go);
	}

	void WaitForCameraMoveToGeneral() {

		warCtrl.SetCameraMoveTo(new Vector3(centerPoint.x, -90, 0));

		Invoke("WaitForShowDragon", 0.3f);
	}

	void WaitForShowDragon() {

		StartCoroutine(ShowDragon());
		SoundController.Instance.PlaySound("00056");
	}

	IEnumerator ShowDragon() {

		for (int i=0; i<10; i++) {
			Rect region = new Rect();
			if (side == WarSceneController.WhichSide.Left) {
				region.x = centerPoint.x - 100 + i * 20;
			} else {
				region.x = centerPoint.x + 100 - i * 20;
			}
			region.y = -level * ySpace;
			region.width = 20;
			region.height = 2 * level * ySpace;

			for (int j=0; j<level; j++) {
				listBody[j * 10 + i].SetActive(true);

				warCtrl.OnMagicHitChecking(MagicController.Instance.GetMagicSide(), 
				                           MagicController.Instance.GetMagicAttack(),
				                           region,
				                           true);
			}

			yield return new WaitForSeconds(0.2f);
		}

		Invoke("WaitForMagicOver", 1);
	}

	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
