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
using System.Text.RegularExpressions;

/**
 * Manages a basic state machine: you provide a list of states and when calling the increase of decrease function, will do so. Does nothing more.
 * */
public  class MouseScenarioManager : MonoBehaviour
{
    public MouseAssistanceDialog m_todo; //try to remove this line

    private List<MouseChallengeAbstract> m_scenarios; //List of scenario

    private static MouseScenarioManager _instance;

    public static MouseScenarioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_scenarios = new List<MouseChallengeAbstract>();

            _instance = this;
        }
        MouseScenarioManager.Instance.s_scenarioAdded += m_todo.callbackAddNewButton; //Try to put this line in GlobalInitializer !!!
        MouseScenarioManager.Instance.s_scenarioChecked += m_todo.callbackCheckButton; //If the task is finished => NOTHING !!! Try to put this line in GlobalInitializer like scenarioAdded!!!
        MouseScenarioManager.Instance.s_scenarioStart += m_todo.callbackStartButton;
    }

    public EventHandler s_scenarioAdded;
    public EventHandler s_scenarioChecked;
    public EventHandler s_scenarioStart;
    public void addScenario(MouseChallengeAbstract scenario,string id)
    {
        bool absent = true;
        foreach (MouseChallengeAbstract challenge in m_scenarios)
        { 
            if (challenge==scenario)
            {
                absent = false;
                break;
            }
        }
        if (absent)
        { 
            m_scenarios.Add(scenario); //add scenario in the list of scenarios
            //string name = getId(scenario);
            MouseEventHandlerArgString arg = new MouseEventHandlerArgString(id); //set a name to the scenario
            s_scenarioAdded?.Invoke(this, arg); //send information of new scenario => callback (MouseGlobalInitializer) => add new button (MouseAssistanceDialog)

            scenario.s_challengeOnSuccess += sendCallBackCheck;
            scenario.s_challengeOnStart += sendCallBackStart;
            /*
            SINON, passer la variable de la première inférence temporelle de chaque scénario en paramètre de addScenario
            Ajouter cette variable au MouseEventHandlerArgString comme l'id
            Quand l'heure de la todolist = heure du MouseEventHandlerArgString, passer le bouton en cyan
            Quand l'heure de la todolist = heure du MouseEventHandlerArgString + 1h, passer le bouton en rouge
            */
            //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Button added");


            void sendCallBackCheck(object o, EventArgs e) //change the scenario e in MouseEventHandlerArgString arg for the id
            {
                s_scenarioChecked?.Invoke(this, arg);
            }

            void sendCallBackStart(object o, EventArgs e)
            {
                s_scenarioStart?.Invoke(this, arg);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public string getId(MouseChallengeAbstract scenario)
    {
        
        string name = "";
        string temp = scenario.name;
        temp = Regex.Replace(temp, "Mouse","");
        temp = Regex.Replace(temp, "Challenge", "");
        foreach (Match matchs in Regex.Matches(temp, @"[A-Z][a-z]+"))
        {
            name = name+ " " + matchs.Value;
        }
       

        return name;
    }
    */
   
    
}