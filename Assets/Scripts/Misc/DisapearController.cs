using UnityEngine;
using System.Collections;

public class DisapearController : MonoBehaviour {

	public float waitTime = 0.5f;

	private float timeTick;

	private exSprite sprite;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<exSprite>();
	}
	
	// Update is called once per frame
	void Update () {
		timeTick += Time.deltaTime;
		if (timeTick > waitTime) {
			if (timeTick - waitTime < 0.5f) {
				sprite.color = new Color(1, 1, 1, 1 - (timeTick - waitTime) * 2);
			} else {
				Destroy(gameObject);
			}
		}
	}

	public void SetWaitTime(float waitTime) {
		this.waitTime = waitTime;
	}
}
