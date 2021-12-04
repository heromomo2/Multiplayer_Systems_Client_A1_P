using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayerWatcher : MonoBehaviour
{
    string our_second_player_name = "Playertwo", our_first_player_name = "Playerone";
    public List<string> record_names;
    public List<MatchData> match_datas;
    public List<Text> gride_space = new List<Text>();
    public NetworkedClient message_receiver_from_server = null;
    private int[] virtual_board = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int maximum_move,minimum_move = 0, selected_move = 0;
    bool has_selected = false;

    // public Text RePlayer_Text, RePlayer_Opponent_Text, RePlayer_Player_Text = null;

    public GameObject network = null;
    public GameObject system_manager = null;
    public GameObject forward_button , backward_button = null;
    public GameObject replayer_text, replayer_second_player_text, replayer_first_player_text = null;
    public GameObject drop_down = null;


    #region GameObject
    #endregion

    #region variables
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "RecordRequest_InputField")
                network = go;
            else if (go.name == "Replay_ForwardButton")
                forward_button = go;
            else if (go.name == "Replay_BackwardButton")
                backward_button = go;
            else if (go.name == "Network")
                network = go;
            else if (go.name == "SystemManagerObject")
                system_manager = go;
            else if (go.name == "Replay_TitleText")
                replayer_text = go;
            else if (go.name == "Replay_Opponent_Text")
                replayer_second_player_text = go;
            else if (go.name == "Replay_Player_Text ")
                replayer_first_player_text = go;
            else if (go.name == "Replayer_Dropdown")
                drop_down = go;
        }

        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += ReplayerWatcherReceived;
        }
        drop_down.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { LoadDropDownChanged(); });
        backward_button.GetComponent<Button>().onClick.AddListener(BackwardButtonPressed);
        forward_button.GetComponent<Button>().onClick.AddListener(ForwardButtonPressed);
        match_datas = new List<MatchData>();
    }

    private void OnDestroy()
    {
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= ReplayerWatcherReceived;
        }

    }

    public void ReplayerWatcherReceived(int signifier, string s, TicTacToeBoard t, MatchData matchData)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.StartSendAllRecoredsName:
                DisableAllInterface();
                Reset5(true, true);
                has_selected = false;
                break;
            case ServerToClientSignifiers.NoRecordsNamefound:
                DisableAllInterface();
                Reset5(true, true);
                break;
            case ServerToClientSignifiers.SendAllRecoredsNameData:
                GetRecordNames(s);
                break;
            case ServerToClientSignifiers.DoneSendAllRecoredsName:
                SetDropDownChanged();
                ReEnbleAllInterface();
                drop_down.GetComponent<Dropdown>().value = -1;
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
       int menuIndex = drop_down.GetComponent<Dropdown>().value;
        List<Dropdown.OptionData> menuOptions = drop_down.GetComponent<Dropdown>().options;
        string value = menuOptions[menuIndex].text;

        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AskForThisRecoredMatchData + "," + value);
        //DisplayWhoturn( new MatchData("TempMatchData", 0, 3), m_RePlayer_FirstPlayer_Text.GetComponent<Text>(), m_RePlayer_SecondPlayer_Text.GetComponent<Text>());
        //SelectedMove = 0;
        //m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        //ResetMBord();
        //CallToDisplayBoard();
        Reset5(true, false);
        has_selected = true;
    }
    public void SetDropDownChanged()
    {
        //m_dropdown.GetComponent<Dropdown>().options.Clear();
        foreach (string rn in record_names)
        {
            drop_down.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = rn });
        }
    }
    public void BackwardButtonPressed()
    {
        Debug.Log("BackwardButton Pressed is called > ");
        //m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        if (selected_move > minimum_move && has_selected)
        {
            ResetMBord();
            selected_move = selected_move - 1;
            MoveThroughtMatchData(selected_move);
            CallToDisplayBoard();
        }
    }
    public void ForwardButtonPressed()
    {
        Debug.Log("ForwardButton Pressed is called > " );
       // m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        if (selected_move < maximum_move && has_selected) 
        {
            ResetMBord();
            selected_move = selected_move + 1;
            MoveThroughtMatchData(selected_move);
            CallToDisplayBoard();
        }
    }
    private void CallToDisplayBoard ()
    {
       // TicTacToeBoard t = MoveBoard[SelectedMove];
        DisplaBoard( virtual_board[0], gride_space[0]); DisplaBoard(virtual_board[1], gride_space[1]); DisplaBoard(virtual_board[2], gride_space[2]);
        DisplaBoard(virtual_board[3], gride_space[3]); DisplaBoard(virtual_board[4] , gride_space[4]); DisplaBoard(virtual_board[5], gride_space[5]);
        DisplaBoard(virtual_board[6], gride_space[6]); DisplaBoard(virtual_board[7], gride_space[7]); DisplaBoard(virtual_board[8], gride_space[8]);

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
        drop_down.GetComponent<Dropdown>().interactable = false;
        forward_button.GetComponent<Button>().interactable = false;
        backward_button.GetComponent<Button>().interactable = false;
    }
    private void ReEnbleAllInterface()
    {
        drop_down.GetComponent<Dropdown>().interactable =  true;
        forward_button.GetComponent<Button>().interactable = true;
        backward_button.GetComponent<Button>().interactable = true;
    }

    private void GetRecordNames(string Record) 
    {
        record_names.Add(Record);
    }


    private void GetMataData(MatchData matchData)
    {
        match_datas.Add(matchData);
        maximum_move = match_datas.Count;

        if(maximum_move == 1 && matchData.PlayerSymbol == 1) 
        {
            our_first_player_name = matchData.Playername.ToString();
        }
        else if(maximum_move == 2 && matchData.PlayerSymbol == 2) 
        {
            our_second_player_name = matchData.Playername.ToString();
        }
    }

    private void MoveThroughtMatchData(int Move) 
    {
      
         MatchData TempmatchData = new MatchData("TempMatchData",0,3);
        for (int i = 0; i < Move; i++) 
        {
             TempmatchData = match_datas[i];
           virtual_board[ match_datas[i].Positoin] = match_datas[i].PlayerSymbol;
        }
        //mboard[TempmatchData.Positoin] = TempmatchData.PlayerSymbol;

        DisplayWhoturn(TempmatchData, replayer_first_player_text.GetComponent<Text>(), replayer_second_player_text.GetComponent<Text>());
        replayer_text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + selected_move.ToString();
    }



    private void ResetMBord()
    {
        for (int element = 0; element < virtual_board.Length; element++)
        {
            virtual_board[element] = 0;
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
            FirstPlayer.text = our_first_player_name + ":  Moved";
            FirstPlayer.color = Color.blue;
            SecondPlayer.color = Color.black;
            SecondPlayer.text = our_second_player_name + ": Waiting";
        }
        else if (m.PlayerSymbol == 2)
        {
            FirstPlayer.text = our_first_player_name + ": Waiting";
            FirstPlayer.color = Color.black;
            SecondPlayer.color = Color.blue;
            SecondPlayer.text = our_second_player_name + ": Moved";
        }
    }

    private void Reset5(bool IsLoadNewMatchdata, bool IsLoadNewRecordList)
    {

        if (IsLoadNewRecordList)
        {
            record_names.Clear();
            drop_down.GetComponent<Dropdown>().options.Clear();
            drop_down.GetComponent<Dropdown>().ClearOptions();
        }
        if (IsLoadNewMatchdata)
        {
            match_datas.Clear();
            ResetMBord();
            selected_move = 0;
            DisplayWhoturn(new MatchData("TempMatchData", 0, 3), replayer_first_player_text.GetComponent<Text>(), replayer_second_player_text.GetComponent<Text>());
            selected_move = 0;
            replayer_text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + selected_move.ToString();
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