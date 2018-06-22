using UnityEngine;
using System.Collections;

public class MagicFireDragonBody : MonoBehaviour {
	
	private float speed;
	private float disappearTime;

	private float timeTick;
	private exSprite sprite;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<exSprite>();
	}
	
	// Update is called once per frame
	void Update () {

		transform.localPosition = new Vector3(transform.localPosition.x + speed * Time.deltaTime,
		                                      transform.localPosition.y, transform.localPosition.z);
		timeTick += Time.deltaTime;
		if (timeTick >= disappearTime) {
			if (timeTick - disappearTime < 0.5f) {
				sprite.color = new Color(1, 1, 1, 1 - (timeTick - disappearTime) * 2);
			} else {
				Destroy(gameObject);
			}
		}
	}

	public void SetInfo(float speed, float disappearTime) {
		this.speed = speed;
		this.disappearTime = disappearTime;
	}
}
