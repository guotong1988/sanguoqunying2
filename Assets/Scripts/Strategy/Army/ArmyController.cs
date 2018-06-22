using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyController : MonoBehaviour {
	
	public enum ArmyState {
		Running,
		Escape,
		Victory,
		Garrison
	}
	
	private ArmyState state = ArmyState.Running;
	private int king;
	private string animName = "";
	
	public GameObject armyPrefab;
	
	private StrategyController strCtrl;
	private Transform flagCtrl;
	private MyPathfinding pathfinding;
	private FlagsController cityFlagsCtrl;
	
	private List<Vector3> route;
	public ArmyInfo armyInfo;
	
	public bool isHFlipped = false;
	private float speed = 10;
	private float timeTick = 0;
	
	private string[] animNames  = new string[]{
			"ArmyRun", "ArmyRunUp", "ArmyRunDown", 
			"ArymEscape", "ArymEscapeUp", "ArymEscapeDown", 
			"ArymVictory", "ArymVictoryUp", "ArymVictoryDown"};
	
	private Vector3[] flagPos = new Vector3[]{
		
		new Vector3(7, 24, 0), 													// Run
		new Vector3(6, 24, 0), 													// RunUp
		new Vector3(0, 24, 0), 													// RunDown
		new Vector3(7, 24, 0),													// Garrison
		new Vector3(8, 24, 0),													// GarrisonUp
		new Vector3(3, 24, 0),													// GarrisonDown
		new Vector3(7, 24, 0),													// Escape
		new Vector3(11, 24, 0), 												// EscapeUp
		new Vector3(0, 24, 0),													// EscapeDown
		new Vector3(5, 30, 0),	new Vector3(2, 31, 0), new Vector3(5, 31, 0),	// Victory
		new Vector3(5, 32, 0), new Vector3(6, 31, 0), new Vector3(5, 31, 0),	// VictoryUp
		new Vector3(2, 30, 0), new Vector3(0, 31, 0), new Vector3(2, 31, 0),	// VictoryDown
	};	
	
	// Use this for initialization
	void Start () {
		
		flagCtrl = transform.GetChild(0);
		strCtrl = GameObject.FindWithTag("StrategyController").GetComponent<StrategyController>();
		pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<MyPathfinding>();
		cityFlagsCtrl = GameObject.FindWithTag("CityFlags").GetComponent<FlagsController>();
	}
	
	void OnDestroy() {
		
		armyInfo.state = (int)state;
		armyInfo.pos = transform.position;
		
		armyInfo.armyCtrl = null;
		armyInfo.isFlipped = isHFlipped;
		armyInfo.timeTick = timeTick;
		
		if (state == ArmyState.Garrison) {
			
			armyInfo.direction = GetComponent<exSpriteAnimation>().GetCurFrameIndex();
		} else {
			if (animName == animNames[0] || animName == animNames[3] || animName == animNames[6]) {
				
				armyInfo.direction = 0;
			} else if (animName == animNames[1] || animName == animNames[4] || animName == animNames[7]) {
				
				armyInfo.direction = 1;
			} else if (animName == animNames[2] || animName == animNames[5] || animName == animNames[8]) {
				
				armyInfo.direction = 2;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		ArmyMoveController();
	}
	
	void ArmyMoveController() {
		
		if (state != ArmyState.Garrison && state != ArmyState.Victory 
			&& route != null && route.Count > 0 ) {
			
			Vector3 target = route[0];
			target.z = transform.position.z;
			
			if (state != ArmyState.Escape) {
				transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
			} else {
				transform.position = Vector3.MoveTowards(transform.position, target, speed * 2 * Time.deltaTime);
			}
			
			Vector3 pos = transform.position;
			pos.z = (pos.y - 400f) / 800f;
			transform.position = pos;
			
			if (Vector3.Distance(transform.position, target) < 1) {
				
				route.RemoveAt(0);
				
				CalcDirection();
				TestReachCity();
			}
		}
		
		if (state != ArmyState.Escape) {
			if (TestEncounterArmy()) {
				return;
			}
		}
		
		if (state == ArmyState.Victory || state == ArmyState.Escape) {
			
			timeTick += Time.deltaTime;
			if (timeTick > 3.0f) {
				timeTick = 0;
				SetArmyRunning();
			}
		}
		
		CalcVictoryFlagPos();
	}
	
	bool TestEncounterArmy() {
		
		for (int i=0; i<Informations.Instance.armys.Count; i++) {
			
			ArmyInfo ai = Informations.Instance.armys[i];
			if (armyInfo == ai || king == ai.king || ai.armyCtrl.GetState() == ArmyState.Escape) continue;
			
			if (Vector3.Distance(transform.position, ai.armyCtrl.transform.position) < 10) {
				
				if (king == Controller.kingIndex) {
					strCtrl.SetWarDialogue(armyInfo, ai);
				} else if (ai.king == Controller.kingIndex) {
					strCtrl.SetWarDialogue(ai, armyInfo);
				} else {
					AttackArmy(ai);
				}

				return true;
			}
		}
		return false;
	}
	
	void AttackArmy(ArmyInfo enemy) {
		
		int result = 0;
		int selfIdx = 0;
		int enemyIdx = 0;
		
		while (result == 0) {
			
			GeneralInfo selfGeneral = Informations.Instance.GetGeneralInfo(armyInfo.generals[selfIdx]);
			GeneralInfo enemyGeneral = Informations.Instance.GetGeneralInfo(enemy.generals[enemyIdx]);
			
			int power1 = selfGeneral.level * 5 + selfGeneral.knightCur * 6 + selfGeneral.soldierCur * 3 + selfGeneral.strength * 2 + selfGeneral.healthCur + selfGeneral.manaCur / 2 + Random.Range(0, 20);
			int power2 = enemyGeneral.level * 5 + enemyGeneral.knightCur * 6 + enemyGeneral.soldierCur * 3 + enemyGeneral.strength * 2 + enemyGeneral.healthCur + enemyGeneral.manaCur / 2 + Random.Range(0, 20);
			
			if (power1 > power2) {
				
				WarResult(selfGeneral, enemyGeneral, power2);
				enemyIdx++;
			} else {
				
				WarResult(enemyGeneral, selfGeneral, power1);
				selfIdx++;
			}
			
			if (selfIdx >= armyInfo.generals.Count) {
				result = 2;
			} else if (enemyIdx >= enemy.generals.Count) {
				result = 1;
			}
		}
		
		if (result == 1) {
			
			WarResult(armyInfo, enemy);
		} else if (result == 2) {
			
			WarResult(enemy, armyInfo);
		}
	}
	
	public void FindArmyCommander() {
		
		int strengthMax = 0;
		int commander = -1;
		
		for (int i=0; i<armyInfo.generals.Count; i++) {
			
			KingInfo kInfo = Informations.Instance.GetKingInfo(armyInfo.king);
			if (armyInfo.commander == armyInfo.generals[i] || kInfo.generalIdx == armyInfo.generals[i]) {
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
				cityFlagsCtrl.SetFlag(c);
			}
		}
		
		for (int i=0; i<Informations.Instance.armys.Count; i++) {
			ArmyInfo ai = Informations.Instance.armys[i];
			
			if (ai.king == kIdx) {
				ai.king = Informations.Instance.kingNum;
				ai.armyCtrl.SetArmyKingFlag();
			}
		}
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(kIdx);
		
		kInfo.generals.Clear();
		kInfo.cities.Clear();
		kInfo.active = 0;
	}
	
	bool CheckIsLevelUp(GeneralInfo wgInfo) {
		
		bool ret = false;
		
		if (wgInfo.level > 4) {
			//wgInfo.experience += 137 * (wgInfo.level - 4) / 2;
			wgInfo.experience += (Misc.GetLevelExperience(wgInfo.level + 1) - Misc.GetLevelExperience(wgInfo.level)) / 4;
		} else {
			wgInfo.experience += 50;
		}
		ret = Misc.CheckIsLevelUp(wgInfo);
		
		return ret;
	}
	
	void WarResult(ArmyInfo winner, ArmyInfo loser) {
		
		winner.armyCtrl.SetArmyVictory();
		
		for (int i=0; i<winner.generals.Count; i++) {
			
			GeneralInfo wgInfo = Informations.Instance.GetGeneralInfo(winner.generals[i]);
			wgInfo.prisonerIdx = -1;
			CheckIsLevelUp(wgInfo);
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
			
			Destroy(loser.armyCtrl.gameObject);
			Informations.Instance.armys.Remove(loser);
		} else {
			
			loser.armyCtrl.FindArmyCommander();
			
			int tmp = loser.cityTo;
			loser.cityTo = loser.cityFrom;
			loser.cityFrom = tmp;
			
			if (pathfinding == null)
				pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<MyPathfinding>();
			
			loser.armyCtrl.SetRoute(pathfinding.GetRoute(loser.armyCtrl.transform.position, loser.cityTo));
			loser.armyCtrl.SetArmyEscape();
		}
	}
	
	void WarResult(GeneralInfo winner, GeneralInfo loser, int damager) {
		
		loser.knightCur = 0;
		loser.soldierCur = 0;
		loser.healthCur = 2;
		loser.manaCur = 2;
		
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
		
		if (Random.Range(0, 100) > 50 + hourse * 25 - loser.escape * 25) {
			loser.prisonerIdx = winner.king;
			loser.escape = 0;
		} else {
			loser.escape++;
		}
		/*
		int lv = loser.level;
		int experience = 0;
		if (lv == 2) {
			experience = damager / 2;
		} else if (lv == 3) {
			experience = damager;
		} else if (lv < 50) {
			lv -= 4;
			experience = damager + (damager / 4) * (lv + 1) * lv / 2;
		}
		
		winner.experience += experience;
		*/
		winner.experience += (Misc.GetLevelExperience(winner.level + 1) - Misc.GetLevelExperience(winner.level)) / 2;
		Misc.CheckIsLevelUp(winner);
		
		damager -= winner.strength;
		
		if (damager > winner.knightCur * 6) {
			
			damager -= winner.knightCur * 6;
			winner.knightCur = 0;
		} else {
			
			winner.knightCur -= damager / 6;
			damager = 0;
			return;
		}
		
		if (damager > winner.soldierCur * 3) {
			
			damager -= winner.soldierCur * 3;
			winner.soldierCur = 0;
		} else {
			
			winner.soldierCur -= damager / 3;
			damager = 0;
			return;
		}
		
		if (damager > winner.manaCur / 2) {
			
			damager -= winner.manaCur / 2;
			winner.manaCur = 0;
		} else {
			
			winner.manaCur -= damager * 2;
			damager = 0;
			return;
		}
		
		if (winner.healthCur - damager < 15) {
			
			winner.healthCur = 15;
		} else {
			
			winner.healthCur = winner.healthCur - damager;
		}
	}
	
	public void IntoTheCity(int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		
		int count = 10 - cInfo.generals.Count;
		count = Mathf.Clamp(count, count, armyInfo.generals.Count);
		for (int i=count-1; i>=0; i--) {
			
			if (armyInfo.generals[i] == Informations.Instance.GetKingInfo(armyInfo.king).generalIdx) {
				cInfo.prefect = armyInfo.generals[i];
			}
			Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).city = cIdx;
			Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).escape = 0;
			cInfo.generals.Add(armyInfo.generals[i]);
			armyInfo.generals.RemoveAt(i);
		}
		
		if (armyInfo.generals.Count == 0) {
			for (int i=0; i<armyInfo.prisons.Count; i++) {
				Informations.Instance.GetGeneralInfo(armyInfo.prisons[i]).city = cIdx;
			}
			cInfo.prisons.AddRange(armyInfo.prisons);
			cInfo.money += armyInfo.money;
			
			Informations.Instance.armys.Remove(armyInfo);
			Destroy(armyInfo.armyCtrl.gameObject);
		} else {
			
			FindArmyCommander();
			SetArmyGarrison();
		}
	}
	
	void TestReachCity() {
		
		int cIdx = pathfinding.GetCityIndex(transform.position, 5);
		
		if (cIdx == -1) return;
		
		int kIdx = Informations.Instance.GetCityInfo(cIdx).king;
		if (kIdx != -1 && kIdx != king && state != ArmyState.Escape) {
			AttackCity(cIdx);
			return;
		}
		
		if (route.Count == 0) {
			
			if (Informations.Instance.GetCityInfo(cIdx).king != king) {
				AttackCity(cIdx);
				return;
			}
			
			IntoTheCity(cIdx);
		}
	}
	
	void AttackCity(int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		
		int result = 0;
		int selfIdx = 0;
		int enemyIdx = 0;
		
		if (cInfo.king != -1) {
			if (cInfo.king == Controller.kingIndex) {
				strCtrl.SetWarDialogue(cIdx, armyInfo);
				return;
			} else if (armyInfo.king == Controller.kingIndex) {
				strCtrl.SetWarDialogue(armyInfo, cIdx);
				return;
			}
		} else {
			result = 1;
		}
		
		while (result == 0) {
			
			GeneralInfo selfGeneral = Informations.Instance.GetGeneralInfo(armyInfo.generals[selfIdx]);
			GeneralInfo enemyGeneral = Informations.Instance.GetGeneralInfo(cInfo.generals[enemyIdx]);
			
			int power1 = selfGeneral.level * 5 + selfGeneral.knightCur * 6 + selfGeneral.soldierCur * 3 + selfGeneral.strength * 2 + selfGeneral.healthCur + selfGeneral.manaCur / 2 + Random.Range(0, 20);
			int power2 = enemyGeneral.level * 5 + enemyGeneral.knightCur * 6 + enemyGeneral.soldierCur * 3 + enemyGeneral.strength * 2 + enemyGeneral.healthCur + enemyGeneral.manaCur / 2 + cInfo.defense / 10 + Random.Range(0, 20);
			
			if (power1 > power2) {
				
				WarResult(selfGeneral, enemyGeneral, power2);
				enemyIdx++;
			} else {
				
				WarResult(enemyGeneral, selfGeneral, power1);
				selfIdx++;
			}
			
			if (selfIdx >= armyInfo.generals.Count) {
				result = 2;
			} else if (enemyIdx >= cInfo.generals.Count) {
				result = 1;
			}
		}
		
		if (result == 1) {
		
			for (int i=0; i<armyInfo.generals.Count; i++) {
				
				Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).prisonerIdx = -1;
				Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).city = cIdx;
				Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).escape = 0;
				CheckIsLevelUp(Informations.Instance.GetGeneralInfo(armyInfo.generals[i]));
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
			Destroy(armyInfo.armyCtrl.gameObject);
			
			if (cInfo.king != -1) {
				KingInfo kInfo = Informations.Instance.GetKingInfo(cInfo.king);
				kInfo.cities.Remove(cIdx);
			}
			
			cInfo.king = armyInfo.king;
			cityFlagsCtrl.SetFlag(cIdx);
			Informations.Instance.GetKingInfo(armyInfo.king).cities.Add(cIdx);
			
		} else if (result == 2) {
			
			for (int i=0; i<cInfo.generals.Count; i++) {
				Informations.Instance.GetGeneralInfo(cInfo.generals[i]).prisonerIdx = -1;
				CheckIsLevelUp(Informations.Instance.GetGeneralInfo(cInfo.generals[i]));
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
				
				state = ArmyState.Escape;
				Destroy(armyInfo.armyCtrl.gameObject);
				Informations.Instance.armys.Remove(armyInfo);
			} else {
				
				FindArmyCommander();
				
				int tmp = armyInfo.cityTo;
				armyInfo.cityTo = armyInfo.cityFrom;
				armyInfo.cityFrom = tmp;
				
				if (pathfinding == null)
					pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<MyPathfinding>();
				
				SetRoute(pathfinding.GetRoute(transform.position, armyInfo.cityTo));
				
				armyInfo.armyCtrl.SetArmyEscape();
			}
		}
		
	}
	
	void SetEscapeFromCity(int cIdx) {
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
		
		do {
		
			GameObject go 		= (GameObject)Instantiate(armyPrefab, pathfinding.GetCityPos(cIdx), transform.rotation);
			ArmyController ac	= go.GetComponent<ArmyController>();
			ArmyInfo ai 		= new ArmyInfo();
			ai.king 			= cInfo.king;
			ai.money			= 0;
			ai.armyCtrl 		= ac;
			
			ac.armyInfo = ai;
			ac.SetArmyKingFlag();
			
			Informations.Instance.armys.Add(ai);
			ai.armyCtrl.SetArmyKingFlag();
			
			go.transform.parent = GameObject.Find("ArmiesRoot").transform;
			
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
			ai.armyCtrl.FindArmyCommander();
			
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
			
			ai.cityFrom = cIdx;
			ai.cityTo 	= cityEscTo;
			
			if (pathfinding == null)
				pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<MyPathfinding>();
			
			ai.armyCtrl.SetRoute(pathfinding.GetRoute(ai.armyCtrl.transform.position, ai.cityTo));
			ai.armyCtrl.SetArmyEscape();
		} while (cInfo.generals.Count > 0);
	}
	
	public void SetRoute(List<Vector3> r) {
		
		route = r;
	}
	
	public void SetArmyKingFlag() {
		
		king = armyInfo.king;
		
		if (flagCtrl == null) {
			flagCtrl = transform.GetChild(0);
		}
		
		//flagCtrl.GetComponent<exSpriteAnimation>().Play("Flag" + (king+1));
        string kingName = ZhongWen.Instance.GetKingName(king);
        for (int i = 0; i < ZhongWen.Instance.kingNames.Length; i++)
        {
            if (ZhongWen.Instance.kingNames[i] == kingName)
            {
                string animName = "Flag" + (i + 1);
                flagCtrl.GetComponent<exSpriteAnimation>().Play(animName);
                break;
            }
        }
	}
	
	public void Pause() {
		
		enabled = false;
		
		if (state != ArmyState.Garrison) {
			GetComponent<exSpriteAnimation>().Pause();
		}
		
		flagCtrl.GetComponent<exSpriteAnimation>().Pause();
	}
	
	public void Resume() {
		
		enabled = true;
		
		if (state != ArmyState.Garrison) {
			GetComponent<exSpriteAnimation>().Resume();
		}
		
		flagCtrl.GetComponent<exSpriteAnimation>().Resume();
	}
	
	public bool GetTouchedFlag() {
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		return flagCtrl.GetComponent<Collider>().Raycast (ray, out hit, 1000.0f);
	}
	
	public ArmyState GetState() {
		
		return state;
	}
	
	public void SetArmyRunning() {
		
		state = ArmyState.Running;
		
		CalcDirection();
	}
	
	public void SetArmyEscape() {
		
		state = ArmyState.Escape;
		timeTick = 0;
		
		CalcDirection();
	}
	
	public void SetArmyVictory() {
		
		state = ArmyState.Victory;
		timeTick = 0;
		
		CalcDirection();
	}
	
	public void SetArmyGarrison() {
		
		state = ArmyState.Garrison;
		
		if (animName != "") {
			
			exSpriteAnimation moveAnim 	= GetComponent<exSpriteAnimation>();
			moveAnim.Stop();
			
			int frame = -1;
			
			if (animName == animNames[0] || animName == animNames[3] || animName == animNames[6]) {
				
				frame = 0;
			} else if (animName == animNames[1] || animName == animNames[4] || animName == animNames[7]) {
				
				frame = 1;
			} else if (animName == animNames[2] || animName == animNames[5] || animName == animNames[8]) {
				
				frame = 2;
			}
			
			moveAnim.SetFrame("ArmyGarrison", frame);
			
			Vector3 pos = flagPos[3 + frame];
			
			if (!isHFlipped) {
				pos.x = -pos.x;
			}
			flagCtrl.localPosition = pos;
			
			animName = "";
		}
	}
	
	public void InitArmyInfo(ArmyInfo ai) {
		
		armyInfo = ai;
		armyInfo.armyCtrl = GetComponent<ArmyController>();
		
		SetArmyKingFlag();
		
		if (armyInfo.cityTo != -1) {
			
			if (pathfinding == null)
				pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<MyPathfinding>();
			SetRoute(pathfinding.GetRoute(armyInfo.pos, armyInfo.cityTo));
		}
		
		state = (ArmyState)armyInfo.state;
		timeTick = armyInfo.timeTick;
		
		if (state == ArmyState.Garrison) {
			
			exSprite moveSprite 		= GetComponent<exSprite>();
			exSpriteAnimation moveAnim 	= GetComponent<exSpriteAnimation>();
			moveAnim.Stop();
			
			int frame = armyInfo.direction;
			if (frame == -1)
				frame = 0;
			moveAnim.SetFrame("ArmyGarrison", frame);
			
			Vector3 pos = flagPos[3 + frame];
			if (!armyInfo.isFlipped) {
				pos.x = -pos.x;
			} else {
				isHFlipped = true;
				moveSprite.HFlip();
				
				exSprite flagSprite = flagCtrl.GetComponent<exSprite>();
				BoxCollider flagCol	= flagCtrl.GetComponent<BoxCollider>();
				flagSprite.HFlip();
				flagCol.center = new Vector3(-flagCol.center.x, flagCol.center.y, flagCol.center.z);
			}
			
			flagCtrl.localPosition = pos;
		} else {
			
			CalcDirection();
		}
	}
	
	void CalcDirection() {
		
		if (route == null || route.Count == 0) return;
		
		Vector3 target = route[0];
		Vector3 targetDir = target - transform.position;
		Vector3 forward = Vector3.right;
		float angle = Vector3.Angle(targetDir, forward);
		int flag = 0;
		
		if (angle <= 30) {
			flag = 0;
		} else if (angle > 150) {
			flag = 1;
		} else if (angle > 30 && angle <= 90) {
			if (target.y >= transform.position.y)
				flag = 2;
			else 
				flag = 4;
		} else if (angle > 90 && angle <= 150) {
			if (target.y > transform.position.y)
				flag = 3;
			else 
				flag = 5;
		}
		
		exSprite moveSprite 		= GetComponent<exSprite>();
		exSpriteAnimation moveAnim 	= GetComponent<exSpriteAnimation>();
		exSprite flagSprite 		= flagCtrl.GetComponent<exSprite>();
		BoxCollider flagCol			= flagCtrl.GetComponent<BoxCollider>();
		
		if (flag % 2 == 0) {
			
			if (!isHFlipped) {
				isHFlipped = true;
				moveSprite.HFlip();
				flagSprite.HFlip();
				flagCol.center = new Vector3(-flagCol.center.x, flagCol.center.y, flagCol.center.z);
			}
		} else {
			if (isHFlipped) {
				isHFlipped = false;
				moveSprite.HFlip();
				flagSprite.HFlip();
				flagCol.center = new Vector3(-flagCol.center.x, flagCol.center.y, flagCol.center.z);
			}
		}
		
		Vector3 pos = Vector3.zero;
		flag /= 2;
		
		switch (state) {
		case ArmyState.Running:
			animName = animNames[flag];
			pos = flagPos[flag];
			break;
		case ArmyState.Escape:
			animName = animNames[flag + 3];
			pos = flagPos[flag + 6];
			break;
		case ArmyState.Victory:
			animName = animNames[flag + 6];
			pos = flagPos[flag * 3 + 9];
			break;
		}
		
		if (!moveAnim.IsPlaying(animName)) {
			moveAnim.Play(animName);
		}
		
		if (!isHFlipped) {
			pos.x = -pos.x;
		}
		flagCtrl.localPosition = pos;
	}
	
	void CalcVictoryFlagPos() {
		
		if (state != ArmyState.Victory) return;
		
		int flag = 0;
		exSpriteAnimation moveAnim 	= GetComponent<exSpriteAnimation>();
		
		int frame = moveAnim.GetCurFrameIndex();
		
		if (animName == animNames[6]) {
			flag = 0;
		} else if (animName == animNames[7]) {
			flag = 1;
		} else if (animName == animNames[8]) {
			flag = 2;
		}
		
		Vector3 pos = flagPos[flag * 3 + 9 + frame];
		
		if (!isHFlipped) {
			pos.x = -pos.x;
		}
		flagCtrl.localPosition = pos;
	}
}
