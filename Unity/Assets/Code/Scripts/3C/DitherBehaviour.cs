using System.Collections.Generic;

using UnityEngine;

using NobunAtelier.Gameplay;

public class DitherBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform rayOrigin;

    [SerializeField]
    private Transform rayTarget;

    [SerializeField]
    private float maxDistance = 100.0f;

    [SerializeField]
    private LayerMask includeMask;

    private DitherFeedback m_feedbackObject = null;
    public event System.Action<DitherFeedback> OnFeedbackObjectAdded;

    public event System.Action<DitherFeedback> OnFeedbackObjectRemoved;

    private void Start()
    {
        OnFeedbackObjectAdded += OnFeedbackObjectAddedEvent;
        OnFeedbackObjectRemoved += OnFeedbackObjectRemovedEvent;
    }

    private void OnFeedbackObjectAddedEvent(DitherFeedback obj)
    {
        obj.SetObscuring(this, true);
    }
    
    private void OnFeedbackObjectRemovedEvent(DitherFeedback obj)
    {
        obj.SetObscuring(this, false);
    }
    
    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 direction = rayTarget.position - rayOrigin.position;
        if (Physics.Raycast(rayOrigin.position, direction.normalized, out hit, maxDistance, includeMask))
        {
            DitherFeedback feedback = hit.collider?.GetComponent<DitherFeedback>();
            if (feedback == null || feedback != m_feedbackObject)
            {
                if (m_feedbackObject != null)
                {
                    OnFeedbackObjectRemoved?.Invoke(m_feedbackObject);
                }

                m_feedbackObject = feedback;

                if (m_feedbackObject != null)
                {
                    OnFeedbackObjectAdded?.Invoke(m_feedbackObject);
                }
            }
        }
        else if (m_feedbackObject != null)
        {
            OnFeedbackObjectRemoved?.Invoke(m_feedbackObject);
            m_feedbackObject = null;
        }
    }
}