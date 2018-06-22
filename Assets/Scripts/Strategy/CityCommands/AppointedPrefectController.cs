using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppointedPrefectController : MonoBehaviour {
	
	public CityCommandsController ccCtrl;
	public ExpeditionController expeditionCtrl;
	public GameObject generalPrefab;
	
	private int state = 0;
	private int mode = 0;
	
	private List<GameObject> generalsList = new List<GameObject>();
	private int cityIdx = -1;
	
	private float timeTick;
	
	private Vector3 topItemPos = new Vector3(-250, 105, 0);
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable() {
		
		state = 0;
		GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}
	
	void OnDisable() {
		
		for (int i=0; i<generalsList.Count; i++) {
			Destroy((GameObject)generalsList[i]);
		}
		
		generalsList.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			OnNormalMode();
			break;
		case 1:
			OnReturnWithoutSelected();
			break;
		case 2:
			OnReturnWithSelected();
			break;
		}
	}
	
	void OnNormalMode() {
		
		if (Misc.GetBack()) {
			
			state = 1;
			GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		}
	}
	
	void OnReturnWithoutSelected() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			if (mode == 0) {
				ccCtrl.gameObject.SetActive(true);
			} else {
				expeditionCtrl.OnReturnWithoutAppointedPrefect();
			}
		}
	}
	
	void OnReturnWithSelected() {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			
			gameObject.SetActive(false);
			
			if (mode == 0) {
				ccCtrl.OnAppointedPrefectMode();
			} else {
				expeditionCtrl.OnReturnWithAppointedPrefect();
			}
		}
	}
	
	public void SetExpeditionMode(int cIdx) {
		
		mode = 1;
		cityIdx = cIdx;
		gameObject.SetActive(true);
	}
	
	public void AddGeneralsList(int cIdx) {
		
		mode = 0;
		cityIdx = cIdx;
		gameObject.SetActive(true);
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		for (int i=0; i<cInfo.generals.Count; i++) {
			
			int gIdx = (int)cInfo.generals[i];
			AddGeneralList(gIdx, i);
		}
	}
	
	public void AddGeneralList(int gIdx, int i) {
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
		GameObject go = (GameObject)Instantiate(generalPrefab);
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(topItemPos.x, topItemPos.y - 30 * i, topItemPos.z);
		
		generalsList.Add(go);
		
		Button button = go.GetComponent<Button>();
		button.SetButtonClickHandler(OnGeneralSelectHandler);
		button.SetButtonData(gIdx);
		
		exSpriteFont font = go.GetComponent<exSpriteFont>();
		
		string str = "";
		string temp;
		
		temp = ZhongWen.Instance.GetGeneralName1(gIdx);
		if (temp.Length == 4 && temp[1] != ' ' && temp[2] != ' ') {
			
			temp += " ";
		} else {
			temp += "   ";
		}
		
		str += temp;
		
		temp = ZhongWen.Instance.dengji + gInfo.level;
		if (gInfo.level < 10) {
			temp += "   ";
		} else {
			temp += "  ";
		}
		
		str += temp;
		
		temp = ZhongWen.Instance.wuli + gInfo.strength;
		if (gInfo.strength < 10) {
			temp += "    ";
		} else if (gInfo.strength < 100) {
			temp += "   ";
		} else {
			temp += "  ";
		}
		
		str += temp;
		
		temp = ZhongWen.Instance.zhili + gInfo.intellect;
		if (gInfo.intellect < 10) {
			temp += "    ";
		} else if (gInfo.intellect < 100) {
			temp += "   ";
		} else {
			temp += "  ";
		}
		
		str += temp;
		
		temp = ZhongWen.Instance.GetArmsName(gInfo.armsCur) + (gInfo.soldierCur + gInfo.knightCur) + ZhongWen.Instance.ren;
		
		str += temp;
		
		font.text = str;
		font.anchor = exPlane.Anchor.MidLeft;
	}
	
	void OnGeneralSelectHandler(object data) {
		
		state = 2;
		Informations.Instance.GetCityInfo(cityIdx).prefect = (int)data;
		GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
	}
}
