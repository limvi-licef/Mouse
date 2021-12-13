using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseDebugMessagesManager : MonoBehaviour
{
    public enum MessageLevel
    {
        Info,
        Warning,
        Error
    }

    public bool displayOnConsole;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayMessage(string className, string functionName, MessageLevel messageLevel, string message)
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
        if (displayOnConsole)
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
