using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	private bool isShow;
	private float volume;

	private float timeTick;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		timeTick += Time.deltaTime;
		if (timeTick < 0.5f) {
			if (isShow) {
				GetComponent<AudioSource>().volume = Mathf.Lerp(0, volume, timeTick * 2);
			} else {
				GetComponent<AudioSource>().volume = Mathf.Lerp(volume, 0, timeTick * 2);
			}
		} else {
			if (isShow) {
				GetComponent<AudioSource>().volume = volume;
				Destroy(this);
			} else {
				Destroy(gameObject);
			}
		}
	}

	public void SetInfo(bool isShow, float volume) {

		this.isShow = isShow;
		this.volume = volume;

		if (isShow) {
			GetComponent<AudioSource>().volume = 0;
		}
	}
}
