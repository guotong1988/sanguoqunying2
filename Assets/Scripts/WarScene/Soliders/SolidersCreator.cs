using UnityEngine;
using System.Collections;

public class SolidersCreator : MonoBehaviour {
	
	public WarSceneController.WhichSide side;
	public WarSceneController warCtrl;
	public Transform mapPointPrefab;
	public Transform horsePrefab;
	
	// Use this for initialization
	void Start () {
		
		InstantiateSoliders();
	}

	SolidersController SetSolider(int soliderType, Vector3 pos, int index, bool isKnight) {

		string sName = "Soliders/";
		if (side == WarSceneController.WhichSide.Left) {
			sName += "Red/";
		} else {
			sName += "Green/";
		}

		if (!isKnight) {
			if (soliderType < 9) {
				sName += "Solider00" + (soliderType+1);
			} else {
				sName += "Solider0" + (soliderType+1);
			}
		} else {
			if (soliderType < 9) {
				sName += "Knight00" + (soliderType+1);
			} else {
				sName += "Knight0" + (soliderType+1);
			}
		}
		
		GameObject go = (GameObject)Instantiate(Resources.Load(sName));
		go.transform.parent = transform;

		if (isKnight && soliderType != 6) {
			Transform horse = (Transform)Instantiate(horsePrefab);
			horse.gameObject.AddComponent<SolidersHorse>();
			horse.gameObject.SetActive(false);
			horse.parent = go.transform;
			horse.localPosition = Vector3.zero;
			horse.localScale = Vector3.one;
		}

		go.transform.localPosition = pos;
		go.transform.eulerAngles = WarSceneController.manEulerAngles;
		
		if (side == WarSceneController.WhichSide.Left) {
			go.transform.localScale = WarSceneController.manScaleLeft;
		} else {
			go.transform.localScale = WarSceneController.manScaleRight;
		}
		
		SolidersController soliderCtrl = go.AddComponent<SolidersController>();
		
		Transform mapFrame = GameObject.Find("Map").transform;
		Transform mapPoint = (Transform)Instantiate(mapPointPrefab);
		mapPoint.parent = mapFrame;
		mapPoint.localPosition = Vector3.zero;
		
		soliderCtrl.SetMapPoint(mapPoint);
		soliderCtrl.SetSide(side);
		soliderCtrl.SetType(soliderType);
		soliderCtrl.SetIsKnight(isKnight);
		
		if (side == WarSceneController.WhichSide.Left) {
			soliderCtrl.SetIndex(-(index+1));
		} else {
			soliderCtrl.SetIndex(index+1);
		}
		
		return soliderCtrl;
	}

