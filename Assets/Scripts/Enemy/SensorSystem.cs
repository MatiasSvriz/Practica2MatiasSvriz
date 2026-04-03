using System;
using UnityEngine;

public class SensorSystem : MonoBehaviour
{

    [field: SerializeField] public float SensorRadius { get; private set; }
    [field: SerializeField] public float SensorAngle { get; private set; } // 90º
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatIsObstacle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    private void Update()
    {
        Debug.Log("Enemy Update funcionando");
    }

    private void FixedUpdate()
    { 
        SearchingTarget();
    }

    public GameObject SearchingTarget()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, SensorRadius, whatIsTarget);

        // 1. Mira a ver si hay objetivo en la zona
        if(results.Length > 0) // Al menos hay un objetivo cerca de mi zona
        {
            Vector3 directionToTarget = results[0].transform.position - transform.position;
            
            // 2. Mira a ver si está dentro de mi ángulo
            if (Vector3.Angle(transform.forward, directionToTarget) <= SensorAngle / 2)
            {
                // 3. Mira a ver si no hay obstaculos entre tú y yo.
                if (!Physics.Raycast(transform.position, directionToTarget,
                        directionToTarget.magnitude, whatIsObstacle))
                {
                    // Debug.Log("He visto a " + results[0].name);
                    return results[0].gameObject;
                }

            }
        }
        
        return null;
    }

    public Vector3 DirFromAngle(float angle, bool relativeToSelf)
    {

        if (relativeToSelf)
        {
            angle += transform.eulerAngles.y;
        }
        
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
