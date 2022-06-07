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

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

/**
 * This class allows to tune some initialization parameters following if the compilation is done in the editor or for the Hololens
 * */
public class MouseUtilitiesGlobalInitializer : MonoBehaviour
{
    public MouseUtilitiesAdminMenu m_adminMenu;
    public GameObject m_virtualRoom;
    public MouseAssistanceDialog m_todo; //R�f�rence vers le gameObject repr�sentant l'agenda

    private void Awake()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        // Tuning parameters following if the software runs on the Unity editor or the Hololens
        if (MouseUtilities.IsEditorSimulator() || MouseUtilities.IsEditorGameView())
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Editor simulator");
        }
        else
        { // Means running in the Hololens, so adjusting some parameters
            MouseDebugMessagesManager.Instance.m_displayOnConsole = false;
            m_adminMenu.m_menuStatic = false;
            m_virtualRoom.SetActive(false); // In the editor, the user does what he wants, but in the hololens, this should surely be disabled.
        }

        // Make links between classes if required

        //MouseScenarioManager.Instance.s_scenarioAdded += m_todo.callbackAddNewButton;
        
        m_todo.setTitle("Choses � faire",0.15f);

        initializeTodoList();
    }

    void initializeTodoList()
    {
        // First: check if some scenarios have been added, and if yes, add them to the GUI
        List<MouseChallengeAbstract> scenarios = MouseScenarioManager.Instance.getScenarios();
        foreach (MouseChallengeAbstract scenario in scenarios)
        {
            addScenarioToGUI(scenario);
        }

        // Second: be prepared in case new scenarios are added
        MouseScenarioManager.Instance.s_scenarioAdded += callbackNewScenarioInManager;
    }

    void callbackNewScenarioInManager(System.Object o, EventArgs e)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        addScenarioToGUI(MouseScenarioManager.Instance.getScenarios().Last());
    }

    void addScenarioToGUI(MouseChallengeAbstract scenario)
    {
        MouseAssistanceButton button = m_todo.addButton(scenario.getId(), true);
        scenario.s_challengeOnStart += button.callbackSetButtonBackgroundCyan; //m_todo.callbackStartButton;
        scenario.s_challengeOnSuccess += button.callbackSetButtonBackgroundGreen; //m_todo.callbackCheckButton;
    }

    // Update is called once per frame
    void Update()
    {
        ToDoListConfig();
    }
    
    void ToDoListConfig()
    {
        string date = System.DateTime.Now.ToString("D", new System.Globalization.CultureInfo("fr-FR"));
        string hour = System.DateTime.Now.ToString("HH:mm");
        m_todo.setDescription("Date : " + date + "                              Heure : " + hour + "\nSaison : "+getSeason(System.DateTime.Now) +"\n\nT�ches � r�aliser : ",0.1f);
    }
    string getSeason(DateTime date)
    {
        float value = (float)date.Month + date.Day / 100f;
        if (value < 3.21 || value >= 12.22) return "Hiver";
        else if (value < 6.21) return "Printemps";
        else if (value < 9.23) return "�t�";
        else return "Automne";
    }
    
}