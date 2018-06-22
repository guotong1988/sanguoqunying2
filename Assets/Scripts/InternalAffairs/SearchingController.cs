using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchingController : MonoBehaviour {
	
	public IAController IACtrl;
	public ListController generalsList;
	public DialogueController bottomDialogue;
	public DialogueController topDialogue;
	
	public MenuDisplayAnim BG;
	public MapController map;
	public CityInfoController cityInfo;
	
	private int state = -1;
	private bool haveGeneral = false;
	
	private int findWhat = -1;
	private int selectGeneralIdx = -1;
	private int findGeneralIdx = -1;
	private int answerIdx = -1;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable() {
		
		if (haveGeneral) {
			state = 2;
			
			BG			.gameObject.SetActive(true);
			generalsList.gameObject.SetActive(true);
			map			.gameObject.SetActive(true);
			cityInfo	.gameObject.SetActive(true);
			
			BG			.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
			generalsList.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
			map			.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
			cityInfo	.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromBottom);
			
			generalsList.SetSelectItemHandler(OnSelectGeneralHandler);
		} else {
			state = 0;
			
			BG.gameObject.SetActive(false);
		}
	}
	
	void OnDisable() {
		haveGeneral = false;
		generalsList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			OnNoGeneralModeHandler();
			break;
		case 1:
			OnNoGeneralDialogueOutHandler();
			break;
		case 2:
			OnGeneralSelectModeHandler();
			break;
		case 3:
			OnCityInfoDownModeHandler();
			break;
		case 4:
			OnGeneralDialogueModeHandler();
			break;
		case 5:
			OnGeneralDialogueDownMdoeHandler();
			break;
		case 6:
			OnKingAskModeHandler();
			break;
		case 7:
			OnGeneralAnswerModeHandler();
			break;
		case 8:
			OnKingAnswerModeHandler();
			break;
		case 9:
			OnKingAnswerOverModeHandler();
			break;
		case 1000:
			OnReturnMainModeHandler();
			break;
		}
	}
	
	void OnNoGeneralModeHandler() {
		if (!bottomDialogue.IsShowingText() && Input.GetMouseButtonUp(0)) {
			state = 1;
			bottomDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
		}
	}
	
	void OnNoGeneralDialogueOutHandler() {
		if (bottomDialogue.gameObject.activeSelf == false) {
			gameObject.SetActive(false);
			IACtrl.ResetState();
		}
	}
	
	void OnGeneralSelectModeHandler() {
		if (Misc.GetBack()) {
			state = 1000;
			
			BG			.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			generalsList.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			map			.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
			cityInfo	.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToBottom);
		}
	}
	
	void OnSelectGeneralHandler() {
		if (state != 2)	return;
		
		state = 3;
		selectGeneralIdx = (int)generalsList.GetSelectItem().GetItemData();
		
		cityInfo.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToBottom);
		generalsList.enabled = false;
	}
	
	void OnCityInfoDownModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick < 0.2f)	return;
		
		timeTick = 0;
		state = 4;
		
		cityInfo.gameObject.SetActive(false);
		
		int gIdx = selectGeneralIdx;
		
		findWhat = Random.Range(0, 100) / 20;
		//test
		//findWhat = 1;
		string msg = "";
		
		switch (findWhat) {
		case 0:
			msg = ZhongWen.Instance.sousuo_nothing;
			break;
		case 1:
		{
			List<int> list = new List<int>();
			for (int i=0; i<Informations.Instance.generalNum; i++) {
				if (Informations.Instance.GetGeneralInfo(i).king == -1) {
					list.Add(i);
				}
			}
			
			if (list.Count == 0) {
				msg = ZhongWen.Instance.sousuo_nothing;
				findWhat = 0;
				break;
			}
			
			findGeneralIdx = (int)list[Random.Range(0, list.Count)];
			
			msg = ZhongWen.Instance.sousuo_rencai;
		}
			break;
		case 2:
		{
			if (Informations.Instance.GetCityInfo(Informations.Instance.GetGeneralInfo(gIdx).city).objects.Count >= 50) {
				msg = ZhongWen.Instance.sousuo_nothing;
				findWhat = 0;
				break;
			}
			
			int equipment = Random.Range(0, Informations.Instance.equipmentNum);
			
			int code = (1 << 16) + equipment;
			Informations.Instance.GetCityInfo(Informations.Instance.GetGeneralInfo(gIdx).city).objects.Add(code);
			
			msg = ZhongWen.Instance.sousuo_wupin + ZhongWen.Instance.GetEquipmentName(equipment);
		}
			break;
		case 3:
		{
			if (Informations.Instance.GetCityInfo(Informations.Instance.GetGeneralInfo(gIdx).city).objects.Count >= 50) {
				msg = ZhongWen.Instance.sousuo_nothing;
				findWhat = 0;
				break;
			}
			
			int arms = 1 << Random.Range(0, Informations.Instance.armsNum);
			
			int code = (2 << 16) + arms;
			Informations.Instance.GetCityInfo(Informations.Instance.GetGeneralInfo(gIdx).city).objects.Add(code);
			
			msg = ZhongWen.Instance.sousuo_wupin + ZhongWen.Instance.GetArmsName(arms) + ZhongWen.Instance.bingfu;
		}
			break;
		case 4:
		{
			if (Informations.Instance.GetCityInfo(Informations.Instance.GetGeneralInfo(gIdx).city).objects.Count >= 50) {
				msg = ZhongWen.Instance.sousuo_nothing;
				findWhat = 0;
				break;
			}
			
			int formation = 1 << Random.Range(0, Informations.Instance.formationNum);
			
			int code = (3 << 16) + formation;
			Informations.Instance.GetCityInfo(Informations.Instance.GetGeneralInfo(gIdx).city).objects.Add(code);
			
			msg = ZhongWen.Instance.sousuo_wupin + ZhongWen.Instance.GetFormationName(formation) + ZhongWen.Instance.zhishu;
		}
			break;
		}
		
		bottomDialogue.SetDialogue(gIdx, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
	}
	
	void OnGeneralDialogueModeHandler() {
		if (!bottomDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 5;
				
				bottomDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
			}
		}
	}
	
	void OnGeneralDialogueDownMdoeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick < 0.2f) return;
		timeTick = 0;
			
		if (findWhat == 1) {
			state = 6;
			
			bottomDialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx, 
				ZhongWen.Instance.GetGeneralName(findGeneralIdx) + ZhongWen.Instance.sousuo_rencai_ask, MenuDisplayAnim.AnimType.InsertFromBottom);
		} else {
			state = 2;
			
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(selectGeneralIdx);
			gInfo.active = 0;
			
			generalsList.enabled = true;
			generalsList.GetSelectItem().SetSelectEnable(false);
			generalsList.SetItemSelected(-1, false);
			
			cityInfo.gameObject.SetActive(true);
			cityInfo.SetCity(gInfo.city);
			cityInfo.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromBottom);
			
			map.ClearSelect();
			map.SelectCity(gInfo.city);
		}
	}
	
	void AddGeneralToCity(int gIdx, int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
		gInfo.active = 0;
		gInfo.king = cInfo.king;
		gInfo.city = cIdx;
		gInfo.loyalty = 90;
		gInfo.prisonerIdx = -1;
		
		int levelSum = 0;
		for (int i=0; i<cInfo.generals.Count; i++) {
			levelSum += Informations.Instance.GetGeneralInfo(cInfo.generals[i]).level;
		}
		int level = levelSum / cInfo.generals.Count;
		gInfo.experience = Misc.GetLevelExperience(level);
		Misc.CheckIsLevelUp(gInfo);
		
		cInfo.generals.Add(gIdx);
		Informations.Instance.GetKingInfo(cInfo.king).generals.Add(gIdx);
	}
	
	void OnKingAskModeHandler() {
		if (!bottomDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 7;
				
				answerIdx = Random.Range(0, 100) / 50;
				if (answerIdx == 0) {
					AddGeneralToCity(findGeneralIdx, Informations.Instance.GetGeneralInfo(selectGeneralIdx).city);
					SoundController.Instance.PlaySound("00045");
				} else {
					SoundController.Instance.PlaySound("00057");
				}
				
				string msg = ZhongWen.Instance.sousuo_rencai_wenda[answerIdx * 2];
				topDialogue.SetDialogue(findGeneralIdx, msg, MenuDisplayAnim.AnimType.InsertFromTop);
			}
		}
	}
	
	void OnGeneralAnswerModeHandler() {
		if (!topDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 8;
				
				string msg = ZhongWen.Instance.sousuo_rencai_wenda[answerIdx * 2 + 1];
				bottomDialogue.SetText(msg);
				Input.ResetInputAxes();
			}
		}
	}
	
	void OnKingAnswerModeHandler() {
		if (!bottomDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 9;
				
				topDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToTop);
				bottomDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
			}
		}
	}
	
	void OnKingAnswerOverModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick < 0.2f) return;
		timeTick = 0;
		
		state = 2;
			
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(selectGeneralIdx);
		gInfo.active = 0;
		
		generalsList.enabled = true;
		generalsList.GetSelectItem().SetSelectEnable(false);
		generalsList.SetItemSelected(-1, false);
		
		cityInfo.gameObject.SetActive(true);
		cityInfo.SetCity(gInfo.city);
		cityInfo.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromBottom);
		
		map.ClearSelect();
		map.SelectCity(gInfo.city);
	}
	
	void OnReturnMainModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			generalsList.gameObject.SetActive(false);
			map			.gameObject.SetActive(false);
			cityInfo	.gameObject.SetActive(false);
			
			IACtrl.OnReturnMain();
		}
	}
	
	public bool AddGeneralsList() {
		if (haveGeneral)	return haveGeneral;
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		
		for (int i=0; i<kInfo.generals.Count; i++) {
			
			int gIdx = (int)kInfo.generals[i];
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
			if (gInfo.city != -1 && (gInfo.strength >= 85 || gInfo.intellect >= 85) && gIdx != kInfo.generalIdx) {
				
				haveGeneral = true;
				
				ListItem li = generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx));
				
				li.SetItemData(gIdx);
				if (gInfo.active == 0) {
					li.SetSelectEnable(false);
				}
				
				if (generalsList.GetCount() == 1) {
					
					cityInfo.SetCity(gInfo.city);
					
					map.ClearSelect();
					map.SelectCity(gInfo.city);
				}
			}
		}
		
		if (!haveGeneral) {
			bottomDialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx, ZhongWen.Instance.meiyouwujiang, MenuDisplayAnim.AnimType.InsertFromBottom);
		}
		
		return haveGeneral;
	}
	
}
