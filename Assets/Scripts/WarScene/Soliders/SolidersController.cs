using UnityEngine;
using System.Collections;

public class SolidersController : MonoBehaviour {
	
	public enum State {
		Idle,
		Running,
		RandomRun,
		MovingBack,
		Escape,
		Dead
	}
	
	private WarSceneController warCtrl;
	private RangedWeaponCreator rwCrt;
	private Transform mapPoint;
	
	private State state;
	private WarSceneController.WhichSide side;
	private int soliderType;
	private bool isKnight;
	
	private exSprite sprite;
	private exSpriteAnimation anim;

	private bool isMagicLock;

	private bool isFighting;
	private bool isParrying;
	private bool hadCheckAttack;
	private bool isOnFightPosition;
	private bool isOnRandomRun;
	private Vector3 randomRunPos;

	private bool isOnFire;
	private exSpriteAnimation fireAnim;
	
	private int dieBodyStep;
	private float timeCoolDownTick;
	private float timeCoolDown;
	private float timeTick;
	private float timeRangeWeapon;
	
	private int index;
	private bool isEnemyGeneral;
	private Transform targetEnemy;
	
	private int locationX;
	private int locationY;
	private Vector3 targetPosition;
	private int targetLocationX;
	private int targetLocationY;
	
	private float manPosMaxX;
	private float manPosMaxY;
	private float locationStepX;
	private float locationStepY;
	
	private float mapPointMaxX = 55;
	private float mapPointMaxY = 12;
	private float runSpeed = 80;
	private float hitDistance = 48;
	public int hitDistance1 = 2;
	private Vector3 deadBodyOffset = new Vector3(0, 50, 0);
	private float checkTargetDistance1 = 1000;
	private float randomRunRange = 100;
	private int life = 2;
	
	// Use this for initialization
	void Start () {
		
		warCtrl = GameObject.FindWithTag("GameController").GetComponent<WarSceneController>();
		rwCrt = GameObject.Find("RangedWeapons").GetComponent<RangedWeaponCreator>();
		
		sprite = GetComponent<exSprite>();
		anim = GetComponent<exSpriteAnimation>();
		
		if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
			timeCoolDown = 1;
		} else {
			timeCoolDown = 0.5f;
		}
		
		manPosMaxX = WarSceneController.manPosMaxX;
		manPosMaxY = WarSceneController.manPosMaxY;
		locationStepX = WarSceneController.locationStepX;
		locationStepY = WarSceneController.locationStepY;
		CheckLocationState();
		
		hitDistance1 = (int)(hitDistance/locationStepX);
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
		
		if (state == State.Dead) {
			OnDyingHandler();
			return;
		}

		if (isMagicLock) {
			timeCoolDownTick -= Time.deltaTime;
			if (timeCoolDownTick <= 0) {
				isMagicLock = false;
				timeCoolDownTick = 0;
				anim.Resume();
			}
			return;
		}

		if (isParrying) {
			OnParryHandler();
		}
		
		if (isFighting) {
			OnFightingAct();
			return;
		}
		
		if (timeCoolDownTick > 0) {
			timeCoolDownTick -= Time.deltaTime;
			if (timeCoolDownTick <= 0) {
				timeCoolDownTick = 0;
			} else {
				return;
			}
		}
		
		if (isFighting || isParrying) {
			return;
		}
		
