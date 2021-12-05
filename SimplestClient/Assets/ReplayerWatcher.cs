using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayerWatcher : MonoBehaviour
{
   
    #region GameObject
    public GameObject network = null;
    public GameObject system_manager = null;
    public GameObject forward_button, backward_button = null;
    public GameObject replayer_text, replayer_second_player_text, replayer_first_player_text = null;
    public GameObject drop_down = null;
    public List<Text> visual_board_text = new List<Text>();
    public NetworkedClient message_receiver_from_server = null;
    #endregion


    #region variables
    string our_second_player_name = "Playertwo", our_first_player_name = "Playerone";
    public List<string> record_names;
    public List<MatchData> match_datas;
    private int[] virtual_board = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    bool has_selected_record = false;
    int maximum_move,minimum_move = 0, selected_move = 0;
    #endregion


    // Start is called before the first frame update
    void Start()
    {    // find  our game object by name
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

        // initializing

        match_datas = new List<MatchData>();

        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += ReplayerWatcherReceivedMsgFromTheServer;
        }

        // set up the dropmenu and buttons (forward,Backward) to the fuctions
        drop_down.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { LoadDropDownChanged(); });
        backward_button.GetComponent<Button>().onClick.AddListener(BackwardButtonPressed);
        forward_button.GetComponent<Button>().onClick.AddListener(ForwardButtonPressed);

        
    }

    private void OnDestroy()
    {
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= ReplayerWatcherReceivedMsgFromTheServer;
        }

    }

    public void ReplayerWatcherReceivedMsgFromTheServer(int signifier, string s, TicTacToeBoard t, MatchData matchData)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.StartSendAllRecoredsName:

                ///- stop player from using the Interface
                //- rest ReplayerWatcher
                //- has_selected_record is set faluse, so the player can't use the buttons(forward&backward) until you get match data
              
                DisableAllInterface();
                ResetReplayerWatcher(true, true);
                has_selected_record = false;

                break;
            case ServerToClientSignifiers.NoRecordsNamefound:

                //- if you have no record we going to disable All interface
                //- ResetReplayerWatcher so you don't see prev player records

                DisableAllInterface();
                ResetReplayerWatcher(true, true);

                break;
            case ServerToClientSignifiers.SendAllRecoredsNameData:
                // get the record names from the server
                LoadRecordNamesFromServer(s);
                break;
            case ServerToClientSignifiers.DoneSendAllRecoredsName:

                // - upate drop_down with all record name as options.
                SetDropDownChanged();
                // let player use the interface after we got the record names
                ReenbleAllInterface();
                //- this here so you can use drop_down if you only have one record name 
                drop_down.GetComponent<Dropdown>().value = -1;
                break;
            case ServerToClientSignifiers.StartSendThisRecoredMatchData:
                //- we don't want the player playing the interface until we done load the data
                DisableAllInterface();
                //- rest board but not the drop_down options
                ResetReplayerWatcher(true, false);
                break;
            case ServerToClientSignifiers.SendAllThisRecoredMatchData:
                //- just get match data from server
                LoadTheMatchDataFromServer(matchData);
                break;
            case ServerToClientSignifiers.DoneSendAllThisRecoredMatchData :
                //- we done load the match data from and now we let player use interface again
                ReenbleAllInterface();
                break;
        }
    }

    /// <summary>
    /// SetDropDownChanged()
    /// - set up drop_down's option with list of recordnames from server
    /// </summary>
    public void SetDropDownChanged()
    {
        foreach (string rn in record_names)
        {
            drop_down.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = rn });
        }
    }

    /// <summary>
    /// BackwardButtonPressed()
    /// - we don't want go out of bound of the match_data that why we have the if statment, minimum_move and has_selected
    /// - it's button call and moves us to the prev state of game
    /// - we reset the VirtualBord{0,0,0,0,0,0,0,0,0}
    /// - we call MoveThroughtMatchData to change the elements in VirtualBord by selected_move
    /// - then we display the board
    /// </summary>

    public void BackwardButtonPressed()
    {
        Debug.Log("BackwardButton Pressed is called > ");
        //m_RePlayer_Text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + SelectedMove.ToString();
        if (selected_move > minimum_move && has_selected_record)
        {
            ResetVirtualBord();
            selected_move = selected_move - 1;
            MoveThroughtMatchData(selected_move);
            DisplayBoard();
        }
    }

    /// <summary>
    /// ForwardButtonPressed()
    /// - we don't want go out of bound of the match_data that why we have the if statment, maximum_move and has_selected
    /// - it's button call and moves use the next state of game
    /// - we reset the VirtualBord{0,0,0,0,0,0,0,0,0}
    /// - we call MoveThroughtMatchData to change the elements in VirtualBord by selected_move
    /// - then we display the board
    /// </summary>
    public void ForwardButtonPressed()
    {
        Debug.Log("ForwardButton Pressed is called > " );
        if (selected_move < maximum_move && has_selected_record) 
        {
            ResetVirtualBord();
            selected_move = selected_move + 1;
            MoveThroughtMatchData(selected_move);
            DisplayBoard();
        }
    }

    /// <summary>
    /// LoadRecordNamesFromServer()
    /// - we get the Record Names From Server
    /// - place in it list of string (record_names)
    /// </summary>
    /// <param name="record"></param>
    
    private void LoadRecordNamesFromServer(string record) 
    {
        record_names.Add(record);
    }

    /// <summary>
    /// LoadTheMatchDataFromServer()
    /// - we get  the match data from server
    /// - pleace it in list of match data (match_data)
    /// - set maximum  to outer bound of list of match data.
    /// - get playerone's name and playertwo's name.
    /// </summary>
    /// <param name="match_data"></param>
   
    private void LoadTheMatchDataFromServer(MatchData match_data)
    {
        match_datas.Add(match_data);
        maximum_move = match_datas.Count;

        //- get the player names of from match data

        if(maximum_move == 1 && match_data.PlayerSymbol == 1) 
        {
            our_first_player_name = match_data.Playername.ToString();
        }
        else if(maximum_move == 2 && match_data.PlayerSymbol == 2) 
        {
            our_second_player_name = match_data.Playername.ToString();
        }
    }

    /// <summary>
    /// MoveThroughtMatchData
    /// -how we iterater throught matchdatas
    /// - also we pass data into DisplayWhoTurnIt
    /// - display what move we are on
    /// </summary>
    /// <param name="move"></param>
    private void MoveThroughtMatchData(int move) 
    {
     
         MatchData temp_match_data = new MatchData("TempMatchData",0,3);

        for (int i = 0; i < move; i++) 
        {
             temp_match_data = match_datas[i];
           virtual_board[ match_datas[i].Positoin] = match_datas[i].PlayerSymbol;
        }
        

        DisplayWhoTurnIt(temp_match_data, replayer_first_player_text.GetComponent<Text>(), replayer_second_player_text.GetComponent<Text>());
        replayer_text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + selected_move.ToString();
    }


    /// <summary>
    /// ResetVirtualBord()
    /// - a simple way to reset the ResetVirtualBord
    /// </summary>
    private void ResetVirtualBord()
    {
        for (int element = 0; element < virtual_board.Length; element++)
        {
            virtual_board[element] = 0;
        }
    }

    private void ResetReplayerWatcher(bool is_load_new_match_data, bool is_load_new_record_List)
    {
        // clean out the list record names in the dropdown
        if (is_load_new_record_List)
        {
            record_names.Clear();
            drop_down.GetComponent<Dropdown>().options.Clear();
            drop_down.GetComponent<Dropdown>().ClearOptions();
        }
        // reset the interface and display the interface.( just the board )
        if (is_load_new_match_data)
        {
            match_datas.Clear();
            ResetVirtualBord();
            selected_move = 0;
            // pass a fake match data
            DisplayWhoTurnIt(new MatchData("TempMatchData", 0, 3), replayer_first_player_text.GetComponent<Text>(), replayer_second_player_text.GetComponent<Text>());
            selected_move = 0;
            replayer_text.GetComponent<Text>().text = "Replayer " + "\n  Move :" + selected_move.ToString();
            DisplayBoard();
        }

    }

    #region Interfacecode
    /// <summary>
    ///  DisplayWhoTurnIt
    /// </summary>
    /// 
    void DisplayWhoTurnIt(MatchData match_data, Text first_player, Text second_player)
    {
        if (match_data.PlayerSymbol == 3)
        {
            first_player.text = "Player:";
            first_player.color = Color.black;
            second_player.color = Color.black;
            second_player.text = "Opponent:";
        }
        else if (match_data.PlayerSymbol == 1)
        {
            first_player.text = our_first_player_name + ":  Moved";
            first_player.color = Color.blue;
            second_player.color = Color.black;
            second_player.text = our_second_player_name + ": Waiting";
        }
        else if (match_data.PlayerSymbol == 2)
        {
            first_player.text = our_first_player_name + ": Waiting";
            first_player.color = Color.black;
            second_player.color = Color.blue;
            second_player.text = our_second_player_name + ": Moved";
        }
    }
    // ReenbleAllInterface()
    //-make the Interface interactable
    private void ReenbleAllInterface()
    {
        drop_down.GetComponent<Dropdown>().interactable = true;
        forward_button.GetComponent<Button>().interactable = true;
        backward_button.GetComponent<Button>().interactable = true;
    }
    // display what on the virtual board
    public void DisplayBoard()
    {
        for (int i = 0; i < virtual_board.Length; i++)
        {
            if (virtual_board[i] == 0)
            {
                visual_board_text[i].text = "";
            }
            else if (virtual_board[i] == 1)
            {
                visual_board_text[i].text = "X";
            }
            else if (virtual_board[i] == 2)
            {
                visual_board_text[i].text = "O";
            }
        }
    }

    public void LoadDropDownChanged()
    {
        //- get option as  a string 
        int menuIndex = drop_down.GetComponent<Dropdown>().value;
        List<Dropdown.OptionData> menuOptions = drop_down.GetComponent<Dropdown>().options;
        string value = menuOptions[menuIndex].text;
        //send request to server for  match data underneath recordname (option)
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AskForThisRecoredMatchData + "," + value);
        //- ResetReplayerWatcher 
        ResetReplayerWatcher(true, false);
        // - you should be use button (forward&Backward) now you have selected record
        has_selected_record = true;
    }

    /// <summary>
    /// DisableAllInterface()
    /// -stop player from using interface in crucial moment. forexample: when we are getting data from the server.
    /// </summary>
    private void DisableAllInterface()
    {
        drop_down.GetComponent<Dropdown>().interactable = false;
        forward_button.GetComponent<Button>().interactable = false;
        backward_button.GetComponent<Button>().interactable = false;
    }

    #endregion
}

#region Protocol
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
#endregion