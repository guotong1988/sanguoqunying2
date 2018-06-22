using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SGGeneralsListController : MonoBehaviour {
	
	public SelectGeneralToWarController sgCtrl;
	
	public GameObject leftRoot;
	public GameObject rightRoot;
	
	public GameObject leftItemPrefab;
	public GameObject rightItemPrefab;
	
	public GameObject leftArrow;
	public GameObject rightArrow;
	
	public ImageButton rightArrowUp;
	public ImageButton rightArrowDown;
	
	public Transform leftBlink;
	public Transform rightBlink;
	
	private int leftSelectIdx;
	private int rightSelectIdx;
	private int leftTopIdx;
	private int rightTopIdx;
	
	private bool isClickEnable = true;
	
	private List<GameObject> leftGeneralsList = new List<GameObject>();
	private List<GameObject> rightGeneralsList = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		
		rightArrowUp.SetButtonClickHandler(OnButtonUp);
		rightArrowDown.SetButtonClickHandler(OnButtonDown);
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( !isClickEnable ) return;
		
		if (Input.GetMouseButtonDown(0)) {
			
			int max = rightTopIdx + 5;
			max = Mathf.Clamp(max, rightTopIdx, rightGeneralsList.Count);
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			for (int i=rightTopIdx; i<max; i++) {
				
				if (i == rightSelectIdx) continue;
				
				if (rightGeneralsList[i].transform.Find("BG").GetComponent<Collider>().Raycast(ray, out hit, 1000)) {
					
					rightSelectIdx = i;
					rightBlink.transform.localPosition = new Vector3(0, -73 * rightSelectIdx, 0);
					
					sgCtrl.OnRightGeneralSelected(rightSelectIdx);
				}
			}
		}
	}
	
	void OnButtonUp() {
		
		if ( !isClickEnable ) return;
		
		if (rightSelectIdx > 0) {
			rightSelectIdx--;
			rightBlink.transform.localPosition = new Vector3(0, -73 * rightSelectIdx, 0);
			
			sgCtrl.OnRightGeneralSelected(rightSelectIdx);
			
			if (rightTopIdx > rightSelectIdx) {
				rightTopIdx--;
				rightRoot.transform.localPosition = new Vector3(0, 73 * rightTopIdx, 0);
				
				rightGeneralsList[rightTopIdx].SetActive(true);
				rightGeneralsList[rightTopIdx+5].SetActive(false);
			}
		}
	}
	
	void OnButtonDown() {
		
		if ( !isClickEnable ) return;
		
		if (rightSelectIdx < rightGeneralsList.Count - 1) {
			rightSelectIdx++;
			rightBlink.transform.localPosition = new Vector3(0, -73 * rightSelectIdx, 0);
			
			sgCtrl.OnRightGeneralSelected(rightSelectIdx);
			
			if (rightSelectIdx - rightTopIdx >= 5) {
				rightGeneralsList[rightTopIdx].SetActive(false);
				rightGeneralsList[rightTopIdx + 5].SetActive(true);
				
				rightTopIdx++;
				rightRoot.transform.localPosition = new Vector3(0, 73 * rightTopIdx, 0);
			}
		}
	}
	
	public void SetGeneralsList(List<int> lg, List<int> rg, bool[] lf, bool[] rf, int ls, int rs, bool isWarOver) {
		
		leftSelectIdx = ls;
		leftTopIdx = leftSelectIdx - 4;
		leftTopIdx = Mathf.Clamp(leftTopIdx, 0, leftSelectIdx);
		leftRoot.transform.localPosition = new Vector3(0, 73 * leftTopIdx, 0);
		
		int pos = 0;
		for (int i=0; i<lg.Count; i++) {
			
			GameObject go = (GameObject)Instantiate(leftItemPrefab);
			go.transform.parent = leftRoot.transform;
			go.transform.localPosition = new Vector3(0, pos, 0);
			
			if (i < leftTopIdx || i >= leftTopIdx + 5) {
				go.SetActive(false);
			}
			
			go.GetComponent<SGGeneralItem>().SetGeneral(lg[i], lf[i]);
			leftGeneralsList.Add(go);
			pos -= 73;
		}
		
		rightSelectIdx = rs;
		rightTopIdx = rightSelectIdx - 4;
		rightTopIdx = Mathf.Clamp(rightTopIdx, 0, rightSelectIdx);
		rightRoot.transform.localPosition = new Vector3(0, 73 * rightTopIdx, 0);
		
		pos = 0;
		for (int i=0; i<rg.Count; i++) {
			
			GameObject go = (GameObject)Instantiate(rightItemPrefab);
			go.transform.parent = rightRoot.transform;
			go.transform.localPosition = new Vector3(0, pos, 0);
			
			if (i < rightTopIdx || i >= rightTopIdx + 5) {
				go.SetActive(false);
			}
			
			go.GetComponent<SGGeneralItem>().SetGeneral(rg[i], rf[i]);
			rightGeneralsList.Add(go);
			pos -= 73;
		}
		
		if (leftGeneralsList.Count > 5) {
			leftArrow.SetActive(true);
		}
		if (rightGeneralsList.Count > 5) {
			rightArrow.SetActive(true);
		}
		
		if ( !isWarOver ) {
			
			leftBlink.localPosition = new Vector3(0, -73 * leftSelectIdx, 0);
			rightBlink.localPosition = new Vector3(0, -73 * rightSelectIdx, 0);
		} else {
			
			leftBlink.gameObject.SetActive(false);
			rightBlink.gameObject.SetActive(false);
		}
	}
	
	public void SetClickEable(bool flag) {
		
		isClickEnable = flag;
		
		rightArrowUp.enabled = flag;
		rightArrowDown.enabled = flag;
	}
}