		switch (state) {
		case State.Idle:
		{
			if (!anim.IsPlaying("Idle")) {
				anim.Play("Idle");
			}
			
			if (CheckCanFight()) {
				return;
			}
			
			RangeSolidersFightCheck();
		}
			break;
		case State.Running:
		{
			if (!anim.IsPlaying("Run")) {
				anim.Play("Run");
			}
			
			if (WarSceneController.state == WarSceneController.State.Dead) {
				SetRandomRun(transform.localPosition, false);
				return;
			}
			
			if (targetEnemy != null && 
				((side == WarSceneController.WhichSide.Left && transform.localPosition.x - targetEnemy.localPosition.x > -100)
				|| side == WarSceneController.WhichSide.Right && transform.localPosition.x - targetEnemy.localPosition.x < 100)) {
				
				Vector3 posBK = transform.localPosition;
				Vector3 targetPos = Vector3.zero;

				if (transform.localPosition.x > targetEnemy.localPosition.x) {
					targetPos = new Vector3(targetEnemy.localPosition.x + hitDistance - locationStepX,
						targetEnemy.localPosition.y, targetEnemy.localPosition.z);
				} else {
					targetPos = new Vector3(targetEnemy.localPosition.x - hitDistance + locationStepX,
						targetEnemy.localPosition.y, targetEnemy.localPosition.z);
				}
				
				int x = (int)((targetPos.x + manPosMaxX) / locationStepX);
				int y = (int)((targetPos.y + manPosMaxY) / locationStepY);
				if (warCtrl.GetLocationFlag(x, y) != 0 && warCtrl.GetLocationFlag(x, y) != index) {
					SetRandomRun(targetPos, false);
					return;
				}
				
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, 
					targetPos, runSpeed * Time.deltaTime);
				
				if ((transform.localPosition.x > posBK.x && transform.localScale.x > 0)
					|| (transform.localPosition.x < posBK.x && transform.localScale.x < 0)) {
					transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
				}
				
				if (transform.localPosition == targetPos) {
					if (isEnemyGeneral == false
					    || (side == WarSceneController.WhichSide.Left && (transform.localPosition.x > targetEnemy.localPosition.x 
					    	|| (transform.localPosition.x < targetEnemy.localPosition.x 
					    	&& Vector3.Distance(warCtrl.leftGeneral.transform.localPosition, warCtrl.rightGeneral.transform.localPosition) > 200)))
					    || (side == WarSceneController.WhichSide.Right && (transform.localPosition.x < targetEnemy.localPosition.x
					        || (transform.localPosition.x > targetEnemy.localPosition.x
					   		&& Vector3.Distance(warCtrl.leftGeneral.transform.localPosition, warCtrl.rightGeneral.transform.localPosition) > 200)))) {
						CheckCanFight();
					} else {
						SetRandomRun(targetPos, false);
					}
				}
				
			} else {
				Vector3 pos = transform.localPosition;
				
				if (side == WarSceneController.WhichSide.Left) {
					pos = new Vector3(pos.x + runSpeed * Time.deltaTime, pos.y, pos.z);
					if (transform.localScale.x > 0) {
						transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
					}
				} else {
					pos = new Vector3(pos.x - runSpeed * Time.deltaTime, pos.y, pos.z);
					if (transform.localScale.x < 0) {
						transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
					}
				}
				
				pos.x = Mathf.Clamp(pos.x, -WarSceneController.manPosMaxX + 100, WarSceneController.manPosMaxX - 100);
				transform.localPosition = pos;
			}
			
			RangeSolidersFightCheck();
		}
			break;
		case State.RandomRun:
		{
			if (!anim.IsPlaying("Run")) {
				anim.Play("Run");
			}
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, 
				targetPosition, runSpeed * Time.deltaTime);
			if (transform.localPosition == targetPosition) {
				if (!isOnRandomRun) {
					SetRun();
				} else {
					SetRandomRun(randomRunPos, true);
				}
			}
		}	
			break;
		case State.MovingBack:
		{
			if (!anim.IsPlaying("Run")) {
				anim.Play("Run");
			}
			
			Vector3 pos = transform.localPosition;
			if (side == WarSceneController.WhichSide.Left) {
				pos = new Vector3(pos.x - runSpeed * Time.deltaTime, pos.y, pos.z);
			} else {
				pos = new Vector3(pos.x + runSpeed * Time.deltaTime, pos.y, pos.z);
			}
			
			pos.x = Mathf.Clamp(pos.x, -WarSceneController.manPosMaxX + 100, WarSceneController.manPosMaxX - 100);
			transform.localPosition = pos;
		}
			break;
		case State.Escape:
			if (!anim.IsPlaying("Run")) {
				anim.Play("Run");
			}
			
			if (side == WarSceneController.WhichSide.Left) {
				transform.localPosition = new Vector3(transform.localPosition.x - runSpeed * Time.deltaTime, 
					transform.localPosition.y, transform.localPosition.z);
				
				if (transform.localPosition.x <= -WarSceneController.manPosMaxX+WarSceneController.locationStepX) {
					Destroy(gameObject);
					Destroy(mapPoint.gameObject);
				} else if (transform.localPosition.x <= -WarSceneController.manPosMaxX+100) {
					float transparent = (transform.localPosition.x+WarSceneController.manPosMaxX)/100f;
					anim.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
				}
			} else {
				transform.localPosition = new Vector3(transform.localPosition.x + runSpeed * Time.deltaTime, 
					transform.localPosition.y, transform.localPosition.z);
				
				if (transform.localPosition.x >= WarSceneController.manPosMaxX-WarSceneController.locationStepX) {
					Destroy(gameObject);
					Destroy(mapPoint.gameObject);
				} else if (transform.localPosition.x >= WarSceneController.manPosMaxX-100) {
					float transparent = (WarSceneController.manPosMaxX-transform.localPosition.x)/100f;
					anim.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
				}
			}
			break;
		}
		
		CheckLocationState();
	}
	
	void SetMapLocation() {
		 mapPoint.localPosition = new Vector3(transform.localPosition.x / manPosMaxX * mapPointMaxX,
					transform.localPosition.y / manPosMaxY * mapPointMaxY, 0);
	}
	
	void OnDyingHandler() {
		
		float upSpeed = 10;
		float bodySpeed = 60;
		
		if (transform.localScale.x < 0) {
			bodySpeed = -bodySpeed;
		}
		
		switch (dieBodyStep) {
		case 0:
			timeTick += Time.deltaTime;
			if (timeTick < 0.2f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z - Mathf.Lerp(upSpeed, 0, timeTick*5) * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 1;
			}
			break;
		case 1:
			timeTick += Time.deltaTime;
			if (timeTick < 0.2f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z + Mathf.Lerp(0, upSpeed*2, timeTick*5) * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 2;
			}
			break;
		case 2:
			timeTick += Time.deltaTime;
			if (timeTick < 0.2f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z + upSpeed*6 * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 3;
				
				anim.SetFrame("Dead", 1);
				if (isOnFire) {
					fireAnim.SetFrame(fireAnim.defaultAnimation.name, 1);
				}
			}
			break;
		case 3:
			timeTick += Time.deltaTime;
			if (timeTick < 0.2f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z - Mathf.Lerp(upSpeed, 0, timeTick*5) * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 4;
			}
			break;
		case 4:
			timeTick += Time.deltaTime;
			if (timeTick < 0.2f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z + Mathf.Lerp(0, upSpeed*4, timeTick*5) * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 5;
				
				anim.SetFrame("Dead", 2);
				if (isOnFire) {
					fireAnim.SetFrame(fireAnim.defaultAnimation.name, 2);
				}
			}
			break;
		case 5:
			timeTick += Time.deltaTime;
			if (timeTick < 0.1f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z - Mathf.Lerp(upSpeed, 0, timeTick*10) * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 6;
			}
			break;
		case 6:
			timeTick += Time.deltaTime;
			if (timeTick < 0.1f) {
				transform.localPosition = new Vector3(transform.localPosition.x + bodySpeed * Time.deltaTime,
				                                      transform.localPosition.y, 
				                                      transform.localPosition.z + Mathf.Lerp(0, upSpeed*4, timeTick*10) * Time.deltaTime);
			} else {
				timeTick = 0;
				dieBodyStep = 7;
			}
			break;
		case 7:
			timeTick += Time.deltaTime;
			if (timeTick < 1f) {
				sprite.color = new Color(1, 1, 1, 1 - timeTick);
				if (isOnFire) {
					fireAnim.gameObject.GetComponent<exSprite>().color = new Color(1, 1, 1, 1 - timeTick);
				}
			} else {
				timeTick = 0;
				dieBodyStep = 8;
				Destroy(gameObject);
			}
			break;
		}
	}
	
	void OnParryHandler() {
		timeTick += Time.deltaTime;
		if (timeTick >= 0.2f) {
			timeTick = 0;
			isParrying = false;
			
			anim.SetFrame("Idle", 0);
		}
	}
	
	bool CheckCanFight() {
		
		bool flag = false;
		
		for (int i=locationX-hitDistance1; i<=locationX + hitDistance1; i++) {
			if (warCtrl.GetLocationIsEnemy(side, i, locationY)) {
				flag = true;
				if ((i < locationX && transform.localScale.x < 0)
					|| (i > locationX && transform.localScale.x > 0)) {
					transform.localScale = new Vector3(-transform.localScale.x,
						transform.localScale.y, transform.localScale.z);
				}
			}
		}
		
		if (flag) {
			SetFight();
		}
		
		return flag;
	}
	
	void RangeSolidersFightCheck() {
		if (timeRangeWeapon <= 0) {
			if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				int start = 0;
				int end = 0;
				if (transform.localScale.x > 0) {
					start = locationX - (int)(checkTargetDistance1/locationStepX);
					start = Mathf.Clamp(start, 0, (int)(manPosMaxX*2/locationStepX));
					end = locationX;
				} else {
					start = locationX;
					end = locationX+(int)(checkTargetDistance1/locationStepX);
					end = Mathf.Clamp(end, 0, (int)(manPosMaxX*2/locationStepX));
				}
				
				for (int i=start; i<=end; i++) {
					if (warCtrl.GetLocationIsEnemy(side, i, locationY)) {
						if (Random.Range(0, 100) > 90) {
							SetFight();
						}
						timeRangeWeapon = Random.Range(0.5f, 1f);
						break;
					}
				}
			}
		} else {
			timeRangeWeapon -= Time.deltaTime;
		}
	}
	
	void OnFightingAct() {
		
		if (anim.IsPlaying()) {
			if (anim.GetCurFrameIndex() == 2 && !hadCheckAttack) {
				hadCheckAttack = true;
				
				if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
					WarSceneController.Direction dir = WarSceneController.Direction.Left;
					if (transform.localScale.x < 0) {
						dir = WarSceneController.Direction.Right;
					}
					rwCrt.SetRangedWeapon(side, dir, soliderType, transform.localPosition);
				} else {
					if (transform.localScale.x < 0) {
						if (warCtrl.OnSoliderHitChecking(side, soliderType, WarSceneController.Direction.Right, hitDistance1, locationX, locationY)) {
							targetEnemy = null;
						}
					} else {
						if (warCtrl.OnSoliderHitChecking(side, soliderType, WarSceneController.Direction.Left, hitDistance1, locationX, locationY)) {
							targetEnemy = null;
						}
					}
				}
			}
		} else {
			
			hadCheckAttack = false;
			isFighting = false;
			timeCoolDownTick = timeCoolDown;
			
			anim.Play("Idle");
		}
	}
	
	void CheckLocationState() {

		locationX = warCtrl.GetLocationPositionX(transform.localPosition.x);
		locationY = warCtrl.GetLocationPositionY(transform.localPosition.y);
//		locationX = (int)((transform.localPosition.x + manPosMaxX) / locationStepX);
//		locationY = (int)((transform.localPosition.y + manPosMaxY) / locationStepY);
		
		SetMapLocation();
	}
	
	public void SetMapPoint(Transform p) {
		mapPoint = p;
	}
	
	void SetFight() {
		isFighting = true;
		isParrying = false;
		timeTick = 0;
		
		anim.Play("Attack");
	}
	
	public void SetRandomRun(Vector3 pos, bool b) {
		
		state = State.RandomRun;
		isOnRandomRun = b;
		randomRunPos = pos;
		
		targetPosition = new Vector3(pos.x + Random.Range(-randomRunRange, randomRunRange),
			pos.y + Random.Range(-randomRunRange/2, randomRunRange/2), transform.localPosition.z);
		targetPosition.x = Mathf.Clamp(targetPosition.x, -WarSceneController.manPosMaxX+1, WarSceneController.manPosMaxX-1);
		targetPosition.y = Mathf.Clamp(targetPosition.y, -WarSceneController.manPosMaxY+1, WarSceneController.manPosMaxY-1);
		
		if (targetPosition.x > transform.localPosition.x) {
			if (transform.localScale.x > 0) {
				transform.localScale = new Vector3(-transform.localScale.x,
						transform.localScale.y, transform.localScale.z);
			}
		} else {
			if (transform.localScale.x < 0) {
				transform.localScale = new Vector3(-transform.localScale.x,
						transform.localScale.y, transform.localScale.z);
			}
		}
	}
	
	public void SetIdle() {
		
		state = State.Idle;
	}
	
	public void SetRun() {
		
		state = State.Running;
		timeCoolDownTick = 0;
	}
	
	public void SetEscape() {
		
		state = State.Escape;
		
		if (side == WarSceneController.WhichSide.Left) {
			transform.localScale = new Vector3(0.5f, 0.5f, 1);
		} else {
			transform.localScale = new Vector3(-0.5f, 0.5f, 1);
		}
	}
	
	public void SetMovingBack() {
		
		state = State.MovingBack;
		
		if (side == WarSceneController.WhichSide.Left) {
			transform.localScale = new Vector3(0.5f, 0.5f, 1);
		} else {
			transform.localScale = new Vector3(-0.5f, 0.5f, 1);
		}
	}

	public void SetOnMagicLock() {

		if (state != State.Dead) {
			isMagicLock = true;
			timeCoolDownTick = 5;
			anim.Pause();
		}
	}

	public bool OnDamage(int t, WarSceneController.Direction dir, bool isFire) {
		if (isFire) {
			SetOnFire();
		}
		return OnDamage(t, dir);
	}

	public bool OnDamage(int t, WarSceneController.Direction dir) {

		if (state == State.Dead)	return false;

		bool flag = true;
		int block = 50;
		
		switch (t) {
		case -1:
			block = 0;
			break;
		case 1000:
			block = 50;
			break;
		case 0:
			if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				block = 20;
			} else if (soliderType == 1 || soliderType == 2 || soliderType == 8) {
				block = 70;
			}
			break;
		case 1:
			if (soliderType == 0 || soliderType == 6 || soliderType == 10) {
				block = 30;
			} else if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				block = 70;
			}
			break;
		case 2:
			if (soliderType == 0 || soliderType == 7 || soliderType == 10) {
				block = 30;
			} else if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				block = 70;
			}
			break;
		case 3:
		case 4:
		case 5:
			if (soliderType == 1 || soliderType == 2 || soliderType == 6 || soliderType == 8|| soliderType == 10) {
				block = 30;
			} else if (soliderType == 0 || soliderType == 7 || soliderType == 9) {
				block = 80;
			}
			break;
		case 6:
			if (soliderType == 2 || soliderType == 8 || soliderType == 10) {
				block = 30;
			} else if (soliderType == 1 || soliderType == 3 || soliderType == 9) {
				block = 70;
			}
			break;
		case 7:
			if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				block = 25;
			} else if (soliderType == 1 || soliderType == 2 || soliderType == 8) {
				block = 65;
			}
			break;
		case 8:
			if (soliderType == 0 || soliderType == 7 || soliderType == 10) {
				block = 25;
			} else if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				block = 65;
			}
			break;
		case 9:
			if (soliderType == 3 || soliderType == 4 || soliderType == 5) {
				block = 25;
			} else if (soliderType == 1 || soliderType == 2 || soliderType == 8) {
				block = 65;
			}
			break;
		case 10:
			block = 60;
			break;
		}
		
		int rand = Random.Range(0, 100);
		
		if (block > rand) {
			flag = false;
		}
		
		if (!flag) {
			if (!isFighting) {
				isParrying = true;
				timeTick = 0;
				
				anim.Stop();
				anim.SetFrame("Parry", 0);
			}
			return false;
		}

		if (isKnight) {
			if (t != -1 && life > 1) {
				life--;
				return false;
			}
		}

		SoundController.Instance.PlaySound3D("00036", transform.position);

		state = State.Dead;
		timeTick = 0;
		anim.Stop();
		anim.SetFrame("Dead", 0);
		
		if ((dir == WarSceneController.Direction.Left && transform.localScale.x > 0)
			|| (dir == WarSceneController.Direction.Right && transform.localScale.x < 0)) {
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		
		if (isKnight && soliderType != 6) {
				Transform horse = transform.GetChild(0);
				horse.gameObject.SetActive(true);
				horse.parent = transform.parent;
				horse.GetComponent<SolidersHorse>().SetHorseRun();

				warCtrl.AddSoliderHorse(horse.GetComponent<SolidersHorse>());
		}
		
		Vector3 pos = transform.localPosition;
		pos += deadBodyOffset;
		transform.localPosition = pos;
		
		warCtrl.RemoveSolider(index);
		Destroy(mapPoint.gameObject);
		
		return true;
	}

	public void SetOnFire() {
		if (state == State.Dead) return;

		isOnFire = true;

		string fireName = "Soliders/Other/SoliderFire";
		GameObject go = (GameObject)Instantiate(Resources.Load(fireName));
		go.transform.parent = this.transform;
		go.transform.localPosition = new Vector3(0, 0, -2);
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;

		fireAnim = go.GetComponent<exSpriteAnimation>();
	}

	public void SetSide(WarSceneController.WhichSide s) {
		side = s;
	}
	
	public void SetType(int t) {
		soliderType = t;
	}
	
	public void SetTargetEnemy(Transform t, bool isGeneral) {
		targetEnemy = t;
		isEnemyGeneral = isGeneral;
	}
	
	public Transform GetTargetEnemy() {
		return targetEnemy;
	}
	
	public State GetState() {
		return state;
	}
	
	public void SetIndex(int i) {
		index = i;
	}
	
	public int GetIndex() {
		return index;
	}
	
	public int GetLocationX() {
		return locationX;
	}
	
	public int GetLocationY() {
		return locationY;
	}

	public bool GetIsKnight() {
		return isKnight;
	}

	public void SetIsKnight(bool b) {
		isKnight = b;
	}

	public void SetPause() {
		if (anim != null && anim.IsPlaying())
			anim.Pause();
	}

	public void SetResume() {
		if (anim != null && anim.IsPaused())
			anim.Resume();
	}
}
