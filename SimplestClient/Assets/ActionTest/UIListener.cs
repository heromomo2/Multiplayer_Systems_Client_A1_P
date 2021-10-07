using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListener : MonoBehaviour
{
    public MessageReceiver m_MessageReceiver = null;

    private void Awake()
    {
        if(m_MessageReceiver != null)
        {
            m_MessageReceiver.OnMessageReceived += PrintHeardMessage;
            m_MessageReceiver.OnMessageReceived += SaveMessage;
        }
    }

    private void OnDestroy()
    {
        if (m_MessageReceiver != null)
        {
            m_MessageReceiver.OnMessageReceived -= PrintHeardMessage;
            m_MessageReceiver.OnMessageReceived -= SaveMessage;
        }
    }

    private void PrintHeardMessage(string message)
    {
        Debug.LogWarning("Message Heard: " + message);
    }

    private void SaveMessage(string messageToSave)
    {
        PlayerPrefs.SetString("SaveFile", messageToSave);
    }
}
