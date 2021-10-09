using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    private NetworkedClient m_MessageReceiverFromServerS = null;

    GameObject postButton, usermessageInput, userscrollView, networkObject;
    public GameObject prefab = null;
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
            else if (go.name == "Network")
                networkObject = go;
        }
        postButton.GetComponent<Button>().onClick.AddListener(PostButtonOnPressed);

        m_MessageReceiverFromServerS = networkObject.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServerS != null)
        {
            m_MessageReceiverFromServerS.OnMessageReceivedFromServerS += AddMessageToChat;
        }
    }


    private void OnDestroy()
    {
        if (m_MessageReceiverFromServerS != null)
        {
            m_MessageReceiverFromServerS.OnMessageReceivedFromServerS -= AddMessageToChat;
        }
    }

    public void PostButtonOnPressed() 
    {
        string ourChatText = usermessageInput.GetComponent<InputField>().text;

        string ourMsg = ClientToServerSignifiers.SendChatMsg + "," + ourChatText;

        networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
    }

    public void AddMessageToChat(string s)
    {
        GameObject newObj;
        newObj = (GameObject)Instantiate(prefab, userscrollView.GetComponentInChildren<>);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
