using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

public class MouseUtilitiesGradationAssistanceManager : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    Dictionary<string, MouseUtilitiesGradationAssistance> m_assistanceGradation; // id of the gradation, actual assistance.

    string m_gradationPrevious;
    string m_gradationCurrent;
    string m_gradationNext;
    string m_gradationInitial;

    //int m_assistanceGradationIndexCurrent;

    private void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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

    public MouseUtilitiesGradationAssistanceManager()
    {
        //m_assistanceGradationIndexCurrent = -1; // i.e. no assistance in the list.
        m_assistanceGradation = new Dictionary<string, MouseUtilitiesGradationAssistance>();
        m_gradationPrevious = "";
        m_gradationCurrent = "";
        m_gradationNext = "";
    }

    public MouseUtilitiesGradationAssistance addNewAssistanceGradation(string id/*, MouseUtilitiesGradationAssistance previous , MouseUtilitiesGradationAssistance next*/)
    {
        MouseUtilitiesGradationAssistance newItem = new MouseUtilitiesGradationAssistance(id);
        newItem.m_debug = m_debug;
        //newItem.addHideShowsPair(pairs);
        //newItem.setGradationNext(next);
        //newItem.setGradationPrevious(previous);

        m_assistanceGradation.Add(newItem.getId(), newItem);

        /*if (m_assistanceGradationIndexCurrent == -1)
        { // If this is the first gradation, then select it as the current one
            m_assistanceGradationIndexCurrent = 0;
        }*/

        if (m_gradationCurrent == "")
        {
            m_gradationCurrent = id;
        }

        // Setting the internal callbacks to actually go to the next state
        List<Action<EventHandler>> fShows = newItem.getFunctionsShow();
        List<EventHandler> fShowsEventHandlers = newItem.getFunctionsShowEventHandlers();

        fShows.Add(delegate (EventHandler e)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Setting the new current: current>previous: " + m_gradationCurrent + " next>current: " + m_gradationNext);


            m_gradationPrevious = m_gradationCurrent;
            m_gradationCurrent = m_gradationNext; // nextGradation.getNextGradationState().getId();//.Item1.getId();
        });

        List<EventHandler> actionsEventHandler = newItem.getFunctionsShowEventHandlers();
        actionsEventHandler.Add(MouseUtilities.getEventHandlerEmpty());

        newItem.m_triggerNext += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseUtilitiesGradationAssistance caller = (MouseUtilitiesGradationAssistance)o;
            MouseUtilisiesGradationAssistanceArgs args = (MouseUtilisiesGradationAssistanceArgs)e;
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Caller: " + caller.getId() + " | next state: " + args.m_nextState);

            goToNextState(args.m_nextState);
        });

        newItem.m_triggerPrevious += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseUtilitiesGradationAssistance caller = (MouseUtilitiesGradationAssistance)o;

            //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Caller: " + caller.getId() + " | going to previous state: " + m_gradationPrevious);

            //goToNextState(m_gradationPrevious);
            goToPreviousState();
        });

        return newItem;
    }

    /*public void addPairToAssistanceGradationNextState(string idAssistance, string idAssistanceNextState, MouseUtilitiesGradationAssistance next, List<MouseUtilitiesGradationAssistanceHideShowPair> pairs)
    {
        m_assistanceGradation[idAssistance].addGradationNext(idAssistanceNextState, next, pairs);
    }

    public void setPairsToAssistanceGradationPreviousState(string idAssistance, MouseUtilitiesGradationAssistance previous, List<MouseUtilitiesGradationAssistanceHideShowPair> pairs)
    {
        m_assistanceGradation[idAssistance].setGradationPrevious(previous, pairs);
    }*/



    /*
     * Return true if max gradation is reached, false otherwise
     * */
    public bool goToNextState(string idNext)
    {
        bool toReturn = false;

        /*int nbGradations = m_assistanceGradation.Count;

        if (m_assistanceGradationIndexCurrent < nbGradations)
        {
            m_assistanceGradationIndexCurrent++;

            m_assistanceGradation[m_assistanceGradationIndexCurrent].callback?.Invoke(this, EventArgs.Empty);
        }


        if (m_assistanceGradationIndexCurrent == nbGradations - 1)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Maximum gradation level reached");
            toReturn = true;
        }*/

        // 

        //Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>> nextGradation = m_assistanceGradation[m_gradationCurrent].getGradationNext(idNext);
        //MouseUtilitiesGradationAssistanceNextState nextGradation = m_assistanceGradation[m_gradationCurrent].getGradationNext(idNext);
        MouseUtilitiesGradationAssistance stateCurrent = m_assistanceGradation[m_gradationCurrent];
        MouseUtilitiesGradationAssistance stateNext = m_assistanceGradation[m_gradationCurrent].getGradationNext(idNext);

        

        if (stateNext == null)
        { // Means we have reached the last state of the state machine. So nothing to do excepted informing the user
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The state " + idNext + " does not exist. Nothing will happen. This can be a normal behavior. For information, current state: " + stateCurrent.getId());

            toReturn = true;
        }
        else
        {
            m_gradationNext = stateNext.getId();
            //MouseUtilitiesGradationAssistance stateNext = stateCurrent.getGradationNext(idNext);
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Transitions from " + stateCurrent.getId() + " to " + stateNext.getId() + ". Number of show functions to call: " + stateNext.getFunctionsShow().Count);

            goToState(stateCurrent, stateNext);

            //raiseEventsGradation(stateCurrent.getFunctionHide(), stateCurrent.getFunctionHideEventHandler(), stateNext.getFunctionsShow(), stateNext.getFunctionsShowEventHandlers());

            //m_assistanceGradation[m_gradationCurrent].

            /*foreach (MouseUtilitiesGradationAssistanceHideShowPair pair in nextGradation.getTransitions())
            {
                List<Action<EventHandler>> actions = pair.getFunctionsShow();
                actions.Add(delegate (EventHandler e)
                {
                    m_gradationCurrent = nextGradation.getNextGradationState().getId();//.Item1.getId();
                });

                List<EventHandler> actionsEventHandler = pair.getFunctionsShowEventHandlers();
                actionsEventHandler.Add(MouseUtilities.getEventHandlerEmpty());

                // Adding internal function to swtch to next state once this is all finished
                raiseEventsGradation(pair.getFunctionHide(), pair.getFunctionHideEventHandler(), actions, actionsEventHandler);
            }*/
        }

        

        return toReturn;
    }

    public void goToPreviousState()
    { // No arguments as previous state is known internally
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Transitions from " + m_assistanceGradation[m_gradationCurrent].getId() + " to " + m_assistanceGradation[m_gradationPrevious].getId());

        m_gradationNext = m_gradationPrevious; // Little trick to handle correctly the transition to the previous state, as next becomes current and current becomes previous.

        goToState(m_assistanceGradation[m_gradationCurrent], m_assistanceGradation[m_gradationPrevious]);
    }

    // Don't make this function public! Should not be called if you do not know what your are doing.
    void goToState(MouseUtilitiesGradationAssistance current, MouseUtilitiesGradationAssistance next)
    { // Current: will call hide function; Next: will call show functions
        raiseEventsGradation(current.getFunctionHide(), current.getFunctionHideEventHandler(), next.getFunctionsShow(), next.getFunctionsShowEventHandlers());
    }

    /*
     * Return true if min gradation is reached, false otherwise
     * */
    /*public bool decreaseGradation()
    {
        bool toReturn = false;*/

        /*if (m_assistanceGradationIndexCurrent > 0) // 0 being the first element of the list, i.e. the minimal one
        {
            m_assistanceGradationIndexCurrent--;

            m_assistanceGradation[m_assistanceGradationIndexCurrent].callback?.Invoke(this, EventArgs.Empty);
        }

        if (m_assistanceGradationIndexCurrent == 0)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Minimum gradation level reached");

            toReturn = true;
        }*/

        //Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>> previousGradation = m_assistanceGradation[m_gradationCurrent].getGradationPrevious();
        //m_assistanceGradation[m_gradationCurrent].getGradationNext(idNext).Item2
        /*MouseUtilitiesGradationAssistance previousGradation = m_assistanceGradation[m_gradationCurrent].getGradationPrevious();

        if (previousGradation == null)
        { // Means we are already at the first state, so nothing to do, excepted informing the caller.
            toReturn = true;
        }
        else
        {
            foreach (MouseUtilitiesGradationAssistanceHideShowPair pair in m_assistanceGradation[m_gradationCurrent].getGradationPrevious().Item2)
            {
                List<Action<EventHandler>> actions = pair.getFunctionsShow();
                actions.Add(delegate (EventHandler e)
                {
                    m_gradationCurrent = previousGradation.Item1.getId();
                });

                List<EventHandler> actionsEventHandlers = pair.getFunctionsShowEventHandlers();
                actionsEventHandlers.Add(MouseUtilities.getEventHandlerEmpty());

                // Adding internal function to swtch to next state once this is all finished
                raiseEventsGradation(pair.getFunctionHide(), pair.getFunctionHideEventHandler(), actions, actionsEventHandlers);
            }
        }

        
        */
    /*    return toReturn;
    }*/

    void raiseEventsGradation(Action<EventHandler> fHide, EventHandler fHideEventHandler, List<Action<EventHandler>> fShows, List<EventHandler> fShowsEventHandler)
    {
        fHide(new EventHandler(delegate (System.Object o, EventArgs e)
        {
            for (int i = 0; i < fShows.Count; i ++)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Function show " + i + " is going to be called");

                fShows[i](fShowsEventHandler[i]);
            }

            /*foreach (Action<EventHandler> fShow in fShows)
            {
                fShow(MouseUtilities.getEventHandlerEmpty());
            }*/

            fHideEventHandler?.Invoke(this, EventArgs.Empty);
        }));
    }

    // This might be optimized by having a pointer to the original state, that updates each time a new state is added.
    /*public void setGradationToMinimum()
    {*/
        /*m_assistanceGradationIndexCurrent = 0;

        m_assistanceGradation[m_assistanceGradationIndexCurrent].callback?.Invoke(this, EventArgs.Empty);*/

        //m_gradationCurrent = 

        // From the current state, going back to the starting state
        //Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>> previousGradation = m_assistanceGradation[m_gradationCurrent].getGradationPrevious();
        /*MouseUtilitiesGradationAssistance previousGradation = m_assistanceGradation[m_gradationCurrent].getGradationPrevious();

        while (previousGradation != null)
        {
            //m_gradationCurrent = previousGradation.Item1.getId();
            m_gradationCurrent = previousGradation.getId();
            previousGradation = m_assistanceGradation[m_gradationCurrent].getGradationPrevious();
        }*/

        //m_gradationCurrent = m_gradationInitial;
        //goToNextState(m_gradationInitial);
    //}
}

