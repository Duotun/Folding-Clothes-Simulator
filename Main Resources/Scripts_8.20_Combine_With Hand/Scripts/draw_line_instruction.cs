
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEditor;
public class draw_line_instruction : MonoBehaviour {

    private LineRenderer linerenderer;
    public instruction nowuse;  //update to find object using tag
    private float counter;
    private float dist;
    private GameObject clothused;
    public Transform origin;
    public Transform destination;
    private GameObject originobject;
    private GameObject destinationobject;
    public float linedraspeed = 10f;
    float x = 0;
    public static int flag = 0;
    public static int reset = 1;
    public static int cnt = 0; // for delete drawing sphere
    int findflag = 0;
    //find sphere by name ? static public
    void Start()
    {
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
            flag = 2; //successfully draw
           
        }
    }
    void find()
    {
        clothused = GameObject.FindWithTag("cloth");
        ObiCloth clothes = clothused.GetComponent<ObiCloth>();
        nowuse = clothused.GetComponent<instruction>();
        if (nowuse != null && clothes != null)
        {
            findflag = 1;
        }
    }
    // Update is called once per frame
    void Update () {
        if (findflag == 0)
        {
            find();  //fornowuse
        }
        else
        {
            if(instruction.dflag==1&&flag==0&& instruction.tmptwoobjectfordraw[0]!=null&& instruction.tmptwoobjectfordraw[1]!=null)
            {
                origin = instruction.tmptwoobjectfordraw[0].transform;
                destination = instruction.tmptwoobjectfordraw[1].transform;
                originobject = instruction.tmptwoobjectfordraw[0];
                destinationobject = instruction.tmptwoobjectfordraw[1];
                set_parameters();
                flag = 1;  
            }
            else if(instruction.dflag==1&&flag==1)
            {
                draw_now();              
            }
            else if (instruction.flag == 1 && cnt == 0)
            {
                linerenderer.SetPosition(1, origin.position);
                originobject.SetActive(false);
                destinationobject.SetActive(false);
                cnt = 1;
            }

        }
    }
}
