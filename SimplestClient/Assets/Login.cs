using UnityEngine;
using UnityEngine.UI;


public class Login : MonoBehaviour
{

    #region GameObject
    private NetworkedClient message_receiver_from_server = null;
    GameObject submit_button, user_name_input_field, password_input_field, create_toggle, login_toggle, status_result_text,system_manager ;
    GameObject network;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Initializing
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "UserNameInputField")
                user_name_input_field = go;
            else if (go.name == "PasswordInputField")
                password_input_field = go;
            else if (go.name == "SubmitButton")
                submit_button = go;
            else if (go.name == "LoginToggle")
                login_toggle = go;
            else if (go.name == "CreateToggle")
                create_toggle = go; 
            else if (go.name == "Network")
                 network = go;
            else if (go.name == "Status_Description")
                status_result_text = go;
            else if (go.name == "SystemManagerObject")
                system_manager = go;
        }

        submit_button.GetComponent<Button>().onClick.AddListener(SubmitButtonOnPressed);
        login_toggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        create_toggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);



        message_receiver_from_server = network.GetComponent<NetworkedClient>();
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += LoginReceivedMessageFromServer;
        }
    }


    #region InterfaceCode

    public void SubmitButtonOnPressed() 
    {
        if (password_input_field.GetComponent<InputField>().text != "" && password_input_field.GetComponent<InputField>().text != null 
           && user_name_input_field.GetComponent<InputField>().text != "" && user_name_input_field.GetComponent<InputField>().text != null)
        {
            Debug.Log("SubmitButton has been press");
            string p = password_input_field.GetComponent<InputField>().text;
            string n = user_name_input_field.GetComponent<InputField>().text;

            string msg;
            if (create_toggle.GetComponent<Toggle>().isOn)
            {
                msg = ClientToServerSignifiers.CreateAcount + "," + n + "," + p;
            }
            else
            {
                msg = ClientToServerSignifiers.Login + "," + n + "," + p;
            }
            network.GetComponent<NetworkedClient>().SendMessageToHost(msg);
            
        }
    }
    
    public void LoginToggleChanged (bool newValue)
    {
        create_toggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    public void CreateToggleChanged(bool newValue) 
    {
        login_toggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    #endregion


    #region ReceviedMessageFromTheServer/Involved
    private void LoginReceivedMessageFromServer(int signifier, string s, TicTacToeBoard t, MatchData matchData) 
    {
        if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            StoreUserNameSystemMangaer(s);
               
        }
        else if (signifier == ServerToClientSignifiers.LoginFailedAccount)
        { 
            InforThePlayerOFResult(2);
        }
        else if (signifier == ServerToClientSignifiers.LoginFailedPassword)
        {
          InforThePlayerOFResult(3);
        }
        else if (signifier == ServerToClientSignifiers.CreateAcountComplete)
        { 
            
           InforThePlayerOFResult(4);
           
        }
        else if (signifier == ServerToClientSignifiers.CreateAcountFailed)
        { 
            InforThePlayerOFResult(5);
        }

    }

    private void OnDestroy()
    {
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= LoginReceivedMessageFromServer;
        }

    }

   void InforThePlayerOFResult(int result) 
   {

        switch (result)
        {
            case 2:
                status_result_text.GetComponent<Text>().text = "Status: User name Invalid";
                break;
            case 3:
                status_result_text.GetComponent<Text>().text = "Status: Password Invalid";
                break;
            case 4:
                status_result_text.GetComponent<Text>().text = "Status: Account Creation Complete";
                break;
            case 5:
                status_result_text.GetComponent<Text>().text = "Status: UserName In used";
                break;       
        }

   }

   void StoreUserNameSystemMangaer(string user_name)
   {
       system_manager.GetComponent<SystemManager>().SetUserName = user_name;
   }
    #endregion



    #region ResetFunction
    public void ResetLogin()
    {
        status_result_text.GetComponent<Text>().text = "Status:";
        user_name_input_field.GetComponent<InputField>().text = "";
        password_input_field.GetComponent<InputField>().text = "";
    }

    #endregion

}
