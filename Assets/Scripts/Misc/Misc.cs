using UnityEngine;
using System.Collections;

public class Misc : MonoBehaviour {

	public static GameObject backButton;
	public static bool isBack = false;
	public static bool isNeedBack = false;
	
	public static bool GetBack() {

		if (!backButton.activeSelf)
			backButton.SetActive(true);

		if (Input.GetMouseButtonDown(1) || Input.touchCount > 1 || Input.GetKeyDown(KeyCode.Escape) || isBack) {
			isBack = true;
			return true;
		}

		isNeedBack = true;

		return false;
	}
	
	public static int GetLevelExperience(int lv) {
		if (lv == 2) {
			return 200;
		} else if (lv == 3) {
			return 377;
		} else if (lv == 4) {
			return 771;
		} else if (lv < 50) {
			lv -= 5;
			return 1302 + 531 * lv + 137 * (lv + 1) * lv / 2;
		} else {
			return 99999;
		}
	}
	
	public static void LoadLevel(string sceneName) {
		
		GameObject go = GameObject.FindWithTag("FadeOut");
		
		if (go == null) {
			Debug.LogError("FadeOut cannot found!");
			return;
		}

		go.SetActive(true);
		go.GetComponent<FadeOut>().SetLevelName(sceneName);
	}
	
	public static int GetArmsIdx(int arms) {
		
		int idx = -1;
		while (arms > 0) {
			arms >>= 1;
			idx++;
		}
		
		return idx;
	}
	
	public static bool CheckIsLevelUp(GeneralInfo gInfo) {
		
		bool flag = false;
		while (gInfo.experience >= Misc.GetLevelExperience(gInfo.level+1) && gInfo.level < 50) {
			gInfo.level++;
			if (gInfo.level <= 20) {
				gInfo.soldierMax = 5 * gInfo.level;
				if (gInfo.soldierCur > gInfo.soldierMax)
					gInfo.soldierCur = gInfo.soldierMax;
			} else if (gInfo.level <= 40) {
				gInfo.soldierMax = 100 - (gInfo.level - 20) * 5;
				gInfo.knightMax = (gInfo.level - 20) * 5;

				if (gInfo.soldierCur > gInfo.soldierMax) {
					gInfo.knightCur += gInfo.soldierMax - gInfo.soldierCur;
					gInfo.soldierCur = gInfo.soldierMax;
				}
				gInfo.knightCur = Mathf.Clamp(gInfo.knightCur, 0, gInfo.knightMax);
			} else if (gInfo.level > 40){
				gInfo.soldierMax = 0;
				gInfo.knightMax = 100;
				gInfo.knightCur += gInfo.soldierCur;
				gInfo.knightCur = Mathf.Clamp(gInfo.knightCur, 0, gInfo.knightMax);
				gInfo.soldierCur = 0;
			}

			gInfo.manaMax += Random.Range(1, 3);
			gInfo.healthMax += Random.Range(1, 3);
			gInfo.manaMax = Mathf.Clamp(gInfo.manaMax, 0, 999);
			gInfo.healthMax = Mathf.Clamp(gInfo.healthMax, 0, 999);
			
			if (gInfo.level == 50) gInfo.experience = 99999;
			
			flag = true;
		}
		
		return flag;
	}
}
