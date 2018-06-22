using UnityEngine;
using System.Collections;

public class MGArrowUpCtrl : MonoBehaviour {

	private WarSceneController.WhichSide side;

	private float timeTick;

	private Vector3 speedLeft = new Vector3(300, 0, -300);
	private Vector3 speedRight = new Vector3(-300, 0, -300);

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
			Destroy(this.gameObject);
		}
	}

	public void SetInfo(WarSceneController.WhichSide side) {
		this.side = side;
	}
}
