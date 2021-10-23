using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
    
{
    public Text GameOverTitle;
    private NetworkedClient m_MessageReceiverFromServer = null;
    private GameObject NetWorkObject, Tic_Tac_Toe;
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
        }
        m_MessageReceiverFromServer = NetWorkObject.GetComponent<NetworkedClient>();
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += GameOverReceived;
        }
    }
    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= GameOverReceived;
        }
    }


    void GameOverReceived(int signifier, string s)
    {
        //if(ServerToClientSignifiers)

    }

    public void GamerOverTextChange ()
    {
        if (Tic_Tac_Toe.GetComponent<TicTacToe>().m_ws == TicTacToe.m_WinnerStatus.Loser)
        {
            GameOverTitle.text = "Game Over \n You lost";
        }
        else if (Tic_Tac_Toe.GetComponent<TicTacToe>().m_ws == TicTacToe.m_WinnerStatus.winner)
        {
            GameOverTitle.text = "Game Over \n You winner";
        }
        else if (Tic_Tac_Toe.GetComponent<TicTacToe>().m_ws == TicTacToe.m_WinnerStatus.draw)
        {
            GameOverTitle.text = "Game Over \n No winners";
        }
    }

    public void ReMatchButtonIsPreessed()
    {
        NetWorkObject.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ReMatchOfTicTacToe + ",");
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
