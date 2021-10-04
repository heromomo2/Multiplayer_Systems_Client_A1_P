using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSytemManager : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle;
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
 
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonOnPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);
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
    }
    
    public void LoginToggleChanged (bool newValue)
    {
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    public void CreateToggleChanged(bool newValue) 
    {
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
