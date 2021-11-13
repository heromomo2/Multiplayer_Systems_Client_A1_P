using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Observer : MonoBehaviour
{
    public NetworkedClient m_MessageReceiverFromServer = null;
    /// Searcher
   public GameObject Observer_Search;
    public InputField m_Observer_Search_InputField;
    public Text m_Observer_Search_Text;
    public Button m_Observer_Search_Button;
   // bool m_IsGameRoomFound = false;
    string m_UserNameSearchFor = "";
   // string m_UserName = "<The Player>";
    /// <summary>
    /// watcher
    /// </summary>
    public GameObject Observer_Watcher;
    public List<Text> m_GridSpaces_Observer_Watcher = new List<Text>();
    public TicTacToeBoard m_Board_Observer_Watcher = null;
    public Text m_Observer_Watcher_Player_text, m_Observer_Watcher_Opponent_text = null;
    string m_UserNameOther = "";
    /// <summary>
    /// /
    /// </summary>
    GameObject Network, SystemMangerObject;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "SystemManagerObject")
                SystemMangerObject = go;
            else if (go.name == "Network")
                Network = go;
        }
        

        m_MessageReceiverFromServer = Network.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += ObserverReceived;
        }
    }
    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= ObserverReceived;
        }

    }
    void ObserverReceived(int sigifier, string s, TicTacToeBoard t)
    {
        switch (sigifier)
        {
            case ServerToClientSignifiers.LoginComplete:
               // SetObservrSearch();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
               // m_IsGameRoomFound = true; 
                m_UserNameOther = s;
                DisplayPlayerSearchResult(0, m_Observer_Search_Text);
               /// SetObservrWatcher();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameFailed:
              //  m_IsGameRoomFound = false;
                DisplayPlayerSearchResult( 1, m_Observer_Search_Text);
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameSizeFailed:
              //  m_IsGameRoomFound = false;
                DisplayPlayerSearchResult( 2, m_Observer_Search_Text);
                break;
            case ServerToClientSignifiers.ObserverGetsMove:
                DisplayMovePart1(t);
                break;
        }
    }


    public void SetObservrSearch()
    {
        m_Observer_Search_InputField.text = "";
        m_Observer_Search_Button.interactable = true;
        m_Observer_Search_Text.text = "Please type a User name to view a GameRoom.";
       // m_IsGameRoomFound = false;
    }

    public void SetObservrWatcher()
    {
        DisplayMovePart1(new TicTacToeBoard(0, 0, 0, 0, 0, 0,0, 0, 0, 0));
        DisplayMovePart3( 41, m_Observer_Watcher_Player_text, m_Observer_Watcher_Opponent_text);
    }

    public void Observer_Search_ButtonIsPressed()
    {
        if (m_Observer_Search_InputField.text != "" && m_Observer_Search_InputField.text != null)
        {
            m_UserNameSearchFor =  m_Observer_Search_InputField.text;
            Network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SearchGameRoomsByUserName + "," + m_UserNameSearchFor);
        }
    }


    public void Observer_Watcher_Logout_ButtonIsPressed()
    {
        Network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.StopObserving + ",");
    }

    void DisplayPlayerSearchResult (int IsGameRoomFound, Text t) 
    {
        if (IsGameRoomFound == 0)
        {
            t.text = m_UserNameSearchFor + " was found in a game.";
            m_Observer_Search_Button.interactable = false;
        }
        else if (IsGameRoomFound == 1)
        {
            t.text = m_UserNameSearchFor + " wasn't in a gameRoom.";
        }
        else if (IsGameRoomFound == 2)
        {
            t.text = m_UserNameSearchFor + "'s gameRoom is at maximum capacity.";
        }

    }

    void ReceivedMove() 
    {
        
    }

    void DisplayMovePart1(TicTacToeBoard b)
    {
        DisplayMovePart2(b.topleft, m_GridSpaces_Observer_Watcher[0]); DisplayMovePart2(b.topmid, m_GridSpaces_Observer_Watcher[1]); DisplayMovePart2(b.topright, m_GridSpaces_Observer_Watcher[2]);
        DisplayMovePart2(b.midleft, m_GridSpaces_Observer_Watcher[3]); DisplayMovePart2(b.midmid, m_GridSpaces_Observer_Watcher[4]); DisplayMovePart2(b.midright, m_GridSpaces_Observer_Watcher[5]);
        DisplayMovePart2(b.botleft, m_GridSpaces_Observer_Watcher[6]); DisplayMovePart2(b.botmid, m_GridSpaces_Observer_Watcher[7]); DisplayMovePart2(b.botright, m_GridSpaces_Observer_Watcher[8]);

        DisplayMovePart3(b.WhosMove, m_Observer_Watcher_Player_text, m_Observer_Watcher_Opponent_text);
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
            P.text =   "Player:";
            P.color = Color.black;
            O.color = Color.black;
            O.text = "Opponent:";
        }
        else if (Whoturn == 43)
        {
            P.text = m_UserNameSearchFor +": Waiting";
            P.color = Color.black;
            O.color = Color.blue;
            O.text = m_UserNameOther + " : Turn";
        }
        else if (Whoturn == 42)
        {
            P.text = m_UserNameSearchFor + ": Turn";
            P.color = Color.blue;
            O.color = Color.black;
            O.text = m_UserNameOther + ": Waiting";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
