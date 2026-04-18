using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LookAroundAction ", story: "[Self] looks around for [Duration] seconds", category: "Action", id: "1d7dd66ac6f5297fa543a1433d1079a8")]
public partial class LookAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private float timer;
    private Quaternion startRotation;

    private UnityEngine.AI.NavMeshAgent agent;
    private bool originalUpdateRotation;

    [SerializeField] private float angleAmplitude = 45f;
    [SerializeField] private float oscillationSpeed = 3f;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("LookAroundAction: Self es null.");
            return Status.Failure;
        }

        timer = 0f;
        startRotation = Self.Value.transform.rotation;

        agent = Self.Value.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            originalUpdateRotation = agent.updateRotation;
            agent.updateRotation = false;

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
            return Status.Failure;

        timer += Time.deltaTime;

        float angle = Mathf.Sin(timer * oscillationSpeed) * angleAmplitude;
        Self.Value.transform.rotation = startRotation * Quaternion.Euler(0f, angle, 0f);

        if (timer >= Duration.Value)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (Self.Value != null)
        {
            Self.Value.transform.rotation = startRotation;
        }

        if (agent != null)
        {
            agent.updateRotation = originalUpdateRotation;

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }
}

