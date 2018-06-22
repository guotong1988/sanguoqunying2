using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListController : MonoBehaviour {
	
	public Transform slider;
	public GameObject itemPrefab;
	public exPlane.Anchor fontAnchor = exPlane.Anchor.MidLeft;
	public int visibleItemNum;
	public bool isClickItem = false;
	
	private List<ListItem> list = new List<ListItem>();
	
	private int topIdx = 0;
	private int selectIdx = -1;
	private int mouseDownIdx = -1;
	private bool canSelect = true;
	private bool isSelectChanged = false;
	private float ySpace = 30;
	
	private bool isMouseDown = false;
	private bool isMouseUp = false;
	private bool isMove = false;
	private Vector3 mouseDownPos;
	
	private bool isSliderMouseDown = false;
	private float sliderLenOrg = 0;
	private float sliderLen;
	private float sliderStepOrg = 10;
	private float sliderStep = 10;
	private Vector3 sliderOrgPos;
	
	private float scaleY = 480f / Screen.height;
	
	public delegate void MessageDelegate();
	private MessageDelegate selectItemHandler = null;
	private MessageDelegate selectChangeHandler = null;
	
	// Use this for initialization
	void Start () {
		slider.parent.Find("ArrowUp").GetComponent<ImageButton>().SetButtonClickHandler(OnArrowUpClickHandler);
		slider.parent.Find("ArrowDown").GetComponent<ImageButton>().SetButtonClickHandler(OnArrowDownClickHandler);
	}
	
	// Update is called once per frame
	void Update () {
		
		HandleTouch();
	}
	
	public void Clear() {
		topIdx = 0;
		selectIdx = -1;
	
		isMouseDown = false;
		isMouseUp = false;
		isMove = false;
		
		sliderStep = sliderStepOrg;
		
		for (int i=0; i<list.Count; i++) {
			ListItem li = list[i];
			if (li != null)
				Destroy(li.gameObject);
		}
		list.Clear();
	}
	
	void HandleListTouch() {
		if (isMouseUp) isMouseUp = false;
		if (isSelectChanged) isSelectChanged = false;
		if (isClickItem && selectIdx != -1) {
			list[selectIdx].SetSelected(false);
			selectIdx = -1;
		}
		
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (GetComponent<Collider>().Raycast (ray, out hit, 1000.0f)) {
				isMouseDown = true;
				isMove = false;
				mouseDownPos = Input.mousePosition;
				
				if (!canSelect) return;
				
				Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 point = new Vector2(mousePoint.x, mousePoint.y);
				for (int i=topIdx; i<topIdx+visibleItemNum; i++) {
					if (i >= list.Count) break;
					
					GameObject go = list[i].gameObject;
					if (!list[i].GetSelectEnable()) continue;
					
					exSpriteFont fontScript = go.GetComponent<exSpriteFont>();
					Rect bound = fontScript.boundingRect;
					bound.x += go.transform.position.x;
					bound.y += go.transform.position.y;
					if (bound.Contains(point)) {
						//if (selectIdx != i) 
						{
							mouseDownIdx = i;
							list[mouseDownIdx].SetMouseDown(true);
						}
						break;
					}
				}
			}
		} else if (isMouseDown && Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
			isMouseUp = true;
			if (mouseDownIdx != -1) {
				list[mouseDownIdx].SetMouseDown(false);
				list[mouseDownIdx].SetSelected(true);
				
				if (selectIdx != -1 && selectIdx != mouseDownIdx) {
					isSelectChanged = true;
					
					list[selectIdx].SetSelected(false);
				}
				
				selectIdx = mouseDownIdx;
				mouseDownIdx = -1;
				
				if (isSelectChanged) {
					if (selectChangeHandler != null) {
						selectChangeHandler();
					}
				}
				
				if (selectItemHandler != null) {
					selectItemHandler();
				}
			}
		} else if (isMouseDown && Input.GetMouseButton(0)) {
			
			float yOff = Input.mousePosition.y - mouseDownPos.y;
			yOff *= scaleY;
			int step = (int)(yOff / ySpace);
			if (!isMove) {
				if (Mathf.Abs(yOff) > ySpace / 2)  {
					isMove = true;
					
					if (mouseDownIdx != -1) {
						list[mouseDownIdx].SetMouseDown(false);
						mouseDownIdx = -1;
					}
					
					if (Mathf.Abs(yOff) > ySpace)  {
						SlideListItem(step);
						mouseDownPos.y += step * ySpace / scaleY;
					}
				}
			} else {
				if (Mathf.Abs(yOff) > ySpace)  {
					SlideListItem(step);
					mouseDownPos.y += (step) * ySpace / scaleY;
				}
			}
		}
	}
	
	void HandleSliderTouch() {
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (slider.GetComponent<Collider>().Raycast (ray, out hit, 1000.0f)) {
				isSliderMouseDown = true;
				mouseDownPos = Input.mousePosition;
			}
		} else if (isSliderMouseDown && Input.GetMouseButtonUp(0)) {
			isSliderMouseDown = false;
		} else if (isSliderMouseDown && Input.GetMouseButton(0)) {
			
			float yOff = Input.mousePosition.y - mouseDownPos.y;
			yOff *= scaleY;
			if (Mathf.Abs(yOff) > sliderStep)  {
				SlideListItem((int)(-yOff / sliderStep));
				mouseDownPos.y += ((int)(yOff / sliderStep)) * sliderStep / scaleY;
			}
		}
	}
	
	void HandleTouch() {
		if (Input.touchCount > 1) return;
		if (list.Count == 0) return;
		
		HandleListTouch();
		HandleSliderTouch();
	}
	
	void AdjustSlider() {
		if (sliderLenOrg == 0) {
			sliderLenOrg = slider.GetComponent<exSprite>().height;
			sliderLen = sliderLenOrg;
		}
		
		int count = list.Count - visibleItemNum;
		if (count <= 0) {
			sliderLen = sliderLenOrg;
		} else {
			sliderLen = sliderLenOrg - count * sliderStepOrg;
			if (sliderLen < sliderStepOrg) {
				sliderLen = sliderStepOrg;
				sliderStep = (sliderLenOrg - sliderLen) / (list.Count - visibleItemNum);
			} else {
				sliderStep = sliderStepOrg;
			}
		}
		
		slider.GetComponent<exSprite>().height = sliderLen;
		
		if (sliderOrgPos == Vector3.zero)
			sliderOrgPos = slider.position;
		slider.position = new Vector3(slider.position.x, sliderOrgPos.y - topIdx * sliderStep, slider.position.z);
		
		Vector3 colSize = slider.GetComponent<BoxCollider>().size;
		Vector3 colCenter = slider.GetComponent<BoxCollider>().center;
		
		colCenter.y = - sliderLen / 2;
		colSize.y = sliderLen;
		
		slider.GetComponent<BoxCollider>().size = colSize;
		slider.GetComponent<BoxCollider>().center = colCenter;
	}
	
	void SlideListItem(int step) {
		if (step == 0) return;
		if (list.Count <= visibleItemNum) return;
		
		int topBK = topIdx;
		topIdx += step;
		topIdx = Mathf.Clamp(topIdx, 0, list.Count-visibleItemNum);
		if (topBK == topIdx) 
			return;
		
		for (int i=0; i<topIdx; i++) {
			ListItem li = list[i];
			li.gameObject.SetActive(false);
		}
		for (int i=topIdx; i<topIdx+visibleItemNum; i++) {
			if (i >= list.Count) break;
			GameObject go = list[i].gameObject;
			go.SetActive(true);
			go.transform.position = new Vector3(transform.position.x, transform.position.y - ySpace * (i-topIdx), transform.position.z);
		}
		for (int i=topIdx+visibleItemNum; i<list.Count; i++) {
			GameObject go = list[i].gameObject;
			go.SetActive(false);
		}
		
		slider.Translate(0, -step * sliderStep, 0);
	}
	
	void OnArrowUpClickHandler() {
		SlideListItem(-1);
	}
	
	void OnArrowDownClickHandler() {
		SlideListItem(1);
	}
	
	public ListItem AddItem(string contex) {
		
		GameObject go = (GameObject)Instantiate(itemPrefab);
		
		go.transform.position = new Vector3(transform.position.x, transform.position.y - ySpace * list.Count, transform.position.z);
		go.transform.parent = transform;
		go.GetComponent<exSpriteFont>().text = contex;
		go.GetComponent<exSpriteFont>().anchor = fontAnchor;
		
		ListItem li = go.GetComponent<ListItem>();
		list.Add(li);
		
		if (list.Count > visibleItemNum) {
			go.SetActive(false);
			AdjustSlider();
		}
		
		return li;
	}
	
	public ListItem InsertItem(int idx, string contex) {
		
		if (idx >= list.Count)  return AddItem(contex);
		
		GameObject go = (GameObject)Instantiate(itemPrefab);
		
		go.transform.parent = transform;
		go.GetComponent<exSpriteFont>().text = contex;
		go.GetComponent<exSpriteFont>().anchor = fontAnchor;
		
		ListItem li = go.GetComponent<ListItem>();
		list.Insert(idx, li);
		
		for (int i=idx; i<list.Count; i++) {
			go = list[i].gameObject;
			if (i < topIdx || i >= topIdx+visibleItemNum) {
				go.SetActive(false);
			} else {
				go.SetActive(true);
				go.transform.position = new Vector3(transform.position.x, transform.position.y - ySpace * (i-topIdx), transform.position.z);
			}
		}
		
		return li;
	}
	
	public int DeleteItem(int index) {
		if (index >= list.Count) {
			return -1;
		}
		
		if (selectIdx == index) selectIdx = -1;
		
		GameObject go = list[index].gameObject;
		Destroy(go);
		
		list.RemoveAt(index);
		
		if (list.Count >= topIdx+visibleItemNum) {
			go = list[topIdx+visibleItemNum-1].gameObject;
			go.SetActive(true);
			
			AdjustSlider();
		} else if (topIdx > 0) {
			topIdx--;
			
			go = list[topIdx].gameObject;
			go.SetActive(true);
			
			AdjustSlider();
		}
		
		for (int i=topIdx; i<topIdx+visibleItemNum; i++) {
			if (i >= list.Count) break;
			
			go = list[i].gameObject;
			go.transform.position = new Vector3(transform.position.x, transform.position.y - ySpace * (i-topIdx), transform.position.z);
		}
		
		return list.Count;
	}
	
	public ListItem GetListItem(int idx) {
		if (idx < 0 || idx >= list.Count)
			return null;
		
		return list[idx];
	}
	
	public ListItem GetSelectItem() {
		if (selectIdx == -1)
			return null;
		
		return list[selectIdx];
	}
	
	public int GetSelectIndex() {
		return selectIdx;
	}
	
	public bool IsMouseUp() {
		return isMouseUp;
	}
	
	public void SetCanSelect(bool flag) {
		canSelect = flag;
	}
	
	public void SetItemSelected(int idx, bool flag) {
		if (idx >= list.Count) return;
		
		if (idx < 0) {
			if (selectIdx != -1) {
				list[selectIdx].SetSelected(false);
				selectIdx = -1;
			}
			
			return;
		}
		
		if (flag == true) {
			if (selectIdx != -1 && selectIdx != idx) {
				isSelectChanged = true;
				list[selectIdx].SetSelected(false);
			}
			
			selectIdx = idx;
			list[selectIdx].SetSelected(true);
			
			if (isSelectChanged && selectChangeHandler != null) 
				selectChangeHandler();
			
			if (selectItemHandler != null)
				selectItemHandler();
		} else {
			if (selectIdx == idx) {
				list[selectIdx].SetSelected(false);
				selectIdx = -1;
			}
		}
	}
	
	public int GetCount() {
		return list.Count;
	}
	
	public bool IsSelectChanged() {
		return isSelectChanged;
	}
	
	public void SetSelectItemHandler(MessageDelegate func) {
		selectItemHandler = new MessageDelegate(func);
	}
	
	public void SetSelectChangeHandler(MessageDelegate func) {
		selectChangeHandler = new MessageDelegate(func);
	}
	
}
