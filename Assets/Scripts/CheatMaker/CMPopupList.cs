using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMPopupList : MonoBehaviour {

	private static bool isShowDropList = false;

	public GameObject itemPrefab;

	public GameObject dropDownList;
	public GameObject gridRoot;

	public GameObject buttonPopupList;
	public UILabel labelSelection;

	public GameObject outSideCollider;
	
	private bool isMouseDown;
	private Vector3 mouseDownPos;

	private int selectIndex = -1;
	private List<GameObject> items;

	private float scaleFactor = Screen.height / 480f;

	public delegate bool OnSelectChangeCallBack(int index);
	private OnSelectChangeCallBack selectChangeCallBack;

	// Use this for initialization
	void Start () {

		dropDownList.SetActive(false);

		UIEventListener.Get(buttonPopupList).onClick += OnButtonPopupListClick;
		UIEventListener.Get(outSideCollider).onClick += OnOutSideClick;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnItemPressed(GameObject go, bool state) {
		if (state && !isMouseDown) {
			isMouseDown = true;
			mouseDownPos = Input.mousePosition;
		}
	}

	void OnItemClick(GameObject go) {
		isMouseDown = false;
		if (Vector3.Distance(mouseDownPos, Input.mousePosition) < 10 * scaleFactor) {
			int index = items.IndexOf(go);
			OnItemSelected(index);
		}
	}

	void OnButtonPopupListClick(GameObject go) {

		if (dropDownList.activeSelf == false) {
			if (!isShowDropList) {
				isShowDropList = true;
				dropDownList.SetActive(true);
			}
		} else {
			isShowDropList = false;
			dropDownList.SetActive(false);
		}
	}

	void OnOutSideClick(GameObject go) {
		isShowDropList = false;
		dropDownList.SetActive(false);
	}

	void OnItemSelected(int index) {

		if (index < 0) {
			labelSelection.text = "";
			if (selectIndex != -1) {
				items[selectIndex].GetComponent<UIButton>().defaultColor = new Color(1, 1, 1, 1);
				selectIndex = index;
			}
			return;
		}

		if (!selectChangeCallBack(index))
			return;

		isShowDropList = false;
		dropDownList.SetActive(false);
		labelSelection.text = items[index].GetComponent<UILabel>().text;
		items[index].GetComponent<UIButton>().defaultColor = new Color(0, 1, 0, 1);

		if (selectIndex == index) return;

		if (selectIndex != -1) {
			items[selectIndex].GetComponent<UIButton>().defaultColor = new Color(1, 1, 1, 1);
		}
		selectIndex = index;
	}

	public void SetItemSelect(int index) {
		OnItemSelected(index);
	}

	public void PopupListInit(List<string> itemsString, OnSelectChangeCallBack selectChangeCallBack) {

		items = new List<GameObject>();
		for (int i=0; i<itemsString.Count; i++) {
			GameObject go = Instantiate(itemPrefab) as GameObject;
			go.transform.parent = gridRoot.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			go.GetComponent<UILabel>().text = itemsString[i];

			items.Add(go);
			UIEventListener.Get(go).onClick += OnItemClick;
			UIEventListener.Get(go).onPress += OnItemPressed;
		}

		this.selectIndex = -1;
		this.selectChangeCallBack = selectChangeCallBack;
	}
}
