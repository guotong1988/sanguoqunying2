using UnityEngine;
using System.Collections;

public class FlagsController : MonoBehaviour {

	public GameObject cityNamePerfab;
	public exSpriteAnimation[] flags;

	void Start () {
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			SetFlag(i);
			
			Vector3 pos = flags[i].transform.position;
			pos.z = (pos.y - 400f) / 800f;
			flags[i].transform.position = pos;

			GameObject go = (GameObject)Instantiate(cityNamePerfab);
			go.transform.parent = flags[i].transform;
			go.transform.localPosition = new Vector3(8, -8, 0);
			go.GetComponent<exSpriteFont>().text = ZhongWen.Instance.GetCityName(i);
		}
	}
	
	public void SetFlag(int cityIdx) {
		
		int kingIdx = Informations.Instance.GetCityInfo(cityIdx).king;
		if (kingIdx == -1) {
			flags[cityIdx].GetComponent<Renderer>().enabled = false;
			return;
		}

        string kingName = ZhongWen.Instance.GetKingName(kingIdx);
        for (int i = 0; i < ZhongWen.Instance.kingNames.Length; i++ )
        {
            if (ZhongWen.Instance.kingNames[i] == kingName)
            {
                string animName = "Flag" + (i + 1);
                flags[cityIdx].Play(animName);
                flags[cityIdx].GetComponent<Renderer>().enabled = true;
                break;
            }
        }
	}
	
	public int GetTouchCityIdx() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			if (flags[i].GetComponent<Collider>().Raycast (ray, out hit, 1000.0f)) {
				return i;
			}
		}
		
		return -1;
	}
	
	public void SetFlagsAnimPause() {
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			if (flags[i].GetComponent<Renderer>().enabled) {
				flags[i].Pause();
			}
		}
	}
	
	public void SetFlagsAnimResume() {
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			if (flags[i].GetComponent<Renderer>().enabled) {
				flags[i].Resume();
			}
		}
	}
}
