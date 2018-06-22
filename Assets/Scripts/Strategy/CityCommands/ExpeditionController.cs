using UnityEngine;
using System.Collections;

public class ExpeditionController: MonoBehaviour {
	
	public StrategyController strCtrl;
	public GameObject cityCommandsCtrl;
	public MyPathfinding path;
	
	public MenuDisplayAnim cityInfoAnim;
	public MenuDisplayAnim takeMoneyAnim;
	public MenuDisplayAnim armyAnim;
	public MenuDisplayAnim generalsAnim;
	
	public exSpriteFont cityName;
	public exSpriteFont prefectName;
	public exSpriteFont generalNum;
	public exSpriteFont soliderNum;
	public exSpriteFont money;
	
	public SliderHController slider;
	public exSpriteFont takeMoney;
	
	public Button okButton;
	public Button cancelButton;
	
	public PushedButton[] generalsList;
	public GameObject[] checkBoxNormal;
	public GameObject[] checkBoxPressed;
	
	public GameObject takePrison;
	public Button yesButton;
	public Button noButton;
	
	public GameObject abandonCity;
	public Button yesButton2;
	public Button noButton2;
	public exSpriteFont abandonCityFont;
	
	public AppointedPrefectController appointedPrefect;
	public SelectCommanderController selectCommander;
	
	public GameObject armyPrefab;
	public SelectTargetCity selectTargetCity;
	public FlagsController flagsCtrl;
	
	int state;
	
	private int cityIdx;
	private CityInfo cInfo;
	
	private int selectedCount = 0;
	private bool[] selectedFlag = new bool[10];
	
	private int moneyTook;
	
	private bool takePrisonFlag = false;
	
	private bool isSelectedPrefect = false;
	private int prefectBK = -1;
	
