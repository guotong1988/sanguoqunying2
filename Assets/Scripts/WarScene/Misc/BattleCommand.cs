using UnityEngine;
using System.Collections;

public class BattleCommand : MonoBehaviour {
	
	public WarSceneController warCtrl;
	public GameObject magic;
	public Button[] commands;

	private bool isAssaultFlag;
	private MenuDisplayAnim menuAnim;
	
	// Use this for initialization
	void Awake () {
		
		for (int i=0; i<commands.Length; i++) {
			commands[i].SetButtonData(i);
			commands[i].SetButtonClickHandler(OnCommand);
		}
		
		menuAnim = GetComponent<MenuDisplayAnim>();
	}
	
	void OnEnable() {
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);

		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);

		if (warCtrl.rightGeneral.GetArmyAssaultFlag() || gInfo.soldierCur + gInfo.knightCur == 0) {

			for (int i=0; i<6; i++) {
				commands[i].SetButtonEnable(false);
			}
		} else {
			for (int i=0; i<6; i++) {
				commands[i].SetButtonEnable(true);
			}
		}

		if (warCtrl.rightGeneral.IsCanReleaseMagic()) {
			commands[6].SetButtonEnable(true);
		} else {
			commands[6].SetButtonEnable(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!menuAnim.IsPlaying() && Misc.GetBack()) {
			OnReturn();
		}
	}
	
	void OnCommand(object data) {

		if (menuAnim.IsPlaying())	return;

		int idx = (int)data;
		switch (idx) {
		case 0:
			warCtrl.SetFrontArmyRun(WarSceneController.WhichSide.Right);
			break;
		case 1:
			warCtrl.SetFrontArmyBack(WarSceneController.WhichSide.Right);
			break;
		case 2:
			warCtrl.SetArmyDisperse();
			break;
		case 3:
			warCtrl.SetArmyFallIn();
			break;
		case 4:
			warCtrl.SetArmyIdle();
			break;
		case 5:
			isAssaultFlag = true;
			warCtrl.SetArmyAssault(WarSceneController.WhichSide.Right);
			break;
		case 6:
			magic.SetActive(true);
			menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
			return;
			//break;
		case 7:
			warCtrl.SetGeneralAssault(WarSceneController.WhichSide.Right);
			break;
		case 8:
			warCtrl.SetArmyEscape(WarSceneController.WhichSide.Right);

			for (int i=0; i<commands.Length; i++) {
				commands[i].SetButtonEnable(false);
			}
			break;
		}
		
		OnReturn();
	}
	
	void OnReturn() {

		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);

		Invoke("ReturnMain", 0.3f);
	}

	void ReturnMain() {

		gameObject.SetActive(false);

		if (isAssaultFlag) {
			isAssaultFlag = false;
			WarSceneController.state = WarSceneController.State.Assault;
		} else {
			warCtrl.OnResumeGame();
			WarSceneController.state = WarSceneController.State.Running;
		}
	}
}
