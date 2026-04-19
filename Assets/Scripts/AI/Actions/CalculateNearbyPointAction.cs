using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateNearbyPointAction", story: "[Self] calculates [Nearbypoint] around [Point]", category: "Action", id: "647f6e72404e4bfc5b4e89ac5bfdfdde")]
public partial class CalculateNearbyPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> Nearbypoint;
    [SerializeReference] public BlackboardVariable<Vector3> Point;
    [SerializeReference] public BlackboardVariable<float> SearchingRadius;

    protected override Status OnStart()
    {
        Vector3 randomPoint;
        do
        { 
            randomPoint = Point.Value + Random.insideUnitSphere * SearchingRadius;
        } while (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out UnityEngine.AI.NavMeshHit hit, 0f, UnityEngine.AI.NavMesh.AllAreas));

        // Comunico al árbol el punto encontrado
        Nearbypoint.Value = randomPoint;
        return Status.Success;
    }

}

