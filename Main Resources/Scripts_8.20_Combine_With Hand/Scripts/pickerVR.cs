using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Obi;
using VRTK;
namespace Obi
{
    public class pickerVR : MonoBehaviour
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

        public event System.EventHandler<ParticlePickEventArgs> OnParticlePicked;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleHeld;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleDragged;
        public event System.EventHandler<ParticlePickEventArgs> OnParticleReleased;

        private MeshCollider meshCollider;
        private ObiClothBase cloth;
        private Mesh currentCollisionMesh;

        private Vector3 lastMousePos = Vector3.zero;
        private int pickedParticleIndex = -1;
        private float pickedParticleDepth = 0;
        private Vector3 hitPoint;

        public ObiClothBase Cloth
        {
            get { return cloth; }
        }

        void Awake()
        {
            //hitPoint = pointer.hitPoint;
            cloth = GetComponent<ObiClothBase>();
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

        void Cloth_Solver_OnFrameBegin(object sender, EventArgs e)
        {
            if (meshCollider == null)
                return;
            RaycastHit hitInfo= VRTK_Pointer.rayhit;
            // Click
            if (VRTK_StraightPointerRenderer.hitflag==true&&VRTK_Pointer.hitflag==true)    //can be optimised by controller_right.down || controller_left.down
            {
                //Debug.Log("??");
                VRTK_StraightPointerRenderer.hitflag = false;
                VRTK_Pointer.hitflag = false;
                //Debug.Log("ss");
                meshCollider.enabled = true;

                GameObject.Destroy(currentCollisionMesh);
                currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
                meshCollider.sharedMesh = currentCollisionMesh;
                //Vector3 hitscreen = new Vector3(pointer.hitPoint.x, pointer.hitPoint.y, 0);
                //Ray ray = new Ray(pointer.hitPoint, new Vector3(0,0,1)); //?
                //Debug.Log(meshCollider);
                hitInfo=VRTK_Pointer.rayhit;
                if (hitInfo.point!=null) //meshCollider.Raycast(ray, out hitInfo, 100)
                {
                    //Debug.Log("ps");
                    int[] tris = currentCollisionMesh.triangles;
                    Vector3[] vertices = currentCollisionMesh.vertices; // triangle multiply*3 vertices

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
                        
                        pickedParticleIndex = cloth.topology.visualMap[closestVertex];
                        pickedParticleDepth = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                        if (OnParticlePicked != null)
                        {
                            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(hitInfo.point.x, hitInfo.point.y, pickedParticleDepth));
                            OnParticlePicked(this, new ParticlePickEventArgs(pickedParticleIndex, worldPosition));
                            //Debug.Log("ps");
                        }
                    }
                }

                meshCollider.enabled = false;
               

            }
            else if (pickedParticleIndex >= 0)
            {

                // Drag:
                Vector3 mouseDelta = hitInfo.point - lastMousePos;
                if (mouseDelta.magnitude > 0.01f && OnParticleDragged != null)
                {

                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(hitInfo.point.x, hitInfo.point.y, pickedParticleDepth));
                    OnParticleDragged(this, new ParticlePickEventArgs(pickedParticleIndex, worldPosition));

                }
                else if (OnParticleHeld != null)
                {

                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(hitInfo.point.x, hitInfo.point.y, pickedParticleDepth));
                    OnParticleHeld(this, new ParticlePickEventArgs(pickedParticleIndex, worldPosition));

                }

                // Release:				
                if (VRTK_Pointer.leaveflag==true)
                {

                    VRTK_Pointer.leaveflag = false;  // haven't consider this
                    if (OnParticleReleased != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(hitInfo.point.x, hitInfo.point.y, pickedParticleDepth));
                        OnParticleReleased(this, new ParticlePickEventArgs(pickedParticleIndex, worldPosition));
                    }

                    pickedParticleIndex = -1;

                }
            }

            lastMousePos = hitInfo.point;
        }
    }
}