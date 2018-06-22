using UnityEngine;
using System.Collections;

public class GPController : MonoBehaviour {
	
	public IAController IACtrl;
	public DialogueController dialogue;
	
	public MenuDisplayAnim BG;
	public ListController generalsList;
	public GameObject jobsCtrl;
	
	public Button[] jobsName;
	
	private int state = -1;
	private bool haveGeneral = false;
	private bool isTopJobLevel = false;
	private int[] jobsIdx = new int[3];
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		generalsList.SetSelectItemHandler(OnGeneralSelectHandler);
	}
	
	void OnEnable() {
		if (haveGeneral) {
			state = 2;
			
			BG			.gameObject.SetActive(true);
			generalsList.gameObject.SetActive(true);
			jobsCtrl	.SetActive(false);
			
			BG.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
			generalsList.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		} else {
			state = 0;
			
			BG			.gameObject.SetActive(false);
			generalsList.gameObject.SetActive(false);
			jobsCtrl	.SetActive(false);
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
			OnNoGeneralsPromotionModeHandler();
			break;
		case 1:
			OnNoGeneralsPromotionOutHandler();
			break;
		case 2:
			OnSelectGeneralModeHandler();
			break;
		case 3:
			OnSelectJobModeHandler();
			break;
		case 4:
			OnKingDialogueModeHandler();
			break;
		case 5:
			OnGeneralDialogueModeHandler();
			break;
		case 1000:
			OnReturnMainModeHandler();
			break;
		}
	}
	
	void OnNoGeneralsPromotionModeHandler() {
		if (!dialogue.IsShowingText() && Input.GetMouseButtonUp(0)) {
			state = 1;
			dialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
		}
	}
	
	void OnNoGeneralsPromotionOutHandler() {
		if (dialogue.gameObject.activeSelf == false) {
			gameObject.SetActive(false);
			IACtrl.ResetState();
		}
	}
	
	void OnSelectGeneralModeHandler() {
		if (Misc.GetBack()) {
			state = 1000;
			
			BG.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			generalsList.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		}
	}
	
	void OnGeneralSelectHandler() {
		int gIdx = (int)generalsList.GetSelectItem().GetItemData();
		
		generalsList.enabled = false;
		
		isTopJobLevel = false;
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		if (gInfo.magic[2] != -1 && gInfo.magic[3] == -1) {
			
			if (gInfo.magic[2] < 18) {
				gInfo.magic[3] = 18;
			} else {
				gInfo.magic[3] = 49;
			}
			gInfo.job = gInfo.magic[3];
			
			string msg = ZhongWen.Instance.shengqian_feng + ZhongWen.Instance.GetGeneralName(gIdx) + ZhongWen.Instance.shengqian_wei + ZhongWen.Instance.GetJobsName(gInfo.magic[3]);
			dialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
			
			state = 4;
			isTopJobLevel = true;
			
			return;
		}
		
		int jobLevel = 0;
		int jobIdx = 0;
		int step = 0;
		
		if (gInfo.level < 20) {
			jobIdx = gInfo.magic[0];
			jobLevel = 2;
			isTopJobLevel = true;
		} else {
			if (gInfo.magic[1] == -1) {
				jobLevel = 2;
				jobIdx = gInfo.magic[0];
				isTopJobLevel = false;
			} else {
				jobLevel = 3;
				jobIdx = gInfo.magic[1];
				if (gInfo.level < 40)
					isTopJobLevel = true;
				else 
					isTopJobLevel = false;
			}
		}
		
		if (jobIdx < 18) {
			jobIdx += 6;
			step = 3;
			
			for (int i=0; i<3; i++) {
				jobsName[i].GetComponent<exSpriteFont>().text = ZhongWen.Instance.GetJobsName(jobIdx);
				jobsIdx[i] = jobIdx;
				
				jobIdx+=step;
				if (jobIdx >= jobLevel * 6)
					jobIdx -= 5;
			}
			
		} else {
			jobIdx += 10;
			step = 5;
			
			for (int i=0; i<3; i++) {
				jobsName[i].GetComponent<exSpriteFont>().text = ZhongWen.Instance.GetJobsName(jobIdx);
				jobsIdx[i] = jobIdx;
				
				jobIdx+=step;
				if (jobIdx >= 19 + jobLevel * 10)
					jobIdx -= 9;
			}
			
		}
		
		state = 3;
		
		jobsCtrl.SetActive(true);
		jobsCtrl.transform.position = new Vector3(
			jobsCtrl.transform.position.x, generalsList.GetSelectItem().transform.position.y, jobsCtrl.transform.position.z);
		
		MenuDisplayAnim anim = jobsCtrl.GetComponent<MenuDisplayAnim>();
		
		Vector3 pos = anim.GetOriginalPosition();
		pos.y = generalsList.GetSelectItem().transform.position.y;
		anim.SetOriginalPosition(pos);
		
		anim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}
	
	void OnSelectJobModeHandler() {
		if (jobsCtrl.GetComponent<MenuDisplayAnim>().IsPlaying())	return;
		
		if (Misc.GetBack()) {
			state = 2;
			
			generalsList.enabled = true;
			jobsCtrl.SetActive(false);
			return;
		}
		
		for (int i=0; i<3; i++) {
			if (jobsName[i].GetButtonState() == Button.ButtonState.Clicked) {
				state = 4;
				
				jobsName[i].SetButtonState(Button.ButtonState.Normal);
				jobsCtrl.SetActive(false);
				
				int gIdx = (int)generalsList.GetSelectItem().GetItemData();
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
				
				if (gInfo.magic[1] == -1) {
					gInfo.magic[1] = jobsIdx[i];
					gInfo.job = jobsIdx[i];
				} else {
					gInfo.magic[2] = jobsIdx[i];
					gInfo.job = jobsIdx[i];
				}
				
				string msg = ZhongWen.Instance.shengqian_feng + ZhongWen.Instance.GetGeneralName(gIdx) + ZhongWen.Instance.shengqian_wei + ZhongWen.Instance.GetJobsName(gInfo.job);
				dialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
			}
		}
	}
	
	void OnKingDialogueModeHandler() {
		if (!dialogue.IsShowingText() && Input.GetMouseButtonUp(0)) {
			state = 5;
			
			Input.ResetInputAxes();
			
			dialogue.SetHeadIndex((int)generalsList.GetSelectItem().GetItemData());
			dialogue.SetText(ZhongWen.Instance.shengqian_answer);
		}
	}
	
	void OnGeneralDialogueModeHandler() {
		if (!dialogue.IsShowingText() && Input.GetMouseButtonUp(0)) {
			state = 2;
			
			Input.ResetInputAxes();
			
			if (isTopJobLevel)
				generalsList.GetSelectItem().SetSelectEnable(false);
			generalsList.enabled = true;
			
			dialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
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
	
	public bool AddGeneralsList() {
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		
		for (int i=0; i<kInfo.generals.Count; i++) {
			
			int gIdx = (int)kInfo.generals[i];
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
			if (gInfo.king == Controller.kingIndex) {
				if (gInfo.level < 10) continue;
				
				int idx = 0;
				if (gInfo.level < 30) {
					idx = gInfo.level / 10;
				} else if (gInfo.level >= 40) {
					idx = 3;
				}
				
				if (gInfo.magic[idx] == -1) {
					haveGeneral = true;
					
					generalsList.AddItem(ZhongWen.Instance.GetGeneralName1(gIdx)).SetItemData(gIdx);
				}
			}
		}
		
		if (!haveGeneral) {
			dialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx, ZhongWen.Instance.shengqian_no, MenuDisplayAnim.AnimType.InsertFromBottom);
		}
		
		return haveGeneral;
	}
}
