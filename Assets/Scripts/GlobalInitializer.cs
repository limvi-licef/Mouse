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
namespace MATCH
{
    public class GlobalInitializer : MonoBehaviour
    {
        public AdminMenu m_adminMenu;
        public GameObject m_virtualRoom;
        MATCH.Assistances.Dialog m_todo;

        private void Awake()
        {
            m_todo = MATCH.Assistances.Factory.Instance.CreateToDoList("Choses � faire", ""); //create to do list
        }


        // Start is called before the first frame update
        void Start()
        {
            // Tuning parameters following if the software runs on the Unity editor or the Hololens
            if (Utilities.Utility.IsEditorSimulator() || Utilities.Utility.IsEditorGameView())
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Editor simulator");
            }
            else
            { // Means running in the Hololens, so adjusting some parameters
                DebugMessagesManager.Instance.m_displayOnConsole = false;
                m_adminMenu.m_menuStatic = false;
                m_virtualRoom.SetActive(false); // In the editor, the user does what he wants, but in the hololens, this should surely be disabled.
            }
            AdminMenu.Instance.addButton("Bring to do list window", delegate () { Utilities.Utility.bringObject(m_todo.transform); }); //add button to the admin menu
            AdminMenu.Instance.addSwitchButton("Lock To Do List", callbackLockToDo);
            initializeTodoList();
        }

        void initializeTodoList()
        {
            // First: check if some scenarios have been added, and if yes, add them to the GUI
            List<MATCH.Scenarios.Scenario> scenarios = MATCH.Scenarios.Manager.Instance.getScenarios();
            foreach (MATCH.Scenarios.Scenario scenario in scenarios)
            {
                addScenarioToGUI(scenario);
            }

            // Second: be prepared in case new scenarios are added
            MATCH.Scenarios.Manager.Instance.s_scenarioAdded += callbackNewScenarioInManager;
        }

        void callbackNewScenarioInManager(System.Object o, EventArgs e)
        {
            DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");

            addScenarioToGUI(MATCH.Scenarios.Manager.Instance.getScenarios().Last());
        }

        void addScenarioToGUI(MATCH.Scenarios.Scenario scenario)
        {
            MATCH.Assistances.Buttons.Basic button = m_todo.addButton(scenario.getId(), true); //add button
            scenario.s_challengeOnStart += button.callbackSetButtonBackgroundCyan; //m_todo.callbackStartButton;
            scenario.s_challengeOnSuccess += button.callbackSetButtonBackgroundGreen; //m_todo.callbackCheckButton;
        }

        // Update is called once per frame
        void Update()
        {
            ToDoListConfig(); //config of the todo list for update the time
        }

        void ToDoListConfig()
        {
            string date = System.DateTime.Now.ToString("D", new System.Globalization.CultureInfo("fr-FR"));
            string hour = System.DateTime.Now.ToString("HH:mm");
            m_todo.setDescription("Date : " + date + "                              Heure : " + hour + "\nSaison : " + getSeason(System.DateTime.Now) + "\n\nT�ches � r�aliser : ", 0.1f);
        }
        string getSeason(DateTime date)
        {
            float value = (float)date.Month + date.Day / 100f;
            if (value < 3.21 || value >= 12.22) return "Hiver";
            else if (value < 6.21) return "Printemps";
            else if (value < 9.23) return "�t�";
            else return "Automne";
        }

        void callbackLockToDo()
        {
            if (m_todo.GetComponent<ObjectManipulator>().enabled)
                m_todo.GetComponent<ObjectManipulator>().enabled = false;
            else
                m_todo.GetComponent<ObjectManipulator>().enabled = true;
        }


    }
}