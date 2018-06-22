using UnityEngine;
using System.Collections;

public class GeneralsHeadSelect : MonoBehaviour {
	
	private GameObject go;
	private int idxCur = -1;
	private int headMax = 255;
	
	public void SetGeneralHead(int idx) {
		if (idx < 0 || idx >= headMax) {
			
			if (go != null) {
				Destroy(go);
				Resources.UnloadUnusedAssets();
			}
			return;
		}
		
		if (idx == idxCur)	return;
		
		if (go != null) {
			Destroy(go);
			Resources.UnloadUnusedAssets();
		}
		
		string headName = "Head/Head";
		if (idx < 9) {
			headName += "00" + (idx+1);
		} else if (idx < 99) {
			headName += "0" + (idx+1);
		} else {
			headName += "" + (idx+1);
		}
		
		go = (GameObject)Instantiate(Resources.Load(headName), transform.position, transform.rotation);
		go.transform.parent = transform;
	}
}
