using UnityEngine;
using System.Collections;

public class EquipmentSelect : MonoBehaviour {
	
	public GameObject[] equipmentPrefabs;
	
	private GameObject go;
	
	public void SetEquipment(int idx) {
		if (go != null) {
			Destroy(go);
		}
		
		if (idx < 0 || idx >= equipmentPrefabs.Length) {
			return;
		}
		
		go = (GameObject)Instantiate(equipmentPrefabs[idx], transform.position, transform.rotation);
		go.transform.parent = transform;
	}
}
