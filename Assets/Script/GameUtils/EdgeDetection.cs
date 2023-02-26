using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : MonoBehaviour
{
    //声明需要的Shader，并据此创建材质
    public Shader EdgeDetectShader;
    private Material _edgeDetectMaterial;

    public Material Material
    {
        get
        {
            _edgeDetectMaterial = CheckShaderAndCreateMaterial(EdgeDetectShader, _edgeDetectMaterial);
            return _edgeDetectMaterial;
        }
    }

    //提供用于调整边缘线强度、描边颜色、背景颜色的参数
    //当EdgesOnly等于0时，边缘会叠加在原图像上，等于1时，只显示边缘，不显示渲染图像
    [Range(0.0f, 1.0f)] public float EdgesOnly = 0.0f;
    public Color EdgeColor = Color.black;
    public Color BackgroundColor = Color.white;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Material != null)
        {
            Material.SetFloat("_EdgeOnly", EdgesOnly);
            Material.SetColor("_EdgeColor", EdgeColor);
            Material.SetColor("_BackgroundColor", BackgroundColor);

            Graphics.Blit(src, dest, Material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    // 检测Material和Shader，在派生类中调用，绑定材质和shader
    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            return null;
        }

        if (shader.isSupported && material && material.shader == shader)
            return material;

        if (!shader.isSupported)
        {
            return null;
        }
        else
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }

}
