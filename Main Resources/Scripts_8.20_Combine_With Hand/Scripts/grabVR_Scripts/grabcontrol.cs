using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Obi;
using VRTK;
namespace Obi
{
    public class grabcontrol : MonoBehaviour
    {

        public class ParticlePickEventArgs : EventArgs
        {

            public int particleIndex;
            public Vector3 worldPosition;

            public ParticlePickEventArgs(int particleIndex, Vector3 worldPosition)
            {
                this.particleIndex = particleIndex;
                this.worldPosition = worldPosition;
            }
        }

        public event System.EventHandler<ParticlePickEventArgs> OnParticlePickedLeft;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleHeldLeft;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleDraggedLeft;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleReleasedLeft;

        public event System.EventHandler<ParticlePickEventArgs> OnParticlePickedRight;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleHeldRight;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleDraggedRight;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleReleasedRight;

        private MeshCollider meshCollider;
        private ObiClothBase cloth;
        private Mesh currentCollisionMesh;

        private Vector3 lastMousePos = Vector3.zero;
        private int pickedParticleIndexLeft = -1;
        private float pickedParticleDepthLeft = 0;

        private int pickedParticleIndexRight = -1;
        private float pickedParticleDepthRight = 0;
        private Vector3 hitPoint;
        public static float distancebetweencontrollerandpoint;
        VRTK_Pointer left;
        VRTK_Pointer right;
        RaycastHit lefthit;
        RaycastHit righthit;
        int leftcontrol, rightcontrol;
        public static float leftdis,rightdis;
        public ObiClothBase Cloth
        {
            get { return cloth; }
        }

        private void Start()
        {   //find Head Model

        }
        void Awake()
        {
            //hitPoint = pointer.hitPoint;


            cloth = GetComponent<ObiClothBase>();

            //if (GameObject.FindWithTag("right") != null)
            //{
                right = GameObject.Find("RightController").GetComponent<VRTK_Pointer>();
                //Debug.Log(right.gameObject.name);
                left = GameObject.Find("LeftController").GetComponent<VRTK_Pointer>();
            //}
            //else
            //{
              //  Debug.Log("fuck?");
            //}
            lastMousePos = hitPoint;  // kick
                                      // trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        void OnEnable()
        {

            // special case for skinned cloth, the collider must be added to the skeleton's root bone:
            if (cloth is ObiCloth && ((ObiCloth)cloth).IsSkinned)
            {

                SkinnedMeshRenderer sk = cloth.GetComponent<SkinnedMeshRenderer>();
                if (sk != null && sk.rootBone != null)
                {
                    meshCollider = sk.rootBone.gameObject.AddComponent<MeshCollider>();
                }
            }
            // regular cloth:
            else
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }

            // in case we were able to create the mesh collider, set it up:
            if (meshCollider != null)
            {
                meshCollider.enabled = false;
                meshCollider.hideFlags = HideFlags.HideAndDontSave;
            }

            if (cloth != null)
                cloth.Solver.OnFrameBegin += Cloth_Solver_OnFrameBegin;
        }

        void OnDisable()
        {

            // destroy the managed mesh collider:
            GameObject.Destroy(meshCollider);

            if (cloth != null)
                cloth.Solver.OnFrameBegin -= Cloth_Solver_OnFrameBegin;
        }

        void leftpress()
        {
            if(leftcontrol==0)
            {
                meshCollider.enabled = true;

                GameObject.Destroy(currentCollisionMesh);
                currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
                meshCollider.sharedMesh = currentCollisionMesh;

                VRTK_StraightPointerRenderer tmp = GameObject.Find("LeftController").GetComponent<VRTK_StraightPointerRenderer>();
                Ray ray = tmp.getray();

                RaycastHit hitInfo;
                //Debug.Log("left_drag");
                if (meshCollider.Raycast(ray, out hitInfo, 100))
                {

                    int[] tris = currentCollisionMesh.triangles;
                    Vector3[] vertices = currentCollisionMesh.vertices;

                    lefthit = hitInfo;
                    // find closest vertex in the triangle we just hit:
                    int closestVertex = -1;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < 3; ++i)
                    {
                        int vertex = tris[hitInfo.triangleIndex * 3 + i];
                        float distance = (vertices[vertex] - hitInfo.point).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestVertex = vertex;
                        }
                    }

                    // get particle index:
                    if (closestVertex >= 0 && closestVertex < cloth.topology.visualMap.Length)
                    {

                        pickedParticleIndexLeft = cloth.topology.visualMap[closestVertex];
                        pickedParticleDepthLeft = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                        if (OnParticlePickedLeft != null)
                        {
                            //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepthLeft));
                            Vector3 worldPosition = new Vector3(hitInfo.point.x, hitInfo.point.y, pickedParticleDepthLeft);
                            OnParticlePickedLeft(this, new ParticlePickEventArgs(pickedParticleIndexLeft, worldPosition));
                        }
                    }
                }

