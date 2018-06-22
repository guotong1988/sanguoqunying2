using UnityEngine;
using System.Collections;

public class SGGeneralItem : MonoBehaviour {
	
	public GeneralsHeadSelect head;
	public exSpriteAnimation arms;
	
	public exSpriteFont generalName;
	public exSpriteFont health;
	public exSpriteFont mana;
	public exSpriteFont soldier;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetGeneral(int gIdx, bool isFail) {
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		head.SetGeneralHead(gIdx);
		head.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
		if (isFail) {
			head.transform.GetChild(0).GetComponent<exSprite>().color = new Color(0.25f, 0.25f, 0.25f, 1);
		}
		
		generalName.text = ZhongWen.Instance.GetGeneralName(gIdx);
		
		health.text = gInfo.healthCur + "";
		mana.text = gInfo.manaCur + "";
		soldier.text = (gInfo.soldierCur + gInfo.knightCur) + "";
		
		int idx = Misc.GetArmsIdx(gInfo.armsCur);
		
		arms.SetFrame(arms.GetCurrentAnimation().name, idx);
	}
}
