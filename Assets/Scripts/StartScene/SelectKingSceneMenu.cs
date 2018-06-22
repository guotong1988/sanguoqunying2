using UnityEngine;
using System.Collections;

public class SelectKingSceneMenu : MonoBehaviour {

    public SelectKingSceneController ssCtrl;

	public GameObject confirmBox;
	public Button btnOK;
	public Button btnCancel;
	
	public PushedButton[] kingName;
	public GameObject[] information;
	public exSprite[] city;
	public int[] cityNum;
	
	private int kingIndex = 0;
	private bool isConfirmBoxShow = false;
	
	private Color gray = new Color(0.5f, 0, 0, 1);
	
	// Use this for initialization
	void Start () {
		confirmBox.SetActive(false);
		
		SelectKing(0);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<kingName.Length; i++) {
			if (kingName[i].GetButtonState() == PushedButton.ButtonState.Down) {
				SelectKing(i);
				
				if (!isConfirmBoxShow) {
					confirmBox.SetActive(true);
					isConfirmBoxShow = true;
				}
				break;
			}
		}
		
		if (isConfirmBoxShow) {
			if (btnOK.GetButtonState() == Button.ButtonState.Clicked) {
				Controller.kingIndex = kingIndex;

				Informations.Reset();
				StrategyController.isFirstEnter = true;
				Misc.LoadLevel("InternalAffairs");
			} else if (btnCancel.GetButtonState() == Button.ButtonState.Clicked || Misc.GetBack()) {
				isConfirmBoxShow = false;
				confirmBox.SetActive(false);
				
				btnCancel.SetButtonState(Button.ButtonState.Normal);
			}
		} else {
			
			if (Misc.GetBack()) {
                if (PlayerPrefs.HasKey("GamePass")) {
                    ssCtrl.SetSelectMOD();
                } else {
                    Misc.LoadLevel("StartScene");
                    GameObject.Destroy(GameObject.Find("MouseTrack"));
                }
			}
		}
	}
	
	void SelectKing(int index) {
		kingIndex = index;
		
		int len = kingName.Length;
		if (index >= len) 
			return;
		
		int cityIdx = 0;
		for (int i=0; i<len; i++) {
			if (i == index) {
				kingName[i].SetButtonState(PushedButton.ButtonState.Pressed);
				information[i].SetActive(true);
				
				for (int j=0; j<cityNum[i]; j++,cityIdx++) {
					city[cityIdx].color = new Color(1, 1, 1, 1);
				}
			} else {
				kingName[i].SetButtonState(PushedButton.ButtonState.Normal);
				information[i].SetActive(false);
				
				for (int j=0; j<cityNum[i]; j++,cityIdx++) {
					city[cityIdx].color = new Color(gray.r, gray.g, gray.b, gray.a);
				}
			}
		}
		
		confirmBox.transform.position = new Vector3(confirmBox.transform.position.x, 160 - 30 * index, confirmBox.transform.position.z);
	}
}

