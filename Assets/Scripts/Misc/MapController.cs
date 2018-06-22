using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour {
	
	private Color gray = new Color(0.5f, 0, 0, 1);
    private exSprite[] cities;

    void Awake()
    {
        InitCities();
    }

	void InitCities() {
		if (cities == null) {
			cities = new exSprite[Informations.Instance.cityNum];
			
			for (int i=0; i<Informations.Instance.cityNum; i++) {
				string cityName = "City" + (i+1);
				cities[i] = transform.Find(cityName).GetComponent<exSprite>();
			}
		}
	}
	
	public void ClearSelect() {
		InitCities();
		
		for (int i=0; i<Informations.Instance.cityNum; i++) {
			cities[i].color = gray;
		}
	}
	
	public void SelectCity(int idx) {
		InitCities();
		
		if (idx < 0 || idx >= Informations.Instance.cityNum) return;
		
		cities[idx].color = new Color(1, 1, 1, 1);
	}
}
