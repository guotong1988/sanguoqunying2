using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAOverController : MonoBehaviour {
	
	public IAController IACtrl;
	
	public Button OKButton;
	public Button CancelButton;
	
	// Use this for initialization
	void Start () {
		OKButton.SetButtonClickHandler(OnOKButtonHandler);
		CancelButton.SetButtonClickHandler(OnCancelButtonHandler);
	}
	
	// Update is called once per frame
	void Update () {
		if (Misc.GetBack()) {
			OnCancelButtonHandler();
		}
	}
	
	void OnOKButtonHandler() {
		gameObject.SetActive(false);
		if (!StrategyController.isFirstEnter) {
			OnIAOver();
		}
		Misc.LoadLevel("Strategy");
	}
	
	void OnCancelButtonHandler() {
		gameObject.SetActive(false);
		
		IACtrl.ResetState();
	}
	
	void OnIAOver() {
		
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			
			CityInfo cInfo = Informations.Instance.GetCityInfo(i);
			
			if (cInfo.king == -1 
				|| cInfo.king == Controller.kingIndex
				|| cInfo.king == Informations.Instance.kingNum) {
				continue;
			}
			
			for (int j=0; j<cInfo.generals.Count; j++) {
				
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(cInfo.generals[j]);
				
				if (gInfo.strength >= 85 || gInfo.intellect >= 85) {
					OnSearching(i);
				} else if (gInfo.strength > 80) {
					OnFortification(i, cInfo.generals[j]);
				} else if (gInfo.intellect > 80) {
					OnDeveloping(i, cInfo.generals[j]);
				}
			}
			
			for (int j=0; j<cInfo.generals.Count; j++) {
				
				if (cInfo.objects.Count > 0) {
					UsingObject(cInfo.generals[j], cInfo.objects);
				} else {
					break;
				}
			}
			
			SurrenderPrisoners(i);
		}
		
		for (int i=0; i<Informations.Instance.generalNum; i++) {
			
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(i);
			
			if (gInfo.prisonerIdx != -1) {
				gInfo.loyalty -= Random.Range(5, 20);
			}

			if (gInfo.king != -1 && gInfo.king != Controller.kingIndex && gInfo.prisonerIdx == -1) {
				GeneralsPromotion(gInfo);
			}
		}
	}
	
	void AddGeneralToCity(int gIdx, int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
		gInfo.active = 0;
		gInfo.king = cInfo.king;
		gInfo.city = cIdx;
		gInfo.loyalty = 90;
		gInfo.prisonerIdx = -1;
		
		int levelSum = 0;
		for (int i=0; i<cInfo.generals.Count; i++) {
			levelSum += Informations.Instance.GetGeneralInfo(cInfo.generals[i]).level;
		}
		int level = levelSum / cInfo.generals.Count;
		gInfo.experience = Misc.GetLevelExperience(level);
		Misc.CheckIsLevelUp(gInfo);
		
		cInfo.generals.Add(gIdx);
		Informations.Instance.GetKingInfo(cInfo.king).generals.Add(gIdx);
	}
	
	void OnSearching(int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		
		int findWhat = 0;
		
		if (Random.Range(0, 100) > 50) {
			findWhat = 1;
		} else {
			findWhat = Random.Range(0, 100) / 20;
		}
		
		switch (findWhat) {
		case 1:
		{
			List<int> list = new List<int>();
			for (int i=0; i<Informations.Instance.generalNum; i++) {
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(i);
				if (gInfo.king == -1) {
					list.Add(i);
				}
			}
			
			if (list.Count == 0) {
				break;
			}
			
			int findGeneralIdx = (int)list[Random.Range(0, list.Count)];
			AddGeneralToCity(findGeneralIdx, cIdx);
		}
			break;
		case 2:
		{
			if (cInfo.objects.Count >= 5) {
				break;
			}
			
			int equipment = Random.Range(0, Informations.Instance.equipmentNum);
			
			int code = (1 << 16) + equipment;
			cInfo.objects.Add(code);
		}
			break;
		case 3:
		{
			if (cInfo.objects.Count >= 5) {
				break;
			}
			
			int arms = 1 << Random.Range(0, Informations.Instance.armsNum);
			
			int code = (2 << 16) + arms;
			cInfo.objects.Add(code);
		}
			break;
		case 4:
		{
			if (cInfo.objects.Count >= 5) {
				break;
			}
			
			int formation = 1 << Random.Range(0, Informations.Instance.formationNum);
			
			int code = (3 << 16) + formation;
			cInfo.objects.Add(code);
		}
			break;
		}
	}
	
	void OnDeveloping(int cIdx, int gIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		int populationAdd = 9000 + Random.Range(0, 1000) + Random.Range(0, (gInfo.intellect - 80) * 100);
		cInfo.population += populationAdd;
		cInfo.population = Mathf.Clamp(cInfo.population, cInfo.population, 999999);
	}
	
	void OnFortification(int cIdx, int gIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		int defenseAdd = 30 + Random.Range(0, 10) + Random.Range(0, gInfo.strength - 80);
		cInfo.defense += defenseAdd;
		cInfo.defense = Mathf.Clamp(cInfo.defense, cInfo.defense, 9999);
	}
	
	void UsingObject(int gIdx, List<int> objects) {
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		int idx  = Random.Range(0, objects.Count);
		int data = objects[idx];
		int code = data & 0xFFFF;
		int type = data >> 16;
		
		switch (type) {
		case 1:
		{
			int equBK = gInfo.equipment;
			
			if (ChangeGeneralsEquitment(gInfo, code)) {
				objects.RemoveAt(idx);
				
				if (equBK != -1) {
					
					int d = (1 << 16) + equBK;
					objects.Insert(idx, d);
				}
			}
			break;
		}
		case 2:
			if ((gInfo.arms & code) == 0) {
				gInfo.arms += code;
				objects.RemoveAt(idx);
			}
			break;
		case 3:
			if ((gInfo.formation & code) == 0) {
				gInfo.formation += code;
				objects.RemoveAt(idx);
			}
			break;
		}
	}
	
	bool ChangeGeneralsEquitment(GeneralInfo gInfo, int idx) {
		
		EquipmentInfo eInfo1 = Informations.Instance.GetEquipmentInfo(idx);
		
		if (gInfo.intellect > 70 && gInfo.intellect - gInfo.strength >= 10 && eInfo1.type == 3) {
			return false;
		} else if (gInfo.strength > 70 && gInfo.strength - gInfo.intellect >= 10 && eInfo1.type == 0) {
			return false;
		}
		
		if (gInfo.equipment != -1) {
			EquipmentInfo eInfo2 = Informations.Instance.GetEquipmentInfo(gInfo.equipment);
			
			if (eInfo1.type == eInfo2.type) {
				if (eInfo1.data <= eInfo2.data)
					return false;
			} else {
				if (gInfo.intellect > 70 && gInfo.intellect - gInfo.strength >= 10 && eInfo1.type != 0) {
					return false;
				} else if (gInfo.strength > 70 && gInfo.strength - gInfo.intellect >= 10 && eInfo1.type != 3) {
					return false;
				}
			}
			
			switch (eInfo2.type) {
			case 0:
				gInfo.intellect -= eInfo2.data;
				gInfo.intellect = Mathf.Clamp(gInfo.intellect, 0, 999);
				break;
			case 1:
				gInfo.manaMax -= eInfo2.data;
				gInfo.manaMax = Mathf.Clamp(gInfo.manaMax, 0, 999);
				gInfo.manaCur -= eInfo2.data;
				gInfo.manaCur = Mathf.Clamp(gInfo.manaCur, 0, gInfo.manaMax);
				break;
			case 2:
				gInfo.healthMax -= eInfo2.data;
				gInfo.healthMax = Mathf.Clamp(gInfo.healthMax, 0, 999);
				gInfo.healthCur -= eInfo2.data;
				gInfo.healthCur = Mathf.Clamp(gInfo.healthCur, 0, gInfo.healthMax);
				break;
			case 3:
				gInfo.strength -= eInfo2.data;
				gInfo.strength = Mathf.Clamp(gInfo.strength, 0, 999);
				break;
			}
		}
		
		gInfo.equipment = idx;
			
		switch (eInfo1.type) {
		case 0:
			gInfo.intellect += eInfo1.data;
			gInfo.intellect = Mathf.Clamp(gInfo.intellect, 0, 999);
			break;
		case 1:
			gInfo.manaMax += eInfo1.data;
			gInfo.manaMax = Mathf.Clamp(gInfo.manaMax, 0, 999);
			gInfo.manaCur += eInfo1.data;
			gInfo.manaCur = Mathf.Clamp(gInfo.manaCur, 0, gInfo.manaMax);
			break;
		case 2:
			gInfo.healthMax += eInfo1.data;
			gInfo.healthMax = Mathf.Clamp(gInfo.healthMax, 0, 999);
			gInfo.healthCur += eInfo1.data;
			gInfo.healthCur = Mathf.Clamp(gInfo.healthCur, 0, gInfo.healthMax);
			break;
		case 3:
			gInfo.strength += eInfo1.data;
			gInfo.strength = Mathf.Clamp(gInfo.strength, 0, 999);
			break;
		}
		
		return true;
	}
	
	void SurrenderPrisoners(int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		for (int i=0; i<cInfo.prisons.Count; i++) {
			
			bool isSuccess = false;
			int gIdx = cInfo.prisons[i];
			
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
			
			if (gInfo.king == cInfo.king) {
				
				isSuccess = true;
			} else if (Random.Range(0, 100) > gInfo.loyalty) {
				
				isSuccess = true;
			} else {
				
				gInfo.loyalty -= Random.Range(5, 20);
				gInfo.loyalty = Mathf.Clamp(gInfo.loyalty, 0, 100);
			}
			
			if (isSuccess) {
				
				gInfo.city = cIdx;
				gInfo.loyalty = 90;
				gInfo.king = cInfo.king;
				gInfo.prisonerIdx = -1;
				
				cInfo.prisons.Remove(gIdx);
				
				cInfo.generals.Add(gIdx);
				Informations.Instance.GetKingInfo(cInfo.king).generals.Add(gIdx);
			}
		}
	}
	
	void GeneralsPromotion(GeneralInfo gInfo) {
		
		if (gInfo.level < 10) {
			return;
		} else if (gInfo.level < 30) {
			if (gInfo.magic[gInfo.level/10] != -1) {
				return;
			}
		} else if (gInfo.level >= 40) {
			if (gInfo.magic[3] != -1) {
				return;
			} else if (gInfo.magic[2] != -1) {
				if (gInfo.magic[2] < 18) {
					gInfo.magic[3] = 18;
				} else {
					gInfo.magic[3] = 49;
				}
				gInfo.job = gInfo.magic[3];
				return;
			}
		}
		
		int jobLevel = 0;
		int jobIdx = 0;
		int step = 0;
		bool isTopJobLevel = false;
		int[] jobsIdx = new int[3];
		
		while (isTopJobLevel == false) {
			
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
					isTopJobLevel = true;
				}
			}
			
			if (jobIdx < 18) {
				jobIdx += 6;
				step = 3;
				
				for (int i=0; i<3; i++) {
					jobsIdx[i] = jobIdx;
					
					jobIdx+=step;
					if (jobIdx >= jobLevel * 6)
						jobIdx -= 5;
				}
				
			} else {
				jobIdx += 10;
				step = 5;
				
				for (int i=0; i<3; i++) {
					jobsIdx[i] = jobIdx;
					
					jobIdx+=step;
					if (jobIdx >= 19 + jobLevel * 10)
						jobIdx -= 9;
				}
				
			}
			
			int job = jobsIdx[Random.Range(0, 3)];
			gInfo.magic[jobLevel-1] = job;
			gInfo.job = job;
		}
	}
}
