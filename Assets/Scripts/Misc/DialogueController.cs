using UnityEngine;
using System.Collections;

public class DialogueController : MonoBehaviour {
	
	public GeneralsHeadSelect headSelct;
	
	private exSpriteFont font;
	private MenuDisplayAnim menuAnim;
	
	private int state;
	private string text;
	private int textIdx;
	private bool isShowingText;
	
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable() {
		
		if (menuAnim == null) {
			menuAnim = GetComponent<MenuDisplayAnim>();
		}
		
		if (font == null) {
			font = transform.Find("Font").GetComponent<exSpriteFont>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			timeTick += Time.deltaTime;
			if (timeTick >= 0.2f) {
				state = 1;
				timeTick = 0;
				textIdx = 0;
			}
			break;
		case 1:
			
			if (Input.GetMouseButtonUp(0)) {
				Input.ResetInputAxes();
				
				state = 2;
				isShowingText = false;
				font.text = "";
				for (int i=0; i<text.Length; i++) {
					if (text[i] == ' ') continue;
					
					font.text += text[i];
				}
				break;
			}
			
			timeTick += Time.deltaTime;
			if (timeTick >= 0.05f) {
				timeTick = 0;
				
				while (text[textIdx] == ' ') textIdx++;
				
				font.text += text[textIdx];
				
				textIdx++;
				if (textIdx >= text.Length) {
					state = 2;
					isShowingText = false;
				}
			}
			break;
		case 2:
			
			break;
		case 3:
			if (!menuAnim.IsPlaying()) {
				state = -1;
			}
			break;
		case 1000:
			state = 1;
			break;
		}
	}
	
	public void SetText(string t) {
		isShowingText = true;
		state = 1000;
		textIdx = 0;
		timeTick = 0;
		
		if (font == null) {
			font = transform.Find("Font").GetComponent<exSpriteFont>();
		}
		font.text = "";
		
		text = t;
		//text.Replace("  ", "");
		
		Input.ResetInputAxes();
	}
	
	public void SetHeadIndex(int idx) {
		headSelct.SetGeneralHead(idx);
	}
	
	public bool IsShowingText() {
		return isShowingText;
	}
	
	public void SetDialogueInset(MenuDisplayAnim.AnimType type) {
		state = 0;
		timeTick = 0;
		
		if (menuAnim == null) {
			menuAnim = GetComponent<MenuDisplayAnim>();
		}
		
		menuAnim.SetAnim(type);
	}
	
	public void SetDialogueOut(MenuDisplayAnim.AnimType type) {
		state = 3;
		
		if (menuAnim == null) {
			menuAnim = GetComponent<MenuDisplayAnim>();
		}
		
		menuAnim.SetAnim(type);
	}
	
	public void SetDialogue(int gHeadIndex, string t, MenuDisplayAnim.AnimType animType) {
		gameObject.SetActive(true);
		
		SetHeadIndex(gHeadIndex);
		
		SetText(t);
		
		SetDialogueInset(animType);
	}
}
