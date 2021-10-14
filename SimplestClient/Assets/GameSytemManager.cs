using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameSytemManager : MonoBehaviour
{
    private NetworkedClient m_MessageReceiverFromServer = null;


    private NetworkedClient m_On = null;

    //private bool IsLogin = false;  // property
    //public bool GetIsLogin // property
    //{
    //    get { return IsLogin; }
    //}



    //private string userName = "NoAccount";  // property
    //public string GetUserName  // property
    //{
    //    get { return userName; }
    //}
    //public string SetUserName  // property
    //{
    //    set { userName = value; }
    //}

    // Start is called before the first frame update

    GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle, StatusText,SystemManager ;
    GameObject networkClient;
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "UserNameInputField")
                userNameInput = go;
            else if (go.name == "PasswordInputField")
                passwordInput = go;
            else if (go.name == "SubmitButton")
                submitButton = go;
            else if (go.name == "LoginToggle")
                loginToggle = go;
            else if (go.name == "CreateToggle")
                createToggle = go; 
            else if (go.name == "Network")
                 networkClient = go;
            else if (go.name == "Status_Description")
                StatusText = go;
            else if (go.name == "SystemManagerObject")
                SystemManager = go;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonOnPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);


        // m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();
        //if (m_MessageReceiverFromServer != null)
        //{
        //   m_MessageReceiverFromServer.OnMessageReceivedFromServer+= LoginStates;
        //}


        m_On = networkClient.GetComponent<NetworkedClient>();
        if (m_On != null)
        {
            m_On.On += LoginStates;
        }
    }




    public void SubmitButtonOnPressed() 
    {
        Debug.Log("SubmitButton has been press");
        string p = passwordInput.GetComponent<InputField>().text;
        string n = userNameInput.GetComponent<InputField>().text;

        string msg;
        if (createToggle.GetComponent<Toggle>().isOn)
        {
            msg = ClientToServerSignifiers.CreateAcount + "," + n + "," + p;
        }
        else
        {
            msg = ClientToServerSignifiers.Login + "," + n + "," + p;
        }
        networkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log("msg: -> " + msg);

        //networkClient.GetComponent<NetworkedClient>().Msg();

    }
    
    public void LoginToggleChanged (bool newValue)
    {
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    public void CreateToggleChanged(bool newValue) 
    {
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }


    private void OnDestroy()
    {
        //if (m_MessageReceiverFromServer != null)
        //{
        //    m_MessageReceiverFromServer.OnMessageReceivedFromServer -= LoginStates;
        //}

        if (m_On != null)
        {
            m_On.On -= LoginStates;
        }

    }

    private void LoginStates(int signifier, string s) 
    {
        switch (signifier) 
        {
            case 1:
                StatusText.GetComponent<Text>().text = "Status: Login Success";
                SystemManager.GetComponent<SystemManager>().SetUserName = s;
                break;
            case 2:
                StatusText.GetComponent<Text>().text = "Status: User name Invalid";
                break;
            case 3:
                StatusText.GetComponent<Text>().text = "Status: Password Invalid";
                break;
            case 4:
                StatusText.GetComponent<Text>().text = "Status: Account Creation Complete";
                break;
            case 5:
                StatusText.GetComponent<Text>().text = "Status: UserName In used";
                break;
        }

        Debug.LogWarning("LoginStates");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

}
