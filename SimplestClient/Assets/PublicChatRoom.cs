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
    GameObject post_button, user_message_input, public_chat_user_content, network, Public_chat_message_content, system_manager, send_to_text, global_button, public_chat_logout_button, public_chat_drop_down_menu;
    #endregion

    #region Variables
    
    bool is_private_message = false;
    private string private_message_user_name;
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
                public_chat_user_content = go;
            else if (go.name == "Network")
                network = go;
            else if (go.name == "Chat_Msg_Content")
                Public_chat_message_content = go;
            else if (go.name == "Chat_SendText")
                send_to_text = go;
            else if (go.name == "SystemManagerObject")
                system_manager = go;
            else if (go.name == "Chat_GlobalButton")
                global_button = go;
            else if (go.name == "Chat_LogOut_Button")
                public_chat_logout_button = go;
            else if (go.name == "Chat_Dropdown")
                public_chat_drop_down_menu = go;
        }


        post_button.GetComponent<Button>().onClick.AddListener(PostButtonOnPressed);

        global_button.GetComponent<Button>().onClick.AddListener(SetChatMessageToGlobal);

        public_chat_logout_button.GetComponent<Button>().onClick.AddListener(PublicChatRoomLogout);

        message_receiver_from_server = network.GetComponent<NetworkedClient>();


        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += PublicChatRoomReceivedFromServer;
        }
        public_chat_drop_down_menu.GetComponent<Dropdown>().onValueChanged.AddListener(PublicChatDropDownPrefixMsg);
    }


   


    

    #region ReceviedMessageFromTheServer/Involved

   

    private void PublicChatRoomReceivedFromServer(int signifier, string s, TicTacToeBoard tt, MatchData matchData)
    {
        
        
        if (signifier == ServerToClientSignifiers.ReceiveListOFPlayerInChat)
        {
            
            CreateObjectsForPublicChatroom(false, false, s);
        }
        else if (signifier == ServerToClientSignifiers.ReceiveClearListOFPlayerInChat)
        {
            ClearPlayersListIntChatroom();

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
    
    // just clear older messages from the chatroom
    void ClearOldChatlMessages()
    {
        if (prefab_text_objects != null && prefab_text_objects.Count != 0)
        {

            for (int i = 0; prefab_text_objects.Count > 50; i++)
            {
                Destroy(prefab_text_objects[i]);
                prefab_text_objects.RemoveAt(i);
            }
           
        }
    }

    

    public void ClearPlayersListIntChatroom()
    {
        DestroyAllObjectsForPublicChatroom(true);
    }

    /// <summary>
    /// CreateObjectsForPublicChatroom
    /// </summary>
    /// create object and adds to a list of object(button or text)
    /// - if private message it's the color to red
    /// - if globol message just see it in chat
    /// - create button for private message.
    void CreateObjectsForPublicChatroom(bool is_text_object, bool is_private_message, string msg)
    {
        GameObject newObj;
        if (is_text_object == true && is_private_message == false)
        {
            // global chat
            newObj = (GameObject)Instantiate(prefab_text_object, Public_chat_message_content.transform);
            newObj.GetComponent<Text>().text = msg;
            prefab_text_objects.Add(newObj);
        }
        else if (is_text_object ==  true && is_private_message == true)
        {
          // private  chat msg
            newObj = (GameObject)Instantiate(prefab_text_object, Public_chat_message_content.transform);
            newObj.GetComponent<Text>().color = Color.red;
            newObj.GetComponent<Text>().text = msg;
            prefab_text_objects.Add(newObj);
        }
        else if (is_text_object == false && is_private_message == false) 
        {
            /// buttons 
            newObj = (GameObject)Instantiate(prefeb_button_object, public_chat_user_content.transform);
            newObj.GetComponent<PrivateMessageButton>().SetName = msg;
            newObj.GetComponentInChildren<Text>().text = msg;
            prefab_buttons.Add(newObj);
        }

    }

    /// <summary>
    /// DestroyAllObjectsForPublicChatroom
    /// - clear list of buttons
    /// - clear list of text object
    /// </summary>
    /// <param name="is_button"></param>
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

    private void OnDestroy()
    {

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= PublicChatRoomReceivedFromServer;
        }

    }
    #endregion

    #region InterfaceCode
    public void PublicChatDropDownPrefixMsg(int val)
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

    //this get called the buttons we create in list of buttons
    // get the player user name  want to send a pm
    // display we senting a pm to and to who
    public void SetChatMessageToPrivate(string s)
    {
        private_message_user_name = s;
        is_private_message = true;
        send_to_text.GetComponent<Text>().text = "Sending Message To: " + s;
    }

   
    private void SetChatMessageToGlobal()
    {
        is_private_message = false;
        send_to_text.GetComponent<Text>().text = "Sending Message To: Globle";
    }

    private  void PostButtonOnPressed()
    {
        string n = system_manager.GetComponent<SystemManager>().GetUserName;
        string ourChatText = user_message_input.GetComponent<InputField>().text;
        if (is_private_message == false)
        {
            string ourMsg = ClientToServerSignifiers.NotifyPublicChatChatOfGlobalMsg + ",Globe " + n + ": " + ourChatText;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
        else
        {
            string ourMsg = ClientToServerSignifiers.NotifyPublicChatWitchAPrivateMsg + ",PM " + n + ": " + ourChatText + "," + private_message_user_name;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }
    }

    private void PublicChatRoomLogout() 
    {
        system_manager.GetComponent<SystemManager>().LogOutPublicChatRoom();
    }

    #endregion




    // Update is called once per frame
    void Update()
    {
        ClearOldChatlMessages();
    }

}