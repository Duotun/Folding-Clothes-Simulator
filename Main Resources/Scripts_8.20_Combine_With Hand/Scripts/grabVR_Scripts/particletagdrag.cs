using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using VRTK;
[RequireComponent(typeof(grabcontrol))]
[ExecuteInEditMode]
public class particletagdrag : MonoBehaviour
{
    KDTree tree;
    int treebuild = 0;
    private Mesh currentCollisionMesh;
    private MeshCollider meshCollider;
    private ObiClothBase cloth;
    // Use this for initialization
    private grabcontrol picker;
    private grabcontrol.ParticlePickEventArgs pickArgsLeft;
    private grabcontrol.ParticlePickEventArgs pickArgsRight;
    private int countparticle = 0;
    private int drawparticle = 0;
    int[][] particleindexsphere = new int[2][];
    int[] centerparciel = new int[2];
    public int counttwo = 0, dcounttwo = 0, flag = 0, dflag = 0;
    int mod2 = 0, bendingflag = 0;
    float currentime = 0f;
    public static int button_choose = 0;
    Vector3 rotateaxis = Vector3.zero;
    public static int lineflag = 0;
    public static int resetflag = 0;
    public static bool takepictureflag = false;
    private int enterfold = 0;
    //GameObject tagobject=new GameObject();
    Vector3 rotatepoint = Vector3.zero;
    public GameObject[] twoobject = new GameObject[2];
    private GameObject[] tmptwoobjectfordraw = new GameObject[2];
    GameObject leftobject;
    GameObject rightobject;
    Vector3 leftposition;
    Vector3 rightposition;
    int leftpin = 0;
    int rightpin = 0;
    Vector3[] twoobejectpositiononcloth = new Vector3[2];
    private void OnEnable()
    {
        picker = this.GetComponent<grabcontrol>();
        picker.OnParticlePickedLeft += Picker_OnParticleDraggedLeft;
        //picker.OnParticleDragged+= Picker_OnParticleDragged;
        picker.OnParticleReleasedLeft += Picker_OnParticleReleasedLeft;
        picker.OnParticlePickedRight += Picker_OnParticleDraggedRight;
        picker.OnParticleReleasedRight += Picker_OnParticleReleasedRight;
    }
    private void OnDisable()
    {
        picker.OnParticlePickedLeft -= Picker_OnParticleDraggedLeft;
        picker.OnParticleReleasedLeft -= Picker_OnParticleReleasedLeft;
        picker.OnParticlePickedRight -= Picker_OnParticleDraggedRight;
        picker.OnParticleReleasedRight -= Picker_OnParticleReleasedRight;
        //picker.OnParticleDragged -= Picker_OnParticleDragged;

    }
    private void Awake()
    {
        cloth = GetComponent<ObiClothBase>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

    }
    void rebuildtree()
    {
        ObiCloth cloth = GetComponent<ObiCloth>();

        // cloth.PullDataFromSolver(ParticleData.POSITIONS|ParticleData.VELOCITIES);
        //Debug.Log(cloth.particleIndices[0]);
        Vector4[] positionss = new Vector4[cloth.positions.Length];
        Vector3[] positions = new Vector3[cloth.positions.Length];
        Vector3[] trynew = new Vector3[2];

        Oni.GetParticlePositions(cloth.Solver.OniSolver, positionss, cloth.positions.Length, cloth.particleIndices[0]);
        //Oni.GetParticlePositions(cloth.Solver.OniSolver, position, cloth.positions.Length, 0);
        //Debug.Log(position[0]);
        for (int i = 0; i < cloth.positions.Length; i++)
        {
            positions[i] = positionss[i];
            //Debug.Log(positions[i]);
        }
        tree = KDTree.MakeFromPoints(positions);

        if (tree == null)
        {
            Debug.Log("error_tree");
        }
    }
    void Start()   // although it seems need to be update, but the initial position of clothes shuold be enough to find near positions of particles
    {
        particleindexsphere[0] = new int[36];
        particleindexsphere[1] = new int[36];
        ObiCloth cloth = GetComponent<ObiCloth>();
        cloth.PinConstraints.PushDataToSolver();
        if (!cloth.InSolver)
        {
            Debug.Log("error");
        }
        // cloth.PullDataFromSolver(ParticleData.POSITIONS|ParticleData.VELOCITIES);
        //Debug.Log(cloth.particleIndices[0]);
        Vector4[] positionss = new Vector4[cloth.positions.Length];
        Vector3[] positions = new Vector3[cloth.positions.Length];
        Vector3[] trynew = new Vector3[2]; 

        // trynew[0] = Vector3.one; trynew[1] = Vector3.one;
        Oni.GetParticlePositions(cloth.Solver.OniSolver, positionss, cloth.positions.Length, cloth.particleIndices[0]);
        //Oni.GetParticlePositions(cloth.Solver.OniSolver, position, cloth.positions.Length, 0);
        //Debug.Log(position[0]);
        for (int i = 0; i < cloth.positions.Length; i++)
        {
            positions[i] = positionss[i];
            //Debug.Log(positions[i]);
        }
        tree = KDTree.MakeFromPoints(positions);

        if (tree == null)
        {
            Debug.Log("error_tree");
        }
        leftobject =  GameObject.Find("LeftController/[VRTK][AUTOGEN][Controller][CollidersContainer]/Head");
        rightobject = GameObject.Find("RightController/[VRTK][AUTOGEN][Controller][CollidersContainer]/Head");
        leftobject.AddComponent<ObiCollider>();
        rightobject.AddComponent<ObiCollider>();
        leftobject.GetComponent<ObiCollider>().Phase = 1;
        rightobject.AddComponent<ObiCollider>().Phase = 1;
        if (leftobject && rightobject)
        {
            Debug.Log(leftobject.transform.position);
            Debug.Log(rightobject.transform.position);
        }
        else
        {
            Debug.Log("failure");
        }
        twoobject[0] = leftobject;
        twoobject[1] = rightobject;
       
    }
    void choose()   //build pin constraints
    {
        if(pickArgsLeft!=null)
        {
            
            ObiSolver solver = picker.Cloth.Solver;
            Vector3 targetPosition = pickArgsLeft.worldPosition;
            if (solver.simulateInLocalSpace)
                targetPosition = solver.transform.InverseTransformPoint(targetPosition);

            // TAG THIS PARTICLE produce a little sphere once click
            Vector4[] positions = new Vector4[1];
            Vector4[] velocities = new Vector4[1];
            Vector3 position = new Vector3();
            int solverIndex = picker.Cloth.particleIndices[pickArgsLeft.particleIndex];
            Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
            // Oni.GetParticlePositions()
           // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
           // sphere.transform.position = positions[0];
           // sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            //Debug.Log(sphere.GetType());
            centerparciel[0] = pickArgsLeft.particleIndex;
            position = positions[0];
            float distance = (leftposition - position).magnitude;
            if(distance<0.08f)
            {
                if (leftpin == 0)   //pin once
                {
                    twoobejectpositiononcloth[0] = position;
                    pin_left();
                }
            }

        }
        else if(pickArgsLeft==null&&leftpin>0)  //release_pin
        {
            release_pin_left();
        }

        if(pickArgsRight!=null)
        {
            //Debug.Log("ppp");
            ObiSolver solver = picker.Cloth.Solver;
            Vector3 targetPosition = pickArgsRight.worldPosition;
            if (solver.simulateInLocalSpace)
                targetPosition = solver.transform.InverseTransformPoint(targetPosition);

            // TAG THIS PARTICLE produce a little sphere once click
            Vector4[] positions = new Vector4[1];
            Vector4[] velocities = new Vector4[1];
            Vector3 position = new Vector3();
            int solverIndex = picker.Cloth.particleIndices[pickArgsRight.particleIndex];
            Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
            // Oni.GetParticlePositions()
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = positions[0];
            //sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            //Debug.Log(sphere.GetType());
            centerparciel[1] = pickArgsRight.particleIndex;
            position = positions[0];
            float distance = (rightposition - position).magnitude;
            if (distance < 0.08f)
            {
                if (rightpin == 0)
                {
                    Debug.Log("sss_1");
                    twoobejectpositiononcloth[1] = position;
                    pin_right();
                }
            }
        }
        else if(pickArgsRight==null&&rightpin>0)
        {
            release_pin_right();
        }

        if(rightpin==0&&leftpin==0&&pickArgsRight==null&&pickArgsLeft==null)  //kdtree update
        {
            rebuildtree();
        }
       /* 
          switch (button_choose)  //
        {
            // case 0: resetflag = 0; particle_drawline_initial(); break;
            case 0: particle_drawline_initial(); break;
            case 1: fold_initial(); break;
        }

        if (button_choose == 1 && drawlineVR.drawflag == 1 && enterfold == 0)   //record rotating axis
        {
            mod2 = 0; rotateaxis = twoobject[1].transform.position - twoobject[0].transform.position;
            rotatepoint = (twoobject[1].transform.position + twoobject[0].transform.position) / 2;
            enterfold = 1;
            twoobject[0] = twoobject[1] = null;
        }

        if (VRTK_ControllerEvents.drawflag == true && button_choose == 1 && enterfold == 1 && lineflag == 1)
        {
            if (twoobject[0] && twoobject[1]) //two object exist
            {
                pin_rotate_ready();
                flag = 1;
            }
        }
        */


    }
    void pin_left()
    {
        leftpin++;
        ObiPinConstraints pins = this.GetComponent<ObiPinConstraints>();
        Debug.Log(pins.gameObject);
        pins.RemoveFromSolver(null);
        ObiPinConstraintBatch batch = pins.GetFirstBatch() as ObiPinConstraintBatch;

        //attach more particle to pin
        kdtreesearch_left();
            for (int j = 0; j < 16; j++)
            {
                batch.AddConstraint(particleindexsphere[0][j], twoobject[0].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
            }
            //twoobject[0].GetComponent<ObiCollider>().Phase = 1;
            //twoobject[0].AddComponent<ObjectDragger>();
        batch.AddConstraint(centerparciel[0], twoobject[0].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        pins.AddToSolver(null);

        
    }
    void pin_right()
    {
        rightpin++;
        ObiPinConstraints pins = this.GetComponent<ObiPinConstraints>();
        Debug.Log(pins.gameObject);
        pins.RemoveFromSolver(null);
        kdtreesearch_right();
        ObiPinConstraintBatch batch = pins.GetFirstBatch() as ObiPinConstraintBatch;
        for (int j = 0; j < 16; j++)
        {
            batch.AddConstraint(particleindexsphere[1][j], twoobject[1].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        }
        //twoobject[0].GetComponent<ObiCollider>().Phase = 1;
        //twoobject[0].AddComponent<ObjectDragger>();
        batch.AddConstraint(centerparciel[1], twoobject[1].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        pins.AddToSolver(null);
        
    }

    void release_pin_left()
    {
        leftpin = 0;
        ObiPinConstraints pins = this.GetComponent<ObiPinConstraints>();
        pins.RemoveFromSolver(null);
        ObiPinConstraintBatch batch = pins.GetFirstBatch() as ObiPinConstraintBatch;
        for (int i = 0; i < 17; i++)
            batch.RemoveConstraint(0);
        pins.AddToSolver(null);
        Debug.Log("leave_left");
    }

    void release_pin_right()
    {
        rightpin = 0;
        ObiPinConstraints pins = this.GetComponent<ObiPinConstraints>();
        pins.RemoveFromSolver(null);
        ObiPinConstraintBatch batch = pins.GetFirstBatch() as ObiPinConstraintBatch;
        for(int i=0;i<17;i++)
            batch.RemoveConstraint(0);
        pins.AddToSolver(null);
        Debug.Log("leave_right");
    }
    void temporarybendingkeep()
    {

        ObiCloth cloth = this.GetComponent<ObiCloth>();
        if (cloth.Initialized == true)
        {
            cloth.BendingConstraints.maxBending = 0f;
            cloth.BendingConstraints.PushDataToSolver();
            StartCoroutine(wait_bending());
            bendingflag = 1;
        }

    }
    IEnumerator wait_bending()
    {
        yield return new WaitForSeconds(3f);
        ObiCloth cloth = this.GetComponent<ObiCloth>();
        cloth.BendingConstraints.maxBending = 0.008f;
        cloth.BendingConstraints.PushDataToSolver();    //update your parameter for constraints
    }
    void FixedUpdate()
    {
        leftposition = leftobject.transform.position;
        rightposition = rightobject.transform.position;

        if (bendingflag == 0)
        {
            temporarybendingkeep();
        }
        choose();
        if (pickArgsLeft != null)
        {
            //Debug.Log("sss");
        }
    }
    // make pin then, rotate, particleindex is critical to link 
    IEnumerator pin_rotate()
    {
        // time loop?

        twoobject[0].transform.RotateAround(rotatepoint, rotateaxis, 0.5f * Mathf.Rad2Deg * Time.fixedDeltaTime);
        twoobject[1].transform.RotateAround(rotatepoint, rotateaxis, 0.5f * Mathf.Rad2Deg * Time.fixedDeltaTime);
        currentime += 0.5f * Mathf.Rad2Deg * Time.fixedDeltaTime;
        yield return new WaitForSeconds(0.2f);

    }
    void pin_rotate_ready()
    {
        //GameObject[] twoparticle = new GameObject[2];
        // Debug.Log("wang");
        ObiPinConstraints pins = this.GetComponent<ObiPinConstraints>();
        pins.RemoveFromSolver(null);
        ObiPinConstraintBatch batch = pins.GetFirstBatch() as ObiPinConstraintBatch;

        //attach more particle to pin
        kdtreesearch_left();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                batch.AddConstraint(particleindexsphere[i][j], twoobject[i].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
            }
            twoobject[i].GetComponent<ObiCollider>().Phase = 1;
            twoobject[i].AddComponent<ObjectDragger>();
            //Debug.Log(twoobject[i].transform.position);
        }
        batch.AddConstraint(centerparciel[0], twoobject[0].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        batch.AddConstraint(centerparciel[1], twoobject[1].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        pins.AddToSolver(null);



    }
   /* void newturn()
    {
        countparticle = 0; drawparticle = 0; counttwo = 0; dcounttwo = 0; flag = 0; dflag = 0; mod2 = 0; currentime = 0f; button_choose = 0; rotateaxis = Vector3.zero; rotatepoint = Vector3.zero;
        button_choose = 0; resetflag = 0; lineflag = 0; enterfold = 0;
        drawlineVR.finishflag = 1;  //back
    }
    */

    void kdtreesearch_left()
    {
        //tree = KDTree.MakeFromPoints();
        particleindexsphere[0] = tree.FindNearestsK(twoobejectpositiononcloth[0], 16);  // find nearst 16
        
        //Debug.Log(particleindexsphere[0][1]);
        //Debug.Log(particleindexsphere[1][1]);
    }
    void kdtreesearch_right()
    {
        //tree = KDTree.MakeFromPoints();

        particleindexsphere[1] = tree.FindNearestsK(twoobejectpositiononcloth[1], 16);
        //Debug.Log(particleindexsphere[0][1]);
        //Debug.Log(particleindexsphere[1][1]);
    }
    //follow the same way choose two particle and then form a line. get the vector!     + render differently
    private void Picker_OnParticleReleasedLeft(object sender, grabcontrol.ParticlePickEventArgs e)
    {
        pickArgsLeft = null;

        //Debug.Log("come on");
    }
    private void Picker_OnParticleDraggedLeft(object sender, grabcontrol.ParticlePickEventArgs e)
    {

        pickArgsLeft = e;

    }
    private void Picker_OnParticleDraggedRight(object sender, grabcontrol.ParticlePickEventArgs e)
    {

        pickArgsRight = e;

    }

    private void Picker_OnParticleReleasedRight(object sender, grabcontrol.ParticlePickEventArgs e)
    {
        pickArgsRight = null;

        //Debug.Log("come on");
    }
}

