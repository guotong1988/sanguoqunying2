using UnityEngine;
using System.Collections;

public class ACInformationController : MonoBehaviour {
	
	public GeneralsHeadSelect head;
	
	public exSpriteFont kingName;
	public exSpriteFont commanderName;
	public exSpriteFont targetcity;
	public exSpriteFont generalNum;
	public exSpriteFont soldierNum;
	public exSpriteFont prisonNum;
	public exSpriteFont money;
	
	public void SetArmy(ArmyInfo armyInfo) {
		
		head.SetGeneralHead(armyInfo.commander);
		
		if (armyInfo.king < Informations.Instance.kingNum) {
			kingName.text = ZhongWen.Instance.GetKingName(armyInfo.king) + ZhongWen.Instance.jun;
		} else {
			kingName.text = ZhongWen.Instance.daozei + ZhongWen.Instance.jun;
		}
		
		string cName = ZhongWen.Instance.GetGeneralName(armyInfo.commander);
		
		commanderName.text = cName;
		
		if (armyInfo.cityTo != -1) {
			targetcity.text = ZhongWen.Instance.GetCityName(armyInfo.cityTo);
		} else {
			targetcity.text = "---";
		}
		
		generalNum.text = armyInfo.generals.Count + "";
		
		if (armyInfo.king == Controller.kingIndex) {
			int sNum = 0;
			for (int i=0; i<armyInfo.generals.Count; i++) {
				sNum += Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).soldierCur;
				sNum += Informations.Instance.GetGeneralInfo(armyInfo.generals[i]).knightCur;
			}
			soldierNum.text = sNum + "";
			
			prisonNum.text 	= armyInfo.prisons.Count + "";
			money.text 		= armyInfo.money + "";
			
		} else {
			soldierNum.text = "---";
			prisonNum.text 	= "---";
			money.text 		= "---";
		}
	}
}
