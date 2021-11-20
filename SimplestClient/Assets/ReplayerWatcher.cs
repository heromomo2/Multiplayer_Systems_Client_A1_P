using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayerWatcher : MonoBehaviour
{
    string m_OurSecondPlayerName = "Playertwo", m_OurFirstPlayerName = "Playerone";
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
    public GameObject m_RePlayer_Text, m_RePlayer_SecondPlayer_Text, m_RePlayer_FirstPlayer_Text = null;
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
                m_RePlayer_SecondPlayer_Text = go;
            else if (go.name == "Replay_Player_Text ")
                m_RePlayer_FirstPlayer_Text = go;
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
        MatchDatas = new List<MatchData>();
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
                Reset5(true, true);
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
                Reset5(true, false);
                break;
            case ServerToClientSignifiers.SendAllThisRecoredMatchData:
                GetMataData(matchData);
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
        //DisplayWhoturn( new MatchData("TempMatchData", 0, 3), m_RePlayer_FirstPlayer_Text.GetComponent<Text>(), m_RePlayer_SecondPlayer_Text.GetComponent<Text>());
        //SelectedMove = 0;
        //m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        //ResetMBord();
        //CallToDisplayBoard();
        Reset5(true, false);

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
        Debug.Log("BackwardButton Pressed is called > ");
        //m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        if (SelectedMove > m_minimumMove )
        {
            ResetMBord();
            SelectedMove = SelectedMove - 1;
            MoveThroughtMatchData(SelectedMove);
            CallToDisplayBoard();
        }
    }
    public void ForwardButtonPressed()
    {
        Debug.Log("ForwardButton Pressed is called > " );
       // m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        if (SelectedMove < m_maximumMove) 
        {
            ResetMBord();
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
        m_maximumMove = MatchDatas.Count;

        if(m_maximumMove == 1 && matchData.PlayerSymbol == 1) 
        {
            m_OurFirstPlayerName = matchData.Playername.ToString();
        }
        else if(m_maximumMove == 2 && matchData.PlayerSymbol == 2) 
        {
            m_OurSecondPlayerName = matchData.Playername.ToString();
        }
    }

    private void MoveThroughtMatchData(int Move) 
    {
      
         MatchData TempmatchData = new MatchData("TempMatchData",0,3);
        for (int i = 0; i < Move; i++) 
        {
             TempmatchData = MatchDatas[i];
           mboard[ MatchDatas[i].Positoin] = MatchDatas[i].PlayerSymbol;
        }
        //mboard[TempmatchData.Positoin] = TempmatchData.PlayerSymbol;

        DisplayWhoturn(TempmatchData, m_RePlayer_FirstPlayer_Text.GetComponent<Text>(), m_RePlayer_SecondPlayer_Text.GetComponent<Text>());
        m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
    }



    private void ResetMBord()
    {
        for (int element = 0; element < mboard.Length; element++)
        {
            mboard[element] = 0;
        }
    }
    void DisplayWhoturn(MatchData m, Text FirstPlayer, Text SecondPlayer)
    {
        if (m.PlayerSymbol == 3)
        {
            FirstPlayer.text = "Player:";
            FirstPlayer.color = Color.black;
            SecondPlayer.color = Color.black;
            SecondPlayer.text = "Opponent:";
        }
        else if (m.PlayerSymbol == 1)
        {
            FirstPlayer.text = m_OurFirstPlayerName + ":  Moved";
            FirstPlayer.color = Color.blue;
            SecondPlayer.color = Color.black;
            SecondPlayer.text = m_OurSecondPlayerName + ": Waiting";
        }
        else if (m.PlayerSymbol == 2)
        {
            FirstPlayer.text = m_OurFirstPlayerName + ": Waiting";
            FirstPlayer.color = Color.black;
            SecondPlayer.color = Color.blue;
            SecondPlayer.text = m_OurSecondPlayerName + ": Moved";
        }
    }

    private void Reset5(bool IsLoadNewMatchdata, bool IsLoadNewRecordList)
    {

        if (IsLoadNewRecordList)
        {
            RecordNames.Clear();
            m_dropdown.GetComponent<Dropdown>().options.Clear();
        }
        else if (IsLoadNewMatchdata)
        {
            MatchDatas.Clear();
            DisplayWhoturn(new MatchData("TempMatchData", 0, 3), m_RePlayer_FirstPlayer_Text.GetComponent<Text>(), m_RePlayer_SecondPlayer_Text.GetComponent<Text>());
            SelectedMove = 0;
            m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
            ResetMBord();
            CallToDisplayBoard();
        }
       

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