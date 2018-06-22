using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class ContinueGame : MonoBehaviour {

	public static bool isCheatMaker;

	public Button[] records;
	
	// Use this for initialization
	void Start () {
		
		GetHeadInfo();
		
		for (int i=0; i<records.Length; i++) {
			if (records[i].GetComponent<exSpriteFont>().text.Length > 10) {
				records[i].SetButtonData(i);
				records[i].SetButtonClickHandler(OnRecordsButton);
			} else {
				records[i].SetButtonEnable(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Misc.GetBack()) {
			Application.LoadLevel(0);
			GameObject.Destroy(GameObject.Find("MouseTrack"));
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

	void OnReadRecordOver() {

		if (!isCheatMaker) {
			Misc.LoadLevel("InternalAffairs");
		} else {
			Misc.LoadLevel("CheatMaker");
		}
	}

	void OnRecordsButton(object data) {

		if (isCheatMaker) {
			CheatMakerController.Instance.recordIndex = (int)data;
		}
		RecordController.Instance.LoadRecord((int)data, OnReadRecordOver);
	}
}
