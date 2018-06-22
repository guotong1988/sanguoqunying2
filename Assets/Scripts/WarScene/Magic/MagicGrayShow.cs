using UnityEngine;
using System.Collections;

public class MagicGrayShow : MonoBehaviour {

	private exSprite spriteBG;

	private bool isGray;
	private float timeTick;
	private float grayValue = 80;

	// Use this for initialization
	void Start () {
		this.enabled = false;
		spriteBG = transform.GetChild(0).GetComponent<exSprite>();
		MagicController.Instance.grayCtrl = this;
	}
	
	// Update is called once per frame
	void Update () {

		timeTick += Time.deltaTime;
		if (isGray) {
			if (timeTick < 0.5f) {
				spriteBG.color = new Color(0, 0, 0, Mathf.Lerp(0, grayValue / 255f, timeTick * 2));
			} else {
				timeTick = 0;
				this.enabled = false;
				spriteBG.color = new Color(0, 0, 0, grayValue / 255f);
			}
		} else {
			if (timeTick < 0.5f) {
				spriteBG.color = new Color(0, 0, 0, Mathf.Lerp(grayValue / 255f, 0, timeTick * 2));
			} else {
				timeTick = 0;
				this.enabled = false;
				spriteBG.color = new Color(0, 0, 0, 0);
			}
		}
	}

	public void SetGray(bool isGray) {
		this.enabled = true;
		this.isGray = isGray;
		spriteBG.gameObject.SetActive(true);

		if (isGray) {
			spriteBG.color = new Color(0, 0, 0, 0);
		} else {
			spriteBG.color = new Color(0, 0, 0, grayValue / 255f);
		}
	}
}
