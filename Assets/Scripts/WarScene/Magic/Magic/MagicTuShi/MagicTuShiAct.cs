using UnityEngine;
using System.Collections;

public class MagicTuShiAct : MonoBehaviour {

	private exSprite sprite;
	private exSpriteAnimation anim;
	private float timeTick;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<exSprite>();
		anim = GetComponent<exSpriteAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!anim.IsPlaying(anim.defaultAnimation.name)) {
			timeTick += Time.deltaTime;
			if (timeTick < 0.5f) {
				sprite.color = new Color(1, 1, 1, 1 - timeTick * 2);
			} else {
				Destroy(gameObject);
			}
		}
	}
}
