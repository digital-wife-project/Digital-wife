using System.IO;
using UnityEngine;
using UnityEditor;

public class ChangeAnimationTypeToHumanoid : ScriptableWizard
{
    public string folderPath = "Assets/YourModelFolder"; // 请替换为你的模型文件夹路径

    [MenuItem("Tools/Change Animation Type to Humanoid")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ChangeAnimationTypeToHumanoid>("Change Animation Type to Humanoid", "Change");
    }

    void OnWizardCreate()
    {
        ChangeAnimationTypeInFolder(folderPath);
        Debug.Log("Animation types changed to Humanoid for all FBX models in folder: " + folderPath);
    }

    void ChangeAnimationTypeInFolder(string path)
    {
        var allFiles = Directory.GetFiles(path, "*.fbx", SearchOption.AllDirectories);
        foreach (var file in allFiles)
        {
            var importer = AssetImporter.GetAtPath(file) as ModelImporter;
            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Human;

                // 如果需要，可以在这里设置人形映射
                // importer.humanDescription = ...;

                // 重新导入模型以应用更改
                importer.SaveAndReimport();
            }
        }
    }
}
