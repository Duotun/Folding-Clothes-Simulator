using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using System;
using UnityEditor;

[RequireComponent(typeof(vectorparticle))]
[ExecuteInEditMode]
public class instruction : MonoBehaviour {
    KDTree tree;
    int treebuild = 0;
    // Use this for initialization
    private vectorparticle picker;
    private vectorparticle.ParticleVectorArgs pickArgs1;
    private vectorparticle.ParticleVectorArgs pickArgs2;
    private vectorparticle.ParticleVectorArgs pickArgs3;
    private vectorparticle.ParticleVectorArgs pickArgs4;
    private int countparticle = 0;
    private int drawparticle = 0;
    int[][] particleindexsphere = new int[2][];
    int[] centerparciel = new int[2];
    public static int counttwo = 0, dcounttwo = 0, flag = 0, dflag = 0;
    int mod2 = 0, bendingflag = 0;
    float currentime = 0f;
    public int button_choose = 0;
    Vector3 rotateaxis = Vector3.zero;
    public int lineflag = 1;
    public static int resetflag = 0;
    public static bool takepictureflag = false;
    //GameObject tagobject=new GameObject();
    Vector3 rotatepoint = Vector3.zero;
    public static GameObject[] twoobject = new GameObject[2];
    public static GameObject[] tmptwoobjectfordraw = new GameObject[2];
    int cnt1 = 0;
    int cnt2 = 0;
    int cnt3 = 0;   //for four particles
    int cnt4 = 0;
    // Use this for initialization
    void Start () {
        particleindexsphere[0] = new int[36];
        particleindexsphere[1] = new int[36];
        ObiCloth cloth = GetComponent<ObiCloth>();

        if (!cloth.InSolver)
        {
            Debug.Log("error");
        }
        Vector4[] positionss = new Vector4[cloth.positions.Length];
        Vector3[] positions = new Vector3[cloth.positions.Length];
        Vector3[] trynew = new Vector3[2];

        Oni.GetParticlePositions(cloth.Solver.OniSolver, positionss, cloth.positions.Length, cloth.particleIndices[0]);
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
    void kdtreesearch()
    {
        //tree = KDTree.MakeFromPoints();
        particleindexsphere[0] = tree.FindNearestsK(twoobject[0].transform.position, 36);  // find nearst 16
        particleindexsphere[1] = tree.FindNearestsK(twoobject[1].transform.position, 36);
        //Debug.Log(particleindexsphere[0][1]);
        //Debug.Log(particleindexsphere[1][1]);
    }
    void reset_parameter_auto()
    {
        screenshot.takeHiResShot = true; //take the picture
        countparticle = 0; drawparticle = 0; counttwo = 0; dcounttwo = 0; flag = 0; dflag = 0; mod2 = 0; currentime = 0f; button_choose = 0; rotateaxis = Vector3.zero; rotatepoint = Vector3.zero;
        button_choose = 0; resetflag = 1; drawline.reset = 1; lineflag = 1; draw_line_instruction.cnt = 0;
        draw_line_instruction.flag = 0; flag = 0;  //back
        Destroy(twoobject[0]); Destroy(tmptwoobjectfordraw[0]);
        Destroy(twoobject[1]); Destroy(tmptwoobjectfordraw[1]);    //trick
        cnt1 = cnt2 = cnt3 = cnt4 = 0; pickArgs1 = null;pickArgs2 = null;pickArgs3 = null;pickArgs4 = null;
        //Debug.Log("again");
    }
	// Update is called once per frame
	void FixedUpdate () {
        if (pickArgs1 != null && cnt1 == 0)
        {
            ObiSolver solver = picker.Cloth.Solver;
            Vector3 targetPosition = pickArgs1.worldPosition;
            if (solver.simulateInLocalSpace)
                targetPosition = solver.transform.InverseTransformPoint(targetPosition);

            // TAG THIS PARTICLE produce a little sphere once click
            Vector4[] positions = new Vector4[1];
            Vector4[] velocities = new Vector4[1];
            int solverIndex = picker.Cloth.particleIndices[pickArgs1.particleIndex];
            Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
            // Oni.GetParticlePositions()
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Debug.Log(sphere.GetType());
            sphere.transform.position = positions[0];
            sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            sphere.AddComponent<ObiCollider>();
            Renderer thissphere = sphere.GetComponent<Renderer>();
            thissphere.material.color = Color.red;
            tmptwoobjectfordraw[0] = sphere;
            sphere.GetComponent<ObiCollider>().Phase = 1;
            // particleindexsphere[counttwo][4] = pickArgs.particleIndex;
           // centerparciel[0] = pickArgs1.particleIndex;
            countparticle++;
            cnt1++;
        }
        else if (pickArgs2 != null && cnt2 == 0)
        {
            ObiSolver solver = picker.Cloth.Solver;
            Vector3 targetPosition = pickArgs2.worldPosition;
            if (solver.simulateInLocalSpace)
                targetPosition = solver.transform.InverseTransformPoint(targetPosition);

            // TAG THIS PARTICLE produce a little sphere once click
            Vector4[] positions = new Vector4[1];
            Vector4[] velocities = new Vector4[1];
            int solverIndex = picker.Cloth.particleIndices[pickArgs2.particleIndex];
            Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
            // Oni.GetParticlePositions()
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Debug.Log(sphere.GetType());
            sphere.transform.position = positions[0];
            sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            sphere.AddComponent<ObiCollider>();
            Renderer thissphere = sphere.GetComponent<Renderer>();
            thissphere.material.color = Color.blue;
            tmptwoobjectfordraw[1] = sphere;
            sphere.GetComponent<ObiCollider>().Phase = 1;
            // particleindexsphere[counttwo][4] = pickArgs.particleIndex;
            //centerparciel[1] = pickArgs2.particleIndex;
            countparticle++;
            cnt2++;
            rotateaxis = tmptwoobjectfordraw[1].transform.position - tmptwoobjectfordraw[0].transform.position;
            rotatepoint = (tmptwoobjectfordraw[1].transform.position + tmptwoobjectfordraw[0].transform.position) / 2;
        }

        if(cnt1==1&&cnt2==1&&dflag==0)
        {
            dflag = 1;
        }

        if(draw_line_instruction.flag==2&&flag==0)  //two black
        {
            //Debug.Log("pos");
            if (pickArgs3!=null&&cnt3==0)
            {
                //Debug.Log("pos===");
                ObiSolver solver = picker.Cloth.Solver;
                Vector3 targetPosition = pickArgs3.worldPosition;
                if (solver.simulateInLocalSpace)
                    targetPosition = solver.transform.InverseTransformPoint(targetPosition);

                // TAG THIS PARTICLE produce a little sphere once click
                Vector4[] positions = new Vector4[1];
                Vector4[] velocities = new Vector4[1];
                int solverIndex = picker.Cloth.particleIndices[pickArgs3.particleIndex];
                Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
                // Oni.GetParticlePositions()
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(sphere.GetType());
                sphere.transform.position = positions[0];
                sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.AddComponent<ObiCollider>();
                Renderer thissphere = sphere.GetComponent<Renderer>();
                thissphere.material.color = Color.black;
                twoobject[0] = sphere;
                sphere.GetComponent<ObiCollider>().Phase = 1;
                // particleindexsphere[counttwo][4] = pickArgs.particleIndex;
                centerparciel[0] = pickArgs3.particleIndex;
                countparticle++;
                cnt3++;
            }

            if (pickArgs4 != null && cnt4 == 0)
            {
                ObiSolver solver = picker.Cloth.Solver;
                Vector3 targetPosition = pickArgs4.worldPosition;
                if (solver.simulateInLocalSpace)
                    targetPosition = solver.transform.InverseTransformPoint(targetPosition);

                // TAG THIS PARTICLE produce a little sphere once click
                Vector4[] positions = new Vector4[1];
                Vector4[] velocities = new Vector4[1];
                int solverIndex = picker.Cloth.particleIndices[pickArgs4.particleIndex];
                Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
                // Oni.GetParticlePositions()
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Debug.Log(sphere.GetType());
                sphere.transform.position = positions[0];
                sphere.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.AddComponent<ObiCollider>();
                Renderer thissphere = sphere.GetComponent<Renderer>();
                thissphere.material.color = Color.black;
                twoobject[1] = sphere;
                sphere.GetComponent<ObiCollider>().Phase = 1;
                // particleindexsphere[counttwo][4] = pickArgs.particleIndex;
                centerparciel[1] = pickArgs4.particleIndex;
                countparticle++;
                cnt4++;
            }
                flag = 1;
            pinrotate_ready();
        }
        else if(draw_line_instruction.flag == 2 && flag == 1)
        {
            rotatenow();
        }

	}
    void pinrotate_ready()
    {
        ObiPinConstraints pins = this.GetComponent<ObiPinConstraints>();
        pins.RemoveFromSolver(null);
        ObiPinConstraintBatch batch = pins.GetFirstBatch() as ObiPinConstraintBatch;
        kdtreesearch();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 16; j++)   //16 or 36 further testing
            {
                batch.AddConstraint(particleindexsphere[i][j], twoobject[i].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
            }
            twoobject[i].GetComponent<ObiCollider>().Phase = 1;
            //twoobject[i].AddComponent<ObjectDragger>();
            //Debug.Log(twoobject[i].transform.position);
        }
        batch.AddConstraint(centerparciel[0], twoobject[0].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        batch.AddConstraint(centerparciel[1], twoobject[1].GetComponent<ObiCollider>(), Vector3.zero, 1.0f);
        pins.AddToSolver(null);
    }



    void rotatenow()
    {
        if (currentime <= 170.0f)
        {
            StartCoroutine(pin_rotate());
        }
        else
        {
            
            Destroy(twoobject[0]);
            Destroy(twoobject[1]);
            reset_parameter_auto();
        }

    }

    IEnumerator pin_rotate()
    {
        // time loop?

        twoobject[0].transform.RotateAround(rotatepoint, rotateaxis, 0.5f * Mathf.Rad2Deg * Time.fixedDeltaTime);
        twoobject[1].transform.RotateAround(rotatepoint, rotateaxis, 0.5f * Mathf.Rad2Deg * Time.fixedDeltaTime);
        currentime += 0.5f * Mathf.Rad2Deg * Time.fixedDeltaTime;
        yield return new WaitForSeconds(0.2f);

    }
    private void OnEnable()
    {
        picker = this.GetComponent<vectorparticle>();
        picker.OnParticlePickedaxis1 += Picker_OnParticleDraggedred;
        picker.OnParticlePickedaxis2 += Picker_OnParticleDraggedblue;
        picker.OnParticlePickedrotation1 += Picker_OnParticleDraggedrotation1;
        picker.OnParticlePickedrotation2 += Picker_OnParticleDraggedrotation2;
        picker.OnParticleReleased += Picker_OnParticleReleased;
    }
    private void OnDisable()
    {
        picker.OnParticlePickedaxis1 -= Picker_OnParticleDraggedred;
        picker.OnParticlePickedaxis2 -= Picker_OnParticleDraggedblue;
        picker.OnParticlePickedrotation1 -= Picker_OnParticleDraggedrotation1;
        picker.OnParticlePickedrotation2 -= Picker_OnParticleDraggedrotation2;
        picker.OnParticleReleased += Picker_OnParticleReleased;
    }
    private void Picker_OnParticleReleased(object sender, vectorparticle.ParticleVectorArgs e)
    {
        pickArgs1 = null;
        pickArgs2 = null;
        pickArgs3 = null;
        pickArgs4 = null;

    }
    private void Picker_OnParticleDraggedred(object sender, vectorparticle.ParticleVectorArgs e)
    {

        pickArgs1 = e;
    }
    private void Picker_OnParticleDraggedblue(object sender, vectorparticle.ParticleVectorArgs e)
    {

        pickArgs2 = e;
    }
    private void Picker_OnParticleDraggedrotation1(object sender, vectorparticle.ParticleVectorArgs e)
    {

        pickArgs3 = e;
    }
    private void Picker_OnParticleDraggedrotation2(object sender, vectorparticle.ParticleVectorArgs e)
    {

        pickArgs4 = e;
    }
}
