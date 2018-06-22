using UnityEngine;
using System.Collections;

public class MagicGuiJi : MonoBehaviour {

	private int state;
	
	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	
	private GeneralController general;
	private GeneralController enemyGeneral;
	
	private GameObject arrow;
	
	private float speed;
	private float speedLeft = 500;
	private float speedRight = -500;
	
	private string arrowName = "Magic/GuiJi/GuiJi";
	private string explodeName = "Magic/Misc/Explode";
	
	// Use this for initialization
	void Start () {
		
		Init();
		GenerateArrow();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (state == 1) {
			Vector3 pos = arrow.transform.localPosition;
			pos.x += speed * Time.deltaTime;
			arrow.transform.localPosition = pos;
			
			if ((side == WarSceneController.WhichSide.Left && pos.x > enemyGeneral.transform.localPosition.x)
			    || (side == WarSceneController.WhichSide.Right && pos.x < enemyGeneral.transform.localPosition.x)) {
				
				state = 2;
				Invoke("WaitForMagicOver", 1);

				Rect region = new Rect();
				if (side == WarSceneController.WhichSide.Left) {
					region.x = pos.x - 20;
				} else {
					region.x = pos.x - 180;
				}
				region.y = -15;
				region.width = 200;
				region.height = 30;
				warCtrl.OnMagicHitChecking(side, MagicController.Instance.GetMagicAttack(), region, false);

				for (int i=0; i<5; i++) {
					GameObject go = (GameObject)Instantiate(Resources.Load(explodeName));
					go.transform.parent = transform;
					pos.x += Random.Range(-20, 20);
					pos.z += Random.Range(-20, 20);
					go.transform.localPosition = pos;
				}
			} else {
				warCtrl.SetCameraPosition(new Vector3(pos.x, -30, 0));

				Rect region = new Rect();
				region.x = pos.x;
				region.y = -15;
				region.width = 20;
				region.height = 30;
				warCtrl.OnMagicHitChecking(side, 0, region, false);
			}
		} else if (state == 2) {
			Vector3 pos = arrow.transform.localPosition;
			pos.x += speed * Time.deltaTime;
			arrow.transform.localPosition = pos;
		}
	}
	
	void Init() {

		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;

		warCtrl = MagicController.Instance.warCtrl;
		side = MagicController.Instance.GetMagicSide();
		if (side == WarSceneController.WhichSide.Left) {
			speed = speedLeft;
			general = MagicController.Instance.warCtrl.leftGeneral;
			enemyGeneral = MagicController.Instance.warCtrl.rightGeneral;
		} else {
			speed = speedRight;
			general = MagicController.Instance.warCtrl.rightGeneral;
			enemyGeneral = MagicController.Instance.warCtrl.leftGeneral;
		}
	}
	
	void GenerateArrow() {
		
		arrow = (GameObject)Instantiate(Resources.Load(arrowName));
		arrow.transform.parent = transform;
		
		float off = 0;
		Vector3 scale = Vector3.zero;
		if (side == WarSceneController.WhichSide.Left) {
			off = 30;
			scale = WarSceneController.manScaleLeft;
			
		} else {
			off = -30;
			scale = WarSceneController.manScaleRight;
		}
		arrow.transform.localPosition = new Vector3(general.transform.localPosition.x + off,
		                                            general.transform.localPosition.y,
		                                            -40);
		arrow.transform.localEulerAngles = WarSceneController.manEulerAngles;
		arrow.transform.localScale = scale;
		arrow.SetActive(false);
	}
	
	void WaitForArrowOut() {
		state = 1;
		arrow.SetActive(true);

		SoundController.Instance.PlaySound("00028");
	}
	
	void WaitForMagicOver() {
		MagicController.Instance.OnMagicOver();
	}
}
