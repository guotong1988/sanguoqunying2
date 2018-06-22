using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using LitJson;

public class RecordHeadInfo {
	public int generalIdx;
	public int cityNum;
	public int generalNum;
	public int HistoryTime;
}

public class RecordController {

	public delegate void RecordCallback();

	private static RecordController mInstance;
	public static RecordController Instance {
		get {
			if (mInstance == null) {
				mInstance = new RecordController();
			}
			return mInstance;
		}
	}

	public RecordHeadInfo[] GetHeadInfo() {
		
		RecordHeadInfo[] headInfoList = new RecordHeadInfo[8];

		for (int i=0; i<8; i++) {
			string recordStr = "Record" + (i + 1);
			int recordExists = PlayerPrefs.GetInt(recordStr, 0);
			if (recordExists == 1) {

				RecordHeadInfo headInfoItem = new RecordHeadInfo();
				headInfoItem.generalIdx = PlayerPrefs.GetInt(recordStr + "GeneralIndex", -1);
				headInfoItem.cityNum = PlayerPrefs.GetInt(recordStr + "CitiesNum", -1);
				headInfoItem.generalNum = PlayerPrefs.GetInt(recordStr + "GeneralsNum", -1);
				headInfoItem.HistoryTime = PlayerPrefs.GetInt(recordStr + "HistoryTime", -1);
				headInfoList[i] = headInfoItem;
			}
		}

		return headInfoList;
	}

