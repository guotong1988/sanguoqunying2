using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConscriptionController : MonoBehaviour {
	
	public GameObject cityCommandsCtrl;
	
	public MenuDisplayAnim commandsLayout;
	public MenuDisplayAnim generalInfoLayout;
	public MenuDisplayAnim generalsListLayout;
	public MenuDisplayAnim armsLayout;
	
	public GeneralsHeadSelect head;
	public exSpriteFont giName;
	public exSpriteFont giLevel;
	public exSpriteFont giSoldier;
	public exSpriteFont giCavalry;
	public exSpriteFont giCity;
	public exSpriteFont giMoney;
	public exSpriteFont giReservist;
	
	public GameObject itemPrefab;
	
	public Transform token;
	
	public SliderHController slider;
	
	public Button[] cmdBtn;
	public Button[] arms;
	
	private int state;
	private int selectIdx;
	private float timeTick;
	
	private Vector3 gListPos = new Vector3(-220, 122, 0);
	private List<PushedButton> gList = new List<PushedButton>();
	
	// Use this for initialization
	void Start () {
		
		slider.SetSliderMoveHandler(OnSliderMoveController);
		
		cmdBtn[0].SetButtonClickHandler(OnMaxConsClickHandler);
		cmdBtn[1].SetButtonClickHandler(OnChangeArmsClickHandler);
		cmdBtn[2].SetButtonClickHandler(OnOverClickHandler);
		
		for (int i=0; i<arms.Length; i++) {
			
			arms[i].SetButtonClickHandler(OnArmsButtonHandler);
			arms[i].SetButtonData(i);
		}
	}
	
	void OnEnable() {
		
		state = 0;
		
		armsLayout.gameObject.SetActive(false);
		
		commandsLayout		.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		generalInfoLayout	.SetAnim(MenuDisplayAnim.AnimType.InsertFromTop);
		generalsListLayout	.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
		
	}
	
	void OnDisable() {
		
		for (int i=0; i<gList.Count; i++) {
			Destroy(((PushedButton)gList[i]).gameObject);
		}
		
		gList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			OnNormalMode();
			break;
		case 1:
			OnChangingToArmsMode();
			break;
		case 2:
			OnSelectArmsMode();
			break;
		case 3:
			OnChangingToCommandsMode();
			break;
		case 4:
			OnReturnMainMode();
			break;
		}
	}
	
	void OnNormalMode() {
		if (Misc.GetBack()) {
			
			ReturnMain();
		}
	}
	
	void OnChangingToArmsMode() {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			
			state = 2;
			timeTick = 0;
			
			commandsLayout.gameObject.SetActive(false);
			armsLayout.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		}
	}
	
	void OnSelectArmsMode() {
		if (Misc.GetBack()) {
			
			state = 3;
			armsLayout.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		}
	}
	
	void OnChangingToCommandsMode() {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			
			state = 0;
			timeTick = 0;
			
			armsLayout.gameObject.SetActive(false);
			commandsLayout.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		}
	}
	
	void ReturnMain() {
		
		state = 4;
			
		commandsLayout		.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		generalInfoLayout	.SetAnim(MenuDisplayAnim.AnimType.OutToTop);
		generalsListLayout	.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
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
	
	void SetGeneralInfo(int idx) {
		
		head.SetGeneralHead(idx);
		
		string gName = ZhongWen.Instance.GetGeneralName(idx);
		
		giName.text = gName;
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(idx);
		
		giLevel		.text = gInfo.level + "";
		giSoldier	.text = gInfo.soldierCur + "/" + gInfo.soldierMax;
		giCavalry	.text = gInfo.knightCur + "/" + gInfo.knightMax;
		
	}
	
	void SetCityInfo(int idx) {
		
		giCity.text = ZhongWen.Instance.GetCityName(idx);
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(idx);
		
		giMoney		.text = cInfo.money + "";
		giReservist	.text = cInfo.reservist + "/" + cInfo.reservistMax;
	}
	
	void AddGeneralsList(int idx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(idx);
		
		int pos = 0;
		
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			int gIdx = (int)cInfo.generals[i];
			
			if (gIdx != Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx) {
				continue;
			}
			
			AddGeneral(gIdx, pos);
			pos++;
		}
		
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			int gIdx = (int)cInfo.generals[i];
			
			if (gIdx == Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx) {
				continue;
			}
			
			AddGeneral(gIdx, pos);
			pos++;
		}
	}
	
	void AddGeneral(int gIdx, int pos) {
		
		GameObject go = (GameObject)Instantiate(itemPrefab);
			
		go.transform.parent = generalsListLayout.transform;
		go.transform.localPosition = new Vector3(gListPos.x, gListPos.y - pos * 30, gListPos.z);
		
		PushedButton pBtn = go.GetComponent<PushedButton>();
		
		gList.Add(pBtn);
		
		pBtn.SetButtonData(gIdx);
		pBtn.SetButtonDownHandler(OnGeneralSelectedHandler);
		if (pos == 0) {
			selectIdx = 0;
			pBtn.SetButtonState(PushedButton.ButtonState.Down);
		}
		
		SetGeneralText(go, gIdx);
	}
	
	void SetGeneralText(GameObject go, int gIdx) {
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		string str = "";
		
		string temp = ZhongWen.Instance.GetGeneralName1(gIdx);
		if (temp.Length == 4 && temp[1] != ' ' && temp[2] != ' ') {
			temp += " ";
		} else {
			temp += "   ";
		}
		
		str += temp;
		
		temp = (gInfo.soldierCur + gInfo.knightCur) + "/" + (gInfo.soldierMax + gInfo.knightMax);
		//temp.PadLeft(7, ' ');
		int count = 7 - temp.Length;
		for (int i=0; i<count; i++) {
			temp = " " + temp;
		}
		
		str += temp;
		
		temp = "  " + ZhongWen.Instance.GetArmsName(gInfo.armsCur);
		
		str += temp;
		
		go.GetComponent<exSpriteFont>().text = str;
	}
	
	void OnGeneralSelectedHandler(object data) {
		
		int gIdx = (int)data;
		
		for (int i=0; i<gList.Count; i++) {
			
			PushedButton pbtn = (PushedButton)gList[i];
			
			if (pbtn.GetButtonData() == data) {
				
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
				
				slider.transform.position = new Vector3(slider.transform.position.x, pbtn.transform.position.y, slider.transform.position.z);
				slider.SetSlider(gInfo.soldierMax + gInfo.knightMax, gInfo.soldierCur + gInfo.knightCur);
				
				selectIdx = i;
				
			} else {
				
				pbtn.SetButtonState(PushedButton.ButtonState.Normal);
			}
		}
		
		SetGeneralInfo(gIdx);
		UpdateChangeArms(gIdx);
	}
	
	void OnSliderMoveController(int offset) {
		
		PushedButton pbtn = (PushedButton)gList[selectIdx];
		
		int gIdx = (int)pbtn.GetButtonData();
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		CityInfo 	cInfo = Informations.Instance.GetCityInfo(gInfo.city);
		
		if (cInfo.reservist - offset < 0 || cInfo.reservist - offset > cInfo.reservistMax) {
			
			slider.SetSlider(gInfo.soldierMax + gInfo.knightMax, gInfo.soldierCur + gInfo.knightCur);
			return;
		}
		
		cInfo.reservist -= offset;
		
		if (offset > 0) {
			if (gInfo.knightCur + offset >= gInfo.knightMax) {
				int sAdd = offset - (gInfo.knightMax - gInfo.knightCur);
				gInfo.knightCur = gInfo.knightMax;
				
				gInfo.soldierCur += sAdd;
				gInfo.soldierCur = Mathf.Clamp(gInfo.soldierCur, 0, gInfo.soldierMax);
			} else {
				gInfo.knightCur += offset;
				gInfo.knightCur = Mathf.Clamp(gInfo.knightCur, 0, gInfo.knightMax);
			}
		} else {
			if (gInfo.soldierCur + offset <= 0) {
				int kAdd = offset + gInfo.soldierCur;
				gInfo.soldierCur = 0;
				
				gInfo.knightCur += kAdd;
				gInfo.knightCur = Mathf.Clamp(gInfo.knightCur, 0, gInfo.knightMax);
			} else {
				gInfo.soldierCur += offset;
				gInfo.soldierCur = Mathf.Clamp(gInfo.soldierCur, 0, gInfo.soldierMax);
			}
		}
		
		giReservist	.text = cInfo.reservist + "/" + cInfo.reservistMax;
		giSoldier	.text = gInfo.soldierCur + "/" + gInfo.soldierMax;
		giCavalry	.text = gInfo.knightCur + "/" + gInfo.knightMax;
		
		SetGeneralText(pbtn.gameObject, gIdx);
	}
	
	void OnMaxConsClickHandler() {
		
		PushedButton pbtn = (PushedButton)gList[selectIdx];
		
		int gIdx = (int)pbtn.GetButtonData();
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		CityInfo 	cInfo = Informations.Instance.GetCityInfo(gInfo.city);
		
		int num = (gInfo.soldierMax + gInfo.knightMax) - (gInfo.soldierCur + gInfo.knightCur);
		
		if (num == 0) {
			return;
		}
		
		if (cInfo.reservist >= num) {
			
			cInfo.reservist  -= num;
			gInfo.soldierCur = gInfo.soldierMax;
			gInfo.knightCur = gInfo.knightMax;
		} else {
			
			if (cInfo.reservist > gInfo.knightMax - gInfo.knightCur) {
				int sAdd = cInfo.reservist - (gInfo.knightMax - gInfo.knightCur);
				gInfo.knightCur = gInfo.knightMax;
				
				gInfo.soldierCur += sAdd;
				gInfo.soldierCur = Mathf.Clamp(gInfo.soldierCur, 0, gInfo.soldierMax);
			} else {
				gInfo.knightCur += cInfo.reservist;
			}
			cInfo.reservist = 0;
		}
		
		giReservist	.text = cInfo.reservist + "/" + cInfo.reservistMax;
		giSoldier	.text = gInfo.soldierCur + "/" + gInfo.soldierMax;
		giCavalry	.text = gInfo.knightCur + "/" + gInfo.knightMax;
		
		SetGeneralText(pbtn.gameObject, gIdx);
		slider.SetSlider(gInfo.soldierMax + gInfo.knightMax, gInfo.soldierCur + gInfo.knightCur);
	}
	
	void OnChangeArmsClickHandler() {
		
		state = 1;
		
		PushedButton pbtn = (PushedButton)gList[selectIdx];
		int gIdx = (int)pbtn.GetButtonData();
		UpdateChangeArms(gIdx);
		
		commandsLayout.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
	}

	void UpdateChangeArms(int gIdx) {

		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		for (int i=0; i<arms.Length; i++) {
			
			if ((gInfo.arms & (1 << i)) != 0) {
				arms[i].SetButtonEnable(true);
			} else {
				arms[i].SetButtonEnable(false);
			}
			
			if ((gInfo.armsCur & (1 << i)) != 0) {
				token.transform.position = new Vector3(token.transform.position.x, 
				                                       arms[i].transform.position.y, token.transform.position.z);
			}
		}
	}

	void OnOverClickHandler() {
		
		ReturnMain();
	}
	
	void OnArmsButtonHandler(object data) {
		
		int idx = (int)data;
		
		PushedButton pbtn = (PushedButton)gList[selectIdx];
		
		int gIdx = (int)pbtn.GetButtonData();
		Informations.Instance.GetGeneralInfo(gIdx).armsCur = 1 << idx;
		
		token.transform.position = new Vector3(token.transform.position.x, 
			   arms[idx].transform.position.y, token.transform.position.z);

		SetGeneralText(pbtn.gameObject, gIdx);
	}
	
	public void SetCity(int idx) {
		
		gameObject.SetActive(true);
		
		SetCityInfo(idx);
		AddGeneralsList(idx);
	}
}
