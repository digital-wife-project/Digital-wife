using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class 获取agent信息 : MonoBehaviour
{
    public string url = "http://localhost:8283/v1/agents/";
    public string bearerToken = "YOUR_BEARER_TOKEN"; // 替换为您的Bearer令牌
    public string Agent_List_fileName = "agent.json"; // JSON文件的名称

    class AgentData
    {
        public string id;
        public string name;
    }
    public string Read_json2string()
    {
        获取agent信息 dataManager = FindObjectOfType<获取agent信息>();
        if (dataManager == null)
        {
            Debug.LogError("GetData component not found in the scene.");
            return null;
        }
        return dataManager.LoadJsonFromFile("agent.json");
    }
    public int CountUniqueIds(string jsonString)
    {
        AgentData[] agents = JsonConvert.DeserializeObject<AgentData[]>(jsonString);
        HashSet<string> uniqueIds = new(agents.Select(a => a.id));
        return uniqueIds.Count;
    }

    public HashSet<string> CountAgentNames(string jsonString)
    {
        AgentData[] agents = JsonConvert.DeserializeObject<AgentData[]>(jsonString);
        HashSet<string> uniqueNames = new(agents.Select(a => a.name));
        return uniqueNames;
    }
    public IEnumerator GetAgentData()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = webRequest.downloadHandler.text;
            SaveJsonToFile(jsonResult, Agent_List_fileName);
            Debug.Log(jsonResult);
            Debug.Log("发送get请求");
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
        }
    }
    //使用这个方法来保存的JSON文件
    void SaveJsonToFile(string jsonData, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        using (StreamWriter writer = new(filePath, false))
        {
            writer.WriteLine(jsonData);
        }

        Debug.Log("Data saved to " + filePath);
    }    // 使用这个方法来读取保存的JSON文件
    public string LoadJsonFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            using StreamReader reader = new(filePath);
            return reader.ReadToEnd();
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }
    class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string System { get; set; }
    }
    public string GetIdByName(string name)
    {

        // 解析JSON字符串为对象数组
        Item[] items = JsonConvert.DeserializeObject<Item[]>(Read_json2string());

        // 遍历数组，查找匹配的name
        foreach (var item in items)
        {
            if (item.Name == name)
            {
                // 找到匹配的name，返回对应的id
                return item.Id;
            }
        }
        // 如果没有找到匹配的name，返回null或空字符串
        return null;
    }
    // 使用这个方法来获取用户选择的agent的ID
    public string GetDescriptionByName(string name)
    {
        // 解析JSON字符串为对象数组
        Item[] items = JsonConvert.DeserializeObject<Item[]>(Read_json2string());

        // 遍历数组，查找匹配的name
        foreach (var item in items)
        {
            if (item.Name == name)
            {
                // 找到匹配的name，返回对应的Description
                return item.Description;
            }
        }
        // 如果没有找到匹配的name，返回null或空字符串
        return null;
    }
    // 使用这个方法来获取用户选择的agent的Description

    public string GetSystemByName(string name)
    {
        // 解析JSON字符串为对象数组
        Item[] items = JsonConvert.DeserializeObject<Item[]>(Read_json2string());

        // 遍历数组，查找匹配的name
        foreach (var item in items)
        {
            if (item.Name == name)
            {
                // 找到匹配的name，返回对应的System
                return item.System;
            }
        }
        // 如果没有找到匹配的name，返回null或空字符串
        return null;
    }
}