	public void LoadRecord(int index, RecordCallback recordCallback) {
		
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
		
		int idx = index;
		string filePath = path + "SANGO0" + (idx+1) + ".SAV.xml";
		if (!File.Exists(filePath)) {
			return;
		}

        Informations.Reset();

		XmlDocument xmlDoc = new XmlDocument();
		StreamReader sr = File.OpenText(filePath);
		xmlDoc.LoadXml(sr.ReadToEnd().Trim());
		XmlElement root = xmlDoc.DocumentElement;
        XmlElement node;

        node = (XmlElement)root.SelectSingleNode("Mod");
        if (node != null)
        {
            Controller.MODSelect = int.Parse(node.GetAttribute("Index"));
        }
        else
        {
            Controller.MODSelect = 1;
        }
        Informations.Instance.kingNum = Informations.Instance.modKingNum[Controller.MODSelect];

        node = (XmlElement)root.SelectSingleNode("HeadInfo");
		Controller.kingIndex 	= int.Parse(node.GetAttribute("SelectKing"));
		Controller.historyTime 	= int.Parse(node.GetAttribute("HistoryTime"));

		node = (XmlElement)root.SelectSingleNode("Misc");
		StrategyController.isFirstEnter = bool.Parse(node.GetAttribute("IsFirstEnter"));
		StrategyController.strategyCamPos = StringToVector3(node.GetAttribute("StrategyCamPos"));

		XmlNodeList nodeList = root.SelectNodes("KingsInfo");
		int i = 0;
		foreach (XmlElement kingNode in nodeList) {
			KingInfo kInfo = new KingInfo();
			kInfo.active 		= int.Parse(kingNode.GetAttribute("active"));
			kInfo.generalIdx 	= int.Parse(kingNode.GetAttribute("generalIdx"));
			Informations.Instance.SetKingInfo(i++, kInfo);
		}
		KingInfo k = new KingInfo();
		k.generalIdx = 0;
		Informations.Instance.SetKingInfo(Informations.Instance.kingNum, k);

		i = 0;
		nodeList = root.SelectNodes("CitiesInfo");
		foreach (XmlElement cityNode in nodeList) {
			CityInfo cInfo = new CityInfo();
			cInfo.king			= int.Parse(cityNode.GetAttribute("king"));
			cInfo.prefect		= int.Parse(cityNode.GetAttribute("prefect"));
			cInfo.population	= int.Parse(cityNode.GetAttribute("population"));
			cInfo.money			= int.Parse(cityNode.GetAttribute("money"));
			cInfo.reservist		= int.Parse(cityNode.GetAttribute("reservist"));
			cInfo.reservistMax	= int.Parse(cityNode.GetAttribute("reservistMax"));
			cInfo.defense		= int.Parse(cityNode.GetAttribute("defense"));

			if (cityNode.HasAttribute("objects")) {
				string str = cityNode.GetAttribute("objects");
				string[] objectsStr = str.Split(',');
				cInfo.objects = new List<int>();
				for (int j=0; j<objectsStr.Length; j++) {
					cInfo.objects.Add(int.Parse(objectsStr[j]));
				}
			}

			Informations.Instance.SetCityInfo(i++, cInfo);
		}

		i = 0;
		nodeList = root.SelectNodes("GeneralsInfo");
		foreach (XmlElement generalNode in nodeList) {
			GeneralInfo gInfo = new GeneralInfo();
			gInfo.active 		= int.Parse(generalNode.GetAttribute("active"));
			gInfo.king 			= int.Parse(generalNode.GetAttribute("king"));
			gInfo.city 			= int.Parse(generalNode.GetAttribute("city"));
			gInfo.prisonerIdx 	= int.Parse(generalNode.GetAttribute("prisonerIdx"));
			gInfo.loyalty 		= int.Parse(generalNode.GetAttribute("loyalty"));
			gInfo.job 			= int.Parse(generalNode.GetAttribute("job"));
			gInfo.equipment 	= int.Parse(generalNode.GetAttribute("equipment"));
			gInfo.strength 		= int.Parse(generalNode.GetAttribute("strength"));
			gInfo.intellect 	= int.Parse(generalNode.GetAttribute("intellect"));
			gInfo.experience 	= int.Parse(generalNode.GetAttribute("experience"));
			gInfo.level 		= int.Parse(generalNode.GetAttribute("level"));
			gInfo.healthMax 	= int.Parse(generalNode.GetAttribute("healthMax"));
			gInfo.healthCur 	= int.Parse(generalNode.GetAttribute("healthCur"));
			gInfo.manaMax 		= int.Parse(generalNode.GetAttribute("manaMax"));
			gInfo.manaCur 		= int.Parse(generalNode.GetAttribute("manaCur"));
			gInfo.soldierMax 	= int.Parse(generalNode.GetAttribute("soldierMax"));
			gInfo.soldierCur 	= int.Parse(generalNode.GetAttribute("soldierCur"));
			gInfo.knightMax 	= int.Parse(generalNode.GetAttribute("knightMax"));
			gInfo.knightCur 	= int.Parse(generalNode.GetAttribute("knightCur"));
			gInfo.arms 			= int.Parse(generalNode.GetAttribute("arms"));
			gInfo.armsCur 		= int.Parse(generalNode.GetAttribute("armsCur"));
			gInfo.formation 	= int.Parse(generalNode.GetAttribute("formation"));
			gInfo.formationCur 	= int.Parse(generalNode.GetAttribute("formationCur"));
			gInfo.escape 		= int.Parse(generalNode.GetAttribute("escape"));

			string[] magics = generalNode.GetAttribute("magic").Split(',');

			for (int m=0; m<4; m++) {
				gInfo.magic[m] = int.Parse(magics[m]);
			}
			
			Informations.Instance.SetGeneralInfo(i++, gInfo);
			
			//check
			gInfo.soldierCur = Mathf.Clamp(gInfo.soldierCur, 0, gInfo.soldierMax);
			gInfo.knightCur = Mathf.Clamp(gInfo.knightCur, 0, gInfo.knightMax);
		}

		nodeList = root.SelectNodes("ArmiesInfo");
		if (nodeList != null && nodeList.Count > 0) {
			i = 0;
			foreach (XmlElement armyNode in nodeList) {
				ArmyInfo armyInfo = new ArmyInfo();
				armyInfo.king 		= int.Parse(armyNode.GetAttribute("king"));
				armyInfo.cityFrom 	= int.Parse(armyNode.GetAttribute("cityFrom"));
				armyInfo.cityTo 	= int.Parse(armyNode.GetAttribute("cityTo"));
				armyInfo.commander 	= int.Parse(armyNode.GetAttribute("commander"));
				armyInfo.money 		= int.Parse(armyNode.GetAttribute("money"));
				armyInfo.state 		= int.Parse(armyNode.GetAttribute("state"));
				armyInfo.direction 	= int.Parse(armyNode.GetAttribute("direction"));
				armyInfo.isFlipped	= bool.Parse(armyNode.GetAttribute("isFlipped"));
				armyInfo.pos		= StringToVector3(armyNode.GetAttribute("pos"));
				armyInfo.timeTick 	= float.Parse(armyNode.GetAttribute("timeTick"));

				if (armyNode.HasAttribute("generals")) {
					string[] generals = armyNode.GetAttribute("generals").Split(',');
					for (int j=0; j<generals.Length; j++) {
						int temp = int.Parse(generals[j]);
						armyInfo.generals.Add(temp);
						//check
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(temp);
						gInfo.city = -1;
						gInfo.king = armyInfo.king;
						gInfo.prisonerIdx = -1;
					}
				}
				
				if (armyInfo.generals.Count == 0)
					continue;
				
				if (armyNode.HasAttribute("prisons")) {
					string[] prisons = armyNode.GetAttribute("prisons").Split(',');
					for (int j=0; j<prisons.Length; j++) {
						int temp = int.Parse(prisons[j]);
						armyInfo.prisons.Add(temp);
						//check
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(temp);
						gInfo.city = -1;
						gInfo.prisonerIdx = armyInfo.king;
					}
				}
				
				Informations.Instance.armys.Add(armyInfo);
			}
		}
		
		Informations.Instance.InitKingInfo();
		Informations.Instance.InitCityInfo();
		
		if (recordCallback != null) {
			recordCallback();
		}
	}

