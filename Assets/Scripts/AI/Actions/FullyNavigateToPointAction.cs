using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FullyNavigateToPointAction", story: "[Self] fully navigates to [Destination]", category: "Action", id: "3c17c86cc4ab1a52452077eba2c16337")]
public partial class FullyNavigateToPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> Destination;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;

    private NavMeshAgent agent;
    private float originalSpeed;

    private Vector3 finalDestination;
    private float timer;
    private float startDistance;

    private bool hasStartedMoving;
    private bool destinationWasSet;

    // Ajustes finos
    private const float arriveThreshold = 0.2f;
    private const float movementThreshold = 0.05f;
    private const float minRunTimeBeforeSuccess = 0.2f;
    private const float maxTimeWithoutMoving = 1.0f;

    protected override Status OnStart()
    {
        timer = 0f;
        hasStartedMoving = false;
        destinationWasSet = false;

        if (Self.Value == null)
        {
            Debug.LogError("FullyNavigateToPointAction: Self es null.");
            return Status.Failure;
        }

        agent = Self.Value.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("FullyNavigateToPointAction: No hay NavMeshAgent en " + Self.Value.name);
            return Status.Failure;
        }

        if (!agent.enabled)
        {
            Debug.LogError("FullyNavigateToPointAction: NavMeshAgent desactivado.");
            return Status.Failure;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("FullyNavigateToPointAction: El agente no está sobre la NavMesh.");
            return Status.Failure;
        }

        originalSpeed = agent.speed;
        agent.speed = MoveSpeed.Value;
        agent.isStopped = false;

        // Ajustamos el destino a una posición válida de NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(Destination.Value, out hit, 4f, NavMesh.AllAreas))
        {
            finalDestination = hit.position;
        }
        else
        {
            finalDestination = Destination.Value;
            Debug.LogWarning("FullyNavigateToPointAction: No se pudo ajustar el destino a NavMesh. Uso destino bruto.");
        }

        startDistance = Vector3.Distance(
            Flatten(Self.Value.transform.position),
            Flatten(finalDestination)
        );

        // Si el destino está absurdamente cerca, no queremos success instantáneo
        if (startDistance <= agent.stoppingDistance + arriveThreshold)
        {
            Debug.LogWarning("FullyNavigateToPointAction: destino demasiado cerca. No se considera navegación útil.");
            return Status.Failure;
        }

        destinationWasSet = agent.SetDestination(finalDestination);

        if (!destinationWasSet)
        {
            Debug.LogError("FullyNavigateToPointAction: SetDestination ha fallado.");
            return Status.Failure;
        }

        Debug.Log("FullyNavigateToPointAction: navegando a " + finalDestination + " | distancia inicial = " + startDistance);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null || !agent.enabled)
            return Status.Failure;

        if (!agent.isOnNavMesh)
            return Status.Failure;

        timer += Time.deltaTime;

        if (agent.pathPending)
            return Status.Running;

        // Si la ruta es inválida, fallamos
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("FullyNavigateToPointAction: ruta inválida.");
            return Status.Failure;
        }

        float currentDistance = Vector3.Distance(
            Flatten(Self.Value.transform.position),
            Flatten(finalDestination)
        );

        // Consideramos que ha empezado a moverse si su velocidad supera un umbral
        if (agent.velocity.magnitude > movementThreshold)
        {
            hasStartedMoving = true;
        }

        // Si tras un tiempo razonable no se ha movido nada, fallamos
        if (!hasStartedMoving && timer > maxTimeWithoutMoving)
        {
            Debug.LogError("FullyNavigateToPointAction: el agente no ha empezado a moverse.");
            return Status.Failure;
        }

        // Nunca devolvemos success demasiado pronto
        if (timer < minRunTimeBeforeSuccess)
            return Status.Running;

        // Éxito solo si:
        // 1. ha comenzado a moverse alguna vez
        // 2. está suficientemente cerca
        // 3. está prácticamente parado
        if (hasStartedMoving &&
            currentDistance <= agent.stoppingDistance + arriveThreshold &&
            agent.velocity.sqrMagnitude < 0.01f)
        {
            Debug.Log("FullyNavigateToPointAction: destino alcanzado.");
            return Status.Success;
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

    private Vector3 Flatten(Vector3 v)
    {
        v.y = 0f;
        return v;
    }
}

