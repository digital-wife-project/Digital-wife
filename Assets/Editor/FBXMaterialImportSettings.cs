using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXMaterialImportSettings : ScriptableObject
{
    [MenuItem("Tools/Set FBX Material Import Settings")]
    static void SetFBXMaterialImportSettings()
    {
        // 设置要搜索的文件夹路径
        string rootFolderPath = "Assets/模型"; // 修改为你的FBX文件所在的根文件夹路径
        SetMaterialImportSettingsRecursively(rootFolderPath);
    }

    static void SetMaterialImportSettingsRecursively(string path)
    {
        // 遍历文件夹中的所有文件
        foreach (string filePath in Directory.GetFiles(path))
        {
            // 检查文件是否为FBX文件
            if (filePath.EndsWith(".fbx"))
            {
                SetMaterialImportSettingsForFBX(filePath);
            }
        }

        // 遍历子文件夹
        foreach (string directoryPath in Directory.GetDirectories(path))
        {
            SetMaterialImportSettingsRecursively(directoryPath);
        }
    }

    static void SetMaterialImportSettingsForFBX(string assetPath)
    {
        ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (modelImporter != null)
        {
            // 设置材质导入模式为旧版
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            modelImporter.materialLocation= ModelImporterMaterialLocation.External;
            // 设置从外部导入名称依据模型材质名称
            modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;

            // 重新导入模型以应用新的设置
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}
