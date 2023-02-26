// CutoutMaskUI.cs
// 创建者： 张毅文
// 创建时间：2022/05/18
// 概要：

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace LinkMemory
{
    public class CutoutMaskUI : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material mat = new Material(base.materialForRendering);
                mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return mat;
            }
        }
    }
}