using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnterChaseAction ", story: "[Self] sets global danger", category: "Action", id: "9ae163575f5a7f39f07143520488cc3a")]
public partial class EnterChaseActionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("SetDangerAction: Self es null.");
            return Status.Failure;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("SetDangerAction: No hay GameManager.");
            return Status.Failure;
        }

        GameManager.Instance.SetDanger(Self.Value);

        Debug.Log("SetDangerAction ejecutada por " + Self.Value.name);

        return Status.Success;
    }
}

