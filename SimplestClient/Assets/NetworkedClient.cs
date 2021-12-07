using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkedClient : MonoBehaviour
{
    private Action<int,string,TicTacToeBoard,MatchData> message_receiver_from_server = null;


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

        // comma-separated values
        string[] csv = msg.Split(',');

        // Msg(csv);

     
        /// extrating data from the msg from server
        /// store them in local vars
        /// 
        int signifier = int.Parse(csv[0]);
        string first_element = csv[1].ToString();
        TicTacToeBoard temp_tic_tac_toe_state;
        MatchData temp_match_data = new MatchData("TempmatchData", 0, 3);

        if (signifier == ServerToClientSignifiers.ObserverGetsMove)
        {
            temp_tic_tac_toe_state = new TicTacToeBoard(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]), int.Parse(csv[7]), int.Parse(csv[8]), int.Parse(csv[9]), int.Parse(csv[10]));
        }
        else
        {
            temp_tic_tac_toe_state = new TicTacToeBoard(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
        if (signifier == ServerToClientSignifiers.SendAllThisRecoredMatchData)
        {
            temp_match_data = new MatchData(csv[1], int.Parse(csv[2]), int.Parse(csv[3]));
        }

        // if  action isn't null
        /// passing  the data into the data
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server(signifier, first_element, temp_tic_tac_toe_state, temp_match_data);
        }

    }


  

    public event Action<int,string, TicTacToeBoard, MatchData> OnMessageReceivedFromServer
    {
        add
        {
            message_receiver_from_server -= value;
            message_receiver_from_server += value;
        }

        remove
        {
            message_receiver_from_server -= value;
        }
    }



    //public void  Msg (string[] csv ) 
    //{
    //    //int signifier = int.Parse(csv[0]);
    //    //string first_element = csv[1].ToString();
    //    //TicTacToeBoard temp_tic_tac_toe_state;
    //    //MatchData temp_match_data = new MatchData("TempmatchData", 0, 3);

    //    //if (signifier == ServerToClientSignifiers.ObserverGetsMove)
    //    //{
    //    //    temp_tic_tac_toe_state = new TicTacToeBoard(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]), int.Parse(csv[7]), int.Parse(csv[8]), int.Parse(csv[9]), int.Parse(csv[10]));
    //    //}
    //    //else
    //    //{
    //    //    temp_tic_tac_toe_state = new TicTacToeBoard(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    //    //}
    //    //if (signifier == ServerToClientSignifiers.SendAllThisRecoredMatchData)
    //    //{
    //    //    temp_match_data = new MatchData(csv[1], int.Parse(csv[2]), int.Parse(csv[3]));
    //    //}


    //    //if (message_receiver_from_server != null)
    //    //{
    //    //    message_receiver_from_server(signifier, first_element, temp_tic_tac_toe_state, temp_match_data);
    //    //}

    //}

    public bool IsConnected()
    {
        return isConnected;
    }  
}






#region Protocol
public class ClientToServerSignifiers
{
    public const int CreateAcount = 1;

    public const int Login = 2;

    public const int NotifyPublicChatChatOfGlobalMsg = 3; // send a globle chat message

    public const int NotifyPublicChatWitchAPrivateMsg = 4;// send a chat private msg

    public const int EnterThePublicChatRoom = 5; // enter the chat room

    public const int NotifyPublicChatOfLogout = 6;//

    public const int JoinQueueForGameRoom = 7;

    public const int TicTacTacDoAMove = 8;

    public const int RematchOfTicTacToe = 9;

    public const int ExitTacTacToe = 10;

    public const int SearchGameRoomsByUserName = 11;

    public const int SendObserverData = 12;

    public const int StopObserving = 13;

    public const int SendGameRoomGlobalChatMSG = 14;

    public const int SendOnlyPlayerGameRoomChatMSG = 15;

    public const int SendOnlyObserverGameRoomChatMSG = 16;

    public const int CreateARecored = 17;

    public const int AskForAllRecoredNames = 18;

    public const int AskForThisRecoredMatchData = 19;
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

    public const int OpponentPlayed = 11;

    public const int GameStart = 12;

    public const int WaitForOppentMoved = 13;

    public const int RematchOfTicTacToeComplete = 14;

    public const int ExitTacTacToeComplete = 15;

    public const int PreventRematch = 16;

    public const int SearchGameRoomsByUserNameComplete = 17;

    public const int SearchGameRoomsByUserNameFailed = 18;

    public const int YouareBeingObserved = 20;

    public const int ObserverGetsMove = 21;

    public const int YouAreNotBeingObserved = 22;

    public const int PlayerDisconnectFromGameRoom = 23;

    public const int StopObservingComplete = 24;

    public const int ReceiveGameRoomChatMSG = 25;

    public const int ReceiveOpponentName = 26;

    public const int SearchGameRoomsByUserNameSizeFailed = 27;

    public const int CreateARecoredSuccess = 28;

    public const int CreateARecoredFail = 29;

    public const int StartSendAllRecoredsName = 30;

    public const int SendAllRecoredsNameData = 31;

    public const int DoneSendAllRecoredsName = 32;

    public const int StartSendThisRecoredMatchData = 33;

    public const int SendAllThisRecoredMatchData = 34;

    public const int DoneSendAllThisRecoredMatchData = 35;

    public const int NoRecordsNamefound = 36;
}


public class MatchData
{
    public int Positoin;
    public int PlayerSymbol;
    public string Playername;

    public MatchData(string playerName, int position, int playerSymbol)
    {
        Positoin = position;
        Playername = playerName;
        PlayerSymbol = playerSymbol;
    }

}
public class TicTacToeBoard
{
    public int top_left_, top_mid_, top_right_, mid_left_, mid_mid_, mid_right_, bot_left_, bot_mid_, bot_right_;
    public int whos_move_;


    public TicTacToeBoard(int tl, int tm, int tr, int ml, int mm, int mr, int bl, int bm, int br, int wsm)
    {
        top_left_ = tl;
        top_mid_ = tm;
        top_right_ = tr;
        mid_left_ = ml;
        mid_mid_ = mm;
        mid_right_ = mr;
        bot_left_ = bl;
        bot_mid_ = bm;
        bot_right_ = br;
        whos_move_ = wsm;
    }

    public int[] GetTicTacToeBoardAsArray(TicTacToeBoard board)
    {
        int[] array = new int[9] { board.top_left_, board.top_mid_, board.top_right_, board.mid_left_, board.mid_mid_, mid_right_, board.bot_left_, board.bot_mid_, board.bot_right_ };
        return array;
    }

}


/*
 * naming convention:

-Enumerator Names->kEnumName
-Funtion-> AddTableEntry
-Constant Names -> kDaysInAWeek
-Class Data Members -> pool_
-Struct Data Members->table_name
-Variable -> table_name
 * 
 * 
 */
#endregion