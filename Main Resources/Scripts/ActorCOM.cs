using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEditor;
[RequireComponent(typeof(ObiActor))]
public class ActorCOM : MonoBehaviour
{

    ObiActor actor;

    void Awake()
    {

        actor = GetComponent<ObiActor>();
    }

    void OnEnable()
    {
        if (actor.Solver != null)
            actor.Solver.RequireRenderablePositions();
    }

    void OnDisable()
    {
        if (actor.Solver != null)
            actor.Solver.RelinquishRenderablePositions();
    }

    void OnDrawGizmos()
    {

        if (actor == null || !actor.InSolver)
            return;

        Gizmos.color = Color.red;
        Vector3 com = Vector3.zero;
        float massAccumulator = 0;

        // To iterate over all particles in an actor, you can use the length of any property array.
        // They are all the same length. In this case, we use the invMasses array.
        for (int i = 0; i < actor.invMasses.Length; ++i)
        {

            if (actor.invMasses[i] > 0)
            {
                massAccumulator += 1.0f / actor.invMasses[i];
                com += actor.GetParticlePosition(i) / actor.invMasses[i];
            }

        }

        com /= massAccumulator;
        Gizmos.DrawWireSphere(com, 0.1f);
    }
}