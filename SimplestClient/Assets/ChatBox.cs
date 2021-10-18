using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    
    private NetworkedClient m_On = null;

    private string m_plyersInChat;

    GameObject postButton, usermessageInput, CU_Content, networkObject,CM_Content,SystemManager, SendText;
    public GameObject prefabTextObject = null,prefebButtonObject;
    public static List<GameObject> ListPrefabButtons = new List<GameObject>();
    public static List<GameObject> ListPrefabTextObject = new List<GameObject>();
    bool IsPrivateMsg = false;
    private string PmUsername;
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
            else if (go.name == "Chat_SendText")
                SendText = go;
            else if (go.name == "SystemManagerObject")
                SystemManager = go;
        }
        postButton.GetComponent<Button>().onClick.AddListener(PostButtonOnPressed);


        m_On = networkObject.GetComponent<NetworkedClient>();

        if (m_On != null)
        {
            m_On.On += GlobalMessageToChat;
            m_On.On += AddListOfPlayerToChat;
            m_On.On += ClearListOfPlayerToChat;
        }
    }


    private void OnDestroy()
    {

        if (m_On != null)
        {
            m_On.On -= GlobalMessageToChat;
            m_On.On -= AddListOfPlayerToChat;
            m_On.On -= ClearListOfPlayerToChat;
        }

    }

    public void PostButtonOnPressed() 
    {
        string n = SystemManager.GetComponent<SystemManager>().GetUserName;
        string ourChatText = usermessageInput.GetComponent<InputField>().text;
        if (IsPrivateMsg == false)
        {
            string ourMsg = ClientToServerSignifiers.SendChatMsg + ",Globe " + n + ": " + ourChatText;
            networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
        else
        {
            string ourMsg = ClientToServerSignifiers.SendChatPrivateMsg + ",PM " + n + ": " + ourChatText + "," + PmUsername;
            networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
    }

    public void GlobalMessageToChat(int signifier, string s)
    {
        if (signifier == ServerToClientSignifiers.ChatView)
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefabTextObject, CM_Content.transform);
            newObj.GetComponent<Text>().text = s;
            ListPrefabTextObject.Add(newObj);
        }
        else if(signifier == ServerToClientSignifiers.LogOutComplete)
        {
            if (ListPrefabTextObject != null && ListPrefabTextObject.Count != 0)
            {
               
                foreach (GameObject t in ListPrefabTextObject)
                {
                    Destroy(t);
                }

                Debug.Log("ListPrefabTextObject isn't empty!!");
            }
        }
        if (signifier == ServerToClientSignifiers.ReceivePrivateChatMsg)
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefabTextObject, CM_Content.transform);
            newObj.GetComponent<Text>().color = Color.red;
            newObj.GetComponent<Text>().text = s;
            ListPrefabTextObject.Add(newObj);
        }

    }
    
    public void SetChatToPrivateMessage( string s)
    {
        PmUsername = s;
        IsPrivateMsg = true;
        SendText.GetComponent<Text>().text = "Sending Message To: " + s;
    }
    public void SetChatToGlobalMessage()
    {
        IsPrivateMsg = false;
        SendText.GetComponent<Text>().text = "Sending Message To: Globle";
    }

    public void AddListOfPlayerToChat(int signifier, string s)
    {
       

        if (signifier == ServerToClientSignifiers.ReceiveListOFPlayerInChat)
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefebButtonObject, CU_Content.transform);
            newObj.GetComponent<PMButton>().SetName = s;
            newObj.GetComponentInChildren<Text>().text = s;
            ListPrefabButtons.Add(newObj); 
        }
    }

    public void ClearListOfPlayerToChat(int signifier, string s)
    {
        if (signifier == ServerToClientSignifiers.ReceiveClearListOFPlayerInChat)
        {
            if (ListPrefabButtons != null || ListPrefabButtons.Count != 0)
            {

                foreach (GameObject g in ListPrefabButtons)
                {
                    Destroy(g);
                }

                Debug.Log("ListPrefabButtons isn't empty!!");
            }
        }
        Debug.Log("ClearListOfPlayerToChat--->Herrre!!");
    }

    public void DropDownPrefixMsg (int val) 
    {
        switch (val) 
        {
            case 0:
                break;
            case 1:
                usermessageInput.GetComponent<InputField>().text = "(ง︡'-'︠)ง Anyone up a for game of Tic tac Toe.(ง︡'-'︠)ง";
                break;
            case 2:
                usermessageInput.GetComponent<InputField>().text = "（っ＾▿＾）howdy people in the chat.（っ＾▿＾）";
                break;
            case 3:
                usermessageInput.GetComponent<InputField>().text = "ʕ•́ᴥ•̀ʔっ I'll be Away for my keyboard for a bit ʕ•́ᴥ•̀ʔっ";
                break;
            case 4:
                usermessageInput.GetComponent<InputField>().text = "I'm login out. ┻━┻ ︵ヽ(`▭´)ﾉ︵﻿ ┻━┻";
                break;
            case 5:
                usermessageInput.GetComponent<InputField>().text = "Hot! hot! I'm on a Winning Streak!.(҂◡̀_◡́)ᕤ ";
                break;
            case 6:
                usermessageInput.GetComponent<InputField>().text = "I just need a win. This losing streak is lasting too long ((╥﹏╥))";
                break;
            case 7:
                usermessageInput.GetComponent<InputField>().text = "(ɔ◔‿◔)ɔ ♥ GiT-GUD (͠◉_◉)";
                break;
            case 8:
                usermessageInput.GetComponent<InputField>().text = "( ◡́.◡̀)(^◡^ ) Well Played! You are a Worthy Opponent!!";
                break;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

}