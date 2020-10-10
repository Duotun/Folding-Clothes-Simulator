using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class easygrab : MonoBehaviour {



    public static SteamVR_TrackedObject trackedObj;

    private GameObject collidingObject;
    private GameObject objectInHand;

    public static Vector3 pinposition;
    public static bool pressflag = false;
    public static bool relieveflag = false;
    public static GameObject trackgameobject;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        Debug.Log(trackedObj);
        pinposition = trackedObj.transform.TransformPoint(GetComponent<BoxCollider>().center);
    }
    // Use this for initialization
    void Start () {
		
	}
    // Update is called once per frame
    void Update () {
        pinposition = trackedObj.transform.TransformPoint(GetComponent<BoxCollider>().center);

        if (Controller.GetHairTriggerDown())
        {
            pressflag = true;
        }

        if (Controller.GetHairTriggerUp())
        {
            relieveflag = true;
        }
    }
}
