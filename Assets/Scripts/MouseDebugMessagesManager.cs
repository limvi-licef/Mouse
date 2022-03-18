using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

public class MouseDebugMessagesManager : MonoBehaviour
{
    List<string> m_classNameFilter;

    public enum MessageLevel
    {
        Info,
        Warning,
        Error
    }

    public bool m_displayOnConsole;

    private void Awake()
    {
        m_classNameFilter = new List<string>();

        // For now, filtering is hard coded
        //m_classNameFilter.Add("MouseChallengeCleanTableReminder");
    }

    // Start is called before the first frame update
    void Start()
    {
        

        if (m_classNameFilter.Count > 0)
        {
            displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Message filtering enabled. Only the messages from the following classes will be displayed: " + m_classNameFilter.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayMessage(string className, string functionName, MessageLevel messageLevel, string message)
    {
        if (m_classNameFilter.Count == 0 || m_classNameFilter.Contains(className))
        {
            // Building message
            string messageToDisplay = "[" + className + "::" + functionName + "] ";

            switch (messageLevel)
            {
                case MessageLevel.Info:
                    messageToDisplay += "Info";
                    break;
                case MessageLevel.Warning:
                    messageToDisplay += "Warning";
                    break;
                case MessageLevel.Error:
                    messageToDisplay += "Error";
                    break;
            }

            messageToDisplay += " - " + message;

            // Message is processed differently following if we want to have it shown in the console or in the Hololens
            if (m_displayOnConsole)
            {
                switch (messageLevel)
                {
                    case MessageLevel.Info:
                        Debug.Log(messageToDisplay);
                        break;
                    case MessageLevel.Warning:
                        Debug.LogWarning(messageToDisplay);
                        break;
                    case MessageLevel.Error:
                        Debug.LogError(messageToDisplay);
                        break;
                }
            }
            else
            {
                TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
                textMesh.SetText(textMesh.text + "\n" + messageToDisplay);
            }
        }
    }
}
