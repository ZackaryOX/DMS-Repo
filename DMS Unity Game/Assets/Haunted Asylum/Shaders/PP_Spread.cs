using UnityEngine;

using UnityEngine.Rendering;

using UnityEngine.Rendering.HighDefinition;

using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/PP_Spread")]

public sealed class PP_Spread : CustomPostProcessVolumeComponent, IPostProcessComponent

{

    [Tooltip("Controls the intensity of the effect.")]

    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    public Texture2D _BloodTexture;

    

    public Texture2D _Noise;
    //public Texture2D _CameraDepth;




    Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()

    {
        

        if (Shader.Find("Hidden/Shader/PP_Spread") != null)

            m_Material = new Material(Shader.Find("Hidden/Shader/PP_Spread"));

    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)

    {

        if (m_Material == null)

            return;

        m_Material.SetFloat("_Intensity", intensity.value);


        if (Camera.main != null)
        {
            {
                m_Material.SetMatrix("_InverseProjection", Camera.main.projectionMatrix.inverse);

                m_Material.SetMatrix("_ViewToWorld", Camera.main.cameraToWorldMatrix);

                m_Material.SetMatrix("_Projection", Camera.main.projectionMatrix);
            }

            m_Material.SetTexture("_InputTexture", source);

            m_Material.SetTexture("_BloodTexture", _BloodTexture);

            m_Material.SetTexture("_Noise", _Noise);


            HDUtils.DrawFullScreen(cmd, m_Material, destination);

        }
    }

    public override void Cleanup() => CoreUtils.Destroy(m_Material);

}