using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    #region Variables
    private int[] virtual_board = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int player_one_board_symbol = 1;
    int player_two_board_symbol = 2;
    int move_count = 0;
    bool do_we_have_a_winner = false;
    bool is_waiting_turn = false;
    bool are_we_player_one = false;
    bool is_being_observed = false;
    LinkedList<TicTacToeBoard> board_states;
    private int current_player_board_symbol;
    
    public int GetCurrentPlayerSymbol
    {
        get { return current_player_board_symbol; }
    }
    public enum win_status
    {
        winner,
        Loser,
        draw,
    }
    public win_status our_win_status;
    int test = 0;
    #endregion

    #region GameObjects
    private NetworkedClient message_receiver_from_server = null;
    private GameObject network, system_manger;
    public GameObject[] buttons = new GameObject[9];
    public GameObject m_TicTacToe_Text;
    #endregion


    private void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Network")
                network = go;
            else if (go.name == "SystemManagerObject")
                system_manger = go;
            else if (go.name == "TicTacToe_Text")
                m_TicTacToe_Text = go;
        }
        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += GameLogicMessageReceivedFromServer;
        }

        GiveButtonsPosition();
        current_player_board_symbol = player_one_board_symbol;
        board_states = new LinkedList<TicTacToeBoard>() ;
    }

    

    #region ReceivingDataFromServer/Involved
    void GameLogicMessageReceivedFromServer(int sigifier, string s, TicTacToeBoard tic_tac_toe_board, MatchData match_data)
    {
        switch (sigifier)
        {

            case ServerToClientSignifiers.ReceiveOpponentName:
               // SystemMangerObject.GetComponent<RecordMaker>().GetThePlayerNameRecord(SystemMangerObject.GetComponent<SystemManager>().GetUserName,s);
                break;
            case ServerToClientSignifiers.GameStart:
                Debug.Log("you set as  x or o");
                board_states.Clear(); is_being_observed = false;
                setCurrentPlayerSymbol(int.Parse(s));
                break;
            case ServerToClientSignifiers.OpponentPlayed:
                OpponentpressAButton(int.Parse(s)); is_waiting_turn = true;
                Debug.LogWarning("Your Opponet Just played. Opponent has pressed-> : " + s);
                ActiveButtonUnMarkSpaces(do_we_have_a_winner);
                DisplayWhoTurn(1);
                test = 42;
                break;
            case ServerToClientSignifiers.WaitForOppentMoved:
                Debug.LogWarning(" wait for your OppenMoved");
                DeactiveButtons(); DisplayWhoTurn(2);
                is_waiting_turn = false;
                test = 43;
                break;
            case ServerToClientSignifiers.RematchOfTicTacToeComplete:
                // Debug.LogWarning("resetting board");
                ResetBoards();
                board_states.Clear();
                setCurrentPlayerSymbol(int.Parse(s));
                break;
            case ServerToClientSignifiers.YouareBeingObserved:
                is_being_observed = true;
                break;
            case ServerToClientSignifiers.YouAreNotBeingObserved:
                is_being_observed = false;
                break;
        }
    }
    private void OnDestroy()
    {
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= GameLogicMessageReceivedFromServer;
        }

    }

    void setCurrentPlayerSymbol(int i)
    {
        if (i == 1)
        {
            current_player_board_symbol = player_one_board_symbol;
            Debug.Log("Im player one");
            is_waiting_turn = true;
            are_we_player_one = true;
            board_states.AddLast(new TicTacToeBoard(virtual_board[0], virtual_board[1], virtual_board[2], virtual_board[3], virtual_board[4], virtual_board[5], virtual_board[6], virtual_board[7], virtual_board[8], 41));
            test = 42;
            DisplayWhoTurn(1);
        }
        else
        {
            current_player_board_symbol = player_one_board_symbol;
            Debug.Log("Im player Two");
            are_we_player_one = false;
            is_waiting_turn = false;
            DeactiveButtons();
            board_states.AddLast(new TicTacToeBoard(virtual_board[0], virtual_board[1], virtual_board[2], virtual_board[3], virtual_board[4], virtual_board[5], virtual_board[6], virtual_board[7], virtual_board[8], 41));
            test = 43;
            DisplayWhoTurn(2);
        }
    }


    private void OpponentpressAButton(int ButtonPositon)
    {
        buttons[ButtonPositon].GetComponent<GridSpace>().ButtonOnclick();
    }

    public void MakeAMove(int ButtonPosition)
    {
      
        virtual_board[ButtonPosition] = current_player_board_symbol;

        board_states.AddLast(new TicTacToeBoard(virtual_board[0], virtual_board[1], virtual_board[2], virtual_board[3], virtual_board[4], virtual_board[5], virtual_board[6], virtual_board[7], virtual_board[8], test));

        if (is_being_observed) 
        {
            network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SendObserverData+ "," + virtual_board[0 ].ToString() + ","+ virtual_board[1].ToString() + "," + virtual_board[2].ToString() + "," + virtual_board[3].ToString() + "," + virtual_board[4].ToString() + "," + virtual_board[5].ToString() + "," + virtual_board[6].ToString() + "," + virtual_board[7].ToString() + "," + virtual_board[8].ToString() + "," +  test.ToString());
        }


        CheckForWin();

        // change turn
        if (current_player_board_symbol == player_one_board_symbol)
        {
            current_player_board_symbol = player_two_board_symbol;
        }
        else
        {
            current_player_board_symbol = player_one_board_symbol;
        }

        // draw check
        move_count++;
        if (move_count >= 9 && do_we_have_a_winner == false)
        {
            Debug.Log("IT's a draw. No winner");
            our_win_status = win_status.draw;
            ReachGameOver();
        }


        if (is_waiting_turn == true)
        {
            network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacTacDoAMove + "," + ButtonPosition.ToString());
        }
         
        
    }
    #endregion

    #region DebugPurpose

    void printoutBoard()
    {
        Debug.Log(virtual_board[0].ToString() + " , " + virtual_board[1].ToString() + " , " + virtual_board[2].ToString());
        Debug.Log(virtual_board[3].ToString() + " , " + virtual_board[4].ToString() + " , " + virtual_board[5].ToString());
        Debug.Log(virtual_board[6].ToString() + " , " + virtual_board[7].ToString() + " , " + virtual_board[8].ToString());
    }


    void printoutAllBoards()
    {
        // TicTacToe tempB;
       foreach (TicTacToeBoard b in board_states)
        {
            Debug.Log("\n board: " );
            Debug.Log(" [ "+ b.top_left_.ToString() + " , " + b.top_mid_.ToString() + " , " + b.top_right_.ToString() + " ] ");
            Debug.Log(" [ " + b.mid_left_.ToString() + " , " + b.mid_mid_.ToString() + " , " + b.mid_right_.ToString()+ " ] ");
            Debug.Log(" [ " + b.bot_left_.ToString() + " , " + b.bot_mid_.ToString() + " , " + b.bot_right_.ToString()+ " ] ");
            Debug.Log("\n");
            //if (b)
        }
       // tempB= m_ListOFBoard.First.Value();
    }

    #endregion

