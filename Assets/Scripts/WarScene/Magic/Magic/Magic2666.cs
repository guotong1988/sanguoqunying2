using UnityEngine;
using System.Collections;

public class Magic2666 : MonoBehaviour {
	
	private float yFrom = -50;
	private Vector3 scaleFrom = new Vector3(1, 1, 1);
	private Vector3 scaleTo = new Vector3(0.01f, 0.01f, 1);

	private int healthyOrg;
	private int healthyFinally;
	private float timeTick;

	private GeneralInfo gInfo;

	private ParticleSystem parts;

	// Use this for initialization
	void Start () {

		//GameObject camGO = GameObject.FindWithTag("CameraMan");
		GameObject camGO = GameObject.FindWithTag("MainCamera");
		transform.parent = camGO.transform;
		transform.localPosition = new Vector3(0, yFrom, -20);

		transform.localScale = scaleFrom;

		parts = GetComponent<ParticleSystem>();

		int healthyAdd = MagicController.Instance.GetMagicAttack();
		if (MagicController.Instance.GetMagicSide() == WarSceneController.WhichSide.Left) {
			gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.leftGeneralIdx);
		} else {
			gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);
		}
		healthyOrg = gInfo.healthCur;
		healthyFinally = gInfo.healthCur + healthyAdd;
		if (healthyFinally > gInfo.healthMax)
			healthyFinally = gInfo.healthMax;

		SoundController.Instance.PlaySound("00012");
	}
	
	// Update is called once per frame
	void Update () {

		timeTick += Time.deltaTime;
		if (timeTick < 0.5f) {
			transform.localPosition = new Vector3(0, transform.localPosition.y + Time.deltaTime * 80, transform.localPosition.z);
		} else if (timeTick < 1f) {
			transform.localPosition = new Vector3(0, transform.localPosition.y + Time.deltaTime * 150, transform.localPosition.z);
			transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, (timeTick - 0.5f) * 2);

			parts.emissionRate = Mathf.Lerp(300, 100, (timeTick - 0.5f) * 2);
		} else if (timeTick < 1.1f) {
			parts.emissionRate = 50;
			transform.localPosition = new Vector3(0, transform.localPosition.y + Time.deltaTime * 100, transform.localPosition.z);
		} else if (timeTick < 1.3f) {
			parts.emissionRate = 5;
		} else {
			OnShowOver();
		}

		if (timeTick < 1) {
			gInfo.healthCur = (int)Mathf.Lerp(healthyOrg, healthyFinally, timeTick);
		} else {
			gInfo.healthCur = healthyFinally;
		}
	}

	void OnShowOver() {
		MagicController.Instance.OnMagicOver();
	}
}
