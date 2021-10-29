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
    string m_UserName = "";
    /// <summary>
    /// watcher
    /// </summary>
    public GameObject Observer_Watcher;

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
    void ObserverReceived(int sigifier, string s)
    {
        switch (sigifier)
        {
            case ServerToClientSignifiers.LoginComplete:
                SetObservrSearch();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
                DisplayPlayerSearchResult(m_IsGameRoomFound, m_Observer_Search_Text);
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameFailed:
                DisplayPlayerSearchResult(m_IsGameRoomFound, m_Observer_Search_Text);
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

    public void Observer_Search_ButtonIsPressed()
    {
        if (m_Observer_Search_InputField.text != "" && m_Observer_Search_InputField.text != null)
        {
            m_UserName =  m_Observer_Search_InputField.text;
            Network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SearchGameRoomsByUserName + "," + m_UserName);
        }
    }


    void DisplayPlayerSearchResult (bool IsGameRoomFound, Text t) 
    {
        if (IsGameRoomFound)
        {
            t.text = "That Player was found in a game.";
            m_Observer_Search_Button.interactable = false;
        }
        else 
        {
            t.text = "That Player wasn't in a gameRoom.";
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
