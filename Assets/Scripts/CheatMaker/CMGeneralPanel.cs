using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMGeneralPanel : MonoBehaviour {

	public TweenPosition tweenPos;

	public GameObject generalRoot;
	public GameObject kingRoot;
	public GameObject cityRoot;
	public GameObject weaponRoot;
	public GameObject magicRoot;

	public UISlider sliderLevel;
	public UISlider sliderStrength;
	public UISlider sliderIntellect;
	public UISlider sliderHealthy;
	public UISlider sliderMana;
	public UISlider sliderSoldier;

	public UILabel[] labelMagic;

	private int generalIndex;
	private GeneralInfo generalInfo;

	private CMPopupList kingPopupList;
	private CMPopupList cityPopupList;
	private CMPopupList weaponPopupList;
	private CMPopupList magicPopupList;
	private List<int> kingIdxList = new List<int>();

	private Vector3 posPopupListGeneral = new Vector3(85, 0, 0);
	private Vector3 posPopupListKing = new Vector3(85, 0, 0);
	private Vector3 posPopupListCity = new Vector3(85, 0, 0);
	private Vector3 posPopupListWeapon = new Vector3(85, 0, 0);
	private Vector3 posPopupListMagic = new Vector3(85, 0, 0);

	// Use this for initialization
	void Start () {
	
		InitGeneralPopupList();
		InitKingPopupList();
		InitCityPopupList();
		InitWeaponPopupList();
		InitMagicPopupList();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitGeneralPopupList() {

		List<string> generals = new List<string>();
		for (int i=0; i<Informations.Instance.generalNum; i++) {
			generals.Add(ZhongWen.Instance.GetGeneralName1(i));
		}

		GameObject go = CheatMakerController.Instance.GetPopupList(generals, OnGeneralSelectChange);
		go.transform.parent = generalRoot.transform;
		go.transform.localPosition = posPopupListGeneral;
		go.transform.localScale = Vector3.one;
	}

	void InitKingPopupList() {

		List<string> kings = new List<string>();
		for (int i=0; i<Informations.Instance.kingNum; i++) {
			if (Informations.Instance.GetKingInfo(i).active == 1) {
				string str = ZhongWen.Instance.GetKingName(i);
				if (str.Length == 2) {
					str = str[0] + "  " + str[1];
				}
				kings.Add(str);
				kingIdxList.Add(i);
			}
		}
		
		GameObject go = CheatMakerController.Instance.GetPopupList(kings, OnKingSelectChange);
		go.transform.parent = kingRoot.transform;
		go.transform.localPosition = posPopupListKing;
		go.transform.localScale = Vector3.one;
		
		kingPopupList = go.GetComponent<CMPopupList>();
	}

	void InitCityPopupList() {

		List<string> cities = new List<string>();
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			string str = ZhongWen.Instance.GetCityName(i);
			cities.Add(str);
		}
		
		GameObject go = CheatMakerController.Instance.GetPopupList(cities, OnCitySelectChange);
		go.transform.parent = cityRoot.transform;
		go.transform.localPosition = posPopupListCity;
		go.transform.localScale = Vector3.one;

		cityPopupList = go.GetComponent<CMPopupList>();
	}

	void InitWeaponPopupList() {

		List<string> weapons = new List<string>();
		for (int i=0; i<Informations.Instance.equipmentNum; i++) {
			weapons.Add(ZhongWen.Instance.GetEquipmentName(i));
		}

		GameObject go = CheatMakerController.Instance.GetPopupList(weapons, OnWeaponSelectChange);
		go.transform.parent = weaponRoot.transform;
		go.transform.localPosition = posPopupListWeapon;
		go.transform.localScale = Vector3.one;
		
		weaponPopupList = go.GetComponent<CMPopupList>();
	}

	void InitMagicPopupList() {

		List<string> magics = new List<string>();
		for (int i=0; i<Informations.Instance.magicNum; i++) {
			magics.Add(ZhongWen.Instance.GetMagicName(i));
		}

		GameObject go = CheatMakerController.Instance.GetPopupList(magics, OnMagicSelectChange);
		go.transform.parent = magicRoot.transform;
		go.transform.localPosition = posPopupListMagic;
		go.transform.localScale = Vector3.one;
		
		magicPopupList = go.GetComponent<CMPopupList>();
	}

	bool OnGeneralSelectChange(int index) {

		generalIndex = index;
		generalInfo = Informations.Instance.GetGeneralInfo(index);

		kingPopupList.SetItemSelect(generalInfo.king);
		cityPopupList.SetItemSelect(generalInfo.city);
		weaponPopupList.SetItemSelect(generalInfo.equipment);
		magicPopupList.SetItemSelect(-1);

		sliderLevel.value = generalInfo.level / 50f;
		sliderStrength.value = generalInfo.strength / 120f;
		sliderIntellect.value = generalInfo.intellect / 120f;
		sliderHealthy.value = generalInfo.healthCur / 200f;
		sliderMana.value = generalInfo.manaCur / 200f;
		sliderSoldier.value = (generalInfo.soldierCur + generalInfo.knightCur) / 100f;

		for (int i=0; i<4; i++) {
			if (generalInfo.magic[i] == -1) {
				labelMagic[i].text = "";
			} else {
				labelMagic[i].text = ZhongWen.Instance.GetMagicName(generalInfo.magic[i]);
			}
		}

		return true;
	}

	bool ReturnSelectGeneralFlag() {
		if (generalInfo == null) {
			CheatMakerController.Instance.GetTips(ZhongWen.Instance.cheatSelectGeneral);
			return false;
		}

		return true;
	}

	void RemoveGeneralFromCityAndArmy() {
		if (generalInfo.city == -1) {
			for (int i=0; i<Informations.Instance.armys.Count; i++) {
				ArmyInfo army = Informations.Instance.armys[i];
				
				if (army.generals.Contains(generalIndex)) {
					army.generals.Remove(generalIndex);
					if (army.generals.Count == 0) {
						for (int j=0; j<army.prisons.Count; j++) {
							Informations.Instance.GetGeneralInfo(army.prisons[i]).king = -1;
						}
						Informations.Instance.armys.Remove(army);
					} else {
						if (army.commander == generalIndex) {
							army.commander = army.generals[0];
						}
					}
					break;
				}
				
				if (army.prisons.Contains(generalIndex)) {
					generalInfo.prisonerIdx = -1;
					army.prisons.Remove(generalIndex);
					break;
				}
			}
		} else {
			CityInfo cInfo = Informations.Instance.GetCityInfo(generalInfo.city);
			if (cInfo.generals.Contains(generalIndex)) {
				cInfo.generals.Remove(generalIndex);
				if (cInfo.generals.Count == 0) {
					if (cInfo.king != -1) {
						Informations.Instance.GetKingInfo(cInfo.king).cities.Remove(generalInfo.city);
						cInfo.king = -1;
					}
					for (int i=0; i<cInfo.prisons.Count; i++) {
						Informations.Instance.GetGeneralInfo(cInfo.prisons[i]).king = -1;
						Informations.Instance.GetGeneralInfo(cInfo.prisons[i]).city = -1;
					}
				} else {
					if (cInfo.prefect == generalIndex) {
						cInfo.prefect = cInfo.generals[0];
					}
				}
			} else if (cInfo.prisons.Contains(generalIndex)) {
				generalInfo.prisonerIdx = -1;
				cInfo.prisons.Remove(generalIndex);
			}
		}
	}

	bool OnKingSelectChange(int index) {
		
		if(!ReturnSelectGeneralFlag())
			return false;
		
		if (generalInfo.king != -1 
		    && generalIndex == Informations.Instance.GetKingInfo(generalInfo.king).generalIdx 
		    && generalInfo.king != kingIdxList[index]) {
			CheatMakerController.Instance.GetTips(ZhongWen.Instance.cheatFailChangeKing);
			return false;
		}

		if (Informations.Instance.GetKingInfo(kingIdxList[index]).cities.Count == 0) {
			CheatMakerController.Instance.GetTips(ZhongWen.Instance.cheatFailChangeKing);
			return false;
		}

		if (generalInfo.king == kingIdxList[index]) 
			return true;
		
		RemoveGeneralFromCityAndArmy();

		generalInfo.king = kingIdxList[index];
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			if (Informations.Instance.GetCityInfo(i).king == generalInfo.king) {
				generalInfo.city = i;
				cityPopupList.SetItemSelect(i);
				Informations.Instance.GetCityInfo(i).generals.Add(generalIndex);
				break;
			}
		}
		
		return true;
	}

	bool OnCitySelectChange(int index) {
		
		if(!ReturnSelectGeneralFlag())
			return false;

		if (generalInfo.king != -1 
		    && generalIndex == Informations.Instance.GetKingInfo(generalInfo.king).generalIdx 
		    && generalInfo.king != Informations.Instance.GetCityInfo(index).king) {
			CheatMakerController.Instance.GetTips(ZhongWen.Instance.cheatFailChangeCity);
			return false;
		}

		if (generalInfo.city == index)
			return true;

		RemoveGeneralFromCityAndArmy();

		generalInfo.city = index;
		CityInfo cInfo = Informations.Instance.GetCityInfo(index);
		if (cInfo.king == -1 && generalInfo.king != -1) {
			cInfo.king = generalInfo.king;
		} else {
			generalInfo.king = Informations.Instance.GetCityInfo(index).king;
		}
		if (generalInfo.king != -1) {
			Informations.Instance.GetCityInfo(index).generals.Add(generalIndex);
		}
		kingPopupList.SetItemSelect(kingIdxList.IndexOf(generalInfo.king));
		return true;
	}

	bool OnWeaponSelectChange(int index) {

		if(!ReturnSelectGeneralFlag())
			return false;

		generalInfo.equipment = index;

		return true;
	}

	bool OnMagicSelectChange(int index) {

		if(!ReturnSelectGeneralFlag())
			return false;

		for (int i=0; i<4; i++) {
			if (generalInfo.magic[i] == -1) {
				generalInfo.magic[i] = index;
				labelMagic[i].text = ZhongWen.Instance.GetMagicName(index);
				return true;
			} else if (generalInfo.magic[i] == index) {
				return true;
			}
		}

		for (int i=0; i<3; i++) {
			generalInfo.magic[i] = generalInfo.magic[i+1];
			labelMagic[i].text = ZhongWen.Instance.GetMagicName(generalInfo.magic[i]);
		}
		generalInfo.magic[3] = index;
		labelMagic[3].text = ZhongWen.Instance.GetMagicName(generalInfo.magic[3]);

		return true;
	}

	public void OnLevelValueChange() {

		if (generalInfo != null) {
			generalInfo.level = (int)(2 + 48 * sliderLevel.value);
			generalInfo.experience = Misc.GetLevelExperience(generalInfo.level);

			if (generalInfo.level <= 20) {
				generalInfo.soldierMax = 5 * generalInfo.level;
				generalInfo.knightMax = 0;
				generalInfo.soldierCur = generalInfo.soldierMax;
				generalInfo.knightCur = generalInfo.knightMax;
			} else if (generalInfo.level <= 40) {
				generalInfo.soldierMax = 100 - (generalInfo.level - 20) * 5;
				generalInfo.knightMax = (generalInfo.level - 20) * 5;

				generalInfo.soldierCur = generalInfo.soldierMax;
				generalInfo.knightCur = generalInfo.knightMax;
			} else if (generalInfo.level > 40){
				generalInfo.soldierMax = 0;
				generalInfo.knightMax = 100;
				generalInfo.soldierCur = generalInfo.soldierMax;
				generalInfo.knightCur = generalInfo.knightMax;
			}
		}
	}

	public void OnStrengthValueChange() {

		if (generalInfo != null) {
			generalInfo.strength = (int)(120f * sliderStrength.value);
		}
	}

	public void OnHealthyValueChange() {

		if (generalInfo != null) {
			generalInfo.healthCur = (int)(200f * sliderHealthy.value);
			if (generalInfo.healthMax < generalInfo.healthCur)
				generalInfo.healthMax = generalInfo.healthCur;
		}
	}

	public void OnIntellectValueChange() {

		if (generalInfo != null) {
			generalInfo.intellect = (int)(120f * sliderIntellect.value);
		}
	}

	public void OnManaValueChange() {

		if (generalInfo != null) {
			generalInfo.manaCur = (int)(200f * sliderMana.value);
			if (generalInfo.manaMax < generalInfo.manaCur)
				generalInfo.manaMax = generalInfo.manaCur;
		}
	}

	public void OnSoldierValueChange() {

		if (generalInfo != null) {
			int num = (int)(100 * sliderSoldier.value);
			if (generalInfo.knightMax < num) {
				generalInfo.knightCur = generalInfo.knightMax;
				num -= generalInfo.knightCur;
			} else {
				generalInfo.knightCur = num;
				num = 0;
			}
			generalInfo.soldierCur = num;
			if (generalInfo.soldierMax < generalInfo.soldierCur)
				generalInfo.soldierMax = generalInfo.soldierCur;
		}
	}

	void OnEnter() {
		tweenPos.Play(true);
	}
	
	void OnReturn() {
		tweenPos.Play(false);
		
	}

	public void OnTweenOver() {
		if (tweenPos.transform.localPosition == tweenPos.from) {
			CheatMakerController.Instance.ChangeState(CheatMakerController.State.MainMenu);
		}
	}
}
