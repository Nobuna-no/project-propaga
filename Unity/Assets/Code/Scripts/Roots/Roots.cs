using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Roots : MonoBehaviour
{
    [SerializeField]
    private Transform origin;
    [SerializeField]
    private Transform dest;
    [SerializeField]
    SplineContainer splineContainer;
    [SerializeField]
    SplineExtrude splineExtrude;
    [SerializeField, MinMaxSlider(0,1)]
    Vector2 m_radiusExtension = Vector2.one * 0.1f;
    [SerializeField]
    float duration = 1.0f;
    private bool m_hasAnimationStarted = false;
    private float m_duration = 0.0f;

    private void Awake()
    {
        splineExtrude.enabled = false;
        splineExtrude.Range = Vector2.zero;
        splineExtrude.Radius = 0;

        for (int i = splineContainer.Splines.Count - 1; i >= 0; i--)
        {
            splineContainer.RemoveSplineAt(i);
        }

        var spline = new Spline();
        spline.Add(new BezierKnot(origin.position, 2, 2), TangentMode.Continuous);
        spline.Add(new BezierKnot(dest.position), TangentMode.Continuous);
        splineContainer.AddSpline(spline);
    }

    [Button]
    // Start is called before the first frame update
    void DoTHings()
    {
        if (m_hasAnimationStarted)
        {
            return;
        }

        m_duration = 0;
        splineExtrude.enabled = true;
        m_hasAnimationStarted = true;
        splineExtrude.Range = Vector2.zero;
        splineExtrude.Radius = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_hasAnimationStarted)
        {
            return;
        }

        splineExtrude.Range = new Vector2(0, Mathf.Lerp(0, 1, m_duration));
        splineExtrude.Radius = Mathf.Lerp(m_radiusExtension.x, m_radiusExtension.y, m_duration);
      {
            m_hasAnimationStarted = false;
        }
    }
}
