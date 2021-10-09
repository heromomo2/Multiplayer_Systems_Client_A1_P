using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
public class NetworkedClient : MonoBehaviour
{
    private Action<int> m_OnMessageReceivedFromServer = null;
    private Action<string> m_OnMessageReceivedFromServerS = null;

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

    public event Action<string> OnMessageReceivedFromServerS
    {
        add
        {
            m_OnMessageReceivedFromServerS -= value;
            m_OnMessageReceivedFromServerS += value;
        }


        remove
        {
            m_OnMessageReceivedFromServerS -= value;
        }
    }


    public void  Msg (string[] csv ) 
    {
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.CreateAcountComplete)
        {
            //Debug.LogWarning("You have CreateAccount. Try Login");

            if (m_OnMessageReceivedFromServer != null)
            {
                m_OnMessageReceivedFromServer(4);
            }
        }
        else if (signifier == ServerToClientSignifiers.CreateAcountFailed)
        {
            //Debug.LogWarning("We have Account with that user name.");
            if (m_OnMessageReceivedFromServer != null)
            {
                m_OnMessageReceivedFromServer(5);
            }
        }
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            
            if (m_OnMessageReceivedFromServer != null)
            {
                m_OnMessageReceivedFromServer(1);
                Debug.LogWarning("You are now Log-in");
            }
        }
        else if (signifier == ServerToClientSignifiers.LoginFailedAccount)
        {
            
            if (m_OnMessageReceivedFromServer != null)
            {
                m_OnMessageReceivedFromServer(2);
                Debug.LogWarning("check if you have Account Or you miss spell your user name");
            }
        }
        else if (signifier == ServerToClientSignifiers.LoginFailedPassword)
        {
           
            if (m_OnMessageReceivedFromServer != null)
            {
                m_OnMessageReceivedFromServer(3);
                Debug.LogWarning("Your password is wrong ");
            }
        }
        else if (signifier == ServerToClientSignifiers.ChatView)
        {
           // Debug.LogWarning("ServerToClientSignifiers got call");

            if (m_OnMessageReceivedFromServerS != null)
            {
                //m_OnMessageReceivedFromServerS(6);
                string t = csv[1].ToString();
                // Debug.LogWarning("Server : " +  t);
                m_OnMessageReceivedFromServerS(t);
            }
        }
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

    public const int SendChatMsg = 3;
}

public class ServerToClientSignifiers
{

    public const int LoginComplete = 1;

    public const int LoginFailedAccount = 2;

    public const int LoginFailedPassword = 3;

    public const int CreateAcountComplete = 4;
    
    public const int CreateAcountFailed = 5;

    public const int ChatView = 6;

}

