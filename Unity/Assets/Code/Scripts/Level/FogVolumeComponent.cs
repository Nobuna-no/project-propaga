using System;
using UnityEngine.Rendering;

[Serializable]
public class FogVolumeComponent : VolumeComponent
{
    public BoolParameter isActive = new BoolParameter(true);
    public ClampedFloatParameter horizontalBlur =
        new ClampedFloatParameter(0.05f, 0, 0.5f);
    public ClampedFloatParameter verticalBlur =
        new ClampedFloatParameter(0.05f, 0, 0.5f);
    public LayerMaskParameter fullMask = new LayerMaskParameter(0);
    public ClampedFloatParameter fullClearStrength = new ClampedFloatParameter(1.0f, 0.0f, 1.0f);
    public LayerMaskParameter halfMask = new LayerMaskParameter(0);
    public ClampedFloatParameter halfClearStrength = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
    public LayerMaskParameter fogMask = new LayerMaskParameter(0);
    public ClampedFloatParameter fogStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
}