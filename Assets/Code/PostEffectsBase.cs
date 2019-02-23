using UnityEngine;

/// <summary>
/// 屏幕后处理效果基类
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectsBase : MonoBehaviour
{
    private Camera mainCamera;
    public Camera MainCamera { get { return mainCamera = mainCamera == null ? GetComponent<Camera>() : mainCamera; } }

    public Shader targetShader;
    private Material targetMaterial = null;
    public Material TargetMaterial { get { return CheckShaderAndCreateMaterial(targetShader,ref targetMaterial); } }

    /// <summary>
    /// 检测资源，如果不支持，关闭脚本活动
    /// </summary>
    protected void Start()
    {
        if (CheckSupport() == false)
            enabled = false;
    }

    /// <summary>
    /// 检测平台是否支持图片渲染
    /// </summary>
    /// <returns></returns>
    protected bool CheckSupport()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            Debug.LogWarning("This platform does not support image effects or render textures.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 检测需要渲染的Shader可用性，然后返回使用了该shader的material
    /// </summary>
    /// <param name="shader">指定shader</param>
    /// <param name="material">创建的材质</param>
    /// <returns>得到指定shader的材质</returns>
    protected Material CheckShaderAndCreateMaterial(Shader shader, ref Material material)
    {
        if (shader == null || !shader.isSupported)
            return null;

        if (material && material.shader == shader)
            return material;

        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }
}
