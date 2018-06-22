using UnityEngine;
using System.Collections;

public class WSInfoPanel : MonoBehaviour {
	
	public WarSceneController warCtrl;
	
	public GeneralsHeadSelect leftHead;
	
	public exSpriteFont leftName;
	public exSpriteFont leftLevel;
	public exSpriteFont leftStrength;
	public exSpriteFont leftSolider;
	public exSpriteFont leftHealthy;
	public exSpriteFont leftMana;
	
	public exSprite leftHealthyBar;
	public exSprite leftHealthyBarBG;
	public exSoftClip leftHealtyClipping;
	public exSprite leftManaBar;
	public exSprite leftManaBarBG;
	public exSoftClip leftManaClipping;
	public exSoftClip leftSwordClipping;
	public exSprite leftSwordSP;
	
	public GeneralsHeadSelect rightHead;
	
	public exSpriteFont rightName;
	public exSpriteFont rightLevel;
	public exSpriteFont rightStrength;
	public exSpriteFont rightSolider;
	public exSpriteFont rightHealthy;
	public exSpriteFont rightMana;
	
	public exSprite rightHealthyBar;
	public exSprite rightHealthyBarBG;
	public exSoftClip rightHealtyClipping;
	public exSprite rightManaBar;
	public exSprite rightManaBarBG;
	public exSoftClip rightManaClipping;
	public exSoftClip rightSwordClipping;
	public exSprite rightSwordSP;
	
	public exSpriteFont timeLeftF;
	
	private int leftGeneralIdx;
	private int rightGeneralIdx;
	private GeneralInfo leftGInfo;
	private GeneralInfo rightGInfo;
	
	private float leftSwordValue;
	private float rightSwordValue;
	
	private float leftHealtyScale;
	private float leftManaScale;
	private float rightHealtyScale;
	private float rightManaScale;
	
	private int timeLeft;
	private float timeTick;
	private float swordTimeTick;
	
	// Use this for initialization
	void Start () {
		InitInfo();

		MagicController.Instance.uiPanel = this;
	}
	
	// Update is called once per frame
	void Update () {
		
		OnGeneralInfoUpdate();
		OnTimeLeftUpdate();
		OnSwordStateUpdate();
	}
	
	void InitInfo() {
		
		leftGeneralIdx = WarSceneController.leftGeneralIdx;
		rightGeneralIdx = WarSceneController.rightGeneralIdx;
		
		leftGInfo = Informations.Instance.GetGeneralInfo(leftGeneralIdx);
		rightGInfo = Informations.Instance.GetGeneralInfo(rightGeneralIdx);
		
		leftHead.SetGeneralHead(leftGeneralIdx);
		leftName.text = ZhongWen.Instance.GetGeneralName(leftGeneralIdx);
		if (leftGInfo.level < 10) {
			leftLevel.text = "0" + leftGInfo.level;
		} else {
			leftLevel.text = "" + leftGInfo.level;
		}
		leftStrength.text 	= "" + leftGInfo.strength;
		leftSolider.text 	= "" + (leftGInfo.soldierCur + leftGInfo.knightCur);
		leftHealthy.text 	= leftGInfo.healthCur + "/" + leftGInfo.healthMax;
		leftMana.text 		= leftGInfo.manaCur + "/" + leftGInfo.manaMax;
		
		if (leftGInfo.healthMax < 150) {
			leftHealtyScale = 1;
			leftHealthyBar.width = leftGInfo.healthMax;
			leftHealthyBarBG.width = leftGInfo.healthMax + 4;
		} else {
			leftHealtyScale = 150f / leftGInfo.healthMax;
			leftHealthyBar.width = 150;
			leftHealthyBarBG.width = 150 + 4;
		}
		leftHealtyClipping.width = leftGInfo.healthCur * leftHealtyScale;
		
		if (leftGInfo.manaMax < 150) {
			leftManaScale = 1;
			leftManaBar.width = leftGInfo.manaMax;
			leftManaBarBG.width = leftGInfo.manaMax + 4;
		} else {
			leftManaScale = 150f / leftGInfo.manaMax;
			leftManaBar.width = 150;
			leftManaBarBG.width = 150 + 4;
		}
		leftManaClipping.width = leftGInfo.manaCur * leftManaScale;
		
		rightHead.SetGeneralHead(rightGeneralIdx);
		rightName.text = ZhongWen.Instance.GetGeneralName(rightGeneralIdx);
		if (rightGInfo.level < 10) {
			rightLevel.text = "0" + rightGInfo.level;
		} else {
			rightLevel.text = "" + rightGInfo.level;
		}
		rightStrength.text 	= "" + rightGInfo.strength;
		rightSolider.text 	= "" + (rightGInfo.soldierCur + rightGInfo.knightCur);
		rightHealthy.text 	= rightGInfo.healthCur + "/" + rightGInfo.healthMax;
		rightMana.text 		= rightGInfo.manaCur + "/" + rightGInfo.manaMax;
		
		if (rightGInfo.healthMax < 150) {
			rightHealtyScale = 1;
			rightHealthyBar.width = rightGInfo.healthMax;
			rightHealthyBarBG.width = rightGInfo.healthMax + 4;
		} else {
			rightHealtyScale = 150f / rightGInfo.healthMax;
			rightHealthyBar.width = 150;
			rightHealthyBarBG.width = 150 + 4;
		}
		rightHealtyClipping.width = rightGInfo.healthCur * rightHealtyScale;
		
		if (rightGInfo.manaMax < 150) {
			rightManaScale = 1;
			rightManaBar.width = rightGInfo.manaMax;
			rightManaBarBG.width = rightGInfo.manaMax + 4;
		} else {
			rightManaScale = 150f / rightGInfo.manaMax;
			rightManaBar.width = 150;
			rightManaBarBG.width = 150 + 4;
		}
		rightManaClipping.width = rightGInfo.manaCur * rightManaScale;
		
		timeLeft = 99;
		timeLeftF.text = "" + timeLeft;
		
		leftSwordValue = 60;
		leftSwordClipping.width = 60;
		rightSwordValue = 60;
		rightSwordClipping.width = 60;
	}
	
