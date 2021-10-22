using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    private NetworkedClient m_MessageReceiverFromServer = null;
    private GameObject networkClient,SystemMangerObject;
    public  GameObject []ListOFButton = new GameObject[9];
    private int [] mboard = {0,0,0,0,0,0,0,0,0 };
     int Oneplayersymbol = 1;
     int Twoplayersymbol = 2;
     int movecount= 0;
    bool DoWeHaveAWinner = false;
    bool m_IsWaitTurn = false;
    private  int CurrentPlayer;
  

    public int GetCurrentPlayerSymbol
    {
        get { return CurrentPlayer; }
    }
   

    //bool playerTu

    private void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Network")
                networkClient = go;
            else if (go.name == "SystemManagerObject")
                     SystemMangerObject = go;
        }
        m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += TicTacToeMessageReceived;
        }

        GiveButtonsPosition();
        CurrentPlayer = Oneplayersymbol;
    }

    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= TicTacToeMessageReceived;
        }

    }


    void TicTacToeMessageReceived(int sigifier, string s)
    {
        switch (sigifier)
        {
            case ServerToClientSignifiers.GameStart:
                Debug.Log("you set as  x or o");
                setCurrentPlayerSymbol(int.Parse(s));
                break;
            case ServerToClientSignifiers.OpponentPlayed:
                
                OpponentpressAButton(int.Parse(s)); m_IsWaitTurn = true;
                Debug.LogWarning("Your Opponet Just played. Opponent has pressed-> : " + s);
                activeButtonUnMarkSpaces(DoWeHaveAWinner);
                break;
            case ServerToClientSignifiers.WaitForOppentMoved:
                Debug.LogWarning(" wait for your OppenMoved");
                DeactiveButtons();
                m_IsWaitTurn = false;
                break;
            case ServerToClientSignifiers.ReMatchOfTicTacToeComplete:
               // Debug.LogWarning("resetting board");
                resetBoard(); setCurrentPlayerSymbol(int.Parse(s));
                break;
        }
    }


    void setCurrentPlayerSymbol(int i)
    {
        if(i == 1) 
        {
           // CurrentPlayer = Oneplayersymbol;
            Debug.Log("Im player one");
            m_IsWaitTurn = true;
        }
        else 
        {
            //CurrentPlayer = Twoplayersymbol;
            Debug.Log("Im player Two");
            m_IsWaitTurn = false;
            DeactiveButtons();
        }
    }



    private void OpponentpressAButton (int ButtonPositon) 
    {
        ListOFButton[ButtonPositon].GetComponent<GridSpace>().ButtonOnclick();
    }

    public void MakeAMove(int ButtonPosition)
    {
       // 
        mboard[ButtonPosition] = CurrentPlayer;
        CheckForWin();


        // change turn
        if (CurrentPlayer == Oneplayersymbol)
        {
            CurrentPlayer = Twoplayersymbol;
        }
        else
        {
            CurrentPlayer = Oneplayersymbol;
        }

        // draw check
        movecount++;
        if(movecount >= 9 && DoWeHaveAWinner == false)
        {
            Debug.Log("IT's a draw. No winner");
        }


        if (m_IsWaitTurn == true) 
        {
            networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToesSomethingSomthing + "," + ButtonPosition.ToString() );
        }
    }


    void printoutBoard()
    {
        Debug.Log(mboard[0].ToString()+ " , " + mboard[1].ToString() + " , " + mboard[2].ToString());
        Debug.Log(mboard[3].ToString() + " , " + mboard[4].ToString() + " , " + mboard[5].ToString());
        Debug.Log(mboard[6].ToString() + " , " + mboard[7].ToString() + " , " + mboard[8].ToString());
    }

    void GiveButtonsPosition() 
    {
        int PositionNumber = 0;
        foreach(GameObject b in ListOFButton)
        {
            
            b.GetComponent<GridSpace>().SetMyPositionOntheBoard = PositionNumber;
            PositionNumber++;
        }
    }

    void CheckForWin()
    {
        Debug.Log("CheckforWin was called");
        // check by row
        for (int i= 0; i < mboard.Length; i += 3)
        {
            if (mboard[i] == CurrentPlayer && mboard[i+1] == CurrentPlayer && mboard[i+2] == CurrentPlayer) 
            {
                Debug.Log("you won by row");
                DoWeHaveAWinner = true;
                DeactiveButtons();
                RearchGameOver();
                break;
            }
        }
        /// check by column
        for (int i = 0; i < 3; i++)
        {
            if (mboard[i] == CurrentPlayer && mboard[i + 3] == CurrentPlayer && mboard[i + 6] == CurrentPlayer)
            {
                Debug.Log("you won by column");
                DoWeHaveAWinner = true;
                DeactiveButtons();
                RearchGameOver();
                break;
            }
        }
        // check by Diagonal
        if (mboard[0] == CurrentPlayer && mboard[4] == CurrentPlayer && mboard[8] == CurrentPlayer ||
            mboard[2] == CurrentPlayer && mboard[4] == CurrentPlayer && mboard[6] == CurrentPlayer)
        {
            DoWeHaveAWinner = true;
            Debug.Log("you won by Diagonal");
            DeactiveButtons();
            RearchGameOver();

        }
    }

    void DeactiveButtons() 
    {
        foreach (GameObject b in ListOFButton) 
        {
            b.GetComponent<Button>().interactable = false;
        }
    }

    void activeButtonUnMarkSpaces( bool DoWeHaveAWinner)
    {
        if (DoWeHaveAWinner== false)
        {

            for (int i = 0; i < 9; i++)
            {
                if (mboard[i] == 0)
                {
                    ListOFButton[i].GetComponent<Button>().interactable = true;
                }
            }
        }
    }


    void resetBoard()
    {
        foreach (GameObject b in ListOFButton)
        {
            b.GetComponent<Button>().interactable = true;
            b.GetComponentInChildren<Text>().text = "";
           
        }
        for(int i= 0; i < mboard.Length; i++) 
        {
            mboard[i] = 0;
        }
        movecount = 0;
        DoWeHaveAWinner = false;
    }
   void RearchGameOver()
   {
        SystemMangerObject.GetComponent<SystemManager>().OpenGameOver();
   }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            printoutBoard();
            Debug.Log("CurrentPlayer -> " + CurrentPlayer );
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            resetBoard();
        }
        
    }
   
}
