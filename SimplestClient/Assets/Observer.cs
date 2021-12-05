using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Observer : MonoBehaviour
{
   // public NetworkedClient m_MessageReceiverFromServer = null;
    /// Searcher
   
   // bool m_IsGameRoomFound = false;
   
   // string m_UserName = "<The Player>";
    /// <summary>
    /// watcher
    /// </summary>
   
    /// <summary>
    /// /
    /// </summary>
    



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
        

        message_receiver_from_Server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_Server != null)
        {
            message_receiver_from_Server.OnMessageReceivedFromServer += ObserverReceived;
        }
    }
    private void OnDestroy()
    {
        if (message_receiver_from_Server != null)
        {
            message_receiver_from_Server.OnMessageReceivedFromServer -= ObserverReceived;
        }

    }

    void ObserverReceived(int sigifier, string s, TicTacToeBoard t, MatchData matchData)
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
                DisplayMovePart1(t);
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
        DisplayMovePart1(new TicTacToeBoard(0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
        DisplayMovePart3(41, observer_watcher_player_text, observer_watcher_opponent_text);
    }
    public void Observer_Watcher_Logout_ButtonIsPressed()
    {
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.StopObserving + ",");
    }
    void DisplayMovePart1(TicTacToeBoard b)
    {
        DisplayMovePart2(b.topleft, grid_spaces_observer_watcher[0]); DisplayMovePart2(b.topmid, grid_spaces_observer_watcher[1]); DisplayMovePart2(b.topright, grid_spaces_observer_watcher[2]);
        DisplayMovePart2(b.midleft, grid_spaces_observer_watcher[3]); DisplayMovePart2(b.midmid, grid_spaces_observer_watcher[4]); DisplayMovePart2(b.midright, grid_spaces_observer_watcher[5]);
        DisplayMovePart2(b.botleft, grid_spaces_observer_watcher[6]); DisplayMovePart2(b.botmid, grid_spaces_observer_watcher[7]); DisplayMovePart2(b.botright, grid_spaces_observer_watcher[8]);

        DisplayMovePart3(b.WhosMove, observer_watcher_player_text, observer_watcher_opponent_text);
    }
    void DisplayMovePart2(int space, Text t)
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

    void DisplayMovePart3(int Whoturn, Text P, Text O)
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

    #endregion


    // Update is called once per frame
    void Update()
    {
        
    }
}
