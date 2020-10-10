using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
public class server_part : MonoBehaviour {

	// Use this for initialization
    public static int[] numberint = new int[8];
    public static string inputstring;
    void Start () {
        InputField input = gameObject.GetComponent<InputField>();
        if (input != null)
        {
            var se = new InputField.SubmitEvent();
            se.AddListener(getvalue);
            input.onEndEdit = se;
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public String getinput()
    {
        return inputstring;
    }
    public void submit()
    {
        InputField input = GameObject.FindWithTag("input").GetComponent<InputField>();
        //Debug.Log("Submit Successfully!");
        inputstring = input.text;
        input.text = null;
        //Debug.Log(inputstring);

    }
    private void getvalue(string arg0)
    {

        inputstring = arg0;
        InputField input = GameObject.FindWithTag("input").GetComponent<InputField>();
        input.text = null;
        // string[] fournumber = inputstring.Split(' ');
        //for(int i=0;i<fournumber.Length;i++)
        // {
        //     numberint[i] = Convert.ToInt32(fournumber[i]);
        //    Debug.Log(numberint[i]);
        // }
        //int.parse();

    }
}
