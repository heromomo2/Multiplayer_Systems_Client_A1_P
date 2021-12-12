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
    int our_player_number;
    bool is_being_observed = false;
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
    int whos_move_for_spectaste = 0;
    #endregion

    #region GameObjects
    private NetworkedClient message_receiver_from_server = null;
    private GameObject network, system_manger;
    public List<Button> buttons;
    public GameObject game_logic_turn_text;
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
                game_logic_turn_text = go;
        }
        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += GameLogicMessageReceivedFromServer;
        }

        buttons = new List <Button>();
        PlaceButtonsInList();
        GiveButtonsPosition();
       
       
    }

    #region Initialzing
    void InitialzingOurPlayer(int set_up_player_by_number)
    {
        if (set_up_player_by_number == 1)
        {
           
            is_waiting_turn = false;
            our_player_number = 1;
            whos_move_for_spectaste = 42;
            DisplayWhoTurnToUser(1);
            current_player_board_symbol = player_one_board_symbol;
        }
        else
        {
            our_player_number = 2;
            is_waiting_turn = true;
            DeactiveButtons();
            whos_move_for_spectaste = 43;
            DisplayWhoTurnToUser(2);
            current_player_board_symbol = player_one_board_symbol;
        }
    }

    void GiveButtonsPosition()
    {
        int button_position_number = 0;

        foreach (Button button in buttons)
        {
            button.GetComponent<GridSpace>().SetPositionOnTheBoard = button_position_number;
            button_position_number++;
        }
    }

    void PlaceButtonsInList()
    {
        for (int i = 0; i <= 9; i++)
        {
            GameObject temp_game_object;

            temp_game_object = GameObject.Find("TicTacToe_Button" + i);
          
            if (temp_game_object != null)
            {
                buttons.Add(temp_game_object.GetComponent<Button>());
            }
        }
    }
    #endregion


    #region ReceivingDataFromServer/Involved
    void GameLogicMessageReceivedFromServer(int sigifier, string s, TicTacToeBoard tic_tac_toe_board, MatchData match_data)
    {
        switch (sigifier)
        {
            case ServerToClientSignifiers.GameStart:
                is_being_observed = false;
                InitialzingOurPlayer(int.Parse(s));
                break;
            case ServerToClientSignifiers.OpponentPlayed:
                whos_move_for_spectaste = 42;
                OpponentPressAButton(int.Parse(s)); 
                is_waiting_turn = false;
                ActiveButtonsThatUnmarkSpaces(do_we_have_a_winner);
                DisplayWhoTurnToUser(1);               
                break;
            case ServerToClientSignifiers.WaitForOppentMoved:
                whos_move_for_spectaste = 43;
                DeactiveButtons();
                DisplayWhoTurnToUser(2);
                is_waiting_turn = true;
                break;
            case ServerToClientSignifiers.RematchOfTicTacToeComplete:
                ResetBoards();
                InitialzingOurPlayer(int.Parse(s));
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

   
    private void OpponentPressAButton(int button_position)
    {
        buttons[button_position].GetComponent<GridSpace>().ButtonOnClick();
    }

    public void MakeAMove(int button_position)
    {
      
        virtual_board[button_position] = current_player_board_symbol;



        // this is only true when we are being  spectated
        // we are sending our observer the game state
        // this gets call everytime you and your opponent make a move
        if (is_being_observed) 
        {
            network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SendObserverData+ "," + virtual_board[0 ].ToString() + ","+ virtual_board[1].ToString() + "," + virtual_board[2].ToString() + "," + virtual_board[3].ToString() + "," + virtual_board[4].ToString() + "," + virtual_board[5].ToString() + "," + virtual_board[6].ToString() + "," + virtual_board[7].ToString() + "," + virtual_board[8].ToString() + "," +  whos_move_for_spectaste.ToString());
        }


        // our win condition
       // it's also how we know we have complete a game
        CheckIfWeWon( current_player_board_symbol);

        // change turn symbol
        // it's change the Current player symbol, so we don't mess up virtual_board if the opponent click a button
        if (current_player_board_symbol == player_one_board_symbol)
        {
            current_player_board_symbol = player_two_board_symbol;
        }
        else
        {
            current_player_board_symbol = player_one_board_symbol;
        }

        // Increment move counter
        move_count++;
        

        /// this check is here to  prevent sending a msg when opponent clicks a button
        if (is_waiting_turn == false)
        {
            network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacTacDoAMove + "," + button_position.ToString());
        }
         
        
    }
    #endregion


    #region WinConditionAndGameOver

    bool CheckIfWeWon(int player_number) 
    {
        for (int i = 0; i < virtual_board.Length; i += 3)
        {
            if (virtual_board[i] == player_number && virtual_board[i + 1] == player_number && virtual_board[i + 2] == player_number)
            {
                do_we_have_a_winner = true;
                DeactiveButtons();

                // check Which player is the winnner here
                if (our_player_number == player_number)
                {
                    our_win_status = win_status.winner;
                }
                else
                {
                    our_win_status = win_status.Loser;
                }

                ReachGameOver();
                return true;
            }
        }
        /// check by column
        for (int i = 0; i < 3; i++)
        {
            if (virtual_board[i] == player_number && virtual_board[i + 3] == player_number && virtual_board[i + 6] == player_number)
            {
                do_we_have_a_winner = true;

                DeactiveButtons();

                // check Which player is the winnner here
                if (our_player_number == player_number)
                {
                    our_win_status = win_status.winner;
                }
                else
                {
                    our_win_status = win_status.Loser;
                }

                ReachGameOver();
                return true;
            }
        }
        // check by Diagonal
        if (virtual_board[0] == player_number && virtual_board[4] == player_number && virtual_board[8] == player_number ||
            virtual_board[2] == player_number && virtual_board[4] == player_number && virtual_board[6] == player_number)
        {
            do_we_have_a_winner = true;
            DeactiveButtons();
            // check Which player is the winnner here
            if (our_player_number == player_number)
            {
                our_win_status = win_status.winner;
            }
            else
            {
                our_win_status = win_status.Loser;
            }

            ReachGameOver();
            return true;
        }

        // checking is a draw
        if (move_count >= 9 && do_we_have_a_winner == false)
        {
            Debug.Log("IT's a draw. No winner");
            our_win_status = win_status.draw;
            ReachGameOver();
        }

        return false;
    }
    void ReachGameOver()
    {
        system_manger.GetComponent<SystemManager>().OpenGameOver();
    }
    #endregion


    #region Interfacecode
    void DeactiveButtons() 
    {
        foreach (Button button in buttons) 
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    void ActiveButtonsThatUnmarkSpaces( bool do_we_have_a_winner)
    {
        if (do_we_have_a_winner== false)
        {

            for (int i = 0; i < buttons.Count; i++)
            {
                if (virtual_board[i] == 0)
                {
                    buttons[i].GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    void DisplayWhoTurnToUser(int i)
    {
        switch (i)
        {
            case 0:
                game_logic_turn_text.GetComponent<Text>().text = "";
                break;
            case 1:
                game_logic_turn_text.GetComponent<Text>().text = "It's your turn now";
                break;
            case 2:
                game_logic_turn_text.GetComponent<Text>().text = "Wait your turn";
                break;

        }

    }


    public void ResetBoards()
    {
        foreach (Button button in buttons)
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponentInChildren<Text>().text = "";
        }
        for (int i = 0; i < virtual_board.Length; i++)
        {
            virtual_board[i] = 0;
        }
        move_count = 0;
        do_we_have_a_winner = false;
    }


    #endregion


}
 