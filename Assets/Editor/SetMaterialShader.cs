using UnityEngine;
using UnityEditor;
using System.IO;

public class SetMaterialShader : ScriptableObject
{
    [MenuItem("Tools/Set Material Shader")]
    static void SetMaterialShaderForFolder()
    {
        // 设置要搜索的文件夹路径
        string rootFolderPath = "Assets/模型"; // 修改为你的材质文件所在的根文件夹路径
        Shader targetShader = Shader.Find("SimpleURPToonLitExample(With Outline)"); // 修改为你的目标Shader名称

        if (targetShader == null)
        {
            Debug.LogError("Shader not found: Specified/ShaderName");
            return;
        }

        SetMaterialShaderRecursively(rootFolderPath, targetShader);
    }

    static void SetMaterialShaderRecursively(string path, Shader targetShader)
    {
        // 遍历文件夹中的所有文件
        foreach (string assetPath in Directory.GetFiles(path, "*.mat", SearchOption.AllDirectories))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material != null)
            {
                // 设置材质的Shader
                material.shader = targetShader;
                // 保存材质的变化
                EditorUtility.SetDirty(material);
            }
        }

        // 保存所有更改到磁盘
        AssetDatabase.SaveAssets();
    }
}
