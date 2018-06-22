using UnityEngine;
using System.Collections;

public class GIArms : MonoBehaviour {
	
	public Transform token;
	public exSpriteFont[] arms;
	
	public void SetGeneral(int idx) {
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(idx);
		
		for (int i=0; i<Informations.Instance.armsNum; i++) {
			if ((gInfo.arms & (1 << i)) == 0) {
				arms[i].topColor = new Color(0.5f, 0.5f, 0.5f, 1);
				arms[i].botColor = new Color(0.5f, 0.5f, 0.5f, 1);
			} else {
				arms[i].topColor = new Color(1, 1, 1, 1);
				arms[i].botColor = new Color(1, 1, 1, 1);
				
				if (gInfo.armsCur == (1 << i)) {
					token.position = new Vector3(arms[i].transform.position.x, token.position.y, token.position.z);
				}
			}
		}
	}
}
