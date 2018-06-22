using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectGeneralToWarController : MonoBehaviour {
	
	public TopBarInfo leftBar;
	public TopBarInfo rightBar;
	
	public SGSelectGeneralInfo leftGeneralInfo;
	public SGSelectGeneralInfo rightGeneralInfo;
	
	public SGGeneralsListController generalListCtrl;
	
	public SGSelectFormation selectFormation;
	public SGGeneralsInfoController generalsInfo;
	public GameObject generalPos;
	public GameObject retreatConfirm;
	
	public DialogueController dialogCtrl;
	
	public Button[] menus;
	
	public static int mode;
	public static int cityAttacked;
	public static ArmyInfo mine;
	public static ArmyInfo enemy;
	
	public static bool isWarBegin = true;
	public static int warResult;
	
	private static int leftSelectIdx;
	private static int rightSelectIdx;
	private static int leftKing;
	private static int rightKing;
	private static int leftDefense;
	private static int rightDefense;
	private static int leftExperience;
	private static int rightExperience;
	private static int mineCommander;
	
	private static List<int> leftGenerals;
	private static List<int> rightGenerals;
	
	private static bool[] leftFailFlag;
	private static bool[] rightFailFlag;
	private static int[] gPos; //0:back 1:front
	
	private int state;
	private int menuSelectIdx = -1;
	private bool isPrisoned;
	private bool isWarOver;
	private int prisonCheckIdx;
	
	// Use this for initialization
	void Start () {
		//test
		/*
		if (isWarBegin) {
			mine = new ArmyInfo();
			mine.king = 0;
			mine.cityFrom = 0;
			mine.cityTo = 1;
			mine.commander = 98;
			mine.generals.Add(98);
			for (int i=0; i<20; i++) {
				int gIdx = Random.Range(0, 90);
				if (Informations.Instance.GetGeneralInfo(gIdx).king != -1) {
					mine.generals.Add(gIdx);
				}
			}
			enemy = new ArmyInfo();
			enemy.king = 1;
			enemy.cityFrom = 1;
			enemy.cityTo = 0;
			
			for (int i=0; i<5; i++) {
				int gIdx = Random.Range(99, 200);
				if (Informations.Instance.GetGeneralInfo(gIdx).king != -1) {
					enemy.generals.Add(gIdx);
				}
			}
		}
		*/
		InitScene();
		InitMenuAction();
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (state) {
		case 1:
			if (!dialogCtrl.IsShowingText() && Input.GetMouseButtonUp(0)) {
				dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				
				state = 0;
				Invoke("OnReturnMain", 0.5f);
			}
			break;
		case 2:
			if (!dialogCtrl.IsShowingText() && Input.GetMouseButtonUp(0)) {
				dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				
				state = 0;
				Invoke("WarOverResult", 0.5f);
			}
			break;
		case 3:
			if (!dialogCtrl.IsShowingText() && Input.GetMouseButtonUp(0)) {
				Input.ResetInputAxes();
				if (warResult == 0) {
					bool flag = false;
					for (int i=0; i<rightGenerals.Count; i++) {
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(rightGenerals[i]);
						if (gInfo.prisonerIdx != -1) {
							gInfo.prisonerIdx = -1;
							flag= true;
						}
					}
					
					if (flag) {
						state = 4;
						dialogCtrl.SetText(ZhongWen.Instance.womengbeifude);
					} else {
						flag = false;
						for (int i=0; i<leftGenerals.Count; i++) {
							GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(leftGenerals[i]);
							if (gInfo.prisonerIdx != -1) {
								flag= true;
								prisonCheckIdx = i;
								break;
							}
						}
						
						if (!flag) {
							dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				
							state = 0;
							Invoke("WarOverResult", 0.5f);
						} else {
							state = 5;
							dialogCtrl.SetText(ZhongWen.Instance.womenfulule + ZhongWen.Instance.GetGeneralName(leftGenerals[prisonCheckIdx++]) + ZhongWen.Instance.tanhao);
						}
					}
				} else {
					bool flag = false;
					for (int i=0; i<leftGenerals.Count; i++) {
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(leftGenerals[i]);
						if (gInfo.prisonerIdx != -1) {
							gInfo.prisonerIdx = -1;
							flag= true;
						}
					}
					
					if (flag) {
						state = 4;
						dialogCtrl.SetText(ZhongWen.Instance.fuludewujiang);
					} else {
						flag = false;
						for (int i=0; i<rightGenerals.Count; i++) {
							GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(rightGenerals[i]);
							if (gInfo.prisonerIdx != -1) {
								flag= true;
								prisonCheckIdx = i;
								break;
							}
						}
						
						if (!flag) {
							dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
				
							state = 0;
							Invoke("WarOverResult", 0.5f);
						} else {
							state = 5;
							dialogCtrl.SetText(ZhongWen.Instance.GetGeneralName(rightGenerals[prisonCheckIdx++]) + ZhongWen.Instance.beifulule + ZhongWen.Instance.tanhao);
						}
					}
				}
			}
			break;
		case 4:
			if (!dialogCtrl.IsShowingText() && Input.GetMouseButtonUp(0)) {
				Input.ResetInputAxes();
				if (warResult == 0) {
					bool flag = false;
					for (int i=0; i<leftGenerals.Count; i++) {
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(leftGenerals[i]);
						if (gInfo.prisonerIdx != -1) {
							flag= true;
							prisonCheckIdx = i;
							break;
						}
					}
					
					if (!flag) {
						dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
			
						state = 0;
						Invoke("WarOverResult", 0.5f);
					} else {
						state = 5;
						dialogCtrl.SetText(ZhongWen.Instance.womenfulule + ZhongWen.Instance.GetGeneralName(leftGenerals[prisonCheckIdx++]) + ZhongWen.Instance.tanhao);
					}
				} else {
					bool flag = false;
					for (int i=0; i<rightGenerals.Count; i++) {
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(rightGenerals[i]);
						if (gInfo.prisonerIdx != -1) {
							flag= true;
							prisonCheckIdx = i;
							break;
						}
					}
					
					if (!flag) {
						dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
			
						state = 0;
						Invoke("WarOverResult", 0.5f);
					} else {
						state = 5;
						dialogCtrl.SetText(ZhongWen.Instance.GetGeneralName(rightGenerals[prisonCheckIdx++]) + ZhongWen.Instance.beifulule + ZhongWen.Instance.tanhao);
					}
				}
			}
			break;
		case 5:
			if (!dialogCtrl.IsShowingText() && Input.GetMouseButtonUp(0)) {
				Input.ResetInputAxes();
				if (warResult == 0) {
					bool flag = false;
					for (int i=prisonCheckIdx; i<leftGenerals.Count; i++) {
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(leftGenerals[i]);
						if (gInfo.prisonerIdx != -1) {
							flag = true;
							prisonCheckIdx = i;
							break;
						}
					}
					
					if (!flag) {
						dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
			
						state = 0;
						Invoke("WarOverResult", 0.5f);
					} else {
						dialogCtrl.SetText(ZhongWen.Instance.womenfulule + ZhongWen.Instance.GetGeneralName(leftGenerals[prisonCheckIdx++]) + ZhongWen.Instance.tanhao);
					}
				} else {
					bool flag = false;
					for (int i=prisonCheckIdx; i<rightGenerals.Count; i++) {
						GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(rightGenerals[i]);
						if (gInfo.prisonerIdx != -1) {
							flag = true;
							prisonCheckIdx = i;
							break;
						}
					}
					
					if (!flag) {
						dialogCtrl.SetDialogueOut(MenuDisplayAnim.AnimType.OutToBottom);
			
						state = 0;
						Invoke("WarOverResult", 0.5f);
					} else {
						dialogCtrl.SetText(ZhongWen.Instance.GetGeneralName(rightGenerals[prisonCheckIdx++]) + ZhongWen.Instance.beifulule + ZhongWen.Instance.tanhao);
					}
				}
			}
			break;
		}
	}
	
	void LateUpdate() {
		
		if (menuSelectIdx != -1) {
			menus[menuSelectIdx].GetComponent<exSpriteFont>().topColor = new Color(1, 0, 0, 1);
			menus[menuSelectIdx].GetComponent<exSpriteFont>().botColor = new Color(1, 0, 0, 1);
		}
	}
	
	void InitScene() {
		
		InitData();
		InitTopBarInformation();
		//InitGeneralInfo();
	}
	
	void InitMenuAction() {
		
		menus[0].SetButtonClickHandler(OnToWar);
		menus[1].SetButtonClickHandler(OnSelectFormation);
		menus[2].SetButtonClickHandler(OnGeneralPosition);
		menus[3].SetButtonClickHandler(OnGeneralInformation);
		menus[4].SetButtonClickHandler(OnEscape);
	}
	
	void InitTopBarInformation() {
		
		int sNum1 = 0;
		for (int i=0; i<leftGenerals.Count; i++) {
			sNum1 += Informations.Instance.GetGeneralInfo(leftGenerals[i]).soldierCur;
			sNum1 += Informations.Instance.GetGeneralInfo(leftGenerals[i]).knightCur;
		}
		
		int sNum2 = 0;
		for (int i=0; i<rightGenerals.Count; i++) {
			sNum2 += Informations.Instance.GetGeneralInfo(rightGenerals[i]).soldierCur;
			sNum2 += Informations.Instance.GetGeneralInfo(rightGenerals[i]).knightCur;
		}
		
		leftBar.SetInformation(leftKing, leftGenerals.Count, sNum1);
		rightBar.SetInformation(rightKing, rightGenerals.Count, sNum2);
	}
	
	void InitGeneralInfo() {
		
		leftGeneralInfo.SetGeneralInformation(leftGenerals[leftSelectIdx], leftDefense, 0);
		rightGeneralInfo.SetGeneralInformation(rightGenerals[rightSelectIdx], rightDefense, gPos[rightSelectIdx]);
	}
	
	void InitData() {
		
		if (isWarBegin) {
			isWarBegin = false;
			
			switch(mode) {
			case 0:
			{
				leftGenerals = new List<int>();
				for (int i=0; i<enemy.generals.Count; i++) {
					int gIdx = enemy.generals[i];
					if (gIdx == enemy.commander) {
						leftGenerals.Insert(0, gIdx);
					} else {
						leftGenerals.Add(gIdx);
					}
				}
				
				rightGenerals = new List<int>();
				for (int i=0; i<mine.generals.Count; i++) {
					int gIdx = mine.generals[i];
					if (gIdx == mine.commander) {
						rightGenerals.Insert(0, gIdx);
					} else {
						rightGenerals.Add(gIdx);
					}
				}
				
				leftKing = enemy.king;
				rightKing = mine.king;
				leftDefense = 0;
				rightDefense = 0;
				mineCommander = mine.commander;
			}
				break;
			case 1:
			{
				CityInfo cInfo = Informations.Instance.GetCityInfo(cityAttacked);
				
				leftGenerals = new List<int>();
				for (int i=0; i<enemy.generals.Count; i++) {
					int gIdx = enemy.generals[i];
					if (gIdx == enemy.commander) {
						leftGenerals.Insert(0, gIdx);
					} else {
						leftGenerals.Add(gIdx);
					}
				}
				
				rightGenerals = new List<int>();
				for (int i=0; i<cInfo.generals.Count; i++) {
					int gIdx = cInfo.generals[i];
					if (gIdx == cInfo.prefect) {
						rightGenerals.Insert(0, gIdx);
					} else {
						rightGenerals.Add(gIdx);
					}
				}
				
				leftKing = enemy.king;
				rightKing = cInfo.king;
				leftDefense = 0;
				rightDefense = cInfo.defense;
				mineCommander = cInfo.prefect;
			}
				break;
			case 2:
			{
				CityInfo cInfo = Informations.Instance.GetCityInfo(cityAttacked);
				
				leftGenerals = new List<int>();
				for (int i=0; i<cInfo.generals.Count; i++) {
					int gIdx = cInfo.generals[i];
					if (gIdx == cInfo.prefect) {
						leftGenerals.Insert(0, gIdx);
					} else {
						leftGenerals.Add(gIdx);
					}
				}
				
				rightGenerals = new List<int>();
				for (int i=0; i<mine.generals.Count; i++) {
					int gIdx = mine.generals[i];
					if (gIdx == mine.commander) {
						rightGenerals.Insert(0, gIdx);
					} else {
						rightGenerals.Add(gIdx);
					}
				}
				
				leftKing = cInfo.king;
				rightKing = mine.king;
				leftDefense = cInfo.defense;
				rightDefense = 0;
				mineCommander = mine.commander;
			}
				break;
			}
			
			leftSelectIdx = 0;
			rightSelectIdx = 0;
			leftFailFlag = new bool[leftGenerals.Count];
			rightFailFlag = new bool[rightGenerals.Count];
			gPos = new int[rightGenerals.Count];
		} else {
			
			GeneralInfo g1 = Informations.Instance.GetGeneralInfo(leftGenerals[leftSelectIdx]);
			GeneralInfo g2 = Informations.Instance.GetGeneralInfo(rightGenerals[rightSelectIdx]);
			
			if (warResult == 0) {
				
				leftFailFlag[leftSelectIdx] = true;
				
				WarResult(g2, g1, leftExperience);
				if (CheckWarOver()) {
					OnWarOver();
				} else {
					SetWarResultDialogue();
				}

				OnSubMenu();
			} else if (warResult == 1) {
				
				rightFailFlag[rightSelectIdx] = true;
				
				WarResult(g1, g2, rightExperience);
				if (CheckWarOver()) {
					OnWarOver();
				}
				 else {
					SetWarResultDialogue();
				}

				OnSubMenu();
			} else if (warResult == 2) {
				/*
				leftFailFlag[leftSelectIdx] = true;
				rightFailFlag[rightSelectIdx] = true;
				
				WarResult(g1, g2, rightExperience);
				WarResult(g2, g1, leftExperience);
				if (CheckWarOver()) {
					OnWarOver();
				}
				*/
			}
		}
		
		if (!isWarOver) {
			do {
				leftSelectIdx = Random.Range(0, leftGenerals.Count);
			} while(leftFailFlag[leftSelectIdx]);
		}
		
		leftGeneralInfo.SetGeneralInformation(leftGenerals[leftSelectIdx], leftDefense, 0);
		OnRightGeneralSelected(rightSelectIdx);
		
		generalListCtrl.SetGeneralsList(leftGenerals, rightGenerals, leftFailFlag, rightFailFlag, leftSelectIdx, rightSelectIdx, isWarOver);
	}
	
	void WarResult(GeneralInfo winner, GeneralInfo loser, int experience) {
		
		//winner.experience += experience;
		winner.experience += (Misc.GetLevelExperience(winner.level + 1) - Misc.GetLevelExperience(winner.level)) / 2;
		CheckLevelUp(winner);
		
		int hourse = 0;
		if (loser.equipment == 27 || loser.equipment == 28
		    || loser.equipment == 29 || loser.equipment == 30) {
			hourse = 1;
		}
		
		if (loser == Informations.Instance.GetGeneralInfo(Informations.Instance.GetKingInfo(loser.king).generalIdx)) {
			if (Informations.Instance.GetKingInfo(loser.king).cities.Count > 0) {
				hourse += 3;
			}
		}
		
		if (!WarSceneController.isEscape && Random.Range(0, 100) > 50 + hourse * 25 - loser.escape * 25) {
			loser.prisonerIdx = winner.king;
			isPrisoned = true;
			loser.escape = 0;
		} else {
			isPrisoned = false;
			loser.escape++;
		}
	}
	
	void CheckLevelUp(GeneralInfo gInfo) {
		
		Misc.CheckIsLevelUp(gInfo);
	}
	
	bool CheckWarOver() {
		
		isWarOver = true;
		bool flag = true;
		for (int i=0; i<leftFailFlag.Length; i++) {
			if (leftFailFlag[i] == false) {
				flag = false;
			}
		}
		
		if (flag) {
			warResult = 0;
			return true;
		}
		
		flag = true;
		for (int i=0; i<rightFailFlag.Length; i++) {
			if (rightFailFlag[i] == false) {
				flag = false;
			}
		}
		
		if (flag) {
			warResult = 1;
			return true;
		}
		
		isWarOver = false;
		
		return false;
	}
	
	void SetWarResultDialogue() {
		
		if (warResult == 0) {
			
			state = 1;
			OnSubMenu();
			
			string msg = "";
			if (isPrisoned) {
				msg = ZhongWen.Instance.wojundasheng + ZhongWen.Instance.GetGeneralName(leftGenerals[leftSelectIdx]) + ZhongWen.Instance.tanhao;
			} else {
				msg = ZhongWen.Instance.wojunshengli;
			}
			dialogCtrl.SetDialogue(mineCommander, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
			
		} else if (warResult == 1) {
			
			state = 1;
			OnSubMenu();
			
			string msg = "";
			if (isPrisoned) {
				msg = ZhongWen.Instance.wozhanbai;
			} else {
				msg = ZhongWen.Instance.henyihan;
			}
			dialogCtrl.SetDialogue(rightGenerals[rightSelectIdx], msg, MenuDisplayAnim.AnimType.InsertFromBottom);
		}
	}
	
	void OnWarOver() {
		
		state = 3;
		string msg = "";
		
		if (warResult == 0) {
			msg = ZhongWen.Instance.wojundahuoquansheng;
		} else {
			msg = ZhongWen.Instance.wojunzhanbai;
		}
		
		dialogCtrl.SetDialogue(mineCommander, msg, MenuDisplayAnim.AnimType.InsertFromBottom);
	}
	
	void WarOverResult() {
		
		Misc.LoadLevel("Strategy");
		
		switch (mode) {
		case 0:
		{
			if (warResult == 0) {
				WarOverResultArmyToArmy(mine, enemy);
			} else if (warResult == 1) {
				WarOverResultArmyToArmy(enemy, mine);
			}
		}
			break;
		case 1:
		{
			if (warResult == 0) {
				WarOverResultArmyToCity(enemy, cityAttacked, false);
			} else if (warResult == 1) {
				WarOverResultArmyToCity(enemy, cityAttacked, true);
			}
		}
			break;
		case 2:
		{
			if (warResult == 0) {
				WarOverResultArmyToCity(mine, cityAttacked, true);
			} else if (warResult == 1) {
				WarOverResultArmyToCity(mine, cityAttacked, false);
			}
		}
			break;
		}
	}
	
	void WarOverResultArmyToArmy(ArmyInfo winner, ArmyInfo loser) {
		
		winner.state = (int)ArmyController.ArmyState.Victory;
		for (int i=0; i<winner.generals.Count; i++) {
			GeneralInfo wgInfo = Informations.Instance.GetGeneralInfo(winner.generals[i]);
			wgInfo.prisonerIdx = -1;
			
			if (wgInfo.level > 4) {
				//wgInfo.experience += 137 * (wgInfo.level - 4) / 2;
				wgInfo.experience += (Misc.GetLevelExperience(wgInfo.level + 1) - Misc.GetLevelExperience(wgInfo.level)) / 2;
			} else {
				wgInfo.experience += 50;
			}
			CheckLevelUp(wgInfo);
		}
		
		int count = loser.generals.Count;
		for (int i=count-1; i>=0; i--) {
			
			int gIdx = loser.generals[i];
			if (Informations.Instance.GetGeneralInfo(gIdx).prisonerIdx == winner.king) {
				winner.prisons.Add(gIdx);
				
				Informations.Instance.GetKingInfo(loser.king).generals.Remove(gIdx);
				loser.generals.RemoveAt(i);
				
				if (gIdx == Informations.Instance.GetKingInfo(loser.king).generalIdx) {
					SetKingOver(loser.king);
				}
			}
		}
		
		if (loser.generals.Count == 0) {
			
			for (int i=0; i<loser.prisons.Count; i++) {
				Informations.Instance.GetGeneralInfo(loser.prisons[i]).prisonerIdx = winner.king;
			}
			
			winner.prisons.AddRange(loser.prisons);
			loser.prisons.Clear();
			
			winner.money += loser.money;
			
			Informations.Instance.armys.Remove(loser);
		} else {
			
			FindArmyCommander(loser);
			
			int tmp = loser.cityTo;
			loser.cityTo = loser.cityFrom;
			loser.cityFrom = tmp;
			
			loser.state = (int)ArmyController.ArmyState.Escape;
		}
	}
	
	void WarOverResultArmyToCity(ArmyInfo armyInfo, int cIdx, bool isWin) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		
		if (isWin) {
		
			for (int i=0; i<armyInfo.generals.Count; i++) {
				
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(armyInfo.generals[i]);
				gInfo.prisonerIdx = -1;
				gInfo.city = cIdx;
				gInfo.escape = 0;
				
				if (gInfo.level > 4) {
					//gInfo.experience += 137 * (gInfo.level - 4) / 2;
					gInfo.experience += (Misc.GetLevelExperience(gInfo.level + 1) - Misc.GetLevelExperience(gInfo.level)) / 2;
				} else {
					gInfo.experience += 50;
				}
				CheckLevelUp(gInfo);
			}
			
			int count = cInfo.generals.Count;
			for (int i=count-1; i>=0; i--) {
				
				if (Informations.Instance.GetGeneralInfo(cInfo.generals[i]).prisonerIdx == armyInfo.king) {
					cInfo.prisons.Add(cInfo.generals[i]);
					Informations.Instance.GetKingInfo(cInfo.king).generals.Remove(cInfo.generals[i]);
					
					if (cInfo.generals[i] == Informations.Instance.GetKingInfo(cInfo.king).generalIdx) {
						SetKingOver(cInfo.king);
					}
					
					cInfo.generals.RemoveAt(i);
				}
			}
			
			count = cInfo.generals.Count;
			if (count > 0) {
				SetEscapeFromCity(cIdx);
			}
			
			//cInfo.generals.Clear();
			cInfo.generals.AddRange(armyInfo.generals);
			cInfo.prefect = armyInfo.commander;
			
			for (int i=0; i<cInfo.prisons.Count; i++) {
				Informations.Instance.GetGeneralInfo(cInfo.prisons[i]).prisonerIdx = armyInfo.king;
			}
			
			for (int i=0; i<armyInfo.prisons.Count; i++) {
				Informations.Instance.GetGeneralInfo(armyInfo.prisons[i]).city = cIdx;
			}
			
			cInfo.prisons.AddRange(armyInfo.prisons);
			cInfo.money += armyInfo.money;
			
			Informations.Instance.armys.Remove(armyInfo);
			
			if (cInfo.king != -1) {
				KingInfo kInfo = Informations.Instance.GetKingInfo(cInfo.king);
				kInfo.cities.Remove(cIdx);
			}
			
			cInfo.king = armyInfo.king;
			Informations.Instance.GetKingInfo(cInfo.king).cities.Add(cIdx);
			
		} else {
			
			for (int i=0; i<cInfo.generals.Count; i++) {
				
				GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(cInfo.generals[i]);
				gInfo.prisonerIdx = -1;
				
				if (gInfo.level > 4) {
					//gInfo.experience += 137 * (gInfo.level - 4) / 2;
					gInfo.experience += (Misc.GetLevelExperience(gInfo.level + 1) - Misc.GetLevelExperience(gInfo.level)) / 2;
				} else {
					gInfo.experience += 50;
				}
				CheckLevelUp(gInfo);
			}
			
			int count = armyInfo.generals.Count;
			for (int i=count-1; i>=0; i--) {
				
				int gIdx = armyInfo.generals[i];
				if (Informations.Instance.GetGeneralInfo(gIdx).prisonerIdx == cInfo.king) {
					
					if (gIdx == Informations.Instance.GetKingInfo(armyInfo.king).generalIdx) {
						SetKingOver(armyInfo.king);
					}
					
					cInfo.prisons.Add(gIdx);
					Informations.Instance.GetGeneralInfo(gIdx).city = cIdx;
					
					Informations.Instance.GetKingInfo(armyInfo.king).generals.Remove(gIdx);
					armyInfo.generals.RemoveAt(i);
				}
			}
			
			if (armyInfo.generals.Count == 0) {
			
				for (int i=0; i<armyInfo.prisons.Count; i++) {
					Informations.Instance.GetGeneralInfo(armyInfo.prisons[i]).prisonerIdx = cInfo.king;
					Informations.Instance.GetGeneralInfo(armyInfo.prisons[i]).city = cIdx;
				}
				
				cInfo.prisons.AddRange(armyInfo.prisons);
				armyInfo.prisons.Clear();
				
				cInfo.money += armyInfo.money;
				Informations.Instance.armys.Remove(armyInfo);
			} else {
				
				FindArmyCommander(armyInfo);

				armyInfo.cityTo = armyInfo.cityFrom;
				armyInfo.cityFrom = cIdx;
				
				armyInfo.state = (int)ArmyController.ArmyState.Escape;
			}
		}
	}
	
	void FindArmyCommander(ArmyInfo armyInfo) {
		
		int strengthMax = 0;
		int commander = -1;
		
		for (int i=0; i<armyInfo.generals.Count; i++) {
			
			if (armyInfo.commander == armyInfo.generals[i] || Informations.Instance.GetKingInfo(armyInfo.king).generalIdx == armyInfo.generals[i]) {
				commander = armyInfo.generals[i];
				break;
			}
			
			int strengthCur = Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).strength;
			if (strengthCur > strengthMax) {
				commander = armyInfo.generals[i]; 
				strengthMax = strengthCur;
			}
		}
		
		armyInfo.commander = commander;
	}
	
	void SetEscapeFromCity(int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		
		do {
			ArmyInfo ai 		= new ArmyInfo();
			ai.king 			= cInfo.king;
			ai.money			= 0;
			
			Informations.Instance.armys.Add(ai);
			
			int count = cInfo.generals.Count;
			int min = count - 5;
			min = Mathf.Clamp(min, 0, count);
			for (int i=count-1; i>=min; i--) {
				int g = cInfo.generals[i];
				ai.generals.Add(g);
				cInfo.generals.RemoveAt(i);
				
				Informations.Instance.GetGeneralInfo(g).city = -1;
			}
			
			ai.commander = -1;
			FindArmyCommander(ai);
			
			int cityEscTo = -1;
			List<int> clist = MyPathfinding.GetCityNearbyIdx(cIdx);
			List<int> canGoList = new List<int>();
			
			if (clist.Count == 1) {
				cityEscTo = clist[0];
			} else {
				
				for (int i=0; i<clist.Count; i++) {
					if (Informations.Instance.GetCityInfo(clist[i]).king == ai.king) {
						canGoList.Add(clist[i]);
					}
				}
				
				if (canGoList.Count > 0) {
					cityEscTo = canGoList[Random.Range(0, canGoList.Count)];
				}
				
				if (cityEscTo == -1) {
					for (int i=0; i<clist.Count; i++) {
						if (Informations.Instance.GetCityInfo(clist[i]).king == -1) {
							canGoList.Add(clist[i]);
						}
					}
					
					if (canGoList.Count > 0) {
						cityEscTo = canGoList[Random.Range(0, canGoList.Count)];
					}
				}
				
				if (cityEscTo == -1) {
					cityEscTo = clist[Random.Range(0, clist.Count)];
				}
			}
			
			ai.cityFrom = cityAttacked;
			ai.cityTo 	= cityEscTo;
			
			ai.state = (int)ArmyController.ArmyState.Escape;
			ai.pos = Vector3.zero;
		} while(cInfo.generals.Count > 0);
	}
	
	void SetKingOver(int kIdx) {
		
		for (int g=0; g<Informations.Instance.generalNum; g++) {
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(g);
			
			if (gInfo.king == kIdx) {

				gInfo.king = Informations.Instance.kingNum;
			}
		}
		
		for (int c=0; c<Informations.Instance.cityNum; c++) {
			CityInfo cInfo = Informations.Instance.GetCityInfo(c);
			
			if (cInfo.king == kIdx) {
				cInfo.king = Informations.Instance.kingNum;
				for (int i=0; i<cInfo.prisons.Count; i++) {
					Informations.Instance.GetGeneralInfo(cInfo.prisons[i]).prisonerIdx = Informations.Instance.kingNum;
				}
			}
		}
		
		for (int i=0; i<Informations.Instance.armys.Count; i++) {
			ArmyInfo ai = Informations.Instance.armys[i];
			
			if (ai.king == kIdx) {
				ai.king = Informations.Instance.kingNum;
			}
			for (int j=0; j<ai.prisons.Count; j++) {
				Informations.Instance.GetGeneralInfo(ai.prisons[j]).prisonerIdx = Informations.Instance.kingNum;
			}
		}
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(kIdx);
		
		kInfo.generals.Clear();
		kInfo.cities.Clear();
		kInfo.active = 0;
	}
	
	void OnToWar() {
		
		menuSelectIdx = 0;
		OnSubMenu();
		
		WarSceneController.leftGeneralIdx = leftGenerals[leftSelectIdx];
		WarSceneController.rightGeneralIdx = rightGenerals[rightSelectIdx];
		WarSceneController.leftDefense = leftDefense;
		WarSceneController.rightDefense = rightDefense;
		WarSceneController.rightGeneralPosition = gPos[rightSelectIdx];
		
		GeneralInfo left = Informations.Instance.GetGeneralInfo(leftGenerals[leftSelectIdx]);
		GeneralInfo right = Informations.Instance.GetGeneralInfo(rightGenerals[rightSelectIdx]);
		
		leftExperience = left.level * 50 + left.knightCur * 6 + left.soldierCur * 3 + left.strength * 2 + left.healthCur + left.manaCur / 2 + leftDefense / 10 + Random.Range(0, 100);
		rightExperience = right.level * 50 + right.knightCur * 6 + right.soldierCur * 3 + right.strength * 2 + right.healthCur + right.manaCur / 2 + rightDefense / 10 + Random.Range(0, 100);
		
		Misc.LoadLevel("WarScene");
	}
	
	void OnSelectFormation() {
		
		menuSelectIdx = 1;
		OnSubMenu();
		selectFormation.SetGeneral(rightGenerals[rightSelectIdx]);
	}
	
	void OnGeneralPosition() {
		
		menuSelectIdx = 2;
		OnSubMenu();
		generalPos.SetActive(true);
	}
	
	void OnGeneralInformation() {
		
		menuSelectIdx = 3;
		OnSubMenu();
		
		generalsInfo.AddGeneralsList(rightGenerals);
		generalsInfo.AddGeneralsList(leftGenerals);
	}
	
	void OnEscape() {
		
		menuSelectIdx = 4;
		OnSubMenu();
		retreatConfirm.SetActive(true);
	}
	
	void OnSubMenu() {
		
		foreach (Button m in menus) {
			m.enabled = false;
		}
		
		generalListCtrl.SetClickEable(false);
	}
	
	public void OnReturnMain() {
		
		state = 0;
		
		int i = 0;
		if (rightFailFlag[rightSelectIdx]) {
			i = 3;
		}
		
		for (; i<menus.Length; i++) {
			menus[i].enabled = true;
		}
		
		if (menuSelectIdx != -1) {
			
			menus[menuSelectIdx].GetComponent<exSpriteFont>().topColor = new Color(1, 1, 1, 1);
			menus[menuSelectIdx].GetComponent<exSpriteFont>().botColor = new Color(1, 1, 1, 1);
			
			menuSelectIdx = -1;
		}
		
		generalListCtrl.SetClickEable(true);
	}
	
	public void UpdateGeneralInfo() {
		
		rightGeneralInfo.SetGeneralInformation(rightGenerals[rightSelectIdx], rightDefense, gPos[rightSelectIdx]);
	}
	
	public void OnSelectGeneralPosition(int pos) {
		
		gPos[rightSelectIdx] = pos;
		UpdateGeneralInfo();
	}
	
	public void OnRetreat() {
		
		warResult = 1;
		state = 3;
		dialogCtrl.SetDialogue(mineCommander, ZhongWen.Instance.qingkuangbumiao, MenuDisplayAnim.AnimType.InsertFromBottom);
	}
	
	public void OnRightGeneralSelected(int idx) {
		
		rightSelectIdx = idx;
		rightGeneralInfo.SetGeneralInformation(rightGenerals[rightSelectIdx], rightDefense, gPos[rightSelectIdx]);
		
		if (rightFailFlag[rightSelectIdx]) {
			
			for (int i=0; i<3; i++) {
				menus[i].SetButtonEnable(false);
			}
		} else {
			for (int i=0; i<3; i++) {
				menus[i].SetButtonEnable(true);
			}
			
			if (state != 0) {
				for (int i=0; i<3; i++) {
					menus[i].enabled = false;
				}
			}
		}
	}
}
