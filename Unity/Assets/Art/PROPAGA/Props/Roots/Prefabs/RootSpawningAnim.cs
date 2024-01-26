using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class RootSpawningAnim : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_spriteSkins;

    [SerializeField]
    private Vector3 offset = Vector3.one;

    [SerializeField]
    private Vector3 originOffset = Vector3.zero;

    [SerializeField, MinMaxSlider(0, 10)]
    private Vector2 m_durationRange = new Vector2(1, 2);

    [SerializeField]
    private float m_growthDelayByDistance = 1;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    private SplineContainer m_parentSplineContainer;
    private Vector3 splineOrigin = Vector3.zero;
    private Vector3 splineEnd = Vector3.zero;
    private Vector3 m_origin;
    private float m_currentTime = 0;
    private float m_normalizeDistanceOnThePath = 0;
    private float m_duration = 0;

    private void Awake()
    {
        if (m_spriteRenderer != null)
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        Debug.Assert(m_spriteRenderer != null, this);
        Debug.Assert(m_spriteSkins != null && m_spriteSkins.Length > 0, this);

        m_spriteRenderer.sprite = m_spriteSkins[Random.Range(0, m_spriteSkins.Length)];
        transform.position += originOffset;
    }

    private void Start()
    {
        m_currentTime = 0;
        m_duration = Random.Range(m_durationRange.x, m_durationRange.y);

        m_parentSplineContainer = GetComponentInParent<SplineContainer>();
        Debug.Assert(m_parentSplineContainer != null);

        var spline = m_parentSplineContainer.Spline;
        var beginKnot = spline.Knots.ElementAt(0);
        splineOrigin = m_parentSplineContainer.transform.position + new Vector3(beginKnot.Position.x, beginKnot.Position.y, beginKnot.Position.z);

        var endKnot = spline.Knots.Last();
        splineEnd = m_parentSplineContainer.transform.position + new Vector3(endKnot.Position.x, endKnot.Position.y, endKnot.Position.z);

        m_normalizeDistanceOnThePath = GetNormalizedPosition(transform.position, splineOrigin, splineEnd);
        m_currentTime -= m_normalizeDistanceOnThePath * m_growthDelayByDistance;

        transform.position += originOffset;
        m_origin = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_currentTime >= 0)
        {
            transform.position = Vector3.Slerp(m_origin, m_origin + offset, m_currentTime);
        }

        m_currentTime += Time.deltaTime / m_duration;
        if (m_currentTime > 1)
        {
            enabled = false;
            gameObject.isStatic = true;
        }
    }

    private static float GetNormalizedPosition(Vector3 target, Vector3 start, Vector3 end)
    {
        float totalDistance = Vector3.Distance(start, end);
        float distanceToEntity = Vector3.Distance(start, target);

        // Ensure the division is safe and won't result in division by zero
        if (totalDistance > 0)
        {
            float t = distanceToEntity / totalDistance;
            return Mathf.Clamp01(t);
        }

        return 0f; // fallback to 0 if totalDistance is 0
    }
}