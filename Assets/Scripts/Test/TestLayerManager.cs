using UnityEngine;
using System.Collections;

public class TestLayerManager : MonoBehaviour {

	public GameObject[] go;

	// Use this for initialization
	void Start () {
		for (int i=0; i<go.Length; i++) {
			GetComponent<exLayerMng>().InsertAt(i, go[i].GetComponent<exLayer>());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
