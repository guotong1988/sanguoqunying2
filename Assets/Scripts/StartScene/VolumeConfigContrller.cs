using UnityEngine;
using System.Collections;

public class VolumeConfigContrller : MonoBehaviour {

	public StartSceneMenu startMenu;

	public SliderHController effectVolume;
	public SliderHController musicVolume;

	private MenuDisplayAnim menuAnim;

	// Use this for initialization
	void Start () {

		if (PlayerPrefs.HasKey("effectVolume")) {
			effectVolume.SetSlider(100, PlayerPrefs.GetInt("effectVolume"));
		} else {
			effectVolume.SetSlider(100, 100);
		}
		effectVolume.SetSliderMoveHandler(OnEffectSliderHandler);

		if (PlayerPrefs.HasKey("musicVolume")) {
			musicVolume.SetSlider(100, PlayerPrefs.GetInt("musicVolume"));
		} else {
			musicVolume.SetSlider(100, 100);
		}
		musicVolume.SetSliderMoveHandler(OnMusicSliderHandler);
	}

	void OnEnable() {
		if (menuAnim == null) {
			menuAnim = GetComponent<MenuDisplayAnim>();
		}
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
	}

	// Update is called once per frame
	void Update () {

		if (!menuAnim.IsPlaying() && Misc.GetBack()) {
			OnReturn();
		}
	}

	void OnEffectSliderHandler(int offset) {
		
		int v = effectVolume.GetCurValue();

		SoundController.Instance.SetEffectVolume(v / 100f);
	}

	void OnMusicSliderHandler(int offset) {

		int v = musicVolume.GetCurValue();

		SoundController.Instance.SetBackgroundMusicVolume(v / 100f);
	}

	void OnReturn() {

		PlayerPrefs.SetInt("effectVolume", effectVolume.GetCurValue());
		PlayerPrefs.SetInt("musicVolume", musicVolume.GetCurValue());
		PlayerPrefs.Save();

		startMenu.SetAnimIn();

		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
	}
}
