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
    }

    public EventHandler s_scenarioAdded;

    public void addScenario(MouseChallengeAbstract scenario)
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
            MouseEventHandlerArgString arg = new MouseEventHandlerArgString(scenario.getId()); //set a name to the scenario
            s_scenarioAdded?.Invoke(this, arg); //send information of new scenario => callback (MouseGlobalInitializer) => add new button (MouseAssistanceDialog)

            scenario.s_challengeOnSuccess += m_todo.callbackCheckButton;
            scenario.s_challengeOnStart += m_todo.callbackStartButton;        
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
}