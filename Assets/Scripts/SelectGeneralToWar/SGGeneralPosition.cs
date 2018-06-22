using UnityEngine;
using System.Collections;

public class SGGeneralPosition : MonoBehaviour {
	
	public SelectGeneralToWarController sgCtrl;
	
	public Button front;
	public Button back;
	public MenuDisplayAnim menuAnim;
	
	// Use this for initialization
	void Start () {
		
		front.SetButtonClickHandler(OnClickFront);
		back.SetButtonClickHandler(OnClickBack);
	}
	
	void OnEnable() {
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!menuAnim.IsPlaying()) {
			if (Misc.GetBack()) {
				menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
				Invoke("ReturnMain", 0.2f);
			}
		}
	}
	
	void OnClickFront() {
		
		sgCtrl.OnSelectGeneralPosition(1);
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		Invoke("ReturnMain", 0.2f);
	}
	
	void OnClickBack() {
		
		sgCtrl.OnSelectGeneralPosition(0);
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		Invoke("ReturnMain", 0.2f);
	}
	
	void ReturnMain() {
		
		gameObject.SetActive(false);
		
		sgCtrl.OnReturnMain();
	}
}
