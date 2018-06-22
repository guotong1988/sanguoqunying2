using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	
	public enum ButtonState {
		Normal,
		Down,
		Pressed,
		Leave,
		Clicked
	};
	
	private ButtonState state = ButtonState.Normal;
	private exSpriteFont fontScript;
	private object data = null;
	
	public delegate void MessageDelegate();
	private MessageDelegate buttonDownHandler = null;
	private MessageDelegate buttonPressHandler = null;
	private MessageDelegate buttonClickHandler = null;
	
	public delegate void MessageDelegate1(object d);
	private MessageDelegate1 buttonClickHandler1 = null;
	
	// Use this for initialization
	void Start () {
		if (fontScript == null) {
			fontScript = GetComponent<exSpriteFont>();
		}
	}
	
	void OnDisable() {
		SetButtonState(ButtonState.Normal);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.touchCount > 1) return;
		
		if (state == ButtonState.Clicked)
			state = ButtonState.Normal;
		
		if (Input.GetMouseButtonDown(0)) {
			
			if (CheckIsHit()) {
				
				state = ButtonState.Down;
				fontScript.botColor = new Color(1, 0, 0, 1);
				fontScript.topColor = new Color(1, 0, 0, 1);
				
				if (buttonDownHandler != null)		buttonDownHandler();
			}
		} else if (Input.GetMouseButtonUp(0)) {
			if (state == ButtonState.Down || state == ButtonState.Pressed) {
				
				if (CheckIsHit()) {
					state = ButtonState.Clicked;
					fontScript.botColor = new Color(1, 1, 1, 1);
					fontScript.topColor = new Color(1, 1, 1, 1);

					if (buttonClickHandler != null)		buttonClickHandler();
					if (buttonClickHandler1 != null)	buttonClickHandler1(data);

					Input.ResetInputAxes();
					SoundController.Instance.PlaySound("00038");
					return;
				}
			}
			
			state = ButtonState.Normal;
		} else if (Input.GetMouseButton(0)) {
			if (state == ButtonState.Down || state == ButtonState.Pressed) {
				
				if (CheckIsHit()) {
					state = ButtonState.Pressed;
					
					if (buttonPressHandler != null)		buttonPressHandler();
				} else {
					state = ButtonState.Leave;
					
					fontScript.botColor = new Color(1, 1, 1, 1);
					fontScript.topColor = new Color(1, 1, 1, 1);
				}
			} else if (state == ButtonState.Leave) {
				
				if (CheckIsHit()) {
					state = ButtonState.Pressed;
					
					if (buttonPressHandler != null)		buttonPressHandler();
					
					fontScript.botColor = new Color(1, 0, 0, 1);
					fontScript.topColor = new Color(1, 0, 0, 1);
				}
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
	
	public void SetButtonEnable(bool flag) {
		
		enabled = flag;
		
		state = ButtonState.Normal;
		
		if (fontScript == null) {
			fontScript = GetComponent<exSpriteFont>();
		}
		
		if (enabled) {
			fontScript.botColor = new Color(1, 1, 1, 1);
			fontScript.topColor = new Color(1, 1, 1, 1);
		} else {
			fontScript.botColor = new Color(0.5f, 0.5f, 0.5f, 1);
			fontScript.topColor = new Color(0.5f, 0.5f, 0.5f, 1);
		}
	}
	
	public ButtonState GetButtonState() {
		return state;
	}
	
	public void SetButtonState(ButtonState s) {
		
		state = s;
		
		if (fontScript == null) {
			fontScript = GetComponent<exSpriteFont>();
		}
		
		if (!enabled) return;
		
		if (state == ButtonState.Normal || state == ButtonState.Leave || state == ButtonState.Clicked) {
			fontScript.botColor = new Color(1, 1, 1, 1);
			fontScript.topColor = new Color(1, 1, 1, 1);
		} else if (state == ButtonState.Down || state == ButtonState.Pressed) {
			fontScript.botColor = new Color(1, 0, 0, 1);
			fontScript.topColor = new Color(1, 0, 0, 1);
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
	
	public void SetButtonPressHandler(MessageDelegate func) {
		buttonPressHandler = new MessageDelegate(func);
	}
	
	public void SetButtonClickHandler(MessageDelegate func) {
		buttonClickHandler = new MessageDelegate(func);
	}
	
	public void SetButtonClickHandler(MessageDelegate1 func) {
		buttonClickHandler1 = new MessageDelegate1(func);
	}
}
