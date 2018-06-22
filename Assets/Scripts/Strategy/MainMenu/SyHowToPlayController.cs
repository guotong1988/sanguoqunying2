using UnityEngine;
using System.Collections;

public class SyHowToPlayController : MonoBehaviour {
	
	public StrategyController strCtrl;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Misc.GetBack()) {
			gameObject.SetActive(false);
			
			strCtrl.ReturnMainMode();
		}
	}
}
