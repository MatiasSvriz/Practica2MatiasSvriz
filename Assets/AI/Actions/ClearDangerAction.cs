using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ClearDangerAction", story: "[Self] clears global danger", category: "Action", id: "69c2b344e3d89b31618bedcb4b9c103a")]
public partial class ClearDangerAction : Action
{
    protected override Status OnStart()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("ClearDangerAction: No hay GameManager.");
            return Status.Failure;
        }

        GameManager.Instance.ClearDanger();

        Debug.Log("Danger desactivado");

        return Status.Success;
    }
}

