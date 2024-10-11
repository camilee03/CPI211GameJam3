using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizationRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Material shaderGraphMaterial;
    class CustomRenderPass : ScriptableRenderPass
    {
        [SerializeField]
        private Material shaderGraphMaterial;
        RTHandle tempTexture, sourceTexture;

        public CustomRenderPass(Material shaderMaterial) : base() //transfer the material down to the subclass render pass
        {
            this.shaderGraphMaterial = shaderMaterial;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            sourceTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;

            tempTexture = RTHandles.Alloc(new RenderTargetIdentifier("_TempTexture"),
                name: "_TempTexture"
                );
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("PixelizationRenderFeature");

            RenderTextureDescriptor targetDesc = renderingData.cameraData.cameraTargetDescriptor;
            targetDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(Shader.PropertyToID(tempTexture.name), targetDesc, FilterMode.Bilinear);

            cmd.Blit(sourceTexture, tempTexture, shaderGraphMaterial);
            cmd.Blit(tempTexture, sourceTexture);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempTexture.Release();
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(this.shaderGraphMaterial);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


