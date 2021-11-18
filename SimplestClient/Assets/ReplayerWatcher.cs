using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayerWatcher : MonoBehaviour
{
    string m_OurOpponentPlayerName = "Opponent", m_OurPlayerName = "Player";
    public List<string> ReplayofNames;
    public List<Text> GrideSpace = new List<Text>();
    public NetworkedClient m_MessageReceiverFromServer = null;
   // public Text RePlayer_Text, RePlayer_Opponent_Text, RePlayer_Player_Text = null;

    public GameObject m_Network = null;
    public GameObject m_SystemManager = null;
    public GameObject m_ForwardButton , m_BackwardButton = null;
    public GameObject m_RePlayer_Text, m_RePlayer_Opponent_Text, m_RePlayer_Player_Text = null;
    public GameObject m_dropdown = null;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "RecordRequest_InputField")
                m_Network = go;
            else if (go.name == "RecordRequest_CreateButton")
                m_ForwardButton = go;
            else if (go.name == "RecordRequest_TitleText")
                m_BackwardButton = go;
            else if (go.name == "Network")
                m_Network = go;
            else if (go.name == "SystemManagerObject")
                m_SystemManager = go;
            else if (go.name == "SystemManagerObject")
                m_RePlayer_Text = go;
            else if (go.name == "SystemManagerObject")
                m_RePlayer_Opponent_Text = go;
            else if (go.name == "SystemManagerObject")
                m_RePlayer_Player_Text = go;
            else if (go.name == "SystemManagerObject")
                m_dropdown = go;
        }

        m_MessageReceiverFromServer = m_Network.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += ReplayerWatcherReceived;
        }
        m_dropdown.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { LoadDropDownChanged(); });
        m_BackwardButton.GetComponent<Button>().onClick.AddListener(BackwardButtonPressed);
        m_ForwardButton.GetComponent<Button>().onClick.AddListener(ForwardButtonPressed);
    }

    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {

            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= ReplayerWatcherReceived;
        }

    }

    public void ReplayerWatcherReceived(int signifier, string s, TicTacToeBoard t)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.CreateARecoredSuccess:
                //DisplayRequestFromRecordResults(true);
                break;
            case ServerToClientSignifiers.CreateARecoredFail:
                //DisplayRequestFromRecordResults(false);
                break;
            case ServerToClientSignifiers.LoginComplete:
                //ResetRecordRequest();
                break;
            case ServerToClientSignifiers.ReMatchOfTicTacToeComplete:
               // ResetRecordRequest();
                break;
        }
    }
    public void LoadDropDownChanged()
    {
       
    }
    public void SetDropDownChanged()
    {
        m_dropdown.GetComponent<Dropdown>().options.Clear();
        foreach (string s in ReplayofNames)
        {
            m_dropdown.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = s });
        }
    }
    public void BackwardButtonPressed()
    {
       
    }
    public void ForwardButtonPressed()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