	Vector3 StringToVector3(string rString) {
		
		string[] strArray = rString.Substring(1, rString.Length-2).Split(',');
		float[] temp = Array.ConvertAll<string, float>(strArray, delegate(string s){ return float.Parse(s); });
		Vector3 rVector3 = new Vector3(temp[0], temp[1], temp[2]);
		
		return rVector3;
	}

	public void SaveRecord(int index, RecordCallback recordCallback) {

		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		string recordStr = "Record" + (index + 1);
		PlayerPrefs.SetInt(recordStr, 1);
		PlayerPrefs.SetInt(recordStr + "SelectKing", Controller.kingIndex);
		PlayerPrefs.SetInt(recordStr + "GeneralIndex", kInfo.generalIdx);
		PlayerPrefs.SetInt(recordStr + "CitiesNum", kInfo.cities.Count);
		PlayerPrefs.SetInt(recordStr + "GeneralsNum", kInfo.generals.Count);
		PlayerPrefs.SetInt(recordStr + "HistoryTime", Controller.historyTime);
        PlayerPrefs.Save();

		string path = "";
		if (Application.isEditor) {
			path = Application.dataPath + "/../SAVES/";
		} else {
			path = Application.persistentDataPath + "/SAVES/";
		}
		DirectoryInfo dir = new DirectoryInfo(path);
		if (!dir.Exists) {
			Directory.CreateDirectory(path);
		}
		
		path += "SANGO0" + (index+1) + ".SAV.xml";

		XmlDocument xmlDoc = new XmlDocument();
		XmlElement rootElement = xmlDoc.CreateElement("GameRecord");
		xmlDoc.AppendChild(rootElement);

        XmlElement node = xmlDoc.CreateElement("Mod");
        node.SetAttribute("Index", Controller.MODSelect.ToString());
        rootElement.AppendChild(node);

		node = xmlDoc.CreateElement("HeadInfo");
		node.SetAttribute("SelectKing", Controller.kingIndex.ToString());
		node.SetAttribute("GeneralIndex", kInfo.generalIdx.ToString());
		node.SetAttribute("CitiesNum", kInfo.cities.Count.ToString());
		node.SetAttribute("GeneralsNum", kInfo.generals.Count.ToString());
		node.SetAttribute("HistoryTime", Controller.historyTime.ToString());
		rootElement.AppendChild(node);
		
		node = xmlDoc.CreateElement("Misc");
		node.SetAttribute("IsFirstEnter", StrategyController.isFirstEnter.ToString());
		node.SetAttribute("StrategyCamPos", StrategyController.strategyCamPos.ToString());
		rootElement.AppendChild(node);

		for (int i=0; i<Informations.Instance.kingNum; i++) {

			node = xmlDoc.CreateElement("KingsInfo");
			kInfo = Informations.Instance.GetKingInfo(i);
			node.SetAttribute("active", kInfo.active.ToString());
			node.SetAttribute("generalIdx", kInfo.generalIdx.ToString());
			rootElement.AppendChild(node);
		}

		for (int i=0; i<Informations.Instance.cityNum; i++) {

			node = xmlDoc.CreateElement("CitiesInfo");
			CityInfo cInfo = Informations.Instance.GetCityInfo(i);

			node.SetAttribute("king", cInfo.king.ToString());
			node.SetAttribute("prefect", cInfo.prefect.ToString());
			node.SetAttribute("population", cInfo.population.ToString());
			node.SetAttribute("money", cInfo.money.ToString());
			node.SetAttribute("reservist", cInfo.reservist.ToString());
			node.SetAttribute("reservistMax", cInfo.reservistMax.ToString());
			node.SetAttribute("defense", cInfo.defense.ToString());

			List<string> objectStr = new List<string>();
			if (cInfo.objects != null && cInfo.objects.Count > 0) {
				for (int j=0; j<cInfo.objects.Count; j++) {
					objectStr.Add(cInfo.objects[j].ToString());
				}
				node.SetAttribute("objects", string.Join(",", objectStr.ToArray()));
			}
			
			rootElement.AppendChild(node);
		}

		for (int i=0; i<Informations.Instance.generalNum; i++) {

			node = xmlDoc.CreateElement("GeneralsInfo");
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(i);

			node.SetAttribute("active", gInfo.active.ToString());
			node.SetAttribute("king", gInfo.king.ToString());
			node.SetAttribute("city", gInfo.city.ToString());
			node.SetAttribute("prisonerIdx", gInfo.prisonerIdx.ToString());
			node.SetAttribute("loyalty", gInfo.loyalty.ToString());
			node.SetAttribute("job", gInfo.job.ToString());
			node.SetAttribute("equipment", gInfo.equipment.ToString());
			node.SetAttribute("strength", gInfo.strength.ToString());
			node.SetAttribute("intellect", gInfo.intellect.ToString());
			node.SetAttribute("experience", gInfo.experience.ToString());
			node.SetAttribute("level", gInfo.level.ToString());
			node.SetAttribute("healthMax", gInfo.healthMax.ToString());
			node.SetAttribute("healthCur", gInfo.healthCur.ToString());
			node.SetAttribute("manaMax", gInfo.manaMax.ToString());
			node.SetAttribute("manaCur", gInfo.manaCur.ToString());
			node.SetAttribute("soldierMax", gInfo.soldierMax.ToString());
			node.SetAttribute("soldierCur", gInfo.soldierCur.ToString());
			node.SetAttribute("knightMax", gInfo.knightMax.ToString());
			node.SetAttribute("knightCur", gInfo.knightCur.ToString());
			node.SetAttribute("arms", gInfo.arms.ToString());
			node.SetAttribute("armsCur", gInfo.armsCur.ToString());
			node.SetAttribute("formation", gInfo.formation.ToString());
			node.SetAttribute("formationCur", gInfo.formationCur.ToString());
			node.SetAttribute("escape", gInfo.escape.ToString());

			List<string> magicStr = new List<string>();
			for (int m=0; m<4; m++) {
				magicStr.Add(gInfo.magic[m].ToString());
			}
			node.SetAttribute("magic", string.Join(",", magicStr.ToArray()));
			
			rootElement.AppendChild(node);
		}
		
		if (Informations.Instance.armys.Count > 0) {

			for (int i=0; i<Informations.Instance.armys.Count; i++) {
				node = xmlDoc.CreateElement("ArmiesInfo");

				ArmyInfo armyInfo = Informations.Instance.armys[i];
				node.SetAttribute("king", armyInfo.king.ToString());
				node.SetAttribute("cityFrom", armyInfo.cityFrom.ToString());
				node.SetAttribute("cityTo", armyInfo.cityTo.ToString());
				node.SetAttribute("commander", armyInfo.commander.ToString());
				node.SetAttribute("money", armyInfo.money.ToString());
				node.SetAttribute("state", armyInfo.state.ToString());
				node.SetAttribute("direction", armyInfo.direction.ToString());
				node.SetAttribute("isFlipped", armyInfo.isFlipped.ToString());
				node.SetAttribute("pos", armyInfo.pos.ToString());
				node.SetAttribute("timeTick", armyInfo.timeTick.ToString());
				
				List<string> generalStr = new List<string>();
				for (int j=0; j<armyInfo.generals.Count; j++) {
					generalStr.Add(armyInfo.generals[j].ToString());
				}
				node.SetAttribute("generals", string.Join(",", generalStr.ToArray()));

				if (armyInfo.prisons.Count > 0) {
					List<string> prisonStr = new List<string>();
					for (int j=0; j<armyInfo.prisons.Count; j++) {
						prisonStr.Add(armyInfo.prisons[j].ToString());
					}
					node.SetAttribute("prisons", string.Join(",", prisonStr.ToArray()));
				}

				rootElement.AppendChild(node);
			}
		}
		
		xmlDoc.Save(path);
		
		if (recordCallback != null) {
			recordCallback();
		}
	}
}


