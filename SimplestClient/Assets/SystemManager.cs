using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    #region Variables
    // where we are store username
    private string user_name = "NoAccount";  // property
    //getter and setter for username
    public string GetUserName  // property
    {
        get { return user_name; }
    }
    public string SetUserName  // property
    {
        set { user_name = value; }
    }
    #endregion

    #region GameObjects
    private NetworkedClient m_MessageReceiverFromServer = null;
    GameObject Login, Chat, networkClient,Menu,RecordRequest, WaitingInQueue, game_logic,GameOver,Replayer,Observer, GameRoomChat, Observer_Search, Observer_watcher;
    #endregion 

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
            else if (go.name == "RecordRequest_UI")
                RecordRequest = go;
            else if (go.name == "Game_UI")
                game_logic = go;
            else if (go.name == "WaitingInQueue_UI")
                WaitingInQueue = go;
            else if (go.name == "GameOverScreen_UI ")
                GameOver = go;
            else if (go.name == "Replayer_UI")
                Replayer = go;
            else if (go.name == "Observer_UI")
                Observer = go;
            else if (go.name == "GameRoomChat_UI")
                GameRoomChat = go;
            else if (go.name == "Observer_Search_UI")
                Observer_Search = go;
            else if (go.name == "Observer_Watcher_UI")
                Observer_watcher = go;
        }


        m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
          
            m_MessageReceiverFromServer.OnMessageReceivedFromServer += SystemManagerReceived;
        }


        ChangeState(GameStates.LoginMenu);
    }


    private void OnDestroy()
    { 
        if (m_MessageReceiverFromServer != null)
        {
            
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= SystemManagerReceived;
        }
    
    }


 
    
    void SystemManagerReceived (int signifier, string s, TicTacToeBoard t, MatchData matchData) 
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.LoginComplete:
                ChangeState(GameStates.MainMenu);
                break;
            case ServerToClientSignifiers.OpponentPlayed:
                Debug.LogWarning("Your Opponet Just played");
                break;
            case ServerToClientSignifiers.GameStart:
                ChangeState(GameStates.TicTacToe);
                break;
            case ServerToClientSignifiers.RematchOfTicTacToeComplete:
                ChangeState(GameStates.TicTacToe);
                break;
            case ServerToClientSignifiers.ExitTacTacToeComplete:
                ChangeState(GameStates.LoginMenu);
                Login.GetComponentInChildren<LogInScript>().ResetLogic();
                game_logic.GetComponent<GameLogic>().ResetBoards();
                break;
            case ServerToClientSignifiers.PlayerDisconnectFromGameRoom:
                ChangeState(GameStates.LoginMenu);
                Login.GetComponentInChildren<LogInScript>().ResetLogic();
                game_logic.GetComponent<GameLogic>().ResetBoards();
                break;
            case ServerToClientSignifiers.StopObservingComplete:
                ChangeState(GameStates.LoginMenu);
                Login.GetComponentInChildren<LogInScript>().ResetLogic();
                game_logic.GetComponent<GameLogic>().ResetBoards();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
                OpenObserverWatcer();
                break;
        }
    
    }


    
    public void OpenGameOver()
    {
        GameOver.GetComponent<GameOver>().GamerOverTextChange();
        ChangeState(GameStates.GameOver);
    }
    public void OpenReplayer()
    {
        ChangeState(GameStates.Replayer);
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AskForAllRecoredNames+ "," + GetUserName);
    }
    public void OpenMenu()
    {
        ChangeState(GameStates.MainMenu);
    }
    public void OpenLogin()
    {
        ChangeState(GameStates.LoginMenu);
    }
    public void OpenObserverSearch()
    {
        ChangeState(GameStates.Observer_Search);
        Observer.GetComponent<Observer>().SetObservrSearch();
    }

    private void OpenObserverWatcer()
    {
        ChangeState(GameStates.Observer_Watcher);
        Observer.GetComponent<Observer>().SetObservrWatcher();
    }
    public void OpenChatRoom()
    {
        // open gameroom Ui and send a msg to server

        ChangeState(GameStates.chatroom);
        string OurEnterTheChatMsg = ClientToServerSignifiers.EnterThePublicChatRoom + "," + GetUserName;
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(OurEnterTheChatMsg);
    }
    public void Logout()
    {
        string logoutMsg = ClientToServerSignifiers.NotifyPublicChatOfLogout + ",";

        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(logoutMsg);
        ChangeState(GameStates.LoginMenu);

    }


    public void GameRoomButtonIsPreessed()
    {

         ChangeState(GameStates.WaitingInQueueforOtherPlayer);
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + ","+ user_name);
    }

    void ChangeState(int newState) 
    {
        switch (newState) 
        {
            case GameStates.LoginMenu:
                Login.SetActive(true);
                Chat.SetActive(false);
                Menu.SetActive(false);
                RecordRequest.SetActive(false);
                WaitingInQueue.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Replayer.SetActive(false);
                Observer.SetActive(false);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
            case GameStates.MainMenu:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(true);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Observer.SetActive(false);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
            case GameStates.WaitingInQueueforOtherPlayer:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(true);
                RecordRequest.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Replayer.SetActive(false);
                Observer.SetActive(false);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
            case GameStates.TicTacToe:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(false);
                game_logic.SetActive(true);
                GameOver.SetActive(false);
                Replayer.SetActive(false);
                Observer.SetActive(false);
                GameRoomChat.SetActive(true);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
            case GameStates.GameOver:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(true);
                game_logic.SetActive(true);
                GameOver.SetActive(true);
                Replayer.SetActive(false);
                Observer.SetActive(false);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
            case GameStates.Replayer:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Replayer.SetActive(true);
                Observer.SetActive(false);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
            case GameStates.Observer_Search:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Replayer.SetActive(false);
                Observer.SetActive(true);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(true);
                Observer_watcher.SetActive(false);
                break;

            case GameStates.Observer_Watcher:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Replayer.SetActive(false);
                Observer.SetActive(true);
                GameRoomChat.SetActive(true);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(true);
                break;

            case GameStates.chatroom:
                Login.SetActive(false);
                Chat.SetActive(true);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                RecordRequest.SetActive(false);
                game_logic.SetActive(false);
                GameOver.SetActive(false);
                Replayer.SetActive(false);
                Observer.SetActive(false);
                GameRoomChat.SetActive(false);
                Observer_Search.SetActive(false);
                Observer_watcher.SetActive(false);
                break;
        }
    }


    // Our Gamest
    static public class GameStates
    {
        public const int LoginMenu = 1;
        public const int MainMenu = 2;

        public const int WaitingInQueueforOtherPlayer = 3;
       
        public const int TicTacToe = 4;

        public const int chatroom = 5;

        public const int GameOver = 6;

        public const int Replayer = 7;

        public const int Observer_Search = 8;

        public const int Observer_Watcher = 9;

    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
