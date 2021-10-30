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
    bool m_IsGameRoomFound = false;
    string m_UserNameSearchFor = "";
   // string m_UserName = "<The Player>";
    /// <summary>
    /// watcher
    /// </summary>
    public GameObject Observer_Watcher;
    public List<Text> m_GridSpaces_Observer_Watcher = new List<Text>();
    public TicTacToeBoard m_Board_Observer_Watcher = null;
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
                SetObservrSearch();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
                m_IsGameRoomFound = true;
                DisplayPlayerSearchResult(m_IsGameRoomFound, m_Observer_Search_Text);
                SetObservrWatcher();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameFailed:
                m_IsGameRoomFound = false;
                DisplayPlayerSearchResult(m_IsGameRoomFound, m_Observer_Search_Text);
                break;
            case ServerToClientSignifiers.ObserverGetsMove:
                DisplayMovePart1(t);
                break;
        }
    }


    void SetObservrSearch()
    {
        Observer_Watcher.SetActive(false);
        Observer_Search.SetActive(true);
        m_Observer_Search_InputField.text = "";
        m_Observer_Search_Button.interactable = true;
        m_Observer_Search_Text.text = "Please type a User name to view a GameRoom.";
        m_IsGameRoomFound = false;
    }

    void SetObservrWatcher()
    {
        Observer_Watcher.SetActive(true);
        Observer_Search.SetActive(false);
    }

    public void Observer_Search_ButtonIsPressed()
    {
        if (m_Observer_Search_InputField.text != "" && m_Observer_Search_InputField.text != null)
        {
            m_UserNameSearchFor =  m_Observer_Search_InputField.text;
            Network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SearchGameRoomsByUserName + "," + m_UserNameSearchFor);
        }
    }


    void DisplayPlayerSearchResult (bool IsGameRoomFound, Text t) 
    {
        if (IsGameRoomFound)
        {
            t.text = m_UserNameSearchFor + " was found in a game.";
            m_Observer_Search_Button.interactable = false;
        }
        else 
        {
            t.text = m_UserNameSearchFor + " wasn't in a gameRoom.";
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
