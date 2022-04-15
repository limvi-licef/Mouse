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
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;
using TMPro;

public class MouseUtilitiesDisplayGraph : MonoBehaviour
{
    MouseUtilitiesGradationAssistanceManager m_manager;
    MouseUtilitiesGradationAssistance m_initialState;
    public GameObject m_refLabel;

    Transform m_parentLabelsView;

    Transform m_refConnectorView;

    Dictionary<string, GameObject> m_states;
    Dictionary<(string, string), GameObject> m_connectors;

    MouseUtilitiesGradationAssistance m_currentHighlightedState;

    private void Awake()
    {
        m_states = new Dictionary<string, GameObject>();
        m_connectors = new Dictionary<(string, string), GameObject>();
        m_parentLabelsView = gameObject.transform.Find("ButtonParent");

        m_refConnectorView = gameObject.transform.Find("Line");

        m_currentHighlightedState = null;

        
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseUtilitiesAdminMenu.Instance.addButton("Bring graph window", callbackBringWindow);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callbackBringWindow()
    {
        gameObject.transform.position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z);
        gameObject.transform.LookAt(Camera.main.transform);
        gameObject.transform.Rotate(new Vector3(0, 1, 0), 180);
    }


    public void setManager(MouseUtilitiesGradationAssistanceManager manager)
    {
        m_manager = manager;

        m_initialState = m_manager.getInitialAssistance();

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Original state: " + m_initialState.getId());
        displayStates(m_initialState, 0);

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Table surface should be touchable again");

        // Subscribing to the signal
        m_manager.s_newStateSelected += callbackNewStateSelected;
    }

    void displayStates(MouseUtilitiesGradationAssistance currentstate, int nbLevels)
    {
        addState(currentstate);

        foreach (KeyValuePair<string, MouseUtilitiesGradationAssistance> nextState in currentstate.getNextStates())
        {
            //Debug.Log()

            //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, nbLevels.ToString() + " " + nextState.Key);

            if(nextState.Key != m_initialState.getId())
            {
                displayStates(nextState.Value, nbLevels+1);
            }

            //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, nbLevels.ToString() + " " + nextState.Key);
            addConnector(currentstate, nextState.Value);
        }
    }

    void addState(MouseUtilitiesGradationAssistance state)
    {
        if (m_states.ContainsKey(state.getId()) == false)
        {
            GameObject temp = Instantiate(m_refLabel, m_parentLabelsView);
            temp.transform.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;

            temp.transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>().SetText(state.getId());

            m_parentLabelsView.GetComponent<GridObjectCollection>().UpdateCollection();

            m_states.Add(state.getId(), temp);
        }
    }

    //int oneConnector = 0;
    void addConnector(MouseUtilitiesGradationAssistance stateStart, MouseUtilitiesGradationAssistance stateEnd)
    {
        if (m_connectors.ContainsKey((stateStart.getId(), stateEnd.getId())) == false)
        {
            /*if (oneConnector <= 4)
            {*/
                Transform temp = Instantiate(m_refConnectorView, /*m_parentLabelsView*/gameObject.transform);

                MouseUtilitiesLineBetweenTwoPoints line = temp.GetComponent<MouseUtilitiesLineBetweenTwoPoints>();
                //line.drawLineWithArrow(m_states[stateStart.getId()], m_states[stateEnd.getId()]);

                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Connector from: " + stateStart.getId() + " to: " + stateEnd.getId());

            RectTransform transformGrid = m_parentLabelsView.GetComponent<RectTransform>();

                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Grid object width: " + transformGrid.rect.width + " height: " + transformGrid.rect.width + " x: " + transformGrid.rect.x +" y: " + transformGrid.rect.y);

            line.drawLineWithArrow(m_states[stateStart.getId()], m_states[stateEnd.getId()]);
                m_connectors.Add((stateStart.getId(), stateEnd.getId()), temp.gameObject);

               /* oneConnector ++;
            }*/
            
        }
    }

    public void callbackNewStateSelected(System.Object o, EventArgs args)
    {
        MouseUtilisiesGradationAssistanceArgCurrentState currentState = (MouseUtilisiesGradationAssistanceArgCurrentState)args;

        //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "New state to be highlighted: " + currentState.m_currentState.);


        if (m_currentHighlightedState != null)
        {
            m_states[m_currentHighlightedState.getId()].transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_HolographicBackPlate", typeof(Material)) as Material;
        }

        m_states[currentState.m_currentState.getId()].transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_Cyan_Glowing", typeof(Material)) as Material;

        // Brut force to highlight the connectors
        foreach (KeyValuePair<(string, string), GameObject> connector in m_connectors)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Connector: " + connector.Key + " Current state id: " + currentState.m_currentState.getId());

            if (connector.Key.Item1 == currentState.m_currentState.getId())
            {
                connector.Value.GetComponent<MouseUtilitiesLineBetweenTwoPoints>().highlightConnector(true);
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Highlight connector");
            }
            else
            {
                connector.Value.GetComponent<MouseUtilitiesLineBetweenTwoPoints>().highlightConnector(false);
            }
        }

        m_currentHighlightedState = currentState.m_currentState;
    }
}
