using UnityEngine;
using System.Collections;

public class SyPowerMap : MonoBehaviour {
	
	public StrategyController strCtrl;
	
	public ListController kingList;
	public MapController map;
	public KingInfoController kingInfo;
	
	private int state = 0;
	private float timeTick = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable() {
		
		AddKingList();
		kingList.SetSelectItemHandler(OnSelectKingHandler);
		kingList.SetItemSelected(0, true);
		
		kingList.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		map		.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
		kingInfo.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromBottom);
		
		map.gameObject.SetActive(true);
	}
	
	void OnDisable() {
		kingList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			OnNormal();
			break;
		case 1:
			OnReturnMain();
			break;
		}
	}
	
	void OnNormal() {
		if (Misc.GetBack()) {
			state = 1;
			
			kingList.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
			map		.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToRight);
			kingInfo.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToBottom);
		}
	}
	
	void OnReturnMain() {
		timeTick += Time.deltaTime;
		if (timeTick < 0.2f) return;
		
		state = 0;
		timeTick = 0;
		
		gameObject.SetActive(false);
		map.gameObject.SetActive(false);
		
		strCtrl.ReturnMainMode();
	}
	
	void AddKingList() {
		for (int i=0; i<Informations.Instance.kingNum; i++) {
			if (Informations.Instance.GetKingInfo(i).active == 1) {
				kingList.AddItem(ZhongWen.Instance.GetGeneralName1(Informations.Instance.GetKingInfo(i).generalIdx)).SetItemData(i);
			}
		}
	}
	
	void OnSelectKingHandler() {
		int kIdx = (int)kingList.GetSelectItem().GetItemData();
		
		kingInfo.SetKing(kIdx);
		
		map.ClearSelect();
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(kIdx);
		
		for (int i=0; i<kInfo.cities.Count; i++) {
			map.SelectCity((int)kInfo.cities[i]);
		}
	}
}
