using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridSpace : MonoBehaviour
{


    private GameObject GameController;
    private int m_MyPositonOnTheBoard;

    public int GetMyPositionOntheBoard   
    {
         get{return m_MyPositonOnTheBoard; }
    }
    public int SetMyPositionOntheBoard
    {
        set { m_MyPositonOnTheBoard = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Game_UI")
                GameController = go;
        }
    }

    void resetBoard()
    {
        
    }

    public void ButtonOnclick() 
    {
        if(GameController.GetComponent<GameLogic>().GetCurrentPlayerSymbol == 1 )
        {
            this.gameObject.GetComponentInChildren<Text>().text = "X";
        }
        else 
        {
            this.gameObject.GetComponentInChildren<Text>().text = "O";
        }
        GameController.GetComponent<GameLogic>().MakeAMove(GetMyPositionOntheBoard);
        this.gameObject.GetComponent<Button>().interactable = false;
    }

}
