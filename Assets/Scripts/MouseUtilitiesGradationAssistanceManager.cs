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
 * State machine to handle the assistance gradation that contains more advanced functionalities that the basic one (who said that sounds logic? You won a candy).
 * The basic idea behing this is that each assistance as a life that start from a moment where it is "showed", and ends when it is "hidden". 
 * So first you create states with the addNewAssistanceGradation function.
 * Then, for each state, you have to add a pointer to one or several functions that should be called when the state is enabled and disabled. See the class below "MouseUtilitiesGradationAssistance" for more details. 
 * The state changes are done asynchrounously. In other words, you do not call the functions to move the states yourself. Rather, for a given state, you attach each next possible states to an event. This is not managed by this class but by the "MouseUtilitiesGradationAssistance" class below. See there for more information. This class registers automatically to each state change so that it knows when to trigger the hide / show functions. The hide function of the current state is first called. When it is finished, it calls all the show functions sequencially. It does not wait for a show function to finish to call the next one.
 * Handles a one level undo.
 * */
public class MouseUtilitiesGradationAssistanceManager
{
    Dictionary<string, MouseUtilitiesGradationAssistance> m_assistanceGradation; // id of the gradation, actual assistance.

    string m_gradationPrevious;
    string m_gradationCurrent;
    string m_gradationNext;
    string m_gradationInitial;

    public event EventHandler s_newStateSelected; // Emitted with a MouseUtilisiesGradationAssistanceArgCurrentState argument

    public string getGradationCurrent()
    {
        return m_gradationCurrent;
    }

    public string getGradationPrevious()
    {
        return m_gradationPrevious;
    }

    public void setGradationInitial(string id)
    {
        m_gradationInitial = id;
    }

    public MouseUtilitiesGradationAssistance getInitialAssistance()
    {
        MouseUtilitiesGradationAssistance toReturn;

        if (m_gradationInitial == "")
        {
            toReturn = null;
        }
        else
        {
            toReturn = m_assistanceGradation[m_gradationInitial];
        }

        return toReturn;
    }

    public MouseUtilitiesGradationAssistanceManager()
    {
        m_assistanceGradation = new Dictionary<string, MouseUtilitiesGradationAssistance>();
        m_gradationPrevious = "";
        m_gradationCurrent = "";
        m_gradationNext = "";
        m_gradationInitial = "";
    }

