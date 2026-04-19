using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsDanger { get; private set; }
    public GameObject CurrentEnemy { get; private set; }
    public int InteractionCount { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SetDanger(GameObject enemy)
    {
        IsDanger = true;
        CurrentEnemy = enemy;
    }

    public void ClearDanger()
    {
        IsDanger = false;
        CurrentEnemy = null;
    }
    
    public void RegisterInteraction()
    {
        InteractionCount++;
        Debug.Log("Interacciones con el enemigo: " + InteractionCount);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateInteractionText(InteractionCount);
        }
    }
    
}
