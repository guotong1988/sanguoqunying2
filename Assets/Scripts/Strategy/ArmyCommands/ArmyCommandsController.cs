using UnityEngine;
using System.Collections;

public class ArmyCommandsController : MonoBehaviour {
	
	public StrategyController 			strCtrl;
	public MyPathfinding				path;
	public SelectTargetCity				selectTargetCity;
	
	public MenuDisplayAnim 				armyCommands;
	public ACInformationController		infoCtrl;
	public CCGeneralsInfoController 	generalsInfo;
	
	public Button[] commands;
	
	private int state = 0;
	private ArmyInfo armyInfo;
	private int commandIdx = -1;
	private int cityCanIntoIdx = -1;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
	
		for (int i=0; i<commands.Length; i++) {
			
			commands[i].SetButtonData(i);
			commands[i].SetButtonClickHandler(OnCommandsButtonClickHandler);
		}
	}
	
	void OnEnable() {
		
		commandIdx = -1;
		
		armyCommands.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		infoCtrl.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
	}
	
	// Update is called once per frame
	void Update () {
	
		switch (state) {
		case 0:
			OnNormalModeHandler();
			break;
		case 1:
			OnChangingToCommandModeHandler();
			break;
		}
	}
	
	void OnNormalModeHandler() {
		if (Misc.GetBack()) {
			state = 1;
			commandIdx = -1;
			
			armyCommands.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			infoCtrl.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		}
	}
	
	void OnChangingToCommandModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			state = 0;
			
			gameObject.SetActive(false);
			
			switch (commandIdx) {
			case -1:
				strCtrl.ReturnMainMode();
				break;
			case 0:
				generalsInfo.AddArmyGeneralsList(armyInfo);
				break;
			case 1:
				selectTargetCity.SetArmy(armyInfo);
				break;
			case 2:
				armyInfo.armyCtrl.SetArmyGarrison();
				strCtrl.ReturnMainMode();
				break;
			case 3:
				generalsInfo.AddArmyPrisonsList(armyInfo);
				break;
			case 4:
				OnIntoCity();
				break;
			}
		}
	}
	
	void OnCommandsButtonClickHandler(object data) {
		
		commandIdx = (int)data;
		state = 1;
		
		armyCommands.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		infoCtrl.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
	}
	
	void OnIntoCity() {
		
		armyInfo.armyCtrl.IntoTheCity(cityCanIntoIdx);
		strCtrl.ReturnMainMode();
	}
	
	public void SetArmyInfo(ArmyInfo a) {
		
		armyInfo = a;
		
		gameObject.SetActive(true);
		
		infoCtrl.SetArmy(armyInfo);
		
		if (armyInfo.king == Controller.kingIndex) {
			for (int i=0; i<commands.Length; i++) {
				commands[i].SetButtonEnable(true);
			}
			
			if (armyInfo.prisons.Count > 0) {
				commands[3].SetButtonEnable(true);
			} else {
				commands[3].SetButtonEnable(false);
			}
			
			Vector3 pos = armyInfo.armyCtrl.transform.position;
			cityCanIntoIdx = path.GetCityIndex(pos, 30);
			if (cityCanIntoIdx != -1) {
				if (Informations.Instance.GetCityInfo(cityCanIntoIdx).king == armyInfo.king) {
					commands[4].SetButtonEnable(true);
				} else {
					commands[4].SetButtonEnable(false);
				}
			} else {
				commands[4].SetButtonEnable(false);
			}
		} else {
			for (int i=0; i<commands.Length; i++) {
				commands[i].SetButtonEnable(false);
			}
		}
	}
}
