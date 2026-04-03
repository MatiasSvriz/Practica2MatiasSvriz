using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RepeatXTimesModifier", story: "Repeat [X] Times", category: "Flow", id: "bb8811c667c7b4d0358272d8448caaf1")]
public partial class RepeatXTimesModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<int> X;
    private int counter = 0;

    protected override Status OnStart()
    {
        counter = 0;
        StartNode(Child);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var status = Child.CurrentStatus;
        
        if (status == Status.Success)
        {
            counter++;
            if (counter < X)
            {
                StartNode(Child);
            }
            else
            {
                counter = 0; // Se han terminado todas las iteraciones, reseteo counter y comunico success
                return Status.Success;
            }
        }
        return Status.Running;
    }
    
}

