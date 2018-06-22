using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public StrategyController strCtrl;
	
	public Button[] commands;
	public GameObject[] commandAct;
	
	public GameObject confirmBox;
	public Button okBtn;
	public Button cancelBtn;
	public exSprite bgSprite;
	
	private bool isQuitConfirmMode = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!isQuitConfirmMode) {
			if (Misc.GetBack()) {
				
				gameObject.SetActive(false);
				strCtrl.ReturnMainMode();
				return;
			}
			
			if (Input.GetMouseButtonUp(0)) {
				Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 point = new Vector2(mousePoint.x, mousePoint.y);
				
				Rect bound = bgSprite.boundingRect;
				bound.x += transform.position.x;
				bound.y += transform.position.y;
				
				if (!bound.Contains(point)) {
					
					gameObject.SetActive(false);
					Input.ResetInputAxes();
					strCtrl.ReturnMainMode();
					return;
				}
			}
			
			for (int i=0; i<4; i++) {
				if (commands[i].GetButtonState() == Button.ButtonState.Clicked) {
					if (i == 3) {
						isQuitConfirmMode = true;
						
						confirmBox.SetActive(true);
						
						Vector3 pos = commands[i].transform.position;
						if (pos.x + 100 + 128 < 320) {
							
							confirmBox.transform.position = new Vector3(pos.x + 100 + 64, pos.y + 10, confirmBox.transform.position.z);
						} else {
							
							confirmBox.transform.position = new Vector3(pos.x - 100 - 64, pos.y + 10, confirmBox.transform.position.z);
						}
					} else {
						commandAct[i].SetActive(true);
						gameObject.SetActive(false);
					}
				}
			}
		} else {
			if (Misc.GetBack()) {
				
				isQuitConfirmMode = false;
				confirmBox.SetActive(false);
				return;
			}
			
			if (okBtn.GetButtonState() == Button.ButtonState.Clicked) {

				Controller.historyTime = 190;
				HistoryTimeController.Reset();
				StrategyController.Reset();
				Informations.Instance.armys.Clear();

				Application.LoadLevel(0);
				GameObject.Destroy(GameObject.Find("MouseTrack"));
				
			} else if (cancelBtn.GetButtonState() == Button.ButtonState.Clicked) {
				
				isQuitConfirmMode = false;
				confirmBox.SetActive(false);
			}
		}
	}
}
