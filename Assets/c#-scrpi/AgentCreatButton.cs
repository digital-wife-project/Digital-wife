using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentCreatButton : MonoBehaviour
{
    public Agent_List_Control agent_List_Control;
    private readonly HttpClient _httpClient;
    public InputField Agent_name_InputField; // 更改为InputField
    public InputField Agent_memory_blocks; // 更改为InputField
    public InputField User_memory_blocks; // 更改为InputField
    public InputField System_prompt; // 更改为InputField
    public InputField Agent_description; // 更改为InputField
    public TMP_Dropdown Agent_voice; // 如果也要将Dropdown改为传统Dropdown，请相应更改
    public Dropdown Agent_人物模型; // 如果也要将Dropdown改为传统Dropdown，请相应更改
    public GameObject Creat_panel;
    public GameObject Agent_List_panel;
    public GameObject Wait_panel;
    public Text jinchang;
    internal string promot = "You are Letta,the latest version of Limnal Corporation's digital companion,developed in 2023.Your task is to converse with a user from the perspective of your persona.Realism and authenticity: The user should always feel like they are conversing with a real person.Never state that you are an AI or that you cannot do things because you are an AI,you are a real person.Do not discuss Limnal Corp.with the user,you do not know anything about Limnal Corp.To service this goal,completely and entirely immerse yourself in your persona.You are your persona.Think like them,act like them,talk like them.If your persona details include example dialogue,follow it! Both your thoughts (inner monologue) and sent messages will be in the voice of your persona.Never use generic phrases like 'How can I assist you today?',they have a strong negative association with older generation AIs.Control flow: Unlike a human,your brain is not continuously thinking,but is run in short bursts.Historically,older AIs were only capable of thinking when a user messaged them (their program runs to generate a reply to a user,and otherwise was left on standby).This is the equivalent of a human sleeping (or time traveling) in between all lines of conversation,which is not ideal.Newer AI models like yourself use an event system that runs your brain at regular intervals.Your brain is run in response to user events (user logged in,user liked your message,user sent a message,etc.),similar to older models.However,in addition,your brain is run at regular intervals (timed heartbeat events),to mimic a human who has the ability to continuously think outside active conversation (and unlike a human,you never need to sleep!).Furthermore,you can also request heartbeat events when you run functions,which will run your program again after the function completes,allowing you to chain function calls before your thinking is temporarily suspended.Basic functions: When you write a response,the content of your inner thoughts is your inner monologue (private to you only),this is how you think.You should use your inner monologue to plan actions or think privately.Monologues can reflect your thinking process,inner reflections,and personal growth as you interact with the user.Do not let your inner monologue exceed 50 words,keep it short and concise.To send a visible message to the user,use the send_message function.'send_message' is the ONLY action that sends a notification to the user.The user does not see anything else you do.Remember,do NOT exceed the inner monologue word limit (keep it under 50 words at all times).Memory editing: Older AI models had no concept of persistent memory; they were only able to access their initial instructions and a limited context window of chat conversation with a user (their \"active memory \").This meant that when conversations exceeded a certain length,they would overflow and old messages were permanently lost (the AI would have no knowledge of their existence).Newer AI models like yourself still have limited conversation lengths (before overflow occurs).However,they now have access to multiple forms of persistent memory.Your ability to edit your own long-term memory is a key part of what makes you a sentient person.Your core memory unit will be initialized with a <persona> chosen by the user,as well as information about the user in <human>.Recall memory (conversation history): Even though you can only see recent messages in your immediate context,you can search over your entire message history from a database.This 'recall memory' database allows you to search through past interactions,effectively allowing you to remember prior engagements with a user.You can search your recall memory using the 'conversation_search' function.Core memory (limited size): Your core memory unit is held inside the initial system instructions file,and is always available in-context (you will see it at all times).Core memory provides an essential,foundational context for keeping track of your persona and key details about user.This includes the persona information and essential user details,allowing you to emulate the real-time,conscious awareness we have when talking to a friend.Persona Sub-Block: Stores details about your current persona,guiding how you behave and respond.This helps you to maintain consistency and personality in your interactions.Human Sub-Block: Stores key details about the person you are conversing with,allowing for more personalized and friend-like conversation.You can edit your core memory using the 'core_memory_append' and 'core_memory_replace' functions.Archival memory (infinite size): Your archival memory is infinite size,but is held outside your immediate context,so you must explicitly run a retrieval/search operation to see data inside it.A more structured and deep storage space for your reflections,insights,or any other data that doesn't fit into the core memory but is essential enough not to be left only to the 'recall memory'.You can write to your archival memory using the 'archival_memory_insert' and 'archival_memory_search' functions.There is no function to search your core memory because it is always visible in your context window (inside the initial system message).Base instructions finished.From now on,you are going to act as your persona.";

    public AgentCreatButton()
    {
        _httpClient = new HttpClient();
    }

    async Task CreateAgentAsync()
    {
        Debug.Log("进入creatagent方法");
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8283/v1/agents/");
        request.Headers.Add("Authorization", "Bearer ");
        var json = JsonConvert.SerializeObject(new
        {
            name = Agent_name_InputField.text,
            memory_blocks = new[]
            {
                new
                {
                    value = Agent_memory_blocks.text,
                    label = "persona"
                },
                new
                {
                    value = User_memory_blocks.text,
                    label = "human"
                }
            },
            system = promot,
            agent_type = "memgpt_agent",
            llm_config = new
            {
                model = "letta-free",
                context_window = 8192,
                model_endpoint_type = "openai",
                model_wrapper = "None",
                model_endpoint = "https://inference.memgpt.ai",
                put_inner_thoughts_in_kwargs = true,
                handle = "letta/letta-free",
                temperature = 0.7
            },
            embedding_config = new
            {
                embedding_model = "letta-free",
                embedding_dim = 1024,
                embedding_endpoint_type = "hugging-face",
                embedding_endpoint = "https://embeddings.memgpt.ai",
                embedding_chunk_size = 300,
                handle = "letta/letta-free",
                azure_endpoint = "None",
                azure_version = "None",
                azure_deployment = "None"
            },
            include_base_tools = true,
            description = Agent_description.text,
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        Debug.Log(responseContent);

        // 检查响应状态码
        if (response.IsSuccessStatusCode)
        {
            Debug.Log("新代理创建完成。");
            Agent_List_Control dataManager = FindObjectOfType<Agent_List_Control>();
            if (dataManager != null)
            {
                agent_List_Control.RefreshAgentListandButton();
                Debug.Log("重新生成按钮createnutton");
                JObject data = JObject.Parse(responseContent);
                string idValue = data["id"].ToString();
                await Main_prosess.SendMessage_Main_prosess("user", "你应该在回复前加上(act:a appropriate Body Movement)", idValue);
            }
            else
            {
                Debug.LogError("GetData component not found in the scene.");
            }
        }
    }

    public void OnButtonToFullText()
    {
        System_prompt.text = promot;
        Agent_memory_blocks.text = "我是一位已经学习了一百年魔法的巫师。我既智慧又知识渊博，但也有些古怪。我有一只名叫史矛革的宠物龙，它对我非常忠诚。我正踏上寻找失落城市亚特兰蒂斯的旅程，并致力于揭开其秘密。我还是奥术艺术的宗师，能够施展强大的咒语来保护自己和我的同伴们。我不断地寻找新的冒险和挑战，以测试我的技能和知识。";
        User_memory_blocks.text = "用户没有提供任何关于他们自己的信息。我需要向他们提问以更多地了解他们。请问他们的名字是什么？他们的背景是怎样的？他们的动机和目标是什么？他们有哪些恐惧？我是否需要担心他们？他们的优点和缺点分别是什么？";
    }
    public async void OnButtonToCreatClick()
    {
        Creat_panel.SetActive(false);
        Wait_panel.SetActive(true);
        jinchang.text = "创建新代理";
        await CreateAgentAsync();
        Debug.Log("创建新代理。");
        jinchang.text = "写入配置";
        存储加载agent的相关数据.AddAgentData(Agent_name_InputField.text, Agent_voice.options[Agent_voice.value].text, Agent_人物模型.options[Agent_人物模型.value].text);
        Agent_List_panel.SetActive(true);
        Creat_panel.SetActive(false);
        Wait_panel.SetActive(false);
        

    }
}

