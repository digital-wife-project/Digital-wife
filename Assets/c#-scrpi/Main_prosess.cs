using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main_prosess : MonoBehaviour
{
    public Audio_process audio_process;
    public AnimationLoader animationLoader;
    public InputField User_input;
    public TMP_Dropdown role_select;
    public Text 推理文本;
    public Text 回答文本;
    public Text 进程文本;
    public Animator 加载的人物amiator;

    public async void OnButtonClickStartMainProcee()
    {
        回答文本.text = null;
        推理文本.text = null;
        进程文本.text = null;
        string content = User_input.text;
        string role = role_select.options[role_select.value].text; // TMP_Dropdown也使用.text来获取选项文本
        进程文本.text = "等待LLM响应";
        var (agent_reasoning, agent_act_content, remaining_string) = await SendMessage_Main_prosess(role, content, Agent操作.select_agent_id);
        进程文本.text = "收到LLM的响应";
        回答文本.text = agent_reasoning;
        推理文本.text = remaining_string;
        进程文本.text = "等待TTS响应";
        var responed_audio_abs_path = await Bert_api.GetVoice(remaining_string, Agent操作.select_TTS_id);
        进程文本.text = "收到TTS响应";
        Debug.Log(responed_audio_abs_path);
        var audio_lenght_sec = audio_process.Count_audio_lenght(responed_audio_abs_path);
        Debug.Log("开始请求生成动画");
        
        进程文本.text = "等待动画生成";
        await animationLoader.Creat_anim("A person" + agent_act_content, audio_lenght_sec, Agent操作.select_人物模型_name);
        进程文本.text = "完成";
        Debug.Log("收到动画生成响应");
        Debug.Log("播放音频");
        加载的人物amiator.enabled = false;
        audio_process.PlayAudio(responed_audio_abs_path);
        animationLoader.Playanimation();
        await Task.Delay((int)(audio_lenght_sec) * 1000);
        加载的人物amiator.enabled = true;
    }

    #region
    public static async Task<(string agent_reasoning, string agentActContent, string remainingString)> SendMessage_Main_prosess(string role, string content, string agent_id)
    {
        Debug.Log(agent_id);
        var (agent_reasoning, agent_content) = await SendMessage(agent_id, role, content);
        Debug.Log(agent_reasoning);
        Debug.Log(agent_content);
        var (agent_act_content, remaining_string) = ExtractActContentAndRemainingString(agent_content);
        return (agent_reasoning, agent_act_content, remaining_string);
    }
    static (string actContent, string remainingString) ExtractActContentAndRemainingString(string input)
    {
        string pattern = @"\((act: [^\)]+)\)";
        string actContent = "";
        string remainingString = input;

        // 使用正则表达式找到所有匹配的(act:xx)片段
        Regex regex = new(pattern);
        Match match = regex.Match(input);

        if (match.Success)
        {
            // 提取冒号后的内容
            actContent = match.Value.Substring(match.Value.IndexOf(':') + 1).TrimEnd(')');

            // 移除整个(act:xx)片段
            remainingString = regex.Replace(input, "").Trim();
        }
        return (actContent, remainingString);
    }
    //发送message的方法
    static async Task<(string reasoning, string content)> SendMessage(string agentId, string send_role, string send_content)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "<>");
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var data = new
        {
            messages = new[]
            {
            new
            {
                role = send_role,
                content = send_content
            }
        }
        };

        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"http://localhost:8283/v1/agents/{agentId}/messages", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseContent);

            string reasoning = null;
            string agent_response = null;

            foreach (var message in responseData.messages)
            {
                if (message.reasoning != null)
                {
                    reasoning = message.reasoning;
                }
                if (message.content != null)
                {
                    agent_response = message.content;
                }
            }

            return (reasoning, agent_response);
        }
        else
        {
            throw new HttpRequestException($"Error: {response.StatusCode}");
        }
    }
    #endregion
}
