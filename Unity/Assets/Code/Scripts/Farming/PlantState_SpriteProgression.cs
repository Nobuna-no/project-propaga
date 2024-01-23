using NaughtyAttributes;
using NobunAtelier;
using Unity.Cinemachine;
using UnityEngine;

public class PlantState_SpriteProgression : GameState
{
    [SerializeField] private AnimatorProgression[] m_spriteProgressions;
    [SerializeField] private Animator m_targetAnimator;

    [SerializeField, AnimatorParam("m_targetAnimator")]
    private string m_lastStageAnimatorTrigger;

    private AnimatorProgression m_currentStatus;
    private PlantStateMachine m_plantSM;
    private float m_nextProgressionRangeStart = 0;

    public override void Enter()
    {
        base.Enter();

        m_plantSM = ParentStateMachine as PlantStateMachine;
        Debug.Assert(m_plantSM != null);
        Debug.Assert(m_targetAnimator != null);
        RefreshSpriteProgression(m_plantSM.CurrentValue / m_plantSM.MaxValue);
    }

    public override void Exit()
    {
        base.Exit();

        m_targetAnimator.SetTrigger(m_lastStageAnimatorTrigger);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        float progress = m_plantSM.CurrentValue / m_plantSM.MaxValue;
        if (progress >= m_nextProgressionRangeStart)
        {
            RefreshSpriteProgression(m_plantSM.CurrentValue / m_plantSM.MaxValue);
        }
    }

    private void RefreshSpriteProgression(float progression)
    {
        for (int i = 0, c = m_spriteProgressions.Length; i < c; i++)
        {
            AnimatorProgression p = m_spriteProgressions[i];
            if (progression >= p.ProgressionRange.x && progression <= p.ProgressionRange.y)
            {
                m_currentStatus = p;
                if (i + 1 < c)
                {
                    m_nextProgressionRangeStart = m_spriteProgressions[i + 1].ProgressionRange.x;
                }
                else
                {
                    m_nextProgressionRangeStart = 1;
                }
            }
        }

        m_targetAnimator.SetTrigger(m_currentStatus.AnimatorTrigger);
    }

    [System.Serializable]
    private class AnimatorProgression
    {
        [SerializeField, MinMaxRangeSlider(0, 1)]
        private Vector2 m_progressionRange = Vector2.zero;

        [SerializeField]
        private string m_animatorTrigger;

        public Vector2 ProgressionRange => m_progressionRange;
        public string AnimatorTrigger => m_animatorTrigger;
    }
}