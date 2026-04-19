using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntilTargetLostEnemyAction", story: "[Self] waits until [Target] is lost", category: "Action", id: "f054cb765b55c37072bfb10d1ca8de03")]
public partial class WaitUntilTargetLostEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<SensorSystem> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
        {
            Debug.LogWarning("WaitUntilTargetLostAction: Self no tiene SensorSystemGuardian.");
            return Status.Failure;
        }

        GameObject possibleTarget = Self.Value.SearchingTarget();

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

