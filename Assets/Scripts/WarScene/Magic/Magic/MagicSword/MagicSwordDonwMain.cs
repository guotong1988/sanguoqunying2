using UnityEngine;
using System.Collections;

public class MagicSwordDonwMain : MonoBehaviour {

	private MagicSword mCtrl;
	private exSprite sprite;

	private int state;
	private float timeTick;
	private float speed = 500;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<exSprite>();
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0) {
			Vector3 pos = transform.localPosition;
			pos.z += speed * Time.deltaTime;
			transform.localPosition = pos;

			if (pos.z >= 0) {
				pos.z = 0;
				state = 1;
				mCtrl.OnSwordDownOver(pos);
			}
		} else {
			timeTick += Time.deltaTime;
			if (timeTick >= 0.5f) {
				Destroy(gameObject);
			} else {
				sprite.color = new Color(1, 1, 1, 1 - timeTick * 2);
			}
		}
	}

	public void SetInfo(MagicSword mCtrl) {
		this.mCtrl = mCtrl;
	}
}
