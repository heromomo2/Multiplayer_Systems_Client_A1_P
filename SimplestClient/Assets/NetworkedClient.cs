using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkedClient : MonoBehaviour
{
    private Action<int,string> m_On = null;


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


         Msg(csv);
        
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


        if( m_On != null)
        {
           m_On(signifier, FirstElement);
        }
        
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

    public const int CreateGameRoomComplete = 11;

    public const int JoinGameRoomComplete = 12;
}