public class MouseUtilitiesGradationAssistance
{
    string m_id;

    //Dictionary<string, Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>> m_nextStates; // Id of the state, <next state corresponding to this ID, list of hide / show pair>. Use of this way because a same state can go to different next states following the provided interaction
    //Dictionary<string, MouseUtilitiesGradationAssistanceNextState> m_nextStates; // Id of the state, <next state corresponding to this ID, list of hide / show pair>. Use of this way because a same state can go to different next states following the provided interaction
    Dictionary<string, MouseUtilitiesGradationAssistance> m_nextStates; // Id of the state, <next state corresponding to this ID, list of hide / show pair>. Use of this way because a same state can go to different next states following the provided interaction

    //Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>> m_gradationPrevious; // One tuple here with the previous state and the list of hide/show pair. Only one here compared to the "m_nextStates" variable as a state can only have one previous state.
    //MouseUtilitiesGradationAssistance m_gradationPrevious;

    //MouseUtilitiesGradationAssistance m_gradationNext;
    //List<MouseUtilitiesGradationAssistanceHideShowPair> m_hideShowsPairs;

    List<Action<EventHandler>> m_functionsShow;
    List<EventHandler> m_functionsShowEventHandlers;
    Action<EventHandler> m_functionHide;
    EventHandler m_functionHideEventHandler;

