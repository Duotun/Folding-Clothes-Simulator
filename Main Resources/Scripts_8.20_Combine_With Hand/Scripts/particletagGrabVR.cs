using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
[RequireComponent(typeof(grabVR))]
public class particletagGrabVR : MonoBehaviour
{
    public float springStiffness = 5f;
    public float springDamping = 5f;

    private grabVR picker;
    private grabVR.ParticlePickEventArgs pickArgs;

    void OnEnable()
    {
        picker = GetComponent<grabVR>();
        picker.OnParticlePicked += Picker_OnParticleDragged;
        picker.OnParticleDragged += Picker_OnParticleDragged;
        picker.OnParticleReleased += Picker_OnParticleReleased;
    }

    void OnDisable()
    {
        picker.OnParticlePicked -= Picker_OnParticleDragged;
        picker.OnParticleDragged -= Picker_OnParticleDragged;
        picker.OnParticleReleased -= Picker_OnParticleReleased;
    }

    void FixedUpdate()
    {
        if (pickArgs != null)
        {

            ObiSolver solver = picker.Cloth.Solver;

            // Calculate picking position in solver space:
            Vector3 targetPosition = pickArgs.worldPosition;
            if (solver.simulateInLocalSpace)
                targetPosition = solver.transform.InverseTransformPoint(targetPosition);

            // Get particle position and velocity:
            Vector4[] positions = new Vector4[1];
            Vector4[] velocities = new Vector4[1];
            int solverIndex = picker.Cloth.particleIndices[pickArgs.particleIndex];
            Oni.GetParticlePositions(solver.OniSolver, positions, 1, solverIndex);
            Oni.GetParticleVelocities(solver.OniSolver, velocities, 1, solverIndex);

            // Calculate effective inverse mass:
            float invMass = picker.Cloth.invMasses[pickArgs.particleIndex] * picker.Cloth.areaContribution[pickArgs.particleIndex];

            if (invMass > 0)
            {
                // Calculate and apply spring force:
                Vector4 force = ((new Vector4(targetPosition[0], targetPosition[1], targetPosition[2], 0) - positions[0]) * springStiffness - velocities[0] * springDamping) / invMass;
                Oni.AddParticleExternalForce(picker.Cloth.Solver.OniSolver, ref force, new int[] { solverIndex }, 1);
            }

        }
    }

    void Picker_OnParticleDragged(object sender, grabVR.ParticlePickEventArgs e)
    {
        Debug.Log("kuku2");
        pickArgs = e;
    }

    void Picker_OnParticleReleased(object sender, grabVR.ParticlePickEventArgs e)
    {
        pickArgs = null;
    }

}