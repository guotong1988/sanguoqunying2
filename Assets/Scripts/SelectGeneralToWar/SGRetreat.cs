using UnityEngine;
using System.Collections;

public class SGRetreat : MonoBehaviour {
	
	public SelectGeneralToWarController sgCtrl;
	
	public Button okButton;
	public Button cancelButton;
	
	// Use this for initialization
	void Start () {
		
		okButton.SetButtonClickHandler(OnOKButton);
		cancelButton.SetButtonClickHandler(OnCancelButton);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Misc.GetBack()) {
			OnCancelButton();
		}
	}
	
	void OnOKButton() {
		
		gameObject.SetActive(false);
		sgCtrl.OnRetreat();
	}
	
	void OnCancelButton() {
		
		gameObject.SetActive(false);
		sgCtrl.OnReturnMain();
	}
}
