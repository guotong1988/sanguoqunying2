using UnityEngine;
using System.Collections;

public class SGSelectFormation : MonoBehaviour {
	
	public SelectGeneralToWarController sgCtrl;
	
	public Transform token;
	public Button[] formations;
	
	private int generalIdx;
	private MenuDisplayAnim menuAnim;
	
	// Use this for initialization
	void Start () {
		
		if (menuAnim == null)
			menuAnim = GetComponent<MenuDisplayAnim>();
		
		for (int i=0; i<formations.Length; i++) {
			
			formations[i].SetButtonData(i);
			formations[i].SetButtonClickHandler(OnButtonClick);
		}
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
	
	void OnButtonClick(object idx) {
		
		int i = (int)idx;
		
		Informations.Instance.GetGeneralInfo(generalIdx).formationCur = 1 << i;
		token.position = new Vector3(token.position.x, formations[i].transform.position.y, token.position.z);
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
		Invoke("ReturnMain", 0.2f);
		
		sgCtrl.UpdateGeneralInfo();
	}
	
	void ReturnMain() {
		
		gameObject.SetActive(false);
		
		sgCtrl.OnReturnMain();
	}
	
	public void SetGeneral(int gIdx) {
		
		generalIdx = gIdx;
		
		if (menuAnim == null)
			menuAnim = GetComponent<MenuDisplayAnim>();
		
		menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		for (int i=0; i<formations.Length; i++) {
			
			if ((gInfo.formation & (1 << i)) == 0) {
				
				formations[i].SetButtonEnable(false);
			} else {
				
				formations[i].SetButtonEnable(true);
				if (gInfo.formationCur == (1 << i)) {
					token.position = new Vector3(token.position.x, formations[i].transform.position.y, token.position.z);
				}
			}
		}
	}
}
