using UnityEngine;
using System.Collections;

public class MagicController {

	public WarSceneController warCtrl;
	public MagicShow magicShow;
	public MagicGrayShow grayCtrl;
	public WSInfoPanel uiPanel;
	public Transform magicRoot;

	private GameObject magicGO;
	private MagicDataInfo info;
	private WarSceneController.WhichSide side;
	private WarSceneController.WhichSide enemySide;

	private int magicCount;

	private static MagicController mInstance;
	public static MagicController Instance {
		get {
			if (mInstance == null) {
				mInstance = new MagicController();
			}
			return mInstance;
		}
	}

	public void SetMagic(int no, WarSceneController.WhichSide side) {

		info = MagicManager.Instance.GetMagicDataInfo(no);

		warCtrl.OnResumeGame();
		WarSceneController.state = WarSceneController.State.Magic;

		this.side = side;
		if (side == WarSceneController.WhichSide.Right) {
			enemySide = WarSceneController.WhichSide.Left;
			magicShow.magicInfo.SetInfo(info);

			warCtrl.rightGeneral.SetGeneralWaitForMagic(info.TIME);
			warCtrl.SetCameraMoveTo(new Vector3(
				warCtrl.rightGeneral.transform.localPosition.x,
				-30,
				0));
		} else {
			enemySide = WarSceneController.WhichSide.Right;
			warCtrl.leftGeneral.SetGeneralWaitForMagic(info.TIME);
			warCtrl.SetCameraMoveTo(new Vector3(
				warCtrl.leftGeneral.transform.localPosition.x,
				-30,
				0));
		}

		if (no == 18 || no == 49) {
			magicCount = 3;
		} else {
			magicCount = 1;
		}

		grayCtrl.SetGray(true);
		warCtrl.WaitforMagicMoveToGeneral();
	}

	public void OnGeneralSetMagic() {
		if (side == WarSceneController.WhichSide.Right) {
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);
			if (gInfo.manaCur >= info.MP) {
				gInfo.manaCur -= info.MP;
			}

			warCtrl.rightGeneral.SetOnMagic(info.TYPE);
		} else {
			GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.leftGeneralIdx);
			if (gInfo.manaCur >= info.MP) {
				gInfo.manaCur -= info.MP;
			}

			warCtrl.leftGeneral.SetOnMagic(info.TYPE);
		}

		warCtrl.SetIsSwordFull(side, false);
		magicGO = GameObject.Instantiate(Resources.Load("Magic/" + info.SCRIPT)) as GameObject;
	}

	public void OnArrowOut() {
		magicGO.SendMessage("WaitForArrowOut", SendMessageOptions.DontRequireReceiver);
	}

	public void OnMagicOver() {

		magicCount--;
		if (magicCount > 0) return;

		if (WarSceneController.state == WarSceneController.State.Magic) {
			WarSceneController.state = WarSceneController.State.Running;
		}

		grayCtrl.SetGray(false);

		if (magicGO != null) {
			GameObject.Destroy(magicGO);
		}
		//Resources.UnloadUnusedAssets();
	}

	public int GetMagicAttack() {
		return info.ATTACK;
	}

	public int GetMagicType() {
		return info.TYPE;
	}

	public WarSceneController.WhichSide GetMagicSide() {
		return side;
	}

	public WarSceneController.WhichSide GetMagicEnemySide() {
		return enemySide;
	}
}
