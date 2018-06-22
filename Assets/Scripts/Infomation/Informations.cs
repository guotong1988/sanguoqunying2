using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class KingInfo {
	public int active = 1;
	public int generalIdx;
	public List<int> cities;
	public List<int> generals;
}

public class CityInfo {
	public int king;
	public int prefect;
	public int population;
	public int money;
	public int reservist;
	public int reservistMax;
	public int defense;
	public List<int> objects;
	public List<int> generals;
	public List<int> prisons;
	
	public int soldiersNum {
		
		get {
			
			int s = 0;
			for (int i=0; i<generals.Count; i++) {
				
				int gIdx = (int)generals[i];
				s += Informations.Instance.GetGeneralInfo(gIdx).soldierCur;
			}
			
			return s;
		}
	}
}

public class GeneralInfo {
	public int active = 1;
	public int king = -1;
	public int city = -1;
	public int prisonerIdx = -1;
	public int loyalty = 100;
	public int[] magic = new int[4];
	public int job;
	public int equipment;
	public int strength;
	public int intellect;
	public int experience;
	public int level;
	public int healthMax;
	public int healthCur;
	public int manaMax;
	public int manaCur;
	public int soldierMax;
	public int soldierCur;
	public int knightMax;
	public int knightCur;
	public int arms;
	public int armsCur;
	public int formation;
	public int formationCur;
	public int escape;
}

public class EquipmentInfo {
	public int type;
	public int data;
}

public class ArmyInfo {
	public int king = -1;
	public int cityFrom = -1;
	public int cityTo = -1;
	public int commander = -1;
	public int money = 0;
	
	public int state;
	public int direction;
	public bool isFlipped;
	public Vector3 pos;
	public float timeTick;
	
	public ArmyController armyCtrl;
	
	public List<int> generals = new List<int>();
	public List<int> prisons = new List<int>();
}

public class Informations {
	
	private KingInfo[] kings;
	private CityInfo[] cities;
	private GeneralInfo[] generals;
	private EquipmentInfo[] equipments;
	
	public int kingNum = 18;
	public int cityNum = 48;
	public int generalNum = 255;
	public int magicNum = 50;
	public int equipmentNum = 31;
	public int armsNum = 11;
	public int formationNum = 7;
	public int jobsNum = 50;

    public int[] modKingNum = new int[] { 14, 18, 8, 8, 5 };

	public List<ArmyInfo> armys = new List<ArmyInfo>();
	
	private static Informations instance;
	public static Informations Instance {
		get {
			if (instance == null) {
				instance = new Informations();
			}
			return instance;
		}
	}

	public static void Reset() {
		instance = null;
	}
	
	public EquipmentInfo GetEquipmentInfo(int idx) {
		if (idx < 0 || idx >= equipmentNum) return null;
		if (equipments == null) {
			InitEquipmentInfo();
		}
		
		return equipments[idx];
	}
	
	public KingInfo GetKingInfo(int idx) {
		if (idx < 0 || idx >= kingNum+1) {
			return null;
		}
		
		if (kings == null) {
			//InitDefaultKingsInfo();
			//InitKingInfo();
            MODLoadController.Instance.LoadMOD(Controller.MODSelect);
		}
		
		return kings[idx];
	}
	
	public void SetKingInfo(int idx, KingInfo kInfo) {
		
		if (kings == null) {
			kings = new KingInfo[kingNum+1];
		}
		
		kings[idx] = kInfo;
	}
	
	public CityInfo GetCityInfo(int idx) {
		if (idx < 0 || idx >= cityNum)
			return null;
		
		if (cities == null) {
			//InitDefaultCitiesInfo();
			//InitCityPrefect();
			//InitCityInfo();
            MODLoadController.Instance.LoadMOD(Controller.MODSelect);
		}
		
		return cities[idx];
	}
	
	public void SetCityInfo(int idx, CityInfo cInfo) {
		
		if (cities == null) {
			cities = new CityInfo[cityNum];
		}
		
		cities[idx] = cInfo;
	}
	
	public GeneralInfo GetGeneralInfo(int idx) {
		if (idx < 0 || idx >= generalNum)
			return null;
		
		if (generals == null) {
			//InitDefaultGeneralsInfo();
            MODLoadController.Instance.LoadMOD(Controller.MODSelect);

            /*
			for (int i=0; i<generalNum; i++) {
				generals[i].job = generals[i].magic[0];
			}
			
			for (int m=0; m<generalNum; m++) {
				int arms = generals[m].arms;
				int i = 0;
				while (i < armsNum && (arms & (1 << i)) == 0)  i++;
				generals[m].armsCur = 1 << i;
			}
			
			for (int m=0; m<generalNum; m++) {
				int formation = generals[m].formation;
				int i = formationNum;
				while (i >= 0 && (formation & (1 << i)) == 0) i--;
				
				generals[m].formationCur = 1 << i;
			}
			*/
			/*
			// test prisoner
			generals[0].prisonerIdx = 0;
			generals[1].prisonerIdx = 0;
			generals[1].city = 9;
			*/
			/*
			// test Promotion
			generals[0].level = 10;
			generals[0].king = 0;
			generals[1].level = 20;
			generals[1].king = 0;
			generals[2].level = 40;
			generals[2].king = 0;
			*/
		}
		
		return generals[idx];
	}
	
	public void SetGeneralInfo(int idx, GeneralInfo gInfo) {
		
		if (generals == null) {
			generals = new GeneralInfo[generalNum];
		}
		
		generals[idx] = gInfo;
	}
	
	public void InitKingInfo() {
		InitKingCities();
		InitKingGenerals();
	}
	
	public void InitCityInfo() {
		//test
		InitCityObjects();
		InitCityGenerals();
	}

    public void InitGeneralInfo() {
        for (int i = 0; i < generalNum; i++)
        {
            generals[i].job = generals[i].magic[0];
        }

        for (int m = 0; m < generalNum; m++)
        {
            int arms = generals[m].arms;
            int i = 0;
            while (i < armsNum && (arms & (1 << i)) == 0) i++;
            generals[m].armsCur = 1 << i;
        }

        for (int m = 0; m < generalNum; m++)
        {
            int formation = generals[m].formation;
            int i = formationNum;
            while (i >= 0 && (formation & (1 << i)) == 0) i--;

            generals[m].formationCur = 1 << i;
        }
    }

	/// <summary>
	/// Inits the king cities.
	/// </summary>
	
	private void InitKingCities() {
		for (int idx=0; idx<kingNum+1; idx++) {
            
			KingInfo kInfo = GetKingInfo(idx);
			kInfo.cities = new List<int>();
			
			for (int i=0; i<cityNum; i++) {
				CityInfo cInfo = GetCityInfo(i);
				
				if (cInfo.king == idx) {
					kInfo.cities.Add(i);
				}
			}
		}
	}
	
	private void InitKingGenerals() {
		for (int idx=0; idx<kingNum+1; idx++) {
			
			KingInfo kInfo = GetKingInfo(idx);
			kInfo.generals = new List<int>();
			
			for (int i=generalNum-1; i>=0; i--) {
				GeneralInfo gInfo = GetGeneralInfo(i);
				
				if (gInfo.king == idx && gInfo.prisonerIdx == -1) {
					kInfo.generals.Add(i);
				}
			}
		}
	}
	
	/// <summary>
	/// Inits the city prefect.
	/// </summary>
	
	public void InitCityPrefect() {
		
		for (int idx=0; idx<cityNum; idx++) {
			
			CityInfo cInfo = GetCityInfo(idx);
			
			if (cInfo.king == -1) 	continue;
				
			KingInfo kInfo = GetKingInfo(cInfo.king);
			if (GetGeneralInfo(kInfo.generalIdx).city == idx) {
				cInfo.prefect = kInfo.generalIdx;
				continue;
			}
			
			int gIdx = -1;
			for (int i=generalNum-1; i>=0; i--) {
				GeneralInfo gInfo = GetGeneralInfo(i);
				
				if (gInfo.city == idx && gInfo.king == cInfo.king && gInfo.prisonerIdx == -1) {
					if (gIdx == -1) {
						gIdx = i;
					} else {
						if (GetGeneralInfo(gIdx).strength < gInfo.strength) {
							gIdx = i;
						}
					}
				}
			}
			
			cInfo.prefect = gIdx;
		}
	}
	
	private void InitCityObjects() {
		
		for (int idx=0; idx<cityNum; idx++) {
			
			CityInfo cInfo = GetCityInfo(idx);
			if (cInfo.objects == null) {
				cInfo.objects = new List<int>();
			}
			//test
			/*
			for (int i=0; i<5; i++) {
				int obj = 0;
				
				if (Random.Range(0, 100) < 33) { 
					obj = 1 << 16;
					obj += Random.Range(0, equipmentNum);
				} else if (Random.Range(0, 100) < 66) {
					obj = 2 << 16;
					obj += 1 << Random.Range(0, armsNum);
				} else {
					obj = 3 << 16;
					obj += 1 << Random.Range(0, formationNum);
				}
				
				cInfo.objects.Add(obj);
			}*/
		}
	}
	
	private void InitCityGenerals() {
		
		for (int idx=0; idx<cityNum; idx++) {
			
			CityInfo cInfo = GetCityInfo(idx);
			
			cInfo.generals = new List<int>();
			cInfo.prisons = new List<int>();
			
			for (int i=generalNum-1; i>=0; i--) {
				GeneralInfo gInfo = GetGeneralInfo(i);
				
				if (gInfo.city == idx) {
					if (cInfo.king == -1) {
						gInfo.king = -1;
						gInfo.city = -1;
					} else if (gInfo.king == cInfo.king && gInfo.prisonerIdx == -1) {
						cInfo.generals.Add(i);
					} else if (gInfo.prisonerIdx != -1) {
						gInfo.prisonerIdx = cInfo.king;
						cInfo.prisons.Add(i);
					} else {
						
						gInfo.king = -1;
						gInfo.city = -1;
					}
				}
			}
		}
	}
	
	
	/// <summary>
	/// Inits the default kings info.
	/// </summary>
	
	private void InitDefaultKingsInfo() {
		if (kings == null) {
			kings = new KingInfo[kingNum+1];
			for (int i=0; i<kingNum+1; i++) {
				kings[i] = new KingInfo();
			}
		}
		
		kings[0].generalIdx = 98;
		kings[1].generalIdx = 134;
		kings[2].generalIdx = 59;
		kings[3].generalIdx = 121;
		kings[4].generalIdx = 71;
		kings[5].generalIdx = 72;
		kings[6].generalIdx = 3;
		kings[7].generalIdx = 156;
		kings[8].generalIdx = 7;
		kings[9].generalIdx = 189;
		kings[10].generalIdx = 79;
		kings[11].generalIdx = 110;
		kings[12].generalIdx = 6;
		kings[13].generalIdx = 132;
		kings[14].generalIdx = 137;
		kings[15].generalIdx = 167;
		kings[16].generalIdx = 172;
		kings[17].generalIdx = 133;
	}
	
	private void InitDefaultCitiesInfo() {
		
		if (cities == null) {
			cities = new CityInfo[cityNum];
			for (int i=0; i<cityNum; i++) {
				cities[i] = new CityInfo();
			}
		}
		
		cities[0X00].king = -1;
		cities[0X00].population = 77000;
		cities[0X00].money = 2827;
		cities[0X00].reservistMax = 140;
		cities[0X00].reservist = 0;
		cities[0X00].defense = 369;
		
		cities[0X01].king = 6;
		cities[0X01].population = 90000;
		cities[0X01].money = 2925;
		cities[0X01].reservistMax = 146;
		cities[0X01].reservist = 0;
		cities[0X01].defense = 252;
		
		cities[0X02].king = 4;
		cities[0X02].population = 172739;
		cities[0X02].money = 3472;
		cities[0X02].reservistMax = 179;
		cities[0X02].reservist = 0;
		cities[0X02].defense = 321;
		
		cities[0X03].king = -1;
		cities[0X03].population = 88000;
		cities[0X03].money = 2910;
		cities[0X03].reservistMax = 145;
		cities[0X03].reservist = 0;
		cities[0X03].defense = 300;
		
		cities[0X04].king = 4;
		cities[0X04].population = 355340;
		cities[0X04].money = 4822;
		cities[0X04].reservistMax = 252;
		cities[0X04].reservist = 0;
		cities[0X04].defense = 439;
		
		cities[0X05].king = 1;
		cities[0X05].population = 330000;
		cities[0X05].money = 4725;
		cities[0X05].reservistMax = 242;
		cities[0X05].reservist = 0;
		cities[0X05].defense = 365;
		
		cities[0X06].king = 7;
		cities[0X06].population = 214229;
		cities[0X06].money = 3795;
		cities[0X06].reservistMax = 195;
		cities[0X06].reservist = 0;
		cities[0X06].defense = 312;
		
		cities[0X07].king = 8;
		cities[0X07].population = 129646;
		cities[0X07].money = 3135;
		cities[0X07].reservistMax = 161;
		cities[0X07].reservist = 0;
		cities[0X07].defense = 320;
		
		cities[0X08].king = 9;
		cities[0X08].population = 167000;
		cities[0X08].money = 3502;
		cities[0X08].reservistMax = 176;
		cities[0X08].reservist = 0;
		cities[0X08].defense = 227;
		
		cities[0X09].king = 0;
		cities[0X09].population = 221000;
		cities[0X09].money = 3907;
		cities[0X09].reservistMax = 198;
		cities[0X09].reservist = 0;
		cities[0X09].defense = 206;
		
		cities[0X0A].king = 3;
		cities[0X0A].population = 425000;
		cities[0X0A].money = 5437;
		cities[0X0A].reservistMax = 280;
		cities[0X0A].reservist = 0;
		cities[0X0A].defense = 500;
		
		cities[0X0B].king = 3;
		cities[0X0B].population = 142000;
		cities[0X0B].money = 3315;
		cities[0X0B].reservistMax = 166;
		cities[0X0B].reservist = 0;
		cities[0X0B].defense = 232;
		
		cities[0X0C].king = 3;
		cities[0X0C].population = 308093;
		cities[0X0C].money = 4492;
		cities[0X0C].reservistMax = 233;
		cities[0X0C].reservist = 0;
		cities[0X0C].defense = 513;
		
		cities[0X0D].king = -1;
		cities[0X0D].population = 89000;
		cities[0X0D].money = 2917;
		cities[0X0D].reservistMax = 145;
		cities[0X0D].reservist = 0;
		cities[0X0D].defense = 245;
		
		cities[0X0E].king = -1;
		cities[0X0E].population = 121000;
		cities[0X0E].money = 3157;
		cities[0X0E].reservistMax = 158;
		cities[0X0E].reservist = 0;
		cities[0X0E].defense = 373;
		
		cities[0X0F].king = 10;
		cities[0X0F].population = 160000;
		cities[0X0F].money = 3450;
		cities[0X0F].reservistMax = 174;
		cities[0X0F].reservist = 0;
		cities[0X0F].defense = 515;
		
		cities[0X10].king = 11;
		cities[0X10].population = 167000;
		cities[0X10].money = 3502;
		cities[0X10].reservistMax = 176;
		cities[0X10].reservist = 0;
		cities[0X10].defense = 140;
		
		cities[0X11].king = 11;
		cities[0X11].population = 188000;
		cities[0X11].money = 3660;
		cities[0X11].reservistMax = 185;
		cities[0X11].reservist = 0;
		cities[0X11].defense = 106;
		
		cities[0X12].king = -1;
		cities[0X12].population = 321000;
		cities[0X12].money = 4657;
		cities[0X12].reservistMax = 238;
		cities[0X12].reservist = 0;
		cities[0X12].defense = 429;
		
		cities[0X13].king = 12;
		cities[0X13].population = 233000;
		cities[0X13].money = 3997;
		cities[0X13].reservistMax = 206;
		cities[0X13].reservist = 0;
		cities[0X13].defense = 305;
		
		cities[0X14].king = -1;
		cities[0X14].population = 251000;
		cities[0X14].money = 4132;
		cities[0X14].reservistMax = 210;
		cities[0X14].reservist = 0;
		cities[0X14].defense = 137;
		
		cities[0X15].king = 5;
		cities[0X15].population = 325000;
		cities[0X15].money = 4687;
		cities[0X15].reservistMax = 240;
		cities[0X15].reservist = 0;
		cities[0X15].defense = 317;
		
		cities[0X16].king = -1;
		cities[0X16].population = 213000;
		cities[0X16].money = 3847;
		cities[0X16].reservistMax = 195;
		cities[0X16].reservist = 0;
		cities[0X16].defense = 207;
		
		cities[0X17].king = 13;
		cities[0X17].population = 370566;
		cities[0X17].money = 4950;
		cities[0X17].reservistMax = 258;
		cities[0X17].reservist = 0;
		cities[0X17].defense = 345;
		
		cities[0X18].king = -1;
		cities[0X18].population = 254000;
		cities[0X18].money = 4155;
		cities[0X18].reservistMax = 211;
		cities[0X18].reservist = 0;
		cities[0X18].defense = 214;
		
		cities[0X19].king = 13;
		cities[0X19].population = 132000;
		cities[0X19].money = 3240;
		cities[0X19].reservistMax = 162;
		cities[0X19].reservist = 0;
		cities[0X19].defense = 236;
		
		cities[0X1A].king = 13;
		cities[0X1A].population = 233000;
		cities[0X1A].money = 3997;
		cities[0X1A].reservistMax = 203;
		cities[0X1A].reservist = 0;
		cities[0X1A].defense = 294;
		
		cities[0X1B].king = -1;
		cities[0X1B].population = 109000;
		cities[0X1B].money = 3067;
		cities[0X1B].reservistMax = 153;
		cities[0X1B].reservist = 0;
		cities[0X1B].defense = 175;
		
		cities[0X1C].king = 2;
		cities[0X1C].population = 224000;
		cities[0X1C].money = 3930;
		cities[0X1C].reservistMax = 199;
		cities[0X1C].reservist = 0;
		cities[0X1C].defense = 473;
		
		cities[0X1D].king = -1;
		cities[0X1D].population = 174000;
		cities[0X1D].money = 3555;
		cities[0X1D].reservistMax = 179;
		cities[0X1D].reservist = 0;
		cities[0X1D].defense = 121;
		
		cities[0X1E].king = -1;
		cities[0X1E].population = 225000;
		cities[0X1E].money = 3937;
		cities[0X1E].reservistMax = 200;
		cities[0X1E].reservist = 0;
		cities[0X1E].defense = 132;
		
		cities[0X1F].king = 14;
		cities[0X1F].population = 154000;
		cities[0X1F].money = 3405;
		cities[0X1F].reservistMax = 171;
		cities[0X1F].reservist = 0;
		cities[0X1F].defense = 182;
		
		cities[0X20].king = 14;
		cities[0X20].population = 290000;
		cities[0X20].money = 4425;
		cities[0X20].reservistMax = 226;
		cities[0X20].reservist = 0;
		cities[0X20].defense = 420;
		
		cities[0X21].king = 15;
		cities[0X21].population = 232000;
		cities[0X21].money = 3990;
		cities[0X21].reservistMax = 202;
		cities[0X21].reservist = 0;
		cities[0X21].defense = 336;
		
		cities[0X22].king = 16;
		cities[0X22].population = 139000;
		cities[0X22].money = 3292;
		cities[0X22].reservistMax = 165;
		cities[0X22].reservist = 0;
		cities[0X22].defense = 160;
		
		cities[0X23].king = -1;
		cities[0X23].population = 170000;
		cities[0X23].money = 3525;
		cities[0X23].reservistMax = 178;
		cities[0X23].reservist = 0;
		cities[0X23].defense = 203;
		
		cities[0X24].king = -1;
		cities[0X24].population = 213000;
		cities[0X24].money = 3847;
		cities[0X24].reservistMax = 195;
		cities[0X24].reservist = 0;
		cities[0X24].defense = 233;
		
		cities[0X25].king = -1;
		cities[0X25].population = 181000;
		cities[0X25].money = 3607;
		cities[0X25].reservistMax = 182;
		cities[0X25].reservist = 0;
		cities[0X25].defense = 502;
		
		cities[0X26].king = -1;
		cities[0X26].population = 115000;
		cities[0X26].money = 3112;
		cities[0X26].reservistMax = 156;
		cities[0X26].reservist = 0;
		cities[0X26].defense = 211;
		
		cities[0X27].king = 17;
		cities[0X27].population = 235000;
		cities[0X27].money = 4012;
		cities[0X27].reservistMax = 204;
		cities[0X27].reservist = 0;
		cities[0X27].defense = 306;
		
		cities[0X28].king = 17;
		cities[0X28].population = 308000;
		cities[0X28].money = 4560;
		cities[0X28].reservistMax = 233;
		cities[0X28].reservist = 0;
		cities[0X28].defense = 284;
		
		cities[0X29].king = -1;
		cities[0X29].population = 236000;
		cities[0X29].money = 4020;
		cities[0X29].reservistMax = 204;
		cities[0X29].reservist = 0;
		cities[0X29].defense = 268;
		
		cities[0X2A].king = 17;
		cities[0X2A].population = 211000;
		cities[0X2A].money = 3832;
		cities[0X2A].reservistMax = 194;
		cities[0X2A].reservist = 0;
		cities[0X2A].defense = 162;
		
		cities[0X2B].king = -1;
		cities[0X2B].population = 176000;
		cities[0X2B].money = 3570;
		cities[0X2B].reservistMax = 180;
		cities[0X2B].reservist = 0;
		cities[0X2B].defense = 193;
		
		cities[0X2C].king = -1;
		cities[0X2C].population = 303000;
		cities[0X2C].money = 4522;
		cities[0X2C].reservistMax = 231;
		cities[0X2C].reservist = 0;
		cities[0X2C].defense = 375;

		cities[0X2D].king = -1;
		cities[0X2D].population = 149000;
		cities[0X2D].money = 3367;
		cities[0X2D].reservistMax = 169;
		cities[0X2D].reservist = 0;
		cities[0X2D].defense = 112;
		
		cities[0X2E].king = -1;
		cities[0X2E].population = 166000;
		cities[0X2E].money = 3495;
		cities[0X2E].reservistMax = 176;
		cities[0X2E].reservist = 0;
		cities[0X2E].defense = 244;
		
		cities[0X2F].king = -1;
		cities[0X2F].population = 62000;
		cities[0X2F].money = 2715;
		cities[0X2F].reservistMax = 134;
		cities[0X2F].reservist = 0;
		cities[0X2F].defense = 108;
	}
	
