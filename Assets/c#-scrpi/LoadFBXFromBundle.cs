using System.Threading.Tasks;
using UnityEngine;

public class LoadFBXFromBundle : MonoBehaviour
{
    public GameObject LoadModelFromBundle(string assetBundleName)
    {
        // 从StreamingAssets文件夹加载AssetBundle
        string bundleUrl = Application.streamingAssetsPath + "/" + assetBundleName;
        string modelName = DownloadAndLoadModel(bundleUrl, assetBundleName);
        return GameObject.Find(modelName);
    }

    private string DownloadAndLoadModel(string bundleUrl, string modelName)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(bundleUrl);
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return null;
        }

        // 从AssetBundle中加载模型
        GameObject model = bundle.LoadAsset<GameObject>(modelName);
        if (model == null)
        {
            Debug.LogError("Failed to load model from AssetBundle!");
            bundle.Unload(false);
            return null;
        }

        // 实例化模型
        GameObject instantiatedModel = Instantiate(model);
        // 可以在这里设置模型的位置、旋转或父对象等

        // 释放AssetBundle，参数为false表示不卸载加载的资产
        bundle.Unload(false);

        return instantiatedModel.name;
    }
}
