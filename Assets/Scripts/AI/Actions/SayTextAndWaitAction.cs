using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SayTextAndWaitAction", story: "[Self] says [Message] and waits [Duration] seconds", category: "Action", id: "ccd12e064b6ee28a9eb8adfb0c015c64")]
public partial class SayTextAndWaitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<string> Message;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private float timer;
    
    protected override Status OnStart()
    {
        timer = 0f;

        if (Self.Value == null)
            return Status.Failure;

        if (!Self.Value.TryGetComponent(out DialoguePopup popup))
            return Status.Failure;
        
        Debug.Log(Self.Value.name + " dice: " + Message.Value);
        popup.ShowText(Message.Value, Duration.Value);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= Duration.Value)
            return Status.Success;

        return Status.Running;
    }
}

