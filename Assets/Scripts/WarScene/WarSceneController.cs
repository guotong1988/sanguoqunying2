using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarSceneController : MonoBehaviour {
	
	public enum State {
		Beginning,
		Running,
		Magic,
		Menu,
		Assault,
		Dead,
		End
	}
	
	public enum WhichSide {
		None,
		Left,
		Right
	}
	
	public enum Direction {
		Left,
		Right
	}
	
	public static int leftGeneralIdx;
	public static int rightGeneralIdx;
	public static int leftDefense;
	public static int rightDefense;
	public static int rightGeneralPosition;
	
	public static State state;

	public static bool isGamePause;

	public static bool isEscape;

	public static float cameraPosMaxX = 1000;
	public static float cameraPosMaxY = 180;
	public static Vector3 cameraPosCur;
	public static float manPosMaxX = 1100;
	public static float manPosMaxY = 200;
	public static float locationStepX = 12;
	public static float locationStepY = 8;
	public static Vector3 manEulerAngles = new Vector3(290, 0, 0);
	public static Vector3 manScaleLeft = new Vector3(-0.5f, 0.5f, 1);
	public static Vector3 manScaleRight = new Vector3(0.5f, 0.5f, 1);
	
	public static int soliderMinPosLeft;
	public static int soliderMaxPosRight;
	
	public Transform groundCam;
	public Transform manCam;
	public GameObject commandsMenu;
	public WSDialogue dialog;
	public WSInfoPanel infoPanel;
	public GeneralController leftGeneral;
	public GeneralController rightGeneral;
	
	private List<SolidersController> leftSoliders;
	private List<SolidersController> rightSoliders;
	private List<SolidersController> leftFrontSoliders;
	private List<SolidersController> leftBackSoliders;
	private List<SolidersController> rightFrontSoliders;
	private List<SolidersController> rightBackSoliders;
	private List<SolidersHorse> soliderHorse;
	
	private bool isLeftSwordFull = false;
	private bool isRightSwordFull = false;
	
	private int step;
	private bool isCameraMoving;
	private float cameraTimeTick;
	private Vector3 cameraPosDest;
	private Vector3 mouseDownPos;
	private bool isMouseMove;
	private bool isMouseDown;
	private WhichSide loser;
	
	private int[,] locationFlag;
	private Vector3 scale = new Vector3 (640f/Screen.width, 480f/Screen.height, 0);
	
	void Awake() {
		//test
		if (leftGeneralIdx == 0 && rightGeneralIdx == 0) {
			leftGeneralIdx = Random.Range(0, 200);
			rightGeneralIdx = Random.Range(0, 200);
//			Informations.Instance.GetGeneralInfo(leftGeneralIdx).armsCur = 0x08;
//			Informations.Instance.GetGeneralInfo(leftGeneralIdx).armsCur = 0x40;
			Informations.Instance.GetGeneralInfo(leftGeneralIdx).soldierCur = Random.Range(0, 50);
			Informations.Instance.GetGeneralInfo(rightGeneralIdx).soldierCur = Random.Range(0, 50);
			Informations.Instance.GetGeneralInfo(leftGeneralIdx).knightCur = Random.Range(0, 50);
			Informations.Instance.GetGeneralInfo(rightGeneralIdx).knightCur = Random.Range(0, 50);

			Informations.Instance.GetGeneralInfo(leftGeneralIdx).soldierMax = 50;
			Informations.Instance.GetGeneralInfo(rightGeneralIdx).soldierMax = 50;
			Informations.Instance.GetGeneralInfo(leftGeneralIdx).knightMax = 50;
			Informations.Instance.GetGeneralInfo(rightGeneralIdx).knightMax = 50;
//
//			Informations.Instance.GetGeneralInfo(leftGeneralIdx).formationCur = 0x02;
//			Informations.Instance.GetGeneralInfo(rightGeneralIdx).formationCur = 0x02;
		}
		//state = State.Running;
		
		locationFlag 		= new int[(int)(manPosMaxX*2/locationStepX)+1, (int)(manPosMaxY*2/locationStepY)+1];
		leftSoliders 		= new List<SolidersController>();
		rightSoliders 		= new List<SolidersController>();
		leftFrontSoliders 	= new List<SolidersController>();
		leftBackSoliders 	= new List<SolidersController>();
		rightFrontSoliders	= new List<SolidersController>();
		rightBackSoliders	= new List<SolidersController>();
		soliderHorse		= new List<SolidersHorse>();

		SetCameraPosition(new Vector3(0, -30, 0));
	}
	
	void Start () {
		Init();

		MagicManager.Instance.LoadConfig();
		MagicController.Instance.warCtrl = this;
		MagicController.Instance.magicRoot = GameObject.Find("MagicRoot").transform;

		if (Random.Range(0, 100) < 50) {
			SoundController.Instance.PlaySound("00002");
		}

		int musicIdx = Random.Range(0, 8);
		string musicName = "0" + (musicIdx + 1);
		SoundController.Instance.PlayBackgroundMusic(musicName);
	}
	
	// Update is called once per frame
	void Update () {
	
		OnCameraMoving();
		
		switch (state) {
		case State.Beginning:
			OnBeginningHandler();
			break;
		case State.Running:
			OnRunningHandler();
			break;
		case State.Assault:
			OnAssaultHandler();
			break;
		case State.Magic:
			OnMagicHandler();
			break;
		case State.End:
			OnEndController();
			break;
		}
	}
	
	void LateUpdate() {
		
		int w = (int)(manPosMaxX*2/locationStepX) + 1;
		int h = (int)(manPosMaxY*2/locationStepY) + 1;
		for (int i=0; i<w; i++) {
			for (int j=0; j<h; j++) {
				locationFlag[i, j] = 0;
			}
		}
		
		int x = 0;
		int y = 0;
		soliderMinPosLeft = 0;
		soliderMaxPosRight = 0;
		
		if (leftGeneral != null) {
			x = GetLocationPositionX(leftGeneral.transform.localPosition.x);
			y = GetLocationPositionY(leftGeneral.transform.localPosition.y);
			locationFlag[x, y] = -1000;
			soliderMinPosLeft = x;
		}
		
		if (rightGeneral != null) {
			x = (int)((rightGeneral.transform.localPosition.x + manPosMaxX) / locationStepX);
			y = (int)((rightGeneral.transform.localPosition.y + manPosMaxY) / locationStepY);
			locationFlag[x, y] = 1000;
			soliderMaxPosRight = x;
		}
		
		for (int i=leftSoliders.Count-1; i>=0; i--) {
			if (leftSoliders[i] != null && leftSoliders[i].GetState() != SolidersController.State.Dead) {
				x = leftSoliders[i].GetLocationX();
				y = leftSoliders[i].GetLocationY();
				
				locationFlag[x, y] = leftSoliders[i].GetIndex();
				if (soliderMinPosLeft > x) {
					soliderMinPosLeft = x;
				}
			} else {
				leftSoliders.RemoveAt(i);
			}
		}
		
		for (int i=rightSoliders.Count-1; i>=0; i--) {
			if (rightSoliders[i] != null && rightSoliders[i].GetState() != SolidersController.State.Dead) {
				x = rightSoliders[i].GetLocationX();
				y = rightSoliders[i].GetLocationY();
				locationFlag[x, y] = rightSoliders[i].GetIndex();
				if (soliderMaxPosRight < x) {
					soliderMaxPosRight = x;
				}
			} else {
				rightSoliders.RemoveAt(i);
			}
		}
		
		int maxIdxX = (int)(manPosMaxX*2/locationStepX);
		int maxIdxY = (int)(manPosMaxY*2/locationStepY);
		
		for (int i=0; i<leftSoliders.Count; i++) {
			if (leftSoliders[i].GetState() == SolidersController.State.Running) {
				
				if (rightSoliders.Count == 0) {
					leftSoliders[i].SetTargetEnemy(rightGeneral.transform, true);
					continue;
				}
				
				Transform t = leftSoliders[i].GetTargetEnemy();
				if (t != null && Vector3.Distance(t.localPosition, leftSoliders[i].transform.localPosition) < 100) {
					continue;
				}
				
				x = leftSoliders[i].GetLocationX();
				y = leftSoliders[i].GetLocationY();
				
				bool flag = false;
				int m = 0;
				int n = 0;
				for (int p=0; p<5; p++) {
					for (int q=0; q<10; q++) {
						m = x - p - 1;
						if (q % 2 == 0) {
							n = y + q / 2 + 1;
						} else {
							n = y - q / 2 - 1;
						}
						
						if (m < 0 || m >= maxIdxX || n < 0 || n >= maxIdxY) continue;
						if (GetLocationIsEnemy(WhichSide.Left, m, n)) {
							for (int j=0; j<rightSoliders.Count; j++) {
								if (rightSoliders[j].GetIndex() == locationFlag[m,n]) {
									if (rightSoliders[j].GetTargetEnemy() == null) {
										leftSoliders[i].SetTargetEnemy(rightSoliders[j].transform, false);
										rightSoliders[j].SetTargetEnemy(leftSoliders[i].transform, false);
										flag = true;
										break;
									}
								}
							}
							if (flag) break;
						}
					}
					if (flag) break;
				}
				
				if (flag) continue;
				
				for (int p=x+1; p<maxIdxX; p++) {
					for (int q=0; q<10; q++) {
						m = p;
						if (q % 2 == 0) {
							n = y + q / 2 + 1;
						} else {
							n = y - q / 2 - 1;
						}
						if (m < 0 || m >= maxIdxX || n < 0 || n >= maxIdxY) continue;
						if (GetLocationIsEnemy(WhichSide.Left, m, n)) {
							for (int j=0; j<rightSoliders.Count; j++) {
								if (rightSoliders[j].GetIndex() == locationFlag[m,n]) {
									if (rightSoliders[j].GetTargetEnemy() == null) {
										leftSoliders[i].SetTargetEnemy(rightSoliders[j].transform, false);
										rightSoliders[j].SetTargetEnemy(leftSoliders[i].transform, false);
										flag = true;
										break;
									}
								}
							}
							if (flag) break;
						}
					}
					if (flag) break;
				}
				
				if (flag) continue;
				
				if (leftSoliders[i].GetTargetEnemy() != null) continue;
				leftSoliders[i].SetTargetEnemy(rightSoliders[Random.Range(0, rightSoliders.Count)].transform, false);
			}
		}
		
		for (int i=0; i<rightSoliders.Count; i++) {
			if (rightSoliders[i].GetState() == SolidersController.State.Running) {
				
				if (leftSoliders.Count == 0) {
					rightSoliders[i].SetTargetEnemy(leftGeneral.transform, true);
					continue;
				}
				
				Transform t = rightSoliders[i].GetTargetEnemy();
				if (t != null && Vector3.Distance(t.localPosition, rightSoliders[i].transform.localPosition) < 100) {
					continue;
				}
				
				x = rightSoliders[i].GetLocationX();
				y = rightSoliders[i].GetLocationY();
				
				bool flag = false;
				int m = 0;
				int n = 0;
				for (int p=0; p<5; p++) {
					for (int q=0; q<10; q++) {
						m = x + p + 1;
						if (q % 2 == 0) {
							n = y + q / 2 + 1;
						} else {
							n = y - q / 2 - 1;
						}
						if (m < 0 || m >= maxIdxX || n < 0 || n >= maxIdxY) continue;
						if (GetLocationIsEnemy(WhichSide.Right, m, n)) {
							for (int j=0; j<leftSoliders.Count; j++) {
								if (leftSoliders[j].GetIndex() == locationFlag[m,n]) {
									if (leftSoliders[j].GetTargetEnemy() == null) {
										rightSoliders[i].SetTargetEnemy(leftSoliders[j].transform, false);
										leftSoliders[j].SetTargetEnemy(rightSoliders[i].transform, false);
										flag = true;
										break;
									}
								}
							}
							if (flag) break;
						}
					}
					if (flag) break;
				}
				
				if (flag) continue;
				
				for (int p=x-1; p>0; p--) {
					for (int q=0; q<10; q++) {
						m = p;
						if (q % 2 == 0) {
							n = y + q / 2 + 1;
						} else {
							n = y - q / 2 - 1;
						}
						if (m < 0 || m >= maxIdxX || n < 0 || n >= maxIdxY) continue;
						if (GetLocationIsEnemy(WhichSide.Right, m, n)) {
							for (int j=0; j<leftSoliders.Count; j++) {
								if (leftSoliders[j].GetIndex() == locationFlag[m,n]) {
									if (leftSoliders[j].GetTargetEnemy() == null) {
										rightSoliders[i].SetTargetEnemy(leftSoliders[j].transform, false);
										leftSoliders[j].SetTargetEnemy(rightSoliders[i].transform, false);
										flag = true;
										break;
									}
								}
							}
							if (flag) break;
						}
					}
					if (flag) break;
				}
				
				if (flag) continue;
				
				if (rightSoliders[i].GetTargetEnemy() != null) continue;
				rightSoliders[i].SetTargetEnemy(leftSoliders[Random.Range(0, leftSoliders.Count)].transform, false);
			}
		}
	}
	
	void Init() {
		
		state = State.Beginning;
		step = 0;

		GeneralInfo gInfo;
		gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.rightGeneralIdx);
		gInfo.healthCur += 10;
		gInfo.healthCur = Mathf.Clamp(gInfo.healthCur, 0, gInfo.healthMax);
		gInfo.manaCur += 5;
		gInfo.manaCur = Mathf.Clamp(gInfo.manaCur, 0, gInfo.manaMax);
		
		gInfo = Informations.Instance.GetGeneralInfo(WarSceneController.leftGeneralIdx);
		gInfo.healthCur += 10;
		gInfo.healthCur = Mathf.Clamp(gInfo.healthCur, 0, gInfo.healthMax);
		gInfo.manaCur += 5;
		gInfo.manaCur = Mathf.Clamp(gInfo.manaCur, 0, gInfo.manaMax);

		SetCameraMoveTo(new Vector3(-600, -30, 0));
	}
	
	void OnBeginningHandler() {
		
		switch (step) {
		case 0:
			if (!isCameraMoving) {
				step = 1;

				OnPauseGame();

				int textIdx = Random.Range(0, 12);
				string text = "";
				if (textIdx < 4) {
					text = ZhongWen.Instance.chuzhan1[textIdx];
				} else {
					textIdx -= 4;
					text = ZhongWen.Instance.chuzhan2[textIdx*2] + ZhongWen.Instance.GetGeneralName(leftGeneralIdx) + ZhongWen.Instance.chuzhan2[textIdx*2+1];
				}
				dialog.SetDialogue(text, WhichSide.Left);
			}
			break;
		case 1:
			if (!dialog.IsShowingText()) {
				if (Input.GetMouseButtonUp(0)) {
					step = 2;
					dialog.gameObject.SetActive(false);
					SetCameraMoveTo(new Vector3(rightGeneral.transform.localPosition.x, -30, 0));
				}
			}
			break;
		case 2:
			if (!isCameraMoving) {
				step = 3;
				
				int textIdx = Random.Range(0, 12);
				string text = "";
				if (textIdx < 4) {
					text = ZhongWen.Instance.chuzhan1[textIdx];
				} else {
					textIdx -= 4;
					text = ZhongWen.Instance.chuzhan2[textIdx*2] + ZhongWen.Instance.GetGeneralName(rightGeneralIdx) + ZhongWen.Instance.chuzhan2[textIdx*2+1];
				}
				dialog.SetDialogue(text, WhichSide.Right);
			}
			break;
		case 3:
			if (!dialog.IsShowingText()) {
				if (Input.GetMouseButtonUp(0)) {
					step = 0;
					dialog.gameObject.SetActive(false);
					SetGameMenu();
				}
			}
			break;
		}
	}
	
	void OnAssaultHandler() {
		
		switch (step) {
		case 0:
			if (!isCameraMoving) {
				step = 1;
				dialog.gameObject.SetActive(true);
			}
			break;
		case 1:
			if (!dialog.IsShowingText()) {
				if (Input.GetMouseButtonUp(0)) {
					state = State.Running;

					OnResumeGame();
					dialog.gameObject.SetActive(false);
					Input.ResetInputAxes();
				}
			}
			break;
		}
	}
	
	void OnEndController() {

		if (!dialog.IsShowingText()) {
			if (Input.GetMouseButtonDown(0)) {
				
				if (loser == WhichSide.Left) {
					if (!rightGeneral.IsDead()) {
						SelectGeneralToWarController.warResult = 0;
					} else {
						SelectGeneralToWarController.warResult = 2;
					}
				} else if (loser == WhichSide.Right) {
					if (!leftGeneral.IsDead()) {
						SelectGeneralToWarController.warResult = 1;
					} else {
						SelectGeneralToWarController.warResult = 2;
					}
				}

				Time.timeScale = 1;
				Application.LoadLevel("SelectGeneralToWar");
			}
		}
	}
	
	void OnRunningHandler() {
		
		if (Input.GetMouseButtonDown(0)) {
			
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			
			if (pos.x < 40f/640f || pos.x > 1-40f/640f || pos.y < 120f/480f) {
				
				isMouseDown = false;
				return;
			}
			
			isMouseDown = true;
			isMouseMove = false;
			mouseDownPos = Input.mousePosition;
			
		} else if (isMouseDown && !isMouseMove && Input.GetMouseButtonUp(0)) {
			
			SetGameMenu();
		} else if (isMouseDown && Input.GetMouseButton(0)) {
			if (!isMouseMove) {
				
				Vector3 offset = mouseDownPos - Input.mousePosition;
				offset.Scale(scale);
				
				if (offset.magnitude > 5) {
					
					isMouseMove = true;
					mouseDownPos = Input.mousePosition;
				}
			} else {
				
				Vector3 offset = mouseDownPos - Input.mousePosition;
				mouseDownPos = Input.mousePosition;
				offset.Scale(scale);
				
				cameraPosCur += offset;
				SetCameraPosition(cameraPosCur);
			}
		}
	}

	void OnMagicHandler() {

	}

	void OnCameraMoving() {
		
		if (!isCameraMoving)
			return;
		
		cameraPosCur = new Vector3(Mathf.Lerp(cameraPosCur.x, cameraPosDest.x, 0.2f),
			Mathf.Lerp(cameraPosCur.y, cameraPosDest.y, 0.2f), 0);
		
		if ((Time.realtimeSinceStartup - cameraTimeTick) > 0.5f) {
			cameraTimeTick = 0;
			isCameraMoving = false;
			cameraPosCur = cameraPosDest;
		}
		
		groundCam.localPosition = cameraPosCur;
		manCam.localPosition = cameraPosCur;
	}
	
	void SetGameMenu() {

		if (!commandsMenu.activeSelf) {
			state = State.Menu;

			OnPauseGame();
			
			commandsMenu.SetActive(true);
		}
	}

	Vector2 GetLocationIdx(Vector3 pos) {
		
		Vector2 loc = new Vector2();
		loc.x = (pos.x + manPosMaxX) / locationStepX;
		loc.y = (pos.y + manPosMaxY) / locationStepY;
		
		return loc;
	}
	
	public void OnTimeUp() {
		SelectGeneralToWarController.warResult = 2;

		Time.timeScale = 1;
		Application.LoadLevel("SelectGeneralToWar");
	}

	public void SetIsSwordFull(WhichSide side, bool flag) {
		if (side == WhichSide.Left) {
			isLeftSwordFull = flag;

			if (!isLeftSwordFull) {
				infoPanel.ResetSwordValue(WhichSide.Left);
			}
		} else {
			isRightSwordFull = flag;

			if (!isRightSwordFull) {
				infoPanel.ResetSwordValue(WhichSide.Right);
			}
		}
	}

	public bool IsSwordFull(WhichSide side) {
		if (side == WhichSide.Left) {
			return isLeftSwordFull;
		} else {
			return isRightSwordFull;
		}
	}
	
	public void SetCameraPosition(Vector3 pos) {
		
		pos.x = Mathf.Clamp(pos.x, -cameraPosMaxX, cameraPosMaxX);
		pos.y = Mathf.Clamp(pos.y, -cameraPosMaxY, cameraPosMaxY);
		
		cameraPosCur = pos;
		groundCam.localPosition = cameraPosCur;
		manCam.localPosition = cameraPosCur;
	}
	
	public void SetCameraMoveTo(Vector3 pos) {
		
		pos.x = Mathf.Clamp(pos.x, -cameraPosMaxX, cameraPosMaxX);
		pos.y = Mathf.Clamp(pos.y, -cameraPosMaxY, cameraPosMaxY);
		
		isCameraMoving = true;
		cameraTimeTick = Time.realtimeSinceStartup;
		cameraPosDest = pos;
	}
	
	public bool IsCameraMoving() {
		return isCameraMoving;
	}
	
	public int GetLocationFlag(int x, int y) {
		return locationFlag[x, y];
	}
	
	public WhichSide GetLocationSide(int x, int y) {
		
		if (locationFlag[x, y] == 0) {
			return WhichSide.None;
		} if (locationFlag[x, y] < 0) {
			return WhichSide.Left;
		} else {
			return WhichSide.Right;
		}
	}
	
	public bool GetLocationIsEnemy(WhichSide s, int x, int y) {
		
		if (locationFlag[x, y] == 0) {
			return false;
		}
		
		if (s == WhichSide.Left && locationFlag[x, y] > 0) {
			return true;
		} else if (s == WhichSide.Right && locationFlag[x, y] < 0) {
			return true;
		}
		
		return false;
	}
	
	public void AddSolider(WhichSide side, SolidersController sCtrl) {
		
		if (side == WhichSide.Left) {
			leftSoliders.Add(sCtrl);
		} else {
			rightSoliders.Add(sCtrl);
		}
	}
	
	public void AddFrontSolider(WhichSide side, SolidersController sCtrl) {
		
		if (side == WhichSide.Left) {
			leftFrontSoliders.Add(sCtrl);
		} else {
			rightFrontSoliders.Add(sCtrl);
		}
	}
	
	public void AddBackSolider(WhichSide side, SolidersController sCtrl) {
		
		if (side == WhichSide.Left) {
			leftBackSoliders.Add(sCtrl);
		} else {
			rightBackSoliders.Add(sCtrl);
		}
	}
	
	public void RemoveSolider(int idx) {
		
		if (idx < 0) {
			for (int i=0; i<leftSoliders.Count; i++) {
				if (leftSoliders[i].GetIndex() == idx) {
					leftFrontSoliders.Remove(leftSoliders[i]);
					leftBackSoliders.Remove(leftSoliders[i]);
					if (leftSoliders[i].GetIsKnight()) {
						Informations.Instance.GetGeneralInfo(leftGeneralIdx).knightCur--;
					} else {
						Informations.Instance.GetGeneralInfo(leftGeneralIdx).soldierCur--;
					}
					
					for (int j=0; j<rightSoliders.Count; j++) {
						if (rightSoliders[j].GetTargetEnemy() == leftSoliders[i].transform) {
							rightSoliders[j].SetTargetEnemy(null, false);
						}
					}
					leftSoliders.RemoveAt(i);
					break;
				}
			}
		} else {
			for (int i=0; i<rightSoliders.Count; i++) {
				if (rightSoliders[i].GetIndex() == idx) {
					rightFrontSoliders.Remove(rightSoliders[i]);
					rightBackSoliders.Remove(rightSoliders[i]);
					if (rightSoliders[i].GetIsKnight()) {
						Informations.Instance.GetGeneralInfo(rightGeneralIdx).knightCur--;
					} else {
						Informations.Instance.GetGeneralInfo(rightGeneralIdx).soldierCur--;
					}
					
					for (int j=0; j<leftSoliders.Count; j++) {
						if (leftSoliders[j].GetTargetEnemy() == rightSoliders[i].transform) {
							leftSoliders[j].SetTargetEnemy(null, false);
						}
					}
					rightSoliders.RemoveAt(i);
					break;
				}
			}
		}
	}

	public SolidersController GetRandomSolider(WhichSide side) {
		if (side == WhichSide.Left) {
			if (leftSoliders.Count > 0) {
				return leftSoliders[Random.Range(0, leftSoliders.Count)];
			} else {
				return null;
			}
		} else {
			if (rightSoliders.Count > 0) {
				return rightSoliders[Random.Range(0, rightSoliders.Count)];
			} else {
				return null;
			}
		}
	}

	public Vector3 GetArmyCentrePoint(WhichSide side) {

		Vector3 ret = Vector3.zero;
		Vector3 sum = new Vector3(0, 0, 0);
		if (side == WhichSide.Left) {
			if (leftSoliders.Count > 0) {
				for (int i=0; i<leftSoliders.Count; i++) {
					sum += leftSoliders[i].transform.localPosition;
				}
				ret = sum / leftSoliders.Count;
			} else {
				ret = leftGeneral.transform.localPosition;
			}
		} else {
			if (rightSoliders.Count > 0) {
				for (int i=0; i<rightSoliders.Count; i++) {
					sum += rightSoliders[i].transform.localPosition;
				}
				ret = sum / rightSoliders.Count;
			} else {
				ret = rightGeneral.transform.localPosition;
			}
		}

		ret.z = 0;
		return ret;
	}

	public void SetSoldierMagicLock(WhichSide side) {

		if (side == WhichSide.Left) {
			for (int i=0; i<leftSoliders.Count; i++) {
				leftSoliders[i].SetOnMagicLock();
			}
		} else {
			for (int i=0; i<rightSoliders.Count; i++) {
				rightSoliders[i].SetOnMagicLock();
			}
		}
	} 

	public void SetArmyIdle() {
		
 		for (int i=0; i<rightSoliders.Count; i++) {
			rightSoliders[i].SetIdle();
		}
	}
	
	public void SetGeneralAssault(WhichSide side) {
		
		if (side == WhichSide.Left) {
			leftGeneral.SetRun();
		} else {
			rightGeneral.SetRun();
		}
	}
	
	public void SetArmyAssault(WhichSide side) {
		
		if (side == WhichSide.Left) {
			for (int i=0; i<leftSoliders.Count; i++) {
				leftSoliders[i].SetRun();
			}
			
			leftGeneral.SetArmyAssault();
			SetCameraMoveTo(new Vector3(leftGeneral.transform.localPosition.x,
				leftGeneral.transform.localPosition.y-30, 0));
		} else {
			for (int i=0; i<rightSoliders.Count; i++) {
				rightSoliders[i].SetRun();
			}
			
			rightGeneral.SetArmyAssault();
			SetCameraMoveTo(new Vector3(rightGeneral.transform.localPosition.x,
				rightGeneral.transform.localPosition.y-30, 0));
		}
		
		state = State.Assault;
		step = 0;
		isMouseDown = true;
		isMouseMove = false;

		dialog.SetDialogue(ZhongWen.Instance.quanjuntuji, side);
		dialog.gameObject.SetActive(false);

		OnPauseGame();

		SoundController.Instance.PlaySound("00053");
	}
	
	public void SetArmyEscape(WhichSide side) {
		
		if (side == WhichSide.Left) {
			leftGeneral.SetEscape();
			
			for (int i=0; i<leftSoliders.Count; i++) {
				leftSoliders[i].SetEscape();
			}
		} else {
			rightGeneral.SetEscape();
			
			for (int i=0; i<rightSoliders.Count; i++) {
				rightSoliders[i].SetEscape();
			}
		}
	}
	
	public void SetFrontArmyRun(WhichSide side) {
		
		if (side == WhichSide.Left) {
			for (int i=0; i<leftFrontSoliders.Count; i++) {
				leftFrontSoliders[i].SetRun();
			}
		} else {
			for (int i=0; i<rightFrontSoliders.Count; i++) {
				rightFrontSoliders[i].SetRun();
			}
		}
	}
	
	public void SetFrontArmyBack(WhichSide side) {
		
		if (side == WhichSide.Left) {
			for (int i=0; i<leftFrontSoliders.Count; i++) {
				leftFrontSoliders[i].SetMovingBack();
			}
		} else {
			for (int i=0; i<rightFrontSoliders.Count; i++) {
				rightFrontSoliders[i].SetMovingBack();
			}
		}
	}
	
	public void SetArmyDisperse() {
		
		Vector3 posUp = new Vector3(600-locationStepX, 149, 0);
		Vector3 posDown = new Vector3(600-locationStepX, -149, 0);
		
		for (int i=0; i<rightFrontSoliders.Count; i++) {
			if (i % 2 == 0) {
				rightFrontSoliders[i].SetRandomRun(posUp, true);
				posUp.x -= locationStepX;
			} else {
				rightFrontSoliders[i].SetRandomRun(posDown, true);
				posDown.x -= locationStepX;
			}
		}
	}
	
	public void SetArmyFallIn() {
		
		Vector3 pos = new Vector3(600-locationStepX, 0, 0);
		for (int i=0; i<rightFrontSoliders.Count; i++) {
			rightFrontSoliders[i].SetRandomRun(pos, true);
			pos.x -= locationStepX / 2;
		}
	}
	
	public bool OnGenenralHitChecking(WhichSide side, int damage, Direction dir, int range, int xPos, int yPos) {
		
		bool flag = false;
		int xS = 0;
		int xE = 0;
		if (dir == Direction.Left) {
			xS = xPos - range;
			xE = xPos;
		} else {
			xS = xPos;
			xE = xPos + range;
		}
		
		for (int i=xS; i<=xE; i++) {
			if (GetLocationIsEnemy(side, i, yPos)) {
				flag = true;
				break;
			}
		}
		if (!flag) return false;

		CheckDamage(side, damage, 0, 1000, dir, xS, xE, yPos, false);
		
		return true;
	}
	
	public bool OnSoliderHitChecking(WhichSide side, int type, Direction dir, int range, int xPos, int yPos) {
		
		bool flag = false;
		int xS = 0;
		int xE = 0;
		if (dir == Direction.Left) {
			xS = xPos - range;
			xE = xPos;
		} else {
			xS = xPos;
			xE = xPos + range;
		}
		
		for (int i=xS; i<=xE; i++) {
			if (GetLocationIsEnemy(side, i, yPos)) {
				flag = true;
				break;
			}
		}
		if (!flag) return false;
		
		CheckDamage(side, 2, 1, type, dir, xS, xE, yPos, false);
		
		return true;
	}

	public bool OnMagicHitChecking(WhichSide side, int gDamage, Rect region, bool isFire) {

		Direction dir = (Direction)Random.Range(0, 2);
		int xS = GetLocationPositionX(region.x);
		int xE = GetLocationPositionX(region.x + region.width);
		int yS = GetLocationPositionY(region.y);
		int yE = GetLocationPositionY(region.y + region.height);

		for (int yPos=yS; yPos<=yE; yPos++) {
			CheckDamage(side, gDamage, -1, -1, dir, xS, xE, yPos, isFire);
		}

		return true;
	}

	bool CheckDamage(WhichSide side, int gDamage, int gType, int type, Direction dir, int xS, int xE, int yPos, bool isFire) {

		int x = 0;
		int y = 0;
		
		if (side == WhichSide.Right) {
			
			for (int i=0; i<leftSoliders.Count; i++) {
				x = leftSoliders[i].GetLocationX();
				y = leftSoliders[i].GetLocationY();
				if (y == yPos && x >= xS && x <= xE) {
					leftSoliders[i].OnDamage(type, dir, isFire);
				}
			}
			
			x = GetLocationPositionX(leftGeneral.transform.localPosition.x);
			y = GetLocationPositionY(leftGeneral.transform.localPosition.y);

			if (y == yPos && x >= xS && x <= xE) {
				leftGeneral.OnDamage(gDamage, gType, isFire);
			}
		} else {
			
			for (int i=0; i<rightSoliders.Count; i++) {
				x = rightSoliders[i].GetLocationX();
				y = rightSoliders[i].GetLocationY();
				if (y == yPos && x >= xS && x <= xE) {
					rightSoliders[i].OnDamage(type, dir, isFire);
				}
			}
			
			x = GetLocationPositionX(rightGeneral.transform.localPosition.x);
			y = GetLocationPositionY(rightGeneral.transform.localPosition.y);
			if (y == yPos && x >= xS && x <= xE) {
				rightGeneral.OnDamage(gDamage, gType, isFire);
			}
		}

		return false;
	}

	public int GetLocationPositionX(float x) {
		return (int)((x + manPosMaxX) / locationStepX);
	}

	public int GetLocationPositionY(float y) {
		return (int)((y + manPosMaxY) / locationStepY);
	}

	public void OnWarResult(WhichSide s, bool isEscape) {

		OnPauseGame();

		if (state == State.Magic)
			MagicController.Instance.OnMagicOver();

		state = State.End;
		loser = s;
		WarSceneController.isEscape = isEscape;
		
		string text = "";
		if (!isEscape) {
			text = ZhongWen.Instance.shengli1[Random.Range(0, 7)];
		} else {
			text = ZhongWen.Instance.shengli2[Random.Range(0, 5)];
		}
		
		if (loser == WhichSide.Left) {
			dialog.SetDialogue(text, WhichSide.Right);
			SetCameraMoveTo(rightGeneral.transform.localPosition);

			SoundController.Instance.PlaySound("00047");
		} else if (loser == WhichSide.Right) {
			dialog.SetDialogue(text, WhichSide.Left);
			SetCameraMoveTo(leftGeneral.transform.localPosition);

			SoundController.Instance.PlaySound("00046");
		}
	}
	
	public void OnGeneralDead(WhichSide side) {
		
		state = State.Dead;
		Time.timeScale = 0.5f;
		
		if (side == WhichSide.Left) {
			rightGeneral.SetIdle();
			SetCameraMoveTo(leftGeneral.transform.localPosition);
			
			for (int i=0; i<leftSoliders.Count; i++) {
				leftSoliders[i].SetEscape();
			}
		} else {
			leftGeneral.SetIdle();
			SetCameraMoveTo(rightGeneral.transform.localPosition);
			
			for (int i=0; i<rightSoliders.Count; i++) {
				rightSoliders[i].SetEscape();
			}
		}

		SoundController.Instance.PlaySound("00023");
	}

	public void WaitforMagicMoveToGeneral() {
		Invoke("OnSetMagic", 0.5f);
	}
	void OnSetMagic() {
		MagicController.Instance.OnGeneralSetMagic();
	}

	public void AddSoliderHorse(SolidersHorse horse) {
		soliderHorse.Add(horse);
	}

	public void RemoveSoliderHorse(SolidersHorse horse) {
		soliderHorse.Remove(horse);
	}

	public void OnPauseGame() {
		isGamePause = true;

		leftGeneral.SetPause();
		rightGeneral.SetPause();

		for (int i=0; i<leftSoliders.Count; i++) {
			if (leftSoliders[i] != null)
				leftSoliders[i].SetPause();
		}
		for (int i=0; i<rightSoliders.Count; i++) {
			if (rightSoliders[i] != null)
				rightSoliders[i].SetPause();
		}
		for (int i=0; i<soliderHorse.Count; i++) {
			if (soliderHorse[i] != null)
				soliderHorse[i].SetPause();
		}
	}

	public void OnResumeGame() {
		isGamePause = false;

		leftGeneral.SetResume();
		rightGeneral.SetResume();
		
		for (int i=0; i<leftSoliders.Count; i++) {
			if (leftSoliders[i] != null)
				leftSoliders[i].SetResume();
		}
		for (int i=0; i<rightSoliders.Count; i++) {
			if (rightSoliders[i] != null)
				rightSoliders[i].SetResume();
		}
		for (int i=0; i<soliderHorse.Count; i++) {
			if (soliderHorse[i] != null)
				soliderHorse[i].SetResume();
		}
	}
}

