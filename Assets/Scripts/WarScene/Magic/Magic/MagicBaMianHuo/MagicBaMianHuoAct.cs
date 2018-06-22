using UnityEngine;
using System.Collections;

public class MagicBaMianHuoAct : MonoBehaviour {

	private Vector3 fromPosition;
	private Vector3 toPosition;

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
			transform.localPosition = Vector3.Lerp(fromPosition, toPosition, timeTick * 2);
			sprite.color = new Color(1, 1, 1, 1 - timeTick * 2);
		} else {
			Destroy(gameObject);
		}
	}

	public void SetInfo(Vector3 fromPosition, Vector3 toPosition) {

		this.fromPosition = fromPosition;
		this.toPosition = toPosition;
	}
}