                meshCollider.enabled = false;
                leftcontrol++;
            }
            
        }
        void rightpress()
        {
            if(rightcontrol==0)
            {
                meshCollider.enabled = true;

                GameObject.Destroy(currentCollisionMesh);
                currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
                meshCollider.sharedMesh = currentCollisionMesh;
                VRTK_StraightPointerRenderer tmp = GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>();
                Ray ray =tmp.getray();

                RaycastHit hitInfo;
                //Debug.Log("right_drag");
                if (meshCollider.Raycast(ray, out hitInfo, 100))
                {

                    righthit = hitInfo;
                    int[] tris = currentCollisionMesh.triangles;
                    Vector3[] vertices = currentCollisionMesh.vertices;

                    // find closest vertex in the triangle we just hit:
                    int closestVertex = -1;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < 3; ++i)
                    {
                        int vertex = tris[hitInfo.triangleIndex * 3 + i];
                        float distance = (vertices[vertex] - hitInfo.point).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestVertex = vertex;
                        }
                    }

                    // get particle index:
                    if (closestVertex >= 0 && closestVertex < cloth.topology.visualMap.Length)
                    {

                        pickedParticleIndexRight = cloth.topology.visualMap[closestVertex];
                        pickedParticleDepthRight = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                        if (OnParticlePickedRight != null)
                        {
                            //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepthRight));
                            Vector3 worldPosition = new Vector3(hitInfo.point.x, hitInfo.point.y, pickedParticleDepthRight);
                            OnParticlePickedRight(this, new ParticlePickEventArgs(pickedParticleIndexRight, worldPosition));
                        }
                    }
                }

                meshCollider.enabled = false;
                rightcontrol++;
            }
            

        }
        void Cloth_Solver_OnFrameBegin(object sender, EventArgs e)
        {
            if (meshCollider == null)
                return;
            RaycastHit hitInfo = VRTK_Pointer.rayhit;
            // Click:
            
            if (left.controller.triggerPressed)
            {
                Debug.Log("fuck");
                leftcontrol = 0;
                leftpress();
            }
            else if (pickedParticleIndexLeft >= 0)
            {
                if (!left.controller.triggerPressed)
                {
                    //Debug.Log("left_release");
                    if (OnParticleReleasedLeft != null)
                    {
                        // Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepthLeft));
                        Vector3 worldPosition = new Vector3(lefthit.point.x, lefthit.point.y, pickedParticleDepthLeft);
                        OnParticleReleasedLeft(this, new ParticlePickEventArgs(pickedParticleIndexLeft, worldPosition));
                    }

                    pickedParticleIndexLeft = -1;

                }
            }

            if (right.controller.triggerPressed)
            {
                rightcontrol = 0;
                rightpress();
            }
            else if (pickedParticleIndexRight >= 0)
            {
                if (!right.controller.triggerPressed)
                {
                    //Debug.Log("right_release");
                    if (OnParticleReleasedRight != null)
                    {
                        // Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepthRight));
                        Vector3 worldPosition = new Vector3(righthit.point.x, righthit.point.y, pickedParticleDepthRight);
                        OnParticleReleasedRight(this, new ParticlePickEventArgs(pickedParticleIndexRight, worldPosition));
                    }

                    pickedParticleIndexRight = -1;

                }
            }

        }
    }
}
