using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchingTargetAction", story: "[Self] is searching for [Target]", category: "Action", id: "ef42c3e4e861b20ed8dd1d608275d0dc")]
public partial class SearchingTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<SensorSystem> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;

    protected override Status OnUpdate()
    {
        
        if (Self.Value == null)
        {
            Debug.LogError("Self.Value es null. No está asignado el SensorSystem en el Blackboard.");
            return Status.Failure;
        }
        
        GameObject possibleTarget = Self.Value.SearchingTarget();
        
        if (Target.Value == null && possibleTarget != null)
        {
            // Pongo en la variable "Target" del blackboard que este es mi objetivo
            Target.SetValueWithoutNotify(possibleTarget);
            return Status.Success;
        }
        else if(possibleTarget == null) // Si no identifico nada con el SensorSystem
        {
            if (Target.Value != null) // Tenía un objetivo
            {
                // Comunico al arbol la última posicion conocida del objetivo
                LastKnownPosition.Value = Target.Value.transform.position;
                Target.Value = null;
                return Status.Failure;
            }
        }
        
        return Status.Running;
        
    }
}

