using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using VRTK;
public class drawlineVR : MonoBehaviour {
    private LineRenderer linerenderer;
    public particletagVR nowuse;  //update to find object using tag
    private float counter = 0f;
    private float dist;
    private GameObject clothused;
    public Transform origin;
    public Transform destination;
    public float linedraspeed = 116f;
    public static int drawflag = 0, findflag = 0;  //drawflag initialize folding
    float x = 0;
    private GameObject originobject;
    private GameObject destinationobject;
    public static int finishflag = 0;
    //find sphere by name ? static public
    void Start()
    {
        linerenderer = GetComponent<LineRenderer>();
        //linerenderer.SetPosition(1, destination.position);

    }
    // Use this for initialization
    void set_parameters()
    {
        linerenderer.SetPosition(0, origin.position);
        linerenderer.SetPosition(1, origin.position);
        x = 0; counter = 0f;
        linerenderer.startWidth = 0.005f; linerenderer.endWidth = 0.005f;
        dist = Vector3.Distance(origin.position, destination.position);
        
    }

    void find()
    {
        clothused = GameObject.FindWithTag("cloth");
        ObiCloth clothes = clothused.GetComponent<ObiCloth>();
        nowuse = clothused.GetComponent<particletagVR>();
        if (nowuse != null && clothes != null)
        {
            findflag = 1;
            
        }
    }
    void draw_line()
    {
        if (x < dist)
        {
            counter += .009f / linedraspeed;
            x = Mathf.Lerp(0, dist, counter);   //return t
            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;
            Vector3 pointAline = x * Vector3.Normalize(pointB - pointA) + pointA;
            linerenderer.SetPosition(1, pointAline);

        }
        else
        {
             if(finishflag==0)
            {
                particletagVR.lineflag = 1;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (findflag == 0)
        {
            find();
        }
        else
        {
            if (nowuse.twoobject[1] != null && nowuse.twoobject[0] != null && drawflag == 0 && finishflag == 0)  //only before grabbing
            {
                origin = nowuse.twoobject[0].transform;
                destination = nowuse.twoobject[1].transform;
                originobject = nowuse.twoobject[0];
                destinationobject = nowuse.twoobject[1];
                set_parameters();
            }

            if (VRTK_ControllerEvents.drawflag==true&& nowuse.twoobject[1] != null && nowuse.twoobject[0] != null)
            {
                drawflag = 1;
                particletagVR.button_choose = 1;
            }

            if(drawflag==1&&finishflag==0)
            {
                
                draw_line();
            }
            else
            {
                if (finishflag > 0)
                {
                    Debug.Log("newturn");
                    drawflag = 0; finishflag = 0; findflag = 0;
                    linerenderer.SetPosition(1, origin.position);
                    Destroy(originobject);
                    Destroy(destinationobject);
                }
            }
        }
    }
}