    public event EventHandler m_triggerNext;
    public event EventHandler m_triggerPrevious;

    public MouseDebugMessagesManager m_debug;

    public MouseUtilitiesGradationAssistance(string id)
    {
        m_id = id;
        //m_hideShowsPairs = new List<MouseUtilitiesGradationAssistanceHideShowPair>();
        //m_gradationPrevious = null;
        //m_gradationNext = null;
        //m_nextStates = new Dictionary<string, Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>>();
        //m_nextStates = new Dictionary<string, MouseUtilitiesGradationAssistanceNextState>();
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

    /*public void setGradationPrevious(MouseUtilitiesGradationAssistance previous, List<MouseUtilitiesGradationAssistanceHideShowPair> pairs)
    {
        m_gradationPrevious = new Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>(previous, pairs);
    }

    public Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>> getGradationPrevious()
    {
        return m_gradationPrevious;
    }*/

    public EventHandler setGradationPrevious(/*MouseUtilitiesGradationAssistance previous*/)
    {
        //m_gradationPrevious = previous;

        return new EventHandler(delegate (System.Object o, EventArgs e)
        {
            //MouseUtilisiesGradationAssistanceArgs temp = new MouseUtilisiesGradationAssistanceArgs();
            //temp.m_nextState = nextState.getId();

            m_triggerPrevious?.Invoke(this, EventArgs.Empty);
        });
    }

    /*public MouseUtilitiesGradationAssistance getGradationPrevious()
    {
        return m_gradationPrevious;
    }*/

    /*public void addGradationNext(string id, MouseUtilitiesGradationAssistance next, List<MouseUtilitiesGradationAssistanceHideShowPair> pairs)
    {
        //m_gradationNext = next;
        MouseUtilitiesGradationAssistanceNextState temp = new MouseUtilitiesGradationAssistanceNextState();
        temp.setNextGradationState(next);
        //MouseUtilitiesGradationAssistanceHideShowPair  temp.addTransitionNextState();
        temp.setTransitionsNextState(pairs);

        //m_nextStates.Add(id, new Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>(next, pairs));
        m_nextStates.Add(id, temp);

    }*/

    /*public MouseUtilitiesGradationAssistanceNextState addGradationNext(string id)
    {
        //m_nextStates.Add(id, new Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>());
        m_nextStates.Add(id, new MouseUtilitiesGradationAssistanceNextState());
        return m_nextStates[id];
    }*/

    public EventHandler addGradationNext(MouseUtilitiesGradationAssistance nextState)
    {
        //m_nextStates.Add(id, new Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>());
        m_nextStates.Add(nextState.getId(), nextState);
        //return m_nextStates[id];

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Next state added: " + nextState.getId());

        return new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseUtilisiesGradationAssistanceArgs temp = new MouseUtilisiesGradationAssistanceArgs();
            temp.m_nextState = nextState.getId();

            m_triggerNext?.Invoke(this, temp);
        });
    }

    // Returns null if key does not exist. Most likely means that you have reached the last state of the state machine. So sad.
    public /*Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>>*/ /*MouseUtilitiesGradationAssistanceNextState*/ MouseUtilitiesGradationAssistance getGradationNext(string id)
    {
        //Tuple<MouseUtilitiesGradationAssistance, List<MouseUtilitiesGradationAssistanceHideShowPair>> toReturn = null;
        //MouseUtilitiesGradationAssistanceNextState toReturn = null;
        MouseUtilitiesGradationAssistance toReturn = null;

        if (m_nextStates.ContainsKey(id))
        {
            toReturn = m_nextStates[id];
        }
        //return m_gradationNext;
        return toReturn;
    }

    /*public void addHideShowsPair(MouseUtilitiesGradationAssistanceHideShowPair pair)
    {
        m_hideShowsPairs.Add(pair);
    }

    public List<MouseUtilitiesGradationAssistanceHideShowPair> getPairs()
    {
        return m_hideShowsPairs;
    }*/

    public void addFunctionShow(Action<EventHandler> fShow, EventHandler handler)
    {
        m_functionsShow.Add(fShow);
        m_functionsShowEventHandlers.Add(handler);
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

    public Action<EventHandler> getFunctionHide()
    {
        return m_functionHide;
    }

    public EventHandler getFunctionHideEventHandler()
    {
        return m_functionHideEventHandler;
    }
}

