using UnityEngine;
using System.Collections;

public class IAController : MonoBehaviour {
	
	public MenuDisplayAnim commandsMenuAnim;
	public MenuDisplayAnim kingInfoAnim;
	
	public GameObject[] commandsCtrl;
	
	public Button[] commandsButton;
	
	private int state = 0;
	private int selectIdx = -1;
	private bool isCommandDisable = false;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		
		commandsButton[0].SetButtonClickHandler(OnUsingObjects);
		commandsButton[1].SetButtonClickHandler(OnGeneralsInfo);
		commandsButton[2].SetButtonClickHandler(OnSurrenderPrisoners);
		commandsButton[3].SetButtonClickHandler(OnSearching);
		commandsButton[4].SetButtonClickHandler(OnFortification);
		commandsButton[5].SetButtonClickHandler(OnDeveloping);
		commandsButton[6].SetButtonClickHandler(OnPowerMap);
		commandsButton[7].SetButtonClickHandler(OnGeneralsPromotion);
		commandsButton[8].SetButtonClickHandler(OnIAOver);
		commandsButton[9].SetButtonClickHandler(OnSaving);
		commandsButton[10].SetButtonClickHandler(OnReturnStart);

		SoundController.Instance.PlayBackgroundMusic("Music02");
	}
	
	// Update is called once per frame
	void Update () {
		
		if (state == -1) return;
		
		if (Input.GetKeyDown(KeyCode.Escape)){
			selectIdx = 8;
			OnIAOver();
		}
		
		if (state == 2) {
			OnChangingToCommand();
		}
	}
	
	void LateUpdate() {
		if (isCommandDisable) {
			isCommandDisable = false;
			
			commandsButton[selectIdx].GetComponent<exSpriteFont>().botColor = new Color(1, 0, 0, 1);
			commandsButton[selectIdx].GetComponent<exSpriteFont>().topColor = new Color(1, 0, 0, 1);
		}
	}
	
	void OnUsingObjects() {
		selectIdx = 0;
		OnCommandClicked();
	}
	
	void OnGeneralsInfo() {
		selectIdx = 1;
		OnCommandClicked();
	}
	
	void OnSurrenderPrisoners() {
		selectIdx = 2;
		
		if (!commandsCtrl[2].GetComponent<SPController>().AddPrisonsList()) {
			
			commandsCtrl[2].SetActive(true);
			SetCommandsDisableState();
		} else {
			OnCommandClicked();
		}
	}
	
	void OnSearching() {
		selectIdx = 3;
		
		if (!commandsCtrl[3].GetComponent<SearchingController>().AddGeneralsList()) {

			commandsCtrl[3].SetActive(true);
			SetCommandsDisableState();
		} else {
			OnCommandClicked();
		}
	}
	
	void OnFortification() {
		selectIdx = 4;
		
		if (!commandsCtrl[4].GetComponent<FortificationController>().AddGeneralsList()) {
			
			commandsCtrl[4].SetActive(true);
			SetCommandsDisableState();
		} else {
			OnCommandClicked();
		}
	}
	
	void OnDeveloping() {
		selectIdx = 5;
		
		if (!commandsCtrl[5].GetComponent<DevelopingController>().AddGeneralsList()) {
			
			commandsCtrl[5].SetActive(true);
			SetCommandsDisableState();
		} else {
			OnCommandClicked();
		}
	}
	
	void OnPowerMap() {
		selectIdx = 6;
		
		OnCommandClicked();
	}
	
	void OnGeneralsPromotion() {
		selectIdx = 7;
		
		if (!commandsCtrl[7].GetComponent<GPController>().AddGeneralsList()) {
			
			commandsCtrl[7].SetActive(true);
			SetCommandsDisableState();
		} else {
			OnCommandClicked();
		}
	}
	
	void OnIAOver() {
		selectIdx = 8;
		
		commandsCtrl[8].SetActive(true);
		SetCommandsDisableState();
	}
	
	void OnSaving() {
		selectIdx = 9;
		OnCommandClicked();
	}

	void OnReturnStart() {
		Misc.LoadLevel("StartScene");
	}

	void OnCommandClicked() {
		state = 2;
		
		commandsMenuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		kingInfoAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
	}
	
	void OnChangingToCommand() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			state = -1;
			timeTick = 0;
			
			gameObject.SetActive(false);
			commandsCtrl[selectIdx].SetActive(true);
		}
	}
	
	public void OnReturnMain() {
		state = 0;
		
		commandsMenuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
		kingInfoAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
			
		gameObject.SetActive(true);
	}
	
	void SetCommandsDisableState() {
		state = 1;
		
		for (int m=0; m<commandsButton.Length; m++) {
			commandsButton[m].enabled = false;
		}
		
		isCommandDisable = true;
		//commandsButton[selectIdx].GetComponent<exSpriteFont>().botColor = new Color(1, 0, 0, 1);
		//commandsButton[selectIdx].GetComponent<exSpriteFont>().topColor = new Color(1, 0, 0, 1);
	}
	
	public void ResetState() {
		state = 0;
		
		for (int m=0; m<commandsButton.Length; m++) {
			commandsButton[m].enabled = true;
		}
		
		commandsButton[selectIdx].GetComponent<exSpriteFont>().botColor = new Color(1, 1, 1, 1);
		commandsButton[selectIdx].GetComponent<exSpriteFont>().topColor = new Color(1, 1, 1, 1);
	}
	
}
