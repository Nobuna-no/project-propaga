using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorHelper : MonoBehaviour
{
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetAnimParamToggleTrue(string animParam)
    {
        if (!this.enabled)
        {
            return;
        }

        m_animator.SetBool(animParam, true);
    }

    public void SetAnimParamToggleFalse(string animParam)
    {
        if (!this.enabled)
        {
            return;
        }

        m_animator.SetBool(animParam, false);
    }
}