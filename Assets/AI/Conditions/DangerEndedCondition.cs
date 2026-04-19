using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DangerEndedCondition", story: "Global danger is false", category: "Conditions", id: "c1d50569d4ee737558ec79a1ad71084e")]
public partial class DangerEndedCondition : Condition
{
    public override bool IsTrue()
    {
        if (GameManager.Instance == null)
            return true;

        return !GameManager.Instance.IsDanger;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
