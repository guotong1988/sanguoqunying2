using UnityEngine;
using System.Collections;

public class GeneralController : MonoBehaviour {
	
	public enum State {
		Idle,
		Running,
		Magic,
		Escape,
		Dead
	};
	
	public WarSceneController.WhichSide side;
	
	public Transform mapPointPrefab;
	
	private GeneralInfo gInfo;
	private exSpriteAnimation head;
	private exSpriteAnimation body;
	private exSpriteAnimation weapon;
	private exSpriteAnimation horse;
	
	private WarSceneController warCtrl;
	private Transform mapPoint;
	
	private State state;
	private State stateBK;
	
	private bool isStopped;
	private bool isFighting;
	private bool isParrying;
	private bool isHited;
	
	private int locationX;
	private int locationY;
	
	private bool hadCheckAttack;
	private bool isMightyHit;
	private bool isHitFront;
	
	private float manPosMaxX;
	private float manPosMaxY;
	private float locationStepX;
	
	private bool armyAssaultFlag;

	private bool isMagicArrow;

	private float timeCoolDown;
	private float timeParry;
	private float leftGeneralIdleTime;
	private float timeTick;
	private float magicTime;
	
	private int dieBodyStep;
	private int dieWeaponStep;
	private float dieTimeTick1;
	private float dieTimeTick2;
	
	private float runSpeed = 80;
	private float hitDistance = 48;
	private float mapPointMaxX = 55;
	private float mapPointMaxY = 12;
	private Vector3 generalPosBack = new Vector3(600, 0, 0);
	private Vector3 generalPosFront = new Vector3(120, 0, 0);
	private Vector3 deadBodyOffset = new Vector3(0, 50, 0);
	private Vector3 weaponOffset = new Vector3(0, 50, 0);
	
	/* 		body animation
	 *  	Idle Run Hit AttackFront1 AttackFront2 AttackFront3 AttackFront4 AttackBack Dead Misc Arrow Dart
	 * 
	 * 		horse animation
	 * 		Idle Run Fight
	 */
	
	// Use this for initialization
	void Start () {
		
		int gIdx;
		if (side == WarSceneController.WhichSide.Left) {
			gIdx = WarSceneController.leftGeneralIdx;
		} else {
			gIdx = WarSceneController.rightGeneralIdx;
		}
		
		gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		GeneralInit(gIdx);
		SetMapLocationFrame();
		
		warCtrl = GameObject.FindWithTag("GameController").GetComponent<WarSceneController>();
		leftGeneralIdleTime = Random.Range(8, 15);
		
		manPosMaxX = WarSceneController.manPosMaxX;
		manPosMaxY = WarSceneController.manPosMaxY;
		locationStepX = WarSceneController.locationStepX;
		CheckLocationState();
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
		
		if (isParrying) {
			OnParryHandler();
		}
		
		if (isHited) {
			OnHitedHandler();
		}
		
		if (isFighting) {
			OnFightingAct();
			return;
		}
		
		if (timeCoolDown > 0) {
			timeCoolDown -= Time.deltaTime;
			if (timeCoolDown <= 0) {
				timeCoolDown = 0;
			} else {
				return;
			}
		}
		
		CheckLocationState();
		CheckLeftGeneralMagicRelease();
		
		if (isFighting || isHited || isParrying) {
			return;
		}
		
		switch (state) {
		case State.Idle:
			OnIdleHandle();
			break;
		case State.Running:
			OnRunningHandle();
			break;
		case State.Magic:
			OnMagicHandle();
			break;
		case State.Escape:
			OnEscapeHandle();
			break;
		}
	}

