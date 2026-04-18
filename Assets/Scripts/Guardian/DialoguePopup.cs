using System.Collections;
using TMPro;
using UnityEngine;

public class DialoguePopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;   // La burbuja
    [SerializeField] private TMP_Text popupText;     // El texto
    [SerializeField] private float defaultDuration = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (popupRoot != null)
            popupRoot.SetActive(false);
    }

    public void ShowText(string message)
    {
        ShowText(message, defaultDuration);
    }

    public void ShowText(string message, float duration)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowRoutine(message, duration));
    }

    private IEnumerator ShowRoutine(string message, float duration)
    {
        popupText.text = message;
        popupRoot.SetActive(true);

        yield return new WaitForSeconds(duration);

        popupRoot.SetActive(false);
        currentRoutine = null;
    }
}