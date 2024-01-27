using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DitherFeedback : MonoBehaviour
{
    [SerializeField]
    List<Renderer> renderers = new List<Renderer>();

    [SerializeField, Range(0.0f, 1.0f)]
    float maxOpacity = 1.0f;

    [SerializeField, Range(0.0f, 1.0f)]
    float minOpacity = 0.0f;

    float currentOpacity = 1.0f;

    [SerializeField]
    float duration = 1.0f;

    private int opacityProperty = Shader.PropertyToID("_Opacity");

    private bool obscuringAny = false;
    private HashSet<Object> obscuringObjects = new HashSet<Object>();
    private Coroutine fadeInProgress = null;

    public bool IsObscuring
    {
        get => obscuringAny;
        private set 
        {
            if (obscuringAny == value)
                return;

            obscuringAny = value;
            Fade(!obscuringAny);
        }
    }

    public void SetObscuring(Object obscuredObject, bool obscure)
    {
        if (obscure)
        {
            obscuringObjects.Add(obscuredObject);
        }
        else
        {
            obscuringObjects.Remove(obscuredObject);
        }

        IsObscuring = obscuringObjects.Count > 0;
    }

    private void OnEnable()
    {
        currentOpacity = maxOpacity;
        UpdateOpacity();
    }

    public void Fade(bool visible)
    {
        if (fadeInProgress != null)
            StopCoroutine(fadeInProgress);

        fadeInProgress = StartCoroutine(Dither(currentOpacity, visible ? maxOpacity : minOpacity));
    }
    
    private IEnumerator Dither(float start, float end)
    {
        if (start == end)
        {
            fadeInProgress = null;
            yield break;
        }

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / duration;
            currentOpacity = Mathf.Lerp(start, end, t);
            UpdateOpacity();
            yield return null;
        }
    
        currentOpacity = end;
        UpdateOpacity();

        fadeInProgress = null;
    }

    private void UpdateOpacity()
    {
        if (renderers == null || renderers.Count == 0)
            return;

        for (int i = 0 ; i < renderers.Count ; ++i)
        {
            renderers[i].material.SetFloat(opacityProperty, currentOpacity);
        }
    }
}