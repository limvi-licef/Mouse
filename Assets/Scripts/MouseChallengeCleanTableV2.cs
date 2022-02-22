using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;

// Todo: Make this class more generic, i.e. by using the normal of the surface, and rotating the cube to use to populate the surface by the normal.
// This class relies on the "Interactable MRTK component to catch a touch event. Other classes implement the interface, as they need the details of the event. Interactable does not provide such details, but is easier to implement. Here we do not need those information, so that is why we use this Interactable component.
public class MouseChallengeCleanTableV2 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;
    
    public GameObject m_hologramToDisplayOnFinished;
    //Transform m_hologramAssistanceStimulateLevel1;
    public GameObject m_hologramAssistanceCueing;
    public GameObject m_hologramAssistanceSolution;
    //public GameObject m_hologramAssistanceReminder;
    //public GameObject m_hologramInteractionSurface;
    
    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    Transform m_containerTableView;
    MouseTable m_containerTableController;
    Transform m_containerRagView;
    MouseRag m_containerRagController;

    bool m_surfaceTableTouched; // Bool to detect the touch trigerring the challenge only once.
    bool m_surfaceRagTouched;

    System.Timers.Timer m_timerCaptureAttention;
    
    enum ChallengeCleanTableStates
    {
        StandBy = 0,
        AssistanceStimulateLevel1Gradation1 = 1,
        AssistanceStimulateLevel1Gradation2 = 2,
        AssistanceStimulateLevel1Gradation3 = 3,
        AssistanceStimulateLevel2 = 4,
        AssistanceCueing = 5,
        AssistanceSolution = 6,
        AssistanceReminder = 7,
        Challenge = 8,
        Success = 9
    }

    ChallengeCleanTableStates m_stateCurrent;
    ChallengeCleanTableStates m_statePrevious;

    //public int m_timerAssistanceStimulateAbstract = 2; // in Seconds
    //bool m_timerAssistanceStimulateAbstractStart;
    //int m_timerAssistanceStimulateAbstractCountdown;

    Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_surfaceTableTouched = false;
        m_surfaceRagTouched = true; // True by default, as we do not want the table to be populated if the user grabs the rag without having the challenge starting first. Indeed, in this situation, that means the users does not need any assistance.

        m_stateCurrent = ChallengeCleanTableStates.StandBy;
        m_statePrevious = ChallengeCleanTableStates.StandBy;

        //m_timerAssistanceStimulateAbstractStart = false;
        //m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;

        // Children
        m_containerRagView = gameObject.transform.Find("Rag");
        m_containerTableView = gameObject.transform.Find("Table");
        //m_hologramAssistanceStimulateLevel1 = m_containerTableView.Find("MouseChallengeCleanTableAssistanceStimulateLevel1");

        // Sanity checks


        // Callbacks
        m_containerTableController = m_containerTableView.GetComponent<MouseTable>();
        m_containerTableController.m_eventInteractionSurfaceTableTouched += callbackTableTouched;
        m_containerTableController.m_eventInteractionSurfaceCleaned += callbackSurfaceCleaned;

        //m_hologramAssistanceReminder.GetComponent<MouseChallengeCleanTableReminder>().m_eventHologramClockTouched += callbackHologramAssistanceReminderTouched;
        m_containerTableController.m_eventReminderClockTouched += callbackHologramAssistanceReminderTouched;
        m_containerTableController.m_eventReminderOkTouched += callbackHologramAssistanceReminderOk;
        m_containerTableController.m_eventReminderBackTouched += callbackHologramAssistanceReminderBack;
        //m_hologramAssistanceReminder.GetComponent<MouseChallengeCleanTableReminder>().m_eventHologramWindowButtonOkTouched += callbackHologramAssistanceReminderOk;
        //m_hologramAssistanceReminder.GetComponent<MouseChallengeCleanTableReminder>().m_eventHologramWindowButtonBackTouched += callbackHologramAssistanceReminderBack;

        //MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_hologramAssistanceStimulateLevel1, callbackHologramAssistanceStimulateLevel1Help);
        m_containerTableController.m_eventAssistanceStimulateLevel1Touched += callbackHologramAssistanceStimulateLevel1Help;

        m_containerRagController = m_containerRagView.GetComponent<MouseRag>();
        m_containerRagController.m_eventHologramHelpButtonTouched += callbackHologramAssistanceStimulateLevel2Help;
        m_containerRagController.m_eventHologramInteractionSurfaceTouched += callbackAssistanceRabGrabbed;
        m_containerRagController.m_eventHologramHelpButtonCueingTouched += callbackHologramAssistanceCueingHelp;
        m_containerRagController.m_eventHologramReminderClockTouched += callbackHologramAssistanceReminderTouched;
        m_containerRagController.m_eventHologramReminderOkTouched += callbackHologramAssistanceReminderOk;
        m_containerRagController.m_eventHologramReminderBackTouched += callbackHologramAssistanceReminderBack;
    }

    void callbackHologramAssistanceStimulateLevel1Help(System.Object o, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.AssistanceStimulateLevel2);

        /*m_hologramAssistanceStimulateLevel1.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
       {
           updateChallenge(ChallengeCleanTableStates.AssistanceStimulateLevel2);

           m_hologramAssistanceStimulateLevel1.gameObject.SetActive(false);
           m_hologramAssistanceStimulateLevel1.localScale = new Vector3(0.2f, 0.2f, 0.2f);
       }));*/


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

    /*
     * Will have to be called when the person is doing a sign with the hand to refuse the challenge AND is focusing on the object.
     * 
     * */
    void callbackAssistanceRefuse(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackInviteChallengeHologramNok", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        /*m_hologramAssistanceStimulateAbstract.SetActive(false);
        m_surfaceTouched = false; // Reset challenge status, so that if the person touche the table surface again, it will restart it.*/

        updateChallenge(ChallengeCleanTableStates.StandBy);
    }

    /*void callbackHologramAssistanceStimulateAbstractReminder(object sender, EventArgs e)
    {

        updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
    }*/

    /*public void callbackHologramAssistanceStimulateAbstractTimerGradationCaptureAttention(System.Object source, ElapsedEventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateTimerGradationCaptureAttention", MouseDebugMessagesManager.MessageLevel.Info, "Timer called");

        m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().increaseAssistanceGradation();
    }*/

    public void callbackHologramAssistanceSimulateLevel1FocusOn()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractFocusOn", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Start timer to determine when going to the next VISUAL capture attention gradation
        //startTimerAssistanceStimulateAbstract();
    }

    public void callbackHologramAssistanceStimulateLevel1FocusOff()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractFocusOff", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Stop timer as the person does not look at the hologram anymore ...
        //stopTimerAssistanceStimulateAbstract();

        // ... so maybe trigger an audio gradation? 

    }

    /*public void callbackHologramAssistanceStimulateAbstractIsMoved()
    { // If the person interact with the object, then no need to continue to have the timer running
        stopTimerAssistanceStimulateAbstract();
    }*/

    /*public void callbackHologramAssistanceCueingOk()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceCueing.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearToPosition(transform.TransformPoint(new Vector3(0, 0, 0)), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackHologramAssistanceStimulateOk", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
            m_hologramAssistanceCueing.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.Challenge); 
        }));
    }*/

    /*public void callbackHologramAssistanceCueingNok()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceCueing.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateNok", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
            m_hologramAssistanceCueing.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.StandBy);
        }));
    }*/

    public void callbackHologramAssistanceReminderTouched(System.Object o, EventArgs e)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        /*MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateLater", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
            m_hologramAssistanceStimulate.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
        }));*/

        updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
    }

    public void callbackHologramAssistanceStimulateLevel2Help(System.Object o, EventArgs e)
    {
        /*MouseUtilitiesAnimation animator = m_hologramAssistanceStimulateLevel2.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateHelp", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceStimulate.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.AssistanceSolution);
        }));*/


        updateChallenge(ChallengeCleanTableStates.AssistanceCueing);

    }

    /*public void callbackHologramAssistanceSolutionOk()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearToPosition(transform.TransformPoint(new Vector3(0f, 0f, 0f)), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceSolutionOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceSolution.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.Challenge);
        }));
    }*/

    /*public void callbackHologramAssistanceSolutionNok()
    {       
        MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceSolutionNok", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceSolution.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.StandBy);
        }));
    }*/

    /*public void callbackHologramAssistanceSolutionLater()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceSolutionLater", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceSolution.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
        }));
    }*/

    public void callbackHologramAssistanceReminderOk(System.Object o, EventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.StandBy);

        /*MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceReminder.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.StandBy);
        }));*/
    }

    public void callbackHologramAssistanceReminderBack(System.Object o, EventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderBack", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        /*if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation1)
        {
            updateChallenge(m_statePrevious);
        }
        else if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel2)
        {
            updateChallenge(m_statePrevious);
        }*/

        updateChallenge(m_statePrevious);

        /*MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();

        if ( m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateAbstract)
        {
            animator.animateDiseappearToPosition(m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().m_hologramReminder.getPositionWorld(), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderBack", MouseDebugMessagesManager.MessageLevel.Info, "Called");

                m_hologramAssistanceReminder.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

                updateChallenge(m_statePrevious);
            }));
        }
        else
        {
            animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderBack", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
                m_hologramAssistanceReminder.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);

                updateChallenge(m_statePrevious); // We want to go to the previous state, so we use the previous state as input parameter
            }));
        }*/
    }

    // TODO: Delegate this to a dedicated script hosted by the Table gameobject?
    /*public void callbackHologramInteractionSurfaceMovedFinished()
    {
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Bring specific components to the center of the interaction surface

        //gameObject.transform.position = m_hologramInteractionSurface.transform.position;
        m_containerTable.transform.position = m_hologramInteractionSurfaceTable.transform.position;
        m_hologramInteractionSurfaceTable.transform.localPosition = new Vector3(0, 0f, 0);
    }*/

    // TODO: Delegate this to a dedicated script hosted by the Rag gameobject?
    /*public void callbackHologramRagInteractionSurfaceMovedFinished()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_containerRag.transform.position = m_hologramInteractionSurfaceRag.transform.position;
        m_hologramInteractionSurfaceRag.transform.localPosition = new Vector3(0, 0f, 0);
    }*/

        void callbackHologramAssistanceCueingHelp(System.Object o, EventArgs e)
    {
        updateChallenge(ChallengeCleanTableStates.AssistanceSolution);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (m_timerAssistanceStimulateAbstractStart)
        {
            m_timerAssistanceStimulateAbstractCountdown -= 1;

            if (m_timerAssistanceStimulateAbstractCountdown > 0)
            {
                if ((m_timerAssistanceStimulateAbstract * 60) % m_timerAssistanceStimulateAbstractCountdown == 0)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Timer: seconds remaining: " + (m_timerAssistanceStimulateAbstractCountdown / 60).ToString());
                }
            }
            else
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Timer finished!");
               m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().increaseAssistanceGradation();

                m_timerAssistanceStimulateAbstractStart = false;
                m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;
            }
        }*/
    }

    public void callbackTableTouched(System.Object o, EventArgs e)
    {
        if (m_surfaceTableTouched == false)
        { // This can be called only if the surface can be touched
            updateChallenge(ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation1);
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

        m_containerTableView.GetComponent<MouseTable>().hideInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());

        m_hologramToDisplayOnFinished.SetActive(false);
        m_surfaceTableTouched = false;
        //m_hologramAssistanceStimulateLevel1.gameObject.SetActive(false);
        //if (m_containerTableController.isAssistanceStimulateLevel1Active())
        //{
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Hiding assistance stimulate level 1");
        m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());
        //}
            
        //if (m_containerTableController.isAssistanceReminderActive())
        //{
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Hiding reminder assistance");
        m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
        //}
        //m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerEmpty());

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

        if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation1)
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
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Surface touched but process disabled");
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation2)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The new state does not seem to be managed");
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulateLevel2)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation1)
            {
                // Display the help and line components
                m_containerTableController.hideAssistanceStimulateLevel1(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerRagController.showAssistanceStimulateLevel2(MouseUtilities.getEventHandlerEmpty());//.setHelpAndLineAnimateAppear();

                    //Hide the clock component and make it appearing close to this new help component
                    m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Assistance reminder hidden - greetings from AssistanceStimulateLevel2 (previous state AssistanceStimulateLevel1Gradation1)"));
                    m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                }));
            }
            else
            {
                m_containerRagController.hideAssistanceReminder(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                    //m_containerRagController.setHelpAndLineAnimateAppear();
                    m_containerRagController.showAssistanceStimulateLevel2(MouseUtilities.getEventHandlerEmpty());
                }));
                
            }
            
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceCueing)
        {
            if ( m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel2)
            {
                m_containerRagController.hideAssistanceStimulateLevel2(/* .setHelpButtonAnimateDisappear(*/new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                    m_containerRagController.showAssistanceCueing(new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {
                        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cueing window should be displayed now");
                    }));
                }));
                //m_containerRagController.hideAssistanceStimulateLevel2();
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                m_containerRagController.hideAssistanceReminder(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_containerRagController.showAssistanceCueing(MouseUtilities.getEventHandlerEmpty());
                    m_containerRagController.showAssistanceReminder(MouseUtilities.getEventHandlerEmpty());
                    //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Calle" +
                        }));
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The new state does not seem to be managed");
            }

        }
        else if (m_stateCurrent == ChallengeCleanTableStates.Challenge)
        {
            if ( m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation1 ||
                m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation2 ||
                m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation3)
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

            

            // TODO: hide all potential assistances that might still be displayed.
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.StandBy)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            { // Means the "ok" button of the reminder assistance has just been pressed, so we have to hide this window.
                m_containerTableController.hideAssistanceReminder(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Table assistance reminder should be hidden now - greeting from standby mode"));
                m_containerRagController.hideAssistanceReminder(MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Rag assistance reminder should be hidden now - greeting from standby mode"));
                m_surfaceTableTouched = false; // Giving the possibility for the user to touch the surface again
                m_surfaceRagTouched = true;
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceReminder)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation1 || m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel1Gradation2)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance: hiding stimulate assistance level 1");

                // Make the cube diseappearing
                m_containerTableController.hideAssistanceStimulateLevel1(MouseUtilities.getEventHandlerEmpty());
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateLevel2)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance: hiding stimulate assistance level 2");

                m_containerRagController.hideAssistanceStimulateLevel2(/* .setHelpButtonAnimateDisappear(*/MouseUtilities.getEventHandlerEmpty());
                //m_containerRagController.hideAssistanceStimulateLevel2();
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

            /*if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulate || m_statePrevious == ChallengeCleanTableStates.AssistanceSolution)
            {
                MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                }));

                //m_hologramAssistanceStimulate.SetActive(false);
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateAbstract)
            {
                m_hologramAssistanceStimulateAbstract.SetActive(false);

                MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearFromPosition(m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().m_hologramReminder.getPositionWorld(), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                    //m_hologramAssistanceStimulate.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);
                }));
            }
            //m_hologramAssistanceReminder.SetActive(true);
            m_messageReminderFocused = true;
            updateMessageReminderMenu();*/
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceSolution)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceCueing)
            {
                m_containerRagController.hideAssistanceCueing(new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                    m_containerRagController.showAssistanceSolution(new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {

                        //m_containerRagController.displayAssistanceSolution()
                    }));
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

            /*MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();
            animator.animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            }));*/
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.Success)
        {
            //m_hologramInteractionSurfaceTable.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>().resetCubesStates();
            m_containerTableView.GetComponent<MouseTable>().hideInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());

        // Play a sound and display and hologram to inform the user that the challenge is completed
        m_hologramToDisplayOnFinished.SetActive(true);
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
        }
    }

    /*void startTimerAssistanceStimulateAbstract ()
    {
        m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;
        m_timerAssistanceStimulateAbstractStart = true;
    }*/

    /*void stopTimerAssistanceStimulateAbstract ()
    {
        m_timerAssistanceStimulateAbstractStart = false;
        m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;
    }*/

    // Utilities functions: to be moved to a dedicated namespace later?
   /* void utilitiesAddTouchCallback(Transform transform, UnityEngine.Events.UnityAction callback)
    {
        GameObject gameObject = transform.gameObject;

        Interactable interactable = gameObject.GetComponent<Interactable>();

        if (interactable == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No interactable component to the gameobject: adding one");

            interactable = gameObject.AddComponent<Interactable>();
        }

        InteractableOnTouchReceiver receiver = interactable.GetReceiver<InteractableOnTouchReceiver>();

        if ( receiver == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No touch receiver to the interactable gameobject: adding one");

            receiver = interactable.AddReceiver<InteractableOnTouchReceiver>();
        }

        receiver.OnTouchStart.AddListener(callback);
    }*/
}
