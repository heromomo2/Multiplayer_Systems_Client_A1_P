using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Observer : MonoBehaviour
{
    #region GameObjects
    public NetworkedClient message_receiver_from_Server = null;
    GameObject network, system_manger_object;
    #endregion

    #region Searcher
    public GameObject observer_search;
    public InputField observer_search_inputfield;
    public Text observer_search_text;
    public Button observer_search_button;
    string user_name_search_for = "";
    #endregion

    #region Watcher
    public GameObject observer_watcher;
    public List<Text> grid_spaces_observer_watcher = new List<Text>();
    public TicTacToeBoard board_observer_watcher = null;
    public Text observer_watcher_player_text, observer_watcher_opponent_text = null;
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
    }

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
            case ServerToClientSignifiers.LoginComplete:
               // SetObservrSearch();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
               // m_IsGameRoomFound = true; 
                user_name_other = s;
                DisplayPlayerSearchResult(0, observer_search_text);
               /// SetObservrWatcher();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameFailed:
              //  m_IsGameRoomFound = false;
                DisplayPlayerSearchResult( 1, observer_search_text);
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameSizeFailed:
              //  m_IsGameRoomFound = false;
                DisplayPlayerSearchResult( 2, observer_search_text);
                break;
            case ServerToClientSignifiers.ObserverGetsMove:
                // DisplayMovePart1(t);
                DisplayBoard(tic_tac_toe_board);
                break;
        }
    }

    #region Searchfunctions
    public void SetObservrSearch()
    {
        observer_search_inputfield.text = "";
        observer_search_button.interactable = true;
        observer_search_text.text = "Please type a User name to view a GameRoom.";
        // m_IsGameRoomFound = false;
    }

    public void Observer_Search_ButtonIsPressed()
    {
        if (observer_search_inputfield.text != "" && observer_search_inputfield.text != null)
        {
            user_name_search_for = observer_search_inputfield.text;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SearchGameRoomsByUserName + "," + user_name_search_for);
        }
    }

    void DisplayPlayerSearchResult(int IsGameRoomFound, Text t)
    {
        if (IsGameRoomFound == 0)
        {
            t.text = user_name_search_for + " was found in a game.";
            observer_search_button.interactable = false;
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
    #endregion

    #region Watcherfunctions

    public void SetObservrWatcher()
    {
        DisplayBoard(new TicTacToeBoard(0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
        DisplayPlayerTurn(41, observer_watcher_player_text, observer_watcher_opponent_text);
    }
    public void Observer_Watcher_Logout_ButtonIsPressed()
    {
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.StopObserving + ",");
    }
   
    void DisplayPlayerTurn(int Whoturn, Text P, Text O)
    {
        if (Whoturn == 41)
        {
            P.text = "Player:";
            P.color = Color.black;
            O.color = Color.black;
            O.text = "Opponent:";
        }
        else if (Whoturn == 43)
        {
            P.text = user_name_search_for + ": Waiting";
            P.color = Color.black;
            O.color = Color.blue;
            O.text = user_name_other + " : Turn";
        }
        else if (Whoturn == 42)
        {
            P.text = user_name_search_for + ": Turn";
            P.color = Color.blue;
            O.color = Color.black;
            O.text = user_name_other + ": Waiting";
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
        DisplayPlayerTurn(board.whos_move_, observer_watcher_player_text, observer_watcher_opponent_text);
    }
    #endregion


}
