/*Copyright 2022 Guillaume Spalla

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

/**
 * Manages the debug messages: can be sent to an hologram or to the console.
 * A filter to get the message only from certain classes is also implemented.
 * */
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
    public bool m_displayMessages; // True: messages displayed; False otherwise

    private static MouseDebugMessagesManager _instance;

    public static MouseDebugMessagesManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_classNameFilter = new List<string>();

            // For now, filtering is hard coded
            //m_classNameFilter.Add("MouseChallengeCleanTableReminder");
            //m_classNameFilter.Add("MouseUtilitiesGradationAssistanceManager");
            /*m_classNameFilter.Add("MouseUtilitiesHolograms");
            m_classNameFilter.Add("MouseCueing");*/

            m_displayMessages = true;

            _instance = this;
        }
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
        if (m_displayMessages)
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
}
