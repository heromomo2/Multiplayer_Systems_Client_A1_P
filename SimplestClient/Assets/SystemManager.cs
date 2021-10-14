using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private string userName = "NoAccount";  // property
    public string GetUserName  // property
    {
        get { return userName; }
    }
    public string SetUserName  // property
    {
        set { userName = value; }
    }

    //private NetworkedClient m_MessageReceiverFromServer = null;
    private NetworkedClient m_On = null;
    GameObject Login, Chat, networkClient;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Login_Ui")
                Login = go;
            else if (go.name == "Chat_UI")
                Chat = go;
            else if (go.name == "Network")
                networkClient = go;
        }


        //m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();
        //if (m_MessageReceiverFromServer != null)
        //{
        //    m_MessageReceiverFromServer.OnMessageReceivedFromServer += OpenChatRoom;
        //}

        m_On = networkClient.GetComponent<NetworkedClient>();

        if (m_On != null)
        {
            m_On.On += OpenChatRoom;
        }

        Login.SetActive(true);
        Chat.SetActive(false);
    }


    private void OnDestroy()
    {
        //if (m_MessageReceiverFromServer != null)
        //{
        //    m_MessageReceiverFromServer.OnMessageReceivedFromServer -= OpenChatRoom;
        //}

        if (m_On != null)
        {
            m_On.On -= OpenChatRoom;
        }


    }





    public void OpenChatRoom(int signifier, string s) 
    {
        switch ( signifier)
        {
            case 1:
                Login.SetActive(false);
                Chat.SetActive(true);
                break;
        }
    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