public class MouseUtilisiesGradationAssistanceArgs : EventArgs
{
    public string m_nextState;
}


public class MouseUtilitiesGradationAssistanceHideShowPair
{
    Action<EventHandler> m_functionHide;
    EventHandler m_functionHideEventHandler;
    List<Action<EventHandler>> m_functionsShow;
    List<EventHandler> m_functionsShowEventHandlers;

    public MouseUtilitiesGradationAssistanceHideShowPair()
    {
        m_functionHide = null;
        m_functionsShow = new List<Action<EventHandler>>();
        m_functionsShowEventHandlers = new List<EventHandler>();
    }

    public void setFunctionHide(Action<EventHandler> f)
    {
        setFunctionHide(f, MouseUtilities.getEventHandlerEmpty());
    }

    public void setFunctionHide(Action<EventHandler> f, EventHandler arg)
    {
        m_functionHide = f;
        m_functionHideEventHandler = arg;
    }

    public void addFunctionShow(Action<EventHandler> f)
    {
        addFunctionShow(f, MouseUtilities.getEventHandlerEmpty());
    }

    public void addFunctionShow(Action<EventHandler> f, EventHandler arg)
    {
        m_functionsShow.Add(f);
        m_functionsShowEventHandlers.Add(arg);
    }

    public Action<EventHandler> getFunctionHide()
    {
        return m_functionHide;
    }

