using UnityEngine;
using System.Collections;

public class SPController : MonoBehaviour {
	
	public IAController IACtrl;
	
	public ListController prisonerList;
	public DialogueController kingDialogue;
	public DialogueController prisonerDialogue;
	
	private int state = -1;
	private bool havePrison = false;
	
	private int prisonIdx;
	private int dialogueIdx;
	
	private float timeTick = 0;
	
	// Use this for initialization
	void Start () {
		prisonerList.SetSelectItemHandler(OnSelectPrisonHandler);
	}
	
	void OnEnable() {
		
		if (havePrison) {
			state = 2;
			
			prisonerList.gameObject.SetActive(true);
			
			prisonerList.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		} else {
			state = 0;
			
			prisonerList.gameObject.SetActive(false);
		}
	}
	
	void OnDisable() {
		havePrison = false;
		prisonerList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			OnNoPrisonDialogueInsertHandler();
			break;
		case 1:
			OnNoPrisonDialogueOutHandler();
			break;
		case 2:
			OnSelectPrisonModeHandler();
			break;
		case 3:
			OnKingAskModeHandler();
			break;
		case 4:
			OnPrisonAnswerModeHandler();
			break;
		case 5:
			OnKingAsnwerModeHandler();
			break;
		case 6:
			OnPrisonNoInCityHandler();
			break;
		case 7:
			OnDialogueOverHandler();
			break;
		case 8:
			OnReturnMainModeHandler();
			break;
		}
	}
	
	void OnNoPrisonDialogueInsertHandler() {
		if (!kingDialogue.IsShowingText() && Input.GetMouseButtonUp(0)) {
			state = 1;
			kingDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
		}
	}
	
	void OnNoPrisonDialogueOutHandler() {
		if (kingDialogue.gameObject.activeSelf == false) {
			gameObject.SetActive(false);
			IACtrl.ResetState();
		}
	}
	
	void OnSelectPrisonModeHandler() {
		if (Misc.GetBack()) {
			state = 8;
			
			prisonerList.gameObject.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		}
	}
	
	void OnSelectPrisonHandler() {
		if (state != 2)	return;
		
		prisonIdx = (int)prisonerList.GetSelectItem().GetItemData();
		string msg = "";
		
		if (Informations.Instance.GetGeneralInfo(prisonIdx).city != -1) {
			state = 3;
			
			if (Informations.Instance.GetGeneralInfo(prisonIdx).king == Controller.kingIndex) {
				msg = ZhongWen.Instance.GetGeneralName(prisonIdx) + ZhongWen.Instance.zhaoxiang_guilai;
			} else {
				msg = ZhongWen.Instance.GetGeneralName(prisonIdx) + ZhongWen.Instance.zhaoxiang_ask;
			}
		} else {
			state = 6;
			Informations.Instance.GetGeneralInfo(prisonIdx).active = 0;
			msg = ZhongWen.Instance.GetGeneralName(prisonIdx) + ZhongWen.Instance.zhaoxiang_buzai;
		}
		
		prisonerList.enabled = false;
		
		kingDialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
	}
	
	void OnKingAskModeHandler() {
		if (!kingDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 4;
				bool isSuccess = false;
				
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(prisonIdx);
				gInfo.active = 0;
				
				if (gInfo.king == Controller.kingIndex) {
					dialogueIdx = 5;
					
					isSuccess = true;
					
				} else if (Random.Range(0, 100) < (100 - gInfo.loyalty) / 2) {
					dialogueIdx = 0;
					
					isSuccess = true;
					
				} else {
					dialogueIdx = (100 - gInfo.loyalty) / 10 + 1;
					dialogueIdx = Mathf.Clamp(dialogueIdx, 0, 4);
					
					gInfo.loyalty -= Random.Range(5, 20);
					gInfo.loyalty = Mathf.Clamp(gInfo.loyalty, 0, 100);
				}
				
				if (isSuccess) {
					
					gInfo.loyalty = 90;
					gInfo.king = Controller.kingIndex;
					gInfo.prisonerIdx = -1;
					gInfo.soldierCur = gInfo.soldierMax;
					gInfo.knightCur = gInfo.knightMax;
					
					CityInfo cInfo = Informations.Instance.GetCityInfo(gInfo.city);
					for (int i=0; i<cInfo.prisons.Count; i++) {
						
						if ((int)cInfo.prisons[i] == prisonIdx) {
							cInfo.prisons.RemoveAt(i);
							break;
						}
					}
					
					cInfo.generals.Add(prisonIdx);
					Informations.Instance.GetKingInfo(Controller.kingIndex).generals.Add(prisonIdx);

					SoundController.Instance.PlaySound("00045");
				} else {
					SoundController.Instance.PlaySound("00057");
				}
				
				string msg = ZhongWen.Instance.zhaoxiang_wenda[dialogueIdx * 2];
				
				prisonerDialogue.SetDialogue(prisonIdx, msg, MenuDisplayAnim.AnimType.InsertFromTop);
			}
		}
	}
	
	void OnPrisonAnswerModeHandler() {
		if (!prisonerDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 5;
				
				Input.ResetInputAxes();
				
				kingDialogue.SetText(ZhongWen.Instance.zhaoxiang_wenda[dialogueIdx * 2 + 1]);
			}
		}
	}
	
	void OnKingAsnwerModeHandler() {
		if (!kingDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 7;
				
				kingDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				prisonerDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToTop);
				
				prisonerList.GetSelectItem().SetSelectEnable(false);
				prisonerList.SetItemSelected(-1, false);
			}
		}
	}
	
	void OnPrisonNoInCityHandler() {
		if (!kingDialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				state = 7;
				
				kingDialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				
				prisonerList.GetSelectItem().SetSelectEnable(false);
				prisonerList.SetItemSelected(-1, false);
			}
		}
	}
	
	void OnDialogueOverHandler() {
		if (!kingDialogue.gameObject.activeSelf && !prisonerDialogue.gameObject.activeSelf) {
			state = 2;
			
			prisonerList.enabled = true;
		}
	}
	
	void OnReturnMainModeHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			IACtrl.OnReturnMain();
		}
	}
	
	public bool AddPrisonsList() {
		
		int king = Controller.kingIndex;
		
		for (int i=0; i<Informations.Instance.generalNum; i++) {
			
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(i);
			
			if (gInfo.prisonerIdx == king) {
				
				havePrison = true;
				
				ListItem li = prisonerList.AddItem(ZhongWen.Instance.GetGeneralName1(i));
				li.SetItemData(i);
				if (gInfo.active == 0) {
					li.SetSelectEnable(false);
				}
			}
		}
		
		if (!havePrison) {
			kingDialogue.SetDialogue(Informations.Instance.GetKingInfo(king).generalIdx, ZhongWen.Instance.zhaoxiang_no, MenuDisplayAnim.AnimType.InsertFromBottom);
		}
		
		return havePrison;
	}
}
