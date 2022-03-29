using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Reflection;
using System.Linq;

public class MouseChallengeCleanTableReminderOneClockMoving : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    Transform m_clockView; // Because we can have multiple clocks
    //Transform m_clockRefView; // The clock object already implemented, that will be duplicated on request of the user

    Transform m_hologramWindowReminderButtonOkView;
    Transform m_hologramWindowReminderView;
    Transform m_hologramWindowReminderButtonBackView;

    public event EventHandler m_eventHologramClockTouched;
    public event EventHandler m_eventHologramWindowButtonOkTouched;
    public event EventHandler m_eventHologramWindowButtonBackTouched;

    MouseUtilitiesGradationManager m_gradationManager;

    Vector3 m_positionLocalOrigin;
    float m_yOffsetOrigin;

    // As the clock does not have an attached script, storing the required information here
    Vector3 m_clockScalingOriginal;
    Vector3 m_clockScalingReduced;

    //bool m_mutexHiding = false;
    MouseUtilitiesMutex m_mutexHide;
    MouseUtilitiesMutex m_mutexShow;

    List<Transform> m_objectsToBeClose;

    bool m_newObjectToFocus;
    Transform m_newObjectToFocusTransform;

    private void Awake()
    {
        // Initialize variables
        m_objectsToBeClose = new List<Transform>();

        // Let's get the children
        m_clockView = gameObject.transform.Find("Clock");
        m_hologramWindowReminderView = gameObject.transform.Find("Text");

        m_hologramWindowReminderButtonOkView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonOk");
        m_hologramWindowReminderButtonBackView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonBack");

        m_mutexHide = new MouseUtilitiesMutex(m_debug);
        m_mutexShow = new MouseUtilitiesMutex(m_debug);

        m_positionLocalOrigin = transform.localPosition;
        m_yOffsetOrigin = transform.localPosition.y;

        m_newObjectToFocus = false;

        m_newObjectToFocusTransform = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        

        // Connect callbacks
        //InteractableOnTouchReceiver touchReceiver = m_hologramClockView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>();
        //touchReceiver.OnTouchStart.AddListener(callbackOnClockTouched);
        MouseUtilitiesHologramInteractions temp = m_clockView.GetComponent<MouseUtilitiesHologramInteractions>();
        //touchReceiver.OnTouchStart.AddListener(callbackOnClockTouched);
        temp.s_touched += callbackOnClockTouched;
        m_hologramWindowReminderButtonOkView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowOkButtonTouched);
        m_hologramWindowReminderButtonBackView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowBackButtonTouched);

        m_gradationManager = transform.GetComponent<MouseUtilitiesGradationManager>();
        if (m_gradationManager == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "MouseUtilitiesGradationManager component not present in that gameobject, whereas it is required. The object will most likely not work properly");
        }
        m_gradationManager.addNewAssistanceGradation("Default", callbackGradationDefault);
        m_gradationManager.addNewAssistanceGradation("HighFollow", callbackGradationHighFollow);

        //m_positionLocalOrigin = transform.localPosition;

        // Getting informations related to the clock
        m_clockScalingOriginal = m_clockView.localScale;
        m_clockScalingReduced = m_clockScalingOriginal / 3.0f;

        
    }

    // Update is called once per frame
    void Update()
    {
       //m_hologramClock.RotateAround(transform.position, Vector3.up, 20 * Time.deltaTime);
       if (m_newObjectToFocus)
        {
            m_newObjectToFocus = false; // Managed so disable to avoid this to be called each time

            gameObject.transform.position = m_newObjectToFocusTransform.position;
            m_positionLocalOrigin = gameObject.transform.localPosition;

        }

    }

    public void addObjectToBeClose(Transform o) // The parent the clock will belong to
    {
        m_objectsToBeClose.Add(o);

        MouseUtilitiesHologramInteractions interactions = o.gameObject.GetComponent<MouseUtilitiesHologramInteractions>();

        if (interactions == null)
        {
            interactions = o.gameObject.AddComponent<MouseUtilitiesHologramInteractions>();
        }

        //touchReceiver.OnTouchStart.AddListener(callbackOnClockTouched);
        interactions.s_focusOn += callbackOnObjectFocus;
    }


    void callbackOnClockTouched(System.Object sender, EventArgs e)
    {// If a clock is touched, all other clocks are hidden

        MouseUtilitiesAnimation animator = m_clockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs ee)
        {
            m_clockView.gameObject.SetActive(false);

            // When the clock has diseappeared, make the text appearing, after moving it to the place of the hidden clock
            m_hologramWindowReminderView.position = m_clockView.position;
            MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            animatorText.animateAppearInPlace(m_debug, MouseUtilities.getEventHandlerEmpty());

            Destroy(m_clockView.GetComponent<MouseUtilitiesAnimation>());
        }));

        m_eventHologramClockTouched?.Invoke(this, EventArgs.Empty); // Informing the world that a clock has been touched

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        /*TODO-done MouseUtilitiesAnimation animator = m_hologramClockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
         {
             m_hologramClockView.gameObject.SetActive(false);

             // When the clock has diseappeared, make the windows appearing
             MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
             animatorText.animateAppearInPlace(m_debug, MouseUtilities.getEventHandlerEmpty());

             Destroy(m_hologramClockView.GetComponent<MouseUtilitiesAnimation>());
         }));*/
    }

    void callbackOnObjectFocus(System.Object sender, EventArgs e)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called - Sender type: " + sender.GetType());
        

        if ( m_newObjectToFocusTransform == null || (m_newObjectToFocusTransform.name != ((GameObject) sender).name))
        { // To avoid doing unnecessary processes if the situation did not change
            string currentObjectName = "not initialized yet";

            if (m_newObjectToFocusTransform != null)
            {
                currentObjectName = ((UnityEngine.GameObject)sender).name;
            }

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Seems to work - Currently focused object: " + currentObjectName + "| New one: " + ((GameObject)sender).name + ". If the 2 names are different, this is good news");
            m_newObjectToFocusTransform = ((GameObject)sender).transform;
            m_newObjectToFocus = true;
        }
    }

    void callbackOnWindowOkButtonTouched()
    {
        m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);

        /*MouseUtilitiesAnimation animator = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
            resetObject();
        }));*/
    }

    void callbackOnWindowBackButtonTouched()
    {
        m_eventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
    }

    public void hide(EventHandler e)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexHide.isLocked() == false)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");
            m_mutexHide.lockMutex();

            GameObject temp;

            if (m_clockView.gameObject.activeSelf)
            {
                temp = m_clockView.gameObject;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock is going to be hidden");
            }
            else
            { // Only two children for know so fine this way. But to be extended if it happens that more children are added in the future
                temp = m_hologramWindowReminderView.gameObject;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Text is going to be hidden");
            }

            EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object oe, EventArgs ee)
            {
                //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance hidden");

                //gameObject.SetActive(false);

                

                m_mutexHide.unlockMutex();
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - object hidden");


                Destroy(temp.GetComponent<MouseUtilitiesAnimation>());
                resetObject();
            }), e };

            MouseUtilitiesAnimation animator = temp.AddComponent<MouseUtilitiesAnimation>();
            animator.animateDiseappearInPlace(m_debug, eventHandlers);
        }
    }

    public void show(EventHandler eventHandler)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexShow.isLocked() == false)
        {
            m_mutexShow.lockMutex();

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");

            MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, transform, m_yOffsetOrigin);

            EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock should be visible now");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

                m_mutexShow.unlockMutex();

            }), eventHandler };

            MouseUtilitiesAnimation animator = m_clockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            animator.animateAppearInPlaceToScaling(m_clockScalingOriginal, m_debug, eventHandlers);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex already locked - nothing to do");
        }
    }

    void callbackGradationDefault(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;

        show(MouseUtilities.getEventHandlerEmpty());

        m_clockView.transform.localScale = m_clockScalingOriginal;
        //clockFollower.localPosition = new Vector3(clockFollower.localPosition.x, clockFollower.localPosition.y, clockFollower.localPosition.z - 0.25f);// Removing the offset
    }

    void callbackGradationHighFollow(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = false;
        GetComponent<RadialView>().enabled = true;

        m_clockView.transform.localScale = m_clockScalingReduced;
        //m_hologramClockView.localPosition = new Vector3(m_hologramClockView.localPosition.x, m_hologramClockView.localPosition.y, m_hologramClockView.localPosition.z + 0.25f); // Adding an offset to avoid that the object remains in front of the user
    }

    public bool increaseGradation()
    {
        return m_gradationManager.increaseGradation();
    }

    public bool decreaseGradation()
    {
        return m_gradationManager.decreaseGradation();
    }

    public void setGradationToMinimum()
    {
        m_gradationManager.setGradationToMinimum();
    }

    // Be aware that this function does not send the object back to its original position
    void resetObject()
    {
        //m_hologramClockView.gameObject.SetActive(true);
        //m_hologramWindowReminderView.gameObject.SetActive(false);

        //TODO m_hologramClockView.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        m_hologramWindowReminderView.gameObject.transform.localScale = new Vector3(1, 1, 1);
        //TODO m_hologramClockView.gameObject.SetActive(false);
        m_hologramWindowReminderView.gameObject.SetActive(false);

        m_gradationManager.setGradationToMinimum();
    }

    public void setObjectToOriginalPosition()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Changing position");

        transform.localPosition = m_positionLocalOrigin;
    }
}
