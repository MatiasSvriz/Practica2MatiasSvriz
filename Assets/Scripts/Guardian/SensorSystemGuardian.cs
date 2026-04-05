using UnityEngine;

public class SensorSystemGuardian : MonoBehaviour
{
    [field: SerializeField] public float SensorRadius { get; private set; }
    [field: SerializeField] public float SensorAngle { get; private set; } // 90º
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatIsObstacle;
    
    void Start()
    {
        
    }

    private void FixedUpdate()
    { 
        //SearchingTarget();
    }
    
    public GameObject SearchingTarget()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, SensorRadius, whatIsTarget);
        
        Debug.Log("Targets en radio: " + results.Length);
        
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
                    Debug.Log("Guardian ha visto a " + results[0].name);
                    return results[0].gameObject;
                }

            }
        }
        
        return null;
    }
}
