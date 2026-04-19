using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayAnimAndWaitAction", story: "[Self] plays animation in param [ParamName]", category: "Action", id: "43b8ee99283170abdf523f1aa76a6ae7")]
public partial class PlayAnimAndWaitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<string> ParamName;

    private float timer;
    private Animator anim;
    private NavMeshAgent agent;
    
    protected override Status OnStart()
    {
        timer = 0f;
        anim = Self.Value.GetComponentInChildren<Animator>();
        agent = Self.Value.GetComponent<NavMeshAgent>();
        
        agent.isStopped = true;
        anim.SetTrigger(ParamName);
        return Status.Success; // Se lanza el trigger y espero a que se complete la animacion
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;
        
        // Si el timer supera el tiempo de la animacion...
        AnimatorStateInfo animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (timer >= animatorStateInfo.length)
        {
            agent.isStopped = false;
            return Status.Success;
        }

        return Status.Running;
    }
    
}

