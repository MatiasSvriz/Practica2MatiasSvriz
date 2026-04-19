using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text interactionText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UpdateInteractionText(GameManager.Instance.InteractionCount);
        }
    }

    public void UpdateInteractionText(int interactionCount)
    {
        if (interactionText == null)
        {
            Debug.LogWarning("UIManager: interactionText no está asignado.");
            return;
        }

        interactionText.text = "Interacciones con enemigo: " + interactionCount;
    }
}