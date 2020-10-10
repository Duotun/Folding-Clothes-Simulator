using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Obi;
using UnityEngineInternal;

/*public class clothinitialize : ObiCloth
{
    ObiCloth tmp=new ObiCloth();
    
}
*/
//[ExecuteInEditMode]


public class autocloth : MonoBehaviour
{
   
    GameObject here;
    private void Awake()
    {
        
        test_mesh();
       // changetoskinnedrender();
       // ObiCloth ss = new ObiCloth();
        //bool s = ss.Initializing;
    }
    // Use this for initialization
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.03f);
    }
    IEnumerator  Start () {

        IEnumerator e = generatecloth();
        yield return e;
        if (!e.MoveNext()) // over the end
        {
            initialize_cloth_property();
           // StartCoroutine(wait());
            //EditorApplication.isPaused = true;
        }
        else
        {
            Debug.Log("Wrong Simulation!");
            EditorApplication.isPaused = true;
        }
         // add two side  all both side is clear need to be initialized
	}
	
     private void changetoskinnedrender()   //mesh render to skinned render
    {
        here = this.gameObject;
        here.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer skintmp = here.GetComponent<SkinnedMeshRenderer>();
        MeshRenderer tmp = here.GetComponent<MeshRenderer>();
        MeshFilter filtertmp = here.GetComponent<MeshFilter>();
        skintmp.sharedMesh = filtertmp.sharedMesh;
        tmp.enabled = false;

    }
    private void test_mesh()
    {
       
       // ObiMeshTopology.CreateInstance("meshtopology");
    }
    IEnumerator generatecloth()
    {
        here = this.gameObject;
        //Debug.Log(here.name);
        //Debug.Log(this.name);
        //ObiMeshTopology.CreateInstance("meshtopology");
        ObiMeshTopology meshtopology = ScriptableObject.CreateInstance<ObiMeshTopology>();
        meshtopology.InputMesh = here.GetComponent<MeshFilter>().sharedMesh;
        // Debug.Log(ss.InputMesh.name);
        meshtopology.Generate(); //for meshtoplogy
        
        GameObject solveroriginal = new GameObject();
        //get solver from the assets
        // solveroriginal.AddComponent<ObiSolver>();   
            here.AddComponent<ObiCloth>();
            ObiCloth tmp = here.GetComponent<ObiCloth>();
            //tmp.Solver.RelinquishRenderablePositions();
            tmp.SelfCollisions = true;
            //set solve parameters
            // or tmp.Solver = solveroriginal.AddComponent<ObiSolver>();
            tmp.Solver = solveroriginal.AddComponent<ObiSolver>();
            tmp.Solver.maxParticles = 6000;
            tmp.Solver.volumeConstraintParameters.enabled = false;
            tmp.Solver.skinConstraintParameters.enabled = false;
            
            tmp.Solver.tetherConstraintParameters.enabled = false;
            
            tmp.Solver.densityConstraintParameters.enabled = false;


            tmp.Solver.bendingConstraintParameters.evaluationOrder = Oni.ConstraintParameters.EvaluationOrder.Parallel;   //sequantial to reduce iterations amount
            tmp.Solver.distanceConstraintParameters.iterations = 5;
            tmp.Solver.bendingConstraintParameters.iterations = 5;
            tmp.Solver.shapeMatchingConstraintParameters.iterations = 5;
            tmp.Solver.collisionConstraintParameters.iterations = 5;
            tmp.Solver.shapeMatchingConstraintParameters.iterations = 5;
            tmp.Solver.stitchConstraintParameters.iterations = 5;
            tmp.Solver.particleCollisionConstraintParameters.iterations = 5;
            tmp.Solver.pinConstraintParameters.iterations = 5;     // pin constraints for particle
            tmp.Solver.substeps = 1;  //improve substeps for a better quality than manual setting
            //  tmp.Solver.
            tmp.SharedTopology = meshtopology;
             


        yield return tmp.StartCoroutine(tmp.GeneratePhysicRepresentationForMesh());
        tmp.AddToSolver(null);   //add constraints back
        
    }
    void initialize_cloth_property()
    {
        // bending constraints
        // cancel some constraints
        ObiTetherConstraints tether = this.GetComponent<ObiTetherConstraints>();
        tether.enabled = false;
        tether.PushDataToSolver();
        ObiSkinConstraints skin = this.GetComponent<ObiSkinConstraints>();
        skin.enabled = false;
        skin.PushDataToSolver();
        ObiVolumeConstraints volume = this.GetComponent<ObiVolumeConstraints>();
        volume.enabled = false;
        volume.PushDataToSolver();
        ObiAerodynamicConstraints aero = this.GetComponent<ObiAerodynamicConstraints>();
        aero.enabled = false;
        aero.PushDataToSolver();
        ObiCloth tmp = this.GetComponent<ObiCloth>();
        tmp.SelfCollisions = true;
        tmp.CollisionMaterial=Resources.Load("collision_material/MediumFriction") as ObiCollisionMaterial;
        tmp.PushDataToSolver();

        
       //tmp.runInEditMode = true;  won't destroy after playing
        //unfix particle
        //for (int i = 0; i < tmp.invMasses.Length; i++) 
          //     tmp.invMasses[i]= 0; 
       

    }
     // void take a picture
    void manualpause()     
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            /* if (!EditorApplication.isPaused)
                 EditorApplication.isPaused = true;
             else
                 EditorApplication.isPaused = false;
                 */
            /*if (Time.timeScale > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
                */
            if (EditorApplication.isPlaying == true)
            {
                Debug.Log("sss");
                Debug.Break();
            }
            else
            {
                ///Debug.Log("sss");
                EditorApplication.isPlaying = true;
            }
        }

    }
    // Update is called once per frame
/*#if UNITY_EDITOR
    void Update()
    {
       // Debug.Log("Its time: " + Time.time);
        if (Application.isEditor && !Application.isPlaying)
        {
            //do what you want
            Debug.Log("Its time: " + Time.time);
        }
    }
#endif
*/

    void Update()
    {
        // manualpause();

       /* if (Application.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Break();
               
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EditorApplication.isPaused = false;
            }

        }
        */
        //if(Application.platform == RuntimePlatform.WindowsEditor)
          //     Debug.Log("Do something special here!");
    }
   
}
