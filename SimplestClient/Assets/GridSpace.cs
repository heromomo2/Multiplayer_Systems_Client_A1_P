using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridSpace : MonoBehaviour
{
    #region GameObjects
    private GameObject game_logic;
    #endregion

    #region Variables
    private int my_positon_on_the_board;

    public int GetPositionOnTheBoard   
    {
         get{return my_positon_on_the_board; }
    }
    public int SetPositionOnTheBoard
    {
        set { my_positon_on_the_board = value; }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Game_UI")
                game_logic = go;
        }
        this.gameObject.GetComponent<Button>().onClick.AddListener(ButtonOnClick);
    }

   

    public void ButtonOnClick() 
    {
        if(game_logic.GetComponent<GameLogic>().GetCurrentPlayerSymbol == 1 )
        {
            this.gameObject.GetComponentInChildren<Text>().text = "X";
        }
        else 
        {
            this.gameObject.GetComponentInChildren<Text>().text = "O";
        }

        game_logic.GetComponent<GameLogic>().MakeAMove(GetPositionOnTheBoard);
        this.gameObject.GetComponent<Button>().interactable = false;
    }

}
