using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchTargetAndDecideStateAction", story: "[Self] searches [target] and sets state to [ChaseState] or [PatrolState]", category: "Action", id: "9465b9ce6b7b1fbdd05e75092664e012")]
public partial class SearchTargetAndDecideStateAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<EnemyState> CurrentState;

    [SerializeReference] public BlackboardVariable<EnemyState> ChaseState;
    [SerializeReference] public BlackboardVariable<EnemyState> PatrolState;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("SearchTargetAndDecideStateAction: Self es null.");
            return Status.Failure;
        }

        SensorSystem sensor = Self.Value.GetComponent<SensorSystem>();

        if (sensor == null)
        {
            Debug.LogError("SearchTargetAndDecideStateAction: No hay SensorSystem en " + Self.Value.name);
            return Status.Failure;
        }

        GameObject possibleTarget = sensor.SearchingTarget();

        if (possibleTarget != null)
        {
            Target.SetValueWithoutNotify(possibleTarget);
            CurrentState.SetValueWithoutNotify(ChaseState.Value);

            Debug.Log(Self.Value.name + " ha encontrado a " + possibleTarget.name + " -> CHASE");
        }
        else
        {
            Target.SetValueWithoutNotify(null);
            CurrentState.SetValueWithoutNotify(PatrolState.Value);

            Debug.Log(Self.Value.name + " no ha encontrado nada -> PATROL");
        }

        return Status.Success;
    }
}

