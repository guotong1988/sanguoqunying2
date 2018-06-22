using UnityEngine;
using System.Collections;

public class SliderHController : MonoBehaviour {
	
	public Transform slider;
	public ImageButton leftButton;
	public ImageButton rightButton;
	
	private Vector3 mouseDownPos;
	
	private int curValue = 0;
	private int maxValue = 0;
	
	private bool isMouseDown = false;
	private float lenOrg = 0;
	private float len;
	private float stepOrg = 10;
	private float xSpace = 10;
	private Vector3 orgPos = Vector3.zero;
	
	public delegate void MessageDelegate(int offset);
	private MessageDelegate sliderMoveHandler = null;
	
	// Use this for initialization
	void Start () {
		
		leftButton	.SetButtonClickHandler(OnButtonLeftController);
		rightButton	.SetButtonClickHandler(OnButtonRightController);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (slider.GetComponent<Collider>().Raycast (ray, out hit, 1000.0f)) {
				isMouseDown = true;
				mouseDownPos = Input.mousePosition;
			}
		} else if (isMouseDown && Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
		} else if (isMouseDown && Input.GetMouseButton(0)) {

			float scaleX = 640f / Screen.width;
			float xOff = Input.mousePosition.x - mouseDownPos.x;
			xOff *= scaleX;
			if (Mathf.Abs(xOff) > xSpace)  {
				int step = (int)(xOff / xSpace);
				SliderMove(step);
				mouseDownPos.x += step * xSpace / scaleX;
			}
		}
	}
	
	void SliderMove(int offset) {
		
		int temp = curValue;
		temp += offset;
		temp = Mathf.Clamp(temp, 0, maxValue);
		
		if (temp == curValue)
			return;
		
		offset = temp - curValue;
		curValue = temp;
		
		slider.Translate(xSpace * offset, 0, 0);
		
		if (sliderMoveHandler != null) {
			sliderMoveHandler(offset);
		}
	}
	
	public int GetCurValue() {
		
		return curValue;
	}
	
	public void SetSlider(int max, int cur) {
		
		if (cur > max)
			cur = max;
		
		maxValue = max;
		curValue = cur;
		
		if (lenOrg == 0) {
			lenOrg = slider.GetComponent<exSprite>().width;
			len = lenOrg;
		}
		
		if (max <= 0) {
			len = lenOrg;
		} else {
			len = lenOrg - max * stepOrg;
			
			if (len < 10) {
				len = 10;
				xSpace = (lenOrg - len) / max;
			} else {
				xSpace = stepOrg;
			}
		}
		
		slider.GetComponent<exSprite>().width = len;
		
		if (orgPos == Vector3.zero)
			orgPos = slider.localPosition;
		slider.localPosition = new Vector3(orgPos.x + cur * xSpace, slider.localPosition.y, slider.localPosition.z);
		
		Vector3 colSize = slider.GetComponent<BoxCollider>().size;
		Vector3 colCenter = slider.GetComponent<BoxCollider>().center;
		
		colCenter.x = len / 2;
		colSize.x = len;
		
		slider.GetComponent<BoxCollider>().size = colSize;
		slider.GetComponent<BoxCollider>().center = colCenter;
	}
	
	public void SetSliderEnable(bool flag) {
		
		enabled 			= flag;
		
		leftButton.enabled 	= flag;
		rightButton.enabled = flag;
	}
	
	void OnButtonLeftController() {
		SliderMove(-1);
	}
	
	void OnButtonRightController() {
		SliderMove(1);
	}
	
	public void SetSliderMoveHandler(MessageDelegate func) {
		sliderMoveHandler = new MessageDelegate(func);
	}
}
