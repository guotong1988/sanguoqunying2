using UnityEngine;
using System.Collections;

public class KingInfoController : MonoBehaviour {
	
	public GeneralsHeadSelect head;
	
	public exSpriteFont kingName;
	public exSpriteFont cityNum;
	public exSpriteFont money;
	public exSpriteFont population;
	public exSpriteFont generalNum;
	public exSpriteFont soldierNum;
	
	public void SetKing(int idx) {
		
		if (idx < 0 || idx >= Informations.Instance.kingNum) {
			return;
		}
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(idx);
		
		head.SetGeneralHead(Informations.Instance.GetKingInfo(idx).generalIdx);
		
		kingName.text = ZhongWen.Instance.GetKingName(idx);
		
		long _money = 0;
		long _population = 0;
		
		for (int i=0; i<kInfo.cities.Count; i++) {
			int cIdx = (int)kInfo.cities[i];
			CityInfo cInfo = Informations.Instance.GetCityInfo(cIdx);
			
			_money += cInfo.money;
			_population += cInfo.population;
		}
		
		cityNum.text 	= kInfo.cities.Count + "";
		money.text		= _money + "";
		population.text	= _population + ZhongWen.Instance.ren;
		
		int _soldierNum = 0;
		
		for (int i=0; i<kInfo.generals.Count; i++) {
			int gIdx = (int)kInfo.generals[i];
			_soldierNum += Informations.Instance.GetGeneralInfo(gIdx).soldierCur;
			_soldierNum += Informations.Instance.GetGeneralInfo(gIdx).knightCur;
		}
		
		generalNum.text = kInfo.generals.Count + ZhongWen.Instance.ren;
		soldierNum.text = _soldierNum + ZhongWen.Instance.ren;
	}
}
