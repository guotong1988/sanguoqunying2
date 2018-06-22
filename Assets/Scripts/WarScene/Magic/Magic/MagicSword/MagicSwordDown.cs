using UnityEngine;
using System.Collections;

public class MagicSwordDown : MonoBehaviour {

	private float speed = 500;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.localPosition;
		pos.z += speed * Time.deltaTime;
		transform.localPosition = pos;
		if (pos.z >= 0) {
			Destroy(gameObject);
		}
	}
}
