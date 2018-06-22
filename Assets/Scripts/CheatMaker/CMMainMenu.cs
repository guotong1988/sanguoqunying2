using UnityEngine;
using System.Collections;

public class CMMainMenu : MonoBehaviour {

	public GameObject btn_king;
	public GameObject btn_city;
	public GameObject btn_general;

	public TweenPosition tweenPos;

	private CheatMakerController.State state;

	// Use this for initialization
	void Start () {

		OnEnter();

		UIEventListener.Get(btn_king).onClick += OnModifyKingClick;
		UIEventListener.Get(btn_city).onClick += OnModifyCitiesClick;
		UIEventListener.Get(btn_general).onClick += OnModifyGeneralsClick;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnModifyKingClick(GameObject go) {

		SetMenuUp();
		state = CheatMakerController.State.KingMode;
	}

	void OnModifyCitiesClick(GameObject go) {

		SetMenuUp();
		state = CheatMakerController.State.CityMode;
	}

	void OnModifyGeneralsClick(GameObject go) {

		SetMenuUp();
		state = CheatMakerController.State.GeneralMode;
	}

	public void SetButtonEnable() {
		btn_king.GetComponent<BoxCollider>().enabled = true;
		btn_city.GetComponent<BoxCollider>().enabled = true;
		btn_general.GetComponent<BoxCollider>().enabled = true;
	}

	public void SetButtonDisable() {
		btn_king.GetComponent<BoxCollider>().enabled = false;
		btn_city.GetComponent<BoxCollider>().enabled = false;
		btn_general.GetComponent<BoxCollider>().enabled = false;
	}

	public void SetMenuUp() {
		tweenPos.Play(false);
		SetButtonDisable();
	}

	public void OnTweenPositionOver() {
		if (tweenPos.transform.localPosition == tweenPos.to) {
			SetButtonEnable();
		} else {
			CheatMakerController.Instance.ChangeState(state);
		}
	}

	void OnEnter() {
		tweenPos.Play(true);
	}

	void OnReturn() {
		SetMenuUp();
		state = CheatMakerController.State.NULL;
	}
}
