using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FleeFromEnemyAction", story: "[Self] flees from [Enemy] while [Danger] is true", category: "Action", id: "ebb9f82c8d220143ffc1fe9d7bb304b8")]
public partial class FleeFromEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<bool> Danger;

    [SerializeReference] public BlackboardVariable<float> FleeDistance;
    [SerializeReference] public BlackboardVariable<float> FleeSpeed;

    private NavMeshAgent agent;
    private float originalSpeed;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("FleeFromEnemyAction: Self es null.");
            return Status.Failure;
        }

        if (Enemy.Value == null)
        {
            Debug.LogError("FleeFromEnemyAction: Enemy es null.");
            return Status.Failure;
        }

        agent = Self.Value.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("FleeFromEnemyAction: No hay NavMeshAgent en " + Self.Value.name);
            return Status.Failure;
        }

        if (!agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogError("FleeFromEnemyAction: El agente no está sobre la NavMesh.");
            return Status.Failure;
        }

        originalSpeed = agent.speed;
        agent.speed = FleeSpeed.Value;
        agent.isStopped = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return Status.Failure;

        if (Self.Value == null || Enemy.Value == null)
            return Status.Failure;

        // Si ya no hay peligro, terminamos y dejamos que el árbol vuelva a Calm
        if (!Danger.Value)
            return Status.Success;

        Vector3 selfPos = Self.Value.transform.position;
        Vector3 enemyPos = Enemy.Value.transform.position;

        // Dirección contraria al enemigo
        Vector3 fleeDir = (selfPos - enemyPos).normalized;

        // Caso raro: están exactamente en la misma posición
        if (fleeDir == Vector3.zero)
            fleeDir = Self.Value.transform.forward;

        Vector3 rawFleePoint = selfPos + fleeDir * FleeDistance.Value;

        // Ajustamos el punto a la NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawFleePoint, out hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Si no encuentra un punto válido cerca, usa el bruto
            agent.SetDestination(rawFleePoint);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
                agent.isStopped = true;
            }

            agent.speed = originalSpeed;
        }
    }
}

