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
    GameObject Login, Chat, networkClient,Menu;

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
            else if (go.name == "Menu_UI ")
                Menu = go;
        }


        //m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();
        //if (m_MessageReceiverFromServer != null)
        //{
        //    m_MessageReceiverFromServer.OnMessageReceivedFromServer += OpenChatRoom;
        //}

        m_On = networkClient.GetComponent<NetworkedClient>();

        if (m_On != null)
        {
            m_On.On += OpenMenu;
            m_On.On += reOpenLogin;
        }

        Login.SetActive(true);
        Chat.SetActive(false);
        Menu.SetActive(false);
    }


    private void OnDestroy()
    {
        //if (m_MessageReceiverFromServer != null)
        //{
        //    m_MessageReceiverFromServer.OnMessageReceivedFromServer -= OpenChatRoom;
        //}

        if (m_On != null)
        {
            m_On.On -= OpenMenu;
            m_On.On -= reOpenLogin;
        }


    }





    public void OpenChatRoom() 
    {
        // open gameroom Ui and send a msg to server
        Login.SetActive(false);
        Chat.SetActive(true);
        Menu.SetActive(false);

        string OurEnterTheChatMsg = ClientToServerSignifiers.EnterTheChatRoom + "," + GetUserName;
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(OurEnterTheChatMsg);
    }

    public void Logout()
    {
        string logoutMsg = ClientToServerSignifiers.Logout+ ",";
       
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(logoutMsg);
    }
    public void reOpenLogin(int signifier, string s)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.LogOutComplete:
                Login.SetActive(true);
                Chat.SetActive(false);
                Menu.SetActive(false);
                break;
        }
    }
    public void OpenGameRooms()
    {
       
    }
    public void OpenMenu(int signifier, string s) 
    {
        switch ( signifier)
        {
            case ServerToClientSignifiers.LoginComplete:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(true);
                break;
        }
    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
