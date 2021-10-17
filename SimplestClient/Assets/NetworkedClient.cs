using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkedClient : MonoBehaviour
{
    private Action<int> m_OnMessageReceivedFromServer = null;
    private Action<string> m_OnMessageReceivedChatRoomMsg = null;
    private Action<string> m_OnMessageReceivedChatUsersList= null;
    private Action<int> m_OnMessageReceivedClearChatUsersList = null;

    private Action<int,string> m_On = null;


    //private string userName = "NoAccount";  // property
    //public string GetUserName  // property
    //{
    //    get { return userName; }
    //}
    //public string SetUserName  // property
    //{
    //    set { userName = value; }
    //}


    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.S))
        //    SendMessageToHost("Hello from client gg");

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "10.0.0.137", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);
            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

       // int signifier = int.Parse(csv[0]);

         Msg(csv);
        
    }


    public event Action<int> OnMessageReceivedFromServer
    {
        add
        {
            m_OnMessageReceivedFromServer -= value;
            m_OnMessageReceivedFromServer += value;
        }
        

        remove
        {
            m_OnMessageReceivedFromServer -= value;
        }
    }

    public event Action<string> OnMessageReceivedChatRoomMsg
    {
        add
        {
            m_OnMessageReceivedChatRoomMsg -= value;
            m_OnMessageReceivedChatRoomMsg += value;
        }


        remove
        {
            m_OnMessageReceivedChatRoomMsg -= value;
        }
    }
    public event Action<string> OnMessageReceivedChatUsers
    {
        add
        {
            m_OnMessageReceivedChatUsersList -= value;
            m_OnMessageReceivedChatUsersList += value;
        }

        remove
        {
            m_OnMessageReceivedChatUsersList -= value;
        }
    }

    public event Action<int> OnMessageReceivedClearChatUsersList
    {
        add
        {
            m_OnMessageReceivedClearChatUsersList -= value;
            m_OnMessageReceivedClearChatUsersList += value;
        }

        remove
        {
            m_OnMessageReceivedClearChatUsersList -= value;
        }
    }

    public event Action<int,string> On
    {
        add
        {
            m_On -= value;
            m_On += value;
        }

        remove
        {
            m_On -= value;
        }
    }



    public void  Msg (string[] csv ) 
    {
        int signifier = int.Parse(csv[0]);
        string FirstElement = csv[1].ToString();

        if (csv[1] == ""|| csv[1] == null)
        {
            Debug.Log("CSV[1] : ->" + csv[1]);
        }
        else
        {
            //FirstElement = "99";
        }

        if( m_On != null)
        {
           m_On(signifier, FirstElement);
        }

        //if (signifier == ServerToClientSignifiers.CreateAcountComplete)
        //{
        //    //Debug.LogWarning("You have CreateAccount. Try Login");

        //    if (m_OnMessageReceivedFromServer != null)
        //    {
        //        m_OnMessageReceivedFromServer(4);
        //    }
        //}
        //else if (signifier == ServerToClientSignifiers.CreateAcountFailed)
        //{
        //    //Debug.LogWarning("We have Account with that user name.");
        //    if (m_OnMessageReceivedFromServer != null)
        //    {
        //        m_OnMessageReceivedFromServer(5);
        //    }
        //}
        //if (signifier == ServerToClientSignifiers.LoginComplete)
        //{
            
        //    if (m_OnMessageReceivedFromServer != null)
        //    {
        //        m_OnMessageReceivedFromServer(1);
        //         SetUserName = csv[1].ToString();
        //        Debug.LogWarning("You are now Log-in");
        //    }
        //}
        //else if (signifier == ServerToClientSignifiers.LoginFailedAccount)
        //{
            
        //    if (m_OnMessageReceivedFromServer != null)
        //    {
        //        m_OnMessageReceivedFromServer(2);
        //        Debug.LogWarning("check if you have Account Or you miss spell your user name");
        //    }
        //}
        //else if (signifier == ServerToClientSignifiers.LoginFailedPassword)
        //{
           
        //    if (m_OnMessageReceivedFromServer != null)
        //    {
        //        m_OnMessageReceivedFromServer(3);
        //        Debug.LogWarning("Your password is wrong ");
        //    }
        //}
        //else if (signifier == ServerToClientSignifiers.ChatView)
        //{
        //   // Debug.LogWarning("ServerToClientSignifiers got call");

        //    if (m_OnMessageReceivedChatRoomMsg != null)
        //    {
        //        string MsgForServer = csv[1].ToString();
        //        // Debug.LogWarning("Server : " +  t);
        //        m_OnMessageReceivedChatRoomMsg(MsgForServer);
        //    }
        //}
        //else if (signifier == ServerToClientSignifiers.ReceiveListOFPlayerInChat)
        //{
        //    Debug.LogWarning("ReceiveListOFPlayerInChat  was received");

        //    if (m_OnMessageReceivedChatUsersList != null)
        //    {
        //        string OtherUserName = csv[1].ToString();
        //        // Debug.LogWarning("Server : " +  t);
        //        m_OnMessageReceivedChatUsersList(OtherUserName);
        //    }
        //}
        //else if (signifier == ServerToClientSignifiers.ReceiveClearListOFPlayerInChat)
        //{
        //    Debug.LogWarning("ReceiveListOFPlayerInChat  was received");

        //    if (m_OnMessageReceivedClearChatUsersList != null)
        //    {
        //        m_OnMessageReceivedClearChatUsersList(9);
        //    }
        //}
        //Debug.Log("Msg function was called");
    }

    public bool IsConnected()
    {
        return isConnected;
    }  
}

public class ClientToServerSignifiers
{
    public const int CreateAcount = 1;

    public const int Login = 2;

    public const int SendChatMsg = 3; // send a globle chat message

    public const int SendChatPrivateMsg = 4;// send a chat private msg

    public const int EnterTheChatRoom = 5; // enter the chat room

    public const int Logout = 6;

}

public class ServerToClientSignifiers
{

    public const int LoginComplete = 1;

    public const int LoginFailedAccount = 2;

    public const int LoginFailedPassword = 3;

    public const int CreateAcountComplete = 4;

    public const int CreateAcountFailed = 5;

    public const int ChatView = 6; // all the receive globe chatmessage.

    public const int ReceivePrivateChatMsg = 7;//  receive a private chat message.

    public const int ReceiveListOFPlayerInChat = 8;// all the list of players in the chat

    public const int ReceiveClearListOFPlayerInChat = 9;// all the list of players 
    
    public const int LogOutComplete = 10;

}

