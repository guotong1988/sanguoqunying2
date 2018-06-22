using UnityEngine;
using System.Collections;

public class CityInfoController : MonoBehaviour {
	
	public GameObject label;
	public exSpriteFont cityName;
	public exSpriteFont generalNum;
	public exSpriteFont reservist;
	public exSpriteFont defense;
	public exSpriteFont population;
	public exSpriteFont money;
	
	
	public void SetCity(int idx) {
		
		if (idx < 0 || idx >= Informations.Instance.cityNum) {
			label.SetActive(false);
			
			cityName.text = "";
			generalNum.text = "";
			reservist.text = "";
			defense.text = "";
			population.text = "";
			money.text = "";
			
			return;
		}
		
		label.SetActive(true);
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(idx);
		
		cityName.text = ZhongWen.Instance.GetCityName(idx);
		
		generalNum.text = "" + cInfo.generals.Count;
		
		reservist.text = cInfo.reservist + "/" + cInfo.reservistMax;
		defense.text = cInfo.defense + "";
		population.text = cInfo.population + ZhongWen.Instance.ren;
		money.text = cInfo.money + "";
	}
}
