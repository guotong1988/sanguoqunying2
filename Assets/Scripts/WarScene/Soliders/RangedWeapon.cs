using UnityEngine;
using System.Collections;

public class RangedWeapon : MonoBehaviour {
	
	private WarSceneController warCtrl;
	private WarSceneController.WhichSide side;
	private WarSceneController.Direction dir;
	private int type;
	
	private int state;
	private exSprite sprite;
	private exSpriteAnimation anim;
	private float orgPosX;
	private float timeTick;
	
	private float upspeed = 300;
	private float speed = 500;
	private Vector3 offset = new Vector3(-20, 50, 0);
	
	// Use this for initialization
	void Start () {
		sprite = transform.GetChild(0).GetComponent<exSprite>();
		anim = transform.GetChild(0).GetComponent<exSpriteAnimation>();
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
		
		if (state == 0) {
			
			Vector3 pos = transform.localPosition;
			
			pos.x += speed * Time.deltaTime;
			if (pos.x >= WarSceneController.manPosMaxX-100 || pos.x <= -WarSceneController.manPosMaxX+100 
				|| Mathf.Abs(pos.x-orgPosX) > 1000) {
				Destroy(gameObject);
				return;
			}
			transform.localPosition = pos;

			int locationX = warCtrl.GetLocationPositionX(transform.localPosition.x);
			int locationY = warCtrl.GetLocationPositionY(transform.localPosition.y);
//			int locationX = (int)((pos.x + WarSceneController.manPosMaxX) / WarSceneController.locationStepX);
//			int locationY = (int)((pos.y + WarSceneController.manPosMaxY) / WarSceneController.locationStepY);
			
			if (warCtrl.GetLocationIsEnemy(side, locationX, locationY)) {
				WarSceneController.Direction dir = WarSceneController.Direction.Left;
				if (speed > 0) {
					dir = WarSceneController.Direction.Right;
				}
				if (warCtrl.OnSoliderHitChecking(side, type, dir, 1, locationX, locationY)) {
					Destroy(gameObject);
				} else {
					state = 1;
					anim.PlayDefault();
				}
			}
		} else {
			timeTick += Time.deltaTime;
			transform.localPosition = new Vector3(transform.localPosition.x - speed / 5 * Time.deltaTime, 
				transform.localPosition.y + upspeed * Time.deltaTime, transform.localPosition.z);
			if (timeTick < 0.5f) {
				sprite.color = new Color(1, 1, 1, 1 - 2 * timeTick);
			} else {
				Destroy(gameObject);
			}
		}
	}
	
	public void Init(WarSceneController.WhichSide s, WarSceneController.Direction d, int t, Vector3 pos, Transform parent, WarSceneController ws) {
		side = s;
		dir = d;
		type = t;
		warCtrl = ws;
		
		transform.parent = parent;
		transform.localPosition = pos;
		transform.GetChild(0).localPosition = offset;
		
		if (dir == WarSceneController.Direction.Left) {
			speed = -speed;
			transform.localScale = WarSceneController.manScaleRight;
		} else {
			transform.localScale = WarSceneController.manScaleLeft;
		}
		orgPosX = transform.localPosition.x;
		
		transform.localEulerAngles = WarSceneController.manEulerAngles;
	}
}
