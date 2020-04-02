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

     GameObject _CameraMain;

     GameObject _ghostPosition;

    public Texture2D _Noise;

    Vector4 _ghostVec;

    bool _startup = false;



    Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/PP_Spread") != null)
        {
            m_Material = new Material(Shader.Find("Hidden/Shader/PP_Spread"));
            //Debug.Log("ShaderFound");
        }

       _ghostPosition = GameObject.Find("AI");

    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)

    {
        
        if (_startup == false)
        {
            GameObject temp2 = GameObject.Find("Player0");
            if (temp2 != null)
            {
                _startup = true;
                _CameraMain = temp2;
               
            }
            else
            {
                //Debug.Log("Camera Load Failed");
            }

        } else { 

            if (m_Material == null)
            {

                return;
            }

             
                m_Material.SetFloat("_Intensity", intensity.value);
              

            m_Material.SetVector("_ghostPos", (_CameraMain.GetComponentInChildren<Camera>().WorldToScreenPoint(new Vector3(_ghostPosition.transform.position.x, _ghostPosition.transform.position.y, _CameraMain.GetComponentInChildren<Camera>().nearClipPlane))));

            Debug.Log(_CameraMain.GetComponentInChildren<Camera>().WorldToScreenPoint(new Vector3(_ghostVec.x, _ghostVec.y, _CameraMain.GetComponentInChildren<Camera>().nearClipPlane)));

                m_Material.SetMatrix("_InverseProjection", _CameraMain.GetComponentInChildren<Camera>().projectionMatrix.inverse);

                m_Material.SetMatrix("_ViewToWorld", _CameraMain.GetComponentInChildren<Camera>().cameraToWorldMatrix);

                m_Material.SetMatrix("_Projection", _CameraMain.GetComponentInChildren<Camera>().projectionMatrix);

                m_Material.SetInt("_screenY", Screen.height);


                m_Material.SetTexture("_InputTexture", source);

                m_Material.SetTexture("_BloodTexture", _BloodTexture);

                m_Material.SetTexture("_Noise", _Noise);



                HDUtils.DrawFullScreen(cmd, m_Material, destination);
            

            Debug.Log("Camera Works");
        }
    }
    


    public override void Cleanup() => CoreUtils.Destroy(m_Material);

}