# WinConditions


    void CheckForWin()
    {
        Debug.Log("CheckforWin was called");
        // check by row
        for (int i= 0; i < virtual_board.Length; i += 3)
        {
            if (virtual_board[i] == current_player_board_symbol && virtual_board[i+1] == current_player_board_symbol && virtual_board[i+2] == current_player_board_symbol) 
            {
                Debug.Log("you won by row");
                do_we_have_a_winner = true;
                DeactiveButtons();
                ReachGameOver();
                break;
            }
        }
        /// check by column
        for (int i = 0; i < 3; i++)
        {
            if (virtual_board[i] == current_player_board_symbol && virtual_board[i + 3] == current_player_board_symbol && virtual_board[i + 6] == current_player_board_symbol)
            {
                Debug.Log("you won by column");
                do_we_have_a_winner = true;
                DeactiveButtons();
                ReachGameOver();
                break;
            }
        }
        // check by Diagonal
        if (virtual_board[0] == current_player_board_symbol && virtual_board[4] == current_player_board_symbol && virtual_board[8] == current_player_board_symbol ||
            virtual_board[2] == current_player_board_symbol && virtual_board[4] == current_player_board_symbol && virtual_board[6] == current_player_board_symbol)
        {
            do_we_have_a_winner = true;
            Debug.Log("you won by Diagonal");
            DeactiveButtons();
            ReachGameOver();
        }
    }

    bool CheckIfWeWon(int Playernumber) 
    {
        for (int i = 0; i < virtual_board.Length; i += 3)
        {
            if (virtual_board[i] == Playernumber && virtual_board[i + 1] == Playernumber && virtual_board[i + 2] == Playernumber)
            {
                return true;
               
            }
        }
        /// check by column
        for (int i = 0; i < 3; i++)
        {
            if (virtual_board[i] == Playernumber && virtual_board[i + 3] == Playernumber && virtual_board[i + 6] == Playernumber)
            {
                return true;
            }
        }
        // check by Diagonal
        if (virtual_board[0] == Playernumber && virtual_board[4] == Playernumber && virtual_board[8] == Playernumber ||
            virtual_board[2] == Playernumber && virtual_board[4] == Playernumber && virtual_board[6] == Playernumber)
        {
            return true;

        }
        return false;
    }

  #region Interfacecode
    void DeactiveButtons() 
    {
        foreach (GameObject b in buttons) 
        {
            b.GetComponent<Button>().interactable = false;
        }
    }

    void ActiveButtonUnMarkSpaces( bool do_we_have_a_winner)
    {
        if (do_we_have_a_winner== false)
        {

            for (int i = 0; i < 9; i++)
            {
                if (virtual_board[i] == 0)
                {
                    buttons[i].GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    void DisplayWhoTurn(int i)
    {
        switch (i)
        {
            case 0:
                m_TicTacToe_Text.GetComponent<Text>().text = "";
                break;
            case 1:
                m_TicTacToe_Text.GetComponent<Text>().text = "It's your turn now";
                break;
            case 2:
                m_TicTacToe_Text.GetComponent<Text>().text = "Wait your turn";
                break;

        }

    }


    void GiveButtonsPosition()
    {
        int PositionNumber = 0;

        foreach (GameObject b in buttons)
        {

            b.GetComponent<GridSpace>().SetMyPositionOntheBoard = PositionNumber;
            PositionNumber++;
        }
    }
    #endregion

    public void ResetBoards()
    {
        foreach (GameObject b in buttons)
        {
            b.GetComponent<Button>().interactable = true;
            b.GetComponentInChildren<Text>().text = ""; 
        }
        for(int i= 0; i < virtual_board.Length; i++) 
        {
            virtual_board[i] = 0;
        }
        move_count = 0;
        do_we_have_a_winner = false;
    }
   void ReachGameOver()
   {
        if (are_we_player_one)
        {
            if (CheckIfWeWon(1))
            {
                our_win_status = win_status.winner;
            }
            else if (CheckIfWeWon(2))
            {
                our_win_status = win_status.Loser;
            }
        }
        else
        {
            if (CheckIfWeWon(2))
            {
                our_win_status = win_status.winner;
            }
            else if (CheckIfWeWon(1))
            {
                our_win_status = win_status.Loser;
            }

        }

        foreach (TicTacToeBoard b in board_states)
        {
           // SystemMangerObject.GetComponent<RecordMaker>().Give_TicTacToeBoard(b.topleft, b.topmid, b.topright, b.midleft, b.midmid, b.midright, b.botleft, b.botmid, b.botright, b.WhosMove);
        }
        board_states.Clear();
        system_manger.GetComponent<SystemManager>().OpenGameOver();
   }



    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            printoutBoard();
            Debug.Log("CurrentPlayer -> " + current_player_board_symbol );
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetBoards();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            printoutAllBoards();
        }

    }


}
 