    public MouseUtilitiesGradationAssistance addNewAssistanceGradation(string id)
    {
        MouseUtilitiesGradationAssistance newItem = new MouseUtilitiesGradationAssistance(id);

        m_assistanceGradation.Add(newItem.getId(), newItem);

        if (m_gradationCurrent == "")
        {
            m_gradationCurrent = id;
        }

        // Setting the internal callbacks to actually go to the next state
        List<Action<EventHandler>> fShows = newItem.getFunctionsShow();
        List<EventHandler> fShowsEventHandlers = newItem.getFunctionsShowEventHandlers();

        fShows.Add(delegate (EventHandler e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Setting the new current: current>previous: " + m_gradationCurrent + " next>current: " + m_gradationNext);


            m_gradationPrevious = m_gradationCurrent;
            m_gradationCurrent = m_gradationNext;

            MouseUtilisiesGradationAssistanceArgCurrentState args = new MouseUtilisiesGradationAssistanceArgCurrentState(m_assistanceGradation[m_gradationCurrent]);
            //args.m_currentState

            s_newStateSelected?.Invoke(this, args);
        });

        List<EventHandler> actionsEventHandler = newItem.getFunctionsShowEventHandlers();
        actionsEventHandler.Add(MouseUtilities.getEventHandlerEmpty());

        newItem.m_triggerNext += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseUtilitiesGradationAssistance caller = (MouseUtilitiesGradationAssistance)o;
            MouseUtilisiesGradationAssistanceArgNextState args = (MouseUtilisiesGradationAssistanceArgNextState)e;

            if (caller.getId() == m_gradationCurrent)
            { // A same trigger can call several time the same event for different objects (typically, the reminder one for instance, which is present at different stages of the scenario). So here we take into account only the trigger sent from the current gradation level.
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Caller: " + caller.getId() + " | next state: " + args.m_nextState);

                goToNextState(args.m_nextState);
            }
            else
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Caller: " + caller.getId() + " is different from current state (" + m_gradationCurrent + ") so nothing will happen. This is labelled as a warning, by most likely this is a good safety thing.");
            }
        });

        newItem.m_triggerPrevious += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseUtilitiesGradationAssistance caller = (MouseUtilitiesGradationAssistance)o;

            goToPreviousState();
        });

        return newItem;
    }

    /*
     * Return true if max gradation is reached, false otherwise
     * */
    bool goToNextState(string idNext)
    {
        bool toReturn = false;

        MouseUtilitiesGradationAssistance stateCurrent = m_assistanceGradation[m_gradationCurrent];
        MouseUtilitiesGradationAssistance stateNext = m_assistanceGradation[m_gradationCurrent].getGradationNext(idNext);

        if (stateNext == null)
        { // Means we have reached the last state of the state machine. So nothing to do excepted informing the user
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The required next state " + idNext + " is not defined as a potentiel next state for the current state " + stateCurrent.getId() + ". Nothing will happen. This can be a normal behavior.");

            toReturn = true;
        }
        else
        {
            m_gradationNext = stateNext.getId();
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Transitions from " + stateCurrent.getId() + " to " + stateNext.getId() + ". Number of show functions to call: " + stateNext.getFunctionsShow().Count);

            goToState(stateCurrent, stateNext);

        }

        return toReturn;
    }

    void goToPreviousState()
    { // No arguments as previous state is known internally
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Transitions from " + m_assistanceGradation[m_gradationCurrent].getId() + " to " + m_assistanceGradation[m_gradationPrevious].getId());

        m_gradationNext = m_gradationPrevious; // Little trick to handle correctly the transition to the previous state, as next becomes current and current becomes previous.

        goToState(m_assistanceGradation[m_gradationCurrent], m_assistanceGradation[m_gradationPrevious]);
    }

    // Be careful with this function ! Should not be called directly if you do not know what your are doing. It is a short function but can mess up many things.
    void goToState(MouseUtilitiesGradationAssistance current, MouseUtilitiesGradationAssistance next)
    { // Current: will call hide function; Next: will call show functions
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called. Going from " + current.getId() + " to " + next.getId());

        raiseEventsGradation(current.getFunctionHide(), current.getFunctionHideEventHandler(), next.getFunctionsShow(), next.getFunctionsShowEventHandlers());
    }

    /**
     * Reponsible to call the hide event of the current state, and the shows functions of the next state.
     * */
    void raiseEventsGradation(Action<EventHandler> fHide, EventHandler fHideEventHandler, List<Action<EventHandler>> fShows, List<EventHandler> fShowsEventHandler)
    {
        fHide(new EventHandler(delegate (System.Object o, EventArgs e)
        {
            for (int i = 0; i < fShows.Count; i ++)
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Function show " + i + " is going to be called");

                fShows[i](fShowsEventHandler[i]);
            }

            fHideEventHandler?.Invoke(this, EventArgs.Empty);
        }));
    }

    public void goBackToOriginalState()
    {
        if (m_gradationInitial == "")
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Initial state not set - however required. So nothing will happen.");
        }
        else
        {
            m_gradationNext = m_gradationInitial;

            goToState(m_assistanceGradation[m_gradationCurrent], m_assistanceGradation[m_gradationNext]);
        }
    }
}

/*
 * This class managed the states for the above state machine manager. 
 * Once you have created a state with the constructor, you have to add a pointer to one or several functions that should be called when the state is enabled, thanks to the "addFunctionShow" function. The only requirement of these functions is to have a EvantHandler as input parameter. If your function does not have such parameter, you can easily encapsulate it in a delegate. The "show" functions are called sequentially.
 * Then do a similar process by setting a hide function using the "setHideFunction" parameter. As you can notice, you can set several "show" function and only one "hide" function. The reason is that it can happen that a "show" is composed of several steps, whereas the "hide" concerns only the object targeted by the state. Moreover, the "hide" function is used as a entry point to call the show functions. But you can also use a delegate to encapsulate this and do several processes. I would not advice that, because my feeling would be that something should be changed in the architecture of the code. However, if you do that, DO NOT FORGET to invoke the EventHandler given as input parameter at the end of your delegate. The reason is that due to the internal process, it won't be called automatically called.
 * */
