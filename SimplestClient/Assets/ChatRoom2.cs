using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom2 : MonoBehaviour
{
    private NetworkedClient m_MessageReceiverFromServer = null;

    private string m_plyersInChat;

    GameObject GRC_postButton, GRC_usermessageInput, networkObject, GRCM_Content, SystemManager, GRC_SendText,GRC_Player_Toggle, GRC_DropDownMenu,
        GRC_Observer_Toggle, GRC_Everyone_Toggle;
    public GameObject prefabTextObject = null;

    

    //public static List<GameObject> ListPrefabButtons = new List<GameObject>();
    [SerializeField]  public  List<GameObject> ListPrefabTextObject = new List<GameObject>();
   // bool IsPrivateMsg = false;
   // private string PmUsername;
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "GameRoomChat_InputField")
                GRC_usermessageInput = go;
            else if (go.name == "GameRoomChat_Button")
                GRC_postButton = go;
            else if (go.name == "Network")
                networkObject = go;
            else if (go.name == "GameRoomChat_Msg_Content")
                GRCM_Content = go;
            else if (go.name == "GameRoomChat_SendText")
                GRC_SendText = go;
            else if (go.name == "SystemManagerObject")
                SystemManager = go;
            else if (go.name == "SystemManagerObject")
                SystemManager = go;
            else if (go.name == "GameRoomChat_Observers_Toggle")
                GRC_Observer_Toggle = go;
            else if (go.name == "GameRoomChat_Player_Toggle")
                GRC_Player_Toggle = go;
            else if (go.name == "GameRoomChat_Global_Toggle")
                GRC_Everyone_Toggle = go;
            else if (go.name == "GameRoomChat_Dropdown")
                GRC_DropDownMenu = go;
        }
        GRC_postButton.GetComponent<Button>().onClick.AddListener(GRC_PostButtonOnPressed);

        GRC_Everyone_Toggle.GetComponent<Toggle>().onValueChanged.AddListener(GlobleToggleChanged);
        GRC_Player_Toggle.GetComponent<Toggle>().onValueChanged.AddListener(PlayerToggleChanged);
        GRC_Observer_Toggle.GetComponent<Toggle>().onValueChanged.AddListener(ObserverToggleChanged);



        m_MessageReceiverFromServer = networkObject.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer += MessageToChat;
        }
        GRC_DropDownMenu.GetComponent<Dropdown>().onValueChanged.AddListener(GRC_DropDownPrefixMsg);
    }



    public void GlobleToggleChanged(bool newValue)
    {
       
        GRC_Player_Toggle.GetComponent<Toggle>().isOn = false;
        GRC_Observer_Toggle.GetComponent<Toggle>().isOn = false;
        GRC_SendText.GetComponent<Text>().text = "Sending Message To: Globle";

        if(GRC_Everyone_Toggle.GetComponent<Toggle>().isOn == false && GRC_Observer_Toggle.GetComponent<Toggle>().isOn == false 
            && GRC_Player_Toggle.GetComponent<Toggle>().isOn == false)
        {
            GRC_SendText.GetComponent<Text>().text = "Sending Message To: NoOne";
        }
    }
    public void PlayerToggleChanged(bool newValue)
    {
      
        GRC_Everyone_Toggle.GetComponent<Toggle>().isOn = false;
        GRC_Observer_Toggle.GetComponent<Toggle>().isOn = false;
        GRC_SendText.GetComponent<Text>().text = "Sending Message To: Players";

        if (GRC_Everyone_Toggle.GetComponent<Toggle>().isOn == false && GRC_Observer_Toggle.GetComponent<Toggle>().isOn == false
            && GRC_Player_Toggle.GetComponent<Toggle>().isOn == false)
        {
            GRC_SendText.GetComponent<Text>().text = "Sending Message To: NoOne";
        }
    }
    public void ObserverToggleChanged(bool newValue)
    {
       
        GRC_Player_Toggle.GetComponent<Toggle>().isOn = false;
        GRC_Everyone_Toggle.GetComponent<Toggle>().isOn = false;
        GRC_SendText.GetComponent<Text>().text = "Sending Message To: Observers";

        if (GRC_Everyone_Toggle.GetComponent<Toggle>().isOn == false && GRC_Observer_Toggle.GetComponent<Toggle>().isOn == false
            && GRC_Player_Toggle.GetComponent<Toggle>().isOn == false)
        {
            GRC_SendText.GetComponent<Text>().text = "Sending Message To: NoOne";
        }
    }


    private void OnDestroy()
    {

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= MessageToChat;
        }

    }

    public void GRC_PostButtonOnPressed()
    {
        string n = SystemManager.GetComponent<SystemManager>().GetUserName;
        string ourChatText = GRC_usermessageInput.GetComponent<InputField>().text;


       
        if (GRC_Player_Toggle.GetComponent<Toggle>().isOn)
        {
            string ourMsg = ClientToServerSignifiers.SendOnlyPlayerGameRoomChatMSG + ", OnlyPlayers <" + n + "> : " + ourChatText;
            networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
        
        else if (GRC_Observer_Toggle.GetComponent<Toggle>().isOn)
        {
            string ourMsg = ClientToServerSignifiers.SendOnlyObserverGameRoomChatMSG + ", OnlyObservers <" + n + "> : " + ourChatText;
            networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);

        }

        else if (GRC_Everyone_Toggle.GetComponent<Toggle>().isOn)
        {
            string ourMsg = ClientToServerSignifiers.SendGameRoomGlobalChatMSG + ", Globe < " + n + " >  : " + ourChatText;
            networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }

        //string ourMsg = ClientToServerSignifiers.SendGameRoomChatMSG + ",Globe " + n + ": " + ourChatText;
        //networkObject.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);

    }

    public void MessageToChat(int signifier, string s, TicTacToeBoard tt, MatchData matchData)
    {
        if (signifier == ServerToClientSignifiers.ReceiveGameRoomChatMSG)
        {
            GameObject newObj;
            newObj = (GameObject)Instantiate(prefabTextObject, GRCM_Content.transform);
            newObj.GetComponent<Text>().text = s;
            ListPrefabTextObject.Add(newObj);
        }
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            // restart
            ClearAllGameRoomChatlMessage();
            GRC_usermessageInput.GetComponent<InputField>().text = "";
            GlobleToggleChanged(true);
        }
    }

    public void SetChatToGlobalMessage()
    {
        //IsPrivateMsg = false;
        GRC_SendText.GetComponent<Text>().text = "Sending Message To: ";
    }

    public void ClearAllGameRoomChatlMessage()
    {
        if (ListPrefabTextObject != null && ListPrefabTextObject.Count != 0)
        {

            foreach (GameObject t in ListPrefabTextObject)
            {
                Destroy(t);
            }

            // Debug.Log("ListPrefabTextObject isn't empty!!");
            ListPrefabTextObject.Clear();
        }
        
    }

    void ClearSomeGameRoomChatlMessage()
    {
        if (ListPrefabTextObject != null && ListPrefabTextObject.Count != 0)
        {
           
            for( int i = 0; ListPrefabTextObject.Count> 50; i++)
            {
                Destroy(ListPrefabTextObject[i]);
                ListPrefabTextObject.RemoveAt(i);
            }
            // Debug.Log("ListPrefabTextObject isn't empty!!");
        }
    }

    private void Update()
    {
        ClearSomeGameRoomChatlMessage();
    }


    public void GRC_DropDownPrefixMsg(int val)
    {
        switch (val)
        {
            case 0:
                break;
            case 1:
                GRC_usermessageInput.GetComponent<InputField>().text = "(ง︡'-'︠)ง Anyone up a for game of Tic tac Toe.(ง︡'-'︠)ง";
                break;
            case 2:
                GRC_usermessageInput.GetComponent<InputField>().text = "（っ＾▿＾）howdy people in the chat.（っ＾▿＾）";
                break;
            case 3:
                GRC_usermessageInput.GetComponent<InputField>().text = "ʕ•́ᴥ•̀ʔっ I'll be Away for my keyboard for a bit ʕ•́ᴥ•̀ʔっ";
                break;
            case 4:
                GRC_usermessageInput.GetComponent<InputField>().text = "I'm login out. ┻━┻ ︵ヽ(`▭´)ﾉ︵﻿ ┻━┻";
                break;
            case 5:
                GRC_usermessageInput.GetComponent<InputField>().text = "Hot! hot! I'm on a Winning Streak!.(҂◡̀_◡́)ᕤ ";
                break;
            case 6:
                GRC_usermessageInput.GetComponent<InputField>().text = "I just need a win. This losing streak is lasting too long ((╥﹏╥))";
                break;
            case 7:
                GRC_usermessageInput.GetComponent<InputField>().text = "(ɔ◔‿◔)ɔ ♥ GiT-GUD (͠◉_◉)";
                break;
            case 8:
                GRC_usermessageInput.GetComponent<InputField>().text = "( ◡́.◡̀)(^◡^ ) Well Played! You are a Worthy Opponent!!";
                break;
        }
    }
}