using UnityEngine;
using System.Collections;

public class SolidersHorse : MonoBehaviour {
	
	private int state;
	private float orgPosX;
	private float speed = 80;
	
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

		if (WarSceneController.isGamePause) 
			return;

		if (WarSceneController.state == WarSceneController.State.Beginning 
			|| WarSceneController.state == WarSceneController.State.Menu
			|| WarSceneController.state == WarSceneController.State.End
			|| WarSceneController.state == WarSceneController.State.Assault) {
			return;
		}
		
		Vector3 pos = transform.localPosition;
			
		pos.x += speed * Time.deltaTime;
		if (pos.x >= WarSceneController.manPosMaxX-300 || pos.x <= -WarSceneController.manPosMaxX+300 
			|| Mathf.Abs(pos.x-orgPosX) > 1000) {
			state = 1;
		}
		transform.localPosition = pos;
		
		if (state == 1) {
			timeTick += Time.deltaTime;
			if (timeTick < 0.5f) {
				sprite.color = new Color(1, 1, 1, 1-timeTick*2);
			} else {
				GameObject.FindWithTag("GameController").GetComponent<WarSceneController>().RemoveSoliderHorse(GetComponent<SolidersHorse>());
				Destroy(gameObject);
			}
		}
	}
	
	public void SetHorseRun() {
		
		orgPosX = transform.localPosition.x;
		if (transform.localScale.x < 0) {
			speed = -speed;
		}
		Vector3 scale = transform.localScale;
		scale.x = -scale.x;
		transform.localScale = scale;
	}

	public void SetPause() {
		if (anim == null)
			anim = GetComponent<exSpriteAnimation>();

		if (anim != null && anim.IsPlaying())
			anim.Pause();
	}

	public void SetResume() {
		if (anim == null)
			anim = GetComponent<exSpriteAnimation>();

		if (anim != null && anim.IsPaused())
			anim.Resume();
	}
}
