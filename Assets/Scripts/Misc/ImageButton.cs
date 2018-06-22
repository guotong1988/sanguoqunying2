using UnityEngine;
using System.Collections;

public class ImageButton : MonoBehaviour {
	
	public enum ButtonState {
		Normal,
		Pressed,
		Clicked
	};
	
	private GameObject normal;
	private GameObject pressed;
	
	private ButtonState state;
	
	public delegate void MessageDelegate();
	private MessageDelegate buttonDownHandler = null;
	private MessageDelegate buttonClickHandler = null;
	
	// Use this for initialization
	void Start () {
		normal = transform.Find("Normal").gameObject;
		pressed = transform.Find("Pressed").gameObject;
		
		if (normal.GetComponent<BoxCollider>() == null) {
			normal.AddComponent<BoxCollider>();
		}
		if (pressed.GetComponent<BoxCollider>() == null) {
			pressed.AddComponent<BoxCollider>();
		}
		
		normal.SetActive(true);
		pressed.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 1) return;
		
		if (state == ButtonState.Clicked)
			state = ButtonState.Normal;
		
		if (Input.GetMouseButtonDown(0)) {
			
			if (CheckIsHit(normal)) {
				state = ButtonState.Pressed;
				
				normal.SetActive(false);
				pressed.SetActive(true);
				
				if (buttonDownHandler != null)		buttonDownHandler();
			}
			
		} else if (state == ButtonState.Pressed && Input.GetMouseButtonUp(0)) {
			
			if (CheckIsHit(pressed)) {
				state = ButtonState.Clicked;
				SoundController.Instance.PlaySound("00038");
				
				if (buttonClickHandler != null)		buttonClickHandler();
				Input.ResetInputAxes();
			}	else {
				state = ButtonState.Normal;
			}
			
			normal.SetActive(true);
			pressed.SetActive(false);
		}
	}
	
	bool CheckIsHit(GameObject go) {
		
		Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 point = new Vector2(mousePoint.x, mousePoint.y);
		
		Rect bound = go.GetComponent<exSprite>().boundingRect;
		bound.x += transform.position.x;
		bound.y += transform.position.y;
		
		return bound.Contains(point);
	}
	
	public ButtonState GetState() {
		return state;
	}
	
	public void SetButtonDownHandler(MessageDelegate func) {
		buttonDownHandler = new MessageDelegate(func);
	}
	
	public void SetButtonClickHandler(MessageDelegate func) {
		buttonClickHandler = new MessageDelegate(func);
	}
}