	void OnIdleHandle() {
		if (!head.IsPlaying("Idle")) {
			head.Play("Idle");
			body.Play("Idle");
			horse.Play("Idle");
		}

		if (WarSceneController.state == WarSceneController.State.Running) {
			if (side == WarSceneController.WhichSide.Left) {
				timeTick += Time.deltaTime;
				if (timeTick >= leftGeneralIdleTime) {
					timeTick = 0;
					if (!armyAssaultFlag) {
						armyAssaultFlag = true;

						GeneralInfo rGCtrl = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);
						if (gInfo.healthCur < 20 && gInfo.soldierCur + gInfo.knightCur < 10
						    && (rGCtrl.healthCur > 50
						    || rGCtrl.soldierCur + rGCtrl.knightCur > 10)
						    && warCtrl.rightGeneral.GetState() != State.Escape) {
							warCtrl.SetArmyEscape(WarSceneController.WhichSide.Left);
							return;
						}

						leftGeneralIdleTime = Random.Range(2, 10);
						if (gInfo.soldierCur + gInfo.knightCur > 0) {
							warCtrl.SetArmyAssault(WarSceneController.WhichSide.Left);
						}
					} else {
						SetRun();
					}
				}
			}
		}
	}

	void OnRunningHandle() {
		if (isStopped){
			if (head.IsPlaying("Run")) {
				head.Play("Idle");
				body.Play("Idle");
				horse.Play("Idle");
			}
			return;
		}
		
		if (!head.IsPlaying("Run")) {
			head.Play("Run");
			body.Play("Run");
			horse.Play("Run");
		}
		SoundController.Instance.PlaySound3D("00021", transform.position);

		if (Vector3.Distance(warCtrl.leftGeneral.transform.localPosition, warCtrl.rightGeneral.transform.localPosition) <= 40)
			return;

		if (side == WarSceneController.WhichSide.Left) {
			transform.localPosition = new Vector3(transform.localPosition.x + runSpeed * Time.deltaTime, 
			                                      transform.localPosition.y, transform.localPosition.z);
		} else {
			transform.localPosition = new Vector3(transform.localPosition.x - runSpeed * Time.deltaTime, 
			                                      transform.localPosition.y, transform.localPosition.z);
		}
	}

	void OnMagicHandle() {

		if (isMagicArrow && body.GetCurFrameIndex() >= 2) {
			isMagicArrow = false;
			MagicController.Instance.OnArrowOut();
		}

		magicTime -= Time.deltaTime;
		if (magicTime <= 0) {
			magicTime = 0;

			if (isMagicArrow) {
				isMagicArrow = false;
				MagicController.Instance.OnArrowOut();
			}

			OnMagicReturn();
		}
	}

	void OnEscapeHandle() {
		if (isStopped){
			if (head.IsPlaying("Run")) {
				head.Play("Idle");
				body.Play("Idle");
				horse.Play("Idle");
			}
			return;
		}

		if (!head.IsPlaying("Run")) {
			head.Play("Run");
			body.Play("Run");
			horse.Play("Run");
		}
		SoundController.Instance.PlaySound3D("00021", transform.position);

		if (side == WarSceneController.WhichSide.Left) {
			transform.localPosition = new Vector3(transform.localPosition.x - runSpeed * Time.deltaTime * 0.6f, 
			                                      transform.localPosition.y, transform.localPosition.z);
			
			if (transform.localPosition.x <= -WarSceneController.manPosMaxX+WarSceneController.locationStepX) {
				Destroy(gameObject);
				warCtrl.OnWarResult(WarSceneController.WhichSide.Left, true);
			} else if (transform.localPosition.x <= -WarSceneController.manPosMaxX+100) {
				float transparent = (transform.localPosition.x+WarSceneController.manPosMaxX)/100f;
				head.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
				body.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
				horse.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
			}
		} else {
			transform.localPosition = new Vector3(transform.localPosition.x + runSpeed * Time.deltaTime * 0.6f, 
			                                      transform.localPosition.y, transform.localPosition.z);
			
			if (transform.localPosition.x >= WarSceneController.manPosMaxX-WarSceneController.locationStepX) {
				Destroy(gameObject);
				warCtrl.OnWarResult(WarSceneController.WhichSide.Right, true);
			} else if (transform.localPosition.x >= WarSceneController.manPosMaxX-100) {
				float transparent = (WarSceneController.manPosMaxX-transform.localPosition.x)/100f;
				head.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
				body.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
				horse.GetComponent<exSprite>().color = new Color(1, 1, 1, transparent);
			}
		}
	}

	void GeneralInit(int gIdx) {
		
		int horseIdx = 0;
		if (gInfo.equipment > 26) {
			horseIdx = gInfo.equipment - 26;
		}
		
		string str = "";
		GameObject go;
		
		if (side == WarSceneController.WhichSide.Left) {
			str = "Generals/HorseRed/";
		} else {
			str = "Generals/HorseGreen/";
		}
		str += "Horse00" + (horseIdx+1);
		go = (GameObject)Instantiate(Resources.Load(str));
		horse = go.GetComponent<exSpriteAnimation>();
		
		if (side == WarSceneController.WhichSide.Left) {
			str = "Generals/HeadRed/";
		} else {
			str = "Generals/HeadGreen/";
		}
		int[,] generalBody = Informations.Instance.generalBody;
		if (generalBody[gIdx, 0] < 9) {
			str += "Head0" + (generalBody[gIdx, 0]+1);
		} else {
			str += "Head" + (generalBody[gIdx, 0]+1);
		}
		go = (GameObject)Instantiate(Resources.Load(str));
		head = go.GetComponent<exSpriteAnimation>();
		
		if (side == WarSceneController.WhichSide.Left) {
			str = "Generals/BodyRed/";
		} else {
			str = "Generals/BodyGreen/";
		}
		if (generalBody[gIdx, 1] < 9) {
			str += "Body0" + (generalBody[gIdx, 1]+1);
		} else {
			str += "Body" + (generalBody[gIdx, 1]+1);
		}
		go = (GameObject)Instantiate(Resources.Load(str));
		body = go.GetComponent<exSpriteAnimation>();
		
		if (generalBody[gIdx, 1] < 9) {
			str = "Generals/Weapon/Weapon0" + (generalBody[gIdx, 1]+1);
		} else {
			str = "Generals/Weapon/Weapon" + (generalBody[gIdx, 1]+1);
		}
		go = (GameObject)Instantiate(Resources.Load(str));
		weapon = go.GetComponent<exSpriteAnimation>();
		
		head.transform.parent 	= transform;
		body.transform.parent 	= transform;
		weapon.transform.parent = transform;
		horse.transform.parent 	= transform;
		
		head.transform.localPosition 	= Vector3.zero;
		body.transform.localPosition 	= Vector3.zero;
		weapon.transform.localPosition 	= new Vector3(0, 0, -5);
		horse.transform.localPosition 	= new Vector3(0, 0, 5f);
		
		head.transform.localScale 	= Vector3.one;
		body.transform.localScale 	= Vector3.one;
		weapon.transform.localScale	= Vector3.one;
		horse.transform.localScale 	= Vector3.one;
		
		head.transform.localRotation 	= Quaternion.identity;
		body.transform.localRotation 	= Quaternion.identity;
		weapon.transform.localRotation	= Quaternion.identity;
		horse.transform.localRotation 	= Quaternion.identity;
		
		if (side == WarSceneController.WhichSide.Right) {
			if (WarSceneController.rightGeneralPosition == 0) {
				transform.localPosition = generalPosBack;
			} else {
				transform.localPosition = generalPosFront;
			}
		}
	}
	
	void SetMapLocationFrame() {
		Transform mapFrame = GameObject.Find("Map").transform;
		mapPoint = (Transform)Instantiate(mapPointPrefab);
		mapPoint.parent = mapFrame;
		mapPoint.localPosition = Vector3.zero;
	}
	
	void OnDyingHandler() {
		
		float upSpeed = 80;
		
		float horseSpeed = runSpeed;
		float weaponSpeed = 80;
		float bodySpeed = 60;
		
		if (side == WarSceneController.WhichSide.Left) {
			horseSpeed = -runSpeed;
		}
		
		horse.transform.localPosition = new Vector3(horse.transform.localPosition.x + horseSpeed * Time.deltaTime,
			horse.transform.localPosition.y, horse.transform.localPosition.z);
		
		switch (dieWeaponStep) {
		case 0:
			dieTimeTick1 += Time.deltaTime;
			if (dieTimeTick1 < 0.5f) {
				weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x + weaponSpeed * Time.deltaTime,
					weapon.transform.localPosition.y + upSpeed*2 * Time.deltaTime, weapon.transform.localPosition.z);
			} else {
				dieTimeTick1 = 0;
				dieWeaponStep = 1;
			}
			break;
		case 1:
			dieTimeTick1 += Time.deltaTime;
			if (dieTimeTick1 < 0.5f) {
				weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x + weaponSpeed * Time.deltaTime,
					weapon.transform.localPosition.y + Mathf.Lerp(upSpeed*2, 0, dieTimeTick1*2) * Time.deltaTime, weapon.transform.localPosition.z);
			} else {
				dieTimeTick1 = 0;
				dieWeaponStep = 2;
			}
			break;
		case 2:
			dieTimeTick1 += Time.deltaTime;
			if (dieTimeTick1 < 0.5f) {
				weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x + weaponSpeed * Time.deltaTime,
					weapon.transform.localPosition.y - Mathf.Lerp(0, upSpeed*2, dieTimeTick1*2) * Time.deltaTime, weapon.transform.localPosition.z);
			} else {
				dieTimeTick1 = 0;
				dieWeaponStep = 3;
			}
			break;
		case 3:
			dieTimeTick1 += Time.deltaTime;
			if (dieTimeTick1 < 0.5f) {
				weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x + weaponSpeed * Time.deltaTime,
					weapon.transform.localPosition.y - upSpeed * Time.deltaTime, weapon.transform.localPosition.z);
				weapon.GetComponent<exSprite>().color = new Color(1, 1, 1, 1 - dieTimeTick1*2);
			} else {
				dieTimeTick1 = 0;
				dieWeaponStep = 5;
			}
			break;
		}
		
		switch (dieBodyStep) {
		case 0:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.25f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y + Mathf.Lerp(upSpeed, 0, dieTimeTick2*4) * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 1;
			}
			break;
		case 1:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.25f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y - Mathf.Lerp(0, upSpeed, dieTimeTick2*4) * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 2;
			}
			break;
		case 2:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.5f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y - upSpeed * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 3;
				
				head.SetFrame("Dead", 1);
				body.SetFrame("Dead", 1);
			}
			break;
		case 3:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.2f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y + Mathf.Lerp(upSpeed, 0, dieTimeTick2*5) * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 4;
			}
			break;
		case 4:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.2f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y - Mathf.Lerp(0, upSpeed, dieTimeTick2*5) * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 5;
				
				head.SetFrame("Dead", 2);
				body.SetFrame("Dead", 2);
			}
			break;
		case 5:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.1f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y + Mathf.Lerp(upSpeed, 0, dieTimeTick2*10) * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 6;
			}
			break;
		case 6:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 0.1f) {
				head.transform.localPosition = new Vector3(head.transform.localPosition.x + bodySpeed * Time.deltaTime,
					head.transform.localPosition.y - Mathf.Lerp(0, upSpeed, dieTimeTick2*10) * Time.deltaTime, head.transform.localPosition.z);
				body.transform.localPosition = head.transform.localPosition;
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 7;
			}
			break;
		case 7:
			dieTimeTick2 += Time.deltaTime;
			if (dieTimeTick2 < 1f) {
				head.GetComponent<exSprite>().color = new Color(1, 1, 1, 1 - dieTimeTick2);
				body.GetComponent<exSprite>().color = new Color(1, 1, 1, 1 - dieTimeTick2);
			} else {
				dieTimeTick2 = 0;
				dieBodyStep = 8;
				warCtrl.OnWarResult(side, false);
			}
			break;
		}
	}
	
	void OnParryHandler() {
		timeParry -= Time.deltaTime;
		if (timeParry <= 0) {
			timeParry = 0;
			isParrying = false;
		}
	}
	
	void OnHitedHandler() {
		if (!body.IsPlaying()) {
			isHited = false;
		}
	}
	
	void OnFightingAct() {
		if (body.IsPlaying()) {
			
			if (body.GetCurFrameIndex() == 2 && !hadCheckAttack) {
				hadCheckAttack = true;
				if (isHitFront) {
					int damage = 0;
					if (!isMightyHit) {
						damage = Random.Range(gInfo.strength / 12 - 3, gInfo.strength / 12 + 3);
					} else {
						damage = Random.Range(gInfo.strength / 12, gInfo.strength / 12 + 5);
					}
					if (transform.localScale.x > 0) {
						warCtrl.OnGenenralHitChecking(side, damage, WarSceneController.Direction.Left, (int)(hitDistance/locationStepX), locationX, locationY);
					} else {
						warCtrl.OnGenenralHitChecking(side, damage, WarSceneController.Direction.Right, (int)(hitDistance/locationStepX), locationX, locationY);
					}
				} else {
					int damage = Random.Range(gInfo.strength / 12 - 3, gInfo.strength / 12 + 3);
					if (transform.localScale.x > 0) {
						warCtrl.OnGenenralHitChecking(side, damage, WarSceneController.Direction.Right, (int)(hitDistance/locationStepX), locationX, locationY);
					} else {
						warCtrl.OnGenenralHitChecking(side, damage, WarSceneController.Direction.Left, (int)(hitDistance/locationStepX), locationX, locationY);
					}
				}
			}
		} else {
			
			hadCheckAttack = false;
			isFighting = false;
			timeCoolDown = (1 - gInfo.strength / 120f) / 2;
			
			head.SetFrame("Idle", 0);
			body.SetFrame("Idle", 0);
			horse.SetFrame("Idle", 0);
		}
	}

	void CheckLeftGeneralMagicRelease() {
		if (side == WarSceneController.WhichSide.Left && IsCanReleaseMagic()) {

			for (int i=3; i>=0; i--) {
				if (gInfo.magic[i] != -1) {
					MagicDataInfo info = MagicManager.Instance.GetMagicDataInfo(gInfo.magic[i]);
					
					if (gInfo.manaCur >= info.MP) {
						MagicController.Instance.SetMagic(gInfo.magic[i], side);

						warCtrl.SetIsSwordFull(side, false);
						break;
					}
				}
			}
		}
	}

	void SetMapLocation() {
		 mapPoint.localPosition = new Vector3(transform.localPosition.x / manPosMaxX * mapPointMaxX,
					transform.localPosition.y / manPosMaxY * mapPointMaxY, 0);
	}
	
	void CheckLocationState() {

		locationX = warCtrl.GetLocationPositionX(transform.localPosition.x);
		locationY = warCtrl.GetLocationPositionY(transform.localPosition.y);
		
		SetMapLocation();

		if (state == State.Magic 
		    || WarSceneController.state == WarSceneController.State.Dead 
		    || WarSceneController.state == WarSceneController.State.End) return;

		isStopped = false;
		bool flag = false;
		
		if (Random.Range(0, 100) > 50) {
			flag = true;
			for (int i=locationX+1; i<=locationX+(int)(hitDistance/locationStepX); i++) {
				if (i >= manPosMaxX*2/locationStepX) {
					continue;
				}
				if (warCtrl.GetLocationFlag(i, locationY) != 0) {
					if (side == WarSceneController.WhichSide.Left) {
						if (warCtrl.GetLocationSide(i, locationY) == side) {
							if (state != State.Escape && i==locationX+1) 
								isStopped = true;
						} else {
							if (state != State.Escape)
								SetAttackFront();
							break;
						}
					} else {
						if (warCtrl.GetLocationSide(i, locationY) != side) {
							if (state != State.Escape)
								SetAttackBack();
							else
								SetAttackFront();
						} else {
							if (state == State.Escape && i==locationX+1) 
								isStopped = true;
						}
					}
				}
			}
		}
		
		if (isFighting) return;
		
		for (int i=locationX-(int)(hitDistance/locationStepX); i<locationX; i++) {
			if (i <= 0) {
				continue;
			}
			if (warCtrl.GetLocationFlag(i, locationY) != 0) {
				if (side == WarSceneController.WhichSide.Right) {
					if (warCtrl.GetLocationSide(i, locationY) == side) {
						if(state != State.Escape && i==locationX-1) 
							isStopped = true;
					} else {
						if (state != State.Escape)
							SetAttackFront();
						break;
					}
				} else {
					if (warCtrl.GetLocationSide(i, locationY) != side) {
						if (state != State.Escape)
							SetAttackBack();
						else
							SetAttackFront();
					} else {
						if (state == State.Escape && i==locationX-1) 
							isStopped = true;
					}
				}
			}
		}
		
		if (isFighting) return;
		
		if (!flag) {
			for (int i=locationX+1; i<=locationX+(int)(hitDistance/locationStepX); i++) {
				if (i >= manPosMaxX*2/locationStepX) {
					continue;
				}
				if (warCtrl.GetLocationFlag(i, locationY) != 0) {
					if (side == WarSceneController.WhichSide.Left) {
						if (warCtrl.GetLocationSide(i, locationY) == side) {
							if (state != State.Escape && i==locationX+1) 
								isStopped = true;
						} else {
							if (state != State.Escape)
								SetAttackFront();
							break;
						}
					} else {
						if (warCtrl.GetLocationSide(i, locationY) != side) {
							if (state != State.Escape)
								SetAttackBack();
							else
								SetAttackFront();
						} else {
							if (state == State.Escape && i==locationX+1) 
								isStopped = true;
						}
					}
				}
			}
		}
	}
	
	void SetAttackFront() {
		isFighting = true;
		isHitFront = true;
		isParrying = false;
		isHited = false;
		timeParry = 0;
		
		int rand = Random.Range(0, 100);
		int type = rand / 25;
		
		if (type == 3) {
			isMightyHit = true;
		}
		
		string anim = "AttackFront" + (type+1);
		
		head.Play(anim);
		body.Play(anim);
		horse.Play("Fight");

		SoundController.Instance.PlaySound3D("00033", transform.position);
	}
	
	void SetAttackBack() {
		isFighting = true;
		isHitFront = false;
		isParrying = false;
		isHited = false;
		timeParry = 0;
		
		head.Play("AttackBack");
		body.Play("AttackBack");
		horse.Play("Fight");

		SoundController.Instance.PlaySound3D("00033", transform.position);
	}
	
	public void SetIdle() {
		state = State.Idle;
		
		head.Play("Idle");
		body.Play("Idle");
		horse.Play("Idle");
	}
	
	public void SetRun() {
		state = State.Running;
		
	}
	
	public void SetEscape() {
		
		state = State.Escape;
		transform.localScale = new Vector3(-transform.localScale.x, 
			transform.localScale.y, transform.localScale.z);
	}
	
	public void SetArmyAssault() {
		
		head.Stop();
		body.Stop();
		horse.Stop();
		
		head.SetFrame("Misc", 1);
		body.SetFrame("Misc", 1);
		horse.SetFrame("Idle", 0);

		armyAssaultFlag = true;
	}

	public void SetGeneralWaitForMagic(float time) {

		stateBK = state;
		state = State.Magic;
		magicTime = time;

		head.Play("Idle");
		body.Play("Idle");
		horse.Play("Idle");
	}

	public void SetOnMagic(int animType) {

		isParrying = false;
		isFighting = false;
		isHited = false;

		head.Stop();
		body.Stop();
		horse.Stop();

		switch (animType) {
		case 0:
		case 1:
			head.SetFrame("Misc", animType);
			body.SetFrame("Misc", animType);
			break;
		case 2:
			isMagicArrow = true;
			head.SetFrame("Arrow", 0);
			body.SetFrame("Arrow", 0);
			head.Play("Arrow");
			body.Play("Arrow");
			break;
		case 3:
			isMagicArrow = true;
			head.SetFrame("Dart", 0);
			body.SetFrame("Dart", 0);
			head.Play("Dart");
			body.Play("Dart");
			break;
		case 4:
			isMagicArrow = true;
			head.SetFrame("AttackFront2", 0);
			body.SetFrame("AttackFront2", 0);
			head.Play("AttackFront2");
			body.Play("AttackFront2");
			break;
		case 5:
			isMagicArrow = true;
			head.SetFrame("AttackFront4", 0);
			body.SetFrame("AttackFront4", 0);
			head.Play("AttackFront4");
			body.Play("AttackFront4");
			break;
		}

		horse.SetFrame("Idle", 0);
	}

	public void OnMagicReturn() {
		if (WarSceneController.state != WarSceneController.State.End 
		    && WarSceneController.state != WarSceneController.State.Dead) {
			state = stateBK;
		}
		
		head.Play("Idle");
		body.Play("Idle");
		horse.Play("Idle");
	}

	public bool IsCanReleaseMagic() {

		if (side == WarSceneController.WhichSide.Left && WarSceneController.state != WarSceneController.State.Running)
			return false;

		if (!warCtrl.IsSwordFull(side))
			return false;

		if (state == State.Escape)
			return false;

		for (int i=0; i<4; i++) {
			if (gInfo.magic[i] != -1) {
				MagicDataInfo info = MagicManager.Instance.GetMagicDataInfo(gInfo.magic[i]);

				if (gInfo.manaCur >= info.MP) {
					return true;
				}
			} else {
				return false;
			}
		}

		return false;
	}

	public void SetOnFire() {
		if (state == State.Dead) return;
		
		string fireName = "Soliders/Other/SoliderFire";
		GameObject go = (GameObject)Instantiate(Resources.Load(fireName));
		go.transform.parent = body.transform;
		go.transform.localPosition = new Vector3(0, 0, -2);
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;
		
		exSpriteAnimation fireAnim = go.GetComponent<exSpriteAnimation>();
		fireAnim.PlayDefault();

		Destroy(go, 2);
	}

	public bool OnDamage(int d, int type, bool isFire) {
		if (WarSceneController.state == WarSceneController.State.Dead || WarSceneController.state == WarSceneController.State.End)
			return false;
		if (isFire) {
			if (body.transform.childCount > 0) {
				return false;
			}
			SetOnFire();
		}
		return OnDamage(d, type);
	}

	bool OnDamage(int d, int type) {
		
		if (state == State.Dead || state == State.Magic) {
			return false;
		}
		
		bool flag = true;
		
		if (type == 0) {
			int difference = 0;
			if (side == WarSceneController.WhichSide.Left) {
				difference = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx).strength
					- Informations.Instance.GetGeneralInfo(WarSceneController.leftGeneralIdx).strength;
			} else {
				difference = Informations.Instance.GetGeneralInfo(WarSceneController.leftGeneralIdx).strength
					- Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx).strength;
			}
			difference = Mathf.Clamp(difference, -20, 20);
			if (Random.Range(0, 100) > 60 + difference) {
				flag = false;
			}
		} else if (type == 1) {
			if (Random.Range(0, 100) > 70 - gInfo.strength / 5) {
				flag = false;
			}
		} else if (type == -1) {
			flag = true;
		}
		
		if (!flag) {
			
			if (!isFighting) {
				isParrying = true;
				timeParry = 0.2f;
				
				head.Stop();
				body.Stop ();
				horse.Stop();
				
				head.SetFrame("Misc", 2);
				body.SetFrame("Misc", 2);
				horse.SetFrame("Idle", 0);

				SoundController.Instance.PlaySound3D("00035", transform.position);
			}
			return false;
		}

		SoundController.Instance.PlaySound3D("00036", transform.position);

		gInfo.healthCur -= d;
		if (gInfo.healthCur <= 0) {
			gInfo.healthCur = 0;
			
			if (state == State.Escape) {
				transform.localScale = new Vector3(-transform.localScale.x,
					transform.localScale.y, transform.localScale.z);
			}
			
			horse.transform.localScale = new Vector3(-horse.transform.localScale.x,
				horse.transform.localScale.y, horse.transform.localScale.z);
			horse.transform.parent = transform.parent;
			horse.Play("Run");
			
			head.Stop();
			body.Stop();
			head.SetFrame("Dead", 0);
			body.SetFrame("Dead", 0);
			head.transform.localPosition += deadBodyOffset;
			body.transform.localPosition += deadBodyOffset;
			
			weapon.gameObject.SetActive(true);
			weapon.transform.localPosition += weaponOffset;
			weapon.PlayDefault();
			
			state = State.Dead;
			timeTick = 0;
			
			warCtrl.OnGeneralDead(side);
		} else {
			if (!isFighting) {
				isHited = true;
				
				head.Play("Hit");
				body.Play("Hit");
				horse.Stop();
				horse.SetFrame("Idle", 0);
			}
		}
		
		return true;
	}
	
	public bool IsDead() {
		
		if (state == State.Dead) {
			return true;
		}
		
		return false;
	}
	
	public void SetPause() {
		head.Pause();
		body.Pause();
		horse.Pause();
	}

	public void SetResume() {
		head.Resume();
		body.Resume();
		horse.Resume();
	}

	public State GetState() {
		return state;
	}

	public bool GetArmyAssaultFlag() {
		return armyAssaultFlag;
	}
}
