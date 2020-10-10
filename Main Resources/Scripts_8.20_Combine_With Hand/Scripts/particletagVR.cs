using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using VRTK;
[RequireComponent(typeof(pickerVR))]
[ExecuteInEditMode]
public class particletagVR : MonoBehaviour {
    KDTree tree;
    int treebuild = 0;
    private Mesh currentCollisionMesh;
    private MeshCollider meshCollider;
    private ObiClothBase cloth;
    // Use this for initialization
    private pickerVR picker;
    private pickerVR.ParticlePickEventArgs pickArgs;
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
    private void OnEnable()
    {
        picker = this.GetComponent<pickerVR>();
        picker.OnParticlePicked += Picker_OnParticleDragged;
        //picker.OnParticleDragged+= Picker_OnParticleDragged;
        picker.OnParticleReleased += Picker_OnParticleReleased;
    }
    private void OnDisable()
    {
        picker.OnParticlePicked -= Picker_OnParticleDragged;
        picker.OnParticleReleased -= Picker_OnParticleReleased;
        //picker.OnParticleDragged -= Picker_OnParticleDragged;

    }
    private void Awake()
    {
        cloth = GetComponent<ObiClothBase>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        /*if (meshCollider != null)
        {
            meshCollider.enabled = false;
            meshCollider.hideFlags = HideFlags.HideAndDontSave;
        }
        */
    }
    void rebuildtree()
    {
        ObiCloth cloth = GetComponent<ObiCloth>();

        // cloth.PullDataFromSolver(ParticleData.POSITIONS|ParticleData.VELOCITIES);
        //Debug.Log(cloth.particleIndices[0]);
        Vector4[] positionss = new Vector4[cloth.positions.Length];
        Vector3[] positions = new Vector3[cloth.positions.Length];
        Vector3[] trynew = new Vector3[2];

        // trynew[0] = Vector3.one; trynew[1] = Vector3.one;
        //Debug.Log(cloth.positions[0]);  // local space 
        //Debug.Log(cloth.particleIndices[0]); ????   ????must fix that!  runtime will produce this
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
            Debug.Log("error");
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
            Debug.Log("error");
        }

    }
    void choose()
    {

        switch (button_choose)  //
        {
           // case 0: resetflag = 0; particle_drawline_initial(); break;
            case 0: particle_drawline_initial(); break;
            case 1: fold_initial(); break;
        }

        if(button_choose==1&&drawlineVR.drawflag==1&&enterfold==0)   //record rotating axis
        {
            mod2 = 0; rotateaxis = twoobject[1].transform.position - twoobject[0].transform.position;
            rotatepoint = (twoobject[1].transform.position + twoobject[0].transform.position) / 2;
            enterfold = 1;
            twoobject[0] = twoobject[1] = null;
        }

        if (VRTK_ControllerEvents.drawflag==true&&button_choose==1&&enterfold==1&&lineflag==1)
        {
            if (twoobject[0] && twoobject[1]) //two object exist
            {
                pin_rotate_ready();
                flag = 1;
            }
        }
    }

    void reset_line()
    {
        countparticle = 0; drawparticle = 0; counttwo = 0; dcounttwo = 0; flag = 0; dflag = 0; mod2 = 0; currentime = 0f; button_choose = 0; rotateaxis = Vector3.zero; rotatepoint = Vector3.zero;
        drawline.cnt = 0; resetflag = 1; drawline.reset = 1; lineflag = 1; // all set to zero 
        drawline.flag = 0;
        //Debug.Log("ss+");
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
        if (bendingflag == 0)
        {
            temporarybendingkeep();
        }
        choose();
        if(pickArgs!=null)
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
        kdtreesearch();
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
    void newturn()
    {
        countparticle = 0; drawparticle = 0; counttwo = 0; dcounttwo = 0; flag = 0; dflag = 0; mod2 = 0; currentime = 0f; button_choose = 0; rotateaxis = Vector3.zero; rotatepoint = Vector3.zero;
        button_choose = 0; resetflag = 0; lineflag = 0; enterfold = 0;
        drawlineVR.finishflag = 1;  //back
    }
    void fold_initial()
    {
        if (flag == 0) //fold flag!
        {
            //Debug.Log("heieihei");
            fold();   //before click
        }
        else
        {
            if (currentime < 170.0f && flag == 1)   //<180 to avoid further collision
            {
                StartCoroutine(pin_rotate());

            }
            else
            {
                if (flag == 1)
                {
                    Destroy(twoobject[0]);
                    Destroy(twoobject[1]);
                    screenshot.takeHiResShot = true;
                    newturn();
                    rebuildtree();

                }
            }

        }
    }
    void fold()
    {
        if (pickArgs != null && countparticle < 1)
        {
            //Debug.Log("Wang!");
            if (counttwo < 2)
            {
                ObiSolver solver = picker.Cloth.Solver;
                Vector3 targetPosition = pickArgs.worldPosition;
                if (solver.simulateInLocalSpace)
                    targetPosition = solver.transform.InverseTransformPoint(targetPosition);

                // TAG THIS PARTICLE produce a little sphere once click
                Vector4[] positions = new Vector4[1];
                Vector4[] velocities = new Vector4[1];
                int solverIndex = picker.Cloth.particleIndices[pickArgs.particleIndex];
                Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
                // Oni.GetParticlePositions()
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(sphere.GetType());
                sphere.transform.position = positions[0];
                sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.AddComponent<ObiCollider>();
                Renderer thissphere = sphere.GetComponent<Renderer>();
                thissphere.material.color = Color.black;
                twoobject[counttwo] = sphere;
                sphere.GetComponent<ObiCollider>().Phase = 1;
                // particleindexsphere[counttwo][4] = pickArgs.particleIndex;
                centerparciel[counttwo] = pickArgs.particleIndex;
                countparticle++;
                counttwo++;
            }
            else
            {
                Destroy(twoobject[mod2]);
                mod2++; mod2 %= 2;
                ObiSolver solver = picker.Cloth.Solver;
                Vector3 targetPosition = pickArgs.worldPosition;
                if (solver.simulateInLocalSpace)
                    targetPosition = solver.transform.InverseTransformPoint(targetPosition);

                // TAG THIS PARTICLE produce a little sphere once click
                Vector4[] positions = new Vector4[1];
                Vector4[] velocities = new Vector4[1];
                int solverIndex = picker.Cloth.particleIndices[pickArgs.particleIndex];
                Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(sphere.GetType());
                sphere.transform.position = positions[0];
                sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.AddComponent<ObiCollider>();
                sphere.GetComponent<ObiCollider>().Phase = 1;
                Renderer thissphere = sphere.GetComponent<Renderer>();
                thissphere.material.color = Color.black;
                twoobject[(mod2 + 1) % 2] = sphere;
                //particleindexsphere[(mod2 + 1) % 2][4] = pickArgs.particleIndex;
                centerparciel[(mod2 + 1) % 2] = pickArgs.particleIndex;
                countparticle++;

            }

        }
        else if (pickArgs == null)    // make one click generate one particle
        {
            countparticle = 0;
        }
    }
    void kdtreesearch()
    {
        //tree = KDTree.MakeFromPoints();
        particleindexsphere[0] = tree.FindNearestsK(twoobject[0].transform.position, 26);  // find nearst 16
        particleindexsphere[1] = tree.FindNearestsK(twoobject[1].transform.position, 26);
        //Debug.Log(particleindexsphere[0][1]);
        //Debug.Log(particleindexsphere[1][1]);
    }
    //follow the same way choose two particle and then form a line. get the vector!     + render differently
    void particle_drawline_initial()
    {
        if (pickArgs != null && drawparticle < 1)
        {
            
            if (dcounttwo < 2)
            {
                ObiSolver solver = picker.Cloth.Solver;
                Vector3 targetPosition = pickArgs.worldPosition;
                if (solver.simulateInLocalSpace)
                    targetPosition = solver.transform.InverseTransformPoint(targetPosition);

                // TAG THIS PARTICLE produce a little sphere once click
                Vector4[] positions = new Vector4[1];
                Vector4[] velocities = new Vector4[1];
                int solverIndex = picker.Cloth.particleIndices[pickArgs.particleIndex];
                Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(sphere.GetType());
                sphere.transform.position = positions[0];
                sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.AddComponent<ObiCollider>();
                Renderer thissphere = sphere.GetComponent<Renderer>();
                if (dcounttwo == 0)
                {
                    thissphere.material.color = Color.red;
                }
                else
                {
                    thissphere.material.color = Color.blue;
                }
                twoobject[dcounttwo] = sphere;
                sphere.GetComponent<ObiCollider>().Phase = 1;
                //particleindexsphere[dcounttwo] = pickArgs.particleIndex;
                drawparticle++;
                dcounttwo++;
            }
            else
            {
                Destroy(twoobject[mod2]);
                mod2++; mod2 %= 2;
                ObiSolver solver = picker.Cloth.Solver;
                Vector3 targetPosition = pickArgs.worldPosition;
                if (solver.simulateInLocalSpace)
                    targetPosition = solver.transform.InverseTransformPoint(targetPosition);
                // TAG THIS PARTICLE produce a little sphere once click
                Vector4[] positions = new Vector4[1];
                Vector4[] velocities = new Vector4[1];
                int solverIndex = picker.Cloth.particleIndices[pickArgs.particleIndex];
                Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(sphere.GetType());
                sphere.transform.position = positions[0];
                sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.AddComponent<ObiCollider>();
                sphere.GetComponent<ObiCollider>().Phase = 1;
                Renderer thissphere = sphere.GetComponent<Renderer>();
                if ((mod2 + 1) % 2 == 0)
                {
                    thissphere.material.color = Color.red;
                }
                else
                {
                    thissphere.material.color = Color.blue;
                }
                twoobject[(mod2 + 1) % 2] = sphere;
                // particleindexsphere[(mod2 + 1) % 2] = pickArgs.particleIndex;
                drawparticle++;

            }

        }
        else if (pickArgs == null)    // make one click generate one particle
        {
            drawparticle = 0;
        }
    }
    /*void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 100, 25), "Confirm Menu");

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(20, 40, 80, 20), "Fold"))
        {
            pin_rotate_ready();
            flag = 1;
        }
        if (GUI.Button(new Rect(20, 60, 80, 20), "Axis"))
        {
            if (twoobject[1] != null && twoobject[0] != null)
            {
                //particle_drawline_initial();
                mod2 = 0; rotateaxis = twoobject[1].transform.position - twoobject[0].transform.position;
                rotatepoint = (twoobject[1].transform.position + twoobject[0].transform.position) / 2;
                button_choose = 1; lineflag = 0;  //Debug.Log("ssnow1");
            }
            //pin_rotate_ready();
        }

        if (GUI.Button(new Rect(20, 80, 80, 20), "Reset"))
        {
            //Debug.Log("psda");
            reset_line();
        }

    }
    */

    private void Picker_OnParticleReleased(object sender, pickerVR.ParticlePickEventArgs e)
    {
        pickArgs = null;
        //Debug.Log("come on");
    }
    private void Picker_OnParticleDragged(object sender, pickerVR.ParticlePickEventArgs e)
    {

        pickArgs = e;
       
    }
}

