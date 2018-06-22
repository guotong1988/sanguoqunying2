using UnityEngine;
using System.Collections;

public class MagicChongCheAct : MonoBehaviour {

	private WarSceneController warCtrl;
	
	private Vector3 fromPosition;
	private Vector3 toPosition;

	private float timeTick;

	// Use this for initialization
	void Start () {
		warCtrl = MagicController.Instance.warCtrl;
	}
	
	// Update is called once per frame
	void Update () {

		timeTick += Time.deltaTime;
		if (timeTick < 3) {
			transform.localPosition = Vector3.Lerp(fromPosition, toPosition, timeTick / 3);

			Rect rect = new Rect();
			rect.x = transform.localPosition.x - 30;
			rect.y = transform.localPosition.y - 30;
			rect.width = 60;
			rect.height = 60;
			warCtrl.OnMagicHitChecking(MagicController.Instance.GetMagicSide(), 0, rect, false);
		} else {
			Destroy(gameObject);
		}
	}

	public void SetInfo(Vector3 fromPosition, Vector3 toPosition) {

		this.fromPosition = fromPosition;
		this.toPosition = toPosition;
	}
}
