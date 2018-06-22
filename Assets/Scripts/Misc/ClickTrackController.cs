using UnityEngine;
using System.Collections;

public class ClickTrackController : MonoBehaviour {
	
	public GameObject pariticlePrefab;
	
	private GameObject particle;
	private ParticleSystem fire;
	private ParticleSystem smoke;
	
	// Use this for initialization
	void Start () {

		DontDestroyOnLoad(this.gameObject);

		particle = (GameObject)Instantiate(pariticlePrefab);
		particle.transform.parent = this.transform;
		fire = particle.transform.Find("Fire").GetComponent<ParticleSystem>();
		smoke = particle.transform.Find("Smoke").GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton(0)) {

			particle.transform.position = CalcPosition();

			if (!fire.isPlaying) {
				fire.Simulate(0);
				smoke.Simulate(0);
				
				fire.Play();
				smoke.Play();
			}
		} else {

			if (fire.isPlaying) {
				fire.Stop();
				smoke.Stop();
			}
		}
	}
	
	Vector3 CalcPosition() {
		Camera cam = Camera.main;
		if (cam == null) {
			cam = UICamera.mainCamera;
		}
		Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = -11;
		return mousePosition;
	}
}
