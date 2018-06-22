using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HistoryTimeController : MonoBehaviour {
	
	public StrategyController strCtrl;
	public exSpriteFont font;
	
	private static int month = 0;
	
	private float timeTick = 0;
	private static int timeCount = 0;
	
	// Use this for initialization
	void Start () {
	
		timeTick = 0;
		
		SetText();
	}
	
	// Update is called once per frame
	void Update () {
		
		TimePassing();
	}
	
	void TimePassing() {
		
		if (StrategyController.state != StrategyController.State.Normal) return;
		
		timeTick += Time.deltaTime;
		
		if (timeTick >= 0.5f) {
			timeTick = 0;
			
			strCtrl.AddReservist();
			
			timeCount++;
			if (timeCount >= 10) {
				timeCount = 0;
				
				AddMonth();
			}
		}
	}
	
	void SetText() {
		
		font.text = ZhongWen.Instance.xiyuan + " " + Controller.historyTime + ZhongWen.Instance.nian;
		
		if (month < 9) {
			
			font.text += " " + (int)(month+1) + ZhongWen.Instance.yue;
		} else {
			
			font.text += "" +  (int)(month+1) + ZhongWen.Instance.yue;
		}
	}
	
	void AddMonth() {
		
		month++;
		if (month == 12) {
			month = 0;
			
			Controller.historyTime++;
			
			gameObject.SetActive(false);
			
			strCtrl.OnTimeOver();
			return;
		}
		
		SetText();
		
		strCtrl.MonthAct();
	}

	public static void Reset() {
		month = 0;
		timeCount = 0;
	}
}