	private void InitDefaultGeneralsInfo() {
		if (generals == null) {
			generals = new GeneralInfo[generalNum];
			
			for (int i=0; i<generalNum; i++) {
				generals[i] = new GeneralInfo();
			}
		}
		
generals[0].king = -1;
generals[0].city = -1;
generals[0].magic[0] = 27;
generals[0].magic[1] = -1;
generals[0].magic[2] = -1;
generals[0].magic[3] = -1;
generals[0].equipment = -1;
generals[0].strength = 84;
generals[0].intellect = 68;
generals[0].experience = 200;
generals[0].level = 2;
generals[0].healthMax = 85;
generals[0].healthCur = 85;
generals[0].manaMax = 42;
generals[0].manaCur = 42;
generals[0].soldierMax = 10;
generals[0].soldierCur = 10;
generals[0].knightMax = 0;
generals[0].knightCur = 0;
generals[0].arms = 0x0009;
generals[0].formation = 0x29;

generals[1].king = -1;
generals[1].city = 18;
generals[1].magic[0] = 21;
generals[1].magic[1] = -1;
generals[1].magic[2] = -1;
generals[1].magic[3] = -1;
generals[1].equipment = -1;
generals[1].strength = 81;
generals[1].intellect = 71;
generals[1].experience = 200;
generals[1].level = 2;
generals[1].healthMax = 85;
generals[1].healthCur = 85;
generals[1].manaMax = 42;
generals[1].manaCur = 42;
generals[1].soldierMax = 10;
generals[1].soldierCur = 10;
generals[1].knightMax = 0;
generals[1].knightCur = 0;
generals[1].arms = 0x0001;
generals[1].formation = 0x42;

generals[2].king = -1;
generals[2].city = -1;
generals[2].magic[0] = 23;
generals[2].magic[1] = -1;
generals[2].magic[2] = -1;
generals[2].magic[3] = -1;
generals[2].equipment = -1;
generals[2].strength = 88;
generals[2].intellect = 30;
generals[2].experience = 200;
generals[2].level = 2;
generals[2].healthMax = 86;
generals[2].healthCur = 86;
generals[2].manaMax = 35;
generals[2].manaCur = 35;
generals[2].soldierMax = 10;
generals[2].soldierCur = 10;
generals[2].knightMax = 0;
generals[2].knightCur = 0;
generals[2].arms = 0x0200;
generals[2].formation = 0x4B;

generals[3].king = 6;
generals[3].city = 1;
generals[3].magic[0] = 24;
generals[3].magic[1] = -1;
generals[3].magic[2] = -1;
generals[3].magic[3] = -1;
generals[3].equipment = -1;
generals[3].strength = 86;
generals[3].intellect = 70;
generals[3].experience = 200;
generals[3].level = 2;
generals[3].healthMax = 85;
generals[3].healthCur = 85;
generals[3].manaMax = 42;
generals[3].manaCur = 42;
generals[3].soldierMax = 10;
generals[3].soldierCur = 10;
generals[3].knightMax = 0;
generals[3].knightCur = 0;
generals[3].arms = 0x0002;
generals[3].formation = 0x14;

generals[4].king = -1;
generals[4].city = 6;
generals[4].magic[0] = 22;
generals[4].magic[1] = -1;
generals[4].magic[2] = -1;
generals[4].magic[3] = -1;
generals[4].equipment = -1;
generals[4].strength = 73;
generals[4].intellect = 42;
generals[4].experience = 200;
generals[4].level = 2;
generals[4].healthMax = 82;
generals[4].healthCur = 82;
generals[4].manaMax = 37;
generals[4].manaCur = 37;
generals[4].soldierMax = 10;
generals[4].soldierCur = 10;
generals[4].knightMax = 0;
generals[4].knightCur = 0;
generals[4].arms = 0x0410;
generals[4].formation = 0x0A;

generals[5].king = -1;
generals[5].city = 7;
generals[5].magic[0] = 23;
generals[5].magic[1] = -1;
generals[5].magic[2] = -1;
generals[5].magic[3] = -1;
generals[5].equipment = -1;
generals[5].strength = 97;
generals[5].intellect = 70;
generals[5].experience = 200;
generals[5].level = 2;
generals[5].healthMax = 87;
generals[5].healthCur = 87;
generals[5].manaMax = 42;
generals[5].manaCur = 42;
generals[5].soldierMax = 10;
generals[5].soldierCur = 10;
generals[5].knightMax = 0;
generals[5].knightCur = 0;
generals[5].arms = 0x0002;
generals[5].formation = 0x05;

generals[6].king = 12;
generals[6].city = 19;
generals[6].magic[0] = 0;
generals[6].magic[1] = -1;
generals[6].magic[2] = -1;
generals[6].magic[3] = -1;
generals[6].equipment = -1;
generals[6].strength = 42;
generals[6].intellect = 81;
generals[6].experience = 200;
generals[6].level = 2;
generals[6].healthMax = 77;
generals[6].healthCur = 77;
generals[6].manaMax = 45;
generals[6].manaCur = 45;
generals[6].soldierMax = 10;
generals[6].soldierCur = 10;
generals[6].knightMax = 0;
generals[6].knightCur = 0;
generals[6].arms = 0x0100;
generals[6].formation = 0x18;

generals[7].king = 8;
generals[7].city = 7;
generals[7].magic[0] = 1;
generals[7].magic[1] = -1;
generals[7].magic[2] = -1;
generals[7].magic[3] = -1;
generals[7].equipment = -1;
generals[7].strength = 45;
generals[7].intellect = 89;
generals[7].experience = 200;
generals[7].level = 2;
generals[7].healthMax = 77;
generals[7].healthCur = 77;
generals[7].manaMax = 46;
generals[7].manaCur = 46;
generals[7].soldierMax = 10;
generals[7].soldierCur = 10;
generals[7].knightMax = 0;
generals[7].knightCur = 0;
generals[7].arms = 0x0100;
generals[7].formation = 0x41;

generals[8].king = -1;
generals[8].city = -1;
generals[8].magic[0] = 19;
generals[8].magic[1] = -1;
generals[8].magic[2] = -1;
generals[8].magic[3] = -1;
generals[8].equipment = -1;
generals[8].strength = 83;
generals[8].intellect = 48;
generals[8].experience = 200;
generals[8].level = 2;
generals[8].healthMax = 85;
generals[8].healthCur = 85;
generals[8].manaMax = 38;
generals[8].manaCur = 38;
generals[8].soldierMax = 10;
generals[8].soldierCur = 10;
generals[8].knightMax = 0;
generals[8].knightCur = 0;
generals[8].arms = 0x0051;
generals[8].formation = 0x25;

generals[9].king = -1;
generals[9].city = -1;
generals[9].magic[0] = 25;
generals[9].magic[1] = -1;
generals[9].magic[2] = -1;
generals[9].magic[3] = -1;
generals[9].equipment = -1;
generals[9].strength = 82;
generals[9].intellect = 49;
generals[9].experience = 200;
generals[9].level = 2;
generals[9].healthMax = 85;
generals[9].healthCur = 85;
generals[9].manaMax = 38;
generals[9].manaCur = 38;
generals[9].soldierMax = 10;
generals[9].soldierCur = 10;
generals[9].knightMax = 0;
generals[9].knightCur = 0;
generals[9].arms = 0x0002;
generals[9].formation = 0x26;

generals[10].king = -1;
generals[10].city = -1;
generals[10].magic[0] = 23;
generals[10].magic[1] = -1;
generals[10].magic[2] = -1;
generals[10].magic[3] = -1;
generals[10].equipment = -1;
generals[10].strength = 90;
generals[10].intellect = 48;
generals[10].experience = 200;
generals[10].level = 2;
generals[10].healthMax = 86;
generals[10].healthCur = 86;
generals[10].manaMax = 38;
generals[10].manaCur = 38;
generals[10].soldierMax = 10;
generals[10].soldierCur = 10;
generals[10].knightMax = 0;
generals[10].knightCur = 0;
generals[10].arms = 0x0023;
generals[10].formation = 0x11;

generals[11].king = 4;
generals[11].city = 4;
generals[11].magic[0] = 24;
generals[11].magic[1] = -1;
generals[11].magic[2] = -1;
generals[11].magic[3] = -1;
generals[11].equipment = -1;
generals[11].strength = 96;
generals[11].intellect = 40;
generals[11].experience = 200;
generals[11].level = 2;
generals[11].healthMax = 87;
generals[11].healthCur = 87;
generals[11].manaMax = 37;
generals[11].manaCur = 37;
generals[11].soldierMax = 10;
generals[11].soldierCur = 10;
generals[11].knightMax = 0;
generals[11].knightCur = 0;
generals[11].arms = 0x0002;
generals[11].formation = 0x44;

generals[12].king = -1;
generals[12].city = -1;
generals[12].magic[0] = 21;
generals[12].magic[1] = -1;
generals[12].magic[2] = -1;
generals[12].magic[3] = -1;
generals[12].equipment = -1;
generals[12].strength = 81;
generals[12].intellect = 46;
generals[12].experience = 200;
generals[12].level = 2;
generals[12].healthMax = 85;
generals[12].healthCur = 85;
generals[12].manaMax = 37;
generals[12].manaCur = 37;
generals[12].soldierMax = 10;
generals[12].soldierCur = 10;
generals[12].knightMax = 0;
generals[12].knightCur = 0;
generals[12].arms = 0x0010;
generals[12].formation = 0x01;

generals[13].king = 3;
generals[13].city = 10;
generals[13].magic[0] = 0;
generals[13].magic[1] = -1;
generals[13].magic[2] = -1;
generals[13].magic[3] = -1;
generals[13].equipment = 16;
generals[13].strength = 48;
generals[13].intellect = 72;
generals[13].experience = 200;
generals[13].level = 2;
generals[13].healthMax = 77;
generals[13].healthCur = 77;
generals[13].manaMax = 42;
generals[13].manaCur = 42;
generals[13].soldierMax = 10;
generals[13].soldierCur = 10;
generals[13].knightMax = 0;
generals[13].knightCur = 0;
generals[13].arms = 0x0008;
generals[13].formation = 0x30;

generals[14].king = -1;
generals[14].city = -1;
generals[14].magic[0] = 27;
generals[14].magic[1] = -1;
generals[14].magic[2] = -1;
generals[14].magic[3] = -1;
generals[14].equipment = 24;
generals[14].strength = 91;
generals[14].intellect = 78;
generals[14].experience = 200;
generals[14].level = 2;
generals[14].healthMax = 86;
generals[14].healthCur = 86;
generals[14].manaMax = 44;
generals[14].manaCur = 44;
generals[14].soldierMax = 10;
generals[14].soldierCur = 10;
generals[14].knightMax = 0;
generals[14].knightCur = 0;
generals[14].arms = 0x0010;
generals[14].formation = 0x41;

generals[15].king = -1;
generals[15].city = -1;
generals[15].magic[0] = 19;
generals[15].magic[1] = -1;
generals[15].magic[2] = -1;
generals[15].magic[3] = -1;
generals[15].equipment = -1;
generals[15].strength = 85;
generals[15].intellect = 71;
generals[15].experience = 200;
generals[15].level = 2;
generals[15].healthMax = 85;
generals[15].healthCur = 85;
generals[15].manaMax = 42;
generals[15].manaCur = 42;
generals[15].soldierMax = 10;
generals[15].soldierCur = 10;
generals[15].knightMax = 0;
generals[15].knightCur = 0;
generals[15].arms = 0x0008;
generals[15].formation = 0x11;

generals[16].king = -1;
generals[16].city = -1;
generals[16].magic[0] = 4;
generals[16].magic[1] = -1;
generals[16].magic[2] = -1;
generals[16].magic[3] = -1;
generals[16].equipment = -1;
generals[16].strength = 68;
generals[16].intellect = 87;
generals[16].experience = 200;
generals[16].level = 2;
generals[16].healthMax = 82;
generals[16].healthCur = 82;
generals[16].manaMax = 46;
generals[16].manaCur = 46;
generals[16].soldierMax = 10;
generals[16].soldierCur = 10;
generals[16].knightMax = 0;
generals[16].knightCur = 0;
generals[16].arms = 0x0040;
generals[16].formation = 0x18;

generals[17].king = -1;
generals[17].city = -1;
generals[17].magic[0] = 1;
generals[17].magic[1] = -1;
generals[17].magic[2] = -1;
generals[17].magic[3] = -1;
generals[17].equipment = -1;
generals[17].strength = 68;
generals[17].intellect = 93;
generals[17].experience = 200;
generals[17].level = 2;
generals[17].healthMax = 82;
generals[17].healthCur = 82;
generals[17].manaMax = 46;
generals[17].manaCur = 46;
generals[17].soldierMax = 10;
generals[17].soldierCur = 10;
generals[17].knightMax = 0;
generals[17].knightCur = 0;
generals[17].arms = 0x0020;
generals[17].formation = 0x42;

generals[18].king = -1;
generals[18].city = -1;
generals[18].magic[0] = 1;
generals[18].magic[1] = -1;
generals[18].magic[2] = -1;
generals[18].magic[3] = -1;
generals[18].equipment = -1;
generals[18].strength = 73;
generals[18].intellect = 99;
generals[18].experience = 200;
generals[18].level = 2;
generals[18].healthMax = 82;
generals[18].healthCur = 82;
generals[18].manaMax = 47;
generals[18].manaCur = 47;
generals[18].soldierMax = 10;
generals[18].soldierCur = 10;
generals[18].knightMax = 0;
generals[18].knightCur = 0;
generals[18].arms = 0x0108;
generals[18].formation = 0x32;

generals[19].king = -1;
generals[19].city = -1;
generals[19].magic[0] = 28;
generals[19].magic[1] = -1;
generals[19].magic[2] = -1;
generals[19].magic[3] = -1;
generals[19].equipment = -1;
generals[19].strength = 96;
generals[19].intellect = 63;
generals[19].experience = 200;
generals[19].level = 2;
generals[19].healthMax = 87;
generals[19].healthCur = 87;
generals[19].manaMax = 41;
generals[19].manaCur = 41;
generals[19].soldierMax = 10;
generals[19].soldierCur = 10;
generals[19].knightMax = 0;
generals[19].knightCur = 0;
generals[19].arms = 0x0004;
generals[19].formation = 0x03;

generals[20].king = 4;
generals[20].city = 4;
generals[20].magic[0] = 4;
generals[20].magic[1] = -1;
generals[20].magic[2] = -1;
generals[20].magic[3] = -1;
generals[20].equipment = -1;
generals[20].strength = 52;
generals[20].intellect = 95;
generals[20].experience = 200;
generals[20].level = 2;
generals[20].healthMax = 79;
generals[20].healthCur = 79;
generals[20].manaMax = 47;
generals[20].manaCur = 47;
generals[20].soldierMax = 10;
generals[20].soldierCur = 10;
generals[20].knightMax = 0;
generals[20].knightCur = 0;
generals[20].arms = 0x0020;
generals[20].formation = 0x12;

generals[21].king = -1;
generals[21].city = -1;
generals[21].magic[0] = 2;
generals[21].magic[1] = -1;
generals[21].magic[2] = -1;
generals[21].magic[3] = -1;
generals[21].equipment = -1;
generals[21].strength = 44;
generals[21].intellect = 81;
generals[21].experience = 200;
generals[21].level = 2;
generals[21].healthMax = 77;
generals[21].healthCur = 77;
generals[21].manaMax = 45;
generals[21].manaCur = 45;
generals[21].soldierMax = 10;
generals[21].soldierCur = 10;
generals[21].knightMax = 0;
generals[21].knightCur = 0;
generals[21].arms = 0x0008;
generals[21].formation = 0x22;

generals[22].king = -1;
generals[22].city = -1;
generals[22].magic[0] = 20;
generals[22].magic[1] = -1;
generals[22].magic[2] = -1;
generals[22].magic[3] = -1;
generals[22].equipment = -1;
generals[22].strength = 85;
generals[22].intellect = 76;
generals[22].experience = 200;
generals[22].level = 2;
generals[22].healthMax = 85;
generals[22].healthCur = 85;
generals[22].manaMax = 44;
generals[22].manaCur = 44;
generals[22].soldierMax = 10;
generals[22].soldierCur = 10;
generals[22].knightMax = 0;
generals[22].knightCur = 0;
generals[22].arms = 0x0012;
generals[22].formation = 0x49;

generals[23].king = -1;
generals[23].city = -1;
generals[23].magic[0] = 24;
generals[23].magic[1] = -1;
generals[23].magic[2] = -1;
generals[23].magic[3] = -1;
generals[23].equipment = -1;
generals[23].strength = 72;
generals[23].intellect = 38;
generals[23].experience = 200;
generals[23].level = 2;
generals[23].healthMax = 82;
generals[23].healthCur = 82;
generals[23].manaMax = 36;
generals[23].manaCur = 36;
generals[23].soldierMax = 10;
generals[23].soldierCur = 10;
generals[23].knightMax = 0;
generals[23].knightCur = 0;
generals[23].arms = 0x0080;
generals[23].formation = 0x46;

generals[24].king = -1;
generals[24].city = -1;
generals[24].magic[0] = 26;
generals[24].magic[1] = -1;
generals[24].magic[2] = -1;
generals[24].magic[3] = -1;
generals[24].equipment = -1;
generals[24].strength = 89;
generals[24].intellect = 76;
generals[24].experience = 200;
generals[24].level = 2;
generals[24].healthMax = 86;
generals[24].healthCur = 86;
generals[24].manaMax = 44;
generals[24].manaCur = 44;
generals[24].soldierMax = 10;
generals[24].soldierCur = 10;
generals[24].knightMax = 0;
generals[24].knightCur = 0;
generals[24].arms = 0x0100;
generals[24].formation = 0x09;

generals[25].king = -1;
generals[25].city = -1;
generals[25].magic[0] = 25;
generals[25].magic[1] = -1;
generals[25].magic[2] = -1;
generals[25].magic[3] = -1;
generals[25].equipment = -1;
generals[25].strength = 81;
generals[25].intellect = 67;
generals[25].experience = 200;
generals[25].level = 2;
generals[25].healthMax = 85;
generals[25].healthCur = 85;
generals[25].manaMax = 42;
generals[25].manaCur = 42;
generals[25].soldierMax = 10;
generals[25].soldierCur = 10;
generals[25].knightMax = 0;
generals[25].knightCur = 0;
generals[25].arms = 0x0080;
generals[25].formation = 0x06;

generals[26].king = -1;
generals[26].city = 38;
generals[26].magic[0] = 22;
generals[26].magic[1] = -1;
generals[26].magic[2] = -1;
generals[26].magic[3] = -1;
generals[26].equipment = -1;
generals[26].strength = 82;
generals[26].intellect = 44;
generals[26].experience = 200;
generals[26].level = 2;
generals[26].healthMax = 85;
generals[26].healthCur = 85;
generals[26].manaMax = 37;
generals[26].manaCur = 37;
generals[26].soldierMax = 10;
generals[26].soldierCur = 10;
generals[26].knightMax = 0;
generals[26].knightCur = 0;
generals[26].arms = 0x0001;
generals[26].formation = 0x01;

generals[27].king = 17;
generals[27].city = 42;
generals[27].magic[0] = 26;
generals[27].magic[1] = -1;
generals[27].magic[2] = -1;
generals[27].magic[3] = -1;
generals[27].equipment = -1;
generals[27].strength = 79;
generals[27].intellect = 77;
generals[27].experience = 200;
generals[27].level = 2;
generals[27].healthMax = 84;
generals[27].healthCur = 84;
generals[27].manaMax = 44;
generals[27].manaCur = 44;
generals[27].soldierMax = 10;
generals[27].soldierCur = 10;
generals[27].knightMax = 0;
generals[27].knightCur = 0;
generals[27].arms = 0x0001;
generals[27].formation = 0x09;

generals[28].king = 3;
generals[28].city = 10;
generals[28].magic[0] = 28;
generals[28].magic[1] = -1;
generals[28].magic[2] = -1;
generals[28].magic[3] = -1;
generals[28].equipment = 9;
generals[28].strength = 110;
generals[28].intellect = 42;
generals[28].experience = 200;
generals[28].level = 2;
generals[28].healthMax = 89;
generals[28].healthCur = 89;
generals[28].manaMax = 37;
generals[28].manaCur = 37;
generals[28].soldierMax = 10;
generals[28].soldierCur = 10;
generals[28].knightMax = 0;
generals[28].knightCur = 0;
generals[28].arms = 0x0006;
generals[28].formation = 0x06;

generals[29].king = -1;
generals[29].city = -1;
generals[29].magic[0] = 21;
generals[29].magic[1] = -1;
generals[29].magic[2] = -1;
generals[29].magic[3] = -1;
generals[29].equipment = -1;
generals[29].strength = 85;
generals[29].intellect = 90;
generals[29].experience = 200;
generals[29].level = 2;
generals[29].healthMax = 85;
generals[29].healthCur = 85;
generals[29].manaMax = 46;
generals[29].manaCur = 46;
generals[29].soldierMax = 10;
generals[29].soldierCur = 10;
generals[29].knightMax = 0;
generals[29].knightCur = 0;
generals[29].arms = 0x0001;
generals[29].formation = 0x11;

generals[30].king = -1;
generals[30].city = 8;
generals[30].magic[0] = 22;
generals[30].magic[1] = -1;
generals[30].magic[2] = -1;
generals[30].magic[3] = -1;
generals[30].equipment = -1;
generals[30].strength = 68;
generals[30].intellect = 48;
generals[30].experience = 200;
generals[30].level = 2;
generals[30].healthMax = 82;
generals[30].healthCur = 82;
generals[30].manaMax = 38;
generals[30].manaCur = 38;
generals[30].soldierMax = 10;
generals[30].soldierCur = 10;
generals[30].knightMax = 0;
generals[30].knightCur = 0;
generals[30].arms = 0x0008;
generals[30].formation = 0x01;

generals[31].king = 0;
generals[31].city = 9;
generals[31].magic[0] = 19;
generals[31].magic[1] = -1;
generals[31].magic[2] = -1;
generals[31].magic[3] = -1;
generals[31].equipment = -1;
generals[31].strength = 78;
generals[31].intellect = 81;
generals[31].experience = 200;
generals[31].level = 2;
generals[31].healthMax = 84;
generals[31].healthCur = 84;
generals[31].manaMax = 45;
generals[31].manaCur = 45;
generals[31].soldierMax = 10;
generals[31].soldierCur = 10;
generals[31].knightMax = 0;
generals[31].knightCur = 0;
generals[31].arms = 0x0002;
generals[31].formation = 0x03;

generals[32].king = -1;
generals[32].city = -1;
generals[32].magic[0] = 1;
generals[32].magic[1] = -1;
generals[32].magic[2] = -1;
generals[32].magic[3] = -1;
generals[32].equipment = -1;
generals[32].strength = 54;
generals[32].intellect = 87;
generals[32].experience = 200;
generals[32].level = 2;
generals[32].healthMax = 80;
generals[32].healthCur = 80;
generals[32].manaMax = 46;
generals[32].manaCur = 46;
generals[32].soldierMax = 10;
generals[32].soldierCur = 10;
generals[32].knightMax = 0;
generals[32].knightCur = 0;
generals[32].arms = 0x0100;
generals[32].formation = 0x11;

generals[33].king = 3;
generals[33].city = 10;
generals[33].magic[0] = 3;
generals[33].magic[1] = -1;
generals[33].magic[2] = -1;
generals[33].magic[3] = -1;
generals[33].equipment = -1;
generals[33].strength = 42;
generals[33].intellect = 91;
generals[33].experience = 200;
generals[33].level = 2;
generals[33].healthMax = 77;
generals[33].healthCur = 77;
generals[33].manaMax = 46;
generals[33].manaCur = 46;
generals[33].soldierMax = 10;
generals[33].soldierCur = 10;
generals[33].knightMax = 0;
generals[33].knightCur = 0;
generals[33].arms = 0x0020;
generals[33].formation = 0x48;

generals[34].king = -1;
generals[34].city = -1;
generals[34].magic[0] = 27;
generals[34].magic[1] = -1;
generals[34].magic[2] = -1;
generals[34].magic[3] = -1;
generals[34].equipment = -1;
generals[34].strength = 87;
generals[34].intellect = 82;
generals[34].experience = 200;
generals[34].level = 2;
generals[34].healthMax = 86;
generals[34].healthCur = 86;
generals[34].manaMax = 45;
generals[34].manaCur = 45;
generals[34].soldierMax = 10;
generals[34].soldierCur = 10;
generals[34].knightMax = 0;
generals[34].knightCur = 0;
generals[34].arms = 0x0002;
generals[34].formation = 0x03;

generals[35].king = 3;
generals[35].city = 12;
generals[35].magic[0] = 19;
generals[35].magic[1] = -1;
generals[35].magic[2] = -1;
generals[35].magic[3] = -1;
generals[35].equipment = -1;
generals[35].strength = 74;
generals[35].intellect = 38;
generals[35].experience = 200;
generals[35].level = 2;
generals[35].healthMax = 83;
generals[35].healthCur = 83;
generals[35].manaMax = 36;
generals[35].manaCur = 36;
generals[35].soldierMax = 10;
generals[35].soldierCur = 10;
generals[35].knightMax = 0;
generals[35].knightCur = 0;
generals[35].arms = 0x0002;
generals[35].formation = 0x42;

generals[36].king = -1;
generals[36].city = -1;
generals[36].magic[0] = 0;
generals[36].magic[1] = -1;
generals[36].magic[2] = -1;
generals[36].magic[3] = -1;
generals[36].equipment = -1;
generals[36].strength = 40;
generals[36].intellect = 83;
generals[36].experience = 200;
generals[36].level = 2;
generals[36].healthMax = 77;
generals[36].healthCur = 77;
generals[36].manaMax = 45;
generals[36].manaCur = 45;
generals[36].soldierMax = 10;
generals[36].soldierCur = 10;
generals[36].knightMax = 0;
generals[36].knightCur = 0;
generals[36].arms = 0x0040;
generals[36].formation = 0x18;

generals[37].king = -1;
generals[37].city = -1;
generals[37].magic[0] = 26;
generals[37].magic[1] = -1;
generals[37].magic[2] = -1;
generals[37].magic[3] = -1;
generals[37].equipment = 18;
generals[37].strength = 98;
generals[37].intellect = 25;
generals[37].experience = 200;
generals[37].level = 2;
generals[37].healthMax = 86;
generals[37].healthCur = 86;
generals[37].manaMax = 34;
generals[37].manaCur = 34;
generals[37].soldierMax = 10;
generals[37].soldierCur = 10;
generals[37].knightMax = 0;
generals[37].knightCur = 0;
generals[37].arms = 0x0080;
generals[37].formation = 0x05;

generals[38].king = -1;
generals[38].city = 11;
generals[38].magic[0] = 27;
generals[38].magic[1] = -1;
generals[38].magic[2] = -1;
generals[38].magic[3] = -1;
generals[38].equipment = -1;
generals[38].strength = 72;
generals[38].intellect = 61;
generals[38].experience = 200;
generals[38].level = 2;
generals[38].healthMax = 82;
generals[38].healthCur = 82;
generals[38].manaMax = 41;
generals[38].manaCur = 41;
generals[38].soldierMax = 10;
generals[38].soldierCur = 10;
generals[38].knightMax = 0;
generals[38].knightCur = 0;
generals[38].arms = 0x0001;
generals[38].formation = 0x11;

generals[39].king = -1;
generals[39].city = -1;
generals[39].magic[0] = 20;
generals[39].magic[1] = -1;
generals[39].magic[2] = -1;
generals[39].magic[3] = -1;
generals[39].equipment = -1;
generals[39].strength = 79;
generals[39].intellect = 44;
generals[39].experience = 200;
generals[39].level = 2;
generals[39].healthMax = 84;
generals[39].healthCur = 84;
generals[39].manaMax = 37;
generals[39].manaCur = 37;
generals[39].soldierMax = 10;
generals[39].soldierCur = 10;
generals[39].knightMax = 0;
generals[39].knightCur = 0;
generals[39].arms = 0x0001;
generals[39].formation = 0x30;

generals[40].king = -1;
generals[40].city = 9;
generals[40].magic[0] = 23;
generals[40].magic[1] = -1;
generals[40].magic[2] = -1;
generals[40].magic[3] = -1;
generals[40].equipment = 13;
generals[40].strength = 103;
generals[40].intellect = 45;
generals[40].experience = 200;
generals[40].level = 2;
generals[40].healthMax = 88;
generals[40].healthCur = 88;
generals[40].manaMax = 37;
generals[40].manaCur = 37;
generals[40].soldierMax = 10;
generals[40].soldierCur = 10;
generals[40].knightMax = 0;
generals[40].knightCur = 0;
generals[40].arms = 0x0004;
generals[40].formation = 0x41;

generals[41].king = -1;
generals[41].city = 8;
generals[41].magic[0] = 25;
generals[41].magic[1] = -1;
generals[41].magic[2] = -1;
generals[41].magic[3] = -1;
generals[41].equipment = -1;
generals[41].strength = 85;
generals[41].intellect = 42;
generals[41].experience = 200;
generals[41].level = 2;
generals[41].healthMax = 85;
generals[41].healthCur = 85;
generals[41].manaMax = 37;
generals[41].manaCur = 37;
generals[41].soldierMax = 10;
generals[41].soldierCur = 10;
generals[41].knightMax = 0;
generals[41].knightCur = 0;
generals[41].arms = 0x0410;
generals[41].formation = 0x09;

generals[42].king = -1;
generals[42].city = 35;
generals[42].magic[0] = 24;
generals[42].magic[1] = -1;
generals[42].magic[2] = -1;
generals[42].magic[3] = -1;
generals[42].equipment = -1;
generals[42].strength = 94;
generals[42].intellect = 67;
generals[42].experience = 200;
generals[42].level = 2;
generals[42].healthMax = 87;
generals[42].healthCur = 87;
generals[42].manaMax = 42;
generals[42].manaCur = 42;
generals[42].soldierMax = 10;
generals[42].soldierCur = 10;
generals[42].knightMax = 0;
generals[42].knightCur = 0;
generals[42].arms = 0x0002;
generals[42].formation = 0x03;

generals[43].king = -1;
generals[43].city = -1;
generals[43].magic[0] = 26;
generals[43].magic[1] = -1;
generals[43].magic[2] = -1;
generals[43].magic[3] = -1;
generals[43].equipment = -1;
generals[43].strength = 78;
generals[43].intellect = 98;
generals[43].experience = 200;
generals[43].level = 2;
generals[43].healthMax = 84;
generals[43].healthCur = 84;
generals[43].manaMax = 47;
generals[43].manaCur = 47;
generals[43].soldierMax = 10;
generals[43].soldierCur = 10;
generals[43].knightMax = 0;
generals[43].knightCur = 0;
generals[43].arms = 0x0008;
generals[43].formation = 0x30;

generals[44].king = -1;
generals[44].city = -1;
generals[44].magic[0] = 22;
generals[44].magic[1] = -1;
generals[44].magic[2] = -1;
generals[44].magic[3] = -1;
generals[44].equipment = -1;
generals[44].strength = 73;
generals[44].intellect = 72;
generals[44].experience = 200;
generals[44].level = 2;
generals[44].healthMax = 82;
generals[44].healthCur = 82;
generals[44].manaMax = 42;
generals[44].manaCur = 42;
generals[44].soldierMax = 10;
generals[44].soldierCur = 10;
generals[44].knightMax = 0;
generals[44].knightCur = 0;
generals[44].arms = 0x0002;
generals[44].formation = 0x50;

generals[45].king = -1;
generals[45].city = -1;
generals[45].magic[0] = 24;
generals[45].magic[1] = -1;
generals[45].magic[2] = -1;
generals[45].magic[3] = -1;
generals[45].equipment = -1;
generals[45].strength = 85;
generals[45].intellect = 40;
generals[45].experience = 200;
generals[45].level = 2;
generals[45].healthMax = 85;
generals[45].healthCur = 85;
generals[45].manaMax = 37;
generals[45].manaCur = 37;
generals[45].soldierMax = 10;
generals[45].soldierCur = 10;
generals[45].knightMax = 0;
generals[45].knightCur = 0;
generals[45].arms = 0x0080;
generals[45].formation = 0x03;

generals[46].king = -1;
generals[46].city = -1;
generals[46].magic[0] = 25;
generals[46].magic[1] = -1;
generals[46].magic[2] = -1;
generals[46].magic[3] = -1;
generals[46].equipment = -1;
generals[46].strength = 88;
generals[46].intellect = 54;
generals[46].experience = 200;
generals[46].level = 2;
generals[46].healthMax = 86;
generals[46].healthCur = 86;
generals[46].manaMax = 40;
generals[46].manaCur = 40;
generals[46].soldierMax = 10;
generals[46].soldierCur = 10;
generals[46].knightMax = 0;
generals[46].knightCur = 0;
generals[46].arms = 0x0080;
generals[46].formation = 0x05;

generals[47].king = -1;
generals[47].city = -1;
generals[47].magic[0] = 0;
generals[47].magic[1] = -1;
generals[47].magic[2] = -1;
generals[47].magic[3] = -1;
generals[47].equipment = -1;
generals[47].strength = 61;
generals[47].intellect = 94;
generals[47].experience = 200;
generals[47].level = 2;
generals[47].healthMax = 81;
generals[47].healthCur = 81;
generals[47].manaMax = 47;
generals[47].manaCur = 47;
generals[47].soldierMax = 10;
generals[47].soldierCur = 10;
generals[47].knightMax = 0;
generals[47].knightCur = 0;
generals[47].arms = 0x0100;
generals[47].formation = 0x12;

generals[48].king = 7;
generals[48].city = 6;
generals[48].magic[0] = 1;
generals[48].magic[1] = -1;
generals[48].magic[2] = -1;
generals[48].magic[3] = -1;
generals[48].equipment = -1;
generals[48].strength = 66;
generals[48].intellect = 90;
generals[48].experience = 200;
generals[48].level = 2;
generals[48].healthMax = 81;
generals[48].healthCur = 81;
generals[48].manaMax = 46;
generals[48].manaCur = 46;
generals[48].soldierMax = 10;
generals[48].soldierCur = 10;
generals[48].knightMax = 0;
generals[48].knightCur = 0;
generals[48].arms = 0x0100;
generals[48].formation = 0x18;

generals[49].king = -1;
generals[49].city = -1;
generals[49].magic[0] = 25;
generals[49].magic[1] = -1;
generals[49].magic[2] = -1;
generals[49].magic[3] = -1;
generals[49].equipment = -1;
generals[49].strength = 78;
generals[49].intellect = 40;
generals[49].experience = 200;
generals[49].level = 2;
generals[49].healthMax = 84;
generals[49].healthCur = 84;
generals[49].manaMax = 37;
generals[49].manaCur = 37;
generals[49].soldierMax = 10;
generals[49].soldierCur = 10;
generals[49].knightMax = 0;
generals[49].knightCur = 0;
generals[49].arms = 0x0200;
generals[49].formation = 0x11;

generals[50].king = -1;
generals[50].city = -1;
generals[50].magic[0] = 24;
generals[50].magic[1] = -1;
generals[50].magic[2] = -1;
generals[50].magic[3] = -1;
generals[50].equipment = -1;
generals[50].strength = 78;
generals[50].intellect = 42;
generals[50].experience = 200;
generals[50].level = 2;
generals[50].healthMax = 84;
generals[50].healthCur = 84;
generals[50].manaMax = 37;
generals[50].manaCur = 37;
generals[50].soldierMax = 10;
generals[50].soldierCur = 10;
generals[50].knightMax = 0;
generals[50].knightCur = 0;
generals[50].arms = 0x0200;
generals[50].formation = 0x11;

generals[51].king = -1;
generals[51].city = -1;
generals[51].magic[0] = 19;
generals[51].magic[1] = -1;
generals[51].magic[2] = -1;
generals[51].magic[3] = -1;
generals[51].equipment = -1;
generals[51].strength = 93;
generals[51].intellect = 96;
generals[51].experience = 200;
generals[51].level = 2;
generals[51].healthMax = 86;
generals[51].healthCur = 86;
generals[51].manaMax = 47;
generals[51].manaCur = 47;
generals[51].soldierMax = 10;
generals[51].soldierCur = 10;
generals[51].knightMax = 0;
generals[51].knightCur = 0;
generals[51].arms = 0x000A;
generals[51].formation = 0x2B;

generals[52].king = 5;
generals[52].city = 21;
generals[52].magic[0] = 22;
generals[52].magic[1] = -1;
generals[52].magic[2] = -1;
generals[52].magic[3] = -1;
generals[52].equipment = 19;
generals[52].strength = 89;
generals[52].intellect = 36;
generals[52].experience = 200;
generals[52].level = 2;
generals[52].healthMax = 85;
generals[52].healthCur = 85;
generals[52].manaMax = 36;
generals[52].manaCur = 36;
generals[52].soldierMax = 10;
generals[52].soldierCur = 10;
generals[52].knightMax = 0;
generals[52].knightCur = 0;
generals[52].arms = 0x0001;
generals[52].formation = 0x24;

generals[53].king = -1;
generals[53].city = -1;
generals[53].magic[0] = 20;
generals[53].magic[1] = -1;
generals[53].magic[2] = -1;
generals[53].magic[3] = -1;
generals[53].equipment = -1;
generals[53].strength = 74;
generals[53].intellect = 58;
generals[53].experience = 200;
generals[53].level = 2;
generals[53].healthMax = 83;
generals[53].healthCur = 83;
generals[53].manaMax = 40;
generals[53].manaCur = 40;
generals[53].soldierMax = 10;
generals[53].soldierCur = 10;
generals[53].knightMax = 0;
generals[53].knightCur = 0;
generals[53].arms = 0x0020;
generals[53].formation = 0x40;

generals[54].king = -1;
generals[54].city = -1;
generals[54].magic[0] = 27;
generals[54].magic[1] = -1;
generals[54].magic[2] = -1;
generals[54].magic[3] = -1;
generals[54].equipment = -1;
generals[54].strength = 83;
generals[54].intellect = 60;
generals[54].experience = 200;
generals[54].level = 2;
generals[54].healthMax = 85;
generals[54].healthCur = 85;
generals[54].manaMax = 41;
generals[54].manaCur = 41;
generals[54].soldierMax = 10;
generals[54].soldierCur = 10;
generals[54].knightMax = 0;
generals[54].knightCur = 0;
generals[54].arms = 0x0001;
generals[54].formation = 0x60;

generals[55].king = -1;
generals[55].city = 36;
generals[55].magic[0] = 23;
generals[55].magic[1] = -1;
generals[55].magic[2] = -1;
generals[55].magic[3] = -1;
generals[55].equipment = -1;
generals[55].strength = 82;
generals[55].intellect = 48;
generals[55].experience = 200;
generals[55].level = 2;
generals[55].healthMax = 85;
generals[55].healthCur = 85;
generals[55].manaMax = 38;
generals[55].manaCur = 38;
generals[55].soldierMax = 10;
generals[55].soldierCur = 10;
generals[55].knightMax = 0;
generals[55].knightCur = 0;
generals[55].arms = 0x0002;
generals[55].formation = 0x21;

generals[56].king = 0;
generals[56].city = 9;
generals[56].magic[0] = 28;
generals[56].magic[1] = -1;
generals[56].magic[2] = -1;
generals[56].magic[3] = -1;
generals[56].equipment = -1;
generals[56].strength = 96;
generals[56].intellect = 70;
generals[56].experience = 200;
generals[56].level = 2;
generals[56].healthMax = 87;
generals[56].healthCur = 87;
generals[56].manaMax = 42;
generals[56].manaCur = 42;
generals[56].soldierMax = 10;
generals[56].soldierCur = 10;
generals[56].knightMax = 0;
generals[56].knightCur = 0;
generals[56].arms = 0x0002;
generals[56].formation = 0x14;

generals[57].king = 0;
generals[57].city = 9;
generals[57].magic[0] = 24;
generals[57].magic[1] = -1;
generals[57].magic[2] = -1;
generals[57].magic[3] = -1;
generals[57].equipment = -1;
generals[57].strength = 93;
generals[57].intellect = 60;
generals[57].experience = 200;
generals[57].level = 2;
generals[57].healthMax = 86;
generals[57].healthCur = 86;
generals[57].manaMax = 41;
generals[57].manaCur = 41;
generals[57].soldierMax = 10;
generals[57].soldierCur = 10;
generals[57].knightMax = 0;
generals[57].knightCur = 0;
generals[57].arms = 0x0002;
generals[57].formation = 0x03;

generals[58].king = -1;
generals[58].city = -1;
generals[58].magic[0] = 22;
generals[58].magic[1] = -1;
generals[58].magic[2] = -1;
generals[58].magic[3] = -1;
generals[58].equipment = -1;
generals[58].strength = 90;
generals[58].intellect = 68;
generals[58].experience = 200;
generals[58].level = 2;
generals[58].healthMax = 86;
generals[58].healthCur = 86;
generals[58].manaMax = 42;
generals[58].manaCur = 42;
generals[58].soldierMax = 10;
generals[58].soldierCur = 10;
generals[58].knightMax = 0;
generals[58].knightCur = 0;
generals[58].arms = 0x0011;
generals[58].formation = 0x09;

generals[59].king = 2;
generals[59].city = 28;
generals[59].magic[0] = 26;
generals[59].magic[1] = -1;
generals[59].magic[2] = -1;
generals[59].magic[3] = -1;
generals[59].equipment = 15;
generals[59].strength = 101;
generals[59].intellect = 85;
generals[59].experience = 200;
generals[59].level = 2;
generals[59].healthMax = 88;
generals[59].healthCur = 88;
generals[59].manaMax = 45;
generals[59].manaCur = 45;
generals[59].soldierMax = 10;
generals[59].soldierCur = 10;
generals[59].knightMax = 0;
generals[59].knightCur = 0;
generals[59].arms = 0x0001;
generals[59].formation = 0x44;

generals[60].king = 2;
generals[60].city = 28;
generals[60].magic[0] = 19;
generals[60].magic[1] = -1;
generals[60].magic[2] = -1;
generals[60].magic[3] = -1;
generals[60].equipment = -1;
generals[60].strength = 95;
generals[60].intellect = 83;
generals[60].experience = 200;
generals[60].level = 2;
generals[60].healthMax = 87;
generals[60].healthCur = 87;
generals[60].manaMax = 45;
generals[60].manaCur = 45;
generals[60].soldierMax = 10;
generals[60].soldierCur = 10;
generals[60].knightMax = 0;
generals[60].knightCur = 0;
generals[60].arms = 0x0002;
generals[60].formation = 0x41;

generals[61].king = -1;
generals[61].city = -1;
generals[61].magic[0] = 27;
generals[61].magic[1] = -1;
generals[61].magic[2] = -1;
generals[61].magic[3] = -1;
generals[61].equipment = -1;
generals[61].strength = 85;
generals[61].intellect = 88;
generals[61].experience = 200;
generals[61].level = 2;
generals[61].healthMax = 85;
generals[61].healthCur = 85;
generals[61].manaMax = 46;
generals[61].manaCur = 46;
generals[61].soldierMax = 10;
generals[61].soldierCur = 10;
generals[61].knightMax = 0;
generals[61].knightCur = 0;
generals[61].arms = 0x0001;
generals[61].formation = 0x22;

generals[62].king = -1;
generals[62].city = -1;
generals[62].magic[0] = 21;
generals[62].magic[1] = -1;
generals[62].magic[2] = -1;
generals[62].magic[3] = -1;
generals[62].equipment = -1;
generals[62].strength = 81;
generals[62].intellect = 53;
generals[62].experience = 200;
generals[62].level = 2;
generals[62].healthMax = 85;
generals[62].healthCur = 85;
generals[62].manaMax = 39;
generals[62].manaCur = 39;
generals[62].soldierMax = 10;
generals[62].soldierCur = 10;
generals[62].knightMax = 0;
generals[62].knightCur = 0;
generals[62].arms = 0x0001;
generals[62].formation = 0x11;

generals[63].king = -1;
generals[63].city = 8;
generals[63].magic[0] = 22;
generals[63].magic[1] = -1;
generals[63].magic[2] = -1;
generals[63].magic[3] = -1;
generals[63].equipment = 14;
generals[63].strength = 100;
generals[63].intellect = 70;
generals[63].experience = 200;
generals[63].level = 2;
generals[63].healthMax = 87;
generals[63].healthCur = 87;
generals[63].manaMax = 42;
generals[63].manaCur = 42;
generals[63].soldierMax = 10;
generals[63].soldierCur = 10;
generals[63].knightMax = 0;
generals[63].knightCur = 0;
generals[63].arms = 0x0001;
generals[63].formation = 0x41;

generals[64].king = -1;
generals[64].city = -1;
generals[64].magic[0] = 1;
generals[64].magic[1] = -1;
generals[64].magic[2] = -1;
generals[64].magic[3] = -1;
generals[64].equipment = -1;
generals[64].strength = 68;
generals[64].intellect = 97;
generals[64].experience = 200;
generals[64].level = 2;
generals[64].healthMax = 82;
generals[64].healthCur = 82;
generals[64].manaMax = 47;
generals[64].manaCur = 47;
generals[64].soldierMax = 10;
generals[64].soldierCur = 10;
generals[64].knightMax = 0;
generals[64].knightCur = 0;
generals[64].arms = 0x0008;
generals[64].formation = 0x18;

generals[65].king = -1;
generals[65].city = -1;
generals[65].magic[0] = 26;
generals[65].magic[1] = -1;
generals[65].magic[2] = -1;
generals[65].magic[3] = -1;
generals[65].equipment = -1;
generals[65].strength = 84;
generals[65].intellect = 84;
generals[65].experience = 200;
generals[65].level = 2;
generals[65].healthMax = 85;
generals[65].healthCur = 85;
generals[65].manaMax = 45;
generals[65].manaCur = 45;
generals[65].soldierMax = 10;
generals[65].soldierCur = 10;
generals[65].knightMax = 0;
generals[65].knightCur = 0;
generals[65].arms = 0x0010;
generals[65].formation = 0x40;

generals[66].king = -1;
generals[66].city = -1;
generals[66].magic[0] = 21;
generals[66].magic[1] = -1;
generals[66].magic[2] = -1;
generals[66].magic[3] = -1;
generals[66].equipment = -1;
generals[66].strength = 66;
generals[66].intellect = 40;
generals[66].experience = 200;
generals[66].level = 2;
generals[66].healthMax = 81;
generals[66].healthCur = 81;
generals[66].manaMax = 37;
generals[66].manaCur = 37;
generals[66].soldierMax = 10;
generals[66].soldierCur = 10;
generals[66].knightMax = 0;
generals[66].knightCur = 0;
generals[66].arms = 0x0101;
generals[66].formation = 0x01;

generals[67].king = -1;
generals[67].city = -1;
generals[67].magic[0] = 19;
generals[67].magic[1] = -1;
generals[67].magic[2] = -1;
generals[67].magic[3] = -1;
generals[67].equipment = -1;
generals[67].strength = 83;
generals[67].intellect = 42;
generals[67].experience = 200;
generals[67].level = 2;
generals[67].healthMax = 85;
generals[67].healthCur = 85;
generals[67].manaMax = 37;
generals[67].manaCur = 37;
generals[67].soldierMax = 10;
generals[67].soldierCur = 10;
generals[67].knightMax = 0;
generals[67].knightCur = 0;
generals[67].arms = 0x0080;
generals[67].formation = 0x06;

generals[68].king = -1;
generals[68].city = 18;
generals[68].magic[0] = 1;
generals[68].magic[1] = -1;
generals[68].magic[2] = -1;
generals[68].magic[3] = -1;
generals[68].equipment = -1;
generals[68].strength = 52;
generals[68].intellect = 95;
generals[68].experience = 200;
generals[68].level = 2;
generals[68].healthMax = 79;
generals[68].healthCur = 79;
generals[68].manaMax = 47;
generals[68].manaCur = 47;
generals[68].soldierMax = 10;
generals[68].soldierCur = 10;
generals[68].knightMax = 0;
generals[68].knightCur = 0;
generals[68].arms = 0x0100;
generals[68].formation = 0x22;

generals[69].king = -1;
generals[69].city = 18;
generals[69].magic[0] = 4;
generals[69].magic[1] = -1;
generals[69].magic[2] = -1;
generals[69].magic[3] = -1;
generals[69].equipment = -1;
generals[69].strength = 46;
generals[69].intellect = 98;
generals[69].experience = 200;
generals[69].level = 2;
generals[69].healthMax = 77;
generals[69].healthCur = 77;
generals[69].manaMax = 47;
generals[69].manaCur = 47;
generals[69].soldierMax = 10;
generals[69].soldierCur = 10;
generals[69].knightMax = 0;
generals[69].knightCur = 0;
generals[69].arms = 0x0008;
generals[69].formation = 0x0A;

generals[70].king = 4;
generals[70].city = 4;
generals[70].magic[0] = 24;
generals[70].magic[1] = -1;
generals[70].magic[2] = -1;
generals[70].magic[3] = -1;
generals[70].equipment = -1;
generals[70].strength = 71;
generals[70].intellect = 50;
generals[70].experience = 200;
generals[70].level = 2;
generals[70].healthMax = 82;
generals[70].healthCur = 82;
generals[70].manaMax = 39;
generals[70].manaCur = 39;
generals[70].soldierMax = 10;
generals[70].soldierCur = 10;
generals[70].knightMax = 0;
generals[70].knightCur = 0;
generals[70].arms = 0x0020;
generals[70].formation = 0x05;

generals[71].king = 4;
generals[71].city = 4;
generals[71].magic[0] = 22;
generals[71].magic[1] = -1;
generals[71].magic[2] = -1;
generals[71].magic[3] = -1;
generals[71].equipment = -1;
generals[71].strength = 81;
generals[71].intellect = 77;
generals[71].experience = 200;
generals[71].level = 2;
generals[71].healthMax = 85;
generals[71].healthCur = 85;
generals[71].manaMax = 44;
generals[71].manaCur = 44;
generals[71].soldierMax = 10;
generals[71].soldierCur = 10;
generals[71].knightMax = 0;
generals[71].knightCur = 0;
generals[71].arms = 0x0001;
generals[71].formation = 0x0A;

generals[72].king = 5;
generals[72].city = 21;
generals[72].magic[0] = 19;
generals[72].magic[1] = -1;
generals[72].magic[2] = -1;
generals[72].magic[3] = -1;
generals[72].equipment = -1;
generals[72].strength = 75;
generals[72].intellect = 70;
generals[72].experience = 200;
generals[72].level = 2;
generals[72].healthMax = 84;
generals[72].healthCur = 84;
generals[72].manaMax = 42;
generals[72].manaCur = 42;
generals[72].soldierMax = 10;
generals[72].soldierCur = 10;
generals[72].knightMax = 0;
generals[72].knightCur = 0;
generals[72].arms = 0x0008;
generals[72].formation = 0x11;

generals[73].king = 4;
generals[73].city = 4;
generals[73].magic[0] = 27;
generals[73].magic[1] = -1;
generals[73].magic[2] = -1;
generals[73].magic[3] = -1;
generals[73].equipment = -1;
generals[73].strength = 64;
generals[73].intellect = 70;
generals[73].experience = 200;
generals[73].level = 2;
generals[73].healthMax = 81;
generals[73].healthCur = 81;
generals[73].manaMax = 42;
generals[73].manaCur = 42;
generals[73].soldierMax = 10;
generals[73].soldierCur = 10;
generals[73].knightMax = 0;
generals[73].knightCur = 0;
generals[73].arms = 0x0100;
generals[73].formation = 0x05;

generals[74].king = 4;
generals[74].city = 4;
generals[74].magic[0] = 21;
generals[74].magic[1] = -1;
generals[74].magic[2] = -1;
generals[74].magic[3] = -1;
generals[74].equipment = -1;
generals[74].strength = 65;
generals[74].intellect = 52;
generals[74].experience = 200;
generals[74].level = 2;
generals[74].healthMax = 81;
generals[74].healthCur = 81;
generals[74].manaMax = 39;
generals[74].manaCur = 39;
generals[74].soldierMax = 10;
generals[74].soldierCur = 10;
generals[74].knightMax = 0;
generals[74].knightCur = 0;
generals[74].arms = 0x0001;
generals[74].formation = 0x12;

generals[75].king = -1;
generals[75].city = -1;
generals[75].magic[0] = 24;
generals[75].magic[1] = -1;
generals[75].magic[2] = -1;
generals[75].magic[3] = -1;
generals[75].equipment = -1;
generals[75].strength = 84;
generals[75].intellect = 89;
generals[75].experience = 200;
generals[75].level = 2;
generals[75].healthMax = 85;
generals[75].healthCur = 85;
generals[75].manaMax = 46;
generals[75].manaCur = 46;
generals[75].soldierMax = 10;
generals[75].soldierCur = 10;
generals[75].knightMax = 0;
generals[75].knightCur = 0;
generals[75].arms = 0x0100;
generals[75].formation = 0x03;

generals[76].king = -1;
generals[76].city = -1;
generals[76].magic[0] = 5;
generals[76].magic[1] = -1;
generals[76].magic[2] = -1;
generals[76].magic[3] = -1;
generals[76].equipment = -1;
generals[76].strength = 42;
generals[76].intellect = 92;
generals[76].experience = 200;
generals[76].level = 2;
generals[76].healthMax = 77;
generals[76].healthCur = 77;
generals[76].manaMax = 46;
generals[76].manaCur = 46;
generals[76].soldierMax = 10;
generals[76].soldierCur = 10;
generals[76].knightMax = 0;
generals[76].knightCur = 0;
generals[76].arms = 0x0008;
generals[76].formation = 0x32;

generals[77].king = -1;
generals[77].city = -1;
generals[77].magic[0] = 27;
generals[77].magic[1] = -1;
generals[77].magic[2] = -1;
generals[77].magic[3] = -1;
generals[77].equipment = -1;
generals[77].strength = 85;
generals[77].intellect = 52;
generals[77].experience = 200;
generals[77].level = 2;
generals[77].healthMax = 85;
generals[77].healthCur = 85;
generals[77].manaMax = 39;
generals[77].manaCur = 39;
generals[77].soldierMax = 10;
generals[77].soldierCur = 10;
generals[77].knightMax = 0;
generals[77].knightCur = 0;
generals[77].arms = 0x0002;
generals[77].formation = 0x44;

generals[78].king = -1;
generals[78].city = -1;
generals[78].magic[0] = 25;
generals[78].magic[1] = -1;
generals[78].magic[2] = -1;
generals[78].magic[3] = -1;
generals[78].equipment = -1;
generals[78].strength = 98;
generals[78].intellect = 48;
generals[78].experience = 200;
generals[78].level = 2;
generals[78].healthMax = 87;
generals[78].healthCur = 87;
generals[78].manaMax = 38;
generals[78].manaCur = 38;
generals[78].soldierMax = 10;
generals[78].soldierCur = 10;
generals[78].knightMax = 0;
generals[78].knightCur = 0;
generals[78].arms = 0x0004;
generals[78].formation = 0x05;

generals[79].king = 10;
generals[79].city = 15;
generals[79].magic[0] = 26;
generals[79].magic[1] = -1;
generals[79].magic[2] = -1;
generals[79].magic[3] = -1;
generals[79].equipment = -1;
generals[79].strength = 91;
generals[79].intellect = 61;
generals[79].experience = 200;
generals[79].level = 2;
generals[79].healthMax = 86;
generals[79].healthCur = 86;
generals[79].manaMax = 41;
generals[79].manaCur = 41;
generals[79].soldierMax = 10;
generals[79].soldierCur = 10;
generals[79].knightMax = 0;
generals[79].knightCur = 0;
generals[79].arms = 0x0010;
generals[79].formation = 0x0C;

generals[80].king = -1;
generals[80].city = -1;
generals[80].magic[0] = 21;
generals[80].magic[1] = -1;
generals[80].magic[2] = -1;
generals[80].magic[3] = -1;
generals[80].equipment = -1;
generals[80].strength = 74;
generals[80].intellect = 86;
generals[80].experience = 200;
generals[80].level = 2;
generals[80].healthMax = 83;
generals[80].healthCur = 83;
generals[80].manaMax = 45;
generals[80].manaCur = 45;
generals[80].soldierMax = 10;
generals[80].soldierCur = 10;
generals[80].knightMax = 0;
generals[80].knightCur = 0;
generals[80].arms = 0x0009;
generals[80].formation = 0x29;

generals[81].king = -1;
generals[81].city = 8;
generals[81].magic[0] = 24;
generals[81].magic[1] = -1;
generals[81].magic[2] = -1;
generals[81].magic[3] = -1;
generals[81].equipment = -1;
generals[81].strength = 84;
generals[81].intellect = 59;
generals[81].experience = 200;
generals[81].level = 2;
generals[81].healthMax = 85;
generals[81].healthCur = 85;
generals[81].manaMax = 40;
generals[81].manaCur = 40;
generals[81].soldierMax = 10;
generals[81].soldierCur = 10;
generals[81].knightMax = 0;
generals[81].knightCur = 0;
generals[81].arms = 0x0001;
generals[81].formation = 0x41;

generals[82].king = 4;
generals[82].city = 4;
generals[82].magic[0] = 21;
generals[82].magic[1] = -1;
generals[82].magic[2] = -1;
generals[82].magic[3] = -1;
generals[82].equipment = -1;
generals[82].strength = 75;
generals[82].intellect = 55;
generals[82].experience = 200;
generals[82].level = 2;
generals[82].healthMax = 84;
generals[82].healthCur = 84;
generals[82].manaMax = 40;
generals[82].manaCur = 40;
generals[82].soldierMax = 10;
generals[82].soldierCur = 10;
generals[82].knightMax = 0;
generals[82].knightCur = 0;
generals[82].arms = 0x0020;
generals[82].formation = 0x01;

generals[83].king = 17;
generals[83].city = 40;
generals[83].magic[0] = 26;
generals[83].magic[1] = -1;
generals[83].magic[2] = -1;
generals[83].magic[3] = -1;
generals[83].equipment = -1;
generals[83].strength = 89;
generals[83].intellect = 75;
generals[83].experience = 200;
generals[83].level = 2;
generals[83].healthMax = 86;
generals[83].healthCur = 86;
generals[83].manaMax = 44;
generals[83].manaCur = 44;
generals[83].soldierMax = 10;
generals[83].soldierCur = 10;
generals[83].knightMax = 0;
generals[83].knightCur = 0;
generals[83].arms = 0x0010;
generals[83].formation = 0x50;

generals[84].king = -1;
generals[84].city = -1;
generals[84].magic[0] = 0;
generals[84].magic[1] = -1;
generals[84].magic[2] = -1;
generals[84].magic[3] = -1;
generals[84].equipment = -1;
generals[84].strength = 37;
generals[84].intellect = 92;
generals[84].experience = 200;
generals[84].level = 2;
generals[84].healthMax = 76;
generals[84].healthCur = 76;
generals[84].manaMax = 46;
generals[84].manaCur = 46;
generals[84].soldierMax = 10;
generals[84].soldierCur = 10;
generals[84].knightMax = 0;
generals[84].knightCur = 0;
generals[84].arms = 0x0040;
generals[84].formation = 0x18;

generals[85].king = -1;
generals[85].city = -1;
generals[85].magic[0] = 20;
generals[85].magic[1] = -1;
generals[85].magic[2] = -1;
generals[85].magic[3] = -1;
generals[85].equipment = -1;
generals[85].strength = 72;
generals[85].intellect = 55;
generals[85].experience = 200;
generals[85].level = 2;
generals[85].healthMax = 82;
generals[85].healthCur = 82;
generals[85].manaMax = 40;
generals[85].manaCur = 40;
generals[85].soldierMax = 10;
generals[85].soldierCur = 10;
generals[85].knightMax = 0;
generals[85].knightCur = 0;
generals[85].arms = 0x0120;
generals[85].formation = 0x02;

generals[86].king = -1;
generals[86].city = -1;
generals[86].magic[0] = 4;
generals[86].magic[1] = -1;
generals[86].magic[2] = -1;
generals[86].magic[3] = -1;
generals[86].equipment = -1;
generals[86].strength = 36;
generals[86].intellect = 94;
generals[86].experience = 200;
generals[86].level = 2;
generals[86].healthMax = 76;
generals[86].healthCur = 76;
generals[86].manaMax = 47;
generals[86].manaCur = 47;
generals[86].soldierMax = 10;
generals[86].soldierCur = 10;
generals[86].knightMax = 0;
generals[86].knightCur = 0;
generals[86].arms = 0x0008;
generals[86].formation = 0x0B;

generals[87].king = -1;
generals[87].city = -1;
generals[87].magic[0] = 23;
generals[87].magic[1] = -1;
generals[87].magic[2] = -1;
generals[87].magic[3] = -1;
generals[87].equipment = -1;
generals[87].strength = 88;
generals[87].intellect = 45;
generals[87].experience = 200;
generals[87].level = 2;
generals[87].healthMax = 86;
generals[87].healthCur = 86;
generals[87].manaMax = 37;
generals[87].manaCur = 37;
generals[87].soldierMax = 10;
generals[87].soldierCur = 10;
generals[87].knightMax = 0;
generals[87].knightCur = 0;
generals[87].arms = 0x0012;
generals[87].formation = 0x05;

generals[88].king = 4;
generals[88].city = 2;
generals[88].magic[0] = 24;
generals[88].magic[1] = -1;
generals[88].magic[2] = -1;
generals[88].magic[3] = -1;
generals[88].equipment = -1;
generals[88].strength = 93;
generals[88].intellect = 67;
generals[88].experience = 200;
generals[88].level = 2;
generals[88].healthMax = 86;
generals[88].healthCur = 86;
generals[88].manaMax = 42;
generals[88].manaCur = 42;
generals[88].soldierMax = 10;
generals[88].soldierCur = 10;
generals[88].knightMax = 0;
generals[88].knightCur = 0;
generals[88].arms = 0x0001;
generals[88].formation = 0x11;

generals[89].king = 1;
generals[89].city = 5;
generals[89].magic[0] = 23;
generals[89].magic[1] = -1;
generals[89].magic[2] = -1;
generals[89].magic[3] = -1;
generals[89].equipment = 10;
generals[89].strength = 108;
generals[89].intellect = 51;
generals[89].experience = 200;
generals[89].level = 2;
generals[89].healthMax = 88;
generals[89].healthCur = 88;
generals[89].manaMax = 39;
generals[89].manaCur = 39;
generals[89].soldierMax = 10;
generals[89].soldierCur = 10;
generals[89].knightMax = 0;
generals[89].knightCur = 0;
generals[89].arms = 0x0002;
generals[89].formation = 0x05;

generals[90].king = -1;
generals[90].city = 37;
generals[90].magic[0] = 5;
generals[90].magic[1] = -1;
generals[90].magic[2] = -1;
generals[90].magic[3] = -1;
generals[90].equipment = -1;
generals[90].strength = 65;
generals[90].intellect = 83;
generals[90].experience = 200;
generals[90].level = 2;
generals[90].healthMax = 81;
generals[90].healthCur = 81;
generals[90].manaMax = 45;
generals[90].manaCur = 45;
generals[90].soldierMax = 10;
generals[90].soldierCur = 10;
generals[90].knightMax = 0;
generals[90].knightCur = 0;
generals[90].arms = 0x0100;
generals[90].formation = 0x60;

generals[91].king = 3;
generals[91].city = 10;
generals[91].magic[0] = 26;
generals[91].magic[1] = -1;
generals[91].magic[2] = -1;
generals[91].magic[3] = -1;
generals[91].equipment = -1;
generals[91].strength = 95;
generals[91].intellect = 88;
generals[91].experience = 200;
generals[91].level = 2;
generals[91].healthMax = 87;
generals[91].healthCur = 87;
generals[91].manaMax = 46;
generals[91].manaCur = 46;
generals[91].soldierMax = 10;
generals[91].soldierCur = 10;
generals[91].knightMax = 0;
generals[91].knightCur = 0;
generals[91].arms = 0x0002;
generals[91].formation = 0x05;

generals[92].king = -1;
generals[92].city = -1;
generals[92].magic[0] = 2;
generals[92].magic[1] = -1;
generals[92].magic[2] = -1;
generals[92].magic[3] = -1;
generals[92].equipment = -1;
generals[92].strength = 35;
generals[92].intellect = 89;
generals[92].experience = 200;
generals[92].level = 2;
generals[92].healthMax = 76;
generals[92].healthCur = 76;
generals[92].manaMax = 46;
generals[92].manaCur = 46;
generals[92].soldierMax = 10;
generals[92].soldierCur = 10;
generals[92].knightMax = 0;
generals[92].knightCur = 0;
generals[92].arms = 0x0008;
generals[92].formation = 0x22;

generals[93].king = 0;
generals[93].city = 9;
generals[93].magic[0] = 26;
generals[93].magic[1] = -1;
generals[93].magic[2] = -1;
generals[93].magic[3] = -1;
generals[93].equipment = -1;
generals[93].strength = 90;
generals[93].intellect = 82;
generals[93].experience = 200;
generals[93].level = 2;
generals[93].healthMax = 86;
generals[93].healthCur = 86;
generals[93].manaMax = 45;
generals[93].manaCur = 45;
generals[93].soldierMax = 10;
generals[93].soldierCur = 10;
generals[93].knightMax = 0;
generals[93].knightCur = 0;
generals[93].arms = 0x0001;
generals[93].formation = 0x0C;

generals[94].king = -1;
generals[94].city = -1;
generals[94].magic[0] = 2;
generals[94].magic[1] = -1;
generals[94].magic[2] = -1;
generals[94].magic[3] = -1;
generals[94].equipment = -1;
generals[94].strength = 57;
generals[94].intellect = 42;
generals[94].experience = 200;
generals[94].level = 2;
generals[94].healthMax = 80;
generals[94].healthCur = 80;
generals[94].manaMax = 37;
generals[94].manaCur = 37;
generals[94].soldierMax = 10;
generals[94].soldierCur = 10;
generals[94].knightMax = 0;
generals[94].knightCur = 0;
generals[94].arms = 0x0048;
generals[94].formation = 0x0A;

generals[95].king = -1;
generals[95].city = -1;
generals[95].magic[0] = 0;
generals[95].magic[1] = -1;
generals[95].magic[2] = -1;
generals[95].magic[3] = -1;
generals[95].equipment = -1;
generals[95].strength = 66;
generals[95].intellect = 44;
generals[95].experience = 200;
generals[95].level = 2;
generals[95].healthMax = 81;
generals[95].healthCur = 81;
generals[95].manaMax = 37;
generals[95].manaCur = 37;
generals[95].soldierMax = 10;
generals[95].soldierCur = 10;
generals[95].knightMax = 0;
generals[95].knightCur = 0;
generals[95].arms = 0x0060;
generals[95].formation = 0x42;

generals[96].king = -1;
generals[96].city = -1;
generals[96].magic[0] = 0;
generals[96].magic[1] = -1;
generals[96].magic[2] = -1;
generals[96].magic[3] = -1;
generals[96].equipment = -1;
generals[96].strength = 32;
generals[96].intellect = 91;
generals[96].experience = 200;
generals[96].level = 2;
generals[96].healthMax = 75;
generals[96].healthCur = 75;
generals[96].manaMax = 46;
generals[96].manaCur = 46;
generals[96].soldierMax = 10;
generals[96].soldierCur = 10;
generals[96].knightMax = 0;
generals[96].knightCur = 0;
generals[96].arms = 0x0060;
generals[96].formation = 0x38;

generals[97].king = -1;
generals[97].city = -1;
generals[97].magic[0] = 23;
generals[97].magic[1] = -1;
generals[97].magic[2] = -1;
generals[97].magic[3] = -1;
generals[97].equipment = -1;
generals[97].strength = 92;
generals[97].intellect = 45;
generals[97].experience = 200;
generals[97].level = 2;
generals[97].healthMax = 86;
generals[97].healthCur = 86;
generals[97].manaMax = 37;
generals[97].manaCur = 37;
generals[97].soldierMax = 10;
generals[97].soldierCur = 10;
generals[97].knightMax = 0;
generals[97].knightCur = 0;
generals[97].arms = 0x0014;
generals[97].formation = 0x14;

generals[98].king = 0;
generals[98].city = 9;
generals[98].magic[0] = 19;
generals[98].magic[1] = -1;
generals[98].magic[2] = -1;
generals[98].magic[3] = -1;
generals[98].equipment = 11;
generals[98].strength = 96;
generals[98].intellect = 96;
generals[98].experience = 200;
generals[98].level = 2;
generals[98].healthMax = 86;
generals[98].healthCur = 86;
generals[98].manaMax = 47;
generals[98].manaCur = 47;
generals[98].soldierMax = 10;
generals[98].soldierCur = 10;
generals[98].knightMax = 0;
generals[98].knightCur = 0;
generals[98].arms = 0x0001;
generals[98].formation = 0x12;

generals[99].king = -1;
generals[99].city = -1;
generals[99].magic[0] = 5;
generals[99].magic[1] = -1;
generals[99].magic[2] = -1;
generals[99].magic[3] = -1;
generals[99].equipment = -1;
generals[99].strength = 59;
generals[99].intellect = 81;
generals[99].experience = 200;
generals[99].level = 2;
generals[99].healthMax = 80;
generals[99].healthCur = 80;
generals[99].manaMax = 45;
generals[99].manaCur = 45;
generals[99].soldierMax = 10;
generals[99].soldierCur = 10;
generals[99].knightMax = 0;
generals[99].knightCur = 0;
generals[99].arms = 0x0140;
generals[99].formation = 0x58;

generals[100].king = -1;
generals[100].city = -1;
generals[100].magic[0] = 23;
generals[100].magic[1] = -1;
generals[100].magic[2] = -1;
generals[100].magic[3] = -1;
generals[100].equipment = -1;
generals[100].strength = 98;
generals[100].intellect = 40;
generals[100].experience = 200;
generals[100].level = 2;
generals[100].healthMax = 87;
generals[100].healthCur = 87;
generals[100].manaMax = 37;
generals[100].manaCur = 37;
generals[100].soldierMax = 10;
generals[100].soldierCur = 10;
generals[100].knightMax = 0;
generals[100].knightCur = 0;
generals[100].arms = 0x0050;
generals[100].formation = 0x05;

generals[101].king = 4;
generals[101].city = 2;
generals[101].magic[0] = 4;
generals[101].magic[1] = -1;
generals[101].magic[2] = -1;
generals[101].magic[3] = -1;
generals[101].equipment = -1;
generals[101].strength = 63;
generals[101].intellect = 85;
generals[101].experience = 200;
generals[101].level = 2;
generals[101].healthMax = 81;
generals[101].healthCur = 81;
generals[101].manaMax = 45;
generals[101].manaCur = 45;
generals[101].soldierMax = 10;
generals[101].soldierCur = 10;
generals[101].knightMax = 0;
generals[101].knightCur = 0;
generals[101].arms = 0x0008;
generals[101].formation = 0x28;

generals[102].king = -1;
generals[102].city = -1;
generals[102].magic[0] = 1;
generals[102].magic[1] = -1;
generals[102].magic[2] = -1;
generals[102].magic[3] = -1;
generals[102].equipment = -1;
generals[102].strength = 49;
generals[102].intellect = 98;
generals[102].experience = 200;
generals[102].level = 2;
generals[102].healthMax = 78;
generals[102].healthCur = 78;
generals[102].manaMax = 47;
generals[102].manaCur = 47;
generals[102].soldierMax = 10;
generals[102].soldierCur = 10;
generals[102].knightMax = 0;
generals[102].knightCur = 0;
generals[102].arms = 0x0008;
generals[102].formation = 0x19;

generals[103].king = 4;
generals[103].city = 2;
generals[103].magic[0] = 0;
generals[103].magic[1] = -1;
generals[103].magic[2] = -1;
generals[103].magic[3] = -1;
generals[103].equipment = -1;
generals[103].strength = 49;
generals[103].intellect = 80;
generals[103].experience = 200;
generals[103].level = 2;
generals[103].healthMax = 78;
generals[103].healthCur = 78;
generals[103].manaMax = 45;
generals[103].manaCur = 45;
generals[103].soldierMax = 10;
generals[103].soldierCur = 10;
generals[103].knightMax = 0;
generals[103].knightCur = 0;
generals[103].arms = 0x0100;
generals[103].formation = 0x42;

generals[104].king = 3;
generals[104].city = 10;
generals[104].magic[0] = 27;
generals[104].magic[1] = -1;
generals[104].magic[2] = -1;
generals[104].magic[3] = -1;
generals[104].equipment = -1;
generals[104].strength = 73;
generals[104].intellect = 42;
generals[104].experience = 200;
generals[104].level = 2;
generals[104].healthMax = 82;
generals[104].healthCur = 82;
generals[104].manaMax = 37;
generals[104].manaCur = 37;
generals[104].soldierMax = 10;
generals[104].soldierCur = 10;
generals[104].knightMax = 0;
generals[104].knightCur = 0;
generals[104].arms = 0x0020;
generals[104].formation = 0x06;

generals[105].king = -1;
generals[105].city = 19;
generals[105].magic[0] = 5;
generals[105].magic[1] = -1;
generals[105].magic[2] = -1;
generals[105].magic[3] = -1;
generals[105].equipment = -1;
generals[105].strength = 66;
generals[105].intellect = 90;
generals[105].experience = 200;
generals[105].level = 2;
generals[105].healthMax = 81;
generals[105].healthCur = 81;
generals[105].manaMax = 46;
generals[105].manaCur = 46;
generals[105].soldierMax = 10;
generals[105].soldierCur = 10;
generals[105].knightMax = 0;
generals[105].knightCur = 0;
generals[105].arms = 0x0008;
generals[105].formation = 0x50;

generals[106].king = 4;
generals[106].city = 2;
generals[106].magic[0] = 2;
generals[106].magic[1] = -1;
generals[106].magic[2] = -1;
generals[106].magic[3] = -1;
generals[106].equipment = -1;
generals[106].strength = 38;
generals[106].intellect = 84;
generals[106].experience = 200;
generals[106].level = 2;
generals[106].healthMax = 76;
generals[106].healthCur = 76;
generals[106].manaMax = 45;
generals[106].manaCur = 45;
generals[106].soldierMax = 10;
generals[106].soldierCur = 10;
generals[106].knightMax = 0;
generals[106].knightCur = 0;
generals[106].arms = 0x0100;
generals[106].formation = 0x41;

generals[107].king = -1;
generals[107].city = -1;
generals[107].magic[0] = 0;
generals[107].magic[1] = -1;
generals[107].magic[2] = -1;
generals[107].magic[3] = -1;
generals[107].equipment = -1;
generals[107].strength = 48;
generals[107].intellect = 93;
generals[107].experience = 200;
generals[107].level = 2;
generals[107].healthMax = 78;
generals[107].healthCur = 78;
generals[107].manaMax = 46;
generals[107].manaCur = 46;
generals[107].soldierMax = 10;
generals[107].soldierCur = 10;
generals[107].knightMax = 0;
generals[107].knightCur = 0;
generals[107].arms = 0x0040;
generals[107].formation = 0x30;

generals[108].king = -1;
generals[108].city = -1;
generals[108].magic[0] = 26;
generals[108].magic[1] = -1;
generals[108].magic[2] = -1;
generals[108].magic[3] = -1;
generals[108].equipment = -1;
generals[108].strength = 78;
generals[108].intellect = 94;
generals[108].experience = 200;
generals[108].level = 2;
generals[108].healthMax = 84;
generals[108].healthCur = 84;
generals[108].manaMax = 47;
generals[108].manaCur = 47;
generals[108].soldierMax = 10;
generals[108].soldierCur = 10;
generals[108].knightMax = 0;
generals[108].knightCur = 0;
generals[108].arms = 0x0100;
generals[108].formation = 0x11;

generals[109].king = -1;
generals[109].city = -1;
generals[109].magic[0] = 26;
generals[109].magic[1] = -1;
generals[109].magic[2] = -1;
generals[109].magic[3] = -1;
generals[109].equipment = -1;
generals[109].strength = 79;
generals[109].intellect = 97;
generals[109].experience = 200;
generals[109].level = 2;
generals[109].healthMax = 84;
generals[109].healthCur = 84;
generals[109].manaMax = 47;
generals[109].manaCur = 47;
generals[109].soldierMax = 10;
generals[109].soldierCur = 10;
generals[109].knightMax = 0;
generals[109].knightCur = 0;
generals[109].arms = 0x0001;
generals[109].formation = 0x21;

generals[110].king = 11;
generals[110].city = 17;
generals[110].magic[0] = 0;
generals[110].magic[1] = -1;
generals[110].magic[2] = -1;
generals[110].magic[3] = -1;
generals[110].equipment = -1;
generals[110].strength = 51;
generals[110].intellect = 70;
generals[110].experience = 200;
generals[110].level = 2;
generals[110].healthMax = 79;
generals[110].healthCur = 79;
generals[110].manaMax = 42;
generals[110].manaCur = 42;
generals[110].soldierMax = 10;
generals[110].soldierCur = 10;
generals[110].knightMax = 0;
generals[110].knightCur = 0;
generals[110].arms = 0x0008;
generals[110].formation = 0x22;

generals[111].king = -1;
generals[111].city = 6;
generals[111].magic[0] = 1;
generals[111].magic[1] = -1;
generals[111].magic[2] = -1;
generals[111].magic[3] = -1;
generals[111].equipment = -1;
generals[111].strength = 58;
generals[111].intellect = 93;
generals[111].experience = 200;
generals[111].level = 2;
generals[111].healthMax = 80;
generals[111].healthCur = 80;
generals[111].manaMax = 46;
generals[111].manaCur = 46;
generals[111].soldierMax = 10;
generals[111].soldierCur = 10;
generals[111].knightMax = 0;
generals[111].knightCur = 0;
generals[111].arms = 0x0008;
generals[111].formation = 0x12;

generals[112].king = 2;
generals[112].city = 28;
generals[112].magic[0] = 19;
generals[112].magic[1] = -1;
generals[112].magic[2] = -1;
generals[112].magic[3] = -1;
generals[112].equipment = 20;
generals[112].strength = 88;
generals[112].intellect = 80;
generals[112].experience = 200;
generals[112].level = 2;
generals[112].healthMax = 85;
generals[112].healthCur = 85;
generals[112].manaMax = 45;
generals[112].manaCur = 45;
generals[112].soldierMax = 10;
generals[112].soldierCur = 10;
generals[112].knightMax = 0;
generals[112].knightCur = 0;
generals[112].arms = 0x0002;
generals[112].formation = 0x41;

generals[113].king = 3;
generals[113].city = 11;
generals[113].magic[0] = 23;
generals[113].magic[1] = -1;
generals[113].magic[2] = -1;
generals[113].magic[3] = -1;
generals[113].equipment = -1;
generals[113].strength = 90;
generals[113].intellect = 45;
generals[113].experience = 200;
generals[113].level = 2;
generals[113].healthMax = 86;
generals[113].healthCur = 86;
generals[113].manaMax = 37;
generals[113].manaCur = 37;
generals[113].soldierMax = 10;
generals[113].soldierCur = 10;
generals[113].knightMax = 0;
generals[113].knightCur = 0;
generals[113].arms = 0x0001;
generals[113].formation = 0x41;

generals[114].king = 3;
generals[114].city = 12;
generals[114].magic[0] = 2;
generals[114].magic[1] = -1;
generals[114].magic[2] = -1;
generals[114].magic[3] = -1;
generals[114].equipment = -1;
generals[114].strength = 45;
generals[114].intellect = 80;
generals[114].experience = 200;
generals[114].level = 2;
generals[114].healthMax = 77;
generals[114].healthCur = 77;
generals[114].manaMax = 45;
generals[114].manaCur = 45;
generals[114].soldierMax = 10;
generals[114].soldierCur = 10;
generals[114].knightMax = 0;
generals[114].knightCur = 0;
generals[114].arms = 0x0040;
generals[114].formation = 0x11;

generals[115].king = -1;
generals[115].city = 28;
generals[115].magic[0] = 24;
generals[115].magic[1] = -1;
generals[115].magic[2] = -1;
generals[115].magic[3] = -1;
generals[115].equipment = -1;
generals[115].strength = 97;
generals[115].intellect = 66;
generals[115].experience = 200;
generals[115].level = 2;
generals[115].healthMax = 87;
generals[115].healthCur = 87;
generals[115].manaMax = 41;
generals[115].manaCur = 41;
generals[115].soldierMax = 10;
generals[115].soldierCur = 10;
generals[115].knightMax = 0;
generals[115].knightCur = 0;
generals[115].arms = 0x0001;
generals[115].formation = 0x09;

generals[116].king = 13;
generals[116].city = 25;
generals[116].magic[0] = 22;
generals[116].magic[1] = -1;
generals[116].magic[2] = -1;
generals[116].magic[3] = -1;
generals[116].equipment = -1;
generals[116].strength = 74;
generals[116].intellect = 38;
generals[116].experience = 200;
generals[116].level = 2;
generals[116].healthMax = 83;
generals[116].healthCur = 83;
generals[116].manaMax = 36;
generals[116].manaCur = 36;
generals[116].soldierMax = 10;
generals[116].soldierCur = 10;
generals[116].knightMax = 0;
generals[116].knightCur = 0;
generals[116].arms = 0x0020;
generals[116].formation = 0x0A;

generals[117].king = 2;
generals[117].city = 28;
generals[117].magic[0] = 27;
generals[117].magic[1] = -1;
generals[117].magic[2] = -1;
generals[117].magic[3] = -1;
generals[117].equipment = -1;
generals[117].strength = 88;
generals[117].intellect = 67;
generals[117].experience = 200;
generals[117].level = 2;
generals[117].healthMax = 86;
generals[117].healthCur = 86;
generals[117].manaMax = 42;
generals[117].manaCur = 42;
generals[117].soldierMax = 10;
generals[117].soldierCur = 10;
generals[117].knightMax = 0;
generals[117].knightCur = 0;
generals[117].arms = 0x0008;
generals[117].formation = 0x18;

generals[118].king = -1;
generals[118].city = 40;
generals[118].magic[0] = 5;
generals[118].magic[1] = -1;
generals[118].magic[2] = -1;
generals[118].magic[3] = -1;
generals[118].equipment = -1;
generals[118].strength = 50;
generals[118].intellect = 82;
generals[118].experience = 200;
generals[118].level = 2;
generals[118].healthMax = 79;
generals[118].healthCur = 79;
generals[118].manaMax = 45;
generals[118].manaCur = 45;
generals[118].soldierMax = 10;
generals[118].soldierCur = 10;
generals[118].knightMax = 0;
generals[118].knightCur = 0;
generals[118].arms = 0x0100;
generals[118].formation = 0x41;

generals[119].king = -1;
generals[119].city = -1;
generals[119].magic[0] = 4;
generals[119].magic[1] = -1;
generals[119].magic[2] = -1;
generals[119].magic[3] = -1;
generals[119].equipment = -1;
generals[119].strength = 45;
generals[119].intellect = 92;
generals[119].experience = 200;
generals[119].level = 2;
generals[119].healthMax = 77;
generals[119].healthCur = 77;
generals[119].manaMax = 46;
generals[119].manaCur = 46;
generals[119].soldierMax = 10;
generals[119].soldierCur = 10;
generals[119].knightMax = 0;
generals[119].knightCur = 0;
generals[119].arms = 0x0040;
generals[119].formation = 0x42;

generals[120].king = -1;
generals[120].city = -1;
generals[120].magic[0] = 2;
generals[120].magic[1] = -1;
generals[120].magic[2] = -1;
generals[120].magic[3] = -1;
generals[120].equipment = -1;
generals[120].strength = 38;
generals[120].intellect = 84;
generals[120].experience = 200;
generals[120].level = 2;
generals[120].healthMax = 76;
generals[120].healthCur = 76;
generals[120].manaMax = 45;
generals[120].manaCur = 45;
generals[120].soldierMax = 10;
generals[120].soldierCur = 10;
generals[120].knightMax = 0;
generals[120].knightCur = 0;
generals[120].arms = 0x0008;
generals[120].formation = 0x03;

generals[121].king = 3;
generals[121].city = 10;
generals[121].magic[0] = 27;
generals[121].magic[1] = -1;
generals[121].magic[2] = -1;
generals[121].magic[3] = -1;
generals[121].equipment = 28;
generals[121].strength = 92;
generals[121].intellect = 60;
generals[121].experience = 200;
generals[121].level = 2;
generals[121].healthMax = 86;
generals[121].healthCur = 86;
generals[121].manaMax = 41;
generals[121].manaCur = 41;
generals[121].soldierMax = 10;
generals[121].soldierCur = 10;
generals[121].knightMax = 0;
generals[121].knightCur = 0;
generals[121].arms = 0x0101;
generals[121].formation = 0x05;

generals[122].king = -1;
generals[122].city = -1;
generals[122].magic[0] = 20;
generals[122].magic[1] = -1;
generals[122].magic[2] = -1;
generals[122].magic[3] = -1;
generals[122].equipment = -1;
generals[122].strength = 78;
generals[122].intellect = 48;
generals[122].experience = 200;
generals[122].level = 2;
generals[122].healthMax = 84;
generals[122].healthCur = 84;
generals[122].manaMax = 38;
generals[122].manaCur = 38;
generals[122].soldierMax = 10;
generals[122].soldierCur = 10;
generals[122].knightMax = 0;
generals[122].knightCur = 0;
generals[122].arms = 0x0200;
generals[122].formation = 0x05;

generals[123].king = -1;
generals[123].city = -1;
generals[123].magic[0] = 3;
generals[123].magic[1] = -1;
generals[123].magic[2] = -1;
generals[123].magic[3] = -1;
generals[123].equipment = -1;
generals[123].strength = 42;
generals[123].intellect = 90;
generals[123].experience = 200;
generals[123].level = 2;
generals[123].healthMax = 77;
generals[123].healthCur = 77;
generals[123].manaMax = 46;
generals[123].manaCur = 46;
generals[123].soldierMax = 10;
generals[123].soldierCur = 10;
generals[123].knightMax = 0;
generals[123].knightCur = 0;
generals[123].arms = 0x0028;
generals[123].formation = 0x3A;

generals[124].king = 3;
generals[124].city = 12;
generals[124].magic[0] = 1;
generals[124].magic[1] = -1;
generals[124].magic[2] = -1;
generals[124].magic[3] = -1;
generals[124].equipment = -1;
generals[124].strength = 50;
generals[124].intellect = 97;
generals[124].experience = 200;
generals[124].level = 2;
generals[124].healthMax = 79;
generals[124].healthCur = 79;
generals[124].manaMax = 47;
generals[124].manaCur = 47;
generals[124].soldierMax = 10;
generals[124].soldierCur = 10;
generals[124].knightMax = 0;
generals[124].knightCur = 0;
generals[124].arms = 0x0008;
generals[124].formation = 0x0A;

generals[125].king = -1;
generals[125].city = 8;
generals[125].magic[0] = 24;
generals[125].magic[1] = -1;
generals[125].magic[2] = -1;
generals[125].magic[3] = -1;
generals[125].equipment = -1;
generals[125].strength = 76;
generals[125].intellect = 68;
generals[125].experience = 200;
generals[125].level = 2;
generals[125].healthMax = 84;
generals[125].healthCur = 84;
generals[125].manaMax = 42;
generals[125].manaCur = 42;
generals[125].soldierMax = 10;
generals[125].soldierCur = 10;
generals[125].knightMax = 0;
generals[125].knightCur = 0;
generals[125].arms = 0x0401;
generals[125].formation = 0x11;

generals[126].king = -1;
generals[126].city = -1;
generals[126].magic[0] = 0;
generals[126].magic[1] = -1;
generals[126].magic[2] = -1;
generals[126].magic[3] = -1;
generals[126].equipment = -1;
generals[126].strength = 57;
generals[126].intellect = 82;
generals[126].experience = 200;
generals[126].level = 2;
generals[126].healthMax = 80;
generals[126].healthCur = 80;
generals[126].manaMax = 45;
generals[126].manaCur = 45;
generals[126].soldierMax = 10;
generals[126].soldierCur = 10;
generals[126].knightMax = 0;
generals[126].knightCur = 0;
generals[126].arms = 0x0008;
generals[126].formation = 0x09;

generals[127].king = -1;
generals[127].city = -1;
generals[127].magic[0] = 24;
generals[127].magic[1] = -1;
generals[127].magic[2] = -1;
generals[127].magic[3] = -1;
generals[127].equipment = -1;
generals[127].strength = 74;
generals[127].intellect = 56;
generals[127].experience = 200;
generals[127].level = 2;
generals[127].healthMax = 83;
generals[127].healthCur = 83;
generals[127].manaMax = 40;
generals[127].manaCur = 40;
generals[127].soldierMax = 10;
generals[127].soldierCur = 10;
generals[127].knightMax = 0;
generals[127].knightCur = 0;
generals[127].arms = 0x000A;
generals[127].formation = 0x0E;

generals[128].king = -1;
generals[128].city = -1;
generals[128].magic[0] = 28;
generals[128].magic[1] = -1;
generals[128].magic[2] = -1;
generals[128].magic[3] = -1;
generals[128].equipment = -1;
generals[128].strength = 98;
generals[128].intellect = 88;
generals[128].experience = 200;
generals[128].level = 2;
generals[128].healthMax = 87;
generals[128].healthCur = 87;
generals[128].manaMax = 46;
generals[128].manaCur = 46;
generals[128].soldierMax = 10;
generals[128].soldierCur = 10;
generals[128].knightMax = 0;
generals[128].knightCur = 0;
generals[128].arms = 0x000A;
generals[128].formation = 0x24;

generals[129].king = -1;
generals[129].city = -1;
generals[129].magic[0] = 20;
generals[129].magic[1] = -1;
generals[129].magic[2] = -1;
generals[129].magic[3] = -1;
generals[129].equipment = -1;
generals[129].strength = 76;
generals[129].intellect = 57;
generals[129].experience = 200;
generals[129].level = 2;
generals[129].healthMax = 84;
generals[129].healthCur = 84;
generals[129].manaMax = 40;
generals[129].manaCur = 40;
generals[129].soldierMax = 10;
generals[129].soldierCur = 10;
generals[129].knightMax = 0;
generals[129].knightCur = 0;
generals[129].arms = 0x0042;
generals[129].formation = 0x13;

generals[130].king = 13;
generals[130].city = 23;
generals[130].magic[0] = 2;
generals[130].magic[1] = -1;
generals[130].magic[2] = -1;
generals[130].magic[3] = -1;
generals[130].equipment = -1;
generals[130].strength = 42;
generals[130].intellect = 86;
generals[130].experience = 200;
generals[130].level = 2;
generals[130].healthMax = 77;
generals[130].healthCur = 77;
generals[130].manaMax = 45;
generals[130].manaCur = 45;
generals[130].soldierMax = 10;
generals[130].soldierCur = 10;
generals[130].knightMax = 0;
generals[130].knightCur = 0;
generals[130].arms = 0x0100;
generals[130].formation = 0x30;

generals[131].king = 13;
generals[131].city = 23;
generals[131].magic[0] = 5;
generals[131].magic[1] = -1;
generals[131].magic[2] = -1;
generals[131].magic[3] = -1;
generals[131].equipment = -1;
generals[131].strength = 44;
generals[131].intellect = 87;
generals[131].experience = 200;
generals[131].level = 2;
generals[131].healthMax = 77;
generals[131].healthCur = 77;
generals[131].manaMax = 46;
generals[131].manaCur = 46;
generals[131].soldierMax = 10;
generals[131].soldierCur = 10;
generals[131].knightMax = 0;
generals[131].knightCur = 0;
generals[131].arms = 0x0008;
generals[131].formation = 0x30;

generals[132].king = 13;
generals[132].city = 23;
generals[132].magic[0] = 2;
generals[132].magic[1] = -1;
generals[132].magic[2] = -1;
generals[132].magic[3] = -1;
generals[132].equipment = -1;
generals[132].strength = 63;
generals[132].intellect = 77;
generals[132].experience = 200;
generals[132].level = 2;
generals[132].healthMax = 81;
generals[132].healthCur = 81;
generals[132].manaMax = 44;
generals[132].manaCur = 44;
generals[132].soldierMax = 10;
generals[132].soldierCur = 10;
generals[132].knightMax = 0;
generals[132].knightCur = 0;
generals[132].arms = 0x0001;
generals[132].formation = 0x11;

generals[133].king = 17;
generals[133].city = 40;
generals[133].magic[0] = 0;
generals[133].magic[1] = -1;
generals[133].magic[2] = -1;
generals[133].magic[3] = -1;
generals[133].equipment = -1;
generals[133].strength = 42;
generals[133].intellect = 78;
generals[133].experience = 200;
generals[133].level = 2;
generals[133].healthMax = 77;
generals[133].healthCur = 77;
generals[133].manaMax = 44;
generals[133].manaCur = 44;
generals[133].soldierMax = 10;
generals[133].soldierCur = 10;
generals[133].knightMax = 0;
generals[133].knightCur = 0;
generals[133].arms = 0x0008;
generals[133].formation = 0x42;

generals[134].king = 1;
generals[134].city = 5;
generals[134].magic[0] = 27;
generals[134].magic[1] = -1;
generals[134].magic[2] = -1;
generals[134].magic[3] = -1;
generals[134].equipment = 17;
generals[134].strength = 90;
generals[134].intellect = 80;
generals[134].experience = 200;
generals[134].level = 2;
generals[134].healthMax = 85;
generals[134].healthCur = 85;
generals[134].manaMax = 45;
generals[134].manaCur = 45;
generals[134].soldierMax = 10;
generals[134].soldierCur = 10;
generals[134].knightMax = 0;
generals[134].knightCur = 0;
generals[134].arms = 0x0008;
generals[134].formation = 0x03;

generals[135].king = -1;
generals[135].city = 8;
generals[135].magic[0] = 0;
generals[135].magic[1] = -1;
generals[135].magic[2] = -1;
generals[135].magic[3] = -1;
generals[135].equipment = -1;
generals[135].strength = 40;
generals[135].intellect = 84;
generals[135].experience = 200;
generals[135].level = 2;
generals[135].healthMax = 77;
generals[135].healthCur = 77;
generals[135].manaMax = 45;
generals[135].manaCur = 45;
generals[135].soldierMax = 10;
generals[135].soldierCur = 10;
generals[135].knightMax = 0;
generals[135].knightCur = 0;
generals[135].arms = 0x0100;
generals[135].formation = 0x22;

generals[136].king = -1;
generals[136].city = -1;
generals[136].magic[0] = 0;
generals[136].magic[1] = -1;
generals[136].magic[2] = -1;
generals[136].magic[3] = -1;
generals[136].equipment = -1;
generals[136].strength = 38;
generals[136].intellect = 51;
generals[136].experience = 200;
generals[136].level = 2;
generals[136].healthMax = 76;
generals[136].healthCur = 76;
generals[136].manaMax = 39;
generals[136].manaCur = 39;
generals[136].soldierMax = 10;
generals[136].soldierCur = 10;
generals[136].knightMax = 0;
generals[136].knightCur = 0;
generals[136].arms = 0x0140;
generals[136].formation = 0x02;

generals[137].king = 14;
generals[137].city = 31;
generals[137].magic[0] = 0;
generals[137].magic[1] = -1;
generals[137].magic[2] = -1;
generals[137].magic[3] = -1;
generals[137].equipment = -1;
generals[137].strength = 42;
generals[137].intellect = 60;
generals[137].experience = 200;
generals[137].level = 2;
generals[137].healthMax = 77;
generals[137].healthCur = 77;
generals[137].manaMax = 41;
generals[137].manaCur = 41;
generals[137].soldierMax = 10;
generals[137].soldierCur = 10;
generals[137].knightMax = 0;
generals[137].knightCur = 0;
generals[137].arms = 0x0001;
generals[137].formation = 0x30;

generals[138].king = 3;
generals[138].city = 10;
generals[138].magic[0] = 27;
generals[138].magic[1] = -1;
generals[138].magic[2] = -1;
generals[138].magic[3] = -1;
generals[138].equipment = -1;
generals[138].strength = 82;
generals[138].intellect = 38;
generals[138].experience = 200;
generals[138].level = 2;
generals[138].healthMax = 85;
generals[138].healthCur = 85;
generals[138].manaMax = 36;
generals[138].manaCur = 36;
generals[138].soldierMax = 10;
generals[138].soldierCur = 10;
generals[138].knightMax = 0;
generals[138].knightCur = 0;
generals[138].arms = 0x0001;
generals[138].formation = 0x11;

generals[139].king = 0;
generals[139].city = 9;
generals[139].magic[0] = 21;
generals[139].magic[1] = -1;
generals[139].magic[2] = -1;
generals[139].magic[3] = -1;
generals[139].equipment = -1;
generals[139].strength = 83;
generals[139].intellect = 56;
generals[139].experience = 200;
generals[139].level = 2;
generals[139].healthMax = 85;
generals[139].healthCur = 85;
generals[139].manaMax = 40;
generals[139].manaCur = 40;
generals[139].soldierMax = 10;
generals[139].soldierCur = 10;
generals[139].knightMax = 0;
generals[139].knightCur = 0;
generals[139].arms = 0x0020;
generals[139].formation = 0x41;

generals[140].king = -1;
generals[140].city = -1;
generals[140].magic[0] = 19;
generals[140].magic[1] = -1;
generals[140].magic[2] = -1;
generals[140].magic[3] = -1;
generals[140].equipment = -1;
generals[140].strength = 79;
generals[140].intellect = 48;
generals[140].experience = 200;
generals[140].level = 2;
generals[140].healthMax = 84;
generals[140].healthCur = 84;
generals[140].manaMax = 38;
generals[140].manaCur = 38;
generals[140].soldierMax = 10;
generals[140].soldierCur = 10;
generals[140].knightMax = 0;
generals[140].knightCur = 0;
generals[140].arms = 0x0020;
generals[140].formation = 0x01;

generals[141].king = -1;
generals[141].city = -1;
generals[141].magic[0] = 4;
generals[141].magic[1] = -1;
generals[141].magic[2] = -1;
generals[141].magic[3] = -1;
generals[141].equipment = -1;
generals[141].strength = 43;
generals[141].intellect = 86;
generals[141].experience = 200;
generals[141].level = 2;
generals[141].healthMax = 77;
generals[141].healthCur = 77;
generals[141].manaMax = 45;
generals[141].manaCur = 45;
generals[141].soldierMax = 10;
generals[141].soldierCur = 10;
generals[141].knightMax = 0;
generals[141].knightCur = 0;
generals[141].arms = 0x0008;
generals[141].formation = 0x2A;

generals[142].king = -1;
generals[142].city = -1;
generals[142].magic[0] = 1;
generals[142].magic[1] = -1;
generals[142].magic[2] = -1;
generals[142].magic[3] = -1;
generals[142].equipment = -1;
generals[142].strength = 56;
generals[142].intellect = 91;
generals[142].experience = 200;
generals[142].level = 2;
generals[142].healthMax = 80;
generals[142].healthCur = 80;
generals[142].manaMax = 46;
generals[142].manaCur = 46;
generals[142].soldierMax = 10;
generals[142].soldierCur = 10;
generals[142].knightMax = 0;
generals[142].knightCur = 0;
generals[142].arms = 0x0100;
generals[142].formation = 0x30;

generals[143].king = 3;
generals[143].city = 10;
generals[143].magic[0] = 0;
generals[143].magic[1] = -1;
generals[143].magic[2] = -1;
generals[143].magic[3] = -1;
generals[143].equipment = -1;
generals[143].strength = 41;
generals[143].intellect = 82;
generals[143].experience = 200;
generals[143].level = 2;
generals[143].healthMax = 77;
generals[143].healthCur = 77;
generals[143].manaMax = 45;
generals[143].manaCur = 45;
generals[143].soldierMax = 10;
generals[143].soldierCur = 10;
generals[143].knightMax = 0;
generals[143].knightCur = 0;
generals[143].arms = 0x0100;
generals[143].formation = 0x12;

generals[144].king = 13;
generals[144].city = 26;
generals[144].magic[0] = 27;
generals[144].magic[1] = -1;
generals[144].magic[2] = -1;
generals[144].magic[3] = -1;
generals[144].equipment = -1;
generals[144].strength = 80;
generals[144].intellect = 72;
generals[144].experience = 200;
generals[144].level = 2;
generals[144].healthMax = 85;
generals[144].healthCur = 85;
generals[144].manaMax = 42;
generals[144].manaCur = 42;
generals[144].soldierMax = 10;
generals[144].soldierCur = 10;
generals[144].knightMax = 0;
generals[144].knightCur = 0;
generals[144].arms = 0x0008;
generals[144].formation = 0x05;

generals[145].king = -1;
generals[145].city = -1;
generals[145].magic[0] = 4;
generals[145].magic[1] = -1;
generals[145].magic[2] = -1;
generals[145].magic[3] = -1;
generals[145].equipment = 2;
generals[145].strength = 68;
generals[145].intellect = 109;
generals[145].experience = 200;
generals[145].level = 2;
generals[145].healthMax = 82;
generals[145].healthCur = 82;
generals[145].manaMax = 49;
generals[145].manaCur = 49;
generals[145].soldierMax = 10;
generals[145].soldierCur = 10;
generals[145].knightMax = 0;
generals[145].knightCur = 0;
generals[145].arms = 0x0009;
generals[145].formation = 0x33;

generals[146].king = -1;
generals[146].city = -1;
generals[146].magic[0] = 1;
generals[146].magic[1] = -1;
generals[146].magic[2] = -1;
generals[146].magic[3] = -1;
generals[146].equipment = -1;
generals[146].strength = 58;
generals[146].intellect = 93;
generals[146].experience = 200;
generals[146].level = 2;
generals[146].healthMax = 80;
generals[146].healthCur = 80;
generals[146].manaMax = 46;
generals[146].manaCur = 46;
generals[146].soldierMax = 10;
generals[146].soldierCur = 10;
generals[146].knightMax = 0;
generals[146].knightCur = 0;
generals[146].arms = 0x0028;
generals[146].formation = 0x56;

generals[147].king = -1;
generals[147].city = -1;
generals[147].magic[0] = 1;
generals[147].magic[1] = -1;
generals[147].magic[2] = -1;
generals[147].magic[3] = -1;
generals[147].equipment = -1;
generals[147].strength = 52;
generals[147].intellect = 91;
generals[147].experience = 200;
generals[147].level = 2;
generals[147].healthMax = 79;
generals[147].healthCur = 79;
generals[147].manaMax = 46;
generals[147].manaCur = 46;
generals[147].soldierMax = 10;
generals[147].soldierCur = 10;
generals[147].knightMax = 0;
generals[147].knightCur = 0;
generals[147].arms = 0x0008;
generals[147].formation = 0x19;

generals[148].king = -1;
generals[148].city = -1;
generals[148].magic[0] = 1;
generals[148].magic[1] = -1;
generals[148].magic[2] = -1;
generals[148].magic[3] = -1;
generals[148].equipment = -1;
generals[148].strength = 69;
generals[148].intellect = 98;
generals[148].experience = 200;
generals[148].level = 2;
generals[148].healthMax = 82;
generals[148].healthCur = 82;
generals[148].manaMax = 47;
generals[148].manaCur = 47;
generals[148].soldierMax = 10;
generals[148].soldierCur = 10;
generals[148].knightMax = 0;
generals[148].knightCur = 0;
generals[148].arms = 0x0008;
generals[148].formation = 0x60;

generals[149].king = -1;
generals[149].city = -1;
generals[149].magic[0] = 26;
generals[149].magic[1] = -1;
generals[149].magic[2] = -1;
generals[149].magic[3] = -1;
generals[149].equipment = -1;
generals[149].strength = 87;
generals[149].intellect = 94;
generals[149].experience = 200;
generals[149].level = 2;
generals[149].healthMax = 86;
generals[149].healthCur = 86;
generals[149].manaMax = 47;
generals[149].manaCur = 47;
generals[149].soldierMax = 10;
generals[149].soldierCur = 10;
generals[149].knightMax = 0;
generals[149].knightCur = 0;
generals[149].arms = 0x0101;
generals[149].formation = 0x33;

generals[150].king = -1;
generals[150].city = -1;
generals[150].magic[0] = 22;
generals[150].magic[1] = -1;
generals[150].magic[2] = -1;
generals[150].magic[3] = -1;
generals[150].equipment = -1;
generals[150].strength = 77;
generals[150].intellect = 53;
generals[150].experience = 200;
generals[150].level = 2;
generals[150].healthMax = 84;
generals[150].healthCur = 84;
generals[150].manaMax = 39;
generals[150].manaCur = 39;
generals[150].soldierMax = 10;
generals[150].soldierCur = 10;
generals[150].knightMax = 0;
generals[150].knightCur = 0;
generals[150].arms = 0x0042;
generals[150].formation = 0x41;

generals[151].king = -1;
generals[151].city = -1;
generals[151].magic[0] = 5;
generals[151].magic[1] = -1;
generals[151].magic[2] = -1;
generals[151].magic[3] = -1;
generals[151].equipment = -1;
generals[151].strength = 58;
generals[151].intellect = 85;
generals[151].experience = 200;
generals[151].level = 2;
generals[151].healthMax = 80;
generals[151].healthCur = 80;
generals[151].manaMax = 45;
generals[151].manaCur = 45;
generals[151].soldierMax = 10;
generals[151].soldierCur = 10;
generals[151].knightMax = 0;
generals[151].knightCur = 0;
generals[151].arms = 0x0001;
generals[151].formation = 0x51;

generals[152].king = 3;
generals[152].city = 10;
generals[152].magic[0] = 27;
generals[152].magic[1] = -1;
generals[152].magic[2] = -1;
generals[152].magic[3] = -1;
generals[152].equipment = -1;
generals[152].strength = 71;
generals[152].intellect = 84;
generals[152].experience = 200;
generals[152].level = 2;
generals[152].healthMax = 82;
generals[152].healthCur = 82;
generals[152].manaMax = 45;
generals[152].manaCur = 45;
generals[152].soldierMax = 10;
generals[152].soldierCur = 10;
generals[152].knightMax = 0;
generals[152].knightCur = 0;
generals[152].arms = 0x0040;
generals[152].formation = 0x60;

generals[153].king = -1;
generals[153].city = 0;
generals[153].magic[0] = 5;
generals[153].magic[1] = -1;
generals[153].magic[2] = -1;
generals[153].magic[3] = -1;
generals[153].equipment = -1;
generals[153].strength = 42;
generals[153].intellect = 80;
generals[153].experience = 200;
generals[153].level = 2;
generals[153].healthMax = 77;
generals[153].healthCur = 77;
generals[153].manaMax = 45;
generals[153].manaCur = 45;
generals[153].soldierMax = 10;
generals[153].soldierCur = 10;
generals[153].knightMax = 0;
generals[153].knightCur = 0;
generals[153].arms = 0x0040;
generals[153].formation = 0x03;

generals[154].king = -1;
generals[154].city = -1;
generals[154].magic[0] = 5;
generals[154].magic[1] = -1;
generals[154].magic[2] = -1;
generals[154].magic[3] = -1;
generals[154].equipment = -1;
generals[154].strength = 62;
generals[154].intellect = 96;
generals[154].experience = 200;
generals[154].level = 2;
generals[154].healthMax = 81;
generals[154].healthCur = 81;
generals[154].manaMax = 47;
generals[154].manaCur = 47;
generals[154].soldierMax = 10;
generals[154].soldierCur = 10;
generals[154].knightMax = 0;
generals[154].knightCur = 0;
generals[154].arms = 0x0108;
generals[154].formation = 0x2E;

generals[155].king = 2;
generals[155].city = 28;
generals[155].magic[0] = 27;
generals[155].magic[1] = -1;
generals[155].magic[2] = -1;
generals[155].magic[3] = -1;
generals[155].equipment = -1;
generals[155].strength = 82;
generals[155].intellect = 66;
generals[155].experience = 200;
generals[155].level = 2;
generals[155].healthMax = 85;
generals[155].healthCur = 85;
generals[155].manaMax = 41;
generals[155].manaCur = 41;
generals[155].soldierMax = 10;
generals[155].soldierCur = 10;
generals[155].knightMax = 0;
generals[155].knightCur = 0;
generals[155].arms = 0x0001;
generals[155].formation = 0x03;

generals[156].king = 7;
generals[156].city = 6;
generals[156].magic[0] = 27;
generals[156].magic[1] = -1;
generals[156].magic[2] = -1;
generals[156].magic[3] = -1;
generals[156].equipment = -1;
generals[156].strength = 69;
generals[156].intellect = 48;
generals[156].experience = 200;
generals[156].level = 2;
generals[156].healthMax = 82;
generals[156].healthCur = 82;
generals[156].manaMax = 38;
generals[156].manaCur = 38;
generals[156].soldierMax = 10;
generals[156].soldierCur = 10;
generals[156].knightMax = 0;
generals[156].knightCur = 0;
generals[156].arms = 0x0008;
generals[156].formation = 0x12;

generals[157].king = 4;
generals[157].city = 4;
generals[157].magic[0] = 28;
generals[157].magic[1] = -1;
generals[157].magic[2] = -1;
generals[157].magic[3] = -1;
generals[157].equipment = -1;
generals[157].strength = 96;
generals[157].intellect = 45;
generals[157].experience = 200;
generals[157].level = 2;
generals[157].healthMax = 87;
generals[157].healthCur = 87;
generals[157].manaMax = 37;
generals[157].manaCur = 37;
generals[157].soldierMax = 10;
generals[157].soldierCur = 10;
generals[157].knightMax = 0;
generals[157].knightCur = 0;
generals[157].arms = 0x0004;
generals[157].formation = 0x05;

generals[158].king = -1;
generals[158].city = -1;
generals[158].magic[0] = 23;
generals[158].magic[1] = -1;
generals[158].magic[2] = -1;
generals[158].magic[3] = -1;
generals[158].equipment = -1;
generals[158].strength = 94;
generals[158].intellect = 52;
generals[158].experience = 200;
generals[158].level = 2;
generals[158].healthMax = 87;
generals[158].healthCur = 87;
generals[158].manaMax = 39;
generals[158].manaCur = 39;
generals[158].soldierMax = 10;
generals[158].soldierCur = 10;
generals[158].knightMax = 0;
generals[158].knightCur = 0;
generals[158].arms = 0x0002;
generals[158].formation = 0x09;

generals[159].king = -1;
generals[159].city = 8;
generals[159].magic[0] = 20;
generals[159].magic[1] = -1;
generals[159].magic[2] = -1;
generals[159].magic[3] = -1;
generals[159].equipment = -1;
generals[159].strength = 71;
generals[159].intellect = 45;
generals[159].experience = 200;
generals[159].level = 2;
generals[159].healthMax = 82;
generals[159].healthCur = 82;
generals[159].manaMax = 37;
generals[159].manaCur = 37;
generals[159].soldierMax = 10;
generals[159].soldierCur = 10;
generals[159].knightMax = 0;
generals[159].knightCur = 0;
generals[159].arms = 0x0020;
generals[159].formation = 0x50;

generals[160].king = -1;
generals[160].city = -1;
generals[160].magic[0] = 3;
generals[160].magic[1] = -1;
generals[160].magic[2] = -1;
generals[160].magic[3] = -1;
generals[160].equipment = -1;
generals[160].strength = 66;
generals[160].intellect = 98;
generals[160].experience = 200;
generals[160].level = 2;
generals[160].healthMax = 81;
generals[160].healthCur = 81;
generals[160].manaMax = 47;
generals[160].manaCur = 47;
generals[160].soldierMax = 10;
generals[160].soldierCur = 10;
generals[160].knightMax = 0;
generals[160].knightCur = 0;
generals[160].arms = 0x0009;
generals[160].formation = 0x38;

generals[161].king = -1;
generals[161].city = -1;
generals[161].magic[0] = 23;
generals[161].magic[1] = -1;
generals[161].magic[2] = -1;
generals[161].magic[3] = -1;
generals[161].equipment = -1;
generals[161].strength = 97;
generals[161].intellect = 70;
generals[161].experience = 200;
generals[161].level = 2;
generals[161].healthMax = 87;
generals[161].healthCur = 87;
generals[161].manaMax = 42;
generals[161].manaCur = 42;
generals[161].soldierMax = 10;
generals[161].soldierCur = 10;
generals[161].knightMax = 0;
generals[161].knightCur = 0;
generals[161].arms = 0x0001;
generals[161].formation = 0x11;

generals[162].king = -1;
generals[162].city = -1;
generals[162].magic[0] = 0;
generals[162].magic[1] = -1;
generals[162].magic[2] = -1;
generals[162].magic[3] = -1;
generals[162].equipment = -1;
generals[162].strength = 32;
generals[162].intellect = 69;
generals[162].experience = 200;
generals[162].level = 2;
generals[162].healthMax = 75;
generals[162].healthCur = 75;
generals[162].manaMax = 42;
generals[162].manaCur = 42;
generals[162].soldierMax = 10;
generals[162].soldierCur = 10;
generals[162].knightMax = 0;
generals[162].knightCur = 0;
generals[162].arms = 0x0050;
generals[162].formation = 0x0A;

generals[163].king = -1;
generals[163].city = -1;
generals[163].magic[0] = 26;
generals[163].magic[1] = -1;
generals[163].magic[2] = -1;
generals[163].magic[3] = -1;
generals[163].equipment = -1;
generals[163].strength = 88;
generals[163].intellect = 70;
generals[163].experience = 200;
generals[163].level = 2;
generals[163].healthMax = 86;
generals[163].healthCur = 86;
generals[163].manaMax = 42;
generals[163].manaCur = 42;
generals[163].soldierMax = 10;
generals[163].soldierCur = 10;
generals[163].knightMax = 0;
generals[163].knightCur = 0;
generals[163].arms = 0x0002;
generals[163].formation = 0x09;

generals[164].king = 1;
generals[164].city = 5;
generals[164].magic[0] = 28;
generals[164].magic[1] = -1;
generals[164].magic[2] = -1;
generals[164].magic[3] = -1;
generals[164].equipment = 8;
generals[164].strength = 109;
generals[164].intellect = 85;
generals[164].experience = 200;
generals[164].level = 2;
generals[164].healthMax = 88;
generals[164].healthCur = 88;
generals[164].manaMax = 45;
generals[164].manaCur = 45;
generals[164].soldierMax = 10;
generals[164].soldierCur = 10;
generals[164].knightMax = 0;
generals[164].knightCur = 0;
generals[164].arms = 0x0001;
generals[164].formation = 0x09;

generals[165].king = -1;
generals[165].city = -1;
generals[165].magic[0] = 24;
generals[165].magic[1] = -1;
generals[165].magic[2] = -1;
generals[165].magic[3] = -1;
generals[165].equipment = -1;
generals[165].strength = 86;
generals[165].intellect = 64;
generals[165].experience = 200;
generals[165].level = 2;
generals[165].healthMax = 85;
generals[165].healthCur = 85;
generals[165].manaMax = 41;
generals[165].manaCur = 41;
generals[165].soldierMax = 10;
generals[165].soldierCur = 10;
generals[165].knightMax = 0;
generals[165].knightCur = 0;
generals[165].arms = 0x0012;
generals[165].formation = 0x19;

generals[166].king = -1;
generals[166].city = -1;
generals[166].magic[0] = 24;
generals[166].magic[1] = -1;
generals[166].magic[2] = -1;
generals[166].magic[3] = -1;
generals[166].equipment = -1;
generals[166].strength = 88;
generals[166].intellect = 71;
generals[166].experience = 200;
generals[166].level = 2;
generals[166].healthMax = 86;
generals[166].healthCur = 86;
generals[166].manaMax = 42;
generals[166].manaCur = 42;
generals[166].soldierMax = 10;
generals[166].soldierCur = 10;
generals[166].knightMax = 0;
generals[166].knightCur = 0;
generals[166].arms = 0x0003;
generals[166].formation = 0x31;

generals[167].king = 15;
generals[167].city = 33;
generals[167].magic[0] = 19;
generals[167].magic[1] = -1;
generals[167].magic[2] = -1;
generals[167].magic[3] = -1;
generals[167].equipment = -1;
generals[167].strength = 80;
generals[167].intellect = 52;
generals[167].experience = 200;
generals[167].level = 2;
generals[167].healthMax = 85;
generals[167].healthCur = 85;
generals[167].manaMax = 39;
generals[167].manaCur = 39;
generals[167].soldierMax = 10;
generals[167].soldierCur = 10;
generals[167].knightMax = 0;
generals[167].knightCur = 0;
generals[167].arms = 0x0100;
generals[167].formation = 0x50;

generals[168].king = 6;
generals[168].city = 1;
generals[168].magic[0] = 22;
generals[168].magic[1] = -1;
generals[168].magic[2] = -1;
generals[168].magic[3] = -1;
generals[168].equipment = -1;
generals[168].strength = 70;
generals[168].intellect = 49;
generals[168].experience = 200;
generals[168].level = 2;
generals[168].healthMax = 82;
generals[168].healthCur = 82;
generals[168].manaMax = 38;
generals[168].manaCur = 38;
generals[168].soldierMax = 10;
generals[168].soldierCur = 10;
generals[168].knightMax = 0;
generals[168].knightCur = 0;
generals[168].arms = 0x0020;
generals[168].formation = 0x08;

generals[169].king = 15;
generals[169].city = 33;
generals[169].magic[0] = 21;
generals[169].magic[1] = -1;
generals[169].magic[2] = -1;
generals[169].magic[3] = -1;
generals[169].equipment = -1;
generals[169].strength = 76;
generals[169].intellect = 45;
generals[169].experience = 200;
generals[169].level = 2;
generals[169].healthMax = 84;
generals[169].healthCur = 84;
generals[169].manaMax = 37;
generals[169].manaCur = 37;
generals[169].soldierMax = 10;
generals[169].soldierCur = 10;
generals[169].knightMax = 0;
generals[169].knightCur = 0;
generals[169].arms = 0x0010;
generals[169].formation = 0x12;

generals[170].king = 17;
generals[170].city = 42;
generals[170].magic[0] = 24;
generals[170].magic[1] = -1;
generals[170].magic[2] = -1;
generals[170].magic[3] = -1;
generals[170].equipment = -1;
generals[170].strength = 89;
generals[170].intellect = 72;
generals[170].experience = 200;
generals[170].level = 2;
generals[170].healthMax = 86;
generals[170].healthCur = 86;
generals[170].manaMax = 42;
generals[170].manaCur = 42;
generals[170].soldierMax = 10;
generals[170].soldierCur = 10;
generals[170].knightMax = 0;
generals[170].knightCur = 0;
generals[170].arms = 0x0001;
generals[170].formation = 0x03;

generals[171].king = 6;
generals[171].city = 1;
generals[171].magic[0] = 21;
generals[171].magic[1] = -1;
generals[171].magic[2] = -1;
generals[171].magic[3] = -1;
generals[171].equipment = -1;
generals[171].strength = 65;
generals[171].intellect = 46;
generals[171].experience = 200;
generals[171].level = 2;
generals[171].healthMax = 81;
generals[171].healthCur = 81;
generals[171].manaMax = 37;
generals[171].manaCur = 37;
generals[171].soldierMax = 10;
generals[171].soldierCur = 10;
generals[171].knightMax = 0;
generals[171].knightCur = 0;
generals[171].arms = 0x0010;
generals[171].formation = 0x40;

generals[172].king = 16;
generals[172].city = 34;
generals[172].magic[0] = 0;
generals[172].magic[1] = -1;
generals[172].magic[2] = -1;
generals[172].magic[3] = -1;
generals[172].equipment = -1;
generals[172].strength = 60;
generals[172].intellect = 86;
generals[172].experience = 200;
generals[172].level = 2;
generals[172].healthMax = 81;
generals[172].healthCur = 81;
generals[172].manaMax = 45;
generals[172].manaCur = 45;
generals[172].soldierMax = 10;
generals[172].soldierCur = 10;
generals[172].knightMax = 0;
generals[172].knightCur = 0;
generals[172].arms = 0x0100;
generals[172].formation = 0x42;

generals[173].king = 2;
generals[173].city = 28;
generals[173].magic[0] = 21;
generals[173].magic[1] = -1;
generals[173].magic[2] = -1;
generals[173].magic[3] = -1;
generals[173].equipment = -1;
generals[173].strength = 62;
generals[173].intellect = 53;
generals[173].experience = 200;
generals[173].level = 2;
generals[173].healthMax = 81;
generals[173].healthCur = 81;
generals[173].manaMax = 39;
generals[173].manaCur = 39;
generals[173].soldierMax = 10;
generals[173].soldierCur = 10;
generals[173].knightMax = 0;
generals[173].knightCur = 0;
generals[173].arms = 0x0010;
generals[173].formation = 0x11;

generals[174].king = 7;
generals[174].city = 6;
generals[174].magic[0] = 0;
generals[174].magic[1] = -1;
generals[174].magic[2] = -1;
generals[174].magic[3] = -1;
generals[174].equipment = -1;
generals[174].strength = 65;
generals[174].intellect = 76;
generals[174].experience = 200;
generals[174].level = 2;
generals[174].healthMax = 81;
generals[174].healthCur = 81;
generals[174].manaMax = 44;
generals[174].manaCur = 44;
generals[174].soldierMax = 10;
generals[174].soldierCur = 10;
generals[174].knightMax = 0;
generals[174].knightCur = 0;
generals[174].arms = 0x0100;
generals[174].formation = 0x50;

generals[175].king = 8;
generals[175].city = 7;
generals[175].magic[0] = 20;
generals[175].magic[1] = -1;
generals[175].magic[2] = -1;
generals[175].magic[3] = -1;
generals[175].equipment = -1;
generals[175].strength = 70;
generals[175].intellect = 40;
generals[175].experience = 200;
generals[175].level = 2;
generals[175].healthMax = 82;
generals[175].healthCur = 82;
generals[175].manaMax = 37;
generals[175].manaCur = 37;
generals[175].soldierMax = 10;
generals[175].soldierCur = 10;
generals[175].knightMax = 0;
generals[175].knightCur = 0;
generals[175].arms = 0x0100;
generals[175].formation = 0x12;

generals[176].king = 3;
generals[176].city = 11;
generals[176].magic[0] = 27;
generals[176].magic[1] = -1;
generals[176].magic[2] = -1;
generals[176].magic[3] = -1;
generals[176].equipment = -1;
generals[176].strength = 69;
generals[176].intellect = 73;
generals[176].experience = 200;
generals[176].level = 2;
generals[176].healthMax = 82;
generals[176].healthCur = 82;
generals[176].manaMax = 42;
generals[176].manaCur = 42;
generals[176].soldierMax = 10;
generals[176].soldierCur = 10;
generals[176].knightMax = 0;
generals[176].knightCur = 0;
generals[176].arms = 0x0001;
generals[176].formation = 0x22;

generals[177].king = 11;
generals[177].city = 16;
generals[177].magic[0] = 5;
generals[177].magic[1] = -1;
generals[177].magic[2] = -1;
generals[177].magic[3] = -1;
generals[177].equipment = -1;
generals[177].strength = 41;
generals[177].intellect = 80;
generals[177].experience = 200;
generals[177].level = 2;
generals[177].healthMax = 77;
generals[177].healthCur = 77;
generals[177].manaMax = 45;
generals[177].manaCur = 45;
generals[177].soldierMax = 10;
generals[177].soldierCur = 10;
generals[177].knightMax = 0;
generals[177].knightCur = 0;
generals[177].arms = 0x0100;
generals[177].formation = 0x48;

generals[178].king = 2;
generals[178].city = 28;
generals[178].magic[0] = 27;
generals[178].magic[1] = -1;
generals[178].magic[2] = -1;
generals[178].magic[3] = -1;
generals[178].equipment = -1;
generals[178].strength = 74;
generals[178].intellect = 65;
generals[178].experience = 200;
generals[178].level = 2;
generals[178].healthMax = 83;
generals[178].healthCur = 83;
generals[178].manaMax = 41;
generals[178].manaCur = 41;
generals[178].soldierMax = 10;
generals[178].soldierCur = 10;
generals[178].knightMax = 0;
generals[178].knightCur = 0;
generals[178].arms = 0x0020;
generals[178].formation = 0x44;

generals[179].king = 10;
generals[179].city = 15;
generals[179].magic[0] = 25;
generals[179].magic[1] = -1;
generals[179].magic[2] = -1;
generals[179].magic[3] = -1;
generals[179].equipment = -1;
generals[179].strength = 70;
generals[179].intellect = 42;
generals[179].experience = 200;
generals[179].level = 2;
generals[179].healthMax = 82;
generals[179].healthCur = 82;
generals[179].manaMax = 37;
generals[179].manaCur = 37;
generals[179].soldierMax = 10;
generals[179].soldierCur = 10;
generals[179].knightMax = 0;
generals[179].knightCur = 0;
generals[179].arms = 0x0020;
generals[179].formation = 0x04;

generals[180].king = 17;
generals[180].city = 39;
generals[180].magic[0] = 22;
generals[180].magic[1] = -1;
generals[180].magic[2] = -1;
generals[180].magic[3] = -1;
generals[180].equipment = -1;
generals[180].strength = 64;
generals[180].intellect = 43;
generals[180].experience = 200;
generals[180].level = 2;
generals[180].healthMax = 81;
generals[180].healthCur = 81;
generals[180].manaMax = 37;
generals[180].manaCur = 37;
generals[180].soldierMax = 10;
generals[180].soldierCur = 10;
generals[180].knightMax = 0;
generals[180].knightCur = 0;
generals[180].arms = 0x0010;
generals[180].formation = 0x01;

generals[181].king = 5;
generals[181].city = 21;
generals[181].magic[0] = 20;
generals[181].magic[1] = -1;
generals[181].magic[2] = -1;
generals[181].magic[3] = -1;
generals[181].equipment = -1;
generals[181].strength = 69;
generals[181].intellect = 63;
generals[181].experience = 200;
generals[181].level = 2;
generals[181].healthMax = 82;
generals[181].healthCur = 82;
generals[181].manaMax = 41;
generals[181].manaCur = 41;
generals[181].soldierMax = 10;
generals[181].soldierCur = 10;
generals[181].knightMax = 0;
generals[181].knightCur = 0;
generals[181].arms = 0x0020;
generals[181].formation = 0x42;

generals[182].king = 3;
generals[182].city = 10;
generals[182].magic[0] = 21;
generals[182].magic[1] = -1;
generals[182].magic[2] = -1;
generals[182].magic[3] = -1;
generals[182].equipment = -1;
generals[182].strength = 70;
generals[182].intellect = 63;
generals[182].experience = 200;
generals[182].level = 2;
generals[182].healthMax = 82;
generals[182].healthCur = 82;
generals[182].manaMax = 41;
generals[182].manaCur = 41;
generals[182].soldierMax = 10;
generals[182].soldierCur = 10;
generals[182].knightMax = 0;
generals[182].knightCur = 0;
generals[182].arms = 0x0001;
generals[182].formation = 0x11;

generals[183].king = 0;
generals[183].city = 9;
generals[183].magic[0] = 24;
generals[183].magic[1] = -1;
generals[183].magic[2] = -1;
generals[183].magic[3] = -1;
generals[183].equipment = -1;
generals[183].strength = 82;
generals[183].intellect = 51;
generals[183].experience = 200;
generals[183].level = 2;
generals[183].healthMax = 85;
generals[183].healthCur = 85;
generals[183].manaMax = 39;
generals[183].manaCur = 39;
generals[183].soldierMax = 10;
generals[183].soldierCur = 10;
generals[183].knightMax = 0;
generals[183].knightCur = 0;
generals[183].arms = 0x0001;
generals[183].formation = 0x0C;

generals[184].king = 10;
generals[184].city = 15;
generals[184].magic[0] = 21;
generals[184].magic[1] = -1;
generals[184].magic[2] = -1;
generals[184].magic[3] = -1;
generals[184].equipment = -1;
generals[184].strength = 71;
generals[184].intellect = 38;
generals[184].experience = 200;
generals[184].level = 2;
generals[184].healthMax = 82;
generals[184].healthCur = 82;
generals[184].manaMax = 36;
generals[184].manaCur = 36;
generals[184].soldierMax = 10;
generals[184].soldierCur = 10;
generals[184].knightMax = 0;
generals[184].knightCur = 0;
generals[184].arms = 0x0008;
generals[184].formation = 0x03;

generals[185].king = 14;
generals[185].city = 32;
generals[185].magic[0] = 27;
generals[185].magic[1] = -1;
generals[185].magic[2] = -1;
generals[185].magic[3] = -1;
generals[185].equipment = -1;
generals[185].strength = 78;
generals[185].intellect = 62;
generals[185].experience = 200;
generals[185].level = 2;
generals[185].healthMax = 84;
generals[185].healthCur = 84;
generals[185].manaMax = 41;
generals[185].manaCur = 41;
generals[185].soldierMax = 10;
generals[185].soldierCur = 10;
generals[185].knightMax = 0;
generals[185].knightCur = 0;
generals[185].arms = 0x0010;
generals[185].formation = 0x01;

generals[186].king = 11;
generals[186].city = 17;
generals[186].magic[0] = 2;
generals[186].magic[1] = -1;
generals[186].magic[2] = -1;
generals[186].magic[3] = -1;
generals[186].equipment = -1;
generals[186].strength = 52;
generals[186].intellect = 71;
generals[186].experience = 200;
generals[186].level = 2;
generals[186].healthMax = 79;
generals[186].healthCur = 79;
generals[186].manaMax = 42;
generals[186].manaCur = 42;
generals[186].soldierMax = 10;
generals[186].soldierCur = 10;
generals[186].knightMax = 0;
generals[186].knightCur = 0;
generals[186].arms = 0x0008;
generals[186].formation = 0x50;

generals[187].king = 14;
generals[187].city = 32;
generals[187].magic[0] = 22;
generals[187].magic[1] = -1;
generals[187].magic[2] = -1;
generals[187].magic[3] = -1;
generals[187].equipment = -1;
generals[187].strength = 66;
generals[187].intellect = 52;
generals[187].experience = 200;
generals[187].level = 2;
generals[187].healthMax = 81;
generals[187].healthCur = 81;
generals[187].manaMax = 39;
generals[187].manaCur = 39;
generals[187].soldierMax = 10;
generals[187].soldierCur = 10;
generals[187].knightMax = 0;
generals[187].knightCur = 0;
generals[187].arms = 0x0001;
generals[187].formation = 0x10;

generals[188].king = 9;
generals[188].city = 8;
generals[188].magic[0] = 0;
generals[188].magic[1] = -1;
generals[188].magic[2] = -1;
generals[188].magic[3] = -1;
generals[188].equipment = -1;
generals[188].strength = 46;
generals[188].intellect = 55;
generals[188].experience = 200;
generals[188].level = 2;
generals[188].healthMax = 77;
generals[188].healthCur = 77;
generals[188].manaMax = 40;
generals[188].manaCur = 40;
generals[188].soldierMax = 10;
generals[188].soldierCur = 10;
generals[188].knightMax = 0;
generals[188].knightCur = 0;
generals[188].arms = 0x0008;
generals[188].formation = 0x12;

generals[189].king = 9;
generals[189].city = 8;
generals[189].magic[0] = 2;
generals[189].magic[1] = -1;
generals[189].magic[2] = -1;
generals[189].magic[3] = -1;
generals[189].equipment = -1;
generals[189].strength = 68;
generals[189].intellect = 60;
generals[189].experience = 200;
generals[189].level = 2;
generals[189].healthMax = 82;
generals[189].healthCur = 82;
generals[189].manaMax = 41;
generals[189].manaCur = 41;
generals[189].soldierMax = 10;
generals[189].soldierCur = 10;
generals[189].knightMax = 0;
generals[189].knightCur = 0;
generals[189].arms = 0x0100;
generals[189].formation = 0x14;
		
generals[190].king = 9;
generals[190].city = 8;
generals[190].magic[0] = 2;
generals[190].magic[1] = -1;
generals[190].magic[2] = -1;
generals[190].magic[3] = -1;
generals[190].equipment = -1;
generals[190].strength = 68;
generals[190].intellect = 60;
generals[190].experience = 200;
generals[190].level = 2;
generals[190].healthMax = 82;
generals[190].healthCur = 82;
generals[190].manaMax = 41;
generals[190].manaCur = 41;
generals[190].soldierMax = 10;
generals[190].soldierCur = 10;
generals[190].knightMax = 0;
generals[190].knightCur = 0;
generals[190].arms = 0x0100;
generals[190].formation = 0x14;

generals[191].king = 17;
generals[191].city = 39;
generals[191].magic[0] = 21;
generals[191].magic[1] = -1;
generals[191].magic[2] = -1;
generals[191].magic[3] = -1;
generals[191].equipment = -1;
generals[191].strength = 76;
generals[191].intellect = 48;
generals[191].experience = 200;
generals[191].level = 2;
generals[191].healthMax = 84;
generals[191].healthCur = 84;
generals[191].manaMax = 38;
generals[191].manaCur = 38;
generals[191].soldierMax = 10;
generals[191].soldierCur = 10;
generals[191].knightMax = 0;
generals[191].knightCur = 0;
generals[191].arms = 0x0001;
generals[191].formation = 0x11;

generals[192].king = 16;
generals[192].city = 34;
generals[192].magic[0] = 2;
generals[192].magic[1] = -1;
generals[192].magic[2] = -1;
generals[192].magic[3] = -1;
generals[192].equipment = -1;
generals[192].strength = 47;
generals[192].intellect = 78;
generals[192].experience = 200;
generals[192].level = 2;
generals[192].healthMax = 78;
generals[192].healthCur = 78;
generals[192].manaMax = 44;
generals[192].manaCur = 44;
generals[192].soldierMax = 10;
generals[192].soldierCur = 10;
generals[192].knightMax = 0;
generals[192].knightCur = 0;
generals[192].arms = 0x0100;
generals[192].formation = 0x21;

generals[193].king = 17;
generals[193].city = 40;
generals[193].magic[0] = 27;
generals[193].magic[1] = -1;
generals[193].magic[2] = -1;
generals[193].magic[3] = -1;
generals[193].equipment = -1;
generals[193].strength = 69;
generals[193].intellect = 67;
generals[193].experience = 200;
generals[193].level = 2;
generals[193].healthMax = 82;
generals[193].healthCur = 82;
generals[193].manaMax = 42;
generals[193].manaCur = 42;
generals[193].soldierMax = 10;
generals[193].soldierCur = 10;
generals[193].knightMax = 0;
generals[193].knightCur = 0;
generals[193].arms = 0x0001;
generals[193].formation = 0x50;

generals[194].king = 17;
generals[194].city = 40;
generals[194].magic[0] = 26;
generals[194].magic[1] = -1;
generals[194].magic[2] = -1;
generals[194].magic[3] = -1;
generals[194].equipment = -1;
generals[194].strength = 82;
generals[194].intellect = 51;
generals[194].experience = 200;
generals[194].level = 2;
generals[194].healthMax = 85;
generals[194].healthCur = 85;
generals[194].manaMax = 39;
generals[194].manaCur = 39;
generals[194].soldierMax = 10;
generals[194].soldierCur = 10;
generals[194].knightMax = 0;
generals[194].knightCur = 0;
generals[194].arms = 0x0002;
generals[194].formation = 0x05;

generals[195].king = 5;
generals[195].city = 21;
generals[195].magic[0] = 22;
generals[195].magic[1] = -1;
generals[195].magic[2] = -1;
generals[195].magic[3] = -1;
generals[195].equipment = -1;
generals[195].strength = 65;
generals[195].intellect = 45;
generals[195].experience = 200;
generals[195].level = 2;
generals[195].healthMax = 81;
generals[195].healthCur = 81;
generals[195].manaMax = 37;
generals[195].manaCur = 37;
generals[195].soldierMax = 10;
generals[195].soldierCur = 10;
generals[195].knightMax = 0;
generals[195].knightCur = 0;
generals[195].arms = 0x0020;
generals[195].formation = 0x01;

generals[196].king = 17;
generals[196].city = 40;
generals[196].magic[0] = 5;
generals[196].magic[1] = -1;
generals[196].magic[2] = -1;
generals[196].magic[3] = -1;
generals[196].equipment = -1;
generals[196].strength = 42;
generals[196].intellect = 64;
generals[196].experience = 200;
generals[196].level = 2;
generals[196].healthMax = 77;
generals[196].healthCur = 77;
generals[196].manaMax = 41;
generals[196].manaCur = 41;
generals[196].soldierMax = 10;
generals[196].soldierCur = 10;
generals[196].knightMax = 0;
generals[196].knightCur = 0;
generals[196].arms = 0x0008;
generals[196].formation = 0x42;

generals[197].king = 7;
generals[197].city = 6;
generals[197].magic[0] = 24;
generals[197].magic[1] = -1;
generals[197].magic[2] = -1;
generals[197].magic[3] = -1;
generals[197].equipment = -1;
generals[197].strength = 75;
generals[197].intellect = 39;
generals[197].experience = 200;
generals[197].level = 2;
generals[197].healthMax = 84;
generals[197].healthCur = 84;
generals[197].manaMax = 36;
generals[197].manaCur = 36;
generals[197].soldierMax = 10;
generals[197].soldierCur = 10;
generals[197].knightMax = 0;
generals[197].knightCur = 0;
generals[197].arms = 0x0010;
generals[197].formation = 0x01;

generals[198].king = 13;
generals[198].city = 26;
generals[198].magic[0] = 21;
generals[198].magic[1] = -1;
generals[198].magic[2] = -1;
generals[198].magic[3] = -1;
generals[198].equipment = -1;
generals[198].strength = 77;
generals[198].intellect = 65;
generals[198].experience = 200;
generals[198].level = 2;
generals[198].healthMax = 84;
generals[198].healthCur = 84;
generals[198].manaMax = 41;
generals[198].manaCur = 41;
generals[198].soldierMax = 10;
generals[198].soldierCur = 10;
generals[198].knightMax = 0;
generals[198].knightCur = 0;
generals[198].arms = 0x0010;
generals[198].formation = 0x01;

generals[199].king = 11;
generals[199].city = 17;
generals[199].magic[0] = 5;
generals[199].magic[1] = -1;
generals[199].magic[2] = -1;
generals[199].magic[3] = -1;
generals[199].equipment = -1;
generals[199].strength = 40;
generals[199].intellect = 82;
generals[199].experience = 200;
generals[199].level = 2;
generals[199].healthMax = 77;
generals[199].healthCur = 77;
generals[199].manaMax = 45;
generals[199].manaCur = 45;
generals[199].soldierMax = 10;
generals[199].soldierCur = 10;
generals[199].knightMax = 0;
generals[199].knightCur = 0;
generals[199].arms = 0x0008;
generals[199].formation = 0x22;

generals[200].king = 11;
generals[200].city = 16;
generals[200].magic[0] = 21;
generals[200].magic[1] = -1;
generals[200].magic[2] = -1;
generals[200].magic[3] = -1;
generals[200].equipment = -1;
generals[200].strength = 71;
generals[200].intellect = 48;
generals[200].experience = 200;
generals[200].level = 2;
generals[200].healthMax = 82;
generals[200].healthCur = 82;
generals[200].manaMax = 38;
generals[200].manaCur = 38;
generals[200].soldierMax = 10;
generals[200].soldierCur = 10;
generals[200].knightMax = 0;
generals[200].knightCur = 0;
generals[200].arms = 0x0001;
generals[200].formation = 0x01;

generals[201].king = 13;
generals[201].city = 25;
generals[201].magic[0] = 0;
generals[201].magic[1] = -1;
generals[201].magic[2] = -1;
generals[201].magic[3] = -1;
generals[201].equipment = -1;
generals[201].strength = 42;
generals[201].intellect = 70;
generals[201].experience = 200;
generals[201].level = 2;
generals[201].healthMax = 77;
generals[201].healthCur = 77;
generals[201].manaMax = 42;
generals[201].manaCur = 42;
generals[201].soldierMax = 10;
generals[201].soldierCur = 10;
generals[201].knightMax = 0;
generals[201].knightCur = 0;
generals[201].arms = 0x0040;
generals[201].formation = 0x18;

generals[202].king = 1;
generals[202].city = 5;
generals[202].magic[0] = 5;
generals[202].magic[1] = -1;
generals[202].magic[2] = -1;
generals[202].magic[3] = -1;
generals[202].equipment = -1;
generals[202].strength = 45;
generals[202].intellect = 80;
generals[202].experience = 200;
generals[202].level = 2;
generals[202].healthMax = 77;
generals[202].healthCur = 77;
generals[202].manaMax = 45;
generals[202].manaCur = 45;
generals[202].soldierMax = 10;
generals[202].soldierCur = 10;
generals[202].knightMax = 0;
generals[202].knightCur = 0;
generals[202].arms = 0x0020;
generals[202].formation = 0x22;

generals[203].king = 2;
generals[203].city = 28;
generals[203].magic[0] = 2;
generals[203].magic[1] = -1;
generals[203].magic[2] = -1;
generals[203].magic[3] = -1;
generals[203].equipment = -1;
generals[203].strength = 54;
generals[203].intellect = 78;
generals[203].experience = 200;
generals[203].level = 2;
generals[203].healthMax = 80;
generals[203].healthCur = 80;
generals[203].manaMax = 44;
generals[203].manaCur = 44;
generals[203].soldierMax = 10;
generals[203].soldierCur = 10;
generals[203].knightMax = 0;
generals[203].knightCur = 0;
generals[203].arms = 0x0100;
generals[203].formation = 0x11;

generals[204].king = -1;
generals[204].city = -1;
generals[204].magic[0] = 5;
generals[204].magic[1] = -1;
generals[204].magic[2] = -1;
generals[204].magic[3] = -1;
generals[204].equipment = -1;
generals[204].strength = 75;
generals[204].intellect = 73;
generals[204].experience = 200;
generals[204].level = 2;
generals[204].healthMax = 84;
generals[204].healthCur = 84;
generals[204].manaMax = 42;
generals[204].manaCur = 42;
generals[204].soldierMax = 10;
generals[204].soldierCur = 10;
generals[204].knightMax = 0;
generals[204].knightCur = 0;
generals[204].arms = 0x0048;
generals[204].formation = 0x1A;
		
generals[205].king = -1;
generals[205].city = -1;
generals[205].magic[0] = 5;
generals[205].magic[1] = -1;
generals[205].magic[2] = -1;
generals[205].magic[3] = -1;
generals[205].equipment = -1;
generals[205].strength = 75;
generals[205].intellect = 73;
generals[205].experience = 200;
generals[205].level = 2;
generals[205].healthMax = 84;
generals[205].healthCur = 84;
generals[205].manaMax = 42;
generals[205].manaCur = 42;
generals[205].soldierMax = 10;
generals[205].soldierCur = 10;
generals[205].knightMax = 0;
generals[205].knightCur = 0;
generals[205].arms = 0x0048;
generals[205].formation = 0x1A;
	
generals[206].king = -1;
generals[206].city = -1;
generals[206].magic[0] = 5;
generals[206].magic[1] = -1;
generals[206].magic[2] = -1;
generals[206].magic[3] = -1;
generals[206].equipment = -1;
generals[206].strength = 75;
generals[206].intellect = 73;
generals[206].experience = 200;
generals[206].level = 2;
generals[206].healthMax = 84;
generals[206].healthCur = 84;
generals[206].manaMax = 42;
generals[206].manaCur = 42;
generals[206].soldierMax = 10;
generals[206].soldierCur = 10;
generals[206].knightMax = 0;
generals[206].knightCur = 0;
generals[206].arms = 0x0048;
generals[206].formation = 0x1A;
		
generals[207].king = -1;
generals[207].city = -1;
generals[207].magic[0] = 21;
generals[207].magic[1] = -1;
generals[207].magic[2] = -1;
generals[207].magic[3] = -1;
generals[207].equipment = -1;
generals[207].strength = 77;
generals[207].intellect = 49;
generals[207].experience = 200;
generals[207].level = 2;
generals[207].healthMax = 84;
generals[207].healthCur = 84;
generals[207].manaMax = 38;
generals[207].manaCur = 38;
generals[207].soldierMax = 10;
generals[207].soldierCur = 10;
generals[207].knightMax = 0;
generals[207].knightCur = 0;
generals[207].arms = 0x0001;
generals[207].formation = 0x11;

generals[208].king = 14;
generals[208].city = 31;
generals[208].magic[0] = 24;
generals[208].magic[1] = -1;
generals[208].magic[2] = -1;
generals[208].magic[3] = -1;
generals[208].equipment = -1;
generals[208].strength = 73;
generals[208].intellect = 40;
generals[208].experience = 200;
generals[208].level = 2;
generals[208].healthMax = 82;
generals[208].healthCur = 82;
generals[208].manaMax = 37;
generals[208].manaCur = 37;
generals[208].soldierMax = 10;
generals[208].soldierCur = 10;
generals[208].knightMax = 0;
generals[208].knightCur = 0;
generals[208].arms = 0x0010;
generals[208].formation = 0x01;

generals[209].king = -1;
generals[209].city = -1;
generals[209].magic[0] = 26;
generals[209].magic[1] = -1;
generals[209].magic[2] = -1;
generals[209].magic[3] = -1;
generals[209].equipment = -1;
generals[209].strength = 79;
generals[209].intellect = 68;
generals[209].experience = 200;
generals[209].level = 2;
generals[209].healthMax = 84;
generals[209].healthCur = 84;
generals[209].manaMax = 42;
generals[209].manaCur = 42;
generals[209].soldierMax = 10;
generals[209].soldierCur = 10;
generals[209].knightMax = 0;
generals[209].knightCur = 0;
generals[209].arms = 0x0008;
generals[209].formation = 0x48;

generals[210].king = -1;
generals[210].city = -1;
generals[210].magic[0] = 25;
generals[210].magic[1] = -1;
generals[210].magic[2] = -1;
generals[210].magic[3] = -1;
generals[210].equipment = -1;
generals[210].strength = 56;
generals[210].intellect = 95;
generals[210].experience = 200;
generals[210].level = 2;
generals[210].healthMax = 80;
generals[210].healthCur = 80;
generals[210].manaMax = 47;
generals[210].manaCur = 47;
generals[210].soldierMax = 10;
generals[210].soldierCur = 10;
generals[210].knightMax = 0;
generals[210].knightCur = 0;
generals[210].arms = 0x0400;
generals[210].formation = 0x30;

generals[211].king = -1;
generals[211].city = -1;
generals[211].magic[0] = 25;
generals[211].magic[1] = -1;
generals[211].magic[2] = -1;
generals[211].magic[3] = -1;
generals[211].equipment = -1;
generals[211].strength = 84;
generals[211].intellect = 62;
generals[211].experience = 200;
generals[211].level = 2;
generals[211].healthMax = 85;
generals[211].healthCur = 85;
generals[211].manaMax = 41;
generals[211].manaCur = 41;
generals[211].soldierMax = 10;
generals[211].soldierCur = 10;
generals[211].knightMax = 0;
generals[211].knightCur = 0;
generals[211].arms = 0x0400;
generals[211].formation = 0x14;

generals[212].king = -1;
generals[212].city = -1;
generals[212].magic[0] = 25;
generals[212].magic[1] = -1;
generals[212].magic[2] = -1;
generals[212].magic[3] = -1;
generals[212].equipment = -1;
generals[212].strength = 82;
generals[212].intellect = 72;
generals[212].experience = 200;
generals[212].level = 2;
generals[212].healthMax = 85;
generals[212].healthCur = 85;
generals[212].manaMax = 42;
generals[212].manaCur = 42;
generals[212].soldierMax = 10;
generals[212].soldierCur = 10;
generals[212].knightMax = 0;
generals[212].knightCur = 0;
generals[212].arms = 0x0400;
generals[212].formation = 0x09;

generals[213].king = -1;
generals[213].city = -1;
generals[213].magic[0] = 25;
generals[213].magic[1] = -1;
generals[213].magic[2] = -1;
generals[213].magic[3] = -1;
generals[213].equipment = -1;
generals[213].strength = 78;
generals[213].intellect = 44;
generals[213].experience = 200;
generals[213].level = 2;
generals[213].healthMax = 84;
generals[213].healthCur = 84;
generals[213].manaMax = 37;
generals[213].manaCur = 37;
generals[213].soldierMax = 10;
generals[213].soldierCur = 10;
generals[213].knightMax = 0;
generals[213].knightCur = 0;
generals[213].arms = 0x0400;
generals[213].formation = 0x01;

generals[214].king = -1;
generals[214].city = -1;
generals[214].magic[0] = 24;
generals[214].magic[1] = -1;
generals[214].magic[2] = -1;
generals[214].magic[3] = -1;
generals[214].equipment = -1;
generals[214].strength = 76;
generals[214].intellect = 39;
generals[214].experience = 200;
generals[214].level = 2;
generals[214].healthMax = 84;
generals[214].healthCur = 84;
generals[214].manaMax = 36;
generals[214].manaCur = 36;
generals[214].soldierMax = 10;
generals[214].soldierCur = 10;
generals[214].knightMax = 0;
generals[214].knightCur = 0;
generals[214].arms = 0x0400;
generals[214].formation = 0x01;

generals[215].king = 8;
generals[215].city = 7;
generals[215].magic[0] = 23;
generals[215].magic[1] = -1;
generals[215].magic[2] = -1;
generals[215].magic[3] = -1;
generals[215].equipment = -1;
generals[215].strength = 93;
generals[215].intellect = 45;
generals[215].experience = 200;
generals[215].level = 2;
generals[215].healthMax = 86;
generals[215].healthCur = 86;
generals[215].manaMax = 37;
generals[215].manaCur = 37;
generals[215].soldierMax = 10;
generals[215].soldierCur = 10;
generals[215].knightMax = 0;
generals[215].knightCur = 0;
generals[215].arms = 0x0402;
generals[215].formation = 0x11;

generals[216].king = -1;
generals[216].city = -1;
generals[216].magic[0] = 24;
generals[216].magic[1] = -1;
generals[216].magic[2] = -1;
generals[216].magic[3] = -1;
generals[216].equipment = -1;
generals[216].strength = 71;
generals[216].intellect = 46;
generals[216].experience = 200;
generals[216].level = 2;
generals[216].healthMax = 82;
generals[216].healthCur = 82;
generals[216].manaMax = 37;
generals[216].manaCur = 37;
generals[216].soldierMax = 10;
generals[216].soldierCur = 10;
generals[216].knightMax = 0;
generals[216].knightCur = 0;
generals[216].arms = 0x0400;
generals[216].formation = 0x11;

generals[217].king = -1;
generals[217].city = -1;
generals[217].magic[0] = 24;
generals[217].magic[1] = -1;
generals[217].magic[2] = -1;
generals[217].magic[3] = -1;
generals[217].equipment = -1;
generals[217].strength = 69;
generals[217].intellect = 47;
generals[217].experience = 200;
generals[217].level = 2;
generals[217].healthMax = 82;
generals[217].healthCur = 82;
generals[217].manaMax = 38;
generals[217].manaCur = 38;
generals[217].soldierMax = 10;
generals[217].soldierCur = 10;
generals[217].knightMax = 0;
generals[217].knightCur = 0;
generals[217].arms = 0x0400;
generals[217].formation = 0x01;

generals[218].king = -1;
generals[218].city = 19;
generals[218].magic[0] = 24;
generals[218].magic[1] = -1;
generals[218].magic[2] = -1;
generals[218].magic[3] = -1;
generals[218].equipment = -1;
generals[218].strength = 72;
generals[218].intellect = 40;
generals[218].experience = 200;
generals[218].level = 2;
generals[218].healthMax = 82;
generals[218].healthCur = 82;
generals[218].manaMax = 37;
generals[218].manaCur = 37;
generals[218].soldierMax = 10;
generals[218].soldierCur = 10;
generals[218].knightMax = 0;
generals[218].knightCur = 0;
generals[218].arms = 0x0001;
generals[218].formation = 0x41;

generals[219].king = -1;
generals[219].city = 20;
generals[219].magic[0] = 23;
generals[219].magic[1] = -1;
generals[219].magic[2] = -1;
generals[219].magic[3] = -1;
generals[219].equipment = -1;
generals[219].strength = 70;
generals[219].intellect = 42;
generals[219].experience = 200;
generals[219].level = 2;
generals[219].healthMax = 82;
generals[219].healthCur = 82;
generals[219].manaMax = 37;
generals[219].manaCur = 37;
generals[219].soldierMax = 10;
generals[219].soldierCur = 10;
generals[219].knightMax = 0;
generals[219].knightCur = 0;
generals[219].arms = 0x0420;
generals[219].formation = 0x03;

generals[220].king = -1;
generals[220].city = -1;
generals[220].magic[0] = 4;
generals[220].magic[1] = -1;
generals[220].magic[2] = -1;
generals[220].magic[3] = -1;
generals[220].equipment = -1;
generals[220].strength = 43;
generals[220].intellect = 72;
generals[220].experience = 200;
generals[220].level = 2;
generals[220].healthMax = 77;
generals[220].healthCur = 77;
generals[220].manaMax = 42;
generals[220].manaCur = 42;
generals[220].soldierMax = 10;
generals[220].soldierCur = 10;
generals[220].knightMax = 0;
generals[220].knightCur = 0;
generals[220].arms = 0x0100;
generals[220].formation = 0x42;

generals[221].king = -1;
generals[221].city = -1;
generals[221].magic[0] = 21;
generals[221].magic[1] = -1;
generals[221].magic[2] = -1;
generals[221].magic[3] = -1;
generals[221].equipment = -1;
generals[221].strength = 83;
generals[221].intellect = 70;
generals[221].experience = 200;
generals[221].level = 2;
generals[221].healthMax = 85;
generals[221].healthCur = 85;
generals[221].manaMax = 42;
generals[221].manaCur = 42;
generals[221].soldierMax = 10;
generals[221].soldierCur = 10;
generals[221].knightMax = 0;
generals[221].knightCur = 0;
generals[221].arms = 0x0041;
generals[221].formation = 0x61;

generals[222].king = -1;
generals[222].city = -1;
generals[222].magic[0] = 27;
generals[222].magic[1] = -1;
generals[222].magic[2] = -1;
generals[222].magic[3] = -1;
generals[222].equipment = -1;
generals[222].strength = 73;
generals[222].intellect = 64;
generals[222].experience = 200;
generals[222].level = 2;
generals[222].healthMax = 82;
generals[222].healthCur = 82;
generals[222].manaMax = 41;
generals[222].manaCur = 41;
generals[222].soldierMax = 10;
generals[222].soldierCur = 10;
generals[222].knightMax = 0;
generals[222].knightCur = 0;
generals[222].arms = 0x0101;
generals[222].formation = 0x48;

generals[223].king = 3;
generals[223].city = 12;
generals[223].magic[0] = 19;
generals[223].magic[1] = -1;
generals[223].magic[2] = -1;
generals[223].magic[3] = -1;
generals[223].equipment = -1;
generals[223].strength = 74;
generals[223].intellect = 63;
generals[223].experience = 200;
generals[223].level = 2;
generals[223].healthMax = 83;
generals[223].healthCur = 83;
generals[223].manaMax = 41;
generals[223].manaCur = 41;
generals[223].soldierMax = 10;
generals[223].soldierCur = 10;
generals[223].knightMax = 0;
generals[223].knightCur = 0;
generals[223].arms = 0x0008;
generals[223].formation = 0x0C;

generals[224].king = -1;
generals[224].city = 32;
generals[224].magic[0] = 27;
generals[224].magic[1] = -1;
generals[224].magic[2] = -1;
generals[224].magic[3] = -1;
generals[224].equipment = -1;
generals[224].strength = 72;
generals[224].intellect = 42;
generals[224].experience = 200;
generals[224].level = 2;
generals[224].healthMax = 82;
generals[224].healthCur = 82;
generals[224].manaMax = 37;
generals[224].manaCur = 37;
generals[224].soldierMax = 10;
generals[224].soldierCur = 10;
generals[224].knightMax = 0;
generals[224].knightCur = 0;
generals[224].arms = 0x0002;
generals[224].formation = 0x21;

generals[225].king = 11;
generals[225].city = 16;
generals[225].magic[0] = 24;
generals[225].magic[1] = -1;
generals[225].magic[2] = -1;
generals[225].magic[3] = -1;
generals[225].equipment = -1;
generals[225].strength = 69;
generals[225].intellect = 32;
generals[225].experience = 200;
generals[225].level = 2;
generals[225].healthMax = 82;
generals[225].healthCur = 82;
generals[225].manaMax = 35;
generals[225].manaCur = 35;
generals[225].soldierMax = 10;
generals[225].soldierCur = 10;
generals[225].knightMax = 0;
generals[225].knightCur = 0;
generals[225].arms = 0x0420;
generals[225].formation = 0x01;

generals[226].king = -1;
generals[226].city = -1;
generals[226].magic[0] = 27;
generals[226].magic[1] = -1;
generals[226].magic[2] = -1;
generals[226].magic[3] = -1;
generals[226].equipment = -1;
generals[226].strength = 76;
generals[226].intellect = 63;
generals[226].experience = 200;
generals[226].level = 2;
generals[226].healthMax = 84;
generals[226].healthCur = 84;
generals[226].manaMax = 41;
generals[226].manaCur = 41;
generals[226].soldierMax = 10;
generals[226].soldierCur = 10;
generals[226].knightMax = 0;
generals[226].knightCur = 0;
generals[226].arms = 0x0002;
generals[226].formation = 0x01;

generals[227].king = -1;
generals[227].city = -1;
generals[227].magic[0] = 26;
generals[227].magic[1] = -1;
generals[227].magic[2] = -1;
generals[227].magic[3] = -1;
generals[227].equipment = -1;
generals[227].strength = 76;
generals[227].intellect = 70;
generals[227].experience = 200;
generals[227].level = 2;
generals[227].healthMax = 84;
generals[227].healthCur = 84;
generals[227].manaMax = 42;
generals[227].manaCur = 42;
generals[227].soldierMax = 10;
generals[227].soldierCur = 10;
generals[227].knightMax = 0;
generals[227].knightCur = 0;
generals[227].arms = 0x0012;
generals[227].formation = 0x09;

generals[228].king = -1;
generals[228].city = -1;
generals[228].magic[0] = 24;
generals[228].magic[1] = -1;
generals[228].magic[2] = -1;
generals[228].magic[3] = -1;
generals[228].equipment = -1;
generals[228].strength = 81;
generals[228].intellect = 56;
generals[228].experience = 200;
generals[228].level = 2;
generals[228].healthMax = 85;
generals[228].healthCur = 85;
generals[228].manaMax = 40;
generals[228].manaCur = 40;
generals[228].soldierMax = 10;
generals[228].soldierCur = 10;
generals[228].knightMax = 0;
generals[228].knightCur = 0;
generals[228].arms = 0x0011;
generals[228].formation = 0x22;

generals[229].king = -1;
generals[229].city = -1;
generals[229].magic[0] = 23;
generals[229].magic[1] = -1;
generals[229].magic[2] = -1;
generals[229].magic[3] = -1;
generals[229].equipment = -1;
generals[229].strength = 79;
generals[229].intellect = 47;
generals[229].experience = 200;
generals[229].level = 2;
generals[229].healthMax = 84;
generals[229].healthCur = 84;
generals[229].manaMax = 38;
generals[229].manaCur = 38;
generals[229].soldierMax = 10;
generals[229].soldierCur = 10;
generals[229].knightMax = 0;
generals[229].knightCur = 0;
generals[229].arms = 0x000A;
generals[229].formation = 0x05;

generals[230].king = 3;
generals[230].city = 11;
generals[230].magic[0] = 24;
generals[230].magic[1] = -1;
generals[230].magic[2] = -1;
generals[230].magic[3] = -1;
generals[230].equipment = -1;
generals[230].strength = 80;
generals[230].intellect = 43;
generals[230].experience = 200;
generals[230].level = 2;
generals[230].healthMax = 85;
generals[230].healthCur = 85;
generals[230].manaMax = 37;
generals[230].manaCur = 37;
generals[230].soldierMax = 10;
generals[230].soldierCur = 10;
generals[230].knightMax = 0;
generals[230].knightCur = 0;
generals[230].arms = 0x0001;
generals[230].formation = 0x14;

generals[231].king = -1;
generals[231].city = 9;
generals[231].magic[0] = 22;
generals[231].magic[1] = -1;
generals[231].magic[2] = -1;
generals[231].magic[3] = -1;
generals[231].equipment = 12;
generals[231].strength = 80;
generals[231].intellect = 58;
generals[231].experience = 200;
generals[231].level = 2;
generals[231].healthMax = 83;
generals[231].healthCur = 83;
generals[231].manaMax = 40;
generals[231].manaCur = 40;
generals[231].soldierMax = 10;
generals[231].soldierCur = 10;
generals[231].knightMax = 0;
generals[231].knightCur = 0;
generals[231].arms = 0x0008;
generals[231].formation = 0x09;

generals[232].king = -1;
generals[232].city = -1;
generals[232].magic[0] = 27;
generals[232].magic[1] = -1;
generals[232].magic[2] = -1;
generals[232].magic[3] = -1;
generals[232].equipment = -1;
generals[232].strength = 75;
generals[232].intellect = 60;
generals[232].experience = 180;
generals[232].level = 2;
generals[232].healthMax = 84;
generals[232].healthCur = 84;
generals[232].manaMax = 41;
generals[232].manaCur = 41;
generals[232].soldierMax = 10;
generals[232].soldierCur = 10;
generals[232].knightMax = 0;
generals[232].knightCur = 0;
generals[232].arms = 0x0010;
generals[232].formation = 0x05;

generals[233].king = -1;
generals[233].city = -1;
generals[233].magic[0] = 19;
generals[233].magic[1] = -1;
generals[233].magic[2] = -1;
generals[233].magic[3] = -1;
generals[233].equipment = -1;
generals[233].strength = 78;
generals[233].intellect = 68;
generals[233].experience = 200;
generals[233].level = 2;
generals[233].healthMax = 84;
generals[233].healthCur = 84;
generals[233].manaMax = 42;
generals[233].manaCur = 42;
generals[233].soldierMax = 10;
generals[233].soldierCur = 10;
generals[233].knightMax = 0;
generals[233].knightCur = 0;
generals[233].arms = 0x0022;
generals[233].formation = 0x06;

generals[234].king = 0;
generals[234].city = 9;
generals[234].magic[0] = 21;
generals[234].magic[1] = -1;
generals[234].magic[2] = -1;
generals[234].magic[3] = -1;
generals[234].equipment = -1;
generals[234].strength = 74;
generals[234].intellect = 70;
generals[234].experience = 200;
generals[234].level = 2;
generals[234].healthMax = 83;
generals[234].healthCur = 83;
generals[234].manaMax = 42;
generals[234].manaCur = 42;
generals[234].soldierMax = 10;
generals[234].soldierCur = 10;
generals[234].knightMax = 0;
generals[234].knightCur = 0;
generals[234].arms = 0x0001;
generals[234].formation = 0x14;

generals[235].king = -1;
generals[235].city = -1;
generals[235].magic[0] = 19;
generals[235].magic[1] = -1;
generals[235].magic[2] = -1;
generals[235].magic[3] = -1;
generals[235].equipment = -1;
generals[235].strength = 82;
generals[235].intellect = 64;
generals[235].experience = 200;
generals[235].level = 2;
generals[235].healthMax = 85;
generals[235].healthCur = 85;
generals[235].manaMax = 41;
generals[235].manaCur = 41;
generals[235].soldierMax = 10;
generals[235].soldierCur = 10;
generals[235].knightMax = 0;
generals[235].knightCur = 0;
generals[235].arms = 0x0001;
generals[235].formation = 0x51;

generals[236].king = -1;
generals[236].city = 27;
generals[236].magic[0] = 0;
generals[236].magic[1] = -1;
generals[236].magic[2] = -1;
generals[236].magic[3] = -1;
generals[236].equipment = -1;
generals[236].strength = 65;
generals[236].intellect = 52;
generals[236].experience = 200;
generals[236].level = 2;
generals[236].healthMax = 81;
generals[236].healthCur = 81;
generals[236].manaMax = 39;
generals[236].manaCur = 39;
generals[236].soldierMax = 10;
generals[236].soldierCur = 10;
generals[236].knightMax = 0;
generals[236].knightCur = 0;
generals[236].arms = 0x0100;
generals[236].formation = 0x03;

generals[237].king = -1;
generals[237].city = -1;
generals[237].magic[0] = 20;
generals[237].magic[1] = -1;
generals[237].magic[2] = -1;
generals[237].magic[3] = -1;
generals[237].equipment = -1;
generals[237].strength = 72;
generals[237].intellect = 54;
generals[237].experience = 200;
generals[237].level = 2;
generals[237].healthMax = 82;
generals[237].healthCur = 82;
generals[237].manaMax = 40;
generals[237].manaCur = 40;
generals[237].soldierMax = 10;
generals[237].soldierCur = 10;
generals[237].knightMax = 0;
generals[237].knightCur = 0;
generals[237].arms = 0x0009;
generals[237].formation = 0x12;

generals[238].king = -1;
generals[238].city = 22;
generals[238].magic[0] = 1;
generals[238].magic[1] = -1;
generals[238].magic[2] = -1;
generals[238].magic[3] = -1;
generals[238].equipment = -1;
generals[238].strength = 34;
generals[238].intellect = 83;
generals[238].experience = 200;
generals[238].level = 2;
generals[238].healthMax = 76;
generals[238].healthCur = 76;
generals[238].manaMax = 45;
generals[238].manaCur = 45;
generals[238].soldierMax = 10;
generals[238].soldierCur = 10;
generals[238].knightMax = 0;
generals[238].knightCur = 0;
generals[238].arms = 0x0040;
generals[238].formation = 0x11;

generals[239].king = -1;
generals[239].city = 36;
generals[239].magic[0] = 2;
generals[239].magic[1] = -1;
generals[239].magic[2] = -1;
generals[239].magic[3] = -1;
generals[239].equipment = -1;
generals[239].strength = 48;
generals[239].intellect = 76;
generals[239].experience = 200;
generals[239].level = 2;
generals[239].healthMax = 78;
generals[239].healthCur = 78;
generals[239].manaMax = 44;
generals[239].manaCur = 44;
generals[239].soldierMax = 10;
generals[239].soldierCur = 10;
generals[239].knightMax = 0;
generals[239].knightCur = 0;
generals[239].arms = 0x0008;
generals[239].formation = 0x24;

generals[240].king = -1;
generals[240].city = 35;
generals[240].magic[0] = 22;
generals[240].magic[1] = -1;
generals[240].magic[2] = -1;
generals[240].magic[3] = -1;
generals[240].equipment = -1;
generals[240].strength = 79;
generals[240].intellect = 36;
generals[240].experience = 200;
generals[240].level = 2;
generals[240].healthMax = 84;
generals[240].healthCur = 84;
generals[240].manaMax = 36;
generals[240].manaCur = 36;
generals[240].soldierMax = 10;
generals[240].soldierCur = 10;
generals[240].knightMax = 0;
generals[240].knightCur = 0;
generals[240].arms = 0x0002;
generals[240].formation = 0x0C;

generals[241].king = -1;
generals[241].city = -1;
generals[241].magic[0] = 26;
generals[241].magic[1] = -1;
generals[241].magic[2] = -1;
generals[241].magic[3] = -1;
generals[241].equipment = -1;
generals[241].strength = 73;
generals[241].intellect = 68;
generals[241].experience = 200;
generals[241].level = 2;
generals[241].healthMax = 82;
generals[241].healthCur = 82;
generals[241].manaMax = 42;
generals[241].manaCur = 42;
generals[241].soldierMax = 10;
generals[241].soldierCur = 10;
generals[241].knightMax = 0;
generals[241].knightCur = 0;
generals[241].arms = 0x0110;
generals[241].formation = 0x0A;

generals[242].king = -1;
generals[242].city = 33;
generals[242].magic[0] = 4;
generals[242].magic[1] = -1;
generals[242].magic[2] = -1;
generals[242].magic[3] = -1;
generals[242].equipment = -1;
generals[242].strength = 30;
generals[242].intellect = 97;
generals[242].experience = 200;
generals[242].level = 2;
generals[242].healthMax = 75;
generals[242].healthCur = 75;
generals[242].manaMax = 47;
generals[242].manaCur = 47;
generals[242].soldierMax = 10;
generals[242].soldierCur = 10;
generals[242].knightMax = 0;
generals[242].knightCur = 0;
generals[242].arms = 0x0020;
generals[242].formation = 0x31;

generals[243].king = -1;
generals[243].city = 35;
generals[243].magic[0] = 4;
generals[243].magic[1] = -1;
generals[243].magic[2] = -1;
generals[243].magic[3] = -1;
generals[243].equipment = -1;
generals[243].strength = 33;
generals[243].intellect = 98;
generals[243].experience = 200;
generals[243].level = 2;
generals[243].healthMax = 75;
generals[243].healthCur = 75;
generals[243].manaMax = 47;
generals[243].manaCur = 47;
generals[243].soldierMax = 10;
generals[243].soldierCur = 10;
generals[243].knightMax = 0;
generals[243].knightCur = 0;
generals[243].arms = 0x0040;
generals[243].formation = 0x1A;

generals[244].king = 2;
generals[244].city = 28;
generals[244].magic[0] = 0;
generals[244].magic[1] = -1;
generals[244].magic[2] = -1;
generals[244].magic[3] = -1;
generals[244].equipment = -1;
generals[244].strength = 55;
generals[244].intellect = 77;
generals[244].experience = 200;
generals[244].level = 2;
generals[244].healthMax = 80;
generals[244].healthCur = 80;
generals[244].manaMax = 44;
generals[244].manaCur = 44;
generals[244].soldierMax = 10;
generals[244].soldierCur = 10;
generals[244].knightMax = 0;
generals[244].knightCur = 0;
generals[244].arms = 0x0008;
generals[244].formation = 0x22;

generals[245].king = -1;
generals[245].city = -1;
generals[245].magic[0] = 1;
generals[245].magic[1] = -1;
generals[245].magic[2] = -1;
generals[245].magic[3] = -1;
generals[245].equipment = -1;
generals[245].strength = 41;
generals[245].intellect = 86;
generals[245].experience = 200;
generals[245].level = 2;
generals[245].healthMax = 77;
generals[245].healthCur = 77;
generals[245].manaMax = 45;
generals[245].manaCur = 45;
generals[245].soldierMax = 10;
generals[245].soldierCur = 10;
generals[245].knightMax = 0;
generals[245].knightCur = 0;
generals[245].arms = 0x0021;
generals[245].formation = 0x13;

generals[246].king = -1;
generals[246].city = -1;
generals[246].magic[0] = 1;
generals[246].magic[1] = -1;
generals[246].magic[2] = -1;
generals[246].magic[3] = -1;
generals[246].equipment = -1;
generals[246].strength = 58;
generals[246].intellect = 83;
generals[246].experience = 200;
generals[246].level = 2;
generals[246].healthMax = 80;
generals[246].healthCur = 80;
generals[246].manaMax = 45;
generals[246].manaCur = 45;
generals[246].soldierMax = 10;
generals[246].soldierCur = 10;
generals[246].knightMax = 0;
generals[246].knightCur = 0;
generals[246].arms = 0x0140;
generals[246].formation = 0x34;

generals[247].king = -1;
generals[247].city = -1;
generals[247].magic[0] = 22;
generals[247].magic[1] = -1;
generals[247].magic[2] = -1;
generals[247].magic[3] = -1;
generals[247].equipment = -1;
generals[247].strength = 85;
generals[247].intellect = 41;
generals[247].experience = 200;
generals[247].level = 2;
generals[247].healthMax = 85;
generals[247].healthCur = 85;
generals[247].manaMax = 37;
generals[247].manaCur = 37;
generals[247].soldierMax = 10;
generals[247].soldierCur = 10;
generals[247].knightMax = 0;
generals[247].knightCur = 0;
generals[247].arms = 0x0011;
generals[247].formation = 0x06;

generals[248].king = 3;
generals[248].city = 12;
generals[248].magic[0] = 21;
generals[248].magic[1] = -1;
generals[248].magic[2] = -1;
generals[248].magic[3] = -1;
generals[248].equipment = -1;
generals[248].strength = 78;
generals[248].intellect = 51;
generals[248].experience = 200;
generals[248].level = 2;
generals[248].healthMax = 84;
generals[248].healthCur = 84;
generals[248].manaMax = 39;
generals[248].manaCur = 39;
generals[248].soldierMax = 10;
generals[248].soldierCur = 10;
generals[248].knightMax = 0;
generals[248].knightCur = 0;
generals[248].arms = 0x0040;
generals[248].formation = 0x01;

generals[249].king = -1;
generals[249].city = -1;
generals[249].magic[0] = 1;
generals[249].magic[1] = -1;
generals[249].magic[2] = -1;
generals[249].magic[3] = -1;
generals[249].equipment = -1;
generals[249].strength = 46;
generals[249].intellect = 85;
generals[249].experience = 200;
generals[249].level = 2;
generals[249].healthMax = 77;
generals[249].healthCur = 77;
generals[249].manaMax = 45;
generals[249].manaCur = 45;
generals[249].soldierMax = 10;
generals[249].soldierCur = 10;
generals[249].knightMax = 0;
generals[249].knightCur = 0;
generals[249].arms = 0x0008;
generals[249].formation = 0x03;

generals[250].king = -1;
generals[250].city = -1;
generals[250].magic[0] = 0;
generals[250].magic[1] = -1;
generals[250].magic[2] = -1;
generals[250].magic[3] = -1;
generals[250].equipment = -1;
generals[250].strength = 43;
generals[250].intellect = 63;
generals[250].experience = 200;
generals[250].level = 2;
generals[250].healthMax = 77;
generals[250].healthCur = 77;
generals[250].manaMax = 41;
generals[250].manaCur = 41;
generals[250].soldierMax = 10;
generals[250].soldierCur = 10;
generals[250].knightMax = 0;
generals[250].knightCur = 0;
generals[250].arms = 0x0100;
generals[250].formation = 0x02;

generals[251].king = -1;
generals[251].city = 36;
generals[251].magic[0] = 19;
generals[251].magic[1] = -1;
generals[251].magic[2] = -1;
generals[251].magic[3] = -1;
generals[251].equipment = -1;
generals[251].strength = 81;
generals[251].intellect = 74;
generals[251].experience = 200;
generals[251].level = 2;
generals[251].healthMax = 85;
generals[251].healthCur = 85;
generals[251].manaMax = 43;
generals[251].manaCur = 43;
generals[251].soldierMax = 10;
generals[251].soldierCur = 10;
generals[251].knightMax = 0;
generals[251].knightCur = 0;
generals[251].arms = 0x0001;
generals[251].formation = 0x06;

generals[252].king = -1;
generals[252].city = -1;
generals[252].magic[0] = 19;
generals[252].magic[1] = -1;
generals[252].magic[2] = -1;
generals[252].magic[3] = -1;
generals[252].equipment = -1;
generals[252].strength = 77;
generals[252].intellect = 44;
generals[252].experience = 200;
generals[252].level = 2;
generals[252].healthMax = 84;
generals[252].healthCur = 84;
generals[252].manaMax = 37;
generals[252].manaCur = 37;
generals[252].soldierMax = 10;
generals[252].soldierCur = 10;
generals[252].knightMax = 0;
generals[252].knightCur = 0;
generals[252].arms = 0x0020;
generals[252].formation = 0x09;

generals[253].king = -1;
generals[253].city = -1;
generals[253].magic[0] = 27;
generals[253].magic[1] = -1;
generals[253].magic[2] = -1;
generals[253].magic[3] = -1;
generals[253].equipment = -1;
generals[253].strength = 81;
generals[253].intellect = 44;
generals[253].experience = 200;
generals[253].level = 2;
generals[253].healthMax = 85;
generals[253].healthCur = 85;
generals[253].manaMax = 37;
generals[253].manaCur = 37;
generals[253].soldierMax = 10;
generals[253].soldierCur = 10;
generals[253].knightMax = 0;
generals[253].knightCur = 0;
generals[253].arms = 0x0002;
generals[253].formation = 0x09;

generals[254].king = -1;
generals[254].city = -1;
generals[254].magic[0] = 2;
generals[254].magic[1] = -1;
generals[254].magic[2] = -1;
generals[254].magic[3] = -1;
generals[254].equipment = -1;
generals[254].strength = 53;
generals[254].intellect = 74;
generals[254].experience = 200;
generals[254].level = 2;
generals[254].healthMax = 79;
generals[254].healthCur = 79;
generals[254].manaMax = 43;
generals[254].manaCur = 43;
generals[254].soldierMax = 10;
generals[254].soldierCur = 10;
generals[254].knightMax = 0;
generals[254].knightCur = 0;
generals[254].arms = 0x0100;
generals[254].formation = 0x03;


	}
	
