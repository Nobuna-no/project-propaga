using System.Data;
using Unity.Cinemachine;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage = null;
    [SerializeField] private ColorProgression[] m_colorProgressions;

    private float m_nextProgressionRangeStart = 0;

    private ColorProgression m_currentStatus;

    public void SetFill(float amount)
    {
        fillImage.fillAmount = amount;
    }

    private void OnEnable()
    {
        RefreshColorProgression(0);
    }

    private void FixedUpdate()
    {
        float progress = fillImage.fillAmount / 1;
        if (progress >= m_nextProgressionRangeStart)
        {
            RefreshColorProgression(progress);
        }
    }

    private void RefreshColorProgression(float progression)
    {
        for (int i = 0, c = m_colorProgressions.Length; i < c; i++)
        {
            ColorProgression p = m_colorProgressions[i];
            if (progression >= p.ProgressionRange.x && progression <= p.ProgressionRange.y)
            {
                m_currentStatus = p;
                if (i + 1 < c)
                {
                    m_nextProgressionRangeStart = m_colorProgressions[i + 1].ProgressionRange.x;
                }
                else
                {
                    m_nextProgressionRangeStart = 1;
                }
            }
        }

        fillImage.color = m_currentStatus.RangeColor;
    }

    [System.Serializable]
    private class ColorProgression
    {
        [SerializeField, MinMaxRangeSlider(0, 1)]
        private Vector2 m_progressionRange = Vector2.zero;

        [SerializeField]
        private Color m_color;

        public Vector2 ProgressionRange => m_progressionRange;
        public Color RangeColor => m_color;
    }
}