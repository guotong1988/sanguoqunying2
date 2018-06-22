using UnityEngine;
using System.Collections;

public class CCGeneralsInfoController : MonoBehaviour {
	
	public GameObject 		cityCommandsCtrl;
	public GameObject 		armyCommandsCtrl;
	
	public ListController 	generalsList;
	
	public GameObject		information;
	public GIInformation 	generalInfo;
	public GIFormation 		formation;
	public GIArms 			arms;
	public GIMagic 			magic;
	public GIEquipment 		equipment;
	
	public MenuDisplayAnim 	arrow;
	public ImageButton 		leftArrow;
	public ImageButton 		rightArrow;
	
	private int state = 0;
	private int mode = 0;
	private int selectIdx = 0;
	private int selectGeneralIdx = 98;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		
		generalsList.SetSelectItemHandler(OnSelectGeneral);
		
		leftArrow.SetButtonClickHandler(OnLeftArrowClickHandler);
		rightArrow.SetButtonClickHandler(OnRightArrowClickHandler);
	}
	
	void OnEnable() {
		state = 0;
		
		information.SetActive(false);
		generalsList.gameObject.SetActive(true);
		generalsList.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}
	
	void OnDisable() {
		generalsList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			OnSelectGeneralModeHandler();
			break;
		case 1:
			OnGeneralInformationModeHandler();
			break;
		case 2:
			OnChangeToInformationModeHandler();
			break;
		case 3:
			OnChangeToGeneralsListModeHandler();
			break;
		case 4:
			OnReturnMainHandler();
			break;
		}
	}
	
	void OnSelectGeneralModeHandler() {
		if (Misc.GetBack()) {
			state = 4;
			generalsList.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			return;
		}
	}
	
	void OnGeneralInformationModeHandler() {
		if (Misc.GetBack()) {
			state = 3;
			
			generalInfo	.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			formation	.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToTop);
			arms		.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
			magic		.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToBottom);
			equipment	.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
			arrow.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			
			return;
		}
	}
	
	void OnChangeToInformationModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			state = 1;
			
			generalsList.gameObject.SetActive(false);
			information.SetActive(true);
			
			generalInfo	.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
			formation	.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromTop);
			arms		.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
			magic		.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromBottom);
			equipment	.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
			arrow.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		}
	}
	
	void OnChangeToGeneralsListModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			state = 0;
			
			generalsList.gameObject.SetActive(true);
			generalsList.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
			
			information.SetActive(false);
		}
	}
	
	void OnReturnMainHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			if (mode == 0) {
				cityCommandsCtrl.SetActive(true);
			} else {
				armyCommandsCtrl.SetActive(true);
			}
		}
	}
	
	void OnSelectGeneral() {
		if (state != 0) return;
		
		state = 2;
				
		selectIdx = generalsList.GetSelectIndex();
		selectGeneralIdx = (int)generalsList.GetSelectItem().GetItemData();
		
		generalsList.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		
		OnChangeGeneralInfo();
	}
	
	void OnLeftArrowClickHandler() {
		if (selectIdx > 0) {
			selectIdx--;
			selectGeneralIdx = (int)generalsList.GetListItem(selectIdx).GetItemData();
			
			OnChangeGeneralInfo();
		}
	}
	
	void OnRightArrowClickHandler() {
		if (selectIdx < generalsList.GetCount() - 1) {
			selectIdx++;
			selectGeneralIdx = (int)generalsList.GetListItem(selectIdx).GetItemData();
			
			OnChangeGeneralInfo();
		}
	}
	
	void OnChangeGeneralInfo() {
		generalInfo	.SetGeneral(selectGeneralIdx);
		formation	.SetGeneral(selectGeneralIdx);
		arms		.SetGeneral(selectGeneralIdx);
		magic		.SetGeneral(selectGeneralIdx);
		equipment	.SetGeneral(selectGeneralIdx);
	}
	
	public void AddGeneralsList(int cIdx) {
		
		mode = 0;
		gameObject.SetActive(true);
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);

		for (int i=0; i<cInfo.generals.Count; i++) {
			
			int gIdx = (int)cInfo.generals[i];
			
			if (gIdx != Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx) {
				continue;
			}
			
			generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
		}
		
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			int gIdx = cInfo.generals[i];
			
			if (gIdx == Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx) {
				continue;
			}
			
			generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
		}
	}
	
	public void AddPrisonsList(int cIdx) {
		
		mode = 0;
		gameObject.SetActive(true);
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);

		for (int i=0; i<cInfo.prisons.Count; i++) {
			
			int gIdx = cInfo.prisons[i];
			generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
		}
	}
	
	public void AddArmyGeneralsList(ArmyInfo armyInfo) {
		
		mode = 1;
		gameObject.SetActive(true);
		
		for (int i=0; i<armyInfo.generals.Count; i++) {
			
			int gIdx = armyInfo.generals[i];
			if (gIdx == armyInfo.commander) {
				generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
				break;
			}
		}
		
		for (int i=0; i<armyInfo.generals.Count; i++) {
			
			int gIdx = armyInfo.generals[i];
			if (gIdx == armyInfo.commander) continue;
			
			generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
		}
	}
	
	public void AddArmyPrisonsList(ArmyInfo armyInfo) {
		
		mode = 1;
		gameObject.SetActive(true);
		
		for (int i=0; i<armyInfo.prisons.Count; i++) {
			
			int gIdx = armyInfo.prisons[i];
			generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
		}
	}
}
