using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class EnableCameraDepthBuffer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var camera = GetComponent<Camera>();
		camera.depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
