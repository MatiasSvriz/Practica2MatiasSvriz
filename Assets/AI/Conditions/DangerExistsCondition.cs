using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DangerExistsCondition", story: "Global danger is true", category: "Conditions", id: "837302a7287075c243c54a6be3eed87e")]
public partial class DangerExistsCondition : Condition
{
    public override bool IsTrue()
    {
        if (GameManager.Instance == null)
            return false;

        return GameManager.Instance.IsDanger;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