	private void InitEquipmentInfo() {
		if (equipments == null) {
			equipments = new EquipmentInfo[equipmentNum];
			for (int i=0; i<equipmentNum; i++) {
				equipments[i] = new EquipmentInfo();
			}
		}
		
		equipments[0].type = 0; // zhi li
		equipments[0].data = 10;
		
		equipments[1].type = 0;
		equipments[1].data = 9;
		
		equipments[2].type = 0;
		equipments[2].data = 9;
		
		equipments[3].type = 0;
		equipments[3].data = 8;
		
		equipments[4].type = 0;
		equipments[4].data = 7;
		
		equipments[5].type = 1; // ji li
		equipments[5].data = 10;
		
		equipments[6].type = 2; // sheng ming
		equipments[6].data = 10;
		
		equipments[7].type = 1;
		equipments[7].data = 10;
		
		equipments[8].type = 3; // wu li
		equipments[8].data = 10;
		
		equipments[9].type = 3;
		equipments[9].data = 10;
		
		equipments[10].type = 3;
		equipments[10].data = 9;
		
		equipments[11].type = 3;
		equipments[11].data = 9;
		
		equipments[12].type = 3;
		equipments[12].data = 8;
		
		equipments[13].type = 3;
		equipments[13].data = 5;
		
		equipments[14].type = 3;
		equipments[14].data = 7;
		
		equipments[15].type = 3;
		equipments[15].data = 7;
		
		equipments[16].type = 3;
		equipments[16].data = 6;
		
		equipments[17].type = 3;
		equipments[17].data = 6;
		
		equipments[18].type = 3;
		equipments[18].data = 6;
		
		equipments[19].type = 3;
		equipments[19].data = 4;
		
		equipments[20].type = 3;
		equipments[20].data = 4;
		
		equipments[21].type = 3;
		equipments[21].data = 4;
		
		equipments[22].type = 3;
		equipments[22].data = 4;
		
		equipments[23].type = 3;
		equipments[23].data = 4;
		
		equipments[24].type = 3;
		equipments[24].data = 2;
		
		equipments[25].type = 3;
		equipments[25].data = 2;
		
		equipments[26].type = 3;
		equipments[26].data = 2;
		
		equipments[27].type = 4;
		equipments[27].data = 3;
		
		equipments[28].type = 4;
		equipments[28].data = 4;
		
		equipments[29].type = 4;
		equipments[29].data = 2;
		
		equipments[30].type = 4;
		equipments[30].data = 1;
	}

