using UnityEngine;

public class Swich_panel : MonoBehaviour
{
    private void Start()
    {
        Main_Panel.SetActive(true);
        Agent_Panel.SetActive(false); 
        Creat_agent_Panel.SetActive(false);
        agent详细信息panel.SetActive(false);
        Wait_panel.SetActive(false);
        Setting_Panel.SetActive(false);
        TTS_OPT_Panel.SetActive(false);
        TD_Model_OPT_Panel.SetActive(false);
        BlinkController挂载.SetActive(false);
        AudioLipSync挂载.SetActive(false);
    }
    public GameObject Agent_Panel;
    public GameObject Main_Panel;
    public GameObject Creat_agent_Panel;
    public GameObject Wait_panel;
    public GameObject agent详细信息panel;
    public GameObject Setting_Panel;
    public GameObject TTS_OPT_Panel;
    public GameObject TD_Model_OPT_Panel;
    public GameObject BlinkController挂载;
    public GameObject AudioLipSync挂载;
    public void Agent_buttom()
    {
        Agent_Panel.SetActive(!Agent_Panel.activeSelf);
        Main_Panel.SetActive(!Main_Panel.activeSelf);
    }
    public void Creat_agent_buttom()
    {
        Creat_agent_Panel.SetActive(!Creat_agent_Panel.activeSelf);
    }
    public void Setting_buttom()
    {
        Setting_Panel.SetActive(!Setting_Panel.activeSelf);
    }
    public void TTS_OPT_Panel_buttom()
    {
        TTS_OPT_Panel.SetActive(!TTS_OPT_Panel.activeSelf);
    }
    public void TD_Model_OPT_Panel_buttom()
    {
        TD_Model_OPT_Panel.SetActive(!TD_Model_OPT_Panel.activeSelf);
    }
}
