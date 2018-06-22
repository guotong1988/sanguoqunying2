using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;

public class SavingController : MonoBehaviour {
	
	public IAController IACtrl;
	
	public MenuDisplayAnim savingCtrl;
	public GameObject confirmBox;
	public Button okButton;
	public Button cancelButton;
	public Button[] records;
	
	private int state;
	private int index;
	private float timeTick;
	private bool isSavingFile;
	private bool isClicked;
	
	// Use this for initialization
	void Start () {
		
		okButton.SetButtonClickHandler(OnOkButton);
		cancelButton.SetButtonClickHandler(OnCancelButton);
		
		for (int i=0; i<records.Length; i++) {
			records[i].SetButtonData(i);
			records[i].SetButtonClickHandler(OnRecordsButton);
		}
		
		GetHeadInfo();
	}
	
	void OnEnable() {
		savingCtrl.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (state) {
		case 0:
			if (Misc.GetBack()) {
				state = 1000;
				savingCtrl.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
				Invoke("OnReturnMain", 0.2f);
			}
			break;
		case 1:
		{
			if (Misc.GetBack()) {
				OnCancelButton();
			}
		}
			break;
		case 2:
		{
			if (!isSavingFile) {
				
				state = 0;
				confirmBox.SetActive(false);
				
				for (int i=0; i<records.Length; i++) {
					records[i].enabled = true;
				}
				
				exSpriteFont font = records[index].GetComponent<exSpriteFont>();
				font.botColor = new Color(1, 1, 1, 1);
				font.topColor = new Color(1, 1, 1, 1);
			}
		}
			break;
		}
	}
	
	void LateUpdate () {
		if (isClicked) {
			isClicked = false;
			
			exSpriteFont font = records[index].GetComponent<exSpriteFont>();
			font.botColor = new Color(1, 0, 0, 1);
			font.topColor = new Color(1, 0, 0, 1);
		}
	}
	
	void OnReturnMain() {
		state = 0;
				
		gameObject.SetActive(false);
		IACtrl.OnReturnMain();
	}
	
	void OnOkButton() {
		
		state = 2;
		isSavingFile = true;
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		string str = ZhongWen.Instance.jindu + ZhongWen.Instance.shuzi[index] + "  ";
		str += ZhongWen.Instance.GetGeneralName1(kInfo.generalIdx) + "  ";
		str += ZhongWen.Instance.chengshu;
		if (kInfo.cities.Count < 10) {
			str += "0" + kInfo.cities.Count + "  ";
		} else {
			str += kInfo.cities.Count + "  ";
		}
		str += ZhongWen.Instance.wujiang;
		if (kInfo.generals.Count < 10) {
			str += "00" + kInfo.generals.Count + "  ";
		} else if (kInfo.generals.Count < 100) {
			str += "0" + kInfo.generals.Count + "  ";
		} else {
			str += kInfo.generals.Count + "  ";
		}
		str += ZhongWen.Instance.shijian + Controller.historyTime + ZhongWen.Instance.nian;
		
		records[index].GetComponent<exSpriteFont>().text = str;
		
		StartCoroutine(SaveFile());
	}
	
	void OnCancelButton() {
		
		exSpriteFont font = records[index].GetComponent<exSpriteFont>();
		font.botColor = new Color(1, 1, 1, 1);
		font.topColor = new Color(1, 1, 1, 1);
		
		state = 0;
		index = -1;
		confirmBox.SetActive(false);
		
		for (int i=0; i<records.Length; i++) {
			records[i].enabled = true;
		}
	}
	
	void OnRecordsButton(object data) {
		
		state = 1;
		isClicked = true;
		index = (int)data;
		for (int i=0; i<records.Length; i++) {
			records[i].enabled = false;
		}
		
		exSpriteFont font = records[index].GetComponent<exSpriteFont>();
		if (font.text.Length > 10) {
			confirmBox.SetActive(true);
		} else {
			OnOkButton();
		}
	}

	void GetHeadInfo() {
		
		RecordHeadInfo[] headInfoList = RecordController.Instance.GetHeadInfo();
		for (int i=0; i<8; i++) {
			if (headInfoList[i] != null) {
				string str = ZhongWen.Instance.jindu + ZhongWen.Instance.shuzi[i] + "  ";
				str += ZhongWen.Instance.GetGeneralName1(headInfoList[i].generalIdx) + "  ";
				str += ZhongWen.Instance.chengshu;
				int citiesNum = headInfoList[i].cityNum;
				if (citiesNum < 10) {
					str += "0" + citiesNum + "  ";
				} else {
					str += citiesNum + "  ";
				}
				str += ZhongWen.Instance.wujiang;
				int generalsNum = headInfoList[i].generalNum;
				if (generalsNum < 10) {
					str += "00" + generalsNum + "  ";
				} else if (generalsNum < 100) {
					str += "0" + generalsNum + "  ";
				} else {
					str += generalsNum + "  ";
				}
				str += ZhongWen.Instance.shijian + headInfoList[i].HistoryTime + ZhongWen.Instance.nian;
				
				records[i].GetComponent<exSpriteFont>().text = str;
			}
		}
	}

	void GetHeadInfo1() {
		
		string path = "";
		if (Application.isEditor) {
			path = Application.dataPath + "/../SAVES/";
		} else {
			path = Application.persistentDataPath + "/SAVES/";
		}
		DirectoryInfo dir = new DirectoryInfo(path);
		if (!dir.Exists) {
			return;
		}
		
		for (int i=0; i<8; i++) {
			
			string filePath = path + "SANGO0" + (i+1) + ".SAV";
			if (File.Exists(filePath)) {
				
				StreamReader sr = File.OpenText(filePath);
				string s = sr.ReadToEnd();
				JsonData jData = JsonMapper.ToObject(s);
				
				string str = ZhongWen.Instance.jindu + ZhongWen.Instance.shuzi[i] + "  ";
				str += ZhongWen.Instance.GetGeneralName1((int)jData["HeadInfo"]["GeneralIndex"]) + "  ";
				str += ZhongWen.Instance.chengshu;
				int citiesNum = (int)jData["HeadInfo"]["CitiesNum"];
				if (citiesNum < 10) {
					str += "0" + citiesNum + "  ";
				} else {
					str += citiesNum + "  ";
				}
				str += ZhongWen.Instance.wujiang;
				int generalsNum = (int)jData["HeadInfo"]["GeneralsNum"];
				if (generalsNum < 10) {
					str += "00" + generalsNum + "  ";
				} else if (generalsNum < 100) {
					str += "0" + generalsNum + "  ";
				} else {
					str += generalsNum + "  ";
				}
				str += ZhongWen.Instance.shijian + jData["HeadInfo"]["HistoryTime"] + ZhongWen.Instance.nian;
				
				records[i].GetComponent<exSpriteFont>().text = str;
				
				sr.Close();
			}
		}
	}

	void OnRecordOver() {
		isSavingFile = false;
	}

	IEnumerator SaveFile() {

		RecordController.Instance.SaveRecord(index, OnRecordOver);
		
		yield return new WaitForSeconds(0.01f);
	}
}
