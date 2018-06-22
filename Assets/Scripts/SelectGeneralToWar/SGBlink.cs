using UnityEngine;
using System.Collections;

public class SGBlink : MonoBehaviour {
	
	public GameObject frame1;
	public GameObject frame2;
	
	private int idx;
	private float timeTick;
	
	// Use this for initialization
	void Start () {
		
		frame1.SetActive(true);
		frame2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
		timeTick += Time.deltaTime;
		if (timeTick >= 0.05f) {
			timeTick = 0;
			
			if (idx == 0) {
				idx = 1;
				
				frame1.SetActive(false);
				frame2.SetActive(true);
			} else {
				idx = 0;
				
				frame1.SetActive(true);
				frame2.SetActive(false);
			}
		}
	}
}
