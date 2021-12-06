using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{


    #region Variables
  
    #endregion

    #region GameObjects
    
    #endregion 

    int test = 0;

  LinkedList <TicTacToeBoard> m_ListOFBoard;

    private NetworkedClient m_MessageReceiverFromServer = null;
    private GameObject networkClient, SystemMangerObject;
    public GameObject[] ListOFButton = new GameObject[9];
    private int[] mboard = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int Oneplayersymbol = 1;
    int Twoplayersymbol = 2;
    int movecount = 0;
    bool DoWeHaveAWinner = false;
    bool m_IsWaitTurn = false;
    bool m_IsOnePlayer = false;
    bool m_IsBeingObserved = false;

    private int CurrentPlayer;

    public GameObject m_TicTacToe_Text;
    public int GetCurrentPlayerSymbol
    {
        get { return CurrentPlayer; }
    }

    public enum m_WinnerStatus
    {
        winner,
        Loser,
        draw,
    }

    public m_WinnerStatus m_ws;





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
            else if (go.name == "TicTacToe_Text")
                m_TicTacToe_Text = go;
        }
        m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer += TicTacToeMessageReceived;
        }

        GiveButtonsPosition();
        CurrentPlayer = Oneplayersymbol;
        m_ListOFBoard = new LinkedList<TicTacToeBoard>() ;
    }

    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= TicTacToeMessageReceived;
        }

    }


    void TicTacToeMessageReceived(int sigifier, string s, TicTacToeBoard t, MatchData matchData)
    {
        switch (sigifier)
        {

            case ServerToClientSignifiers.ReceiveOpponentName:
               // SystemMangerObject.GetComponent<RecordMaker>().GetThePlayerNameRecord(SystemMangerObject.GetComponent<SystemManager>().GetUserName,s);
                break;
            case ServerToClientSignifiers.GameStart:
                Debug.Log("you set as  x or o");
                m_ListOFBoard.Clear(); m_IsBeingObserved = false;
                setCurrentPlayerSymbol(int.Parse(s));
                break;
            case ServerToClientSignifiers.OpponentPlayed:
                OpponentpressAButton(int.Parse(s)); m_IsWaitTurn = true;
                Debug.LogWarning("Your Opponet Just played. Opponent has pressed-> : " + s);
                activeButtonUnMarkSpaces(DoWeHaveAWinner);
                DisplayWhoTurn(1);
                test = 42;
                break;
            case ServerToClientSignifiers.WaitForOppentMoved:
                Debug.LogWarning(" wait for your OppenMoved");
                DeactiveButtons(); DisplayWhoTurn(2);
                m_IsWaitTurn = false;
                test = 43;
                break;
            case ServerToClientSignifiers.RematchOfTicTacToeComplete:
                // Debug.LogWarning("resetting board");
                resetBoard();
                m_ListOFBoard.Clear();
                setCurrentPlayerSymbol(int.Parse(s));
                break;
            case ServerToClientSignifiers.YouareBeingObserved:
                m_IsBeingObserved = true;
                break;
            case ServerToClientSignifiers.YouAreNotBeingObserved:
                m_IsBeingObserved = false;
                break;
        }
    }


    void setCurrentPlayerSymbol(int i)
    {
        if (i == 1)
        {
            CurrentPlayer = Oneplayersymbol;
            Debug.Log("Im player one");
            m_IsWaitTurn = true;
            m_IsOnePlayer = true;
            m_ListOFBoard.AddLast(new TicTacToeBoard(mboard[0], mboard[1], mboard[2], mboard[3], mboard[4], mboard[5], mboard[6], mboard[7], mboard[8], 41));
            test = 42;
            DisplayWhoTurn(1);
        }
        else
        {
            CurrentPlayer = Oneplayersymbol;
            Debug.Log("Im player Two");
            m_IsOnePlayer = false;
            m_IsWaitTurn = false;
            DeactiveButtons();
            m_ListOFBoard.AddLast(new TicTacToeBoard(mboard[0], mboard[1], mboard[2], mboard[3], mboard[4], mboard[5], mboard[6], mboard[7], mboard[8], 41));
            test = 43;
            DisplayWhoTurn(2);
        }
    }


    private void OpponentpressAButton(int ButtonPositon)
    {
        ListOFButton[ButtonPositon].GetComponent<GridSpace>().ButtonOnclick();
    }

    public void MakeAMove(int ButtonPosition)
    {
      


        mboard[ButtonPosition] = CurrentPlayer;

        m_ListOFBoard.AddLast(new TicTacToeBoard(mboard[0], mboard[1], mboard[2], mboard[3], mboard[4], mboard[5], mboard[6], mboard[7], mboard[8], test));

        if (m_IsBeingObserved) 
        {
            networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SendObserverData+ "," + mboard[0 ].ToString() + ","+ mboard[1].ToString() + "," + mboard[2].ToString() + "," + mboard[3].ToString() + "," + mboard[4].ToString() + "," + mboard[5].ToString() + "," + mboard[6].ToString() + "," + mboard[7].ToString() + "," + mboard[8].ToString() + "," +  test.ToString());
        }


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
        if (movecount >= 9 && DoWeHaveAWinner == false)
        {
            Debug.Log("IT's a draw. No winner");
            m_ws = m_WinnerStatus.draw;
            ReachGameOver();
        }


        if (m_IsWaitTurn == true)
        {
            networkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacTacDoAMove + "," + ButtonPosition.ToString());
        }
         
        
            
        
    }


    void printoutBoard()
    {
        Debug.Log(mboard[0].ToString() + " , " + mboard[1].ToString() + " , " + mboard[2].ToString());
        Debug.Log(mboard[3].ToString() + " , " + mboard[4].ToString() + " , " + mboard[5].ToString());
        Debug.Log(mboard[6].ToString() + " , " + mboard[7].ToString() + " , " + mboard[8].ToString());
    }


    void printoutAllBoards()
    {
        // TicTacToe tempB;
       foreach (TicTacToeBoard b in m_ListOFBoard)
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
                ReachGameOver();
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
                ReachGameOver();
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
            ReachGameOver();
        }
    }

     bool CheckIfWeWon(int Playernumber) 
    {
        for (int i = 0; i < mboard.Length; i += 3)
        {
            if (mboard[i] == Playernumber && mboard[i + 1] == Playernumber && mboard[i + 2] == Playernumber)
            {
                return true;
               
            }
        }
        /// check by column
        for (int i = 0; i < 3; i++)
        {
            if (mboard[i] == Playernumber && mboard[i + 3] == Playernumber && mboard[i + 6] == Playernumber)
            {
                return true;
            }
        }
        // check by Diagonal
        if (mboard[0] == Playernumber && mboard[4] == Playernumber && mboard[8] == Playernumber ||
            mboard[2] == Playernumber && mboard[4] == Playernumber && mboard[6] == Playernumber)
        {
            return true;

        }
        return false;
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


    public void resetBoard()
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
   void ReachGameOver()
   {
        if (m_IsOnePlayer)
        {
            if (CheckIfWeWon(1))
            {
                m_ws = m_WinnerStatus.winner;
            }
            else if (CheckIfWeWon(2))
            {
                m_ws = m_WinnerStatus.Loser;
            }
        }
        else
        {
            if (CheckIfWeWon(2))
            {
                m_ws = m_WinnerStatus.winner;
            }
            else if (CheckIfWeWon(1))
            {
                m_ws = m_WinnerStatus.Loser;
            }

        }

        foreach (TicTacToeBoard b in m_ListOFBoard)
        {
           // SystemMangerObject.GetComponent<RecordMaker>().Give_TicTacToeBoard(b.topleft, b.topmid, b.topright, b.midleft, b.midmid, b.midright, b.botleft, b.botmid, b.botright, b.WhosMove);
        }
        m_ListOFBoard.Clear();
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
        else if (Input.GetKeyDown(KeyCode.M))
        {
            printoutAllBoards();
        }

    }


   

}
 public class TicTacToeBoard 
 {
    public int top_left_, top_mid_, top_right_, mid_left_, mid_mid_, mid_right_, bot_left_, bot_mid_, bot_right_;
    public int whos_move_;
        

    public  TicTacToeBoard (int tl, int tm, int tr, int ml, int mm,int mr, int bl, int bm, int br, int wsm )
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

    public int[] GetTicTacToeBoardAsArray(TicTacToeBoard b) 
    {
        int[] array = new int[9] { b.top_left_ , b.top_mid_ ,b.top_right_ , b.mid_left_, b.mid_mid_, mid_right_, b.bot_left_, b.bot_mid_,b.bot_right_};
        return array;
    }

 }