using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchingTargetGuardianAction", story: "[Self] is searching for [Target]", category: "Action", id: "82955b33282083ebd1721a197132e889")]
public partial class SearchingTargetGuardianAction : Action
{
    [SerializeReference] public BlackboardVariable<SensorSystemGuardian> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
        {
            Debug.LogWarning("SearchingTargetGuardianAction: Self no tiene asignado un SensorSystemGuardian.");
            return Status.Failure;
        }

        GameObject possibleTarget = Self.Value.SearchingTargetGuardian();

        if (Target.Value == null && possibleTarget != null)
        {
            Target.SetValueWithoutNotify(possibleTarget);
            return Status.Success;
        }

        if (possibleTarget == null && Target.Value != null)
        {
            LastKnownPosition.Value = Target.Value.transform.position;
            Target.Value = null;
        }

        return Status.Running;
    }
}

