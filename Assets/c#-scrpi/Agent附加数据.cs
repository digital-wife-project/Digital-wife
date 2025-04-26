using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Agent附加数据 : MonoBehaviour
{
    public TMP_Dropdown TTS_opt; // 拖拽一个 TMP Dropdown 到这个字段
    public Dropdown Cerate_人物模型_OPT; // 拖拽一个 TMP Dropdown 到这个字段
    public Dropdown Change_人物模型_Change_OPT; // 拖拽一个 TMP Dropdown 到这个字段

    void Start()
    {
        StartCoroutine(GetTTSModelList());
        FillDropdownWithManifestFiles();
    }

    public void FillDropdownWithManifestFiles()
    {
        // StreamingAssets文件夹的路径
        string streamingAssetsPath = Application.streamingAssetsPath;
        // 查找所有文件
        string[] manifestFiles = Directory.GetFiles(streamingAssetsPath, "*.manifest");

        // 用于存储下拉列表选项的列表
        List<string> options = new List<string>();

        // 遍历所有找到的文件
        foreach (var file in manifestFiles)
        {
            // 获取文件名（包含后缀名）
            string fileNameWithExtension = Path.GetFileName(file);
            // 去除文件后缀名
            string fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
            // 添加文件名到选项列表
            options.Add(fileName);
        }

        // 清除下拉列表的现有选项
        Cerate_人物模型_OPT.ClearOptions();
        Change_人物模型_Change_OPT.ClearOptions();

        // 将选项列表添加到下拉列表
        Cerate_人物模型_OPT.AddOptions(options);
        Change_人物模型_Change_OPT.AddOptions(options);

    }
    IEnumerator GetTTSModelList()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:5000/models/get_local");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            PopulateTTSDropdown(www.downloadHandler.text);
        }
    }

    void PopulateTTSDropdown(string jsonResponse)
    {
        JObject jsonObject = JObject.Parse(jsonResponse);
        List<string> options = new();

        foreach (var pair in jsonObject)
        {
            options.Add(pair.Key);
        }

        TTS_opt.ClearOptions();
        TTS_opt.AddOptions(options);
    }

    public void StartGetTTSModelList()
    {
        StartCoroutine(GetTTSModelList());
    }

}
