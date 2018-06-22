using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheatMakerController {

	public enum State {
		NULL,
		MainMenu,
		KingMode,
		CityMode,
		GeneralMode
	}

	private State state = State.NULL;

	public int recordIndex;

	public GameObject buttonSave;
	public GameObject buttonReturn;

	private GameObject mainMenu;
	private GameObject kingPanel;
	private GameObject cityPanel;
	private GameObject generalPanel;
	
	private GameObject panelsRoot;
	private GameObject buttonsRoot;

	private static CheatMakerController mInstance;
	public static CheatMakerController Instance {
		get {
			if (mInstance == null) {
				mInstance = new CheatMakerController();
			}
			return mInstance;
		}
	}

	public void ChangeState(State newState) {

		switch (state) {
		case State.MainMenu:
			mainMenu.SetActive(false);
			break;
		case State.KingMode:
			kingPanel.SetActive(false);
			break;
		case State.CityMode:
			cityPanel.SetActive(false);
			break;
		case State.GeneralMode:
			generalPanel.SetActive(false);
			break;
		}

		switch (newState) {
		case State.NULL:
			Application.LoadLevel(0);
			GameObject.Destroy(GameObject.Find("MouseTrack"));
			break;
		case State.MainMenu:
			if (mainMenu == null) {
				mainMenu = GetPanel("CheatMaker/MainMenu");
			}
			mainMenu.SetActive(true);
			mainMenu.SendMessage("OnEnter");
			break;
		case State.KingMode:
			if (kingPanel == null) {
				kingPanel = GetPanel("CheatMaker/KingPanel");
			}
			kingPanel.SetActive(true);
			kingPanel.SendMessage("OnEnter");
			break;
		case State.CityMode:
			if (cityPanel == null) {
				cityPanel = GetPanel("CheatMaker/CityPanel");
			}
			cityPanel.SetActive(true);
			cityPanel.SendMessage("OnEnter");
			break;
		case State.GeneralMode:
			if (generalPanel == null) {
				generalPanel = GetPanel("CheatMaker/GeneralPanel");
			}
			generalPanel.SetActive(true);
			generalPanel.SendMessage("OnEnter");
			break;
		}

		state = newState;

		if (buttonReturn == null) {
			buttonReturn = GetButton("CheatMaker/ButtonReturn", new Vector3(258, -210, 0));
			UIEventListener.Get(buttonReturn).onClick += OnButtonReturn;

			buttonSave = GetButton("CheatMaker/ButtonSave", new Vector3(158, -210, 0));
			UIEventListener.Get(buttonSave).onClick += OnButtonSave;

		}
		buttonReturn.GetComponent<BoxCollider>().enabled = true;
	}

	GameObject GetPanel(string name) {

		if (panelsRoot == null) {
			panelsRoot = GameObject.Find("UI Root/Camera/Panels");
		}

		GameObject go = GameObject.Instantiate(Resources.Load(name)) as GameObject;
		go.transform.parent = panelsRoot.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;

		return go;
	}

	GameObject GetButton(string name, Vector3 position) {

		if (buttonsRoot == null) {
			buttonsRoot = GameObject.Find("UI Root/Camera/Buttons");
		}

		GameObject go = GameObject.Instantiate(Resources.Load(name)) as GameObject;
		go.transform.parent = buttonsRoot.transform;
		go.transform.localPosition = position;
		go.transform.localScale = Vector3.one;
		
		return go;
	}

	void SaveOver() {
		buttonSave.GetComponent<BoxCollider>().enabled = true;

		GetTips(ZhongWen.Instance.cheatSaveOver);
	}

	void OnButtonSave(GameObject go) {

		buttonSave.GetComponent<BoxCollider>().enabled = false;
		RecordController.Instance.SaveRecord(recordIndex, SaveOver);
	}

	void OnButtonReturn(GameObject go) {
		switch (state) {
		case State.MainMenu:
			mainMenu.SendMessage("OnReturn");
			break;
		case State.KingMode:
			kingPanel.SendMessage("OnReturn");
			break;
		case State.CityMode:
			cityPanel.SendMessage("OnReturn");
			break;
		case State.GeneralMode:
			generalPanel.SendMessage("OnReturn");
			break;
		}

		buttonReturn.GetComponent<BoxCollider>().enabled = false;
	}

	public GameObject GetPopupList(List<string> items, CMPopupList.OnSelectChangeCallBack itemSelectedCallBack) {

		GameObject go = GameObject.Instantiate(Resources.Load("CheatMaker/PopupList")) as GameObject;
		go.GetComponent<CMPopupList>().PopupListInit(items, itemSelectedCallBack);

		return go;
	}

	public GameObject GetTips(string message) {

		GameObject go = GameObject.Instantiate(Resources.Load("CheatMaker/Tips")) as GameObject;
		go.transform.parent = UICamera.mainCamera.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;

		go.transform.GetChild(0).GetComponent<UILabel>().text = message;

		GameObject.Destroy(go, 2f);
		return go;
	}
}
