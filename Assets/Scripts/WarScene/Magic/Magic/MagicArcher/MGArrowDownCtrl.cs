using UnityEngine;
using System.Collections;

public class MGArrowDownCtrl : MonoBehaviour {

	private bool isWithFire;
	private WarSceneController.WhichSide side;
	private SolidersController soliderCtrl;
	private GeneralController generalCtrl;

	private float timeTick;
	
	private Vector3 speedLeft = new Vector3(300, 0, 300);
	private Vector3 speedRight = new Vector3(-300, 0, 300);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeTick += Time.deltaTime;
		if (timeTick < 0.5f) {
			if (side == WarSceneController.WhichSide.Left) {
				this.transform.localPosition += speedLeft * Time.deltaTime;
			} else {
				this.transform.localPosition += speedRight * Time.deltaTime;
			}
		} else {
			if (soliderCtrl != null) {

				soliderCtrl.OnDamage(-1, (WarSceneController.Direction)Random.Range(0, 2), isWithFire);
			} else if (generalCtrl != null) {
				int damage;
				if (!isWithFire) {
					damage = MagicManager.Instance.GetMagicDataInfo(1).ATTACK;
				} else {
					damage = MagicManager.Instance.GetMagicDataInfo(27).ATTACK;
				}
				generalCtrl.OnDamage(damage, -1, isWithFire);
			}

			Destroy(this.gameObject);
		}
	}

	public void SetInfo(bool isWithFire, WarSceneController.WhichSide side, Vector3 positionStart, SolidersController soliderCtrl, GeneralController generalCtrl) {
		this.isWithFire = isWithFire;
		this.side = side;
		this.soliderCtrl = soliderCtrl;
		this.generalCtrl = generalCtrl;

		this.transform.localPosition = positionStart;
	}
}
