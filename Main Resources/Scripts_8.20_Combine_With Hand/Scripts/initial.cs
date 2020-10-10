using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Obi;
public class initial : MonoBehaviour {
    
    // Use this for initialization
    int addclothflag = 0;
    int addpickerflag = 0;
	void Start () {
        EditorWindow currentscreen= EditorWindow.GetWindow<EditorWindow>();
        //Debug.Log(currentscreen.position.size);
        //currentscreen.position = new Rect(currentscreen.position.center, currentscreen.position.size);
        //Screen.SetResolution(1000, 600, true);
        //Debug.Log(Screen.width);
        //Debug.Log(Screen.height);
        this.gameObject.tag = "cloth";
        ObiCloth cloth = this.gameObject.GetComponent<ObiCloth>();
        grabcontrol picker = this.gameObject.GetComponent<grabcontrol>();
        if(cloth!=null)
        {
            addclothflag = 1;
           if(picker!=null)
            {
                addpickerflag = 1;
            }
        }
        addscript();
       
	}

    void addtagscript()
    {
        if (addpickerflag == 0)
        {
            ObiCloth cloth = this.gameObject.GetComponent<ObiCloth>();
             if(cloth != null &&cloth.Initialized == true)
            {
                
                this.gameObject.AddComponent<particletagdrag>();
                addpickerflag = 1;
            }
        }
    }
    void addscript()   //
    {
        if(addclothflag==0)
        {
            this.gameObject.AddComponent<autotwoface>();
            this.gameObject.AddComponent<autocloth>();
            addclothflag = 1;
         
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(2f);
    }
    // Update is called once per frame
    void Update()
    {

        if (addpickerflag == 0)
        {
            StartCoroutine(wait());
            addtagscript();
        }
        if (Input.GetKeyDown(KeyCode.Escape))  //exit
        {
            //Debug.Log("fuck?");
            EditorApplication.isPlaying = false;
        }
    }
}
