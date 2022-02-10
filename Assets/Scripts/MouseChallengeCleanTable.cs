using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

// Todo: Make this class more generic, i.e. by using the normal of the surface, and rotating the cube to use to populate the surface by the normal.
// This class relies on the "Interactable MRTK component to catch a touch event. Other classes implement the interface, as they need the details of the event. Interactable does not provide such details, but is easier to implement. Here we do not need those information, so that is why we use this Interactable component.
public class MouseChallengeCleanTable : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;
    public GameObject m_hologramToUseToPopulateSurface;
    public GameObject m_hologramToDisplayOnFinished;
    public GameObject m_hologramInviteChallenge;
    public GameObject m_hologramChallengeDetailsToDisplay;
    public GameObject m_hologramChallengeDetailsReminder;
    public GameObject m_hologramInteractionSurface;
    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    bool m_surfaceTouched; // Bool to detect the touch trigerring the challenge only once.

    bool m_palmOpened;
    bool m_messageDetailedFocused;
    bool m_messageReminderFocused;

    Dictionary<Tuple<float, float>, Tuple<GameObject, bool>> m_cubesTouched;

    // Start is called before the first frame update
    void Start()
    {
        m_surfaceTouched = false;
        m_palmOpened = false;
        m_messageDetailedFocused = false;
        m_cubesTouched = new Dictionary<Tuple<float, float>, Tuple<GameObject, bool>>();
        m_messageReminderFocused = false;

        // Connect all the callbacks
        m_hologramInteractionSurface.GetComponent<TapToPlace>().OnPlacingStopped.AddListener(callbackOnTapToPlaceFinished);
        m_hologramInteractionSurface.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackOnTapToPlaceFinished); // Use the same callback than for taptoplace as the process to do is the same
        m_hologramInteractionSurface.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(onTouch);

        // Sanity checks
        if (m_hologramInviteChallenge.GetComponent<MouseChallengeCleanTableInvite>() == null)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "Start", MouseDebugMessagesManager.MessageLevel.Error, "The invite challenge hologram must have the MouseChallengeCubeInteractions component. The object will most likely crash because the implementation is not that safe.");
        }
        if (m_hologramChallengeDetailsToDisplay.GetComponent<MouseChallengeCleanTableDetailsChallenge>() == null)
        {
            m_debug.displayMessage("MouseChallengeCleanTable", "Start", MouseDebugMessagesManager.MessageLevel.Error, "This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge");
        }

    }

    void cubeTouched(object sender, EventArgs e)
    {
        MouseChallengeCleanTableHologramForSurfaceToClean tempCube = (MouseChallengeCleanTableHologramForSurfaceToClean)sender;

        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "cubeTouched", MouseDebugMessagesManager.MessageLevel.Info, "Cube touched. Position: " + tempCube.transform.localPosition.x.ToString() + " " + tempCube.transform.localPosition.z.ToString());

        Tuple<float, float> tempTuple = new Tuple<float, float>(tempCube.transform.localPosition.x, tempCube.transform.localPosition.z);

        m_cubesTouched[tempTuple] = new Tuple<GameObject, bool>(m_cubesTouched[tempTuple].Item1, true);

        checkIfSurfaceClean();
    }

    void callbackInviteChallengeHologramHelp(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage("MouseChallengeCleanTable", "inviteChallengeHologramTouched", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramInviteChallenge.SetActive(false);
        m_hologramChallengeDetailsToDisplay.SetActive(true);
        
        //populateTablePanel();
    }

    void callbackInviteChallengeHologramOk(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackInviteChallengeHologramOk", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramInviteChallenge.SetActive(false);
        //m_hologramChallengeDetailsToDisplay.SetActive(true);

        populateTablePanel();
    }

    void callbackInviteChallengeHologramNok(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        m_debug.displayMessage("MouseChallengeCleanTable", "callbackInviteChallengeHologramNok", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramInviteChallenge.SetActive(false);
        m_surfaceTouched = false; // Reset challenge status, so that if the person touche the table surface again, it will restart it.
    }

    public void callbackForMessageDetailingChallengeOkButton()
    {
        m_debug.displayMessage("MouseHandInteractions", "callbackForMessageDetailingChallengeOkButton", MouseDebugMessagesManager.MessageLevel.Info, "Called (has m_messageDetailedFocused = false)");
        m_hologramChallengeDetailsToDisplay.SetActive(false);
        populateTablePanel();
        m_messageDetailedFocused = false;
        updateMessageDetailedMenu();
    }

    public void callbackForMessageDetailingChallengeNotOkButton()
    {
        m_debug.displayMessage("MouseHandInteractions", "callbackForMessageDetailingChallengeNotOkButton", MouseDebugMessagesManager.MessageLevel.Info, "Called (has m_messageDetailedFocused = false)");
        m_hologramChallengeDetailsToDisplay.SetActive(false);
        m_surfaceTouched = false; // Reset challenge status, so that if the person touche the table surface again, it will restart it.
        m_messageDetailedFocused = false;
        updateMessageDetailedMenu();
    }

    public void callbackForMessageDetailingChallengeMaybeLaterButton()
    {
        m_debug.displayMessage("MouseHandInteractions", "callbackForMessageDetailingChallengeMaybeLaterButton", MouseDebugMessagesManager.MessageLevel.Info, "Called (has m_messageDetailedFocused = false)");
        m_hologramChallengeDetailsToDisplay.SetActive(false);

        //m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackForMessageDetailingChallengeMaybeLaterButton", MouseDebugMessagesManager.MessageLevel.Warning, "TODO: Needs to be implemented");
        m_messageDetailedFocused = false;
        updateMessageDetailedMenu();

        m_hologramChallengeDetailsReminder.SetActive(true);
        m_messageReminderFocused = true;
        updateMessageReminderMenu();
    }

    public void callbackHandPalmFacingUser()
    {
        m_debug.displayMessage("MouseHandInteractions", "callbackHandPalmFacingUser", MouseDebugMessagesManager.MessageLevel.Info, "Hand palm detected");
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
        //m_debug.displayMessage("MouseHandInteractions", "callbackHandPalmFacingUser", MouseDebugMessagesManager.MessageLevel.Info, "Hand palm not detected anymore");
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
        m_debug.displayMessage("MouseHandInteractions", "callbackMessageDetailingChallengeOnFocus", MouseDebugMessagesManager.MessageLevel.Info, "Message focused (has m_messageDetailedFocused = true)");
        m_messageDetailedFocused = true;
        updateMessageDetailedMenu();
    }

    public void callbackMessageDetailingChallengeOnLosingFocus()
    {
        m_debug.displayMessage("MouseHandInteractions", "callbackMessageDetailingChallengeOnLosingFocus", MouseDebugMessagesManager.MessageLevel.Info, "Message not focused anymore");
    }

    public void callbackReminderWindowOnFocus()
    {
        m_debug.displayMessage("MouseHandInteractions", "callbackReminderWindowOnFocus", MouseDebugMessagesManager.MessageLevel.Info, "Message focused");
        m_messageReminderFocused = true;
        updateMessageReminderMenu();
    }

    public void callbackReminderWindowOkButton()
    {
        m_hologramChallengeDetailsReminder.SetActive(false);
        m_messageReminderFocused = false;
        updateMessageReminderMenu();
    }

    public void callbackReminderWindowBackButton()
    {
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackReminderWindowBackButton", MouseDebugMessagesManager.MessageLevel.Info, "Called (has m_messageDetailedFocused = true)");
        m_hologramChallengeDetailsReminder.SetActive(false);
        m_messageReminderFocused = false;
        updateMessageReminderMenu();

        m_hologramChallengeDetailsToDisplay.SetActive(true);
        m_messageDetailedFocused = true;
        updateMessageDetailedMenu();
    }

    public void callbackOnTapToPlaceFinished()
    {
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Bring specific components to the center of the interaction surface
        //m_hologramInviteChallenge.transform.localPosition = new Vector3(m_hologramInteractionSurface.transform.localPosition.x, m_hologramInteractionSurface.transform.localPosition.y+50, m_hologramInteractionSurface.transform.localPosition.z);

        gameObject.transform.position = m_hologramInteractionSurface.transform.position;
        //gameObject.transform. = m_hologramInteractionSurface.transform.localScale;
        m_hologramInteractionSurface.transform.localPosition = new Vector3(0, 0, 0);
        //m_hologramInteractionSurface.transform.localScale = new Vector3(1, 1, 1);

    }

    // This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge
    void updateMessageDetailedMenu ()
    {
        MouseChallengeCleanTableDetailsChallenge cleanTableDetailsChallenge = m_hologramChallengeDetailsToDisplay.GetComponent<MouseChallengeCleanTableDetailsChallenge>();

        if (cleanTableDetailsChallenge == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageDetailedMenu", MouseDebugMessagesManager.MessageLevel.Warning, "This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge");
            return;
        }
        if (m_messageDetailedFocused && m_palmOpened)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageDetailedMenu", MouseDebugMessagesManager.MessageLevel.Info, "Display palm menu");
            cleanTableDetailsChallenge.displayMenus(false, true);
        }
        else if (m_messageDetailedFocused && m_palmOpened == false)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageDetailedMenu", MouseDebugMessagesManager.MessageLevel.Info, "Display window menu");
            cleanTableDetailsChallenge.displayMenus(true, false);
        }
        else if ( m_messageDetailedFocused == false)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageDetailedMenu", MouseDebugMessagesManager.MessageLevel.Info, "Closing menus");
            cleanTableDetailsChallenge.displayMenus(false, false);
        }
    }

    void updateMessageReminderMenu()
    {
        MouseChallengeCleanTableDetailsChallenge cleanTableDetailsChallenge = m_hologramChallengeDetailsReminder.GetComponent<MouseChallengeCleanTableDetailsChallenge>();

        if (cleanTableDetailsChallenge == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageReminderMenu", MouseDebugMessagesManager.MessageLevel.Warning, "This function can run properly only if the gameobject managing the display of the detailed message contains the script MouseChallengeCleanTableDetailsChallenge");
            return;
        }

        if (m_messageReminderFocused && m_palmOpened)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageReminderMenu", MouseDebugMessagesManager.MessageLevel.Info, "Display palm menu");
            cleanTableDetailsChallenge.displayMenus(false, true);
        }
        else if (m_messageReminderFocused && m_palmOpened == false)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageReminderMenu", MouseDebugMessagesManager.MessageLevel.Info, "Display window menu");
            cleanTableDetailsChallenge.displayMenus(true, false);
        }
        else if (m_messageReminderFocused == false)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "updateMessageReminderMenu", MouseDebugMessagesManager.MessageLevel.Info, "Closing menus");
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

            /*m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Info, "Size of table panel: x=" + gameObject.GetComponent<Renderer>().bounds.size.x.ToString() + " y=" + gameObject.GetComponent<Renderer>().bounds.size.y.ToString());
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Info, "Local position of table panel: x =" + goLocalPosition.x.ToString() + " y =" + goLocalPosition.y.ToString() + " z =" + goLocalPosition.z.ToString());
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Info, "Position of table panel: x =" + gameObject.transform.position.x.ToString() + " y =" + gameObject.transform.position.y.ToString() + " z =" + gameObject.transform.position.z.ToString());*/

            /*float goScaleX = gameObject.transform.localScale.x;
            float goScaleY = gameObject.transform.localScale.y;
            float goScaleZ = gameObject.transform.localScale.z;*/

            float goScaleX = 1.0f;
            float goScaleZ = 1.0f;

            /*float goScaleX = m_hologramInteractionSurface.transform.localScale.x;
            float goScaleZ = m_hologramInteractionSurface.transform.localScale.z;*/

            float posX = 0.0f;
            float posZ = 0.0f;

            /*float incrementX = goScaleX / m_numberOfCubesToAddInColumn;
            float incrementZ = goScaleZ / m_numberOfCubesToAddInRow;*/

            /*float incrementX = 1.0f / m_numberOfCubesToAddInColumn;
            float incrementZ = 1.0f / m_numberOfCubesToAddInRow;*/

            float incrementX = goScaleX / m_numberOfCubesToAddInColumn;
            float incrementZ = goScaleZ / m_numberOfCubesToAddInRow;

            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Table panel position x=" + gameObject.transform.position.x.ToString() + " z=" + gameObject.transform.position.z.ToString() + " increment: x = " + incrementX.ToString() + " z = " + incrementZ.ToString());

            for (posX = 0.0f; posX < goScaleX; posX += incrementX)
            {
                for (posZ = 0.0f; posZ < goScaleZ; posZ += incrementZ)
                {
                    GameObject temp = Instantiate(m_hologramToUseToPopulateSurface);
                    //temp.transform.SetParent(gameObject.transform, false);
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
        if (transform.hasChanged)
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Transform has changed");
            transform.hasChanged = false;
        }
    }

    public void onTouch()
    {
        if (m_surfaceTouched == false) // Surface can be touched only once
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_hologramInviteChallenge.SetActive(true);
            /*MouseChallengeCleanTableInvite cubeInteractions = m_hologramInviteChallenge.GetComponent<MouseChallengeCleanTableInvite>();
            cubeInteractions.m_mouseChallengeCleanTableInviteHologramTouched += inviteChallengeHologramTouched; // Subsribe to the event so that the object is aware with the invite challenge hologram has been touched*/
            MouseUtilitiesHologramInteractionSwipes cubeInteractions = m_hologramInviteChallenge.GetComponent<MouseUtilitiesHologramInteractionSwipes>();
            cubeInteractions.m_utilitiesInteractionSwipesEventHelp += callbackInviteChallengeHologramHelp; // Subsribe to the event so that the object is aware with the invite challenge hologram has been touched
            cubeInteractions.m_utilitiesInteractionSwipesEventOk += callbackInviteChallengeHologramOk;
            cubeInteractions.m_utilitiesInteractionSwipesEventNok += callbackInviteChallengeHologramNok;
            m_surfaceTouched = true;

            // Play sound to get the user's attention from audio on top of visually
            //m_hologramInteractionSurface.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);

        }
        else
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Surface touched but process disabled");
        }
        
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
        m_hologramInviteChallenge.SetActive(false);

    }
}
