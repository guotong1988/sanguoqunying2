using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UOController : MonoBehaviour {
	
	private class CityObjectsData {
		public int city;
		public int code;
		
		public CityObjectsData(int c, int co) {
			city = c;
			code = co;
		}
	}
	
	public IAController IACtrl;
	public ListController generalsInfoList;
	public ListController objsList;
	public MenuDisplayAnim generalsMenuAnim;
	public MenuDisplayAnim objsMenuAnim;
	
	public GameObject confirmBox;
	public DialogueController dialogue;
	
	private int state = 0;
	private Button ok;
	private Button cancel;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		generalsInfoList.SetSelectChangeHandler(OnSelectGeneral);
		objsList.SetSelectItemHandler(OnSelectObject);
		
		ok = confirmBox.transform.Find("OK").GetComponent<Button>();
		cancel = confirmBox.transform.Find("Cancel").GetComponent<Button>();
		
		ok.SetButtonClickHandler(OnConfirmButtonOK);
		cancel.SetButtonClickHandler(OnConfirmButtonCancel);
	}
	
	void OnEnable() {
		// Test
		//Controller.kingIndex = 1;
		state = 0;
		
		AddGeneralsInfo();
		AddObjects();
		
		OnSelectGeneral();
		
		generalsMenuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
		objsMenuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}
	
	void OnDisable() {
		generalsInfoList.Clear();
		objsList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (state) {
		case 0:
			OnNormalHandler();
			break;
		case 1:
			OnConfirmModeHandler();
			break;
		case 2:
			OnDialogueShowingModeHandler();
			break;
		case 3:
			OnReturnMainHandler();
			break;
		}
	}
	
	void OnNormalHandler() {
		if (Misc.GetBack()) {
			state = 3;
			
			generalsMenuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
			objsMenuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		}
		
		if (generalsInfoList.enabled == false && objsList.enabled == false) {
			generalsInfoList.enabled = true;
			objsList.enabled = true;
		}
	}
	
	void OnConfirmModeHandler() {
		if (Misc.GetBack()) {
			OnConfirmButtonCancel();
		}
	}
	
	void OnDialogueShowingModeHandler() {
		if (!dialogue.IsShowingText()) {
			if (Input.GetMouseButtonUp(0)) {
				dialogue.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				state = 0;
			}
		}
	}
	
	void OnReturnMainHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			state = 0;
			timeTick = 0;
			
			gameObject.SetActive(false);
			IACtrl.OnReturnMain();
		}
	}
	
	void AddGeneralsInfo() {
		
		int kingIdx = Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx;
		
		string str = GetGeneralInfo(kingIdx);
		ListItem li = generalsInfoList.AddItem(str);
		li.SetItemData(kingIdx);
		
		generalsInfoList.SetItemSelected(0, true);
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		
		for (int i=0; i<kInfo.generals.Count; i++) {
			
			int gIdx = (int)kInfo.generals[i];
			
			if (gIdx != kingIdx) {
				li = generalsInfoList.AddItem(GetGeneralInfo(gIdx));
				li.SetItemData(gIdx);
			}
		}
	}
	
	string GetGeneralInfo(int idx) {
		string strInfo = "";
		string temp;
		string name;
		
		name = ZhongWen.Instance.GetGeneralName1(idx);
		temp = name;
		if (name.Length == 3 || (name.Length == 4 && name[1] == ' ')) {
			temp += "    ";
		} else if (name.Length == 4) {
			temp += "  ";
		}
		strInfo += temp;
		
		temp = "" + Informations.Instance.GetGeneralInfo(idx).strength;
		if (temp.Length == 2) temp = " " + temp;
		temp += "    ";
		strInfo += temp;
		
		temp = "" + Informations.Instance.GetGeneralInfo(idx).intellect;
		if (temp.Length == 2) temp = " " + temp;
		temp += " ";
		strInfo += temp;
		
		name = ZhongWen.Instance.GetEquipmentName(Informations.Instance.GetGeneralInfo(idx).equipment);
		temp = name;
		
		if (name == "") {
			temp = "            ";
		} else if (name.Length == 2) {
			temp = name[0].ToString() + "  " + name[1].ToString();
			temp = "    " + temp + "  ";
		} else if (name.Length == 3) {
			temp = "    " + temp + "  ";
		} else if (name.Length == 4) {
			temp = "   " + temp + " ";
		} else if (name.Length == 5){
			temp = "  " + name;
		}
		
		strInfo += temp;
		
		return strInfo;
	}
	
	void AddObjects() {
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		
		for (int i=0; i<kInfo.cities.Count; i++) {
			
			int cIdx = (int)kInfo.cities[i];
			List<int> obj = Informations.Instance.GetCityInfo(cIdx).objects;
			
			if (obj.Count > 0) {
				for (int j=0; j<obj.Count; j++) {
					string objName = "";
					int code = (int)obj[j];
					int idx = code & 0xFFFF;
					int type = code >> 16;
					switch (type) {
					case 1:
						objName = ZhongWen.Instance.GetEquipmentName(idx);
						break;
					case 2:
						objName = ZhongWen.Instance.GetArmsName(idx) + ZhongWen.Instance.bingfu;
						break;
					case 3:
						objName = ZhongWen.Instance.GetFormationName(idx) + ZhongWen.Instance.zhishu;
						break;
					}
					
					CityObjectsData data = new CityObjectsData(cIdx, code);
					
					ListItem li = objsList.AddItem(objName);
					li.SetItemData(data);
				}
			}
		}
	}
	
	void OnSelectGeneral() {
		if (state != 0)	return;
		
		int count = objsList.GetCount();
		int gIdx = (int)generalsInfoList.GetSelectItem().GetItemData();
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		for (int i=0; i<count; i++) {
			ListItem item = objsList.GetListItem(i);
			
			CityObjectsData cityData = (CityObjectsData)item.GetItemData();
			int data = cityData.code;
			if (data == -1) continue;
			
			int code = data & 0xFFFF;
			int type = data >> 16;
			bool flag = true;
			
			switch (type) {
			case 1:
				if (gInfo.equipment == code)		flag = false;
				break;
			case 2:
				if ((gInfo.arms & code) != 0)		flag = false;
				break;
			case 3:
				if ((gInfo.formation & code) != 0) 	flag = false;
				break;
			}

			item.SetSelectEnable(flag);
		}
		
		if (count > 0 && ((CityObjectsData)objsList.GetListItem(count-1).GetItemData()).code == -1) {
			if (gInfo.equipment == -1) 
				objsList.DeleteItem(count-1);
		} else {
			if (gInfo.equipment != -1) {
				
				ListItem li = objsList.AddItem(ZhongWen.Instance.xiexia);
				
				CityObjectsData data = new CityObjectsData(0, -1);
				li.SetItemData(data);
			}
		}
	}
	
	void OnSelectObject() {
		if (state != 0)	return;
		/*
		state = 1;
				
		confirmBox.SetActive(true);
		confirmBox.transform.position = new Vector3(
			confirmBox.transform.position.x, objsList.GetSelectItem().transform.position.y, confirmBox.transform.position.z);
		
		generalsInfoList.enabled = false;
		objsList.enabled = false;
		*/
		generalsInfoList.enabled = false;
		objsList.enabled = false;
		
		OnConfirmButton();
	}
	
	void OnConfirmButtonOK() {
		OnConfirmButton();
	}
	
	void OnConfirmButtonCancel() {
		
		state = 0;
		confirmBox.SetActive(false);
	}
	
	void OnConfirmButton() {
		
		confirmBox.SetActive(false);
		
		int gIdx = (int)generalsInfoList.GetSelectItem().GetItemData();
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		CityObjectsData cityData = (CityObjectsData)objsList.GetSelectItem().GetItemData();
		
		int data = cityData.code;
		
		if (data == -1) {
			state = 0;
			
			string objName = ZhongWen.Instance.GetEquipmentName(gInfo.equipment);
			int code = (1 << 16) + gInfo.equipment;
			
			List<int> o = Informations.Instance.GetCityInfo(gInfo.city).objects;
			o.Add(code);
			
			CityObjectsData d = new CityObjectsData(gInfo.city, code);
			
			objsList.AddItem(objName).SetItemData(d);
			
			
			EquipmentInfo eInfo = Informations.Instance.GetEquipmentInfo(gInfo.equipment);
			if (eInfo.type == 0) {
				gInfo.intellect -= eInfo.data;
			} else if (eInfo.type == 3) {
				gInfo.strength -= eInfo.data;
			}
			
			gInfo.equipment = -1;
			objsList.DeleteItem(objsList.GetSelectIndex());
		} else {
			List<int> objs = Informations.Instance.GetCityInfo(cityData.city).objects;
			
			int code = data & 0xFFFF;
			int type = data >> 16;
			
			int cObjIdx = 0;
			for (;cObjIdx<objs.Count; cObjIdx++) {
				if ((int)objs[cObjIdx] == cityData.code)
					break;
			}
			
			objs.RemoveAt(cObjIdx);
			
			switch (type) {
			case 1:
				state = 0;
				
				if (gInfo.equipment != -1) {
					string objName = ZhongWen.Instance.GetEquipmentName(gInfo.equipment);
					int d = (1 << 16) + gInfo.equipment;
					
					cityData.code = d;
					
					objsList.GetSelectItem().GetComponent<exSpriteFont>().text = objName;
					objsList.GetSelectItem().SetItemData(cityData);
					objsList.SetItemSelected(-1, false);
					
					objs.Insert(cObjIdx, d);
				} else {
					objsList.DeleteItem(objsList.GetSelectIndex());
					
				}
				
				ChangeGeneralsEquitment(gInfo, code);
				break;
			case 2:
				state = 2;
				
				gInfo.arms += code;
				objsList.DeleteItem(objsList.GetSelectIndex());
				
				dialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx,
					ZhongWen.Instance.GetGeneralName(gIdx) + ZhongWen.Instance.jublzh + ZhongWen.Instance.GetArmsName(code) + ZhongWen.Instance.denengli,
					MenuDisplayAnim.AnimType.InsertFromBottom);
				
				break;
			case 3:
				state = 2;
				
				gInfo.formation += code;
				objsList.DeleteItem(objsList.GetSelectIndex());
				
				dialogue.SetDialogue(Informations.Instance.GetKingInfo(Controller.kingIndex).generalIdx,
					ZhongWen.Instance.GetGeneralName(gIdx) + ZhongWen.Instance.xuehuile + ZhongWen.Instance.GetFormationName(code),
					MenuDisplayAnim.AnimType.InsertFromBottom);
				
				break;
			}
		}
		
		generalsInfoList.GetSelectItem().GetComponent<exSpriteFont>().text = GetGeneralInfo(gIdx);
		OnSelectGeneral();
	}
	
	void ChangeGeneralsEquitment(GeneralInfo gInfo, int idx) {
		if (gInfo.equipment != -1) {
			EquipmentInfo eInfo = Informations.Instance.GetEquipmentInfo(gInfo.equipment);
			switch (eInfo.type) {
			case 0:
				gInfo.intellect -= eInfo.data;
				gInfo.intellect = Mathf.Clamp(gInfo.intellect, 0, 999);
				break;
			case 1:
				gInfo.manaMax -= eInfo.data;
				gInfo.manaMax = Mathf.Clamp(gInfo.manaMax, 0, 999);
				gInfo.manaCur -= eInfo.data;
				gInfo.manaCur = Mathf.Clamp(gInfo.manaCur, 0, gInfo.manaMax);
				break;
			case 2:
				gInfo.healthMax -= eInfo.data;
				gInfo.healthMax = Mathf.Clamp(gInfo.healthMax, 0, 999);
				gInfo.healthCur -= eInfo.data;
				gInfo.healthCur = Mathf.Clamp(gInfo.healthCur, 0, gInfo.healthMax);
				break;
			case 3:
				gInfo.strength -= eInfo.data;
				gInfo.strength = Mathf.Clamp(gInfo.strength, 0, 999);
				break;
			}
			gInfo.equipment = -1;
		}
		
		if (idx >= 0 && idx < Informations.Instance.equipmentNum) {
			EquipmentInfo eInfo = Informations.Instance.GetEquipmentInfo(idx);
			gInfo.equipment = idx;
			
			switch (eInfo.type) {
			case 0:
				gInfo.intellect += eInfo.data;
				gInfo.intellect = Mathf.Clamp(gInfo.intellect, 0, 999);
				break;
			case 1:
				gInfo.manaMax += eInfo.data;
				gInfo.manaMax = Mathf.Clamp(gInfo.manaMax, 0, 999);
				gInfo.manaCur += eInfo.data;
				gInfo.manaCur = Mathf.Clamp(gInfo.manaCur, 0, gInfo.manaMax);
				break;
			case 2:
				gInfo.healthMax += eInfo.data;
				gInfo.healthMax = Mathf.Clamp(gInfo.healthMax, 0, 999);
				gInfo.healthCur += eInfo.data;
				gInfo.healthCur = Mathf.Clamp(gInfo.healthCur, 0, gInfo.healthMax);
				break;
			case 3:
				gInfo.strength += eInfo.data;
				gInfo.strength = Mathf.Clamp(gInfo.strength, 0, 999);
				break;
			}
		}
	}
}
