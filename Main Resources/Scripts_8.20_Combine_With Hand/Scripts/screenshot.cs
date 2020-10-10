using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
public class screenshot : MonoBehaviour {

    public static int resWidth = 640;
    public static int resHeight = 480;

    public static byte[] bytes;
    public static string filename;
    public static bool takeHiResShot = false;    //for the instruction script auto.
    new Camera camera = new Camera();

    private void Start()
    {
         camera = GameObject.Find("Shot_Camera").GetComponent<Camera>();

         //resWidth = Screen.width/2;
         //resHeight = Screen.height;
        //Debug.Log(camera);
    }
    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void LateUpdate()     //presskey or auto
    {
         //takeHiResShot |= Input.GetKeyDown(KeyCode.LeftShift);
         if (takeHiResShot)    //for server
         {
             RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
             camera.targetTexture = rt;
             Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
             camera.Render();
             RenderTexture.active = rt;
             screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
             camera.targetTexture = null;
             RenderTexture.active = null; // JC: added to avoid errors
             Destroy(rt);
             bytes = screenShot.EncodeToPNG();
             //Debug.Log(bytes.Length);
             filename = ScreenShotName(resWidth, resHeight);
             System.IO.File.WriteAllBytes(filename, bytes);
             Debug.Log("Take the picture.");
             takeHiResShot = false;
         }

        /*
         if (takeHiResShot)         // press
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            bytes = screenShot.EncodeToPNG();
            filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
        */
        /*
       takeHiResShot |= Obi.particletag.takepictureflag;
       if (takeHiResShot)
       {
           RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
           camera.targetTexture = rt;
           Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
           camera.Render();
           RenderTexture.active = rt;
           screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
           camera.targetTexture = null;
           RenderTexture.active = null; // JC: added to avoid errors
           Destroy(rt);
           byte[] bytes = screenShot.EncodeToPNG();
           string filename = ScreenShotName(resWidth, resHeight);
           System.IO.File.WriteAllBytes(filename, bytes);
           Debug.Log(string.Format("Took screenshot to: {0}", filename));
           takeHiResShot = false;
           Obi.particletag.takepictureflag = false;
       }
       */
    }

}
