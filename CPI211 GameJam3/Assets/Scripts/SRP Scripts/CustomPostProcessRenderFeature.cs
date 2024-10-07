using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
    //Sets up and configures custom render passes and queues them into the renderer
    //A feature can have multiple passes if needed for an effect
    [SerializeField]
    private Shader m_bloomShader; //we are interrupting a bloom shader to take its texture and modify it with the ben day dots before rendering it
    [SerializeField]
    private Shader m_compositeShader; //the ben day dots are then composited with the original bloom texture

    private Material m_bloomMaterial;
    private Material m_compositeMaterial;

    private CustomPostProcessPass m_customPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_customPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_customPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            m_customPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_customPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }

        base.SetupRenderPasses(renderer, renderingData);
    }

    public override void Create()
    {
        m_bloomMaterial = CoreUtils.CreateEngineMaterial(m_bloomShader);
        m_compositeMaterial = CoreUtils.CreateEngineMaterial(m_compositeShader);

        m_customPass = new CustomPostProcessPass(m_bloomMaterial, m_compositeMaterial);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_bloomMaterial);
        CoreUtils.Destroy(m_compositeMaterial);
    }
}
