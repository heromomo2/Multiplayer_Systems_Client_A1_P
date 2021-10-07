using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameSytemManager : MonoBehaviour
{
    private NetworkedClient m_MessageReceiverFromServer = null;


    //private void Awake()
    //{
    //    if (m_MessageReceiverFromServer != null)
    //    {
    //        m_MessageReceiverFromServer.OnMessageReceivedFromServer+= LoginStates;
    //    }
    //}

        // Start is called before the first frame update

        GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle, StatusText;
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
 
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonOnPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);
        

         m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();
        if (m_MessageReceiverFromServer != null)
        {
           m_MessageReceiverFromServer.OnMessageReceivedFromServer+= LoginStates;
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
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= LoginStates;
        }
    }

    private void LoginStates(int i) 
    {
        switch (i) 
        {
            case 1:
                StatusText.GetComponent<Text>().text = "Status: You are now Log-in";
                break;
            case 2:
                StatusText.GetComponent<Text>().text = "Status: Your User name is wrong";
                break;
            case 3:
                StatusText.GetComponent<Text>().text = "Status: Your password is wrong";
                break;
            case 4:
                StatusText.GetComponent<Text>().text = "Status: You have Created Account. Try Login";
                break;
            case 5:
                StatusText.GetComponent<Text>().text = "Status: We have Account with that user name.";
                break;
        }

        Debug.LogWarning("LoginStates");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

}
