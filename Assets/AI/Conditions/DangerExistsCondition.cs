using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DangerExistsCondition", story: "[Danger] is true", category: "Conditions", id: "837302a7287075c243c54a6be3eed87e")]
public partial class DangerExistsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> Danger;

    public override bool IsTrue()
    {
        return Danger.Value;
    }
}
