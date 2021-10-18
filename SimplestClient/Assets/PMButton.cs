using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PMButton : MonoBehaviour
{
    public GameObject Chat_UI = null;
   
    private string Name;
    public string GetName {get { return Name; } }
    public string SetName { set { Name = value; } }
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Chat_UI")
                Chat_UI = go;
        }
        this.gameObject.GetComponent<Button>().onClick.AddListener(PMbutttonOnClick);
    }


    public void PMbutttonOnClick()
    {
        Chat_UI.GetComponent<ChatBox>().SetChatToPrivateMessage(Name);
    }

    void OnDestroy()
    {
        Chat_UI.GetComponent<ChatBox>().SetChatToGlobalMessage();
    }

}
