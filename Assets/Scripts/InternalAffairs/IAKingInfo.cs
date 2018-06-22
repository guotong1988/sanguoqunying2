using UnityEngine;
using System.Collections;

public class IAKingInfo : MonoBehaviour {
	
	public exSpriteFont historyTime;
	public GameObject[] kingsHead;
	public exSpriteFont kingName;
	public exSpriteFont cityNum;
	public exSpriteFont money;
	public exSpriteFont population;
	public exSpriteFont generalNum;
	public exSpriteFont soldierNum;
	
	public GameObject[] background;
	
	private Vector3 headPos = new Vector3(97.5f, 70, 0);
	
	// Use this for initialization
	void Start () {
		GameObject go = (GameObject)Instantiate(kingsHead[Controller.kingIndex]);
		go.transform.position = new Vector3(transform.position.x + headPos.x, transform.position.y + headPos.y, transform.position.z + headPos.z);
		go.transform.parent = transform;
		
		historyTime.text = Controller.historyTime + ZhongWen.Instance.nian;
		kingName.text = ZhongWen.Instance.GetKingName(Controller.kingIndex);
		
		GetCityInfo();
	}
	
	void OnEnable() {
		GetGeneralInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void GetCityInfo() {
		
		long m = 0;
		long p = 0;
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		
		for (int i=0; i<kInfo.cities.Count; i++) {
			CityInfo cInfo = Informations.Instance.GetCityInfo((int)kInfo.cities[i]);
			
			m += cInfo.money;
			p += cInfo.population;
		}
		
		cityNum.text = "" + kInfo.cities.Count;
		money.text = "" + m;
		population.text = p + ZhongWen.Instance.ren;
		
		if (kInfo.cities.Count < 16) {
			Instantiate(background[0]);
		} else if (kInfo.cities.Count < 32) {
			Instantiate(background[1]);
		} else {
			Instantiate(background[2]);
		}
	}
	
	void GetGeneralInfo() {
		
		int s = 0;
		
		KingInfo kInfo = Informations.Instance.GetKingInfo(Controller.kingIndex);
		
		for (int i=0; i<kInfo.generals.Count; i++) {
			s += Informations.Instance.GetGeneralInfo((int)kInfo.generals[i]).soldierCur;
			s += Informations.Instance.GetGeneralInfo((int)kInfo.generals[i]).knightCur;
		}
		
		generalNum.text = kInfo.generals.Count + ZhongWen.Instance.ren;
		soldierNum.text = s + ZhongWen.Instance.ren;
	}
}
