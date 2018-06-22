using UnityEngine;
using System.Collections;

public class TopBarInfo : MonoBehaviour {
	
	public exSpriteFont armyName;
	public exSpriteFont generalsNum;
	public exSpriteFont solidersNum;
	
	public void SetInformation(int kingIdx, int gNum, int sNum) {
		
		if (kingIdx < Informations.Instance.kingNum) {
			armyName.text = ZhongWen.Instance.GetKingName(kingIdx) + ZhongWen.Instance.jun;
		} else {
			armyName.text = ZhongWen.Instance.daozei + ZhongWen.Instance.jun;
		}
		
		generalsNum.text = gNum + "";
		solidersNum.text = sNum + "";
	}
}
