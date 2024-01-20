using NaughtyAttributes;
using NobunAtelier;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OmmrootControllerModule : PlayerControllerModuleBase
{
    [SerializeField]
    private Character m_originalCharacter;

    [SerializeField]
    private string m_confirmDirectionActionName = "Confirm";

    [SerializeField]
    private float m_floorOffset = 1f;

    private InputAction m_confirmAction;
    private OmmrootNode m_targetBehavior;

    [SerializeField] private UnityEvent OnGrowthConfirmation;

    [InfoBox("Can be use in case the player is trying to grow in an inaccessible tile.")]
    [SerializeField] private UnityEvent OnGrowthAttemptFailed;

    public void SetTargetOmmroot(InteractableObjectBehaviour target)
    {
        Debug.Assert(target != null, this);
        m_targetBehavior = target.GetComponentInParent<OmmrootNode>();
        Debug.Assert(m_targetBehavior != null,
            $"target object don't have an {typeof(OmmrootNode).Name} in its parent.", this);
    }

    public override void EnableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
    {
        m_confirmAction = activeActionMap.FindAction(m_confirmDirectionActionName);
        Debug.Assert(m_confirmAction != null);

        m_confirmAction.performed += OnConfirmActionPerformed;

        ControlledCharacter.ResetCharacter(m_targetBehavior.transform.position + Vector3.up * m_floorOffset,
            GetClosestDiscreteRotation(m_originalCharacter.Rotation));
    }

    public override void DisableModuleInput(PlayerInput playerInput, InputActionMap activeActionMap)
    {
        if (m_confirmAction != null)
        {
            m_confirmAction.performed -= OnConfirmActionPerformed;
        }

        m_confirmAction = null;
    }

    private Quaternion GetClosestDiscreteRotation(Quaternion input)
    {
        // Get the euler angles from the input quaternion
        Vector3 euler = input.eulerAngles;

        // Quantize the euler angles to the nearest discrete rotation
        euler.x = euler.z = 0f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;

        // Convert the quantized euler angles back to quaternion
        Quaternion result = Quaternion.Euler(euler);

        return result;
    }

    private void OnConfirmActionPerformed(InputAction.CallbackContext obj)
    {
        Debug.Log("OnConfirmActionPerformed");
        if (!m_targetBehavior)
        {
            return;
        }

        float yRotation = ControlledCharacter.Rotation.eulerAngles.y;

        if (IsCloseTo(yRotation, 0f))
        {
            if (!m_targetBehavior.IsTopRootAccessible())
            {
                OnGrowthAttemptFailed?.Invoke();
                return;
            }

            m_targetBehavior.GrowTopRoot();
            OnGrowthConfirmation?.Invoke();
        }
        else if (IsCloseTo(yRotation, 90f))
        {
            // right
            if (!m_targetBehavior.IsRightRootAccessible())
            {
                OnGrowthAttemptFailed?.Invoke();
                return;
            }

            m_targetBehavior.GrowRightRoot();
            OnGrowthConfirmation?.Invoke();
        }
        else if (IsCloseTo(yRotation, 180f))
        {
            // bottom
            if (!m_targetBehavior.IsBottomRootAccessible())
            {
                OnGrowthAttemptFailed?.Invoke();
                return;
            }

            m_targetBehavior.GrowBottomRoot();
            OnGrowthConfirmation?.Invoke();
        }
        else if (IsCloseTo(yRotation, 270f))
        {
            // left
            if (!m_targetBehavior.IsLeftRootAccessible())
            {
                OnGrowthAttemptFailed?.Invoke();
                return;
            }

            m_targetBehavior.GrowLeftRoot();
            OnGrowthConfirmation?.Invoke();
        }
    }

    private static bool IsCloseTo(float a, float b, float tolerance = 5f)
    {
        return Mathf.Abs(Mathf.DeltaAngle(a, b)) < tolerance;
    }
}