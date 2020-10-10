using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class rotatearound : MonoBehaviour {

    // Use this for initialization
    GameObject cube;
	void Start () {
       cube= GameObject.Find("Cube");
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(cube.transform.position, Vector3.up, 3.0f);
	}
}
