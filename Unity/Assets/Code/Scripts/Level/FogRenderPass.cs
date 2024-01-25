using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogRenderPass : ScriptableRenderPass
{
    private static readonly int horizontalBlurId =
        Shader.PropertyToID("_HorizontalBlur");
    private static readonly int verticalBlurId =
        Shader.PropertyToID("_VerticalBlur");
    private static readonly int clearStrengthId =
        Shader.PropertyToID("_FogStrength");

    private FogSettings defaultSettings;
    private FogMaterialSettings materials;
	List<ShaderTagId> passNames = new List<ShaderTagId>();

    private const int kLayers = 3;
    FilteringSettings[] filteringSettings = new FilteringSettings[kLayers]
                    {
                        new FilteringSettings(RenderQueueRange.all),
                        new FilteringSettings(RenderQueueRange.all),
                        new FilteringSettings(RenderQueueRange.all)
                    };


	private RenderTextureDescriptor textureDescriptor;
    private RTHandle safeZoneTextureHandle;
    private RTHandle blurTextureHandle;

    public FogRenderPass(FogMaterialSettings materials, FogSettings defaultSettings)
    {
        this.materials = materials;
        this.defaultSettings = defaultSettings;

		passNames.Add(new ShaderTagId("SRPDefaultUnlit"));
		passNames.Add(new ShaderTagId("UniversalForward"));
		passNames.Add(new ShaderTagId("UniversalForwardOnly"));

		textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Set the texture size to be the same as the camera target size.
        textureDescriptor.width = cameraTextureDescriptor.width;
        textureDescriptor.height = cameraTextureDescriptor.height;

        // Check if the descriptor has changed, and reallocate the RTHandle if necessary
        RenderingUtils.ReAllocateIfNeeded(ref safeZoneTextureHandle, textureDescriptor);
        RenderingUtils.ReAllocateIfNeeded(ref blurTextureHandle, textureDescriptor);

        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<FogVolumeComponent>();

        float fullFogStrength = volumeComponent.fogStrength.overrideState ? 
            volumeComponent.fogStrength.value : defaultSettings.fogStrength;
        ConfigureTarget(safeZoneTextureHandle);
        ConfigureClear(ClearFlag.Color, Color.white * fullFogStrength);
    }

    private bool UpdateFogSettings(out float[] strengths)
    {
        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<FogVolumeComponent>();
        float horizontalFog = volumeComponent.horizontalBlur.overrideState ?
            volumeComponent.horizontalBlur.value : defaultSettings.horizontalBlur;
        float verticalFog = volumeComponent.verticalBlur.overrideState ?
            volumeComponent.verticalBlur.value : defaultSettings.verticalBlur;

		if (materials.blur != null)
		{
			materials.blur.SetFloat(horizontalBlurId, horizontalFog);
			materials.blur.SetFloat(verticalBlurId, verticalFog);
		}

        filteringSettings[0].layerMask = volumeComponent.halfMask.overrideState ?
            volumeComponent.halfMask.value : defaultSettings.halfMask;
        filteringSettings[1].layerMask = volumeComponent.fullMask.overrideState ?
            volumeComponent.fullMask.value : defaultSettings.fullMask;
        filteringSettings[2].layerMask = volumeComponent.fogMask.overrideState ?
            volumeComponent.fogMask.value : defaultSettings.fogMask;

        strengths = new float[kLayers];
        strengths[0] = volumeComponent.halfClearStrength.overrideState ?
            volumeComponent.halfClearStrength.value : defaultSettings.halfClearStrength;
        strengths[1] = volumeComponent.fullClearStrength.overrideState ?
            volumeComponent.fullClearStrength.value : defaultSettings.fullClearStrength;
        strengths[2] = volumeComponent.fogStrength.overrideState ?
            volumeComponent.fogStrength.value : defaultSettings.fogStrength;

        return volumeComponent.isActive.overrideState ? volumeComponent.isActive.value : true;
	}

    private RendererListParams[] GetRenderingLists(ref RenderingData renderingData)
	{
		ref CullingResults cullingResults = ref renderingData.cullResults;
		SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
		DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(passNames, ref renderingData, sortingCriteria);
        drawingSettings.overrideMaterial = materials.render;
        drawingSettings.overrideMaterialPassIndex = 0;

        return new RendererListParams[kLayers]
        {   new RendererListParams(cullingResults, drawingSettings, filteringSettings[0]),
            new RendererListParams(cullingResults, drawingSettings, filteringSettings[1]),
            new RendererListParams(cullingResults, drawingSettings, filteringSettings[2]),
             };
	}

	public override void Execute(ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        float[] strengths;
        if (!UpdateFogSettings(out strengths))
        {
            return;
        }

        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        RenderObjects(cmd, context, ref renderingData, strengths);

        Blur(cmd, safeZoneTextureHandle, safeZoneTextureHandle);

        Blit(cmd, safeZoneTextureHandle, cameraTargetHandle, materials.blit);

        //Execute the command buffer and release it back to the pool.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void Blur(CommandBuffer cmd, RTHandle source, RTHandle destination)
    {
        Blit(cmd, source, blurTextureHandle, materials.blur, 0);
        Blit(cmd, blurTextureHandle, destination, materials.blur, 1);
    }

    private void RenderObjects(CommandBuffer cmd, ScriptableRenderContext context,
		ref RenderingData renderingData, float[] strengths)
	{
		RendererListParams[] rendererListParams = GetRenderingLists(ref renderingData);

        for (int i = 0 ; i < kLayers ; ++i)
        {
		    RendererList rendererList = context.CreateRendererList(ref rendererListParams[i]);
            cmd.SetGlobalFloat(clearStrengthId, strengths[i]);
            cmd.DrawRendererList(rendererList);
        }
    }

    public void Dispose()
    {
        if (blurTextureHandle != null) blurTextureHandle.Release();
        if (safeZoneTextureHandle != null) safeZoneTextureHandle.Release();
    }
}