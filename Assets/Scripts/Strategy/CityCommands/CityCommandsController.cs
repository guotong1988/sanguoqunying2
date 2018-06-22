using UnityEngine;
using System.Collections;

public class CityCommandsController : MonoBehaviour {
	
	public StrategyController 			strCtrl;
	
	public MenuDisplayAnim 				cityCommands;
	public CCInformationController 		infoCtrl;
	public DialogueController 			kingDialog;
	
	public ExpeditionController 		expedition;
	public ConscriptionController 		conscription;
	public CCGeneralsInfoController 	generalsInfo;
	public AppointedPrefectController 	prefectAppointed;
	
	public Button[] commands;
	
	private int state = 0;
	private int cityIdx;
	private int commandIdx = -1;
	
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		for (int i=0; i<commands.Length; i++) {
			
			commands[i].SetButtonData(i);
			commands[i].SetButtonClickHandler(OnCommandsButtonClickHandler);
		}
	}
	
	void OnEnable() {

		state = 1000;
		commandIdx = -1000;
		timeTick = 0;
		
		kingDialog.gameObject.SetActive(false);
		
		cityCommands.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		infoCtrl.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
		
		infoCtrl.SetCity(cityIdx);
	}
	
	// Update is called once per frame
	void Update () {

		if (StrategyController.state != StrategyController.State.Pause) {
			gameObject.SetActive(false);
			return;
		}

		switch (state) {
		case 0:
			OnNormalModeHandler();
			break;
		case 1:
			OnChangingToCommandModeHandler();
			break;
		case 2:
			OnAppointedPrefectModeController();
			break;
		case 1000:
			OnBeginHandler();
			break;
		}
	}
	
	void OnNormalModeHandler() {
		if (Misc.GetBack()) {
			state = 1;
			commandIdx = -1;
			
			cityCommands.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			infoCtrl.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		}
	}
	
	void OnChangingToCommandModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			switch (commandIdx) {
			case -1:
				strCtrl.ReturnMainMode();
				break;
			case 0:
				expedition.SetCity(cityIdx);
				break;
			case 1:
				conscription.SetCity(cityIdx);
				break;
			case 2:
				generalsInfo.AddGeneralsList(cityIdx);
				break;
			case 3:
				generalsInfo.AddPrisonsList(cityIdx);
				break;
			case 4:
				prefectAppointed.AddGeneralsList(cityIdx);
				break;
			}
		}
	}
	
	void OnAppointedPrefectModeController() {
		
		if (!kingDialog.IsShowingText() && Input.GetMouseButtonUp(0)) {
			
			state = 0;
			
			for (int i=0; i<commands.Length; i++) {
				
				commands[i].enabled = true;
			}
			
			Input.ResetInputAxes();
			kingDialog.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
		}
	}
	
	void OnCommandsButtonClickHandler(object data) {
		
		commandIdx = (int)data;
		state = 1;
		
		cityCommands.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		infoCtrl.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
	}

	void OnBeginHandler() {
		if (!cityCommands.GetComponent<MenuDisplayAnim>().IsPlaying()) {
			state = 0;
		}
	}

	public void SetCity(int idx) {
		
		cityIdx = idx;
		
		gameObject.SetActive(true);
		
		if (Informations.Instance.GetCityInfo(idx).king == Controller.kingIndex) {
			for (int i=0; i<commands.Length; i++) {
				commands[i].SetButtonEnable(true);
			}
			
			if (Informations.Instance.GetCityInfo(idx).prisons.Count > 0) {
				commands[3].SetButtonEnable(true);
			} else {
				commands[3].SetButtonEnable(false);
			}
			
			KingInfo kInfo = Informations.Instance.GetKingInfo(Informations.Instance.GetCityInfo(idx).king);
			int kingCityIdx = Informations.Instance.GetGeneralInfo(kInfo.generalIdx).city;
			if (kingCityIdx != idx) {
				commands[4].SetButtonEnable(true);
			} else {
				commands[4].SetButtonEnable(false);
			}
		} else {
			for (int i=0; i<commands.Length; i++) {
				commands[i].SetButtonEnable(false);
			}
		}
	}
	
	public void OnAppointedPrefectMode() {
		
		state = 2;
		
		gameObject.SetActive(true);
		
		for (int i=0; i<commands.Length; i++) {
			
			commands[i].enabled = false;
		}
		
		string str = ZhongWen.Instance.ming + ZhongWen.Instance.GetGeneralName(Informations.Instance.GetCityInfo(cityIdx).prefect) + 
						ZhongWen.Instance.wei + ZhongWen.Instance.GetCityName(cityIdx) + ZhongWen.Instance.taishou;
			
		kingDialog.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx,
			str, MenuDisplayAnim.AnimType.InsertFromBottom);
	}
}
