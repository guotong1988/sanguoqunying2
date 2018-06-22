using UnityEngine;
using System.Collections;

public class TestAStar : MonoBehaviour {
	
	public Transform target;
	
	private MyPathfinding pathfinding;
	private ArmyController armyMoving;
	
	// Use this for initialization
	void Start () {
		
		pathfinding = GameObject.FindWithTag("Pathfinding").GetComponent<MyPathfinding>();
		armyMoving = GetComponent<ArmyController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown(0)) {
			
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;
			
			target.position = mousePos;
			
			armyMoving.SetRoute(pathfinding.GetRoute(transform.position, target.position));
		}
	}
	
	
}
