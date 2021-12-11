using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordRequest : MonoBehaviour
{
    #region GameObjects
    public GameObject input_field = null;
    public GameObject create_button = null;
    public GameObject information_text = null;
    public GameObject network_object = null;
    public GameObject system_manager_object = null;

    public NetworkedClient message_receiver_from_server = null;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        /// find the GameOject by name
        /// initialize gameobject
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "RecordRequest_InputField")
                input_field = go;
            else if (go.name == "RecordRequest_CreateButton")
                create_button = go;
            else if (go.name == "RecordRequest_TitleText")
                information_text = go;
            else if (go.name == "Network")
                 network_object = go;
            else if (go.name == "SystemManagerObject")
                system_manager_object = go;
        }

        /// initialize  the message_receiver_from_server
        message_receiver_from_server = network_object.GetComponent<NetworkedClient>();

        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer += RecordRequestReceivedDataFromServer;
        }

        /// set up createButton  to the CreateButtonOnPressed function
        create_button.GetComponent<Button>().onClick.AddListener(CreateButtonOnPressed);
    }


    private void OnDestroy()
    {
        if (message_receiver_from_server != null)
        {
            message_receiver_from_server.OnMessageReceivedFromServer -= RecordRequestReceivedDataFromServer;
        }
    }

    /// <summary>
    /// RecordRequestReceivedDataFromServer
    /// </summary>
    /// depending siginier we will know is what data being pass in and what to do with it
    public void RecordRequestReceivedDataFromServer(int signifier, string s, TicTacToeBoard t, MatchData matchData)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.CreateARecoredSuccess:
                ///- Success results
                /// pass into a true to display it
                DisplayRequestFromRecordResults(true);
                break;
            case ServerToClientSignifiers.CreateARecoredFail:
                ///- fail results
                /// pass into a false to display it
                DisplayRequestFromRecordResults(false);
                break;
            case ServerToClientSignifiers.LoginComplete:
                // we need to reset because the previous player has log out
                ResetRecordRequest();
                break;
            case ServerToClientSignifiers.RematchOfTicTacToeComplete:
                // we need to reset for next the game of TicTacToeComplete.
                ResetRecordRequest();
                break;
        }
    }


    /// <summary>
    /// ResetRecordRequest()
    /// - its use to reset RecordRequest_UI game object back to a state before  sending any request
    /// </summary>
    private void ResetRecordRequest()
    {

        create_button.GetComponent<Button>().interactable = true;
        input_field.GetComponent<InputField>().interactable = true;
        information_text.GetComponent<Text>().text = "Make a record Bottom or Just continue ";
        information_text.GetComponent<Text>().color = Color.black;
        input_field.GetComponent<InputField>().text = "";
       Debug.Log(" ResetRecordRequest be called");
    }

    // send a request to create record to server
    // give the server our username name and text in the inputfiend as the record name
    private void CreateButtonOnPressed()
    {
        message_receiver_from_server.SendMessageToHost(ClientToServerSignifiers.CreateARecored + "," + system_manager_object.GetComponent<SystemManager>().GetUserName + "," + input_field.GetComponent<InputField>().text.ToString());
    }

    // depending what msg the Server send back
    // we will know if record name is vaild or not
    // we don't want you mulitple request after you get it right
    // that why the button isn't interactable afterward
    private void DisplayRequestFromRecordResults(bool IsRecordNameVaild) 
    {
        if (IsRecordNameVaild)
        {
            information_text.GetComponent<Text>().text = "Success recoded saved.";
            information_text.GetComponent<Text>().color = Color.blue;
            input_field.GetComponent<InputField>().interactable = false;
        }
        else
        {
            information_text.GetComponent<Text>().text = "Invaild record Name.";
            information_text.GetComponent<Text>().color = Color.red;
        }
    } 
    
}
