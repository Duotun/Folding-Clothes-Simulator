using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour {

    public float speed = 6.0f;
    private CharacterController _charCont;


    private void Start()
    {
        _charCont = GetComponent<CharacterController>();
    }
    void Update()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Translate(0, 0, x);
        transform.Translate(-z, 0, 0);
       
    }
   
}
