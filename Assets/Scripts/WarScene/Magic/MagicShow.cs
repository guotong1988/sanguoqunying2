using UnityEngine;
using System.Collections;

public class MagicShow : MonoBehaviour {

	public WarSceneController warCtrl;
	public Button[] magics;

	public MagicInfoShow magicInfo;

	private MenuDisplayAnim menuAnim;
	private GeneralInfo gInfo;

	// Use this for initialization
	void Start () {
		MagicController.Instance.magicShow = this;
	}

	void OnEnable() {
		if (menuAnim == null)
			menuAnim = GetComponent<MenuDisplayAnim>();
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);

		gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);
		for (int i=0; i<4; i++) {
			if (gInfo.magic[i] != -1) {
				magics[i].SetButtonData(i);
				magics[i].SetButtonClickHandler(OnMagicClick);
				exSpriteFont sf = magics[i].GetComponent<exSpriteFont>();
				sf.text = ZhongWen.Instance.GetMagicName(gInfo.magic[i]);
				if (gInfo.manaCur >= MagicManager.Instance.GetMagicDataInfo(gInfo.magic[i]).MP) {
					magics[i].SetButtonEnable(true);
				} else {
					magics[i].SetButtonEnable(false);
				}
			} else {
				magics[i].gameObject.SetActive(false);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (!menuAnim.IsPlaying() && Misc.GetBack()) {

			warCtrl.OnResumeGame();

			WarSceneController.state = WarSceneController.State.Running;
			menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		}
	}

	void OnMagicClick(object data) {

		int idx = (int)data;
		int magicIndex = gInfo.magic[idx];

		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		MagicController.Instance.SetMagic(magicIndex, WarSceneController.WhichSide.Right);

		Misc.isBack = true;
	}
}
