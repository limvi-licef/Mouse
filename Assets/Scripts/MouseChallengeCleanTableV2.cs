using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;

public class MouseChallengeCleanTableV2 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;
    
    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    Transform m_containerTableView;
    MouseTable m_containerTableController;
    Transform m_containerRagView;
    MouseRag m_containerRagController;

    bool m_surfaceTableTouched; // Bool to detect the touch trigerring the challenge only once.
    bool m_surfaceRagTouched;

    public MouseUtilitiesTimer m_timer;

    enum ChallengeCleanTableStates
    {
        StandBy = 0,
        AssistanceStimulateLevel1 = 1,
        AssistanceStimulateLevel2 = 2,
        AssistanceCueing = 3,
        AssistanceSolution = 4,
        AssistanceReminder = 5,
        Challenge = 6,
        Success = 7
    }

    ChallengeCleanTableStates m_stateCurrent;
    ChallengeCleanTableStates m_statePrevious;

    Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_surfaceTableTouched = false;
        m_surfaceRagTouched = true; // True by default, as we do not want the table to be populated if the user grabs the rag without having the challenge starting first. Indeed, in this situation, that means the users does not need any assistance.

        m_stateCurrent = ChallengeCleanTableStates.StandBy;
        m_statePrevious = ChallengeCleanTableStates.StandBy;

        m_timer.m_timerDuration = 20;

        // Children
        m_containerRagView = gameObject.transform.Find("Rag");
        m_containerTableView = gameObject.transform.Find("Table");


        // Sanity checks


        // Callbacks
        m_containerTableController = m_containerTableView.GetComponent<MouseTable>();
        m_containerTableController.m_eventInteractionSurfaceTableTouched += callbackTableTouched;
        m_containerTableController.m_eventInteractionSurfaceCleaned += callbackSurfaceCleaned;

        m_containerTableController.m_eventReminderClockTouched += callbackHologramAssistanceReminderTouched;
        m_containerTableController.m_eventReminderOkTouched += callbackHologramAssistanceReminderOk;
        m_containerTableController.m_eventReminderBackTouched += callbackHologramAssistanceReminderBack;

        m_containerTableController.m_eventAssistanceStimulateLevel1Touched += callbackHologramAssistanceStimulateLevel1Help;

        m_containerRagController = m_containerRagView.GetComponent<MouseRag>();
        m_containerRagController.m_eventHologramHelpButtonTouched += callbackHologramAssistanceStimulateLevel2Help;
        m_containerRagController.m_eventHologramInteractionSurfaceTouched += callbackAssistanceRabGrabbed;
        m_containerRagController.m_eventHologramHelpButtonCueingTouched += callbackHologramAssistanceCueingHelp;
        m_containerRagController.m_eventHologramReminderClockTouched += callbackHologramAssistanceReminderTouched;
        m_containerRagController.m_eventHologramReminderOkTouched += callbackHologramAssistanceReminderOk;
        m_containerRagController.m_eventHologramReminderBackTouched += callbackHologramAssistanceReminderBack;

        m_containerTableController.m_eventAssistanceChallengeSuccessOk += callbackAssistanceChallengeSuccessOk;

        // Timer for gradation
        m_timer.m_eventTimerFinished += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            if (m_containerTableController.increaseGradationAssistanceStimulateLevel1() == false) 
            {  
                m_timer.m_timerDuration = 20;
                m_timer.startTimerOneShot();
            }
            else
            { // Means we have reached the last level
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Last gradation level reached for stimulate assistance level 1 - no timer will be started anymore");

                // Last level reached: increase the gradation level of the reminder window so that it will also follow the user
                m_containerTableController.increaseGradationAssistanceReminder();
            }
        });
    }

    void callbackHologramAssistanceStimulateLevel1Help(System.Object o, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.AssistanceStimulateLevel2);
    }

    void callbackAssistanceRabGrabbed(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_surfaceRagTouched == false)
        {
            updateChallenge(ChallengeCleanTableStates.Challenge);
            m_surfaceRagTouched = true;
        }
        
    }

    void callbackAssistanceChallengeSuccessOk(object sender, EventArgs e)
    {
        updateChallenge(ChallengeCleanTableStates.StandBy);
    }

    /*public void callbackHologramAssistanceSimulateLevel1FocusOn()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractFocusOn", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    public void callbackHologramAssistanceStimulateLevel1FocusOff()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractFocusOff", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Stop timer as the person does not look at the hologram anymore ...
        //stopTimerAssistanceStimulateAbstract();

        // ... so maybe trigger an audio gradation? 

    }*/

    public void callbackHologramAssistanceReminderTouched(System.Object o, EventArgs e)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
    }

    public void callbackHologramAssistanceStimulateLevel2Help(System.Object o, EventArgs e)
    {
        updateChallenge(ChallengeCleanTableStates.AssistanceCueing);

    }

    public void callbackHologramAssistanceReminderOk(System.Object o, EventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.StandBy);
    }

    public void callbackHologramAssistanceReminderBack(System.Object o, EventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderBack", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(m_statePrevious);
    }

        void callbackHologramAssistanceCueingHelp(System.Object o, EventArgs e)
    {
        updateChallenge(ChallengeCleanTableStates.AssistanceSolution);
    }

    // Update is called once per frame

    bool m_initFirstState = false;
    void Update()
    {
        if (m_initFirstState == false)
        { // Quick and very dirty way to initialise the first state to another state than the default state. The boolean allows to have it called only once. There is most lileky a much more elegant way to do, but well ... I have more urgent things to do at the moment.
            updateChallenge(ChallengeCleanTableStates.AssistanceStimulateLevel1);

            m_initFirstState = true;
        }
    }

    public void callbackTableTouched(System.Object o, EventArgs e)
    {
        if (m_surfaceTableTouched == false)
        { // This can be called only if the surface can be touched
            updateChallenge(ChallengeCleanTableStates.AssistanceStimulateLevel1);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Surface touched but process disabled");
        }
    }

    public void callbackSurfaceCleaned(System.Object o, EventArgs e)
    {
        updateChallenge(ChallengeCleanTableStates.Success);
    }

    public void resetChallenge()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "resetChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        
        m_surfaceTableTouched = false;
        m_surfaceRagTouched = false;

        m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());
        m_containerTableController.setAssistanceStimulateLevel1ToOriginalPosition();
        m_containerTableController.setGradationAssistanceStimulateLevel1ToMinimum();
        m_containerTableController.setGradationAssistanceReminderToMinimum();
        m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
        m_containerTableController.setAssistanceReminderToOriginalPosition();
        m_containerTableController.hideInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
        m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
        m_containerRagController.setAssistanceReminderToOriginalPosition();
        m_containerRagController.hideAssistanceCueing(MouseUtilities.getEventHandlerEmpty());
        m_containerRagController.hideAssistanceSolution(MouseUtilities.getEventHandlerEmpty());
        m_containerRagController.hideAssistanceStimulateLevel2(MouseUtilities.getEventHandlerEmpty());
        m_containerTableController.hideAssistanceChallengeSuccess(MouseUtilities.getEventHandlerEmpty());

        m_timer.stopTimer();

        if (m_stateCurrent != ChallengeCleanTableStates.StandBy) // To avoid an infinite loop in the case the function is called from the standby state
        {
            updateChallenge(ChallengeCleanTableStates.StandBy);
        }
        
    }

    void updateChallenge(ChallengeCleanTableStates newState)
    {
        m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Current state: " + m_stateCurrent.ToString() + " \nNew state: " + newState.ToString());

        m_statePrevious = m_stateCurrent;
        m_stateCurrent = newState;

        if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulateLevel1)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                m_containerTableController.hideAssistanceReminder(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerTableController.showAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());
                    m_containerTableController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                }));
            }
            else if (m_surfaceTableTouched == false) // Surface can be touched only once
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Assistance stimulate level 1 and reminder assistance are going to be shown");

                // Display the cube with the rag icon
                m_containerTableController.showAssistanceStimulateLevel1(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Assistance stimulate level 1 should be displayed now"));

                // Display the reminder icon
                m_containerTableController.showAssistanceReminder(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Assistance reminder should be displayed now"));

                m_surfaceTableTouched = true;
                m_surfaceRagTouched = false; // From now on, if the user touches the rag, the surface will be populated (at least the callback supposed to trigger it will be fired.

                // Play sound to get the user's attention from audio on top of visually
                m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);

                // Start timer for gratation
                m_timer.startTimerOneShot();
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Surface touched but process disabled");
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulateLevel2)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1)
            {
                m_timer.stopTimer();

                // Display the help and line components - From the new "opening cube", we leave it displayed until the next assistance gradation.
                m_containerRagController.showAssistanceStimulateLevel2(MouseUtilities.getEventHandlerEmpty());//.setHelpAndLineAnimateAppear();

                //Hide the clock component and make it appearing close to this new help component
                m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                m_containerTableController.setGradationAssistanceStimulateLevel1ToMinimum();
                m_containerTableController.setGradationAssistanceReminderToMinimum();
            }
            else
            { // Means it is from a reminder
                m_containerRagController.hideAssistanceReminder(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                    m_containerRagController.showAssistanceStimulateLevel2(MouseUtilities.getEventHandlerEmpty());


                    m_containerTableController.hideAssistanceReminder(new EventHandler(delegate (System.Object ooo, EventArgs eee)
                    {
                        m_timer.stopTimer();

                        m_containerTableController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                        m_containerTableController.showAssistanceStimulateLevel1(new EventHandler(delegate (System.Object oo, EventArgs ee)
                        { // Opening the cube
                            m_containerTableController.assistanceStimulateLevel1OpeningCube(MouseUtilities.getEventHandlerEmpty());
                        }));

                        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Showing assistance reminder on table surface");
                    }));
                }));

                
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceCueing)
        {
            if ( m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel2)
            {
                m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());
                m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());

                m_containerRagController.hideAssistanceStimulateLevel2(/* .setHelpButtonAnimateDisappear(*/new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                    m_containerRagController.showAssistanceCueing(new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {
                        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cueing window should be displayed now");
                    }));
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                m_containerRagController.hideAssistanceReminder(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerRagController.showAssistanceCueing(MouseUtilities.getEventHandlerEmpty());
                    m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                }));
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The new state does not seem to be managed");
            }

        }
        else if (m_stateCurrent == ChallengeCleanTableStates.Challenge)
        {
            if ( m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1)
            {
                m_containerTableController.hideAssistanceStimulateLevel1(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerTableController.showInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                }));

                m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel2)
            {
                m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());
                m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());

                m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                m_containerRagController.hideAssistanceStimulateLevel2(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerTableController.showInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceCueing)
            {
                m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                m_containerRagController.hideAssistanceCueing(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerTableController.showInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceSolution)
            {
                m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                m_containerRagController.hideAssistanceSolution(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerTableController.showInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                m_containerTableController.showInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
                m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The new state does not seem to be managed");
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceReminder)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance: hiding stimulate assistance level 1");

                // Make the cube diseappearing
                m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());

                m_containerTableController.setGradationAssistanceStimulateLevel1ToMinimum();
                m_containerTableController.setGradationAssistanceReminderToMinimum();
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel2)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance: hiding stimulate assistance level 2");

                m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());

                m_containerRagController.hideAssistanceStimulateLevel2(MouseUtilities.getEventHandlerEmpty());
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceCueing)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance: hiding cueing assistance");

                m_containerRagController.hideAssistanceCueing(MouseUtilities.getEventHandlerEmpty());
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceSolution)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance: hiding assistance solution");

                m_containerRagController.hideAssistanceSolution(MouseUtilities.getEventHandlerEmpty());
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceSolution)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceCueing)
            {
                m_containerRagController.hideAssistanceCueing(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                    m_containerRagController.showAssistanceSolution(MouseUtilities.getEventHandlerEmpty());
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                m_containerRagController.hideAssistanceReminder(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerRagController.showAssistanceSolution(MouseUtilities.getEventHandlerEmpty());
                    m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                }));
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The new state does not seem to be managed");
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.Success)
        {
            //m_hologramInteractionSurfaceTable.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>().resetCubesStates();
            m_containerTableView.GetComponent<MouseTable>().hideInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());

            // Play a sound and display and hologram to inform the user that the challenge is completed
            m_containerTableController.showAssistanceChallengeSuccess(MouseUtilities.getEventHandlerEmpty());
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.StandBy)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            { // Means the "ok" button of the reminder assistance has just been pressed, so we have to hide this window.
                m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Table assistance reminder should be hidden now - greeting from standby mode"));
                m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Rag assistance reminder should be hidden now - greeting from standby mode"));
                m_surfaceTableTouched = false; // Giving the possibility for the user to touch the surface again
                m_surfaceRagTouched = true;

                m_timer.stopTimer();
                m_containerTableController.setGradationAssistanceStimulateLevel1ToMinimum();
                m_containerTableController.setAssistanceReminderToOriginalPosition();
                m_containerTableController.setAssistanceStimulateLevel1ToOriginalPosition();
                m_containerTableController.setGradationAssistanceReminderToMinimum();
            }
            else if (m_statePrevious == ChallengeCleanTableStates.Success)
            {
                m_containerTableController.hideAssistanceChallengeSuccess(new EventHandler(delegate (object o, EventArgs e)
                {
                    m_surfaceTableTouched = false; // Giving the possibility for the user to touch the surface again
                    m_surfaceRagTouched = true;
                    //m_containerTableController.

                    //resetChallenge();
                    m_timer.stopTimer();
                    m_containerTableController.setGradationAssistanceStimulateLevel1ToMinimum();
                    m_containerTableController.setGradationAssistanceReminderToMinimum();
                }));
                
            }
            else
            { // Otherwise means interrruption by the user
                resetChallenge();
            }
        }
    }
}
