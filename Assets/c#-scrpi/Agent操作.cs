using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LipSync;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Agent操作 : MonoBehaviour
{
    public GameObject Main_Panel;
    public GameObject Agent_Panel;
    public GameObject agent详细信息panel;
    public InputField Agent_name;
    public Text 进程;
    public InputField Agent_description;
    public TMP_Dropdown TTS_opt; // 拖拽一个 TMP Dropdown 到这个字段
    public Dropdown 人物模型;
    public GameObject WaitingPanel;
    public InputField System_prompt;
    public Agent_List_Control agent_List_Control;
    public Agent附加数据 agent附加数据;
    public LoadFBXFromBundle loadFBXFromBundle;
    public AnimationLoader animationLoader;
    public 获取agent信息 获取agent;
    public BlinkController blinkController;
    public Main_prosess main_Prosess;
    public static string select_agent_id;
    public static string select_人物模型_name;
    public static int select_TTS_id;
    public GameObject 加载的人物模型;
    public GameObject BlinkController挂载;
    public GameObject AudioLipSync挂载;
    public LipSync.AudioLipSync lipSync;
    public CameraController cameraController;
    public RuntimeAnimatorController animatorController;

    #region
    // 使用这个方法来使用按钮触发delet
    public async Task OnDeleteButtonClicked()
    {
        Debug.Log("点击删除Agent");
        await DeleteAgent(获取agent.GetIdByName(Agent_List_Control.user_chioce));
        存储加载agent的相关数据.DeleteAgentData(Agent_List_Control.user_chioce);
    }
    // 使用这个方法来发送delet请求
    async Task DeleteAgent(string agentId)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");
        var response = await client.DeleteAsync("http://localhost:8283/v1/agents/" + agentId);
        var content = await response.Content.ReadAsStringAsync();
        Debug.Log(content);
        Agent_List_Control dataManager = FindObjectOfType<Agent_List_Control>();
        if (dataManager != null)
        {
            Debug.Log("重新生成按钮");
            agent_List_Control.RefreshAgentListandButton();
            agent详细信息panel.SetActive(false);
            Agent_Panel.SetActive(true);
        }
        else
        {
            Debug.LogError("GetData component not found in the scene.");
        }
        // 设置面板状态


    }

    public void OnChangeAgentINFButtonClicked()
    {
        Debug.Log("修改Agent信息");
        if (Agent_name.text != null || Agent_description.text != null || System_prompt.text != null)
        {
            存储加载agent的相关数据.AddAgentData(Agent_name.text, TTS_opt.options[TTS_opt.value].text, 人物模型.options[人物模型.value].text);
            WaitingPanel.SetActive(true);
            _ = Change_Agent_INF(Agent_name.text, Agent_description.text, System_prompt.text);
        }
        else
        {
            Debug.Log("请输入修改内容");
        }
    }
    async Task Change_Agent_INF(string Newname, string Newdescription, string Newsystem_promote)
    {
        Debug.Log("发送修改请求");
        string AgentID = 获取agent.GetIdByName(Agent_List_Control.user_chioce);
        using var client = new HttpClient();
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), "http://localhost:8283/v1/agents/" + AgentID);
        request.Headers.Add("Authorization", "Bearer ");

        var body = new
        {
            name = Newname,
            system = Newsystem_promote,
            description = Newdescription
        };

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        Debug.Log(responseContent);
        Debug.Log("修改成功");
        WaitingPanel.SetActive(false);
        agent详细信息panel.SetActive(false);
        Agent_Panel.SetActive(true);
    }
    public void Full_placeholder()
    {
        Agent_name.placeholder.GetComponent<Text>().text = "当前名称为" + Agent_List_Control.user_chioce;
        Agent_description.placeholder.GetComponent<Text>().text = "当前描述为" + 获取agent.GetDescriptionByName(Agent_List_Control.user_chioce);
        System_prompt.placeholder.GetComponent<Text>().text = "当前系统提示为" + 获取agent.GetSystemByName(Agent_List_Control.user_chioce);
    }

    public void OnChangeTTSButtonClicked()
    {
        int selectedIndex = TTS_opt.value;
        TMP_Dropdown.OptionData selectedOption = TTS_opt.options[selectedIndex];
        Debug.Log("更改TTS为 " + selectedOption.text);
        存储加载agent的相关数据.ModifyJson(Agent_List_Control.user_chioce, "Tts", selectedOption.text);
    }
    public void OnChange人物模型ButtonClicked()
    {
        int selectedIndex = 人物模型.value;
        Dropdown.OptionData selectedOption = 人物模型.options[selectedIndex];
        存储加载agent的相关数据.ModifyJson(Agent_List_Control.user_chioce, "Model", selectedOption.text);
    }

    public void FullTTSdropdown()
    {
        StartCoroutine(GetRequest());
    }

    IEnumerator GetRequest()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get("http://127.0.0.1:5000/models/get_local?root_dir=Data");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            Dictionary<string, List<string>> data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonResponse);

            // 清空下拉列表的选项
            TTS_opt.ClearOptions();

            // 将字典的键添加到下拉列表中
            List<string> list = new(data.Keys);
            List<string> options = list;
            TTS_opt.AddOptions(options);
        }
        else
        {
            Debug.LogError("Failed to fetch data: " + webRequest.error);
        }
    }

    async Task TTS_api_test(string TTS_name)
    {
        select_TTS_id = await Bert_api.GetLocalModels(TTS_name);
        await Bert_api.GetVoice("你好，hello，こんにちは", select_TTS_id);
    }
    #endregion

    public async void Buttom_select_agent()
    {
        WaitingPanel.SetActive(true);
        agent详细信息panel.SetActive(false);
        Agent_Panel.SetActive(false);
        进程.text = "正在加载人物模型";

        if (加载的人物模型!= null)
        {
            Destroy(加载的人物模型);
        }
        Debug.Log("用户选择的agent名字");
        Debug.Log(Agent_List_Control.user_chioce);
        select_agent_id = 获取agent.GetIdByName(Agent_List_Control.user_chioce);
        Debug.Log("selsct按钮的agent id");
        Debug.Log(select_agent_id);
        var Agent_detail = 存储加载agent的相关数据.LoadAgentDataByName(Agent_List_Control.user_chioce);
        Debug.Log((string)Agent_detail["Model"]);
        Debug.Log((string)Agent_detail["Tts"]);
        select_人物模型_name = (string)Agent_detail["Model"];
        加载的人物模型 = loadFBXFromBundle.LoadModelFromBundle(select_人物模型_name);
        Animation basemodel = 加载的人物模型.AddComponent<Animation>();
        
        cameraController.targetObject= 加载的人物模型;
        cameraController.MoveCameraToFrontOfObject();

        basemodel.wrapMode = WrapMode.Once;
        basemodel.playAutomatically = true;
        basemodel.animatePhysics= true;

        BlinkController挂载.SetActive(true);
        blinkController.skinnedMeshRenderer = 加载的人物模型.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        blinkController.Imitalte();

        AudioLipSync挂载.SetActive(true);
        lipSync.Setbodyskin(加载的人物模型.transform.Find("Body").GetComponent<SkinnedMeshRenderer>());

        animationLoader.LoadBaseModel(加载的人物模型);
        Animator Animator人物模型 = 加载的人物模型.GetComponent<Animator>();
        Animator人物模型.runtimeAnimatorController = animatorController;
        Animator人物模型.enabled = true;
        animationLoader.animator= Animator人物模型;
        main_Prosess.加载的人物amiator = Animator人物模型;

        进程.text = "正在加载TTS和T2M";
        await TTS_api_test((string)Agent_detail["Tts"]);
        Main_Panel.SetActive(true);
        WaitingPanel.SetActive(false);
    }
}
