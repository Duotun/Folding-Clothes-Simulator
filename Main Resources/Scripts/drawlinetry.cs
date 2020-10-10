using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawlinetry : MonoBehaviour {
    private LineRenderer linerenderer;
    private float counter=0f;
    private float dist;

    public Transform origin;
    public Transform destination;
    public float linedraspeed = 1f;
    int drawflag = 0;
    float x=0;
    //find sphere by name ? static public
    void Start()
    {
        linerenderer = GetComponent<LineRenderer>();
        linerenderer.SetPosition(0, origin.position);
        linerenderer.startWidth = 0.005f; linerenderer.endWidth = 0.005f;
        dist = Vector3.Distance(origin.position, destination.position);
        //linerenderer.SetPosition(1, destination.position);
       
    }
    // Use this for initialization

	
	// Update is called once per frame
	void Update () {


        if (x<dist)
        {
            counter += .5f / linedraspeed;
            x = Mathf.Lerp(0, dist, counter);   //return t   0- .5f
            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;
            Vector3 pointAline = x * Vector3.Normalize(pointB - pointA) + pointA;
            linerenderer.SetPosition(1, pointAline);
        }
    }
}
