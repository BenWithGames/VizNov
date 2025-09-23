using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeSprite : MonoBehaviour
{
    private SpriteRenderer sr;
    private Coroutine fadeRoutine;

    [Header("Fade Setting")]
    [Tooltip("Time (in seconds) it takes to fully fade in or out.")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>Fade sprite to fully visible (alpha = 1).</summary>
    public void FadeIn()
    {
        StartFade(1f);
    }

    /// <summary>Fade sprite to fully invisible (alpha = 0).</summary>
    public void FadeOut()
    {
        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTo(targetAlpha, fadeDuration));
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = sr.color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);

            Color c = sr.color;
            c.a = newAlpha;
            sr.color = c;

            yield return null;
        }

        // snap exactly to target
        Color final = sr.color;
        final.a = targetAlpha;
        sr.color = final;

        fadeRoutine = null;
    }
}
 