    public EventHandler getFunctionHideEventHandler()
    {
        return m_functionHideEventHandler;
    }

    public List<Action<EventHandler>> getFunctionsShow()
    {
        return m_functionsShow;
    }

    public List<EventHandler> getFunctionsShowEventHandlers()
    {
        return m_functionsShowEventHandlers;
    }
}



public class MouseUtilitiesGradationAssistanceNextState
{
    MouseUtilitiesGradationAssistance m_nextGradationState;
    List<MouseUtilitiesGradationAssistanceHideShowPair> m_transitionsToNextState;

    public MouseUtilitiesGradationAssistanceNextState()
    {
        m_nextGradationState = null;

        m_transitionsToNextState = new List<MouseUtilitiesGradationAssistanceHideShowPair>();
    }

    public void setNextGradationState(MouseUtilitiesGradationAssistance nextState)
    {
        m_nextGradationState = nextState;
    }

    public MouseUtilitiesGradationAssistanceHideShowPair addTransitionNextState()
    {
        m_transitionsToNextState.Add(new MouseUtilitiesGradationAssistanceHideShowPair());

        return m_transitionsToNextState.Last();
    }

    // Be careful: replaces the current list of transitions.
    public void setTransitionsNextState(List<MouseUtilitiesGradationAssistanceHideShowPair> transitions)
    {
        m_transitionsToNextState = transitions;
    }

    public List<MouseUtilitiesGradationAssistanceHideShowPair> getTransitions()
    {
        return m_transitionsToNextState;
    }

    public MouseUtilitiesGradationAssistance getNextGradationState()
    {
        return m_nextGradationState;
    }
}