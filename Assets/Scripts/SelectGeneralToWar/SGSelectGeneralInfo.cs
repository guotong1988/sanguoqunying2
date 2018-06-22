using UnityEngine;
using System.Collections;

public class SGSelectGeneralInfo : MonoBehaviour {
	
	public GeneralsHeadSelect headSelect;
	
	public exSpriteFont generalName;
	public exSpriteFont level;
	public exSpriteFont strength;
	public exSpriteFont intellect;
	public exSpriteFont cityDefense;
	public exSpriteFont terrain;
	public exSpriteFont formation;
	public exSpriteFont position;
	
	public void SetGeneralInformation(int gIdx, int defense, int pos) {
		
		headSelect.SetGeneralHead(gIdx);
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		generalName	.text = ZhongWen.Instance.GetGeneralName(gIdx);
		level		.text = gInfo.level 	+ "";
		strength	.text = gInfo.strength 	+ "";
		intellect	.text = gInfo.intellect	+ "";
		cityDefense	.text = defense + "";
		
		terrain.text = "0";
		formation.text = ZhongWen.Instance.GetFormationName(gInfo.formationCur);
		
		if (pos == 0) {
			position.text = ZhongWen.Instance.houlie;
		} else {
			position.text = ZhongWen.Instance.qianlie;
		}
	}
}
