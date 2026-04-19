using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "LookAroundAction",
    story: "[Self] looks around for [Duration] seconds",
    category: "Action",
    id: "1d7dd66ac6f5297fa543a1433d1079a8"
)]
public partial class LookAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Duration;

    [SerializeField] private float angleAmplitude = 60f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float pauseTime = 0.35f;

    private float totalTimer;
    private float stateTimer;

    private Quaternion centerRotation;
    private Quaternion leftRotation;
    private Quaternion rightRotation;

    private NavMeshAgent agent;
    private bool originalUpdateRotation;
    private bool originalIsStopped;

    private enum LookState
    {
        RotateLeft,
        PauseLeft,
        RotateRight,
        PauseRight,
        RotateCenter,
        PauseCenter
    }

    private LookState currentState;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("LookAroundAction: Self es null.");
            return Status.Failure;
        }

        totalTimer = 0f;
        stateTimer = 0f;

        centerRotation = Self.Value.transform.rotation;
        leftRotation = centerRotation * Quaternion.Euler(0f, -angleAmplitude, 0f);
        rightRotation = centerRotation * Quaternion.Euler(0f, angleAmplitude, 0f);

        currentState = LookState.RotateLeft;

        agent = Self.Value.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            originalUpdateRotation = agent.updateRotation;
            originalIsStopped = agent.isStopped;

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

        totalTimer += Time.deltaTime;
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case LookState.RotateLeft:
                RotateTowards(leftRotation);
                if (HasReachedRotation(leftRotation))
                    ChangeState(LookState.PauseLeft);
                break;

            case LookState.PauseLeft:
                if (stateTimer >= pauseTime)
                    ChangeState(LookState.RotateRight);
                break;

            case LookState.RotateRight:
                RotateTowards(rightRotation);
                if (HasReachedRotation(rightRotation))
                    ChangeState(LookState.PauseRight);
                break;

            case LookState.PauseRight:
                if (stateTimer >= pauseTime)
                    ChangeState(LookState.RotateCenter);
                break;

            case LookState.RotateCenter:
                RotateTowards(centerRotation);
                if (HasReachedRotation(centerRotation))
                    ChangeState(LookState.PauseCenter);
                break;

            case LookState.PauseCenter:
                if (stateTimer >= pauseTime)
                    ChangeState(LookState.RotateLeft);
                break;
        }

        if (totalTimer >= Duration.Value)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (Self.Value != null)
        {
            Self.Value.transform.rotation = centerRotation;
        }

        if (agent != null)
        {
            agent.updateRotation = originalUpdateRotation;

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = originalIsStopped;
            }
        }
    }

    private void RotateTowards(Quaternion targetRotation)
    {
        Self.Value.transform.rotation = Quaternion.RotateTowards(
            Self.Value.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private bool HasReachedRotation(Quaternion targetRotation)
    {
        float angle = Quaternion.Angle(Self.Value.transform.rotation, targetRotation);
        return angle < 1f;
    }

    private void ChangeState(LookState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }
}