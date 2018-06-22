using UnityEngine;
using System.Collections;

public class MagicInfoShow : MonoBehaviour {

	public exSpriteFont targetInfo;
	public exSpriteFont expInfo;

	private MenuDisplayAnim menuAnim;

	private float timeStartup;

	void Update () {
		if (!menuAnim.IsPlaying() && Time.realtimeSinceStartup - timeStartup > 2) {
			menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);
		}
	}

	public void SetInfo(MagicDataInfo info) {
		gameObject.SetActive(true);

		if (menuAnim == null)
			menuAnim = GetComponent<MenuDisplayAnim>();
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);

		targetInfo.text = info.ACTIVE;
		expInfo.text = info.MP.ToString();

		timeStartup = Time.realtimeSinceStartup;
	}
}
