using UnityEngine;
using System.Collections;

public class CMSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CheatMakerController.Instance.ChangeState(CheatMakerController.State.MainMenu);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
