using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{

    private NetworkedClient m_MessageReceiverFromServer = null;
    GameObject Login, Chat, networkClient;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.name == "Login_Ui")
                Login = go;
            else if (go.name == "Chat_UI")
                Chat = go;
            else if (go.name == "Network")
                networkClient = go;
        }
        m_MessageReceiverFromServer = networkClient.GetComponent<NetworkedClient>();
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer += OpenChatRoom;
        }
        Login.SetActive(true);
        Chat.SetActive(false);
    }


    private void OnDestroy()
    {
        if (m_MessageReceiverFromServer != null)
        {
            m_MessageReceiverFromServer.OnMessageReceivedFromServer -= OpenChatRoom;
        }
    }





    public void OpenChatRoom(int i) 
    {
        switch (i)
        {
            case 1:
                Login.SetActive(false);
                Chat.SetActive(true);
                break;
        }
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
