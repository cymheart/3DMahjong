using UnityEngine;
using System.Collections;

//非运行时也触发效果
[ExecuteInEditMode]
//屏幕后处理特效一般都需要绑定在摄像机上
[RequireComponent(typeof(Camera))]
//提供一个后处理的基类，主要功能在于直接通过Inspector面板拖入shader，生成shader对应的材质
public class PostEffectBase : MonoBehaviour
{

    //Inspector面板上直接拖入
    public Shader shader = null;
    private Material _material = null;
    public Material _Material
    {
        get
        {
            if (_material == null)
                _material = GenerateMaterial(shader);
            return _material;
        }
    }

    //根据shader创建用于屏幕特效的材质
    protected Material GenerateMaterial(Shader shader)
    {
        if (shader == null)
            return null;
        //需要判断shader是否支持
        if (shader.isSupported == false)
            return null;
        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;
        return null;
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
