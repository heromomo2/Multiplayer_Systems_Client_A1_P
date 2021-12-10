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

    #region SystemManagerGameObjects
    private NetworkedClient message_receiver_from_server = null;
    GameObject login, public_chat, network,menu,record_request, waiting_in_queue, game_logic,game_over,replayer,observer, game_room_chat_room, observer_search, observer_watcher;

    #endregion

    #region MainMenuGameObjects
    GameObject main_menu_public_chat_room_button, main_menu_game_room_button_chat_room_button, main_menu_leave_button, main_menu_observer_button, main_menu_replayer_button;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // gettting all our gameobjects
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Login_Ui")
                login = go;
            else if (go.name == "Chat_UI")
                public_chat = go;
            else if (go.name == "Network")
                network = go;
            else if (go.name == "Menu_UI ")
                menu = go;
            else if (go.name == "RecordRequest_UI")
                record_request = go;
            else if (go.name == "Game_UI")
                game_logic = go;
            else if (go.name == "WaitingInQueue_UI")
                waiting_in_queue = go;
            else if (go.name == "GameOverScreen_UI ")
                game_over = go;
            else if (go.name == "Replayer_UI")
                replayer = go;
            else if (go.name == "Observer_UI")
                observer = go;
            else if (go.name == "GameRoomChat_UI")
                game_room_chat_room = go;
            else if (go.name == "Observer_Search_UI")
                observer_search = go;
            else if (go.name == "Observer_Watcher_UI")
                observer_watcher = go;
            else if (go.name == "Enter_Public_Chat_Button")
                main_menu_public_chat_room_button = go;
            else if (go.name == "Enter_GameRoom_Button")
                main_menu_game_room_button_chat_room_button = go;
            else if (go.name == "Menu_LogOut_Button")
                main_menu_leave_button = go;
            else if (go.name == "Enter_ObserverRoom_Button")
                main_menu_observer_button = go;
            else if (go.name == "Enter_ReplayRoom_Button")
                main_menu_replayer_button = go;
        }
    


        // set up our connection to NewtworkObject
        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
          
            message_receiver_from_server.OnMessageReceivedFromServer += SystemManagerReceived;
        }



        // set up the Main menu buttons
       main_menu_game_room_button_chat_room_button.GetComponent<Button>().onClick.AddListener(GameRoomButtonIsPressed);
       main_menu_observer_button.GetComponent<Button>().onClick.AddListener(ObserverButtonIsPressed);
       main_menu_public_chat_room_button.GetComponent<Button>().onClick.AddListener(PublicChatRoomButtonIsPressed);
       main_menu_leave_button.GetComponent<Button>().onClick.AddListener(LeaveButtonIsPressed);
       main_menu_replayer_button.GetComponent<Button>().onClick.AddListener(ReplayerButtonIsPressed);

        // set  our gamestate to login screen when we boot up

        ChangeState(GameStates.LoginMenu);

    }


    private void OnDestroy()
    { 
        if (message_receiver_from_server != null)
        {
            
            message_receiver_from_server.OnMessageReceivedFromServer -= SystemManagerReceived;
        }
    
    }


    #region ReceiviedFromTheServer

    void SystemManagerReceived (int signifier, string s, TicTacToeBoard t, MatchData matchData) 
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.LoginComplete:
                login.GetComponentInChildren<Login>().ResetLogin();
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
                login.GetComponentInChildren<Login>().ResetLogin();
                game_logic.GetComponent<GameLogic>().ResetBoards();
                break;
            case ServerToClientSignifiers.PlayerDisconnectFromGameRoom:
                ChangeState(GameStates.LoginMenu);
                login.GetComponentInChildren<Login>().ResetLogin();
                game_logic.GetComponent<GameLogic>().ResetBoards();
                break;
            case ServerToClientSignifiers.StopObservingComplete:
                ChangeState(GameStates.LoginMenu);
                login.GetComponentInChildren<Login>().ResetLogin();
                game_logic.GetComponent<GameLogic>().ResetBoards();
                break;
            case ServerToClientSignifiers.SearchGameRoomsByUserNameComplete:
                OpenObserverWatcer();
                break;
            case ServerToClientSignifiers.LogOutComplete:
                login.GetComponentInChildren<Login>().ResetLogin();
                break;
        }
    
    }

    private void OpenObserverWatcer()
    {
        ChangeState(GameStates.Observer_Watcher);
        observer.GetComponent<Observer>().SetObservrWatcher();
    }
    #endregion

    #region FunctionsBeCalledOutClass
    public void OpenGameOver()
    {
        game_over.GetComponent<GameOver>().GamerOverMessageText();
        ChangeState(GameStates.GameOver);
    }

    public void OpenMenu()
    {
        ChangeState(GameStates.MainMenu);
    }
   
   
    
    public void LogOutPublicChatRoom()
    {
        string logoutMsg = ClientToServerSignifiers.NotifyPublicChatOfLogout + ",";

        network.GetComponent<NetworkedClient>().SendMessageToHost(logoutMsg);
        ChangeState(GameStates.LoginMenu);

    }

    #endregion


    #region MainMenu
    public void PublicChatRoomButtonIsPressed()
    {
        // open gameroom Ui and send a msg to server

        ChangeState(GameStates.chatroom);
        string OurEnterTheChatMsg = ClientToServerSignifiers.EnterThePublicChatRoom + "," + GetUserName;
        network.GetComponent<NetworkedClient>().SendMessageToHost(OurEnterTheChatMsg);
    }

    public void GameRoomButtonIsPressed()
    {

        ChangeState(GameStates.WaitingInQueueforOtherPlayer);
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + "," + user_name);
    }

    public void ReplayerButtonIsPressed()
    {
        ChangeState(GameStates.Replayer);
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AskForAllRecoredNames + "," + GetUserName);
    }
    public void LeaveButtonIsPressed()
    {
        ChangeState(GameStates.LoginMenu);
    }

    public void ObserverButtonIsPressed()
    {
        ChangeState(GameStates.Observer_Search);
        observer.GetComponent<Observer>().SetObservrSearch();
    }

    #endregion


    #region GameStateMahice
    void ChangeState(int newState) 
    {
        switch (newState) 
        {
            case GameStates.LoginMenu:
                login.SetActive(true);
                public_chat.SetActive(false);
                menu.SetActive(false);
                record_request.SetActive(false);
                waiting_in_queue.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                replayer.SetActive(false);
                observer.SetActive(false);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
                break;
            case GameStates.MainMenu:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(true);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                observer.SetActive(false);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
                break;
            case GameStates.WaitingInQueueforOtherPlayer:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(false);
                waiting_in_queue.SetActive(true);
                record_request.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                replayer.SetActive(false);
                observer.SetActive(false);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
                break;
            case GameStates.TicTacToe:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(false);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(false);
                game_logic.SetActive(true);
                game_over.SetActive(false);
                replayer.SetActive(false);
                observer.SetActive(false);
                game_room_chat_room.SetActive(true);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
                break;
            case GameStates.GameOver:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(false);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(true);
                game_logic.SetActive(true);
                game_over.SetActive(true);
                replayer.SetActive(false);
                observer.SetActive(false);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
                break;
            case GameStates.Replayer:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(false);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                replayer.SetActive(true);
                observer.SetActive(false);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
                break;
            case GameStates.Observer_Search:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(false);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                replayer.SetActive(false);
                observer.SetActive(true);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(true);
                observer_watcher.SetActive(false);
                break;

            case GameStates.Observer_Watcher:
                login.SetActive(false);
                public_chat.SetActive(false);
                menu.SetActive(false);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                replayer.SetActive(false);
                observer.SetActive(true);
                game_room_chat_room.SetActive(true);
                observer_search.SetActive(false);
                observer_watcher.SetActive(true);
                break;

            case GameStates.chatroom:
                login.SetActive(false);
                public_chat.SetActive(true);
                menu.SetActive(false);
                waiting_in_queue.SetActive(false);
                record_request.SetActive(false);
                game_logic.SetActive(false);
                game_over.SetActive(false);
                replayer.SetActive(false);
                observer.SetActive(false);
                game_room_chat_room.SetActive(false);
                observer_search.SetActive(false);
                observer_watcher.SetActive(false);
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
    #endregion 
}
