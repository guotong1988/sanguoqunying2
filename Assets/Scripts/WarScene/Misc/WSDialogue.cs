using UnityEngine;
using System.Collections;

public class WSDialogue : MonoBehaviour {
	
	private exSpriteFont font;
	
	private string text;
	private bool isShowingText;
	private int textIdx;
	private float timeTick;
	
	private float offset = 50;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (isShowingText) {
			if (Input.GetMouseButtonUp(0)) {
				Input.ResetInputAxes();
				
				isShowingText = false;
				font.text = "";
				for (int i=0; i<text.Length; i++) {
					if (text[i] == ' ') continue;
					
					font.text += text[i];
				}
				return;
			}
			
			if ((Time.realtimeSinceStartup - timeTick) >= 0.05f) {
				timeTick = Time.realtimeSinceStartup;
				
				while (text[textIdx] == ' ') textIdx++;
				
				font.text += text[textIdx];
				
				textIdx++;
				if (textIdx >= text.Length) {
					isShowingText = false;
				}
			}
		}
	}
	
	public void SetDialogue(string t, WarSceneController.WhichSide side) {
		
		gameObject.SetActive(true);
		isShowingText = true;
		text = t;
		textIdx = 0;
		timeTick = Time.realtimeSinceStartup;
		
		if (font == null) {
			font = transform.Find("Font").GetComponent<exSpriteFont>();
		}
		font.text = "";
		
		if (side == WarSceneController.WhichSide.Left) {
			transform.localPosition = new Vector3(offset, transform.localPosition.y, transform.localPosition.z);
		} else {
			transform.localPosition = new Vector3(-offset, transform.localPosition.y, transform.localPosition.z);
		}
	}
	
	public bool IsShowingText() {
		return isShowingText;
	}
}
