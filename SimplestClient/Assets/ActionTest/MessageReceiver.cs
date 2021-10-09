using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class MessageReceiver : MonoBehaviour
{

    private Action<string> m_OnMessageReceived = null;


    private double m_Timer = 0;

    public event Action<string> OnMessageReceived
    {
        add
        {
            m_OnMessageReceived -= value;
            m_OnMessageReceived += value;
        }
        remove
        {
            m_OnMessageReceived -= value;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer > 2)
        {
            m_Timer = 0;
            ReceiveRandomNumber();
        }
    }

    private void ReceiveRandomNumber()
    {
        float recievedMessage = UnityEngine.Random.Range(0.0f, 1000.0f);

        Debug.Log("Received: " + recievedMessage.ToString());

        if (m_OnMessageReceived != null)
        {
            m_OnMessageReceived(recievedMessage.ToString());
        }
    }
}
