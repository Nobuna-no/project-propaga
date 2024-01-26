using NobunAtelier;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OmmnoobControllerModule : PlayerControllerModuleBase
{
    [SerializeField] private string m_exitDirectionActionName = "Cancel";
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private RawImage m_targetImage;
    [SerializeField] private UnityEvent OnSignExit;

    private InputAction m_cancelAction;

    public void SetTargetSign(InteractableObjectBehaviour target)
    {
        Debug.Assert(target != null, this);

        var sign = target as FTUESign;
        Debug.Assert(sign != null, this);
        m_targetImage.texture = sign.TutoTexture;
        m_canvas.gameObject.SetActive(true);
    }

    public override void EnableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
    {
        m_cancelAction = activeActionMap.FindAction(m_exitDirectionActionName);
        Debug.Assert(m_cancelAction != null);

        m_cancelAction.performed += OnCancelActionPerformed;
    }

    public override void DisableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
    {
        if (m_cancelAction != null)
        {
            m_cancelAction.performed -= OnCancelActionPerformed;
        }

        m_cancelAction = null;
    }

    private void OnCancelActionPerformed(InputAction.CallbackContext obj)
    {
        m_targetImage.texture = null;
        m_canvas.gameObject.SetActive(false);
        OnSignExit?.Invoke();
    }
}