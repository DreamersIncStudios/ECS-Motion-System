using System;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
	
	[SerializeField]float[] distances = new float[32];
	void Start() {

		Camera.main.layerCullDistances = distances;
		Camera.main.layerCullSpherical = true;
	}
}
