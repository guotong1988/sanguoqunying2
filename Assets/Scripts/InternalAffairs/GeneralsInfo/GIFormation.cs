using UnityEngine;
using System.Collections;

public class GIFormation : MonoBehaviour {
	
	public Transform token;
	public exSpriteFont[] formations;
	
	public void SetGeneral(int idx) {
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(idx);
		
		for (int i=0; i<Informations.Instance.formationNum; i++) {
			if ((gInfo.formation & (1 << i)) == 0) {
				formations[i].topColor = new Color(0.5f, 0.5f, 0.5f, 1);
				formations[i].botColor = new Color(0.5f, 0.5f, 0.5f, 1);
			} else {
				formations[i].topColor = new Color(1, 1, 1, 1);
				formations[i].botColor = new Color(1, 1, 1, 1);
				
				if (gInfo.formationCur == (1 << i)) {
					token.position = new Vector3(token.position.x, formations[i].transform.position.y, token.position.z);
				}
			}
		}
	}
}
