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

// Todo: Make this class more generic, i.e. by using the normal of the surface, and rotating the cube to use to populate the surface by the normal.
// This class relies on the "Interactable MRTK component to catch a touch event. Other classes implement the interface, as they need the details of the event. Interactable does not provide such details, but is easier to implement. Here we do not need those information, so that is why we use this Interactable component.
// Old version of the clean table challenge. Will most likely diseappear one of these days.
public class MouseChallengeCleanTableV1 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;
    public GameObject m_hologramToUseToPopulateSurface;
    public GameObject m_hologramToDisplayOnFinished;
    public GameObject m_hologramAssistanceStimulateAbstract;
    public GameObject m_hologramAssistanceStimulate;
    public GameObject m_hologramAssistanceSolution;
    public GameObject m_hologramAssistanceReminder;
    public GameObject m_hologramInteractionSurface;
    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    bool m_surfaceTouched; // Bool to detect the touch trigerring the challenge only once.

    bool m_palmOpened;
    bool m_messageDetailedFocused;
    bool m_messageReminderFocused;
    //bool m_startAnimationDisplayMessageGradationLevel1;

    System.Timers.Timer m_timerCaptureAttention;
    
    Dictionary<Tuple<float, float>, Tuple<GameObject, bool>> m_cubesTouched;
    
    enum ChallengeCleanTableStates
    {
        AssistanceStimulateAbstract = 0,
        AssistanceStimulate = 1,
        AssistanceSolution = 2,
        AssistanceReminder = 3,
        Challenge = 4,
        StandBy = 5
    }

    ChallengeCleanTableStates m_stateCurrent;
    ChallengeCleanTableStates m_statePrevious;

    public int m_timerAssistanceStimulateAbstract = 2; // in Seconds
    bool m_timerAssistanceStimulateAbstractStart;
    int m_timerAssistanceStimulateAbstractCountdown;

    Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        m_surfaceTouched = false;
        m_palmOpened = false;
        m_messageDetailedFocused = false;
        m_cubesTouched = new Dictionary<Tuple<float, float>, Tuple<GameObject, bool>>();
        m_messageReminderFocused = false;
        //m_startAnimationDisplayMessageGradationLevel1 = false;

        m_stateCurrent = ChallengeCleanTableStates.StandBy;
        m_statePrevious = ChallengeCleanTableStates.StandBy;

        m_timerAssistanceStimulateAbstractStart = false;
        m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;

        // Sanity checks
        if (m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>() == null)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "Start", MouseDebugMessagesManager.MessageLevel.Error, "The invite challenge hologram must have the MouseUtilitiesHologramInteractionSwipes component. The object will most likely crash because the implementation is not that safe.");
        }
        if (m_hologramAssistanceStimulateAbstract.GetComponent<Interactable>() == null)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "Start", MouseDebugMessagesManager.MessageLevel.Error, "The invite challenge hologram must have the Interactable component. The object will most likely crash because the implementation is not that safe.");
        }
        if (m_hologramAssistanceSolution.GetComponent<MouseChallengeCleanTableDetailsChallenge>() == null)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "Start", MouseDebugMessagesManager.MessageLevel.Error, "This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge");
        }

        // Connect all the callbacks
        m_hologramInteractionSurface.GetComponent<TapToPlace>().OnPlacingStopped.AddListener(callbackOnTapToPlaceFinished);
        m_hologramInteractionSurface.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackOnTapToPlaceFinished); // Use the same callback than for taptoplace as the process to do is the same
        m_hologramInteractionSurface.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackTableTouched);

        MouseUtilitiesHologramInteractionSwipes cubeInteractions = m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>();
        cubeInteractions.m_utilitiesInteractionSwipesEventHelp += callbackHologramAssistanceStimulateAbstractHelp; // Subsribe to the event so that the object is aware with the invite challenge hologram has been touched
        cubeInteractions.m_utilitiesInteractionSwipesEventOk += callbackHologramAssistanceStimulateAbstractOk;
        cubeInteractions.m_utilitiesInteractionSwipesEventNok += callbackHologramAssistanceStimulateAbstractNok;
        cubeInteractions.m_utilitiesInteractionSwipesEventReminder += callbackHologramAssistanceStimulateAbstractReminder;

    }

    void cubeTouched(object sender, EventArgs e)
    {
        MouseChallengeCleanTableHologramForSurfaceToClean tempCube = (MouseChallengeCleanTableHologramForSurfaceToClean)sender;

        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "cubeTouched", MouseDebugMessagesManager.MessageLevel.Info, "Cube touched. Position: " + tempCube.transform.localPosition.x.ToString() + " " + tempCube.transform.localPosition.z.ToString());

        Tuple<float, float> tempTuple = new Tuple<float, float>(tempCube.transform.localPosition.x, tempCube.transform.localPosition.z);

        m_cubesTouched[tempTuple] = new Tuple<GameObject, bool>(m_cubesTouched[tempTuple].Item1, true);

        checkIfSurfaceClean();
    }

    void callbackHologramAssistanceStimulateAbstractHelp(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        updateChallenge(ChallengeCleanTableStates.AssistanceStimulate);
    }

    void callbackHologramAssistanceStimulateAbstractOk(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackInviteChallengeHologramOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.Challenge);
    }

    void callbackHologramAssistanceStimulateAbstractNok(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackInviteChallengeHologramNok", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.StandBy);
    }

    void callbackHologramAssistanceStimulateAbstractReminder(object sender, EventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractReminder", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
    }

    public void callbackHologramAssistanceStimulateAbstractTimerGradationCaptureAttention(System.Object source, ElapsedEventArgs e)
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateTimerGradationCaptureAttention", MouseDebugMessagesManager.MessageLevel.Info, "Timer called");

        m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().increaseAssistanceGradation();
    }

    public void callbackHologramAssistanceStimulateAbstractFocusOn()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractFocusOn", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Start timer to determine when going to the next VISUAL capture attention gradation
        startTimerAssistanceStimulateAbstract();
    }

    public void callbackHologramAssistanceStimulateAbstractFocusOff()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateAbstractFocusOff", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Stop timer as the person does not look at the hologram anymore ...
        stopTimerAssistanceStimulateAbstract();

        // ... so maybe trigger an audio gradation? 

    }

    public void callbackHologramAssistanceStimulateAbstractIsMoved()
    { // If the person interact with the object, then no need to continue to have the timer running
        stopTimerAssistanceStimulateAbstract();
    }

    public void callbackHologramAssistanceStimulateOk()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearToPosition(transform.TransformPoint(new Vector3(0, 0, 0)), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackHologramAssistanceStimulateOk", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
            m_hologramAssistanceStimulate.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.Challenge); 
        }));
    }

    public void callbackHologramAssistanceStimulateNok()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateNok", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
            m_hologramAssistanceStimulate.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.StandBy);
        }));
    }

    public void callbackHologramAssistanceStimulateLater()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateLater", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
            m_hologramAssistanceStimulate.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
        }));
    }

    public void callbackHologramAssistanceStimulateHelp()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceStimulateHelp", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceStimulate.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.AssistanceSolution);
        }));
    }

    public void callbackHologramAssistanceSolutionOk()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearToPosition(transform.TransformPoint(new Vector3(0f, 0f, 0f)), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceSolutionOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceSolution.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.Challenge);
        }));
    }

    public void callbackHologramAssistanceSolutionNok()
    {       
        MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceSolutionNok", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceSolution.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.StandBy);
        }));
    }

    public void callbackHologramAssistanceSolutionLater()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceSolutionLater", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceSolution.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.AssistanceReminder);
        }));
    }

    public void callbackHologramAssistanceReminderOk()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();

        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramAssistanceReminder.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

            updateChallenge(ChallengeCleanTableStates.StandBy);
        }));
    }

    public void callbackHologramAssistanceReminderBack()
    {
        MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();

        if ( m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateAbstract)
        {
            animator.animateDiseappearToPosition(m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().m_hologramReminder.getPositionWorld(), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderBack", MouseDebugMessagesManager.MessageLevel.Info, "Called");

                m_hologramAssistanceReminder.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

                updateChallenge(m_statePrevious);
            }));
        }
        else
        {
            animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "callbackHologramAssistanceReminderBack", MouseDebugMessagesManager.MessageLevel.Info, "Animation for reminder window finished");
                m_hologramAssistanceReminder.GetComponent<MouseUtilitiesHolograms>().resetHologram(m_positionLocalReferenceForHolograms);

                updateChallenge(m_statePrevious); // We want to go to the previous state, so we use the previous state as input parameter
            }));
        }
    }

    public void callbackHandPalmFacingUser()
    {
        m_palmOpened = true;
        if (m_messageDetailedFocused)
        {
            updateMessageDetailedMenu();
        }
        else if (m_messageReminderFocused)
        {
            updateMessageReminderMenu();
        }
    }

    public void callbackHandPalmNotFacinUserAnymore()
    {
        m_palmOpened = false;
        if (m_messageDetailedFocused)
        {
            updateMessageDetailedMenu();
        }
        else if (m_messageReminderFocused)
        {
            updateMessageReminderMenu();
        }
    }

    public void callbackMessageDetailingChallengeOnFocus()
    {
        m_messageDetailedFocused = true;
        updateMessageDetailedMenu();
    }

    public void callbackMessageDetailingChallengeOnLosingFocus()
    {

    }

    public void callbackReminderWindowOnFocus()
    {
        m_messageReminderFocused = true;
        updateMessageReminderMenu();
    }

    public void callbackOnTapToPlaceFinished()
    {
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        gameObject.transform.position = m_hologramInteractionSurface.transform.position;
        m_hologramInteractionSurface.transform.localPosition = new Vector3(0, 0f, 0);
    }

    // This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge
    void updateMessageDetailedMenu ()
    {
        MouseChallengeCleanTableDetailsChallenge cleanTableDetailsChallenge = m_hologramAssistanceSolution.GetComponent<MouseChallengeCleanTableDetailsChallenge>();

        if (cleanTableDetailsChallenge == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageDetailedMenu", MouseDebugMessagesManager.MessageLevel.Warning, "This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge");
            return;
        }
        if (m_messageDetailedFocused && m_palmOpened)
        {
            cleanTableDetailsChallenge.displayMenus(false, true);
        }
        else if (m_messageDetailedFocused && m_palmOpened == false)
        {
            cleanTableDetailsChallenge.displayMenus(true, false);
        }
        else if ( m_messageDetailedFocused == false)
        {
            cleanTableDetailsChallenge.displayMenus(false, false);
        }
    }

    void updateMessageReminderMenu()
    {
        MouseChallengeCleanTableDetailsChallenge cleanTableDetailsChallenge = m_hologramAssistanceReminder.GetComponent<MouseChallengeCleanTableDetailsChallenge>();

        if (cleanTableDetailsChallenge == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageReminderMenu", MouseDebugMessagesManager.MessageLevel.Warning, "This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge");
            return;
        }

        if (m_messageReminderFocused && m_palmOpened)
        {
            cleanTableDetailsChallenge.displayMenus(false, true);
        }
        else if (m_messageReminderFocused && m_palmOpened == false)
        {
            cleanTableDetailsChallenge.displayMenus(true, false);
        }
        else if (m_messageReminderFocused == false)
        {
            cleanTableDetailsChallenge.displayMenus(false, false);
        }
    }

    public void populateTablePanel()
    {
        if (m_hologramToUseToPopulateSurface.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>() == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Error, "The GameObject used to populate the surface should contain the component: MouseChallengeCubeInteractions");
        }
        else
        {
            Vector3 goLocalPosition = gameObject.transform.localPosition;

            float goScaleX = 1.0f;
            float goScaleZ = 1.0f;

            float posX = 0.0f;
            float posZ = 0.0f;

            float incrementX = goScaleX / m_numberOfCubesToAddInColumn;
            float incrementZ = goScaleZ / m_numberOfCubesToAddInRow;

            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Table panel position x=" + gameObject.transform.position.x.ToString() + " z=" + gameObject.transform.position.z.ToString() + " increment: x = " + incrementX.ToString() + " z = " + incrementZ.ToString());

            for (posX = 0.0f; posX < goScaleX; posX += incrementX)
            {
                for (posZ = 0.0f; posZ < goScaleZ; posZ += incrementZ)
                {
                    GameObject temp = Instantiate(m_hologramToUseToPopulateSurface);
                    temp.transform.SetParent(m_hologramInteractionSurface.transform, false);
                    temp.transform.localPosition = Vector3.zero;
                    temp.transform.localScale = new Vector3(incrementX, 0.01f, incrementZ);
                    float posXP = posX /*- incrementX / 2.0f*/ - goScaleX/2.0f + temp.transform.localScale.x/2.0f;
                    float posZP = posZ /*- incrementZ / 2.0f*/ - goScaleZ/2.0f + temp.transform.localScale.z / 2.0f;

                    m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Position of the cube in x=" + posXP.ToString() + " z=" + posZP.ToString() + " | Size of the cube: x= " + temp.transform.localScale.x.ToString() +" z=" + temp.transform.localScale.z.ToString() );

                    temp.transform.localPosition = new Vector3(posXP, goLocalPosition.y + 3.0f, posZP);

                    MouseChallengeCleanTableHologramForSurfaceToClean cubeInteractions = temp.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>();
                    cubeInteractions.CubeTouchedEvent += cubeTouched;
                    m_cubesTouched.Add(new Tuple<float,float>(posXP, posZP), new Tuple<GameObject, bool>(temp, false));
                    temp.SetActive(true); // Hidden by default
                }
            }   
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timerAssistanceStimulateAbstractStart)
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
        }
    }

    public void callbackTableTouched()
    {
        m_statePrevious = m_stateCurrent;
        m_stateCurrent = ChallengeCleanTableStates.AssistanceStimulateAbstract;

        updateChallenge(ChallengeCleanTableStates.AssistanceStimulateAbstract);
     }

    void checkIfSurfaceClean()
    {
        bool allCubesTouched = true; // By default weconsidered all cubes are touched. then we browse the values of the dictionary, and if we find a cube that has not been touched, then we set this boolean to false.

        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject,bool>> tempKeyValue in m_cubesTouched)
        {
            if (tempKeyValue.Value.Item2 == false)
            {
                allCubesTouched = false;
            }
        }

        if (allCubesTouched)
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "checkIfSurfaceClean", MouseDebugMessagesManager.MessageLevel.Info, "All cubes touched !!!!");
            closeChallenge();
            
        }
        else
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "checkIfSurfaceClean", MouseDebugMessagesManager.MessageLevel.Info, "Still some work to do ...");
        }
    }

    void closeChallenge()
    {
        // Here we will remove the cubes from the table and display an hologram in the center of the table
        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> tempKeyValue in m_cubesTouched)
        {
            tempKeyValue.Value.Item1.SetActive(false);
        }

        // Play a sound and display and hologram to inform the user that the challenge is completed
        m_hologramToDisplayOnFinished.SetActive(true);
        m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
    }

    public void resetChallenge()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "resetChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Here we will remove the cubes from the table and display an hologram in the center of the table
        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> tempKeyValue in m_cubesTouched)
        {
            Destroy(tempKeyValue.Value.Item1);
        }

        m_cubesTouched.Clear();

        m_hologramToDisplayOnFinished.SetActive(false);
        m_surfaceTouched = false;
        m_hologramAssistanceStimulateAbstract.SetActive(false);
        m_hologramAssistanceReminder.SetActive(false);

        updateChallenge(ChallengeCleanTableStates.StandBy);

    }

    void updateChallenge(ChallengeCleanTableStates newState)
    {
        m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Current state: " + m_stateCurrent.ToString() + "New state: " + newState.ToString());

        m_statePrevious = m_stateCurrent;
        m_stateCurrent = newState;

        if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulateAbstract)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                MouseUtilitiesAnimation animator = m_hologramAssistanceStimulateAbstract.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearInPlaceToScaling(new Vector3(0.3f, 0.3f, 0.3f), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                }));
            }
            else if (m_surfaceTouched == false) // Surface can be touched only once
            {
                m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Called");

                MouseUtilitiesAnimation animator = m_hologramAssistanceStimulateAbstract.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearInPlaceToScaling(new Vector3(0.3f, 0.3f, 0.3f), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                }));

                m_surfaceTouched = true;

                // Play sound to get the user's attention from audio on top of visually
                m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
            }
            else
            {
                m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Surface touched but process disabled");
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.Challenge)
        {
            // First we not go in for subtleties, i.e. we just hide all assistance windows without looking which one is currently displayed.

            if (m_statePrevious == ChallengeCleanTableStates.AssistanceSolution)
            {
                m_messageDetailedFocused = false;
                updateMessageDetailedMenu();
            }

            populateTablePanel();
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.StandBy)
        {
            // Here as well, we not go in for subtleties, i.e. we just hide all assistance windows without looking which one is currently displayed.
            m_hologramAssistanceStimulateAbstract.SetActive(false);
            m_hologramAssistanceStimulate.SetActive(false);
            m_hologramAssistanceSolution.SetActive(false);

            if (m_statePrevious == ChallengeCleanTableStates.AssistanceSolution)
            {
                m_messageDetailedFocused = false;
                updateMessageDetailedMenu();
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                m_hologramAssistanceReminder.SetActive(false);
                m_messageReminderFocused = false;
                updateMessageReminderMenu();
            }

            m_surfaceTouched = false; // Reset challenge status, so that if the person touche the table surface again, it will restart it.
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceStimulate)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateAbstract)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "inviteChallengeHologramTouched", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                m_hologramAssistanceStimulateAbstract.SetActive(false);

                // Animate the next window to display
                MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearFromPosition(m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().m_hologramHelp.getPositionWorld(), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                    //m_hologramAssistanceStimulate.GetComponent<MouseUtilities>().resetHologram(m_positionLocalReferenceForHolograms);
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceReminder)
            {
                MouseUtilitiesAnimation animator = m_hologramAssistanceStimulate.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                }));

                m_messageReminderFocused = false;
                updateMessageReminderMenu();
            }
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceReminder)
        {
            if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulate || m_statePrevious == ChallengeCleanTableStates.AssistanceSolution)
            {
                MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                }));
            }
            else if (m_statePrevious == ChallengeCleanTableStates.AssistanceStimulateAbstract)
            {
                m_hologramAssistanceStimulateAbstract.SetActive(false);

                MouseUtilitiesAnimation animator = m_hologramAssistanceReminder.AddComponent<MouseUtilitiesAnimation>();
                animator.animateAppearFromPosition(m_hologramAssistanceStimulateAbstract.GetComponent<MouseUtilitiesHologramInteractionSwipes>().m_hologramReminder.getPositionWorld(), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
                }));
            }
            m_messageReminderFocused = true;
            updateMessageReminderMenu();
        }
        else if (m_stateCurrent == ChallengeCleanTableStates.AssistanceSolution)
        {
            MouseUtilitiesAnimation animator = m_hologramAssistanceSolution.AddComponent<MouseUtilitiesAnimation>();
            animator.animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage("MouseChallengeCleanTable", "updateChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            }));
        }
    }

    void startTimerAssistanceStimulateAbstract ()
    {
        m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;
        m_timerAssistanceStimulateAbstractStart = true;
    }

    void stopTimerAssistanceStimulateAbstract ()
    {
        m_timerAssistanceStimulateAbstractStart = false;
        m_timerAssistanceStimulateAbstractCountdown = m_timerAssistanceStimulateAbstract * 60;
    }
}
