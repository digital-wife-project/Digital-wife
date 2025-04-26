#pragma warning disable 649
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TriLibCore;
using TriLibCore.Extensions;
using TriLibCore.Utils;
using UnityEngine;

public class AnimationLoader : MonoBehaviour
{
    string currentAnimationClipName;
    public GameObject baseModel;
    public Animator animator;
    public string currentAnimationName;

    public async Task Creat_anim(string letta_output_action, float audio_lenght_sec, string avater_name)
    {
        Debug.Log("开始生成动画");
        int action_lenth = (int)(audio_lenght_sec / 0.05)+15;
        string anim_path = await SendGetRequest(letta_output_action, action_lenth, avater_name);
        Debug.Log("动画生成完成,收到响应");
        Debug.Log(anim_path);
        LoadAnimation(anim_path);
    }
    public void Playanimation()
    {
        //animator.enabled = false;
        _baseAnimation.CrossFade(currentAnimationClipName);
        //currentAnimationClipName = null;

    }
    #region
    // 示例调用
    //await SendGetRequest("your_text_here", 123);
    static async Task<string> SendGetRequest(string inputText, int inputInt, string avater_name)
    {
        using HttpClient client = new();
        var url = $"http://localhost:19256/t2m_nomal?config_int={inputInt}&config_text={inputText}&avater_name={avater_name}";

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            Debug.Log("Response: " + responseString);
            return responseString;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            Debug.Log("Validation Error: " + responseString);
            return responseString;
        }
        else
        {
            Debug.Log("Error: " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
            return null;
        }
    }
    class Path_Response
    {
        public string Path { get; set; }
    }

    /// <summary>
    /// 基础模型中的动画组件。
    /// </summary>
    private Animation _baseAnimation;

    /// <summary>
    /// 存储已加载动画的AssetLoaderContext列表。
    /// 如果动画在基础模型之前加载，则使用此列表来处理动画。
    /// </summary>
    private readonly IList<AssetLoaderContext> _loadedAnimations = new List<AssetLoaderContext>();

    /// <summary>
    /// 开始时加载基础模型，然后加载同一文件夹中的所有额外动画。
    /// </summary>

    /// <summary>
    /// 从给定的模型文件名加载动画。
    /// </summary>
    void LoadAnimation(string modelPath)
    {
        // 创建默认的加载选项。
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        // 设置加载选项，不导入网格、纹理和材质。
        assetLoaderOptions.ImportMeshes = false;
        assetLoaderOptions.ImportTextures = false;
        assetLoaderOptions.ImportMaterials = false;
        Debug.Log("传递给LoadAnimation方法的参数: " + modelPath);
        // 从文件加载模型，并提供回调函数。
        AssetLoader.LoadModelFromFile(modelPath.Replace("\"", ""), OnAnimationModelLoad, null, OnProgress, OnError, gameObject, assetLoaderOptions);
    }

    /// <summary>
    /// 当动画模型加载完成时调用此方法，收集所有动画并将其添加到基础模型的动画组件中。
    /// </summary>
    /// <param name="assetLoaderContext">用于加载模型的上下文。</param>
    private void OnAnimationModelLoad(AssetLoaderContext assetLoaderContext)
    {
        // 输出已加载的动画文件名。
        Debug.Log($"动画已加载: {FileUtils.GetShortFilename(assetLoaderContext.Filename)}");
        // 如果基础动画组件已初始化，则添加动画。
        if (_baseAnimation != null)
        {
            currentAnimationName= AddAnimationAndPlay(assetLoaderContext);
        }
        else
        {
            // 否则，将加载的动画上下文添加到列表中，稍后处理。
            _loadedAnimations.Add(assetLoaderContext);
        }
        // 禁用加载的动画模型游戏对象。
        assetLoaderContext.RootGameObject.SetActive(false);
    }

    /// <summary>
    /// 将给定AssetLoaderContext的根游戏对象中的动画剪辑添加到基础模型的动画列表中。
    /// </summary>
    /// <param name="loadedAnimationContext">包含已加载动画组件的AssetLoaderContext。</param>
    private string AddAnimationAndPlay(AssetLoaderContext loadedAnimationContext)
    {
        // 获取加载的动画组件。
        Animation rootGameObjectAnimation = loadedAnimationContext.RootGameObject.GetComponent<Animation>();
        if (rootGameObjectAnimation != null)
        {
            // 获取简短的文件名。
            var shortFilename = FileUtils.GetShortFilename(loadedAnimationContext.Filename);
            // 获取所有动画剪辑。
            var newAnimationClips = rootGameObjectAnimation.GetAllAnimationClips();
            foreach (var newAnimationClip in newAnimationClips)
            {
                // 构建动画名称。
                var animationName = $"{shortFilename}_{newAnimationClip.name}";
                // 将新的动画剪辑添加到基础动画组件中。
                _baseAnimation.AddClip(newAnimationClip, animationName);
                Debug.Log($"添加动画: {animationName}");
                currentAnimationClipName = animationName;
            }
            Debug.Log("播放动画");
            return currentAnimationClipName;

        }
        // 销毁加载的动画模型游戏对象，因为它不再需要。
        Destroy(loadedAnimationContext.RootGameObject);
        return null;
    }

    /// <summary>
    /// 加载基础模型，包括所有模型数据。
    /// </summary>
    public void LoadBaseModel(GameObject baseModel)
    {
        // 获取基础模型的动画组件。
        _baseAnimation = baseModel.GetComponent<Animation>();
        if (_baseAnimation == null)
        {
            Debug.LogError("基础模型没有Animation组件。请确保基础模型具有Animation组件。");
            return;
        }
        // 处理之前加载的动画。
        for (var i = _loadedAnimations.Count - 1; i >= 0; i--)
        {
            AddAnimationAndPlay(_loadedAnimations[i]);
            // 从列表中移除已处理的动画。
            _loadedAnimations.RemoveAt(i);
        }
    }

    /// <summary>
    /// 当发生任何错误时调用此方法。
    /// </summary>
    /// <param name="obj">包含原始异常和错误发生时传递的上下文的错误对象。</param>
    private void OnError(IContextualizedError obj)
    {
        // 输出加载模型时发生的错误。
        Debug.LogError($"加载模型时发生错误: {obj.GetInnerException()}");
    }

    /// <summary>
    /// 当模型加载进度改变时调用此方法。
    /// </summary>
    /// <param name="assetLoaderContext">用于加载模型的上下文。</param>
    /// <param name="progress">加载进度。</param>
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        // 这里可以添加代码来处理加载进度，例如更新UI进度条。
    }

    /// <summary>
    /// 当基础模型的网格和层级结构加载完成后调用此方法。
    /// </summary>
    /// <param name="assetLoaderContext">用于加载模型的上下文。</param>
    //private void OnBaseModelLoad(AssetLoaderContext assetLoaderContext)
    //{
    //    // 这里可以添加代码来处理基础模型加载完成后的逻辑。
    //}
    #endregion
}
