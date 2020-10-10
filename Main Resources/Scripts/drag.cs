using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag : MonoBehaviour {

    Vector3 dist;
    float posX;
    float posY;
    float distance = 10;
  /*private void OnMouseDown()
    {
        dist = Camera.main.WorldToScreenPoint(transform.position);
        Debug.Log(dist);
        Debug.Log("sss");
        posX = Input.mousePosition.x - dist.x;
        posY = Input.mousePosition.y - dist.y;

    
    }
    */
    private void OnMouseDrag()
    {
        Vector3 curPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);
        transform.position = worldPos;
    }
}
