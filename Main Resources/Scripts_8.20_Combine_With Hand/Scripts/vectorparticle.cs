using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obi
{
    public class vectorparticle : MonoBehaviour
    {

        public class ParticleVectorArgs : EventArgs
        {

            public int particleIndex;
            public Vector3 worldPosition;

            public ParticleVectorArgs(int particleIndex, Vector3 worldPosition)
            { //new method
                this.particleIndex = particleIndex;
                this.worldPosition = worldPosition;
            }
        }

        public event System.EventHandler<ParticleVectorArgs> OnParticlePickedaxis1;
        public event System.EventHandler<ParticleVectorArgs> OnParticlePickedaxis2;
        public event System.EventHandler<ParticleVectorArgs> OnParticlePickedrotation1;
        public event System.EventHandler<ParticleVectorArgs> OnParticlePickedrotation2;
        // public event System.EventHandler<ParticleVectorArgs> OnParticleHeld;
        //public event System.EventHandler<ParticleVectorArgs> OnParticleDragged;
        public event System.EventHandler<ParticleVectorArgs> OnParticleReleased;

        private MeshCollider meshCollider;
        private ObiClothBase cloth;
        private Mesh currentCollisionMesh;
        public static int fournumberint=0;
        private Vector3 lastMousePos = Vector3.zero;
        private int pickedParticleIndex = -1;
        private float pickedParticleDepth = 0;
        int[] coordinates;
        public ObiClothBase Cloth
        {
            get { return cloth; }
        }

        void Awake()
        {
            cloth = GetComponent<ObiClothBase>();
            //lastMousePos = Input.mousePosition;
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
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                fournumberint = 1;
                coordinates = TCPTestClient.numberint;
            }
        }
        void Cloth_Solver_OnFrameBegin(object sender, EventArgs e)
        {
            if (meshCollider == null)
                return;

            //get message
            if (fournumberint >= 1)
            {
                switch (fournumberint)
                {
                    case 0: break;
                    case 1: getindex1(); fournumberint++; break;
                    case 2: getindex2(); fournumberint++; break;
                    case 3: getindex3(); fournumberint++; break;
                    case 4: getindex4(); fournumberint++; break;
                    case 5: fournumberint = 0; break;
                }
            }
            /*else if (pickedParticleIndex >= 0)
            {
                // Release:				
                if (Input.GetKeyUp(KeyCode.S))
                {

                    if (OnParticleReleased != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticleReleased(this, new ParticleVectorArgs(pickedParticleIndex, worldPosition));
                    }

                    pickedParticleIndex = -1;

                }
            }
            */

            //lastMousePos = Input.mousePosition;    may be useful
        }
    void getindex1()
        {
            meshCollider.enabled = true;

            GameObject.Destroy(currentCollisionMesh);
            currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
            meshCollider.sharedMesh = currentCollisionMesh;
            //change here
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
           // Debug.Log(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
            RaycastHit hitInfo;
            if (meshCollider.Raycast(ray, out hitInfo, 100))
            {

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
                        closestVertex = vertex;   //update closest vertex
                    }
                }

                // get particle index:
                if (closestVertex >= 0 && closestVertex < cloth.topology.visualMap.Length)
                {

                    pickedParticleIndex = cloth.topology.visualMap[closestVertex];
                    pickedParticleDepth = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                    if (OnParticlePickedaxis1 != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticlePickedaxis1(this, new ParticleVectorArgs(pickedParticleIndex, worldPosition));
                    }
                }
            }

            meshCollider.enabled = false;
        }
        void getindex2()
        {
            meshCollider.enabled = true;

            GameObject.Destroy(currentCollisionMesh);
            currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
            meshCollider.sharedMesh = currentCollisionMesh;
            //change here
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
           // Debug.Log(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
            RaycastHit hitInfo;
            if (meshCollider.Raycast(ray, out hitInfo, 100))
            {

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
                        closestVertex = vertex;   //update closest vertex
                    }
                }

                // get particle index:
                if (closestVertex >= 0 && closestVertex < cloth.topology.visualMap.Length)
                {

                    pickedParticleIndex = cloth.topology.visualMap[closestVertex];
                    pickedParticleDepth = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                    if (OnParticlePickedaxis2 != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticlePickedaxis2(this, new ParticleVectorArgs(pickedParticleIndex, worldPosition));
                    }
                }
            }

            meshCollider.enabled = false;
        }
        void getindex3()
        {
            meshCollider.enabled = true;

            GameObject.Destroy(currentCollisionMesh);
            currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
            meshCollider.sharedMesh = currentCollisionMesh;
            //change here
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
            //Debug.Log(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
            RaycastHit hitInfo;
            if (meshCollider.Raycast(ray, out hitInfo, 100))
            {

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
                        closestVertex = vertex;   //update closest vertex
                    }
                }

                // get particle index:
                if (closestVertex >= 0 && closestVertex < cloth.topology.visualMap.Length)
                {

                    pickedParticleIndex = cloth.topology.visualMap[closestVertex];
                    pickedParticleDepth = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                    if (OnParticlePickedrotation1 != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticlePickedrotation1(this, new ParticleVectorArgs(pickedParticleIndex, worldPosition));
                    }
                }
            }

            meshCollider.enabled = false;
        }
        void getindex4()
        {
            meshCollider.enabled = true;

            GameObject.Destroy(currentCollisionMesh);
            currentCollisionMesh = GameObject.Instantiate(cloth.clothMesh);
            meshCollider.sharedMesh = currentCollisionMesh;
            //change here
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
            //Debug.Log(new Vector3(coordinates[2 * (fournumberint - 1)], coordinates[2 * (fournumberint - 1) + 1]));
            RaycastHit hitInfo;
            if (meshCollider.Raycast(ray, out hitInfo, 100))
            {

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
                        closestVertex = vertex;   //update closest vertex
                    }
                }

                // get particle index:
                if (closestVertex >= 0 && closestVertex < cloth.topology.visualMap.Length)
                {

                    pickedParticleIndex = cloth.topology.visualMap[closestVertex];
                    pickedParticleDepth = Mathf.Abs((cloth.transform.TransformPoint(vertices[closestVertex]) - Camera.main.transform.position).z);

                    if (OnParticlePickedrotation2 != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticlePickedrotation2(this, new ParticleVectorArgs(pickedParticleIndex, worldPosition));
                    }
                }
            }

            meshCollider.enabled = false;
        }
    }
}
