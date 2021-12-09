using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOver : MonoBehaviour

{
    #region GameObject/UI
    public GameObject game_over_title_text;
    private NetworkedClient message_receiver_from_server = null;
    private GameObject network, game_logic, rematch_button, quit_button;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //Initializing
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Network")
                network = go;
            else if (go.name == "Game_UI")
                game_logic = go;
            else if (go.name == "GameOverScreen_RematchButton")
                rematch_button = go;
            else if (go.name == "GameOverScreen_QuitButton")
                quit_button = go;
            else if (go.name == "GameOverScreen_TitleText")
                game_over_title_text = go;
        }
        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += GameOverReceivedMessageFromTheServer;
        }

        rematch_button.GetComponent<Button>().onClick.AddListener(RematchButtonIsPreessed);
        quit_button.GetComponent<Button>().onClick.AddListener(QuitButtonIsPreessed);
    }
  
    #region ReceivedMessageFromTheServer

    void GameOverReceivedMessageFromTheServer(int signifier, string s, TicTacToeBoard t, MatchData match_datas)
    {
        if(signifier == ServerToClientSignifiers.PreventRematch) 
        {
            RematchButtonChanges(1);
        }
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            RematchButtonChanges(2);
        }
    }
    private void OnDestroy()
    {
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= GameOverReceivedMessageFromTheServer;
        }
    }

    private void RematchButtonChanges(int number)
    {
        if (number == 1)
        {
            rematch_button.GetComponent<Button>().interactable = false;
            rematch_button.GetComponent<Text>().text = "No Rematch";
        }
        else if ( number == 2) 
        {
            rematch_button.GetComponent<Button>().interactable = true;
            rematch_button.GetComponent<Text>().text = "Rematch";
        }  
    }

    #endregion

    #region InterfaceCode

    private void  RematchButtonIsPreessed()
    {
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.RematchOfTicTacToe + ",");
    }

    private void QuitButtonIsPreessed()
    {
        network.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ExitTacTacToe+ ",");
    }

    public void GamerOverMessageText()
    {
        if (game_logic.GetComponent<GameLogic>().our_win_status == GameLogic.win_status.Loser)
        {
            game_over_title_text.GetComponent<Text>().text = "Game Over \n You lost";
        }
        else if (game_logic.GetComponent<GameLogic>().our_win_status == GameLogic.win_status.winner)
        {
            game_over_title_text.GetComponent<Text>().text = "Game Over \n You winner";
        }
        else if (game_logic.GetComponent<GameLogic>().our_win_status == GameLogic.win_status.draw)
        {
            game_over_title_text.GetComponent<Text>().text = "Game Over \n No winners";
        }
    }

    #endregion
}