	void OnTimeLeftUpdate() {
		
		if (WarSceneController.state != WarSceneController.State.Running && WarSceneController.state != WarSceneController.State.Magic)
			return;
		
		timeTick += Time.deltaTime;
		if (timeTick >= 1) {
			timeTick = 0;
			
			timeLeft--;
			if (timeLeft <= 0) {
				timeLeft = 0;
				warCtrl.OnTimeUp();
			}
			
			timeLeftF.text = "" + timeLeft;
		}
	}
	
	void OnGeneralInfoUpdate() {
		
		leftHealthy.text 	= leftGInfo.healthCur + "/" + leftGInfo.healthMax;
		leftMana.text 		= leftGInfo.manaCur + "/" + leftGInfo.manaMax;
		rightHealthy.text 	= rightGInfo.healthCur + "/" + rightGInfo.healthMax;
		rightMana.text 		= rightGInfo.manaCur + "/" + rightGInfo.manaMax;
		
		leftHealtyClipping.width 	= leftGInfo.healthCur * leftHealtyScale;
		leftManaClipping.width 		= leftGInfo.manaCur * leftManaScale;
		rightHealtyClipping.width 	= rightGInfo.healthCur * rightHealtyScale;
		rightManaClipping.width 	= rightGInfo.manaCur * rightManaScale;
		
		leftSolider.text 	= "" + (leftGInfo.soldierCur + leftGInfo.knightCur);
		rightSolider.text 	= "" + (rightGInfo.soldierCur + rightGInfo.knightCur);
	}
	
	void OnSwordStateUpdate() {
		
		if (WarSceneController.state != WarSceneController.State.Running && WarSceneController.state != WarSceneController.State.Magic)
			return;
		
		swordTimeTick += Time.deltaTime;
		if (swordTimeTick >= 1f) {
			swordTimeTick = 0;
		}

		float step = Time.deltaTime * (leftGInfo.strength / 30f + 3f);
		if (warCtrl.leftGeneral.GetState() == GeneralController.State.Escape)
			step = -step;

		if (leftSwordValue < 132) {

			leftSwordValue += step;
			leftSwordValue = Mathf.Clamp(leftSwordValue, 1, 132f);
			leftSwordClipping.width = leftSwordValue;
			if (leftSwordValue == 132) {
				warCtrl.SetIsSwordFull(WarSceneController.WhichSide.Left, true);
			}
		} else {

			if (warCtrl.leftGeneral.GetState() == GeneralController.State.Escape)
				leftSwordValue += step;

			if (swordTimeTick < 0.5f) {
				leftSwordSP.color = new Color(1, 1, 1, 1 - swordTimeTick);
			} else {
				leftSwordSP.color = new Color(1, 1, 1, swordTimeTick);
			}
		}

		step = Time.deltaTime * (rightGInfo.strength / 30f + 3f);
		if (warCtrl.rightGeneral.GetState() == GeneralController.State.Escape)
			step = -step;

		if (rightSwordValue < 132) {
			
			rightSwordValue += step;
			rightSwordValue = Mathf.Clamp(rightSwordValue, 1, 132f);
			rightSwordClipping.width = rightSwordValue;

			if (rightSwordValue == 132) {
				warCtrl.SetIsSwordFull(WarSceneController.WhichSide.Right, true);
			}
		} else {

			if (warCtrl.rightGeneral.GetState() == GeneralController.State.Escape)
				rightSwordValue += step;

			if (swordTimeTick < 0.5f) {
				rightSwordSP.color = new Color(1, 1, 1, 1 - swordTimeTick);
			} else {
				rightSwordSP.color = new Color(1, 1, 1, swordTimeTick);
			}
		}
	}
	
	public void ResetSwordValue(WarSceneController.WhichSide side) {

		if (side == WarSceneController.WhichSide.Left) {
			leftSwordValue = 0;
			leftSwordClipping.width = leftSwordValue;
			leftSwordSP.color = new Color(1, 1, 1, 1);
		} else {
			rightSwordValue = 0;
			rightSwordClipping.width = leftSwordValue;
			rightSwordSP.color = new Color(1, 1, 1, 1);
		}
	}
}
