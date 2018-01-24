using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour {
	public GameObject selectionQuad;
	public GameObject cursor;
	private CameraScript cameraScript;

	private Renderer renderer;
	// Use this for initialization
	void Start () {
		cameraScript = GetComponent<CameraScript> ();
		renderer = selectionQuad.GetComponent<Renderer> ();
	}

	private bool firstDown = true;
	private Vector3 startPosition;
	private Vector3 size = new Vector3(1,1,1);
	
	// Update is called once per frame
	void Update () {
		Vector3 currentPosition = cursor.transform.position;

		if (cameraScript.GetMouseDown(0)) {
			if (firstDown) {
				renderer.enabled = true;
				startPosition = currentPosition;
				firstDown = false;
			}
			size.x = startPosition.x - currentPosition.x;
			size.y = startPosition.z - currentPosition.z;
			Vector3 pos = startPosition;
			pos.x -= size.x / 2.0f;
			pos.z -= size.y / 2.0f;
			selectionQuad.transform.position = pos;
			selectionQuad.transform.localScale = size;
		} else {
			firstDown = true;
			renderer.enabled = false;
		}
	}
}
