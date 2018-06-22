using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectTargetCity : MonoBehaviour {
	
	public StrategyController strCtrl;
	public MyPathfinding path;
	
	public FlagsController flagsCtrl;
	
	private ArmyInfo armyInfo;
	
	private bool isMouseMove;
	private Vector3 mouseDownPos;
	
	private Vector3 scale = new Vector3(640f/Screen.width, 480f/Screen.height, 0);
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Misc.GetBack()) {
			gameObject.SetActive(false);
			strCtrl.ReturnMainMode();
			return;
		}
		
		if (Input.GetMouseButtonDown(0)) {
				
			isMouseMove = false;
			mouseDownPos = Input.mousePosition;
			
		} else if (!isMouseMove && Input.GetMouseButtonUp(0)) {
			
			int targetCity = flagsCtrl.GetTouchCityIdx();
			if (targetCity != -1) {
				
				armyInfo.armyCtrl.SetRoute(path.GetRoute(armyInfo.armyCtrl.transform.position, targetCity));
				armyInfo.armyCtrl.SetArmyRunning();
				if (targetCity == armyInfo.cityFrom) {
					armyInfo.cityFrom = armyInfo.cityTo;
				}
				armyInfo.cityTo = targetCity;
				
				gameObject.SetActive(false);
				strCtrl.ReturnMainMode();

				Input.ResetInputAxes();
			}
		} else if (Input.GetMouseButton(0)) {
			
			if (!isMouseMove) {
				
				Vector3 offset = mouseDownPos - Input.mousePosition;
				offset.Scale(scale);
				
				if (Mathf.Abs(offset.x) > 5 || Mathf.Abs(offset.y) > 5) {
					
					isMouseMove = true;
					mouseDownPos = Input.mousePosition;
					/*
					Vector3 pos = Camera.main.transform.position;
					
					pos += offset;
					pos.x = Mathf.Clamp(pos.x, -320, 320);
					pos.y = Mathf.Clamp(pos.y, -240, 240);
					
					Camera.main.transform.position = pos;
					*/
				}
			} else {
				
				Vector3 offset = mouseDownPos - Input.mousePosition;
				mouseDownPos = Input.mousePosition;
				offset.Scale(scale);
					
				Vector3 pos = Camera.main.transform.position;
				
				pos += offset;
				pos.x = Mathf.Clamp(pos.x, -320, 320);
				pos.y = Mathf.Clamp(pos.y, -240, 240);
				
				Camera.main.transform.position = pos;
				
			}
		}
	}
	
	public void SetArmy(ArmyInfo a) {

		armyInfo = a;
		gameObject.SetActive(true);
	}
	
	
}
