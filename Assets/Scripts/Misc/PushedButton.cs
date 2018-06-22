using UnityEngine;
using System.Collections;

public class PushedButton : MonoBehaviour {
	
	public enum ButtonState {
		Normal,
		Down,
		Pressed
	};
	
	public Color selectedColor = new Color(1, 0, 0, 1);
	
	private ButtonState state = ButtonState.Normal;
	private exSpriteFont fontScript;
	private object data = null;
	
	private string mText;
	public string text {
		
		get { return mText; }
		
		set {
			if (fontScript == null) {
				fontScript = GetComponent<exSpriteFont>();
			}
			
			mText = value;
			fontScript.text = mText;
		}
	}
	
	public delegate void MessageDelegate();
	private MessageDelegate buttonDownHandler = null;
	
	public delegate void MessageDelegate1(object d);
	private MessageDelegate1 buttonDownHandler1 = null;
	
	// Use this for initialization
	void Start () {
		if (fontScript == null) {
			fontScript = GetComponent<exSpriteFont>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.touchCount > 1) return;
		
		if (state == ButtonState.Down)
			state = ButtonState.Pressed;
		
		if (Input.GetMouseButtonDown(0)) {
			
			if (CheckIsHit()) {
				state = ButtonState.Down;
				
				fontScript.botColor = selectedColor;
				fontScript.topColor = selectedColor;
				
				if (buttonDownHandler != null) 		buttonDownHandler();
				if (buttonDownHandler1 != null) 	buttonDownHandler1(data);
			}
		}
	}
	
	bool CheckIsHit() {
		
		Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 point = new Vector2(mousePoint.x, mousePoint.y);
		
		Rect bound = fontScript.boundingRect;
		bound.x += transform.position.x;
		bound.y += transform.position.y;
		
		return bound.Contains(point);
	}
	
	public ButtonState GetButtonState() {
		return state;
	}
	
	public void SetButtonState(ButtonState s) {
		state = s;
		
		if (fontScript == null) {
			fontScript = GetComponent<exSpriteFont>();
		}
		
		if (state == ButtonState.Normal) {
			fontScript.botColor = new Color(1, 1, 1, 1);
			fontScript.topColor = new Color(1, 1, 1, 1);
		} else if (state == ButtonState.Down || state == ButtonState.Pressed) {
			fontScript.botColor = selectedColor;
			fontScript.topColor = selectedColor;
			
			if (buttonDownHandler != null) 		buttonDownHandler();
			if (buttonDownHandler1 != null) 	buttonDownHandler1(data);
		}
	}
	
	public void SetButtonData(object d) {
		data = d;
	}
	
	public object GetButtonData() {
		return data;
	}
	
	public void SetButtonDownHandler(MessageDelegate func) {
		buttonDownHandler = new MessageDelegate(func);
	}
	
	public void SetButtonDownHandler(MessageDelegate1 func) {
		buttonDownHandler1 = new MessageDelegate1(func);
	}
}
