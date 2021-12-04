using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PMButton : MonoBehaviour
{
    #region GameObjects

    public GameObject chat_ui = null;
    #endregion

    #region Variables
    private string player_name;
    public string GetName {get { return player_name; } }
    public string SetName { set { player_name = value; } }
    #endregion
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Chat_UI")
                chat_ui = go;
        }
        this.gameObject.GetComponent<Button>().onClick.AddListener(PriavateMessageButtonOnClick);
    }


    public void PriavateMessageButtonOnClick()
    {
        chat_ui.GetComponent<ChatBox>().SetChatToPrivateMessage(player_name);
    }

    //void OnDestroy()
    //{
    //    //Chat_UI.GetComponent<ChatBox>().SetChatToGlobalMessage();
    //}

}
