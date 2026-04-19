using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntilTargetLostAction", story: "[Self] waits until [Target] is lost", category: "Action", id: "b3ea74678a3ebac354eab9813d6722cd")]
public partial class WaitUntilTargetLostAction : Action
{
    [SerializeReference] public BlackboardVariable<SensorSystemGuardian> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
        {
            Debug.LogWarning("WaitUntilTargetLostAction: Self no tiene SensorSystemGuardian.");
            return Status.Failure;
        }

        GameObject possibleTarget = Self.Value.SearchingTargetGuardian();

        // Si todavía ve al enemigo, seguimos protegiendo
        if (possibleTarget != null)
        {
            Target.Value = possibleTarget;
            return Status.Running;
        }

        // Si ya no lo ve, guardamos última posición y limpiamos Target
        if (Target.Value != null)
        {
            LastKnownPosition.Value = Target.Value.transform.position;
            Target.Value = null;
        }

        return Status.Success;
    }
}

