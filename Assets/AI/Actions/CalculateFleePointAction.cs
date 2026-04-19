using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateFleePointAction", story: "[Self] calculates flee point into [FleePoint]", category: "Action", id: "eac1ba4ac51307a83a6ed1475375da8e")]
public partial class CalculateFleePointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> FleePoint;
    [SerializeReference] public BlackboardVariable<float> FleeDistance;

    // Distancia mínima real que queremos entre el NPC y el punto final
    private const float minValidDistance = 3.5f;

    // Cuántos intentos hacemos alejándonos más
    private const int maxTries = 5;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            Debug.LogError("CalculateFleePointAction: Self es null.");
            return Status.Failure;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("CalculateFleePointAction: GameManager.Instance es null.");
            return Status.Failure;
        }

        GameObject enemy = GameManager.Instance.CurrentEnemy;

        if (enemy == null)
        {
            Debug.LogError("CalculateFleePointAction: CurrentEnemy es null.");
            return Status.Failure;
        }

        Vector3 selfPos = Self.Value.transform.position;
        Vector3 enemyPos = enemy.transform.position;

        // Trabajamos en plano horizontal
        Vector3 flatSelf = selfPos;
        Vector3 flatEnemy = enemyPos;
        flatSelf.y = 0f;
        flatEnemy.y = 0f;

        Vector3 fleeDir = (flatSelf - flatEnemy).normalized;

        if (fleeDir == Vector3.zero)
            fleeDir = Self.Value.transform.forward;

        // Aseguramos una distancia base razonable
        float baseDistance = Mathf.Max(FleeDistance.Value, minValidDistance);

        UnityEngine.AI.NavMeshHit hit;
        bool foundValidPoint = false;
        Vector3 chosenPoint = selfPos + fleeDir * baseDistance;

        // Probamos varios puntos cada vez más lejos
        for (int i = 0; i < maxTries; i++)
        {
            float testedDistance = baseDistance + i * 2f;
            Vector3 rawCandidate = selfPos + fleeDir * testedDistance;

            if (UnityEngine.AI.NavMesh.SamplePosition(rawCandidate, out hit, 4f, UnityEngine.AI.NavMesh.AllAreas))
            {
                Vector3 flatHit = hit.position;
                flatHit.y = 0f;

                float realDistance = Vector3.Distance(flatSelf, flatHit);

                if (realDistance >= minValidDistance)
                {
                    chosenPoint = hit.position;
                    foundValidPoint = true;

                    Debug.Log("FleePoint válido encontrado en intento " + i +
                              " | punto = " + hit.position +
                              " | distancia real = " + realDistance);
                    break;
                }
                else
                {
                    Debug.LogWarning("Punto demasiado cerca tras SamplePosition. Distancia real = " + realDistance);
                }
            }
        }

        // Si no encontramos uno bueno en NavMesh, usamos un fallback lejos
        if (!foundValidPoint)
        {
            chosenPoint = selfPos + fleeDir * (baseDistance + 4f);
            Debug.LogWarning("No se encontró FleePoint válido en NavMesh. Uso fallback = " + chosenPoint);
        }

        FleePoint.SetValueWithoutNotify(chosenPoint);
        return Status.Success;
    }
}

