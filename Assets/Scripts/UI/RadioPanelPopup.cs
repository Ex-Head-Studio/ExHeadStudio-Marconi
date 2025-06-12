using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RadioPanelPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public GameObject panel;
    public float fadeDuration = 0.3f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    public void ShowPopup()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeCanvasGroup(1f));
    }

    public void HidePopup()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeCanvasGroup(0f));
    }

    private IEnumerator FadeCanvasGroup(float tagetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, tagetAlpha, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = tagetAlpha;
    }
}
