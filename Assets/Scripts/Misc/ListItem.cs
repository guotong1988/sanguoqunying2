using UnityEngine;
using System.Collections;

public class ListItem : MonoBehaviour {
	
	private bool isSelected = false;
	private bool isMouseDown = false;
	private bool selectEnable = true;
	private exSpriteFont font;
	
	private object itemData = null;
	
	// Use this for initialization
	void Start () {
		if (font == null) {
			font = GetComponent<exSpriteFont>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetSelected(bool flag) {
		if (font == null) {
			font = GetComponent<exSpriteFont>();
		}
		
		if (!selectEnable)	return;
		
		isSelected = flag;
		if (isSelected) {
			font.botColor = new Color(0, 1, 0, 1);
			font.topColor = new Color(0, 1, 0, 1);
		} else {
			font.botColor = new Color(1, 1, 1, 1);
			font.topColor = new Color(1, 1, 1, 1);
		}
	}
	
	public void SetMouseDown(bool flag) {
		if (font == null) {
			font = GetComponent<exSpriteFont>();
		}
		
		if (!selectEnable) return;
		
		isMouseDown = flag;
		
		if (isMouseDown) {
			font.botColor = new Color(1, 0, 0, 1);
			font.topColor = new Color(1, 0, 0, 1);
		} else {
			font.botColor = new Color(1, 1, 1, 1);
			font.topColor = new Color(1, 1, 1, 1);
		}
	}
	
	public void SetSelectEnable(bool flag) {
		if (font == null) {
			font = GetComponent<exSpriteFont>();
		}
		
		selectEnable = flag;
		isSelected = false;
		
		if (selectEnable) {
			font.botColor = new Color(1, 1, 1, 1);
			font.topColor = new Color(1, 1, 1, 1);
		} else {
			font.botColor = new Color(0.5f, 0.5f, 0.5f, 1);
			font.topColor = new Color(0.5f, 0.5f, 0.5f, 1);
		}
	}
	
	public bool GetSelectEnable() {
		return selectEnable;
	}
	
	public object GetItemData() {
		return itemData;
	}
	
	public void SetItemData(object data) {
		itemData = data;
	}
	
	
}