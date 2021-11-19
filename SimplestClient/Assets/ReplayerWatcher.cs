using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayerWatcher : MonoBehaviour
{
    string m_OurOpponentPlayerName = "Opponent", m_OurPlayerName = "Player";
    public List<string> RecordNames;
    public List<MatchData> MatchDatas;
    public List<Text> GrideSpace = new List<Text>();
    public NetworkedClient m_MessageReceiverFromServer = null;
    private int[] mboard = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int m_maximumMove,m_minimumMove = 0, SelectedMove = 0;

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
            else if (go.name == "Replay_ForwardButton")
                m_ForwardButton = go;
            else if (go.name == "Replay_BackwardButton")
                m_BackwardButton = go;
            else if (go.name == "Network")
                m_Network = go;
            else if (go.name == "SystemManagerObject")
                m_SystemManager = go;
            else if (go.name == "Replay_TitleText")
                m_RePlayer_Text = go;
            else if (go.name == "Replay_Opponent_Text")
                m_RePlayer_Opponent_Text = go;
            else if (go.name == "Replay_Player_Text ")
                m_RePlayer_Player_Text = go;
            else if (go.name == "Replayer_Dropdown")
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

    public void ReplayerWatcherReceived(int signifier, string s, TicTacToeBoard t, MatchData matchData)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.StartSendAllRecoredsName:
                DisableAllInterface();
                break;
            case ServerToClientSignifiers.DoneSendAllRecoredsName:
                SetDropDownChanged();
                ReEnbleAllInterface();
                break;
            case ServerToClientSignifiers.SendAllRecoredsNameData:
                GetRecordNames(s);
                break;
            case ServerToClientSignifiers.StartSendThisRecoredMatchData:
                DisableAllInterface();
                break;
            case ServerToClientSignifiers.SendAllThisRecoredMatchData:
                DisableAllInterface();
                break;
            case ServerToClientSignifiers.DoneSendAllThisRecoredMatchData :
                ReEnbleAllInterface();
                break;
        }
    }
    public void LoadDropDownChanged()
    {
       int menuIndex = m_dropdown.GetComponent<Dropdown>().value;
        List<Dropdown.OptionData> menuOptions = m_dropdown.GetComponent<Dropdown>().options;
        string value = menuOptions[menuIndex].text;

        m_Network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AskForThisRecoredMatchData + "," + value);
    }
    public void SetDropDownChanged()
    {
        //m_dropdown.GetComponent<Dropdown>().options.Clear();
        foreach (string rn in RecordNames)
        {
            m_dropdown.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = rn });
        }
    }
    public void BackwardButtonPressed()
    {
        if (SelectedMove > m_minimumMove )
        {
            SelectedMove = SelectedMove - 1;
            MoveThroughtMatchData(SelectedMove);
            CallToDisplayBoard();
        }
    }
    public void ForwardButtonPressed()
    {
        if (SelectedMove < m_maximumMove) 
        {
            SelectedMove = SelectedMove + 1;
            MoveThroughtMatchData(SelectedMove);
            CallToDisplayBoard();
        }
    }
    private void CallToDisplayBoard ()
    {
       // TicTacToeBoard t = MoveBoard[SelectedMove];
        DisplaBoard( mboard[0], GrideSpace[0]); DisplaBoard(mboard[1], GrideSpace[1]); DisplaBoard(mboard[2], GrideSpace[2]);
        DisplaBoard(mboard[3], GrideSpace[3]); DisplaBoard(mboard[4] , GrideSpace[4]); DisplaBoard(mboard[5], GrideSpace[5]);
        DisplaBoard(mboard[6], GrideSpace[6]); DisplaBoard(mboard[7], GrideSpace[7]); DisplaBoard(mboard[8], GrideSpace[8]);

       // DisplayWhoturn(t.WhosMove, RePlayer_Player_Text, RePlayer_Opponent_Text);
    }

    void DisplaBoard(int space, Text t)
    {
        if (space == 0)
        {
            t.text = "";
        }
        else if (space == 1)
        {
            t.text = "X";
        }
        else if (space == 2)
        {
            t.text = "O";
        }
    }

    private void DisableAllInterface()
    {
        m_dropdown.GetComponent<Dropdown>().interactable = false;
        m_ForwardButton.GetComponent<Button>().interactable = false;
        m_BackwardButton.GetComponent<Button>().interactable = false;
    }
    private void ReEnbleAllInterface()
    {
        m_dropdown.GetComponent<Dropdown>().interactable =  true;
        m_ForwardButton.GetComponent<Button>().interactable = true;
        m_BackwardButton.GetComponent<Button>().interactable = true;
    }

    private void GetRecordNames(string Record) 
    {
        RecordNames.Add(Record);
    }


    private void GetMataData(MatchData matchData)
    {
        MatchDatas.Add(matchData);
    }

    private void MoveThroughtMatchData(int Move) 
    {
       m_maximumMove = MatchDatas.Count;

        for (int element = 0; element > mboard.Length; element++) 
        {
            mboard[element] = 0;
        }
        MatchData TempmatchData = new MatchData("TempMatchData",0,3);
        for (int i = 0; i < Move; i++) 
        {
            TempmatchData = MatchDatas[i];
        }
        mboard[TempmatchData.Positoin] = TempmatchData.PlayerSymbol;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class MatchData
{
    public int Positoin;
    public int PlayerSymbol;
    public string Playername;

    public MatchData(string playerName, int position, int playerSymbol)
    {
        Positoin = position;
        Playername = playerName;
        PlayerSymbol = playerSymbol;
    }

}