using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web;
using UnityEngine;

public class Bert_api : MonoBehaviour
{
    //加载模型并返回模型id
    public static async Task<int> GetLocalModels(string userChoice)
    {
        using HttpClient client = new();
        var response = await client.GetAsync($"http://127.0.0.1:5000/models/get_local?root_dir=Data");
        var data = JObject.Parse(await response.Content.ReadAsStringAsync());
        var result = $"./Data/{userChoice}/{data[userChoice][0]}";

        var addResponse = await client.GetAsync($"http://localhost:5000/models/add?model_path={result}&language=AUTO");
        var responseData = JObject.Parse(await addResponse.Content.ReadAsStringAsync());
        var modelId = responseData["Data"]["model_id"].ToObject<int>();
        return modelId;
    }
    //生成音频并返回音频的绝对路径
    public static async Task<string> GetVoice(string agentSaying, int modelId)
    {
        // 假设这是您的服务器地址
        string baseUrl = "http://localhost:5000/voice";

        // 构建查询字符串
        var query = new System.Collections.Generic.Dictionary<string, string>
    {
        { "text", agentSaying },
        { "model_id", modelId.ToString() },
        { "speaker_id", "0" },
        { "auto_split", "True" }
    };

        // 将参数添加到URL中
        string queryString = string.Join("&", query.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        string urlWithQuery = $"{baseUrl}?{queryString}";

        // 发送 GET 请求
        using HttpClient client = new();
        var response = await client.GetAsync(urlWithQuery);

        // 打印响应内容
        if (response.IsSuccessStatusCode)
        {
            // 检查响应的内容类型是否为音频
            string contentType = response.Content.Headers.ContentType.MediaType;
            if (contentType.StartsWith("audio"))
            {
                // 指定保存的文件名
                string filename = "output.wav";

                // 获取当前工作目录
                string currentDirectory = Environment.CurrentDirectory;

                // 构建文件的绝对路径
                string filePath = Path.Combine(currentDirectory, filename);

                // 以二进制写入模式打开文件
                using (FileStream audioFile = new(filePath, FileMode.Create))
                {
                    // 写入音频数据
                    await response.Content.CopyToAsync(audioFile);
                }

                // 返回音频文件的绝对路径
                return filePath;
            }
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
        // 如果请求失败，返回 null
        return null;
    }
    public static async Task DeleteModels()
    {
        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync("http://localhost:5000/models/info");
        if (response.IsSuccessStatusCode)
        {
            string responseData = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(responseData);
            Console.WriteLine(data);
            if (data.HasValues)
            {
                foreach (var i in data.Properties())
                {
                    string requestUrl = $"http://localhost:5000/models/delete?model_id={i.Name}";
                    HttpResponseMessage deleteResponse = await client.GetAsync(requestUrl);
                    if (deleteResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("模型删除成功。");
                    }
                    else
                    {
                        Console.WriteLine($"模型删除失败，状态码：{deleteResponse.StatusCode}");
                    }
                }
            }
            else
            {
                Console.WriteLine("当前未加载模型");
            }
        }

    }

}