using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMCityPanel : MonoBehaviour {

	public TweenPosition tweenPos;

	public GameObject cityRoot;
	public GameObject kingRoot;

	public UISlider sliderMoney;
	public UISlider sliderPopulation;
	public UISlider sliderReservist;
	public UISlider sliderDefense;

	private int cityIndex;
	private CityInfo cityInfo = null;

	private CMPopupList kingPopupList;
	private List<int> kingIdxList = new List<int>();

	private Vector3 cityPopdownListPos = new Vector3(3, 130, -20);
	private Vector3 kingPopdownListPos = new Vector3(3, 80, -20);

	// Use this for initialization
	void Start () {
	
		InitCityPopupList();
		InitKingPopupList();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitCityPopupList() {

		List<string> cities = new List<string>();
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			string str = ZhongWen.Instance.GetCityName(i);
			cities.Add(str);
		}
		
		GameObject go = CheatMakerController.Instance.GetPopupList(cities, OnCitySelectChange);
		go.transform.parent = cityRoot.transform;
		go.transform.localPosition = cityPopdownListPos;
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
		go.transform.localPosition = kingPopdownListPos;
		go.transform.localScale = Vector3.one;

		kingPopupList = go.GetComponent<CMPopupList>();
	}

	bool OnCitySelectChange(int index) {

		cityIndex = index;
		cityInfo = Informations.Instance.GetCityInfo(index);

		kingPopupList.SetItemSelect(cityInfo.king);

		sliderMoney.value = cityInfo.money / 999999f;
		sliderPopulation.value = cityInfo.population / 999999f;
		sliderDefense.value = cityInfo.defense / 999f;
		sliderReservist.value = cityInfo.reservist / 999f;

		return true;
	}

	bool OnKingSelectChange(int index) {

		if (cityInfo == null) {
			CheatMakerController.Instance.GetTips(ZhongWen.Instance.cheatSelectCity);
			return false;
		}

		if (cityInfo.king != -1 && cityInfo.king != kingIdxList[index]
		    && cityInfo.prefect == Informations.Instance.GetKingInfo(cityInfo.king).generalIdx) {
			CheatMakerController.Instance.GetTips(ZhongWen.Instance.cheatCanNotSelect);
			return false;
		}

		if (cityInfo.king != -1) {
			Informations.Instance.GetKingInfo(cityInfo.king).cities.Remove(cityIndex);
		}

		cityInfo.king = kingIdxList[index];

		if (cityInfo.king != -1) {
			Informations.Instance.GetKingInfo(cityInfo.king).cities.Add(cityIndex);
		}

		for (int i=0; i<cityInfo.generals.Count; i++) {
			Informations.Instance.GetGeneralInfo(cityInfo.generals[i]).king = cityInfo.king;
		}

		for (int i=0; i<cityInfo.prisons.Count; i++) {
			Informations.Instance.GetGeneralInfo(cityInfo.prisons[i]).prisonerIdx = cityInfo.king;
		}

		return true;
	}

	public void OnMoneyValueChange() {

		if (cityInfo != null) {
			cityInfo.money = (int)(999999f * sliderMoney.value);
		}
	}

	public void OnPopulationValueChange() {

		if (cityInfo != null) {
			cityInfo.population = (int)(999999f * sliderPopulation.value);
		}
	}

	public void OnReservistValueChange() {

		if (cityInfo != null) {
			cityInfo.reservist = (int)(999f * sliderReservist.value);
			if (cityInfo.reservistMax < cityInfo.reservist)
				cityInfo.reservistMax = cityInfo.reservist;
		}
	}

	public void OnDefenseValueChange() {

		if (cityInfo != null) {
			cityInfo.defense = (int)(999f * sliderDefense.value);
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
