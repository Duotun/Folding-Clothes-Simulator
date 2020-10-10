using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiActor))]
public class VelocityVisualizer : MonoBehaviour
{

    ObiActor actor;

    void Awake()
    {
        actor = GetComponent<ObiActor>();
    }

    void OnDrawGizmos()
    {

        if (actor == null || !actor.InSolver)
            return;

        Gizmos.color = Color.black;
        Gizmos.matrix = actor.ActorLocalToWorldMatrix;

        actor.PullDataFromSolver(ParticleData.POSITIONS | ParticleData.VELOCITIES);

        for (int i = 0; i < actor.velocities.Length; ++i)
        {
            Gizmos.DrawRay(actor.positions[i], actor.velocities[i] * Time.fixedDeltaTime);
            Gizmos.DrawRay(actor.positions[i], actor.velocities[i] * Time.fixedDeltaTime);
            Gizmos.DrawRay(actor.positions[i], actor.velocities[i] * Time.fixedDeltaTime);
        }
    }
}
			
