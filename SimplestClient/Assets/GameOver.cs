using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
    
{
    public Text GameOverTitle;
    private NetworkedClient m_MessageReceiverFromServer = null;
    private GameObject NetWorkObject, Tic_Tac_Toe, ReMatchButton;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Network")
                NetWorkObject = go;
            else if (go.name == "Game_UI")
                Tic_Tac_Toe = go;
            else if (go.name == "GameOverScreen_RematchButton")
                ReMatchButton = go;
        }
        m_MessageReceiverFromServer = NetWorkObject.GetComponent<NetworkedClient>();
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer += GameOverReceived;
        }
    }
    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= GameOverReceived;
        }
    }


    void GameOverReceived(int signifier, string s, TicTacToeBoard t, MatchData matchData)
    {
        if(signifier == ServerToClientSignifiers.PreventRematch) 
        {
            ReMatchButton.GetComponent<Button>().interactable = false;
        }
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            ReMatchButton.GetComponent<Button>().interactable = true;
        }
    }

    public void GamerOverTextChange ()
    {
        if (Tic_Tac_Toe.GetComponent<GameLogic>().our_win_status == GameLogic.win_status.Loser)
        {
            GameOverTitle.text = "Game Over \n You lost";
        }
        else if (Tic_Tac_Toe.GetComponent<GameLogic>().our_win_status == GameLogic.win_status.winner)
        {
            GameOverTitle.text = "Game Over \n You winner";
        }
        else if (Tic_Tac_Toe.GetComponent<GameLogic>().our_win_status == GameLogic.win_status.draw)
        {
            GameOverTitle.text = "Game Over \n No winners";
        }
    }

    public void ReMatchButtonIsPreessed()
    {
        NetWorkObject.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.RematchOfTicTacToe + ",");
    }

    public void QuitButtonIsPreessed()
    {
        NetWorkObject.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ExitTacTacToe+ ",");
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
