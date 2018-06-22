using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController {

	public float effectVolume = 1f;
	public float musicVolume = 1f;

	private GameObject root;

	private string musicName;
	private GameObject musicGO;
	private Dictionary<string, GameObject> sounds = new Dictionary<string, GameObject>();

	private static SoundController mInstance;
	public static SoundController Instance
	{
		get {
			if (mInstance == null) {
				mInstance = new SoundController();
			}

			return mInstance;
		}
	}

	public void PlaySound(string soundName) {
		GetSound(soundName, effectVolume);
	}

	public void PlaySound3D(string soundName, Vector3 position) {
		GameObject obj = GetSound(soundName, effectVolume);
		obj.transform.position = position;
	}

	public void PlayBackgroundMusic(string musicName) {

		if (this.musicName == musicName) return;
		this.musicName = musicName;

		MusicController musicCtrl;
		if (musicGO != null) {
			musicCtrl = musicGO.AddComponent<MusicController>();
			musicCtrl.SetInfo(false, musicVolume);
		}

		musicGO = new GameObject("BackgroundMusic");
		AudioSource audioSource = musicGO.AddComponent<AudioSource>();
		audioSource.playOnAwake = true;
		audioSource.clip = Resources.Load<AudioClip>(musicName);
		audioSource.loop = true;
		audioSource.Play();

		musicCtrl = musicGO.AddComponent<MusicController>();
		musicCtrl.SetInfo(true, musicVolume);
		
		GameObject.DontDestroyOnLoad(musicGO);
	}

	public void SetEffectVolume(float volume) {

		effectVolume = volume;
		foreach(GameObject go in sounds.Values) {
			go.GetComponent<AudioSource>().volume = effectVolume;
		}
	}

	public void SetBackgroundMusicVolume(float volume) {

		musicVolume = volume;
		if (musicGO != null) {
			musicGO.GetComponent<AudioSource>().volume = musicVolume;
		}
	}

	GameObject GetSound(string soundName, float volume) {
		if (root == null) {
			root = new GameObject("Sounds");
		}

		GameObject obj;
		AudioSource audioSource;
		if (!sounds.ContainsKey(soundName) || sounds[soundName] == null) {
			obj = new GameObject(soundName);
			audioSource = obj.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.clip = Resources.Load<AudioClip>(soundName);
			audioSource.minDistance = 100;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.volume = volume;

			if (!sounds.ContainsKey(soundName)) {
				sounds.Add(soundName, obj);
			} else {
				sounds[soundName] = obj;
			}

			obj.transform.parent = root.transform;
		} else {
			audioSource = sounds[soundName].GetComponent<AudioSource>();
		}

		if (!audioSource.isPlaying) {
			audioSource.Play();
		}

		return sounds[soundName];
	}
}
