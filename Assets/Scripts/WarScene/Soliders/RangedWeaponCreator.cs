using UnityEngine;
using System.Collections;

public class RangedWeaponCreator : MonoBehaviour {
	
	public GameObject[] rangedWeaponPrefab;
	
	private WarSceneController warCtrl;
	
	void Start() {
		warCtrl = GameObject.FindWithTag("GameController").GetComponent<WarSceneController>();
	}
	
	public void SetRangedWeapon(WarSceneController.WhichSide s, WarSceneController.Direction d, int t, Vector3 pos) {
		
		int idx = t - 3;
		GameObject go = (GameObject)Instantiate(rangedWeaponPrefab[idx]);
		RangedWeapon rw = go.AddComponent<RangedWeapon>();
		rw.Init(s, d, t, pos, transform, warCtrl);
	}
}
