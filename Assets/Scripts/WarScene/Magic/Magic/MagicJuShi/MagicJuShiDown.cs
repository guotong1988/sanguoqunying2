using UnityEngine;
using System.Collections;

public class MagicJuShiDown : MonoBehaviour {

	MagicJuShi magicCtrl;

	Vector3 fromPosition;
	Vector3 toPosition;

	private float timeTick;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		timeTick += Time.deltaTime;
		if (timeTick < 0.25f) {
			transform.localPosition = Vector3.Lerp(fromPosition, toPosition, timeTick * 4);
		} else {
			magicCtrl.OnJuShiDown(toPosition);
			Destroy(gameObject);
		}
	}

	public void SetInfo(Vector3 fromPosition, Vector3 toPosition, MagicJuShi magicCtrl) {

		this.magicCtrl = magicCtrl;
		this.fromPosition = fromPosition;
		this.toPosition = toPosition;
	}
}
