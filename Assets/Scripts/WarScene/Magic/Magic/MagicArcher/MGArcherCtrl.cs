using UnityEngine;
using System.Collections;

public class MGArcherCtrl : MonoBehaviour {

	private int index;
	private bool isWithFire;
	private MagicArcher archerCtrl;

	private bool isArrowOut = false;
	private exSpriteAnimation anim;
	private Transform fire;
	
	private Vector3[] firePos = new Vector3[]{
		new Vector3(-30, 20, -2),
		new Vector3(-45, 45, -2),
		new Vector3(-32, 80, -2)
	};

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (anim.IsPlaying()) {
			if (anim.GetCurFrameIndex() < 3) {
				if (isWithFire) {
					fire.localPosition = firePos[anim.GetCurFrameIndex()];
				}
			} else {
				if (!isArrowOut) {
					isArrowOut = true;
					archerCtrl.ShowArrowUp(index);

					if (isWithFire) {
						fire.gameObject.SetActive(false);
					}

					SoundController.Instance.PlaySound("00001");
				}
			}
		}
	}

	public void SetInfo(int index, bool isWithFire, float waitTime, MagicArcher archerCtrl) {

		this.index = index;
		this.isWithFire = isWithFire;
		this.archerCtrl = archerCtrl;

		if (isWithFire) {
			fire = this.transform.GetChild(0);
			fire.localPosition = firePos[0];
		}

		anim = GetComponent<exSpriteAnimation>();
		Invoke("WaitForPlayAnim", waitTime);
	}

	void WaitForPlayAnim() {
		anim.PlayDefault();
	}
}