public class MouseUtilitiesGradationAssistance
{
    string m_id;

    Dictionary<string, MouseUtilitiesGradationAssistance> m_nextStates; // Id of the state, <next state corresponding to this ID, list of hide / show pair>. Use of this way because a same state can go to different next states following the provided interaction

    List<Action<EventHandler>> m_functionsShow;
    List<EventHandler> m_functionsShowEventHandlers;
    Action<EventHandler> m_functionHide;
    EventHandler m_functionHideEventHandler;

    public event EventHandler m_triggerNext;
    public event EventHandler m_triggerPrevious;

    public MouseUtilitiesGradationAssistance(string id)
    {
        m_id = id;
        m_nextStates = new Dictionary<string, MouseUtilitiesGradationAssistance>();

        m_functionsShow = new List<Action<EventHandler>>();
        m_functionsShowEventHandlers = new List<EventHandler>();
        m_functionHide = null;
        m_functionHideEventHandler = null;
    }

    public string getId()
    {
        return m_id;
    }

    public EventHandler setGradationPrevious()
    {
        return delegate 
        {
            m_triggerPrevious?.Invoke(this, EventArgs.Empty);
        };
    }

    public EventHandler goToState(MouseUtilitiesGradationAssistance nextState)
    {
        m_nextStates.Add(nextState.getId(), nextState);

        return new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseUtilisiesGradationAssistanceArgNextState temp = new MouseUtilisiesGradationAssistanceArgNextState();
            temp.m_nextState = nextState.getId();

            m_triggerNext?.Invoke(this, temp);
        });
    }

    // Returns null if key does not exist. Most likely means that you have reached the last state of the state machine. So sad.
    public MouseUtilitiesGradationAssistance getGradationNext(string id)
    {
        MouseUtilitiesGradationAssistance toReturn = null;

        if (m_nextStates.ContainsKey(id))
        {
            toReturn = m_nextStates[id];
        }

        return toReturn;
    }

    public void addFunctionShow(Action<EventHandler> fShow, EventHandler handler)
    {
        m_functionsShow.Add(fShow);
        m_functionsShowEventHandlers.Add(handler);
    }

    public void addFunctionShow(MouseAssistanceAbstract assistance, EventHandler callback)
    {
        addFunctionShow(assistance.show, callback);
    }

    /**
     * Then no callback trigerred when processed finished
     * */
    public void addFunctionShow(MouseAssistanceAbstract assistance)
    {
        addFunctionShow(assistance.show, MouseUtilities.getEventHandlerEmpty());
    }

    public List<Action<EventHandler>> getFunctionsShow()
    {
        return m_functionsShow;
    }

    public List<EventHandler> getFunctionsShowEventHandlers()
    {
        return m_functionsShowEventHandlers;
    }

    public void setFunctionHide(Action<EventHandler> fHide, EventHandler handler)
    {
        m_functionHide = fHide;
        m_functionHideEventHandler = handler;
    }

    public void setFunctionHide(MouseAssistanceAbstract assistance, EventHandler callback)
    {
        setFunctionHide(assistance.hide, callback);
    }

    public void setFunctionHide(MouseAssistanceAbstract assistance)
    {
        setFunctionHide(assistance.hide, MouseUtilities.getEventHandlerEmpty());
    }

    public void setFunctionHideAndShow(MouseAssistanceAbstract assistance)
    {
        setFunctionHide(assistance);
        addFunctionShow(assistance);
    }

    public Action<EventHandler> getFunctionHide()
    {
        return m_functionHide;
    }

    public EventHandler getFunctionHideEventHandler()
    {
        return m_functionHideEventHandler;
    }

    public Dictionary<string, MouseUtilitiesGradationAssistance> getNextStates()
    {
        return m_nextStates;
    }
}

/**
 * Simple encapsulation to inform the main class about the ID of the next state
 * */
public class MouseUtilisiesGradationAssistanceArgNextState : EventArgs
{
    public string m_nextState;
}
public class MouseUtilisiesGradationAssistanceArgCurrentState : EventArgs
{
    public MouseUtilitiesGradationAssistance m_currentState;

    public MouseUtilisiesGradationAssistanceArgCurrentState(MouseUtilitiesGradationAssistance currentState)
    {
        m_currentState = currentState;
    }
}
