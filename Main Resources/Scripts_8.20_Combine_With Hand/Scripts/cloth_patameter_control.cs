using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEditor;

public class cloth_patameter_control : MonoBehaviour {


    ObiCloth cloth;
	// Use this for initialization
	void Start () {
        cloth = GetComponent<ObiCloth>();
        particle_control();
	}
	
    void particle_control()
    {
        Debug.Log(cloth.GetParticlePosition(0));
        
    }
	// Update is called once per frame
	void Update () {
		
	}
}
