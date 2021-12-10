using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublicChatRoom : MonoBehaviour
{

    #region GameObject
    private NetworkedClient message_receiver_from_server = null;
    public GameObject prefab_text_object = null, prefeb_button_object;
    public List<GameObject> prefab_buttons = new List<GameObject>();
    public List<GameObject> prefab_text_objects = new List<GameObject>();
    GameObject post_button, user_message_input, CU_Content, network, CM_Content, system_manager, send_text;
    #endregion

    #region Variables
    private string m_plyersInChat;
    bool IsPrivateMsg = false;
    private string PmUsername;
    #endregion



    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Chat_InputField")
                user_message_input = go;
            else if (go.name == "Chat_Button")
                post_button = go;
            else if (go.name == "Chat_Users_Content")
                CU_Content = go;
            else if (go.name == "Network")
                network = go;
            else if (go.name == "Chat_Msg_Content")
                CM_Content = go;
            else if (go.name == "Chat_SendText")
                send_text = go;
            else if (go.name == "SystemManagerObject")
                system_manager = go;
        }
        post_button.GetComponent<Button>().onClick.AddListener(PostButtonOnPressed);


        message_receiver_from_server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += PublicChatRoomReceivedFromServer;
        }
    }


    private void OnDestroy()
    {

        if (message_receiver_from_server != null)
        {           
            message_receiver_from_server.OnMessageReceivedFromServer -= PublicChatRoomReceivedFromServer;
        }

    }


    #region Initialzing
    #endregion

    #region ReceviedMessageFromTheServer/Involved

   

    private void PublicChatRoomReceivedFromServer(int signifier, string s, TicTacToeBoard tt, MatchData matchData)
    {
        
        
        if (signifier == ServerToClientSignifiers.ReceiveListOFPlayerInChat)
        {
            //AddListOfPlayerToChat(s);
            CreateObjectsForPublicChatroom(false, false, s);
        }
        else if (signifier == ServerToClientSignifiers.ReceiveClearListOFPlayerInChat)
        {
            ClearListOfPlayerToChat();

        }
        else if (signifier == ServerToClientSignifiers.LogOutComplete)
        {
            DestroyAllObjectsForPublicChatroom(false);
        }
        else if (signifier == ServerToClientSignifiers.ReceivePrivateChatMsg) 
        {
            CreateObjectsForPublicChatroom(true, true, s);
        }
        else if (signifier == ServerToClientSignifiers.ChatView)
        {
            CreateObjectsForPublicChatroom(true, false, s);
        }
    }
    //public void AddListOfPlayerToChat( string player_name)
    //{
    //   GameObject newObj;
    //   newObj = (GameObject)Instantiate(prefeb_button_object, CU_Content.transform);
    //   newObj.GetComponent<PMButton>().SetName = player_name;
    //   newObj.GetComponentInChildren<Text>().text = player_name;
    //   prefab_buttons.Add(newObj);   
    //}
    void ClearSomeChatroomChatlMessage()
    {
        if (prefab_text_objects != null && prefab_text_objects.Count != 0)
        {

            for (int i = 0; prefab_text_objects.Count > 50; i++)
            {
                Destroy(prefab_text_objects[i]);
                prefab_text_objects.RemoveAt(i);
            }
            // Debug.Log("ListPrefabTextObject isn't empty!!");
        }
    }

    //public void GlobalMessageToChat(int signifier, string s, TicTacToeBoard tt, MatchData matchData)
    //{
    //    if (signifier == ServerToClientSignifiers.ChatView)
    //    {
    //        GameObject newObj;
    //        newObj = (GameObject)Instantiate(prefab_text_object, CM_Content.transform);
    //        newObj.GetComponent<Text>().text = s;
    //        prefab_text_objects.Add(newObj);
    //    }
    //    else if (signifier == ServerToClientSignifiers.LogOutComplete)
    //    {
    //        if (prefab_text_objects != null && prefab_text_objects.Count != 0)
    //        {

    //            foreach (GameObject t in prefab_text_objects)
    //            {
    //                Destroy(t);
    //            }

    //            Debug.Log("ListPrefabTextObject isn't empty!!");
    //            prefab_text_objects.Clear();
    //        }
    //    }
    //    if (signifier == ServerToClientSignifiers.ReceivePrivateChatMsg)
    //    {
    //        GameObject newObj;
    //        newObj = (GameObject)Instantiate(prefab_text_object, CM_Content.transform);
    //        newObj.GetComponent<Text>().color = Color.red;
    //        newObj.GetComponent<Text>().text = s;
    //        prefab_text_objects.Add(newObj);
    //    }

    //}


    public void ClearListOfPlayerToChat()
    {

        DestroyAllObjectsForPublicChatroom(true);

       
    }

    void CreateObjectsForPublicChatroom(bool is_text_object, bool is_private_message, string msg)
    {
        GameObject newObj;
        if (is_text_object == true && is_private_message == false)
        {
            // global chat
            newObj = (GameObject)Instantiate(prefab_text_object, CM_Content.transform);
            newObj.GetComponent<Text>().text = msg;
            prefab_text_objects.Add(newObj);
        }
        else if (is_text_object ==  true && is_private_message == true)
        {
          // private  chat msg
            newObj = (GameObject)Instantiate(prefab_text_object, CM_Content.transform);
            newObj.GetComponent<Text>().color = Color.red;
            newObj.GetComponent<Text>().text = msg;
            prefab_text_objects.Add(newObj);
        }
        else if (is_text_object == false && is_private_message == false) 
        {
            /// buttons 
            newObj = (GameObject)Instantiate(prefeb_button_object, CU_Content.transform);
            newObj.GetComponent<PMButton>().SetName = msg;
            newObj.GetComponentInChildren<Text>().text = msg;
            prefab_buttons.Add(newObj);
        }

    }

    public void DestroyAllObjectsForPublicChatroom(bool is_button)
    {

        if (is_button)
        {
            if (prefab_buttons != null || prefab_buttons.Count != 0)
            {

                foreach (GameObject g in prefab_buttons)
                {
                    Destroy(g);
                }

                Debug.Log("ListPrefabButtons isn't empty!!");
                prefab_buttons.Clear();
            }

        }
        else 
        {
            if (prefab_text_objects != null && prefab_text_objects.Count != 0)
            {

                foreach (GameObject t in prefab_text_objects)
                {
                    Destroy(t);
                }

                Debug.Log("ListPrefabTextObject isn't empty!!");
                prefab_text_objects.Clear();
            }
        }
    }

    #endregion

    #region InterfaceCode
    public void DropDownPrefixMsg(int val)
    {
        switch (val)
        {
            case 0:
                break;
            case 1:
                user_message_input.GetComponent<InputField>().text = "(ง︡'-'︠)ง Anyone up a for game of Tic tac Toe.(ง︡'-'︠)ง";
                break;
            case 2:
                user_message_input.GetComponent<InputField>().text = "（っ＾▿＾）howdy people in the chat.（っ＾▿＾）";
                break;
            case 3:
                user_message_input.GetComponent<InputField>().text = "ʕ•́ᴥ•̀ʔっ I'll be Away for my keyboard for a bit ʕ•́ᴥ•̀ʔっ";
                break;
            case 4:
                user_message_input.GetComponent<InputField>().text = "I'm login out. ┻━┻ ︵ヽ(`▭´)ﾉ︵﻿ ┻━┻";
                break;
            case 5:
                user_message_input.GetComponent<InputField>().text = "Hot! hot! I'm on a Winning Streak!.(҂◡̀_◡́)ᕤ ";
                break;
            case 6:
                user_message_input.GetComponent<InputField>().text = "I just need a win. This losing streak is lasting too long ((╥﹏╥))";
                break;
            case 7:
                user_message_input.GetComponent<InputField>().text = "(ɔ◔‿◔)ɔ ♥ GiT-GUD (͠◉_◉)";
                break;
            case 8:
                user_message_input.GetComponent<InputField>().text = "( ◡́.◡̀)(^◡^ ) Well Played! You are a Worthy Opponent!!";
                break;
        }
    }

    public void SetChatToPrivateMessage(string s)
    {
        PmUsername = s;
        IsPrivateMsg = true;
        send_text.GetComponent<Text>().text = "Sending Message To: " + s;
    }

    public void SetChatToGlobalMessage()
    {
        IsPrivateMsg = false;
        send_text.GetComponent<Text>().text = "Sending Message To: Globle";
    }

    public void PostButtonOnPressed()
    {
        string n = system_manager.GetComponent<SystemManager>().GetUserName;
        string ourChatText = user_message_input.GetComponent<InputField>().text;
        if (IsPrivateMsg == false)
        {
            string ourMsg = ClientToServerSignifiers.NotifyPublicChatChatOfGlobalMsg + ",Globe " + n + ": " + ourChatText;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
        else
        {
            string ourMsg = ClientToServerSignifiers.NotifyPublicChatWitchAPrivateMsg + ",PM " + n + ": " + ourChatText + "," + PmUsername;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
    }
    #endregion




    // Update is called once per frame
    void Update()
    {
        ClearSomeChatroomChatlMessage();
    }

}