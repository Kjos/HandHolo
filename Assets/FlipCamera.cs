using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCamera : MonoBehaviour {
	private Camera camera;
	private Matrix4x4 flipMatrix;
	private Vector3 flipVector = new Vector3 (-1, 1, 1);
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
		flipMatrix = Matrix4x4.Scale (flipVector);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnPreCull () {
		camera.ResetWorldToCameraMatrix();
		camera.ResetProjectionMatrix();
		camera.projectionMatrix = camera.projectionMatrix * flipMatrix;
	}

	void OnPreRender () {
		GL.invertCulling = true;
	}

	void OnPostRender () {
		GL.invertCulling = false;
	}
}
