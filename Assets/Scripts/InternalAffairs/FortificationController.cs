using UnityEngine;
using System.Collections;

public class FortificationController : MonoBehaviour {
	
	public IAController IACtrl;
	public ListController generalsList;
	public DialogueController bottomDialogue;
	public DialogueController topDialogue;
	
	public MenuDisplayAnim BG;
	public MapController map;
	public CityInfoController cityInfo;
	
	private int state = -1;
	private bool haveGeneral = false;
	
	private int selectGeneralIdx = -1;
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable() {
		
		// test
		AddGeneralsList();
		
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
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(selectGeneralIdx);
		CityInfo cInfo = Informations.Instance.GetCityInfo(gInfo.city);
		
		int defenseAdd = 30 + Random.Range(0, 10) + Random.Range(0, gInfo.strength - 80);
		cInfo.defense += defenseAdd;
		cInfo.defense = Mathf.Clamp(cInfo.defense, cInfo.defense, 9999);
		
		string msg = ZhongWen.Instance.GetCityName(gInfo.city) + ZhongWen.Instance.chucheng + defenseAdd;
		bottomDialogue.SetDialogue(selectGeneralIdx, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
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
			
			if (gInfo.city != -1 && gInfo.strength >= 80 && gIdx != kInfo.generalIdx) {
				
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
