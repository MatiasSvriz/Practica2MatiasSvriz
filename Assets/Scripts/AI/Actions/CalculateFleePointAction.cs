using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateFleePointAction", story: "[Self] calculates flee point into [FleePoint]", category: "Action", id: "eac1ba4ac51307a83a6ed1475375da8e")]
public partial class CalculateFleePointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> FleePoint;
    [SerializeReference] public BlackboardVariable<float> FleeDistance;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("Self es null");
            return Status.Failure;
        }

        Vector3 selfPos = Self.Value.transform.position;

        // Dirección contraria a donde mira
        Vector3 fleeDir = -Self.Value.transform.forward;

        // Opcional: mantenerlo plano
        fleeDir.y = 0f;
        fleeDir.Normalize();

        // Punto objetivo
        Vector3 targetPoint = selfPos + fleeDir * FleeDistance.Value;

        // Ajustarlo al NavMesh
        UnityEngine.AI.NavMeshHit hit;

        if (UnityEngine.AI.NavMesh.SamplePosition(targetPoint, out hit, 3f, UnityEngine.AI.NavMesh.AllAreas))
        {
            FleePoint.SetValueWithoutNotify(hit.position);

            Debug.Log("Flee simple hacia: " + hit.position);
            return Status.Success;
        }

        Debug.LogWarning("No se pudo proyectar en NavMesh");
        return Status.Failure;
    }
}

