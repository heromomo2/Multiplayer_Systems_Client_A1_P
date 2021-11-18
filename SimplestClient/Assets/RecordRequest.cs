using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordRequest : MonoBehaviour
{
    public GameObject m_inputField = null;
    public GameObject m_CreateButton = null;
    public GameObject m_InformationText = null;
    public GameObject m_Network = null;
    public GameObject m_SystemManager = null;

    public NetworkedClient m_MessageReceiverFromServer = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "RecordRequest_InputField")
                m_inputField = go;
            else if (go.name == "RecordRequest_CreateButton")
                m_CreateButton = go;
            else if (go.name == "RecordRequest_TitleText")
                m_InformationText = go;
            else if (go.name == "Network")
                 m_Network = go;
            else if (go.name == "SystemManagerObject")
                m_SystemManager = go;
        }

        m_MessageReceiverFromServer = m_Network.GetComponent<NetworkedClient>();

        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever += RecordRequestReceived;
        }
        m_CreateButton.GetComponent<Button>().onClick.AddListener(CreateButtonOnPressed);
    }


    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromSever -= RecordRequestReceived;
        }
    }


    public  void RecordRequestReceived(int signifier, string s, TicTacToeBoard t)
    {
        switch (signifier)
        {
            case ServerToClientSignifiers.CreateARecoredSuccess:
                DisplayRequestFromRecordResults(true);
                break;
            case ServerToClientSignifiers.CreateARecoredFail:
                DisplayRequestFromRecordResults(false);
                break;
            case ServerToClientSignifiers.LoginComplete:
                ResetRecordRequest();
                break;
            case ServerToClientSignifiers.ReMatchOfTicTacToeComplete:
                ResetRecordRequest();
                break;
        }
    }
    private void ResetRecordRequest()
    {

        m_CreateButton.GetComponent<Button>().interactable = true;
        m_inputField.GetComponent<InputField>().interactable = true;
        m_InformationText.GetComponent<Text>().text = "Make a record Bottom or Just continue ";
        m_InformationText.GetComponent<Text>().color = Color.black;
        m_inputField.GetComponent<InputField>().text = "";
       Debug.Log(" ResetRecordRequest be called");
    }


    private void CreateButtonOnPressed()
    {
        m_MessageReceiverFromServer.SendMessageToHost(ClientToServerSignifiers.CreateARecored + "," + m_SystemManager.GetComponent<SystemManager>().GetUserName + "," + m_inputField.GetComponent<InputField>().text.ToString());
    }

    private void DisplayRequestFromRecordResults(bool IsRecordNameVaild) 
    {
        if (IsRecordNameVaild)
        {
            m_InformationText.GetComponent<Text>().text = "Success recoded saved.";
            m_InformationText.GetComponent<Text>().color = Color.blue;
            m_inputField.GetComponent<InputField>().interactable = false;
        }
        else
        {
            m_InformationText.GetComponent<Text>().text = "Invaild record Name.";
            m_InformationText.GetComponent<Text>().color = Color.red;
        }
    }
    // Update is called once per frame
    
}
