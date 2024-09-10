using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    Shader _bloomShader;

    [SerializeField]
    Shader _compositeShader;

    Material _bloomMaterial;
    Material _compositeMaterial;

    CustomPostProcessPass _customPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(_customPass);
        }
    }

    public override void Create()
    {
        _bloomMaterial = CoreUtils.CreateEngineMaterial(_bloomShader);
        _compositeMaterial = CoreUtils.CreateEngineMaterial(_compositeShader);
        _customPass = new (_bloomMaterial, _compositeMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if(renderingData.cameraData.cameraType == CameraType.Game)
        {
            _customPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Color);
            _customPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_bloomMaterial);
        CoreUtils.Destroy(_compositeMaterial);
    }
}
