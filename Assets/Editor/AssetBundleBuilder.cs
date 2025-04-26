using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetBundleBuilder:MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        string sourceFolder = "Assets/模型";
        string outputFolderPath = "Assets/StreamingAssets";

        // 获取源文件夹下的所有子文件夹
        string[] subFolders = AssetDatabase.GetSubFolders(sourceFolder);

        if (subFolders.Length == 0)
        {
            Debug.LogWarning("在 " + sourceFolder + " 中未找到子文件夹");
            return;
        }

        // 遍历每个子文件夹
        for (int i = 0; i < subFolders.Length; i++)
        {
            string subFolder = subFolders[i];
            string bundleName = Path.GetFileName(subFolder);

            // 获取子文件夹中的所有资产路径
            string[] assetGUIDs = AssetDatabase.FindAssets("", new string[] { subFolder });
            string[] assetPaths = assetGUIDs.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();

            if (assetPaths.Length == 0)
            {
                Debug.LogWarning(subFolder + " 中未找到资产");
                continue;
            }

            // 显示进度条
            EditorUtility.DisplayProgressBar("构建AssetBundles", "处理中 " + bundleName, i / (float)subFolders.Length);

            // 设置AssetBundle名称
            SetAssetBundleName(assetPaths, bundleName);

            // 打包AssetBundle
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputFolderPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            if (manifest == null)
            {
                Debug.LogError(bundleName + " 的AssetBundles构建失败");
            }
            else
            {
                Debug.Log("成功构建 " + bundleName + " 的AssetBundles");

                // 加载并记录AssetBundleManifest
                LoadAndLogManifest(manifest);
            }

            // 重置AssetBundle名称
            ResetAssetBundleName(assetPaths);
        }

        // 隐藏进度条
        EditorUtility.ClearProgressBar();
    }

    private static void SetAssetBundleName(string[] assetPaths, string bundleName)
    {
        foreach (string assetPath in assetPaths)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                Debug.LogWarning(assetPath + " 的导入器未找到");
                continue;
            }
            importer.assetBundleName = bundleName;
        }
    }

    private static void ResetAssetBundleName(string[] assetPaths)
    {
        foreach (string assetPath in assetPaths)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                Debug.LogWarning(assetPath + " 的导入器未找到");
                continue;
            }
            importer.assetBundleName = "";
        }
    }

    private static void LoadAndLogManifest(AssetBundleManifest manifest)
    {
        if (manifest != null)
        {
            string[] allAssetBundles = manifest.GetAllAssetBundles();
            Debug.Log("Manifest中的AssetBundles: " + string.Join(", ", allAssetBundles));
        }
        else
        {
            Debug.LogError("加载AssetBundleManifest失败");
        }
    }
}
