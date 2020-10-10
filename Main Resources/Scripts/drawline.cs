using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawline : MonoBehaviour {

    // Use this for initialization
    private LineRenderer linerenderer;
    public Obi.particletag nowuse;
    private float counter;
    private float dist;

    public Transform origin;
    public Transform destination;
    private GameObject originobject;
    private GameObject destinationobject;
    public float linedraspeed = 6f;
    float x = 0;
    public static int flag = 0;
    public static int reset = 1;
    public static int cnt = 0; // for delete drawing sphere
    //find sphere by name ? static public
	void Start () {
        linerenderer = GetComponent<LineRenderer>();
        
	}
	void set_parameters()
    {
        linerenderer.SetPosition(0, origin.position);
        linerenderer.SetPosition(1, origin.position);
        x = 0; counter = 0f;
        linerenderer.startWidth = 0.005f; linerenderer.endWidth = 0.005f;
        dist = Vector3.Distance(origin.position, destination.position);
    }
    void draw_now()
    {
        if (x < dist)
        {
            counter += .2f / linedraspeed;
            x = Mathf.Lerp(0, dist, counter);   //return t
            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;
            Vector3 pointAline = x * Vector3.Normalize(pointB - pointA) + pointA;
            linerenderer.SetPosition(1, pointAline);
        }
        else
        {
            if(nowuse.flag==1&&cnt==0)  // continuing
            {
                linerenderer.SetPosition(1, origin.position);
                Destroy(originobject);
                Destroy(destinationobject);
                //originobject.SetActive(false);
                //destinationobject.SetActive(false);
                //Debug.Log("kuku1");
                cnt = 1;
                //flag = 0;
            }
           
        }
    }
    private void reset_line()  // method multiple?
    {
        if (originobject != null&&destinationobject!=null)
        {
            linerenderer.SetPosition(0, Vector3.zero);
            linerenderer.SetPosition(1, Vector3.zero);
            Destroy(originobject); Destroy(destinationobject);
            reset = 0;
            
        }
        else    //multi
        {
            
            reset = 0;
            flag = 0;
        }
    }
    // Update is called once per frame
    void Update () {

        if(Obi.particletag.resetflag==1&&reset==1)
        {
            reset_line();
            
        }
        if (flag == 0)
        {
            if (nowuse.button_choose == 1|| nowuse.twoobject[1]!=null)  // click on fold
            {

                origin = nowuse.twoobject[0].transform;
                destination = nowuse.twoobject[1].transform;
                originobject = nowuse.twoobject[0];
                destinationobject= nowuse.twoobject[1];
                set_parameters();
              
                if (nowuse.button_choose == 1)
                {
                    flag = 1;  
                }
            }
        }
        else
        {
            if (nowuse.lineflag == 0)
            {
               // Debug.Log("kuku");
                draw_now();
            }
            else
            {
                if (originobject != null && destinationobject != null)
                {
                    linerenderer.SetPosition(0, origin.position);
                    linerenderer.SetPosition(1, origin.position);
                }
            }
        }
	}
}
