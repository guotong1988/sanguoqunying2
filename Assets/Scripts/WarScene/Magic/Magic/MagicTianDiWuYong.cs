using UnityEngine;
using System.Collections;

public class MagicTianDiWuYong : MonoBehaviour {

	private string[] group = new string[]{"2617", "2628", "2626"};

	// Use this for initialization
	void Start () {
	
		Init();
		Invoke("WaitForCameraMoveToGeneral", 0.3f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Init() {
		
		transform.parent = MagicController.Instance.magicRoot;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	void WaitForCameraMoveToGeneral() {
		StartCoroutine(SetMagic());
	}

	IEnumerator SetMagic() {

		GameObject.Instantiate(Resources.Load("Magic/" + group[0]));
		yield return new WaitForSeconds(1.5f);

		GameObject.Instantiate(Resources.Load("Magic/" + group[1]));
		yield return new WaitForSeconds(1f);

		GameObject.Instantiate(Resources.Load("Magic/" + group[2]));
	}
}
