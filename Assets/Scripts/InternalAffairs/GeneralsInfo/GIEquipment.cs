using UnityEngine;
using System.Collections;

public class GIEquipment : MonoBehaviour {

	public EquipmentSelect equipment;
	
	public void SetGeneral(int idx) {
		equipment.SetEquipment(Informations.Instance.GetGeneralInfo(idx).equipment);
	}
}
