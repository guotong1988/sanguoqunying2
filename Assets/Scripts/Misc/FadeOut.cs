using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {
	
	private exSprite sprite;
	private float timeTick;
	private string levelName;
	
	// Use this for initialization
	void Start () {
		
		enabled = false;
		timeTick = 0;
		sprite = transform.GetChild(0).GetComponent<exSprite>();
		sprite.color = new Color(0, 0, 0, 0);
		sprite.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timeTick < 0.5f) {
			timeTick += Time.deltaTime;
			timeTick = Mathf.Clamp(timeTick, 0f, 0.5f);
			
			sprite.color = new Color(0, 0, 0, timeTick * 2);
		} else {
			Application.LoadLevel(levelName);
		}
	}
	
	public void SetLevelName(string n) {
		enabled = true;
		levelName = n;
		
		sprite.gameObject.SetActive(true);
	}
}

