using UnityEngine;
using System.Collections;

public class StartSceneMenu : MonoBehaviour {

	public GameObject volumeConfigPanel;

	public Button startGame;
	public Button continueGame;
	public Button volumeConfig;
	public Button quitGame;

	public Button cheatMaker;

	private MenuDisplayAnim menuAnim;
	
	// Use this for initialization
	void Start () {
		menuAnim = GetComponent<MenuDisplayAnim>();
		SetAnimIn();

		if (PlayerPrefs.HasKey("effectVolume")) {
			SoundController.Instance.effectVolume = PlayerPrefs.GetInt("effectVolume") / 100f;
		} else {
			SoundController.Instance.effectVolume = 1;
		}
		if (PlayerPrefs.HasKey("musicVolume")) {
			SoundController.Instance.musicVolume = PlayerPrefs.GetInt("musicVolume") / 100f;
		} else {
			SoundController.Instance.musicVolume = 1;
		}

		SoundController.Instance.PlayBackgroundMusic("Music01");
	}
	
	// Update is called once per frame
	void Update () {
		if (startGame != null && startGame.GetButtonState() == Button.ButtonState.Clicked) {
			Misc.LoadLevel("SelectKing");
		} else if (continueGame != null && continueGame.GetButtonState() == Button.ButtonState.Clicked) {
			ContinueGame.isCheatMaker = false;
			Misc.LoadLevel("ContinueGame");
		} else if (volumeConfig != null && volumeConfig.GetButtonState() == Button.ButtonState.Clicked) {
			SetAnimOut();
		} else if (quitGame != null && quitGame.GetButtonState() == Button.ButtonState.Clicked) {
			Application.Quit();
		} else if (cheatMaker.GetButtonState() == Button.ButtonState.Clicked) {
			ContinueGame.isCheatMaker = true;
			Misc.LoadLevel("ContinueGame");
		}
	}

	public void SetAnimIn() {
		gameObject.SetActive(true);
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}

	public void SetAnimOut() {
		volumeConfigPanel.SetActive(true);
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
	}
}
