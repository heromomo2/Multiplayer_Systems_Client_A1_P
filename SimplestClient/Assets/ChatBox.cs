using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    GameObject postButton, usermessageInput, userscrollView, networkObject;
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Chat_InputField")
                usermessageInput = go;
            else if (go.name == "Chat_Button")
                postButton = go;
            else if (go.name == "Chat_Scrol View")
                userscrollView = go;
            else if (go.name == "NetworkObject")
                networkObject = go;

        }
        postButton.GetComponent<Button>().onClick.AddListener(PostButtonOnPressed);
    }


    public void PostButtonOnPressed() 
    {
        string ourChatText = usermessageInput.GetComponent<InputField>().text;

        string ourMsg = ClientToServerSignifiers.SendChatMsg + "," + ourChatText;

        networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
