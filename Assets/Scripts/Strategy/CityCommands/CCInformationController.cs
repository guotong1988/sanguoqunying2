using UnityEngine;
using System.Collections;

public class CCInformationController : MonoBehaviour {
	
	public GeneralsHeadSelect head;
	public exSpriteFont historyTime;
	public exSpriteFont prefect;
	public exSpriteFont kingName;
	public exSpriteFont cityName;
	public exSpriteFont defense;
	public exSpriteFont population;
	public exSpriteFont money;
	public exSpriteFont generalNum;
	public exSpriteFont soldierNum;
	public exSpriteFont reservistNum;
	public exSpriteFont prisonNum;
	
	
	public void SetCity(int idx) {
		
		if (idx < 0 || idx >= Informations.Instance.cityNum) {
			return;
		}
		
		CityInfo cInfo = Informations.Instance.GetCityInfo(idx);
		
		historyTime.text = Controller.historyTime + ZhongWen.Instance.nian;
		
		if (cInfo.king == -1) {
			
			head.SetGeneralHead(-1);
			
			prefect.text = ZhongWen.Instance.wurenzhanling;
			kingName.text = ZhongWen.Instance.wurenzhanling;
		} else {
			
			head.SetGeneralHead(cInfo.prefect);
			
			string pName = ZhongWen.Instance.GetGeneralName(cInfo.prefect);
			
			prefect.text = pName;
			
			
			if (cInfo.king < Informations.Instance.kingNum) {
				kingName.text = ZhongWen.Instance.GetKingName(cInfo.king);
			} else {
				kingName.text = ZhongWen.Instance.daozei;
			}
		}
		
		cityName.text = ZhongWen.Instance.GetCityName(idx);
		
		if (cInfo.king == Controller.kingIndex) {
			defense.text = cInfo.defense + "";
			population.text = cInfo.population + "";
			money.text = cInfo.money + "";
			
			reservistNum.text = cInfo.reservist + "/" + cInfo.reservistMax;
		} else {
			if (cInfo.king == -1) {
				defense.text = cInfo.defense + "";
				population.text = cInfo.population + "";
			} else {
				defense.text = "---";
				population.text = "---";
			}
			money.text = "---";
			reservistNum.text = "---";
		}
		
		if (cInfo.king != -1) {
			
			generalNum.text = cInfo.generals.Count + "";
			
			if (cInfo.king == Controller.kingIndex) {
				
				soldierNum.text = cInfo.soldiersNum + "";
				prisonNum.text = cInfo.prisons.Count + "";
			} else {
				soldierNum.text = "---";
				prisonNum.text = "---";
			}
		} else {
			generalNum.text = "---";
			soldierNum.text = "---";
			prisonNum.text = "---";
		}
	}
	
}
