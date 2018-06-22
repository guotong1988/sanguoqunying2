using UnityEngine;
using System.Collections;

public class WSMapFrame : MonoBehaviour {
	
	public WarSceneController wsCtrl;
	public Transform frame;
	
	private bool isMouseDown;
	
	private Vector2 size = new Vector2(113, 28);
	private float frameMaxX = 48f;
	private float frameMaxY = 7f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		frame.transform.localPosition = new Vector3(WarSceneController.cameraPosCur.x / WarSceneController.cameraPosMaxX * frameMaxX,
			WarSceneController.cameraPosCur.y / WarSceneController.cameraPosMaxY * frameMaxY, frame.transform.localPosition.z);
		
		if (WarSceneController.state != WarSceneController.State.Running) {
			return;
		}
		
		if (Input.GetMouseButtonDown(0)) {
			
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (mousePos.x >= transform.position.x - size.x / 2 && mousePos.x <= transform.position.x + size.x / 2
				&& mousePos.y >= transform.position.y - size.y / 2 && mousePos.y <= transform.position.y + size.y / 2) {
				
				isMouseDown = true;
				
				float offX = mousePos.x - transform.position.x;
				offX = Mathf.Clamp(offX, -frameMaxX, frameMaxX);
				float offY = mousePos.y - transform.position.y;
				offY = Mathf.Clamp(offY, -frameMaxY, frameMaxY);
				
				Vector3 pos = new Vector3(offX / frameMaxX * WarSceneController.cameraPosMaxX,
					offY / frameMaxY * WarSceneController.cameraPosMaxY, 0);
				wsCtrl.SetCameraPosition(pos);
			} else {
				isMouseDown = false;
			}
		} else if (isMouseDown && Input.GetMouseButton(0)) {
			
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (mousePos.x >= transform.position.x - size.x / 2 && mousePos.x <= transform.position.x + size.x / 2
				&& mousePos.y >= transform.position.y - size.y / 2 && mousePos.y <= transform.position.y + size.y / 2) {
				
				float offX = mousePos.x - transform.position.x;
				offX = Mathf.Clamp(offX, -frameMaxX, frameMaxX);
				float offY = mousePos.y - transform.position.y;
				offY = Mathf.Clamp(offY, -frameMaxY, frameMaxY);
				
				Vector3 pos = new Vector3(offX / frameMaxX * WarSceneController.cameraPosMaxX,
					offY / frameMaxY * WarSceneController.cameraPosMaxY, 0);
				wsCtrl.SetCameraPosition(pos);
			}
		}
	}
}
