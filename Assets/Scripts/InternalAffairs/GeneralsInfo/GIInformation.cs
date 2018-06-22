using UnityEngine;
using System.Collections;

public class GIInformation : MonoBehaviour {
	
	public GeneralsHeadSelect headSelect;
	
	public exSpriteFont generalName;
	public exSpriteFont kingName;
	public exSpriteFont job;
	public exSpriteFont level;
	public exSpriteFont force;
	public exSpriteFont health;
	public exSpriteFont intellect;
	public exSpriteFont mana;
	public exSpriteFont soldier;
	public exSpriteFont experience;
	public exSpriteFont equipment;
	
	public void SetGeneral(int idx) {
		
		headSelect.SetGeneralHead(idx);
		
		string str = ZhongWen.Instance.GetGeneralName1(idx);
		
		generalName.text = str;
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(idx);
		
		if (gInfo.prisonerIdx != -1 || gInfo.king == -1 || gInfo.king == Informations.Instance.kingNum) {
			kingName.text = "---";
		} else {
			kingName.text = ZhongWen.Instance.GetKingName(gInfo.king);
		}
		
		if (gInfo.prisonerIdx != -1 || gInfo.king == -1 || gInfo.king == Informations.Instance.kingNum 
			|| Informations.Instance.GetKingInfo(gInfo.king).generalIdx == idx) {
			job.text = "---";
		} else {
			job.text = ZhongWen.Instance.GetJobsName(gInfo.job);
		}
		
		level.text = "" + gInfo.level;
		force.text = "" + gInfo.strength;
		intellect.text = "" + gInfo.intellect;
		health.text = gInfo.healthCur + "/" + gInfo.healthMax;
		mana.text = gInfo.manaCur + "/" + gInfo.manaMax;
		soldier.text = (gInfo.soldierCur + gInfo.knightCur) + "/" + (gInfo.soldierMax + gInfo.knightMax);
		experience.text = gInfo.experience + "/" + Misc.GetLevelExperience(gInfo.level+1);
		equipment.text = ZhongWen.Instance.GetEquipmentName(gInfo.equipment);
	}
}
