using UnityEngine;
using System.Collections;

public class MagicFireDown : MonoBehaviour {
	
	private Magic2617 mCtrl;

	private float speed = 500;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.localPosition;
		pos.z += speed * Time.deltaTime;
		transform.localPosition = pos;

		if (pos.z >= 0) {
			mCtrl.OnFireDownOver(pos);
			Destroy(gameObject);
		}
	}

	public void SetInfo(Vector3 downPos, Magic2617 mCtrl) {

		downPos.z = -150;
		transform.localPosition = downPos;

		this.mCtrl = mCtrl;
	}
}
