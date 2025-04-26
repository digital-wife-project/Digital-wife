using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Agent_List_Control : MonoBehaviour
{
    public GameObject Agent_Panel;
    public GameObject agent详细信息panel;
    public static string user_chioce;
    public GameObject TMP按钮预制体; // TMP按钮预制体，需要在Inspector中赋值
    public Transform list_agent面板Transform; // 面板Transform，需要在Inspector中赋值
    public float spacing = 10f; // 按钮之间的间隔
    public GameObject list_panel;
    public Agent操作 agent操作;
    public 获取agent信息 getagent信息;

    void Start()
    {
        _ = Bert_api.DeleteModels();
        RefreshAgentListandButton();
    }
    //创建以及刷新按钮列表
    public void RefreshAgentListandButton()
    {
        DeleteAllTMPButtons();
        StartCoroutine(getagent信息.GetAgentData());
        string jsonData = getagent信息.Read_json2string();
        Debug.Log("刷新json");
        if (jsonData != null)
        {
            // 获取唯一ID的数量
            _ = getagent信息.CountUniqueIds(jsonData);
            // 获取唯一的Agent名称列表
            HashSet<string> uniqueNames = getagent信息.CountAgentNames(jsonData);
            // 将HashSet转换为List并排序
            List<string> sortedNames = uniqueNames.ToList();
            sortedNames.Sort();
            // 刷新按钮
            CreateButtons(sortedNames);
        }
        else
        {
            Debug.LogError("Failed to read JSON data.");
        }
    }
    #region
    // 创建按钮的方法
    void CreateButtons(List<string> sortedNames)
    {
        // 获取按钮的宽度
        float buttonWidth = TMP按钮预制体.GetComponent<RectTransform>().sizeDelta.x;
        // 获取按钮的高度
        float buttonHeight = TMP按钮预制体.GetComponent<RectTransform>().sizeDelta.y;
        // 计算按钮的总宽度
        float totalWidth = buttonWidth + spacing;
        // 每行按钮数量
        int buttonsPerRow = 4;
        // 计算最大列数
        int maxColumns = Mathf.CeilToInt((float)sortedNames.Count / buttonsPerRow);

        // 获取面板的RectTransform组件
        RectTransform panelRectTransform = list_agent面板Transform.GetComponent<RectTransform>();
        Debug.Log("刷新按钮");

        // 遍历排序后的名字列表
        for (int i = 0; i < sortedNames.Count; i++)
        {
            // 获取当前名字
            string name = sortedNames[i];

            // 实例化按钮
            GameObject newButton = Instantiate(TMP按钮预制体, list_agent面板Transform);
            // 设置按钮的名称
            newButton.name = "AgentButton" + name;

            // 获取按钮上的文本组件
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            // 如果文本组件存在，设置文本内容
            if (buttonText != null)
            {
                buttonText.text = "Agent " + name;
            }
            // 如果文本组件不存在，输出错误信息
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on button: " + newButton.name);
            }

            // 计算按钮所在的行和列
            int row = i / buttonsPerRow;
            int column = i % buttonsPerRow;
            // 获取按钮的RectTransform组件
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            // 计算按钮的位置，从左上角开始排列
            Vector3 position = new(column * totalWidth,
                                   -row * (buttonHeight + spacing),
                                   0);
            // 设置按钮的位置
            rectTransform.anchoredPosition = position;

            // 尝试获取按钮的Button组件
            if (newButton.TryGetComponent<Button>(out var buttonComponent))
            {
                // 记录按钮的索引
                string buttonIndex = name;
                // 添加按钮点击事件
                buttonComponent.onClick.AddListener(() => ButtonClicked(buttonIndex));
            }
            // 如果Button组件不存在，输出错误信息
            else
            {
                Debug.LogError("Button component not found on button: " + newButton.name);
            }
        }
    }
    //使用这个方法来定义agent按钮点击后引发的事件
    void ButtonClicked(string buttonIndex)
    {
        user_chioce = buttonIndex;
        Debug.Log("Button for Agent " + buttonIndex + " clicked!");
        agent操作.Full_placeholder();
        Agent_Panel.SetActive(false);
        agent详细信息panel.SetActive(true);
    }
    //使用这个方法来发送get请求
    // 调用这个方法来删除所有按钮
    public void DeleteAllTMPButtons()
    {
        // 检查面板是否有效
        if (list_panel != null)
        {
            // 获取面板下的所有子对象
            Button[] buttons = list_panel.GetComponentsInChildren<Button>();

            // 遍历所有子对象
            foreach (Button button in buttons)
            {
                // 检查按钮是否包含TMP文本组件
                TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    // 销毁按钮所在的GameObject
                    Destroy(button.gameObject);
                }
            }
            Debug.Log("删除所有TMP按钮");
        }
        else
        {
            Debug.LogError("Panel reference is not set!");
        }
    }
#endregion
}
