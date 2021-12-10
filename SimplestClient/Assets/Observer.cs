using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Observer : MonoBehaviour
{
    [Header(" others")]
    #region GameObjects
    public NetworkedClient message_receiver_from_Server = null;
    GameObject network, system_manger_object;
    #endregion

    [Header("Searcher")]
    #region Searcher Gameobject/variables
    public GameObject observer_search_leave_button;
    public GameObject observer_search;
    public GameObject observer_search_inputfield;
    public GameObject observer_search_result_text;
    public GameObject observer_search_button;
    string user_name_search_for = "";
    #endregion

    [Header("Watcher")]
    #region Watcher Gameobject/variables
    public GameObject observer_Watcher_logot_button;
    public GameObject observer_watcher;
    public List<Text> grid_spaces_observer_watcher;
    public TicTacToeBoard board_observer_watcher = null;
    public GameObject observer_watcher_player_text, observer_watcher_opponent_text = null;
    string user_name_other = "";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "SystemManagerObject")
                system_manger_object = go;
            else if (go.name == "Network")
                network = go;
            else if (go.name == "Observer_Leave_Button")
                observer_search_leave_button = go;
            else if (go.name == "Observer_Search_UI")
                observer_search = go;
            else if (go.name == "Observer_Search_InputField")
                observer_search_inputfield = go;
            else if (go.name == "Observer_Search_Button")
                observer_search_button = go;
            else if (go.name == "Observer_Search_Result_Text")
                observer_search_result_text = go;
            else if (go.name == "Observer_Watcher_LogOut_Button")
                observer_Watcher_logot_button = go;
            else if (go.name == "Observer_Watcher_UI")
                observer_watcher = go;
            else if (go.name == "Observer_watcher_Player_Text ")
                observer_watcher_player_text = go;
            else if (go.name == "Observer_watcher_Opponent_Text")
                observer_watcher_opponent_text = go;
        }
        
        // initializing 
        message_receiver_from_Server = network.GetComponent<NetworkedClient>();
        


        ///  - check message_receiver_from_Server isn't null
        ///  -// this line of code just register ObserverReceivedFuntion from the action.
        ///  ObserverReceivedFuntion will  get call by the action and will get any data pas into it.


        if (message_receiver_from_Server != null)
        {
            message_receiver_from_Server.OnMessageReceivedFromServer += ObserverReceivedFromServer;
        }

        /// get the textobject and place them in the list (grid_spaces_observer_watcher)
        grid_spaces_observer_watcher = new List<Text>();

        for (int i = 0; i <= 9; i++)
        {
            
            GameObject temp_game_object;

            temp_game_object = GameObject.Find("Observer_Gride_Space_Text"+ i);
            Debug.Log("Observer_Gride_Space_Text :" + i);
            if (temp_game_object != null)
            {
                grid_spaces_observer_watcher.Add(temp_game_object.GetComponent<Text>());
            }
        }

        observer_Watcher_logot_button.GetComponent<Button>().onClick.AddListener(ObserverWatcherLogoutButtonIsPressed);
        observer_search_leave_button.GetComponent<Button>().onClick.AddListener(ObserverSearchLeaveButtonIsPressed);
        observer_search_button.GetComponent<Button>().onClick.AddListener(ObserverSearchButtonIsPressed);

    }
    #region ReceivingDataFromServer/Involved
    /// <summary>
    ///  OnDestroy()
    ///  - check message_receiver_from_Server isn't null
    ///  -// this line of code just Unregister ObserverReceivedFuntion from the action.
    ///  ObserverReceivedFuntion will not get call by the action and will  not get any data pas into it.
    /// </summary>
    private void OnDestroy()
    {
        if (message_receiver_from_Server != null)
        {
            message_receiver_from_Server.OnMessageReceivedFromServer -= ObserverReceivedFromServer;
        }

    }

    void ObserverReceivedFromServer(int sigifier, string s, TicTacToeBoard tic_tac_toe_board, MatchData match_data)
    {
        switch (sigifier)
        {
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
                // found the gameroom
                user_name_other = s;
                DisplayPlayerSearchResult(0, observer_search_result_text.GetComponent<Text>());
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameFailed:
                // didn't find the game room
                DisplayPlayerSearchResult( 1, observer_search_result_text.GetComponent<Text>());
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameSizeFailed:
                // found the gameroom and it's filled
                DisplayPlayerSearchResult( 2, observer_search_result_text.GetComponent<Text>());
                break;
            case ServerToClientSignifiers.ObserverGetsMove:
                // get  the game state from the sever
                DisplayBoard(tic_tac_toe_board);
                break;
        }
    }
    #endregion

    #region SearchGameRoomFromSpectatingAndFunctionsInvovle


    public void SetObservrSearch()
    {
        observer_search_inputfield.GetComponent<InputField>().text = "";
        observer_search_button.GetComponent<Button>().interactable = true;
        observer_search_result_text.GetComponent<Text>().text = "Please type a User name to view a GameRoom.";
    }

    public void ObserverSearchButtonIsPressed()
    {
        if (observer_search_inputfield.GetComponent<InputField>().text != "" && observer_search_inputfield.GetComponent<InputField>().text != null)
        {
            user_name_search_for = observer_search_inputfield.GetComponent<InputField>().text;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SearchGameRoomsByUserName + "," + user_name_search_for);
        }
    }

    void DisplayPlayerSearchResult(int IsGameRoomFound, Text t)
    {
        if (IsGameRoomFound == 0)
        {
            t.text = user_name_search_for + " was found in a game.";
            observer_search_button.GetComponent<Button>().interactable = false;
        }
        else if (IsGameRoomFound == 1)
        {
            t.text = user_name_search_for + " wasn't in a gameRoom.";
        }
        else if (IsGameRoomFound == 2)
        {
            t.text = user_name_search_for + "'s gameRoom is at maximum capacity.";
        }

    }


    void ObserverSearchLeaveButtonIsPressed() 
    {
        system_manger_object.GetComponent<SystemManager>().OpenMenu();
    }
    #endregion

    #region WatcherFunctions

    public void SetObservrWatcher()
    {
        DisplayBoard(new TicTacToeBoard(0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
        DisplayPlayerTurn(41, observer_watcher_player_text.GetComponent<Text>(), observer_watcher_opponent_text.GetComponent<Text>());
    }
    public void ObserverWatcherLogoutButtonIsPressed()
    {
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.StopObserving + ",");
    }
   
    void DisplayPlayerTurn(int Whoturn, Text player_view, Text opponent_player)
    {
        if (Whoturn == 41)
        {
            player_view.text = "Player:";
            player_view.color = Color.black;
            opponent_player.color = Color.black;
            opponent_player.text = "Opponent:";
        }
        else if (Whoturn == 43)
        {
            player_view.text = user_name_search_for + ": Waiting";
            player_view.color = Color.black;
            opponent_player.color = Color.blue;
            opponent_player.text = user_name_other + " : Turn";
        }
        else if (Whoturn == 42)
        {
            player_view.text = user_name_search_for + ": Turn";
            player_view.color = Color.blue;
            opponent_player.color = Color.black;
            opponent_player.text = user_name_other + ": Waiting";
        }
    }

    public void DisplayBoard(TicTacToeBoard board)
    {
       int [] virtual_board = board.GetTicTacToeBoardAsArray(board);

        for (int i = 0; i < virtual_board.Length; i++)
        {
            if (virtual_board[i] == 0)
            {
                grid_spaces_observer_watcher[i].text = "";
            }
            else if (virtual_board[i] == 1)
            {
                grid_spaces_observer_watcher[i].text = "X";
            }
            else if (virtual_board[i] == 2)
            {
                grid_spaces_observer_watcher[i].text = "O";
            }
        }
        DisplayPlayerTurn(board.whos_move_, observer_watcher_player_text.GetComponent<Text>(), observer_watcher_opponent_text.GetComponent<Text>());
    }


    #endregion


}
