using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomChatRoom : MonoBehaviour
{
    #region GameObject

    private NetworkedClient message_receiver_from_Server = null;


    GameObject game_room_chat_room_post_button, game_room_chat_room_user_message_input, network, game_room_chat_room_message_content, system_manager, game_room_chat_room_send_text, game_room_chat_room_player_toggle, game_room_chat_room_drop_down_menu,
        game_room_chat_room_observer_toggle, game_room_chat_room_everyone_toggle;
    public GameObject prefab_text_object = null;


    [SerializeField]  public  List<GameObject> ListPrefabTextObject = new List<GameObject>();
   
    #endregion 



    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "GameRoomChat_InputField")
                game_room_chat_room_user_message_input = go;
            else if (go.name == "GameRoomChat_Button")
                game_room_chat_room_post_button = go;
            else if (go.name == "Network")
                network = go;
            else if (go.name == "GameRoomChat_Msg_Content")
                game_room_chat_room_message_content = go;
            else if (go.name == "GameRoomChat_SendText")
                game_room_chat_room_send_text = go;
            else if (go.name == "SystemManagerObject")
                system_manager = go;
            else if (go.name == "SystemManagerObject")
                system_manager = go;
            else if (go.name == "GameRoomChat_Observers_Toggle")
                game_room_chat_room_observer_toggle = go;
            else if (go.name == "GameRoomChat_Player_Toggle")
                game_room_chat_room_player_toggle = go;
            else if (go.name == "GameRoomChat_Global_Toggle")
                game_room_chat_room_everyone_toggle = go;
            else if (go.name == "GameRoomChat_Dropdown")
                game_room_chat_room_drop_down_menu = go;
        }
        game_room_chat_room_post_button.GetComponent<Button>().onClick.AddListener(GameRoomChatRoomPostButtonOnPressed);

        game_room_chat_room_everyone_toggle.GetComponent<Toggle>().onValueChanged.AddListener(GlobleToggleChanged);
        game_room_chat_room_player_toggle.GetComponent<Toggle>().onValueChanged.AddListener(PlayerToggleChanged);
        game_room_chat_room_observer_toggle.GetComponent<Toggle>().onValueChanged.AddListener(ObserverToggleChanged);



        message_receiver_from_Server = network.GetComponent<NetworkedClient>();

        if (message_receiver_from_Server != null)
        {
            message_receiver_from_Server.OnMessageReceivedFromServer += GameRoomChatReceivedFromTheServer;
        }
        game_room_chat_room_drop_down_menu.GetComponent<Dropdown>().onValueChanged.AddListener(GameRoomChatRoomDropDownPrefixMsg);
    }



    
    
    

    #region MessageFromTheServer
    public void GameRoomChatReceivedFromTheServer(int signifier, string s, TicTacToeBoard tt, MatchData match_data)
    {
        if (signifier == ServerToClientSignifiers.ReceiveGameRoomChatMSG)
        {

            CreateMessageToDisplayOnGameRoomChat(s);
        }
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            ResetGameRoomChat();
        }
    }


    private void ResetGameRoomChat()
    {
        // restart
        ClearAllGameRoomChatMessage();
        game_room_chat_room_user_message_input.GetComponent<InputField>().text = "";
        GlobleToggleChanged(true);
    }

    private void CreateMessageToDisplayOnGameRoomChat(string msg) 
    {
        GameObject newObj;
        newObj = (GameObject)Instantiate(prefab_text_object, game_room_chat_room_message_content.transform);
        newObj.GetComponent<Text>().text = msg;
        ListPrefabTextObject.Add(newObj);
    }

    private  void ClearAllGameRoomChatMessage()
    {
        if (ListPrefabTextObject != null && ListPrefabTextObject.Count != 0)
        {

            foreach (GameObject t in ListPrefabTextObject)
            {
                Destroy(t);
            }

           
            ListPrefabTextObject.Clear();
        }
    }

    private void OnDestroy()
    {

        if (message_receiver_from_Server != null)
        {
            message_receiver_from_Server.OnMessageReceivedFromServer -= GameRoomChatReceivedFromTheServer;
        }

    }

    #endregion



   



    #region Interface


    public void GameRoomChatRoomDropDownPrefixMsg(int val)
    {
        switch (val)
        {
            case 0:
                break;
            case 1:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "(ง︡'-'︠)ง Anyone up a for game of Tic tac Toe.(ง︡'-'︠)ง";
                break;
            case 2:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "（っ＾▿＾）howdy people in the chat.（っ＾▿＾）";
                break;
            case 3:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "ʕ•́ᴥ•̀ʔっ I'll be Away for my keyboard for a bit ʕ•́ᴥ•̀ʔっ";
                break;
            case 4:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "I'm login out. ┻━┻ ︵ヽ(`▭´)ﾉ︵﻿ ┻━┻";
                break;
            case 5:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "Hot! hot! I'm on a Winning Streak!.(҂◡̀_◡́)ᕤ ";
                break;
            case 6:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "I just need a win. This losing streak is lasting too long ((╥﹏╥))";
                break;
            case 7:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "(ɔ◔‿◔)ɔ ♥ GiT-GUD (͠◉_◉)";
                break;
            case 8:
                game_room_chat_room_user_message_input.GetComponent<InputField>().text = "( ◡́.◡̀)(^◡^ ) Well Played! You are a Worthy Opponent!!";
                break;
        }
    }
    public void GameRoomChatRoomPostButtonOnPressed()
    {
        string n = system_manager.GetComponent<SystemManager>().GetUserName;
        string ourChatText = game_room_chat_room_user_message_input.GetComponent<InputField>().text;



        if (game_room_chat_room_player_toggle.GetComponent<Toggle>().isOn)
        {
            string ourMsg = ClientToServerSignifiers.SendOnlyPlayerGameRoomChatMSG + ", OnlyPlayers <" + n + "> : " + ourChatText;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }

        else if (game_room_chat_room_observer_toggle.GetComponent<Toggle>().isOn)
        {
            string ourMsg = ClientToServerSignifiers.SendOnlyObserverGameRoomChatMSG + ", OnlyObservers <" + n + "> : " + ourChatText;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);

        }

        else if (game_room_chat_room_everyone_toggle.GetComponent<Toggle>().isOn)
        {
            string ourMsg = ClientToServerSignifiers.SendGameRoomGlobalChatMSG + ", Globe < " + n + " >  : " + ourChatText;
            network.GetComponent<NetworkedClient>().SendMessageToHost(ourMsg);
        }

        

    }

    public void GlobleToggleChanged(bool newValue)
    {

        game_room_chat_room_player_toggle.GetComponent<Toggle>().isOn = false;
        game_room_chat_room_observer_toggle.GetComponent<Toggle>().isOn = false;
        game_room_chat_room_send_text.GetComponent<Text>().text = "Sending Message To: Globle";

        if (game_room_chat_room_everyone_toggle.GetComponent<Toggle>().isOn == false && game_room_chat_room_observer_toggle.GetComponent<Toggle>().isOn == false
            && game_room_chat_room_player_toggle.GetComponent<Toggle>().isOn == false)
        {
            game_room_chat_room_send_text.GetComponent<Text>().text = "Sending Message To: NoOne";
        }
    }

    public void ObserverToggleChanged(bool newValue)
    {

        game_room_chat_room_player_toggle.GetComponent<Toggle>().isOn = false;
        game_room_chat_room_everyone_toggle.GetComponent<Toggle>().isOn = false;
        game_room_chat_room_send_text.GetComponent<Text>().text = "Sending Message To: Observers";

        if (game_room_chat_room_everyone_toggle.GetComponent<Toggle>().isOn == false && game_room_chat_room_observer_toggle.GetComponent<Toggle>().isOn == false
            && game_room_chat_room_player_toggle.GetComponent<Toggle>().isOn == false)
        {
            game_room_chat_room_send_text.GetComponent<Text>().text = "Sending Message To: NoOne";
        }
    }


    public void PlayerToggleChanged(bool newValue)
    {

        game_room_chat_room_everyone_toggle.GetComponent<Toggle>().isOn = false;
        game_room_chat_room_observer_toggle.GetComponent<Toggle>().isOn = false;
        game_room_chat_room_send_text.GetComponent<Text>().text = "Sending Message To: Players";

        if (game_room_chat_room_everyone_toggle.GetComponent<Toggle>().isOn == false && game_room_chat_room_observer_toggle.GetComponent<Toggle>().isOn == false
            && game_room_chat_room_player_toggle.GetComponent<Toggle>().isOn == false)
        {
            game_room_chat_room_send_text.GetComponent<Text>().text = "Sending Message To: NoOne";
        }
    }

    void ClearOldGameRoomChatMessage()
    {
        if (ListPrefabTextObject != null && ListPrefabTextObject.Count != 0)
        {

            for (int i = 0; ListPrefabTextObject.Count > 50; i++)
            {
                Destroy(ListPrefabTextObject[i]);
                ListPrefabTextObject.RemoveAt(i);
            }
           
        }
    }
    #endregion 


    private void Update()
    {
        ClearOldGameRoomChatMessage();
    }
}