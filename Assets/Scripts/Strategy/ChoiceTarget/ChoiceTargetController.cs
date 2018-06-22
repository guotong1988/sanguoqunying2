using UnityEngine;
using System.Collections;

public class ChoiceTargetController : MonoBehaviour {
	
	public class TargetData {
		public int type; // 0:city 1:army
		public object data;
		
		public TargetData(int t, object d) {
			type = t;
			data = d;
		}
	}
	
	public StrategyController strCtrl;
	
	public CityCommandsController cityCmdCtrl;
	public ArmyCommandsController armyCmdCtrl;
	
	public ListController targetList;
	
	private int state;
	
	private MenuDisplayAnim menuAnim;
	private TargetData targetData;
	
	private float timeTick;
	
	// Use this for initialization
	void Awake () {
	
		targetList.SetSelectItemHandler(OnTargetItemClickHandler);
	}
	
	void OnDisable() {
		
		targetList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
	
		switch (state) {
		case 0:
			OnNormalMode();
			break;
		case 1:
			OnChangingToTargetCommandMode();
			
			break;
		case 2:
			OnReturnMainMode();
			break;
		}
	}
	
	void OnNormalMode() {
		if (Misc.GetBack()) {
			
			state = 2;
			menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		}
	}
	
	void OnChangingToTargetCommandMode() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			ChangeToTargetCommands();
		}
	}
	
	void OnReturnMainMode() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			strCtrl.ReturnMainMode();
		}
	}
	
	public void AddCityTarget(int city) {
		
		string cityName = ZhongWen.Instance.GetCityName(city);
		if (cityName.Length == 1) {
			cityName = "  " + cityName + "  " + "  " + ZhongWen.Instance.chengTarget;
		} else {
			cityName = cityName[0] + "  " + cityName[1] + "  " + ZhongWen.Instance.chengTarget;
		}
		
		TargetData data = new TargetData(0, city);
		ListItem item = targetList.AddItem(cityName);
		item.SetItemData(data);
	}
	
	public void AddArmyTarget(ArmyInfo armyInfo) {
		
		string generalName = ZhongWen.Instance.GetGeneralName1(armyInfo.commander);
		if (generalName.Length == 4 && generalName[1] != ' ' && generalName[2] != ' ') {
			generalName = generalName + ZhongWen.Instance.budui;
		} else {
			generalName = generalName + "  " + ZhongWen.Instance.budui;
		}
		
		TargetData data = new TargetData(1, armyInfo);
		ListItem item = targetList.AddItem(generalName);
		item.SetItemData(data);
	}
	
	public void Show() {
		
		if (targetList.GetCount() == 0) return;
		
		if (targetList.GetCount() == 1) {
			
			targetData = (TargetData)targetList.GetListItem(0).GetItemData();
			ChangeToTargetCommands();
			targetList.Clear();
			return;
		}
		
		state = 0;
		
		if (menuAnim == null) {
			menuAnim = GetComponent<MenuDisplayAnim>();
		}
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
		
		gameObject.SetActive(true);
	}
	
	void OnTargetItemClickHandler() {
		
		state = 1;
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		
		targetData = (TargetData)targetList.GetSelectItem().GetItemData();
	}
	
	void ChangeToTargetCommands() {
		
		if (targetData.type == 0) {
			
			cityCmdCtrl.SetCity((int)targetData.data);
		} else {
			
			armyCmdCtrl.SetArmyInfo((ArmyInfo)targetData.data);
		}
	}
}
