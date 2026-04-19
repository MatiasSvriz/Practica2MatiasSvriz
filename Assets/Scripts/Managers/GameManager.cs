using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsDanger { get; private set; }
    public GameObject CurrentEnemy { get; private set; }

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
}
