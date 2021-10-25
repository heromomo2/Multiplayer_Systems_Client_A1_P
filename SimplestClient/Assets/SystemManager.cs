using UnityEngine;
using UnityEngine.UI;

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
    


    private NetworkedClient m_MessageReceiverFromServer = null;
    GameObject Login, Chat, networkClient,Menu,Lobby, WaitingInQueue, Tic_Tac_Toe,GameOver;

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
            else if (go.name == "Lobby_UI")
                Lobby = go;
            else if (go.name == "Game_UI")
                Tic_Tac_Toe = go;
            else if (go.name == "WaitingInQueue_UI")
                WaitingInQueue = go;
            else if (go.name == "GameOverScreen_UI ")
                GameOver = go;
        }


        m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            //m_MessageReceiverFromServer.OnMessageReceivedFromSever += OpenMenu;
            //m_MessageReceiverFromServer.OnMessageReceivedFromSever += reOpenLogin;
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += SystemManagerReceived;
        }

        //Login.SetActive(true);
        //Chat.SetActive(false);
        //Menu.SetActive(false);
        //Lobby.SetActive(false);
        ChangeState(GameStates.LoginMenu);
    }


    private void OnDestroy()
    { 
        if (m_MessageReceiverFromServer != null)
        {
            //m_MessageReceiverFromServer.OnMessageReceivedFromSever -= OpenMenu;
            //m_MessageReceiverFromServer.OnMessageReceivedFromSever -= reOpenLogin;
            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= SystemManagerReceived;
        }
    
    }


    //public void OpenChatRoom() 
    //{
    //    // open gameroom Ui and send a msg to server
    //    Login.SetActive(false);
    //    Chat.SetActive(true);
    //    Menu.SetActive(false);
    //    Lobby.SetActive(false);

    //    string OurEnterTheChatMsg = ClientToServerSignifiers.EnterTheChatRoom + "," + GetUserName;
    //    networkClient.GetComponent<NetworkedClient>().SendMessageToHost(OurEnterTheChatMsg);
    //}
    //public void OpenLobbyRoom()
    //{
    //    // open gameroom Ui and send a msg to server
    //    Login.SetActive(false);
    //    Chat.SetActive(false);
    //    Menu.SetActive(false);
    //    Lobby.SetActive(true);

    //    //string OurEnterTheChatMsg = ClientToServerSignifiers.EnterTheChatRoom + "," + GetUserName;
    //    //networkClient.GetComponent<NetworkedClient>().SendMessageToHost(OurEnterTheChatMsg);
    //}
    //public void Logout()
    //{
    //    string logoutMsg = ClientToServerSignifiers.Logout+ ",";
       
    //    networkClient.GetComponent<NetworkedClient>().SendMessageToHost(logoutMsg);
    //}
    //public void reOpenLogin(int signifier, string s)
    //{
    //    switch (signifier)
    //    {
    //        case ServerToClientSignifiers.LogOutComplete:
    //            Login.SetActive(true);
    //            Chat.SetActive(false);
    //            Menu.SetActive(false);
    //            Lobby.SetActive(false);
    //            break;
    //    }
    //}
    
    //public void OpenMenu(int signifier, string s) 
    //{
    //    switch ( signifier)
    //    {
    //        case ServerToClientSignifiers.LoginComplete:
    //            Login.SetActive(false);
    //            Chat.SetActive(false);
    //            Menu.SetActive(true);
    //            break;
    //    }
    //}
    
    void SystemManagerReceived (int sigifier, string s) 
    {
        switch (sigifier)
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
            case ServerToClientSignifiers.ReMatchOfTicTacToeComplete:
                ChangeState(GameStates.TicTacToe);
                break;
            case ServerToClientSignifiers.ExitTacTacToeComplete:
                ChangeState(GameStates.LoginMenu);
                Login.GetComponentInChildren<LogInScript>().ResetLogic();
                Tic_Tac_Toe.GetComponent<TicTacToe>().resetBoard();
                break;
        }
    
    }


    
    public void OpenGameOver()
    {
        //if (i ==1) 
        //GameOver.GetComponentInChildren<Text>().text = "GameOver" + "PlayerOne has Won";
        //else if (i == 2)
        //GameOver.GetComponentInChildren<Text>().text = "GameOver" + "PlayerTwo has Won";
        //else if (i == 3)
        //GameOver.GetComponentInChildren<Text>().text = "GameOver" + "No One has Won";
        GameOver.GetComponent<GameOver>().GamerOverTextChange();
        ChangeState(GameStates.GameOver);

        // networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ReMatchOfTicTacToe + ",");

    }

    public void GameRoomButtonIsPreessed()
    {

         ChangeState(GameStates.WaitingInQueueforOtherPlayer);
        // Debug.Log("You should be pressing tictactoe button right now");
        // string OurEnterTheChatMsg = ClientToServerSignifiers.EnterTheChatRoom + "," + GetUserName;
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + "");
    }

    void ChangeState(int newState) 
    {
        switch (newState) 
        {
            case GameStates.LoginMenu:
                Login.SetActive(true);
                Chat.SetActive(false);
                Menu.SetActive(false);
                Lobby.SetActive(false);
                WaitingInQueue.SetActive(false);
                Tic_Tac_Toe.SetActive(false);
                GameOver.SetActive(false);
                break;
            case GameStates.MainMenu:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(true);
                WaitingInQueue.SetActive(false);
                Lobby.SetActive(false);
                Tic_Tac_Toe.SetActive(false);
                GameOver.SetActive(false);
                break;
            case GameStates.WaitingInQueueforOtherPlayer:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(true);
                Lobby.SetActive(false);
                Tic_Tac_Toe.SetActive(false);
                GameOver.SetActive(false);
                break;
            case GameStates.TicTacToe:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                Lobby.SetActive(false);
                Tic_Tac_Toe.SetActive(true);
                GameOver.SetActive(false);
                break;
            case GameStates.GameOver:
                Login.SetActive(false);
                Chat.SetActive(false);
                Menu.SetActive(false);
                WaitingInQueue.SetActive(false);
                Lobby.SetActive(true);
                Tic_Tac_Toe.SetActive(true);
                GameOver.SetActive(true);
                break;
        }
    }



    static public class GameStates
    {
        public const int LoginMenu = 1;
        public const int MainMenu = 2;

        public const int WaitingInQueueforOtherPlayer = 3;
       
        public const int TicTacToe = 4;

        public const int chatroom = 5;

        public const int GameOver = 5;
    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
