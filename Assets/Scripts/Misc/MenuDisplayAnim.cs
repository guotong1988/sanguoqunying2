using UnityEngine;
using System.Collections;

public class MenuDisplayAnim : MonoBehaviour {
	
	public enum AnimType {
		InsertFromLeft,
		InsertFromRight,
		InsertFromTop,
		InsertFromBottom,
		OutToLeft,
		OutToRight,
		OutToTop,
		OutToBottom
	};
	
	public bool autoPlay = true;
	public AnimType  type;
	public float offset = 300;
	
	private bool isPlaying;
	
	private Vector3 orgPos = Vector3.one;
	private Vector3 destPos;
	private float timeTick;
	private float realtime;
	
	//private float speed = 400;
	
	// Use this for initialization
	void Start () {
		timeTick = 0;
		if (orgPos == Vector3.one) {
			orgPos = transform.localPosition;
		}
		
		if (autoPlay) {
			SetAnim(type);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPlaying) return;
		
		timeTick += Time.realtimeSinceStartup - realtime;
		realtime = Time.realtimeSinceStartup;
		
		if (timeTick >= 0.4f) {
			timeTick = 0;
			isPlaying = false;
			transform.localPosition = destPos;
			
			if (type == AnimType.OutToLeft || type == AnimType.OutToRight
				|| type == AnimType.OutToTop || type == AnimType.OutToBottom) {
				gameObject.SetActive(false);
			}
			return;
		}
		
		switch (type) {
		case AnimType.InsertFromLeft:
		case AnimType.InsertFromRight:
		case AnimType.OutToLeft:
		case AnimType.OutToRight:
			transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, destPos.x, timeTick), transform.localPosition.y, transform.localPosition.z);
			break;
		case AnimType.InsertFromTop:
		case AnimType.InsertFromBottom:
		case AnimType.OutToTop:
		case AnimType.OutToBottom:
			transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, destPos.y, timeTick), transform.localPosition.z);
			break;
		}
	}
	
	public void SetAnim(AnimType animType) {
		
		gameObject.SetActive(true);
		
		type = animType;
		isPlaying = true;
		timeTick = 0;
		realtime = Time.realtimeSinceStartup;
		
		if (orgPos == Vector3.one) {
			orgPos = transform.localPosition;
		}
		
		switch (type) {
		case AnimType.InsertFromLeft:
			transform.localPosition = new Vector3(orgPos.x - offset, transform.localPosition.y, transform.localPosition.z);
			break;
		case AnimType.InsertFromRight:
			transform.localPosition = new Vector3(orgPos.x + offset, transform.localPosition.y, transform.localPosition.z);
			break;
		case AnimType.InsertFromTop:
			transform.localPosition = new Vector3(transform.localPosition.x, orgPos.y + offset, transform.localPosition.z);
			break;
		case AnimType.InsertFromBottom:
			transform.localPosition = new Vector3(transform.localPosition.x, orgPos.y - offset, transform.localPosition.z);
			break;
		}
		
		switch (type) {
		case AnimType.InsertFromTop:
		case AnimType.InsertFromBottom:
		case AnimType.InsertFromLeft:
		case AnimType.InsertFromRight:
			destPos = orgPos;
			break;
		case AnimType.OutToLeft:
			destPos = new Vector3(orgPos.x - offset, orgPos.y, orgPos.z);
			break;
		case AnimType.OutToRight:
			destPos = new Vector3(orgPos.x + offset, orgPos.y, orgPos.z);
			break;
		case AnimType.OutToTop:
			destPos = new Vector3(orgPos.x, orgPos.y + offset, orgPos.z);
			break;
		case AnimType.OutToBottom:
			destPos = new Vector3(orgPos.x, orgPos.y - offset, orgPos.z);
			break;
		}
	}
	
	public Vector3 GetOriginalPosition() {
		if (orgPos == Vector3.one) {
			orgPos = transform.localPosition;
		}
		
		return orgPos;
	}
	
	public void SetOriginalPosition(Vector3 pos) {
		orgPos = pos;
	}
	
	public bool IsPlaying() {
		return isPlaying;
	}
}

