using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateStrategicPointAction", story: "[Self] selects a strategic point into [InvestigatePoint]", category: "Action", id: "b6898191d95e03cc4a8ba38ed750c409")]
public partial class CalculateStrategicPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<List<GameObject>> StrategicPoints;
    [SerializeReference] public BlackboardVariable<Vector3> InvestigatePoint;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("CalculateStrategicPointAction: Self es null.");
            return Status.Failure;
        }

        if (StrategicPoints.Value == null || StrategicPoints.Value.Count == 0)
        {
            Debug.LogError("CalculateStrategicPointAction: No hay StrategicPoints.");
            return Status.Failure;
        }

        Vector3 referencePosition;

        if (Player.Value != null)
        {
            referencePosition = Player.Value.transform.position;
        }
        else
        {
            referencePosition = Self.Value.transform.position;
        }

        GameObject bestPoint = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < StrategicPoints.Value.Count; i++)
        {
            GameObject point = StrategicPoints.Value[i];

            if (point == null)
                continue;

            float dist = Vector3.Distance(referencePosition, point.transform.position);

            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestPoint = point;
            }
        }

        if (bestPoint == null)
        {
            Debug.LogError("CalculateStrategicPointAction: No se encontró ningún punto válido.");
            return Status.Failure;
        }

        InvestigatePoint.SetValueWithoutNotify(bestPoint.transform.position);

        Debug.Log("InvestigatePoint elegido: " + bestPoint.name + " en " + bestPoint.transform.position);

        return Status.Success;
    }
}

