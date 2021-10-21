using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    public  GameObject []ListOFButton = new GameObject[9];
    private int [] mboard = {0,0,0,0,0,0,0,0,0 };
     int Oneplayersymbol = 1;
     int Twoplayersymbol = 2;
     int movecount= 0;
    bool DoWeHaveAWinner = false;
   private  int CurrentPlayer;
  

    public int GetCurrentPlayerSymbol
    {
        get { return CurrentPlayer; }
    }
   

    //bool playerTu

    private void Start()
    {
       GiveButtonsPosition();
        CurrentPlayer = Oneplayersymbol;
    }


    public void MakeAMove(int ButtonPosition)
    {
       // 
        mboard[ButtonPosition] = CurrentPlayer;
        CheckForWin();
        // change turn
        if(CurrentPlayer == Oneplayersymbol)
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
    
        }
    }

    void DeactiveButtons() 
    {
        foreach (GameObject b in ListOFButton) 
        {
            b.GetComponent<Button>().interactable = false;
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
