using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autotwoface : MonoBehaviour {

    // Use this for initialization
    private Shader shader;
    private Shader shader_backface;
   // public Texture texture;
    private Color materials_color;
	void Start () {
        SkinnedMeshRenderer cloth = this.GetComponent<SkinnedMeshRenderer>();
        GameObject tmp = this.gameObject;
        
        //Debug.Log(tmp);
        // tmp.AddComponent<Material>();
        //MeshRenderer cloth=this.GetComponent<MeshRenderer>();
        // utilise setvalue to 
        // from the early step it is only one material often, then use this to produce another two materials.
        shader_backface = Resources.Load("shader/StandardBackfaces") as Shader;
       // materials_color = tmp.GetComponent<Material>().color;
        //cloth.materials.SetValue(2,2);
        Renderer rendertmp = tmp.GetComponent<Renderer>();
        materials_color = rendertmp.material.color;     //use renderer to find materials
        rendertmp.materials = new Material[2];
        rendertmp.materials[1].shader = shader_backface;
        rendertmp.materials[0].color = materials_color;
        rendertmp.materials[1].color = materials_color;
        //Debug.Log(rendertmp);
        //Obi.ObiActor.
    }
    // Update is called once per frame
    void Update () {
		
	}
}