	private ArmyController armyCtrl;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		switch (state) {
		case 0:
			OnNormalMode();
			break;
		case 1:
			OnSelectIsAbandonCityMode();
			break;
		case 2:
			OnSelectIsTakePrisonMode();
			break;
		case 3:
			OnChangingToAppointedPrefectMode();
			break;
		case 4:
			OnChangingToSelectCommanderMode();
			break;
		case 1000:
			OnReturnMainMode();
			break;
		}
	}
	
	void OnNormalMode() {
		
		if (Misc.GetBack()) {
			
			ReturnMain();
		}
	}
	
	void OnSelectIsAbandonCityMode() {
		
		if (Misc.GetBack() 
			|| noButton2.GetButtonState() == Button.ButtonState.Clicked) {
			
			state = 0;
			SetAllButtonEnabled(true);
			abandonCity.SetActive(false);
			
			return;
		}
		
		if (yesButton2.GetButtonState() == Button.ButtonState.Clicked) {
			
			takePrisonFlag = true;
			isSelectedPrefect = true;
			prefectBK = cInfo.prefect;
			
			abandonCity.SetActive(false);
			
			SetArmy();
			
			cityInfoAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
			takeMoneyAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
			armyAnim		.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			generalsAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		}
	}
	
	void OnSelectIsTakePrisonMode() {
		
		if (Misc.GetBack()) {
			
			state = 0;
			SetAllButtonEnabled(true);
			takePrison.SetActive(false);
			
			return;
		}
		
		if (yesButton.GetButtonState() == Button.ButtonState.Clicked) {
			
			takePrisonFlag = true;
			takePrison.SetActive(false);
			
			IsPrefectSelected();
			
		} else if (noButton.GetButtonState() == Button.ButtonState.Clicked) {
			
			takePrisonFlag = false;
			takePrison.SetActive(false);
			
			IsPrefectSelected();
		}
	}
	
	void OnChangingToAppointedPrefectMode() {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			appointedPrefect.SetExpeditionMode(cityIdx);
			AddGeneralsToAppointedPrefect();
		}
	}
	
	void OnChangingToSelectCommanderMode() {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			selectCommander.gameObject.SetActive(true);
			AddGeneralsToSelectCommander();
		}
	}
	
	void ReturnMain() {
		
		state = 1000;
		
		cityInfoAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
		takeMoneyAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
		armyAnim		.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		generalsAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
	}
	
	void OnReturnMainMode() {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			
			state = 0;
			timeTick = 0;
			
			gameObject		.SetActive(false);
			cityCommandsCtrl.SetActive(true);
		}
	}
	
	public void SetCity(int idx) {
		
		state = 0;
		gameObject.SetActive(true);
		
		cityIdx = idx;
		cInfo 	= Informations.Instance.GetCityInfo(cityIdx);
		
		SetCityInfo();
		SetGeneralsList();
		SetMoney();
		
		SetAllButtonEnabled(true);
		
		okButton.SetButtonEnable(false);
		okButton.SetButtonClickHandler(OnOkButtonHandler);
		cancelButton.SetButtonClickHandler(OnCancelButtonHandler);
		
		cityInfoAnim	.SetAnim(MenuDisplayAnim.AnimType.InsertFromTop);
		takeMoneyAnim	.SetAnim(MenuDisplayAnim.AnimType.InsertFromTop);
		armyAnim		.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		generalsAnim	.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
	}
	
	void SetCityInfo() {
		
		cityName.text = ZhongWen.Instance.GetCityName(cityIdx);
		
		string str = ZhongWen.Instance.GetGeneralName(cInfo.prefect);
		
		prefectName.text = str;
		money.text = "" + cInfo.money;
		
		int count = cInfo.generals.Count;
		generalNum.text = "" + count;
		soliderNum.text = "" + cInfo.soldiersNum;
	}
	
	void SetGeneralsList() {
		
		int count = cInfo.generals.Count;
		int idx = 0;
		
		selectedCount = 0;
		for (int i=0; i<10; i++) {
			
			generalsList[i].gameObject	.SetActive(false);
			checkBoxNormal[i]			.SetActive(false);
			checkBoxPressed[i]			.SetActive(false);
			
			selectedFlag[i] = false;
		}
		
		for (int i=count-1; i>=0; i--) {
			
			int gIdx = (int)cInfo.generals[i];
			
			if (gIdx != cInfo.prefect)
				continue;
			
			generalsList[idx].text = SetGeneralInfo(gIdx);
			generalsList[idx].gameObject.SetActive(true);
			generalsList[idx].SetButtonDownHandler(OnGeneralSelectedHandler);
			generalsList[idx].SetButtonData(gIdx);
			generalsList[idx].SetButtonState(PushedButton.ButtonState.Normal);
			
			checkBoxNormal[idx].SetActive(true);
			
			idx++;
		}
		
		for (int i=count-1; i>=0; i--) {
			
			int gIdx = (int)cInfo.generals[i];
			
			if (gIdx == cInfo.prefect)
				continue;
			
			generalsList[idx].text = SetGeneralInfo(gIdx);
			generalsList[idx].gameObject.SetActive(true);
			generalsList[idx].SetButtonDownHandler(OnGeneralSelectedHandler);
			generalsList[idx].SetButtonData(gIdx);
			generalsList[idx].SetButtonState(PushedButton.ButtonState.Normal);
			
			checkBoxNormal[idx].SetActive(true);
			
			idx++;
		}
	}
	
	string SetGeneralInfo(int gIdx) {
		
		string str = "";
		string temp = "";
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
		temp = ZhongWen.Instance.GetGeneralName1(gIdx);
		if (temp.Length == 4 && temp[1] != ' ' && temp[2] != ' ') {
			temp += "  ";
		} else {
			temp += "    ";
		}
		
		str = "  " + temp;
		
		temp = gInfo.level + "    ";
		if (gInfo.level < 10) {
			temp = "0" + temp;
		}
		
		str += temp;
		
		temp = gInfo.strength + "   ";
		if (gInfo.strength < 10) {
			temp = "  " + temp;
		} else if (gInfo.strength < 100) {
			temp = " " + temp;
		}
		
		str += temp;
		
		temp = gInfo.intellect + "   ";
		if (gInfo.intellect < 10) {
			temp = "  " + temp;
		} else if (gInfo.intellect < 100) {
			temp = " " + temp;
		}
		
		str += temp;
		
		temp = ZhongWen.Instance.GetArmsName(gInfo.armsCur) + " ";
		
		str += temp;

		int soldierNum = gInfo.soldierCur+gInfo.knightCur;
		temp = soldierNum + "";
		if (soldierNum < 10) {
			temp = "  " + temp;
		} else if (soldierNum < 100) {
			temp = " " + temp;
		}
		
		str += temp;
		
		return str;
	}
	
	void SetMoney() {
		
		int m = cInfo.money;
		
		int stepNum = m / 100;
		if (m % 100 != 0) {
			stepNum++;
		}
		
		slider.SetSlider(stepNum, 0);
		slider.SetSliderMoveHandler(OnMoneySliderHandler);
		
		moneyTook = 0;
		takeMoney.text = "0";
	}
	
	void OnMoneySliderHandler(int offset) {
		
		int v = slider.GetCurValue();
		
		moneyTook = 100 * v;
		moneyTook = Mathf.Clamp(moneyTook, moneyTook, cInfo.money);
		
		takeMoney.text = "" + moneyTook;
	}
	
	void OnGeneralSelectedHandler(object data) {
		
		int idx = -1;
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			if (generalsList[i].GetButtonData() == data) {
				
				idx = i;
				break;
			}
		}
		
		if (selectedFlag[idx]) {
			
			selectedFlag[idx] = false;
			selectedCount--;
			
			checkBoxNormal[idx].SetActive(true);
			checkBoxPressed[idx].SetActive(false);
			
			generalsList[idx].SetButtonState(PushedButton.ButtonState.Normal);
			
		} else if (selectedCount < 5) {
			
			selectedFlag[idx] = true;
			selectedCount++;
			
			checkBoxNormal[idx].SetActive(false);
			checkBoxPressed[idx].SetActive(true);
			
		} else {
			
			generalsList[idx].SetButtonState(PushedButton.ButtonState.Normal);
		}
		
		if (selectedCount == 0 && okButton.enabled) {
			okButton.SetButtonEnable(false);
		} else if (selectedCount > 0 && !okButton.enabled) {
			okButton.SetButtonEnable(true);
		}
	}
	
	void OnOkButtonHandler() {
		
		if (selectedCount == cInfo.generals.Count) {
			state = 1;
			
			SetAllButtonEnabled(false);
			
			abandonCityFont.text = ZhongWen.Instance.qiantichuzhen + ZhongWen.Instance.GetCityName(cityIdx) + ZhongWen.Instance.cheng;
			abandonCity.SetActive(true);
			
		} else if (cInfo.prisons!= null && cInfo.prisons.Count > 0) {
			state = 2;
			
			SetAllButtonEnabled(false);
			
			takePrison.SetActive(true);
		} else {
			
			
			takePrisonFlag = false;
			
			IsPrefectSelected();
		}
	}
	
	void OnCancelButtonHandler() {
		
		ReturnMain();
	}
	
	void SetAllButtonEnabled(bool flag) {
		
		slider.SetSliderEnable(flag);
		okButton.enabled = flag;
		cancelButton.enabled = flag;
		
		int count = cInfo.generals.Count;
		for (int i=0; i<count; i++) {
			generalsList[i].enabled = flag;
		}
	}
	
	void IsPrefectSelected() {
		
		isSelectedPrefect = false;
		
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			if (generalsList[i].GetButtonState() == PushedButton.ButtonState.Pressed) {
				
				if (((int)generalsList[i].GetButtonData()) == cInfo.prefect) {
					
					isSelectedPrefect = true;
					prefectBK = cInfo.prefect;
					break;
				}
			}
		}
		
		if (isSelectedPrefect) {
			
			if (cInfo.generals.Count - selectedCount == 1) {
				
				SetArmy();
			} else {
				state = 3;
			}
		} else {
		
			if (selectedCount == 1) {
				SetArmy();
			} else {
				state = 4;
			}
		}
		
		cityInfoAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
		takeMoneyAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
		armyAnim		.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		generalsAnim	.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
	}
	
	void SetArmy() {

		Vector3 pos 			= path.GetCityPos(cityIdx);
		
		GameObject go 			= (GameObject)Instantiate(armyPrefab, pos, transform.rotation);
		armyCtrl	 			= go.GetComponent<ArmyController>();
		
		ArmyInfo armyInfo 		= new ArmyInfo();
		armyInfo.king 			= cInfo.king;
		armyInfo.cityFrom		= cityIdx;
		armyInfo.cityTo			= -1;
		armyInfo.money			= moneyTook;
		armyInfo.armyCtrl 		= armyCtrl;
		
		armyCtrl.armyInfo = armyInfo;
		Informations.Instance.armys.Add(armyInfo);
		
		GameObject root = GameObject.Find("ArmiesRoot");
		go.transform.parent = root.transform;
		
		armyCtrl.SetArmyKingFlag();
		armyCtrl.Pause();
		
		for (int i=cInfo.generals.Count-1; i>=0; i--) {
			
			if (generalsList[i].GetButtonState() == PushedButton.ButtonState.Pressed) {
				
				int gIdx = (int)generalsList[i].GetButtonData();
				
				armyInfo.generals.Add(gIdx);
				cInfo.generals.Remove(gIdx);
				
				Informations.Instance.GetGeneralInfo(gIdx).city = -1;
				
				if (selectedCount == 1) {
					armyInfo.commander = gIdx;
				}
			}
		}
		
		if (cInfo.generals.Count == 1) {
			cInfo.prefect = cInfo.generals[0];
		} else if (cInfo.generals.Count == 0) {
			Informations.Instance.GetKingInfo(cInfo.king).cities.Remove(cityIdx);
			cInfo.king = -1;
			flagsCtrl.SetFlag(cityIdx);
		}
		
		if (isSelectedPrefect) {
			armyInfo.commander = prefectBK;
		} else {
			if (state == 4) {
				armyInfo.commander = selectCommander.GetCommander();
			}
		}
		
		if (takePrisonFlag) {
			takePrisonFlag = false;
			
			armyInfo.prisons.AddRange(cInfo.prisons);
			cInfo.prisons.Clear();
			
			for (int i=0; i<armyInfo.prisons.Count; i++) {
				Informations.Instance.GetGeneralInfo(armyInfo.prisons[i]).city = -1;
			}
		}
		
		selectTargetCity.SetArmy(armyInfo);
	}
	
	void AddGeneralsToAppointedPrefect() {
		
		int idx = 0;
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			if (generalsList[i].GetButtonState() != PushedButton.ButtonState.Pressed) {
				
				appointedPrefect.AddGeneralList((int)generalsList[i].GetButtonData(), idx++);
			}
		}
	}
	
	void AddGeneralsToSelectCommander() {
		
		int idx = 0;
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			if (generalsList[i].GetButtonState() == PushedButton.ButtonState.Pressed) {
				
				selectCommander.AddGeneralList((int)generalsList[i].GetButtonData(), idx++);
			}
		}
	}
	
	public void OnReturnWithoutAppointedPrefect() {
		
		state = 0;
		gameObject.SetActive(true);
		
		cityInfoAnim	.SetAnim(MenuDisplayAnim.AnimType.InsertFromTop);
		takeMoneyAnim	.SetAnim(MenuDisplayAnim.AnimType.InsertFromTop);
		armyAnim		.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		generalsAnim	.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
	}
	
	public void OnReturnWithAppointedPrefect() {
		
		SetArmy();
	}
}
