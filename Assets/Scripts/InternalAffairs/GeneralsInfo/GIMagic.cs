using UnityEngine;
using System.Collections;

public class GIMagic : MonoBehaviour {
	
	public exSpriteFont[] magic;

	
	public void SetGeneral(int idx) {
		GeneralInfo gInfo = Informations.Instance.GetGeneralInfo(idx);
		
		for (int i=0; i<gInfo.magic.Length; i++) {
			int mIdx = gInfo.magic[i];
			if (mIdx == -1) {
				magic[i].gameObject.SetActive(false);
				continue;
			}
			
			string mName = ZhongWen.Instance.GetMagicName(mIdx);
			
			string text = mName[0].ToString();
			
			for (int j=1; j<mName.Length; j++) {
				text += "\n" + mName[j];
			}
			
			magic[i].text = text;
		}
	}
}