	public int[,] generalBody = new int[255, 2]{
		{7, 5},		//丁奉
		{2, 12},		//于禁
		{2, 15},		//兀突骨
		{5, 8},		//公孙瓒
		{7, 5},		//卞喜
		{7, 15},		//太史慈
		{7, 6},		//孔岫
		{0, 11},		//孔融
		{3, 12},		//文钦
		{4, 0},		//文聘
		{6, 3},		//文鸯
		{7, 11},		//文丑
		{4, 6},		//牛金
		{9, 5},		//王允
		{0, 14},		//王双
		{9, 16},		//司马炎
		{3, 3},		//司马昭
		{4, 6},		//司马师
		{9, 16},		//司马懿
		{0, 15},		//甘宁
		{7, 4},		//田丰
		{6, 6},		//伊籍
		{1, 15},		//全琮
		{7, 11},		//忙牙长
		{9, 11},		//朱桓
		{1, 16},		//朵思大王
		{7, 12},		//吴兰
		{1, 12},		//吴懿
		{0, 9},		//吕布
		{1, 16},		//吕蒙
		{9, 5},		//宋宪
		{6, 6},		//李典
		{1, 0},		//李恢
		{6, 14},		//李儒
		{0, 4},		//李严
		{5, 6},		//李傕
		{0, 2},		//步鹭
		{3, 16},		//沙摩诃
		{9, 3},		//车胄
		{0, 6},		//邢道荣
		{7, 4},		//典韦
		{4, 5},		//周仓
		{1, 4},		//周泰
		{4, 16},		//周瑜
		{6, 0},		//孟达
		{4, 0},		//孟优
		{7, 12},		//孟获
		{5, 14},		//法正
		{3, 16},		//沮授
		{8, 4},		//金环三结
		{1, 11},		//阿会喃
		{4, 12},		//姜维
		{7, 12},		//纪灵
		{5, 6},		//胡车儿
		{8, 4},		//凌统
		{4, 5},		//凌操
		{7, 15},		//夏侯惇
		{1, 14},		//夏侯渊
		{6, 5},		//夏侯霸
		{4, 3},		//孙坚
		{3, 14},		//孙策
		{6, 14},		//孙权
		{0, 0},		//孙翊
		{1, 2},		//徐晃
		{3, 16},		//徐庶
		{1, 7},		//徐盛
		{7, 6},		//徐质
		{1, 1},		//祝融夫人
		{9, 16},		//荀攸
		{9, 16},		//荀彧
		{4, 1},		//袁尚
		{2, 7},		//袁绍
		{2, 1},		//袁术
		{6, 5},		//袁熙
		{0, 3},		//袁谭
		{6, 15},		//郝昭
		{4, 4},		//马良
		{3, 0},		//马岱
		{4, 12},		//马超
		{0, 4},		//马腾
		{9, 0},		//马谡
		{9, 12},		//高顺
		{3, 5},		//高览
		{4, 15},		//张任
		{3, 12},		//张松
		{8, 16},		//张虎
		{7, 12},		//张昭
		{9, 8},		//张苞
		{6, 15},		//张郃
		{7, 10},		//张飞
		{2, 7},		//张鲁
		{5, 11},		//张辽
		{6, 14},		//张纮
		{4, 14},		//曹仁
		{8, 5},		//曹芳
		{3, 14},		//曹爽
		{9, 7},		//曹植
		{9, 8},		//曹彰
		{2, 3},		//曹操
		{7, 11},		//曹睿
		{4, 12},		//许褚
		{9, 5},		//逢纪
		{9, 16},		//郭嘉
		{6, 12},		//郭图
		{0, 7},		//郭汜
		{7, 15},		//陈宫
		{8, 7},		//陈琳
		{6, 8},		//陈群
		{1, 11},		//陆抗
		{2, 16},		//陆逊
		{0, 16},		//陶谦
		{2, 1},		//程昱
		{7, 0},		//程普
		{4, 14},		//华雄
		{6, 7},		//华歆
		{6, 7},		//黄忠
		{8, 7},		//黄祖
		{5, 2},		//黄盖
		{1, 15},		//黄权
		{7, 0},		//杨修
		{7, 4},		//董允
		{7, 15},		//董卓
		{5, 14},		//董荼那
		{2, 2},		//贾充
		{9, 16},		//贾诩
		{3, 6},		//廖化
		{7, 5},		//满宠
		{5, 8},		//赵统
		{5, 5},		//赵云
		{5, 0},		//赵广
		{9, 2},		//蒯良
		{3, 8},		//蒯越
		{3, 2},		//刘表
		{2, 0},		//刘焉
		{2, 2},		//刘备
		{6, 0},		//刘晔
		{9, 0},		//刘禅
		{1, 0},		//刘繇
		{9, 6},		//樊稠
		{4, 5},		//乐进
		{7, 5},		//潘璋
		{6, 7},		//蒋济
		{5, 1},		//蒋琬
		{6, 14},		//蔡邕
		{9, 6},		//蔡瑁
		{9, 16},		//诸葛亮
		{8, 0},		//诸葛恪
		{8, 8},		//诸葛瑾
		{9, 16},		//鲁肃
		{1, 1},		//邓艾
		{4, 12},		//邓忠
		{2, 15},		//邓芝
		{0, 11},		//卢植
		{2, 1},		//阎圃
		{3, 12},		//钟会
		{3, 3},		//韩当
		{2, 11},		//韩馥
		{4, 14},		//颜良
		{7, 15},		//魏延
		{1, 14},		//魏续
		{1, 12},		//庞统
		{7, 11},		//庞德
		{2, 0},		//谯周
		{5, 5},		//关平
		{4, 13},		//关羽
		{6, 12},		//关索
		{0, 11},		//关兴
		{2, 8},		//严白虎
		{8, 15},		//严纲
		{9, 6},		//严舆
		{7, 15},		//严颜
		{8, 1},		//公孙越
		{5, 15},		//王朗
		{4, 0},		//朱治
		{7, 6},		//辛评
		{9, 14},		//武安国
		{4, 0},		//皇甫嵩
		{1, 6},		//孙乾
		{3, 4},		//祖茂
		{7, 15},		//马玩
		{5, 2},		//高沛
		{8, 5},		//张勋
		{3, 1},		//张济
		{8, 6},		//曹洪
		{0, 3},		//梁兴
		{9, 3},		//陈武
		{4, 7},		//陈登
		{8, 16},		//陈横
		{0, 3},		//乔玄
		{5, 3},		//乔瑁
		{1, 0},		//关凤
		{9, 11},		//杨怀
		{0, 0},		//虞翻
		{4, 11},		//邹靖
		{3, 16},		//雷铜
		{4, 6},		//雷薄
		{9, 12},		//刘璋
		{2, 0},		//潘凤
		{9, 0},		//霍峻
		{8, 1},		//糜竺
		{5, 4},		//糜芳
		{1, 3},		//韩嵩
		{5, 11},		//简雍
		{0, 0},		//阚泽
		{3, 14},		//曹丕
		{1, 0},		//貂蝉
		{1, 0},		//孙尚香
		{1, 11},		//何进
		{6, 12},		//张英
		{0, 2},		//丁原
		{2, 3},		//张角
		{6, 0},		//张梁
		{1, 7},		//张宝
		{4, 16},		//程远志
		{0, 15},		//邓茂
		{5, 0},		//管亥
		{1, 0},		//赵弘
		{4, 0},		//韩忠
		{5, 2},		//龚都
		{4, 11},		//何仪
		{9, 3},		//龚景
		{1, 11},		//曹真
		{6, 3},		//刘封
		{9, 16},		//董承
		{1, 0},		//董袭
		{9, 5},		//张闿
		{8, 3},		//张翼
		{0, 4},		//张嶷
		{7, 3},		//彻里吉
		{2, 2},		//臧霸
		{0, 14},		//徐荣
		{9, 16},		//夏侯恩
		{2, 1},		//淳于琼
		{6, 0},		//曹休
		{0, 8},		//曹纯
		{2, 0},		//孙韶
		{1, 1},		//金旋
		{9, 0},		//公孙康
		{6, 0},		//向朗
		{1, 0},		//吕范
		{4, 12},		//李异
		{2, 5},		//夏侯尚
		{5, 16},		//于吉
		{0, 16},		//左慈
		{1, 6},		//孙静
		{2, 3},		//桓范
		{1, 11},		//费祎
		{4, 12},		//轲比能
		{0, 14},		//董旻
		{9, 2},		//刘琦
		{0, 15},		//刘琮
		{1, 6},		//蒋钦
		{5, 4},		//苏飞
		{9, 8},		//谭雄
		{4, 11},		//顾雍
		
	};
}