	void InstantiateSoliders() {
		int gIdx = 0;
		if (side == WarSceneController.WhichSide.Left) {
			gIdx = WarSceneController.leftGeneralIdx;
		} else {
			gIdx = WarSceneController.rightGeneralIdx;
		}
		
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(gIdx);
		
		//test
		//gInfo.soldierCur = Random.Range(1, 100);
		
		if (gInfo.soldierCur == 0 && gInfo.knightCur == 0)
			return;
		
		int soliderType = Misc.GetArmsIdx(gInfo.armsCur);
//		OnFormation6(soliderType, gInfo.soldierCur, gInfo.knightCur);

		switch(gInfo.formationCur) {
		case 0x1:
			OnFormation1(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		case 0x2:
			OnFormation2(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		case 0x4:
			OnFormation1(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		case 0x8:
			OnFormation3(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		case 0x10:
			OnFormation4(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		case 0x20:
			OnFormation5(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		case 0x40:
			OnFormation6(soliderType, gInfo.soldierCur, gInfo.knightCur);
			break;
		}

	}

	#region Formation1
	void OnFormation1(int sType, int sNum, int kNum) {
		
		int num = sNum + kNum;
		int frontNum = Mathf.CeilToInt(num * 7 / 10f);
		int backNum = Mathf.FloorToInt(num * 3 / 10f);
		int fRowNum = 0;
		int bRowNum = 0;
		int[] fRows = new int[7];
		int[] bRows = new int[3];
		
		if (frontNum <= 12) {
			fRowNum = frontNum / 4;
			if (fRowNum == 0) {
				if (frontNum % 4 > 0) {
					fRowNum = 1;
					fRows[0] = frontNum;
				}
			} else if (fRowNum == 1) {
				fRows[0] = 4;
				if (frontNum % 4 > 0) {
					fRowNum= 2;
					fRows[1] = frontNum % 4;
				}
			} else {
				fRows[0] = 4;
				fRows[1] = 4;
				fRows[2] = frontNum % 8;
				if (frontNum % 4 > 0) {
					fRowNum++;
				}
			}
		} else if (frontNum >= 49) {
			fRowNum = 7;
			int left = frontNum % 7;
			int row = frontNum / 7;
			for (int i=0; i<7; i++) {
				if (i < left) { 
					fRows[i] = row + 1;
				} else {
					fRows[i] = row;
				}
			}
		} else {
			for (int i=4; i<=7; i++) {
				if (i*i > frontNum && i*(i-1) <= frontNum) {
					fRowNum = i;
					int left = frontNum % (i-1);
					int row = frontNum / (i-1);
					for (int j=0; j<i-1; j++) {
						fRows[j] = row;
					}
					fRows[i-1] = left;
				} else if (i*i <= frontNum && i*(i+1) > frontNum) {
					fRowNum = i;
					int left = frontNum % i;
					int row = frontNum / i;
					for (int j=0; j<i; j++) {
						if (j < left) {
							fRows[j] = row + 1;
						} else {
							fRows[j] = row;
						}
					}
				}
			}
		}
		
		if (backNum <= 12) {
			bRowNum = backNum / 4;
			if (bRowNum == 0) {
				if (backNum % 4 > 0) {
					bRowNum = 1;
					bRows[0] = backNum;
				}
			} else if (bRowNum == 1) {
				bRows[0] = 4;
				if (backNum % 4 > 0) {
					bRowNum = 2;
					bRows[1] = backNum % 4;
				}
			} else {
				bRows[0] = 4;
				bRows[1] = 4;
				bRows[2] = backNum % 8;
				if (backNum % 4 > 0) {
					bRowNum++;
				}
			}
		} else {
			bRowNum = 3;
			int left = backNum % 3;
			int row = backNum / 3;
			for (int i=0; i<3; i++) {
				if (i < left) {
					bRows[i] = row + 1;
				} else {
					bRows[i] = row;
				}
			}
		}
		
		int soliderIndex = 0;
		int knightNum = kNum;
		int soliderNum = frontNum - knightNum;
		Vector3 posBase = Vector3.zero;
		int xSpace = 4 * (int)WarSceneController.locationStepX;
		int ySpace = 4 * (int)WarSceneController.locationStepY;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+xSpace, 0, 0);
		} else {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+xSpace, 0, 0);
		}
		
		for (int i=0; i<fRowNum; i++) {
			for (int j=0; j<fRows[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
		
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+xSpace, 0, 0);
		} else {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+xSpace, 0, 0);
		}
		
		for (int i=0; i<bRowNum; i++) {
			for (int j=0; j<bRows[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				} else {
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
	}
	#endregion

	#region Formation2
	void OnFormation2(int sType, int sNum, int kNum) {
		
		int num = sNum + kNum;
		int frontNum = Mathf.CeilToInt(num * 4 / 10f);
		int backNum = Mathf.FloorToInt(num * 6 / 10f);
		int fRowNum = 0;
		int bRowNum1 = 0;
		int bRowNum2 = 0;
		int[] fRows = new int[4];
		int[] bRows1 = new int[4];
		int[] bRows2 = new int[4];

		if (num == 1) frontNum = 0;

		if (frontNum <= 12) {
			fRowNum = frontNum / 4;
			if (fRowNum == 0) {
				if (frontNum % 4 > 0) {
					fRowNum = 1;
					fRows[0] = frontNum;
				}
			} else if (fRowNum == 1) {
				fRows[0] = 4;
				if (frontNum % 4 > 0) {
					fRowNum= 2;
					fRows[1] = frontNum % 4;
				}
			} else {
				fRows[0] = 4;
				fRows[1] = 4;
				fRows[2] = frontNum % 8;
				if (frontNum % 4 > 0) {
					fRowNum++;
				}
			}
		} else {
			fRowNum = 4;
			int left = frontNum % 4;
			int row = frontNum / 4;
			for (int i=0; i<4; i++) {
				if (i < left) { 
					fRows[i] = row + 1;
				} else {
					fRows[i] = row;
				}
			}
		}
		
		int backNumCal = backNum + 1;
		int currRow = 0;
		while (backNumCal > 0) {
			
			currRow = 0;
			while (currRow < 3 && bRows1[currRow] - bRows1[currRow+1] > 2)
				currRow++;
			if (currRow == 3 && bRows1[2] - bRows1[3] <= 2) currRow = 0;
			if (currRow+1 > bRowNum1) bRowNum1 = currRow + 1;
			bRows1[currRow]++;
			backNumCal--;
			if (backNumCal == 0) break;
			
			currRow = 0;
			while (currRow < 3 && bRows2[currRow] - bRows2[currRow+1] > 2)
				currRow++;
			if (currRow == 3 && bRows2[2] - bRows2[3] <= 2) currRow = 0;
			if (currRow+1 > bRowNum2) bRowNum2 = currRow + 1;
			bRows2[currRow]++;
			backNumCal--;
			if (backNumCal == 0) break;
		}
		
		int soliderIndex = 0;
		int knightNum = kNum;
		int soliderNum = frontNum - knightNum;
		Vector3 posBase = Vector3.zero;
		int xSpace = 4 * (int)WarSceneController.locationStepX;
		int ySpace = 4 * (int)WarSceneController.locationStepY;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+6*xSpace, 0, 0);
		} else {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+6*xSpace, 0, 0);
		}
		
		for (int i=0; i<fRowNum; i++) {
			for (int j=0; j<fRows[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}

		soliderNum = (backNum+1) / 2 + (backNum+1) % 2 - knightNum;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+xSpace, 0, 0);
		} else {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+xSpace, 0, 0);
		}
		
		for (int i=0; i<bRowNum1; i++) {
			for (int j=0; j<bRows1[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
		
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600, 0, 0);
		} else {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600, 0, 0);
		}
		
		for (int i=0; i<bRowNum2; i++) {
			for (int j=0; j<bRows2[i]; j++) {
				
				if (i == 0 && j == 0) continue;
				
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				} else {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
	}
	#endregion

	#region Formation3
	void OnFormation3(int sType, int sNum, int kNum) {
		
		int num = sNum + kNum;
		int frontNum = Mathf.CeilToInt(num * 7 / 10f);
		int backNum = Mathf.FloorToInt(num * 3 / 10f);
		int fRowNum = 0;
		int bRowNum = 0;
		int[] fRows = new int[5];
		int[] bRows = new int[3];
		
		fRowNum = Mathf.CeilToInt((float)frontNum / 14);
		for (int i=0; i<fRowNum-1; i++) {
			fRows[i] = 14;
		}
		if (fRowNum > 0) {
			fRows[fRowNum-1] = frontNum % 14;
			if (fRows[fRowNum-1] == 0)
				fRows[fRowNum-1] = 14;
		}
		
		bRowNum = Mathf.CeilToInt((float)backNum / 14);
		for (int i=0; i<bRowNum-1; i++) {
			bRows[i] = 14;
		}
		if (bRowNum > 0) {
			bRows[bRowNum-1] = backNum % 14;
			if (bRows[bRowNum-1] == 0)
				bRows[bRowNum-1] = 14;
		}
		
		int soliderIndex = 0;
		int knightNum = kNum;
		int soliderNum = frontNum - knightNum;
		Vector3 posBase = Vector3.zero;
		int xSpace = 4 * (int)WarSceneController.locationStepX;
		int ySpace = 3 * (int)WarSceneController.locationStepY;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+xSpace, 0, 0);
		} else {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+xSpace, 0, 0);
		}
		
		for (int i=0; i<fRowNum; i++) {
			for (int j=0; j<fRows[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*(j/2+i), posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*(j/2+i), posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
		
		soliderNum = backNum - knightNum;
		if (side == WarSceneController.WhichSide.Left) {
			posBase = new Vector3(-600-3*xSpace, 0, 0);
		} else {
			posBase = new Vector3(600-3*xSpace, 0, 0);
		}
		for (int i=0; i<bRowNum; i++) {
			for (int j=0; j<bRows[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*(j/2+i), posBase.y-ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*(j/2+i), posBase.y+ySpace*((int)(j+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
	}
	#endregion

	#region Formation4
	void OnFormation4(int sType, int sNum, int kNum) {
		
		int num = sNum + kNum;
		int [] rowNum = new int[5];
		int[,] rows = new int[5, 5];
		
		while (num > 0) {
			for (int i=0; i<5; i++) {
				rowNum[i]++;
				num--;
				if (num == 0) break;
			}
		}
		
		for (int i=0; i<5; i++) {
			int numCal = rowNum[i];
			int count = 1;
			for (int j=0; j<4; j++) {
				if (numCal > count * count)
					count++;
			}
			if (count == 5) count = 4;
			for (int n=0; n<5; n++) {
				for (int m=0; m<count; m++) {
					rows[i, n]++;
					numCal--;
					if (numCal == 0) break;
				}
				if (numCal == 0) break;
			}
		}
		
		int soliderIndex = 0;
		int knightNum = kNum;
		int soliderNum = sNum;
		Vector3 posBase = Vector3.zero;
		Vector2[] off = new Vector2[5]{new Vector2(-5, 0), 
			new Vector2(1, 0), new Vector2(1, -6), 
			new Vector2(1, 6), new Vector2(7, 0)};
		int xBase = -600;
		float xSpace = 3.5f * (int)WarSceneController.locationStepX;
		float ySpace = 3f * (int)WarSceneController.locationStepY;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 3.5f * (int)WarSceneController.locationStepX;
			xBase = -600;
		} else {
			xBase = 600;
			xSpace = -3.5f * (int)WarSceneController.locationStepX;
		}
		
		for (int i=0; i<5; i++) {
			posBase = new Vector3(xBase + off[i].x * xSpace, off[i].y * ySpace, 0);
			
			for (int n=0; n<5; n++) {
				for (int m=0; m<rows[i, n]; m++) {
					Vector3 pos = Vector3.zero;
					if (m % 2 == 0) {
						pos = new Vector3(posBase.x + n * xSpace, posBase.y - ySpace * m / 2, 0);
					} else {
						pos = new Vector3(posBase.x + n * xSpace, posBase.y + ySpace * ((int)(m+1) / 2), 0);
					}
					
					if (soliderNum > 0) {
						soliderNum--;
						SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
						warCtrl.AddSolider(side, sCtrl);
						if (i == 0) 
							warCtrl.AddBackSolider(side, sCtrl);
						else 
							warCtrl.AddFrontSolider(side, sCtrl);
					} else if (knightNum > 0) {
						knightNum--;
						SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
						warCtrl.AddSolider(side, sCtrl);
						if (i == 0)
							warCtrl.AddBackSolider(side, sCtrl);
						else
							warCtrl.AddFrontSolider(side, sCtrl);
					}
					soliderIndex++;
				}
			}
		}
	}
	#endregion

	#region Formation5
	void OnFormation5(int sType, int sNum, int kNum) {
		
		int num = sNum + kNum;
		int [] rowNum = new int[20];
		int[,] rows = new int[20, 3];
		
		while (num > 0) {
			for (int i=0; i<20; i++) {
				rowNum[i]++;
				num--;
				if (num == 0) break;
			}
		}
		
		for (int i=0; i<20; i++) {
			int numCal = rowNum[i];
			int count = 1;
			for (int j=0; j<2; j++) {
				if (numCal > count * count)
					count++;
			}
			if (count == 3) count = 2;
			for (int n=0; n<3; n++) {
				for (int m=0; m<count; m++) {
					rows[i, n]++;
					numCal--;
					if (numCal == 0) break;
				}
				if (numCal == 0) break;
			}
		}
		
		int soliderIndex = 0;
		int knightNum = kNum;
		int soliderNum = sNum;
		Vector3 posBase = Vector3.zero;
		Vector2[] off = new Vector2[20]{
			new Vector2(-3, 0), new Vector2(-3, 4), new Vector2(-3, -4), new Vector2(-3, 8), new Vector2(-3, -8), 
			new Vector2(1, 0), new Vector2(1, 4), new Vector2(1, -4), new Vector2(1, 8), new Vector2(1, -8), 
			new Vector2(5, 0), new Vector2(5, 4), new Vector2(5, -4), new Vector2(5, 8), new Vector2(5, -8), 
			new Vector2(9, 0), new Vector2(9, 4), new Vector2(9, -4), new Vector2(9, 8), new Vector2(9, -8)
		};
		int xBase = -600;
		float xSpace = 3.5f * (int)WarSceneController.locationStepX;
		float ySpace = 2.5f * (int)WarSceneController.locationStepY;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 3.5f * (int)WarSceneController.locationStepX;
			xBase = -600;
		} else {
			xBase = 600;
			xSpace = -3.5f * (int)WarSceneController.locationStepX;
		}
		
		for (int i=0; i<20; i++) {
			posBase = new Vector3(xBase + off[i].x * xSpace, off[i].y * ySpace, 0);
			
			for (int n=0; n<3; n++) {
				for (int m=0; m<rows[i, n]; m++) {
					Vector3 pos = Vector3.zero;
					if (m % 2 == 0) {
						pos = new Vector3(posBase.x + n * xSpace, posBase.y - ySpace * m / 2, 0);
					} else {
						pos = new Vector3(posBase.x + n * xSpace, posBase.y + ySpace * ((int)(m+1) / 2), 0);
					}
					
					if (soliderNum > 0) {
						soliderNum--;
						SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
						warCtrl.AddSolider(side, sCtrl);
						if (i < 4) 
							warCtrl.AddBackSolider(side, sCtrl);
						else 
							warCtrl.AddFrontSolider(side, sCtrl);
					} else if (knightNum > 0) {
						knightNum--;
						SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
						warCtrl.AddSolider(side, sCtrl);
						if (i < 4)
							warCtrl.AddBackSolider(side, sCtrl);
						else
							warCtrl.AddFrontSolider(side, sCtrl);
					}
					soliderIndex++;
				}
			}
		}
	}
	#endregion

	#region Formation6
	void OnFormation6(int sType, int sNum, int kNum) {

		int num = sNum + kNum;
		int frontNum = Mathf.CeilToInt(num * 7 / 10f);
		int backNum = Mathf.FloorToInt(num * 3 / 10f);
		int fRowNum = 0;
		int bRowNum = 0;
		int[] fRows = new int[3];
		int[] bRows = new int[3];

		if (backNum <= 2) {
			bRowNum = 1;
			bRows[0] = backNum;
		} else if (backNum <= 4) {
			bRowNum = 2;
			bRows[0] = 2;
			bRows[1] = backNum - bRows[0];
		} else {
			bRowNum = 3;
			bRows[0] = backNum / 3;
			bRows[1] = backNum / 3;
			bRows[2] = backNum / 3;
			int mod = backNum % 3;
			if (mod == 2) {
				bRows[0]++;
				bRows[1]++;
			} else if (mod == 1) {
				bRows[0]++;
			}
		}

		if (frontNum <= 30) {
			fRowNum = 1;
			fRows[0] = frontNum;
		} else if (frontNum <= 54) {
			fRowNum = 2;
			fRows[0] = 30;
			fRows[1] = frontNum - 30;
		} else {
			fRowNum = 3;
			fRows[0] = 30;
			fRows[1] = 24;
			fRows[2] = frontNum - 54;
		}

		int soliderIndex = 0;
		int knightNum = kNum;
		int soliderNum = sNum;
		Vector3 posBase = Vector3.zero;
		int xSpace = 4 * (int)WarSceneController.locationStepX;
		int ySpace = 4 * (int)WarSceneController.locationStepY;
		if (side == WarSceneController.WhichSide.Left) {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+xSpace, 0, 0);
		} else {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+xSpace, 0, 0);
		}

		for (int i=0; i<bRowNum; i++) {
			for (int j=0; j<bRows[i]; j++) {
				Vector3 pos = Vector3.zero;
				if (j % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y+ySpace*j/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*i, posBase.y-ySpace*((int)(j+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddBackSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}

		if (side == WarSceneController.WhichSide.Left) {
			xSpace = 4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(-600+xSpace, 0, 0);
		} else {
			xSpace = -4 * (int)WarSceneController.locationStepX;
			posBase = new Vector3(600+xSpace, 0, 0);
		}
		if (fRows[0] < 5) {
			for (int i=0; i<fRows[0]; i++) {
				Vector3 pos = Vector3.zero;
				if (i % 2 == 0) {
					pos = new Vector3(posBase.x, posBase.y+ySpace*i/2, 0);
				} else {
					pos = new Vector3(posBase.x, posBase.y-ySpace*((int)(i+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		} else {
			int[] numTemp = new int[3];
			numTemp[0] = fRows[0] / 3;
			numTemp[1] = fRows[0] / 3;
			numTemp[2] = fRows[0] / 3;
			int m = fRows[0] % 3;
			if (m == 1) {
				numTemp[0]++;
			} else if (m == 2) {
				numTemp[1]++;
				numTemp[2]++;
			}

			for (int i=0; i<numTemp[0]; i++) {
				Vector3 pos = Vector3.zero;
				if (i % 2 == 0) {
					pos = new Vector3(posBase.x, posBase.y+ySpace*i/2, 0);
				} else {
					pos = new Vector3(posBase.x, posBase.y-ySpace*((int)(i+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}

			for (int i=0; i<numTemp[1]; i++) {
				Vector3 pos = Vector3.zero;
				pos = new Vector3(posBase.x+(i+1)*xSpace, posBase.y-ySpace*numTemp[0]/2, 0);
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}

			for (int i=0; i<numTemp[2]; i++) {
				Vector3 pos = Vector3.zero;
				pos = new Vector3(posBase.x+(i+1)*xSpace, posBase.y+ySpace*((int)(numTemp[0]-1)/2), 0);
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}

		if (fRowNum >= 2) {
			int temp = fRows[1];
			if (temp > 8) {
				temp = 8;
			}
			for (int i=0; i<temp; i++) {
				Vector3 pos = Vector3.zero;
				if (i % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace, posBase.y+ySpace*i/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace, posBase.y-ySpace*((int)(i+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}

			int[] sideTemp = new int[2];
			sideTemp[0] = (fRows[1]-8) / 2;
			sideTemp[1] = (fRows[1]-8) / 2;
			if ((fRows[1]-8) % 2 == 1) {
				sideTemp[0]++;
			}
			for (int i=0; i<sideTemp[0]; i++) {
				Vector3 pos = Vector3.zero;
				pos = new Vector3(posBase.x+(i+2)*xSpace, posBase.y-ySpace*4, 0);
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
			for (int i=0; i<sideTemp[1]; i++) {
				Vector3 pos = Vector3.zero;
				pos = new Vector3(posBase.x+(i+2)*xSpace, posBase.y+ySpace*3, 0);
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}

		if (fRowNum >= 3) {
			int temp = fRows[2];
			if (temp > 6) {
				temp = 6;
			}
			for (int i=0; i<temp; i++) {
				Vector3 pos = Vector3.zero;
				if (i % 2 == 0) {
					pos = new Vector3(posBase.x+xSpace*2, posBase.y+ySpace*i/2, 0);
				} else {
					pos = new Vector3(posBase.x+xSpace*2, posBase.y-ySpace*((int)(i+1)/2), 0);
				}
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
			
			int[] sideTemp = new int[2];
			sideTemp[0] = (fRows[2]-6) / 2;
			sideTemp[1] = (fRows[2]-6) / 2;
			if ((fRows[2]-6) % 2 == 1) {
				sideTemp[0]++;
			}
			for (int i=0; i<sideTemp[0]; i++) {
				Vector3 pos = Vector3.zero;
				pos = new Vector3(posBase.x+(i+3)*xSpace, posBase.y-ySpace*3, 0);
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
			for (int i=0; i<sideTemp[1]; i++) {
				Vector3 pos = Vector3.zero;
				pos = new Vector3(posBase.x+(i+3)*xSpace, posBase.y+ySpace*2, 0);
				
				if (soliderNum > 0) {
					soliderNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, false);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				} else if (knightNum > 0) {
					knightNum--;
					SolidersController sCtrl = SetSolider(sType, pos, soliderIndex, true);
					warCtrl.AddSolider(side, sCtrl);
					warCtrl.AddFrontSolider(side, sCtrl);
				}
				soliderIndex++;
			}
		}
	}
	#endregion
}
