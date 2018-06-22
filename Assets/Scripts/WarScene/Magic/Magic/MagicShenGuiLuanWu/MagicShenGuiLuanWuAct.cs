using UnityEngine;
using System.Collections;

public class MagicShenGuiLuanWuAct : MonoBehaviour {

	private float speed;
	private float timeTick;

	private exSprite sprite;

	// Use this for initialization
	void Start () {

		sprite = GetComponent<exSprite>();
	}
	
	// Update is called once per frame
	void Update () {

		timeTick += Time.deltaTime;
		if (timeTick < 0.5f) {
			Vector3 pos = transform.localPosition;
			pos.x += speed * Time.deltaTime;
			transform.localPosition = pos;

			sprite.color = new Color(1, 1, 1, 1 - timeTick * 2);
		} else {
			Destroy(gameObject);
		}
	}

	public void SetInfo(float speed) {
		this.speed = speed;
	}
}
