using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    //private NetworkedClient m_OnMessageReceivedChatRoomMsg = null;

    //private NetworkedClient m_OnMessageReceivedChatUsersList = null;

    //private NetworkedClient m_OnMessageReceivedClearChatUsersList = null;


    private NetworkedClient m_On = null;

    private string m_plyersInChat;

    GameObject postButton, usermessageInput, CU_Content, networkObject,CM_Content,SystemManager;
    public GameObject prefabTextObject = null,prefebButtonObject;
    public static List<GameObject> ListPrefabButtons = new List<GameObject>();
   // public static List<string> ListofPlyers = new List<string>();
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Chat_InputField")
                usermessageInput = go;
            else if (go.name == "Chat_Button")
                postButton = go;
            else if (go.name == "Chat_Users_Content")
                CU_Content = go;
            else if (go.name == "Network")
                networkObject = go;
            else if (go.name == "Chat_Msg_Content")
                CM_Content = go;
            else if (go.name == "SystemManagerObject")
                SystemManager = go;
        }
        postButton.GetComponent<Button>().onClick.AddListener(PostButtonOnPressed);

        //m_OnMessageReceivedChatRoomMsg = networkObject.GetComponent<NetworkedClient>();
        //m_OnMessageReceivedChatUsersList = networkObject.GetComponent<NetworkedClient>();
        //m_OnMessageReceivedClearChatUsersList = networkObject.GetComponent<NetworkedClient>();

        //if (m_OnMessageReceivedChatRoomMsg != null)
        //{
        //    m_OnMessageReceivedChatRoomMsg.OnMessageReceivedChatRoomMsg += AddGlobalMessageToChat;
        //}
        //if (m_OnMessageReceivedChatUsersList != null) 
        //{
        //    m_OnMessageReceivedChatUsersList.OnMessageReceivedChatUsers += AddListOfPlayerToChat;
        //}
        //if (m_OnMessageReceivedClearChatUsersList != null) 
        //{
        //    m_OnMessageReceivedClearChatUsersList.OnMessageReceivedClearChatUsersList += ClearListOfPlayerToChat;
        //}


        m_On = networkObject.GetComponent<NetworkedClient>();

        if (m_On != null)
        {
            m_On.On += AddGlobalMessageToChat;
            m_On.On += AddListOfPlayerToChat;
            m_On.On += ClearListOfPlayerToChat;
        }
    }


    private void OnDestroy()
    {
        //if (m_OnMessageReceivedChatRoomMsg != null)
        //{
        //    m_OnMessageReceivedChatRoomMsg.OnMessageReceivedChatRoomMsg -= AddGlobalMessageToChat;
        //}
        //if (m_OnMessageReceivedChatUsersList != null) 
        //{
        //    m_OnMessageReceivedChatUsersList.OnMessageReceivedChatUsers -= AddListOfPlayerToChat;
        //}
        //if (m_OnMessageReceivedClearChatUsersList != null) 
        //{
        //    m_OnMessageReceivedClearChatUsersList.OnMessageReceivedClearChatUsersList -= ClearListOfPlayerToChat;
        //}

        if (m_On != null)
        {
            m_On.On -= AddGlobalMessageToChat;
            m_On.On -= AddListOfPlayerToChat;
            m_On.On -= ClearListOfPlayerToChat;
        }

    }

    public void PostButtonOnPressed() 
    {
         string n = SystemManager.GetComponent<SystemManager>().GetUserName;
        string ourChatText = usermessageInput.GetComponent<InputField>().text;


        string ourMsg = ClientToServerSignifiers.SendChatMsg + "," + n +": "+ ourChatText;

        //string ourMsg = ClientToServerSignifiers.SendChatMsg + "," + " :" + ourChatText;

        networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
    }

    public void AddGlobalMessageToChat(int signifier, string s)
    {
        if (signifier == ServerToClientSignifiers.ChatView)
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefabTextObject, CM_Content.transform);
            newObj.GetComponent<Text>().text = s;
        }
    } 
    public void AddPrivateMessageToChat()
    {
      // at pm message to  the chat
    }

    public void AddListOfPlayerToChat(int signifier, string s)
    {
        //GameObject newObj;
        //newObj = (GameObject)Instantiate(prefebButtonObject, CM_Content.transform);

        //ListofPlyers.Add(s);
        //Debug.Log("Player : " + s);

        if (signifier == ServerToClientSignifiers.ReceiveListOFPlayerInChat)
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefabTextObject, CU_Content.transform);
            newObj.GetComponent<Text>().text = s;
            ListPrefabButtons.Add(newObj); 
        }
    }

    public void ClearListOfPlayerToChat(int signifier, string s)
    {
        if (signifier == ServerToClientSignifiers.ReceiveClearListOFPlayerInChat)
        {
            if (ListPrefabButtons != null || ListPrefabButtons.Count != 0)
            {
                //ListPrefabButtons.Clear();

                foreach (GameObject g in ListPrefabButtons)
                {
                    Destroy(g);
                }

                Debug.Log("ListPrefabButtons isn't empty!!");
            }
        }
        Debug.Log("ClearListOfPlayerToChat--->Herrre!!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
