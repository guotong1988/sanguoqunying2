using UnityEngine;
using System.Collections;

public class VictoryDialogController : MonoBehaviour {

	public exSpriteFont showText;
	private int state;
	private string text;
	private int index;
	private float timeTick;

	// Use this for initialization
	void Start () {
		state = 0;
		index = 0;
		showText.text = "";
		text = ZhongWen.Instance.yu + ZhongWen.Instance.xiyuan + Controller.historyTime.ToString()
			+ ZhongWen.Instance.nian + ZhongWen.Instance.daohao 
				+ ZhongWen.Instance.GetKingName(Controller.kingIndex) + ZhongWen.Instance.tongyile;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == 0) {
			timeTick += Time.deltaTime;
			if (timeTick > 0.5f) {
				timeTick = 0;
				index++;
				showText.text = text.Substring(0, index);
				if (index >= text.Length) {
					state = 1;
				}
			}
		} else if (state == 1) {
			if (Input.GetMouseButtonDown(0)) {
				state = 2;
				Misc.LoadLevel("GameVictory");
			}
		}
	}

}
