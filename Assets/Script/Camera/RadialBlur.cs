using System;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable]
[VolumeComponentMenu("Post-processing/Custom/RadialBlur")]
public sealed class RadialBlur : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    private const string ShaderName = "Hidden/Shader/RadialBlur";

    private static readonly int Intensity = Shader.PropertyToID("_Intensity");
    private static readonly int SampleCount = Shader.PropertyToID("_SampleCount");
    private static readonly int InputTexture = Shader.PropertyToID("_InputTexture");

    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public ClampedIntParameter sampleCount = new ClampedIntParameter(8, 4, 16);

    private Material _material;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public bool IsActive()
    {
        return _material != null && intensity.value > 0f;
    }

    public override void Setup()
    {
        if (Shader.Find(ShaderName) != null)
        {
            _material = new Material(Shader.Find(ShaderName));
        }
        else
        {
            Debug.LogError($"Unable to find shader '{ShaderName}'. Post Process Volume RadialBlur is unable to load.");
        }
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (_material == null)
        {
            return;
        }

        _material.SetFloat(Intensity, intensity.value);
        _material.SetFloat(SampleCount, sampleCount.value);
        _material.SetTexture(InputTexture, source);
        HDUtils.DrawFullScreen(cmd, _material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(_material);
    }
}