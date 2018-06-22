using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMKingPanel : MonoBehaviour {
	
	public TweenPosition tweenPos;

	private List<int> kingIdxList = new List<int>();

	// Use this for initialization
	void Start () {

		int controllerIndex = 0;
		List<string> items = new List<string>();
		for (int i=0; i<Informations.Instance.kingNum; i++) {
			if (Informations.Instance.GetKingInfo(i).active == 1) {
				string str = ZhongWen.Instance.GetKingName(i);
				if (str.Length == 2) {
					str = str[0] + "  " + str[1];
				}
				items.Add(str);
				kingIdxList.Add(i);

				if (i == Controller.kingIndex)
					controllerIndex = kingIdxList.Count - 1;
			}
		}

		GameObject go = CheatMakerController.Instance.GetPopupList(items, OnSelectChange);
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;

		go.GetComponent<CMPopupList>().SetItemSelect(controllerIndex);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool OnSelectChange(int index) {

		Controller.kingIndex = kingIdxList[index];
		return true;
	}

	void OnEnter() {
		tweenPos.Play(true);
	}
	
	void OnReturn() {
		tweenPos.Play(false);
	}

	public void OnTweenOver() {
		if (tweenPos.transform.localPosition == tweenPos.from) {
			CheatMakerController.Instance.ChangeState(CheatMakerController.State.MainMenu);
		}
	}
}
