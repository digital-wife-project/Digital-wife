using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class 存储加载agent的相关数据 : MonoBehaviour
{
    internal static string digSaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dig_save");
    public static void AddAgentData(string agentName, string agentVoice, string agentModel)
    {
        // 定义存储路径
        string digSaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dig_save");
        string saveJsonPath = Path.Combine(digSaveFolderPath, "save.json");

        // 确保dig_save文件夹存在
        if (!Directory.Exists(digSaveFolderPath))
        {
            Directory.CreateDirectory(digSaveFolderPath);
        }

        // 创建代理内容
        JObject agentContent = new()
        {
            ["tts"] = agentVoice,
            ["model"] = agentModel
        };

        // 创建代理数据
        JObject agentData = new()
        {
            [agentName] = agentContent
        };

        try
        {
            // 检查文件是否存在并读取
            JObject saveData = File.Exists(saveJsonPath) ? JObject.Parse(File.ReadAllText(saveJsonPath)) : new JObject();

            // 合并代理数据
            saveData.Merge(agentData, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            // 将更新后的数据写回文件
            using StreamWriter file = File.CreateText(saveJsonPath);
            using JsonTextWriter writer = new(file);
            saveData.WriteTo(writer);
        }
        catch (Exception ex)
        {
            // 处理异常，比如记录日志、通知用户等
            Console.WriteLine("无法写入文件: " + ex.Message);
        }
    }
    public static Dictionary<string, object> LoadAgentDataByName(string agentName)
    {
        string saveJsonPath = Path.Combine(digSaveFolderPath, "save.json");
        string jsonContent = System.IO.File.ReadAllText(saveJsonPath);
        var agents = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonContent);

        if (agents.TryGetValue(agentName, out var agentDetails))
        {
            return agentDetails;
        }

        return null; // 如果没有找到代理，返回null
    }
    public static void DeleteAgentData(string agentName)
    {
        string saveJsonPath = Path.Combine(digSaveFolderPath, "save.json");
        // 确保save.json文件存在
        if (File.Exists(saveJsonPath))
        {
            // 读取现有文件内容
            string jsonContent = File.ReadAllText(saveJsonPath);
            // 将内容解析为JObject
            JObject saveData = JObject.Parse(jsonContent);
            // 如果存在要删除的代理条目，则移除它
            if (saveData[agentName] != null)
            {
                saveData.Remove(agentName);
            }
            // 将更新后的数据写回文件
            File.WriteAllText(saveJsonPath, saveData.ToString());
            Debug.Log("Agent deleted successfully.");
        }
        else
        {
            Debug.Log("Save file does not exist.");
        }
    }
    // 定义枚举类型表示要修改的选项
    // 定义Agent类表示JSON中的agent对象
    public class Agent
    {
        public string Tts { get; set; }
        public string Model { get; set; }
    }

    // 定义JsonData类表示整个JSON数据，继承自Dictionary<string, Agent>
    public class JsonData : Dictionary<string, Agent>
    {
    }

    // JsonModifier类包含修改JSON文件的方法
    /// <summary>
    /// 修改指定JSON文件中agent的指定选项内容
    /// </summary>
    /// <param name="filePath">JSON文件的路径</param>
    /// <param name="agentName">要修改的agent名称</param>
    /// <param name="opt">要修改的选项类型</param>
    /// <param name="content">新的内容</param>
    /// <param name="errorMessage">输出的错误信息</param>
    /// <returns>操作是否成功</returns>
    public static void ModifyJson(string agentName, string opt, string content)
    {
        string filePath = Path.Combine(digSaveFolderPath, "save.json");

        // 读取JSON文件
        string json = File.ReadAllText(filePath);
        // 解析JSON
        JsonData data = JsonConvert.DeserializeObject<JsonData>(json);
        // 获取agent对象
        Agent agent = data[agentName];
        // 更改指定的内容
        switch (opt)
        {
            case "Tts":
                agent.Tts = content;
                break;
            case "Model":
                agent.Model = content;
                break;
        }
        // 保存回文件
        File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}
