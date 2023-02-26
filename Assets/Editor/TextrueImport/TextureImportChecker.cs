// TextureFormatChecker.cs
// 创建者： 张毅文
// 创建时间：2022/07/27
// 概要：

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureImportChecker : AssetPostprocessor
{
    private const string PORTRAITS_PATH = "Assets/Art/hall/variation/portraits";
    private const string ITEMS_PATH = "Assets/Art/hall/variation/items";
    private const string ENVIRONMENT_PATH = "Assets/Art/hall/variation/environment";
    
    private void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        var importPath = AssetDatabase.GetAssetPath(textureImporter);
        Debug.Log("import path: " + importPath);

        CheckTextureImportInDir(PORTRAITS_PATH, importPath, ".jpg");
        CheckTextureImportInDir(ITEMS_PATH, importPath, ".png");
        CheckTextureImportInDir(ENVIRONMENT_PATH, importPath, ".jpg");
        
        ImportTextureToSpriteInDir(PORTRAITS_PATH, textureImporter);
        ImportTextureToSpriteInDir(ITEMS_PATH, textureImporter);
        ImportTextureToSpriteInDir(ENVIRONMENT_PATH, textureImporter);
    }
    
    /// <summary>
    /// 检查指定文件夹下的texture导入
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="importPath"></param>
    private void CheckTextureImportInDir(string dirPath, string importPath, string expExtensionName)
    {
        if (!importPath.Contains(@dirPath))//主要是针对是否导入项目某个文件夹下进行判断
        {
            return;
        }

        if (!DoesExtensionNameMatch(importPath, expExtensionName))
        {
            Debug.LogError("导入"+ importPath + "扩展名不符合该文件夹下的要求 " + expExtensionName + "。现自动更改扩展名...");
            
            RenameExtension(importPath, expExtensionName);
        }
    }

    /// <summary>
    /// 检查texture扩展名是否与期望的相同
    /// </summary>
    /// <param name="texturePath">texture路径</param>
    /// <param name="expExtensionName">期望的扩展名</param>
    private bool DoesExtensionNameMatch(string texturePath, string expExtensionName)
    {
        var extensionName = System.IO.Path.GetExtension(texturePath);
        return extensionName == expExtensionName;
    }
    
    /// <summary>
    /// 修改扩展名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="targetExtension"></param>
    private void RenameExtension(string path, string targetExtension)
    {
        FileInfo fileInfo = new FileInfo(path);
        fileInfo.MoveTo(Path.ChangeExtension(path, targetExtension));
        Debug.Log("扩展名更改完成");
    }

    /// <summary>
    /// 在目标文件下的导入的texture设置类型为sprite
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="textureImporter"></param>
    private void ImportTextureToSpriteInDir(string dirPath, TextureImporter textureImporter)
    {
        string importPath = AssetDatabase.GetAssetPath(textureImporter);
        
        if (!importPath.Contains(@dirPath))//主要是针对是否导入项目某个文件夹下进行判断
        {
            return;
        }
        
        textureImporter.textureType = TextureImporterType.Sprite;
    }
}