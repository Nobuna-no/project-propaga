using System;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FogRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private FogMaterialSettings materials;
    [SerializeField] private FogSettings settings;
    private FogRenderPass fogRenderPass;

    public override void Create()
    {
        if (materials.IsAnyNull)
        {
            return;
        }
        fogRenderPass = new FogRenderPass(materials, settings)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(fogRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        fogRenderPass?.Dispose();
    }
}

[Serializable]
public class FogSettings
{
    [Range(0, 0.4f)] public float horizontalBlur;
    [Range(0, 0.4f)] public float verticalBlur;

	public LayerMask fullMask;
	public float fullClearStrength;
	public LayerMask halfMask;
	public float halfClearStrength;
	public LayerMask fogMask;
	public float fogStrength;
}

[Serializable]
public struct FogMaterialSettings
{
    public Material blur;
    public Material render;
    public Material blit;

    public bool IsAnyNull
    {
        get => blur == null || render == null || blit == null;
    }
}