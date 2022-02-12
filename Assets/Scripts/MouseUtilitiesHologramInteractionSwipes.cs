using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

public class MouseUtilitiesHologramInteractionSwipes : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    bool m_hologramsAlreadyDisplayed;
    float m_animationSpeed;
    Vector3 m_ParentOrigin;

    public event EventHandler m_utilitiesInteractionSwipesEventOk;
    public event EventHandler m_utilitiesInteractionSwipesEventNok;
    public event EventHandler m_utilitiesInteractionSwipesEventHelp;

    //MouseUtilitiesAnimation m_animatorParent;

    enum InteractionState
    {
        InteractionStateParentStandBy = 0,      // Nothing happens with the parent
        InteractionStateParentMoveStart = 1,    // The parent is starting to be moved
        InteractionStateParentMoveOngoing = 2,  // The parent is being moved
        InteractionStateParentMoveEnd = 3,      // The parent is released
        InteractionStateParentHitNothing = 4,   // When the parent is released, it does not hit anything
        InteractionStateParentHitOk = 5,        // When the parent is released, it hits the ok button
        InteractionStateParentHitNok = 6,       // When the parent is released, it hits the nok button
        InteractionStateParentHitHelp = 7,       // When the parent is released, it hits the help button
        InteractionStateParentAnimationOngoing = 8,
        InteractionStateParentAnimationFinished = 9,
        InteractionStateMenuAnimationOngoing = 10,
        InteractionStateMenuAnimationFinished = 11
    }

    public enum HologramPositioning
    {
        Left = 0,
        Right = 1,
        Bottom = 2,
        Top = 3
    }

    public struct InteractionHolograms
    {
        public GameObject hologram;
        public HologramPositioning positionRelativeToParent;
        public Vector3 positionOriginWorld;
        public bool touched; // Should be set to true if touched by the main cube, false otherwise
    }

    public InteractionHolograms m_hologramNok;
    public InteractionHolograms m_hologramOk;
    public InteractionHolograms m_hologramHelp;

    InteractionState m_objectStatus;

    // Start is called before the first frame update
    void Start()
    {
        m_objectStatus = InteractionState.InteractionStateParentStandBy; // To start, nothing is happening with the object, so stand by status.

        m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Start", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_animationSpeed = 1.0f;

        m_hologramsAlreadyDisplayed = false;

        // Instanciate Nok hologram
        m_hologramNok.positionRelativeToParent = HologramPositioning.Bottom;
        m_hologramNok.hologram = instantiateHologramInstructions("Mouse_Refuse", convertRelativeToConcretePosition(m_hologramNok.positionRelativeToParent),
            convertPositionRelativeToScaling(m_hologramNok.positionRelativeToParent), 
            convertPositionRelativeToRotation(m_hologramNok.positionRelativeToParent));
        m_hologramNok.positionOriginWorld = m_hologramNok.hologram.transform.position;
        m_hologramNok.touched = false;

        // Instanciate Ok hologram
        m_hologramOk.positionRelativeToParent = HologramPositioning.Right;
        m_hologramOk.hologram = instantiateHologramInstructions("Mouse_Agree", convertRelativeToConcretePosition(m_hologramOk.positionRelativeToParent),
            convertPositionRelativeToScaling(m_hologramOk.positionRelativeToParent),
            convertPositionRelativeToRotation(m_hologramOk.positionRelativeToParent));
        m_hologramOk.positionOriginWorld = m_hologramOk.hologram.transform.position;
        m_hologramOk.touched = false;

        // Instanciate help hologram
        m_hologramHelp.positionRelativeToParent = HologramPositioning.Left;
        m_hologramHelp.hologram = instantiateHologramInstructions("Mouse_Help", convertRelativeToConcretePosition(m_hologramHelp.positionRelativeToParent),
            convertPositionRelativeToScaling(m_hologramHelp.positionRelativeToParent),
            convertPositionRelativeToRotation(m_hologramHelp.positionRelativeToParent));
        m_hologramHelp.positionOriginWorld = m_hologramHelp.hologram.transform.position;
        m_hologramHelp.touched = false;

        // Get parent's origin
        m_ParentOrigin = gameObject.transform.position;

        // Set billboard to parent
        setBillboardToGameObject(true);

        // Prepare parent's animator, i.e. connect the callback. For the initialization of the different parameters, will be done when required during the process
        MouseUtilitiesAnimation animatorParent = gameObject.AddComponent<MouseUtilitiesAnimation>();
        animatorParent.m_eventAnimationFinished += callbackAnimationParentFinished;
        animatorParent.m_animationSpeed = 1.0f;
        animatorParent.m_debug = m_debug;
        animatorParent.m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnPositioning;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_objectStatus == InteractionState.InteractionStateParentStandBy)
        { // No interactions are happening with the parent. The person might be moving around, and so the "reference" position of the surrounding cubes (i.e. being used above when the parent is being moved by hand) is updated
            if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) < 1.5f && m_hologramsAlreadyDisplayed == false)
            {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "User close to hologram");

                m_hologramsAlreadyDisplayed = true;

                displayInteractionHolograms(m_hologramsAlreadyDisplayed);
            }
            else if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) > 1.5f && m_hologramsAlreadyDisplayed)
            {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "User far from hologram");

                // Time to hide the holograms and set the status boolean to false
                m_hologramsAlreadyDisplayed = false;
                displayInteractionHolograms(m_hologramsAlreadyDisplayed);
            }

            //Vector3 parentPosition = gameObject.transform.transform.position;
            m_ParentOrigin = gameObject.transform.position;

            m_hologramOk.positionOriginWorld = m_hologramOk.hologram.transform.position;
            m_hologramNok.positionOriginWorld = m_hologramNok.hologram.transform.position;
            m_hologramHelp.positionOriginWorld = m_hologramHelp.hologram.transform.position;
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentMoveStart)
        {
            m_objectStatus = InteractionState.InteractionStateParentMoveOngoing;
            setBillboardToGameObject(false);
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentMoveOngoing)
        { // If the parent is being moved by the hand, two processes to do here ...
          // First : the parent's position has to be updated in case it reaches the "boundaries" it can be moved to
            Vector3 parentPosition = gameObject.transform.position;

            if (parentPosition.x <= m_ParentOrigin.x - 0.5f)
            {
                parentPosition.x = m_ParentOrigin.x - 0.5f;
            }

            if (parentPosition.x >= m_ParentOrigin.x + 0.5f)
            {
                parentPosition.x = m_ParentOrigin.x + 0.5f;
            }

            if (parentPosition.y <= m_ParentOrigin.y - 0.5f)
            {
                parentPosition.y = m_ParentOrigin.y - 0.5f;
            }

            if (parentPosition.y >= m_ParentOrigin.y + 0.5f)
            {
                parentPosition.y = m_ParentOrigin.y + 0.5f;
            }

            if (parentPosition.z <= m_ParentOrigin.z - 0.5f)
            {
                parentPosition.z = m_ParentOrigin.z - 0.5f;
            }

            if (parentPosition.z >= m_ParentOrigin.z + 0.5f)
            {
                parentPosition.z = m_ParentOrigin.z + 0.5f;
            }

            gameObject.transform.position = parentPosition;

            /*m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "callbackParentIsMoved", MouseDebugMessagesManager.MessageLevel.Info, "Parent's position is: " + gameObject.transform.position.ToString() + " and should be: " + parentPosition.ToString());*/

            // Second: the surrounding cubes are kept in their original position.
            m_hologramOk.hologram.transform.position = m_hologramOk.positionOriginWorld;
            m_hologramNok.hologram.transform.position = m_hologramNok.positionOriginWorld;
            m_hologramHelp.hologram.transform.position = m_hologramHelp.positionOriginWorld;
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentMoveEnd) //m_hologramParentMoveEnd == true)
        {
            // If one of the following three conditions is true, no animation: the whole hologram disapears. If the else condition is reached, that means no interaction occured, and thus the animation can run
            if (Vector3.Distance(gameObject.transform.position, m_hologramNok.hologram.transform.position) < 0.15f)
            {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Hologram Nok has been touched");
                m_objectStatus = InteractionState.InteractionStateParentHitNok;
            }
            else if (Vector3.Distance(gameObject.transform.position, m_hologramOk.hologram.transform.position) < 0.15f)
            {
                m_objectStatus = InteractionState.InteractionStateParentHitOk;
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Hologram Ok has been touched");
            }
            else if (Vector3.Distance(gameObject.transform.position, m_hologramHelp.hologram.transform.position) < 0.15f)
            {
                m_objectStatus = InteractionState.InteractionStateParentHitHelp;
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Hologram Help has been touched");
            }
            else
            { // If we reach this point, that means nothing has been hit when releasing the parent's object
                m_objectStatus = InteractionState.InteractionStateParentHitNothing;
            }
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentHitNothing)
        {
            // Start the animation: be careful, the animation run on another object, so some processes will run on the callback called bby the dedicated object when the animation is finished
            MouseUtilitiesAnimation animatorParent = gameObject.GetComponent<MouseUtilitiesAnimation>();
            animatorParent.m_positionEnd = m_ParentOrigin;
            animatorParent.m_scalingEnd = gameObject.transform.localScale;

            m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "InteractionStateParentHitNothing - Local scale of parent object: " + gameObject.transform.localScale.ToString());

            m_objectStatus = InteractionState.InteractionStateParentAnimationOngoing;

            animatorParent.startAnimation();
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentAnimationOngoing)
        {
            /*m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "InteractionStateParentAnimationFinished - Nok's position is: " + m_hologramNok.hologram.transform.position.ToString() + " and should be: " + m_hologramNok.positionOriginWorld);*/

            m_hologramOk.hologram.transform.position = m_hologramOk.positionOriginWorld;
            m_hologramNok.hologram.transform.position = m_hologramNok.positionOriginWorld;
            m_hologramHelp.hologram.transform.position = m_hologramHelp.positionOriginWorld;
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentAnimationFinished)
        {
            m_objectStatus = InteractionState.InteractionStateParentStandBy;

            gameObject.transform.position = m_ParentOrigin;
            m_hologramNok.hologram.transform.localPosition = convertRelativeToConcretePosition(m_hologramNok.positionRelativeToParent);
            m_hologramOk.hologram.transform.localPosition = convertRelativeToConcretePosition(m_hologramOk.positionRelativeToParent);
            m_hologramHelp.hologram.transform.localPosition = convertRelativeToConcretePosition(m_hologramHelp.positionRelativeToParent);

            m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "InteractionStateParentAnimationFinished - Nok's position is: " + m_hologramNok.hologram.transform.position.ToString() + " and should be: " + m_hologramNok.positionOriginWorld);

            setBillboardToGameObject(true);
        }
        else if (m_objectStatus == InteractionState.InteractionStateParentHitNok ||
            m_objectStatus == InteractionState.InteractionStateParentHitOk ||
            m_objectStatus == InteractionState.InteractionStateParentHitHelp)
        {
            m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Hitting menus - Called");
            // Animate to make the parent's cube disapearing "in" the touched hologram.i.e. adding one animation per hologram
            MouseUtilitiesAnimation animator = m_hologramNok.hologram.AddComponent<MouseUtilitiesAnimation>();
            animator.m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
            animator.m_positionEnd = m_hologramNok.hologram.transform.position;
            animator.m_debug = m_debug;
            animator.m_scalingEnd = new Vector3(0.001f, 0.001f, 0.001f);
            animator.m_animationSpeed = 3.0f;
            animator.m_scalingstep = 0.003f;
            animator.startAnimation();

            animator = m_hologramOk.hologram.AddComponent<MouseUtilitiesAnimation>();
            animator.m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
            animator.m_positionEnd = m_hologramOk.hologram.transform.position;
            animator.m_debug = m_debug;
            animator.m_scalingEnd = new Vector3(0.001f, 0.001f, 0.001f);
            animator.m_animationSpeed = 3.0f;
            animator.m_scalingstep = 0.003f;
            animator.startAnimation();

            animator = m_hologramHelp.hologram.AddComponent<MouseUtilitiesAnimation>();
            animator.m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
            animator.m_positionEnd = m_hologramHelp.hologram.transform.position;
            animator.m_debug = m_debug;
            animator.m_scalingEnd = new Vector3(0.001f, 0.001f, 0.001f);
            animator.m_animationSpeed = 3.0f;
            animator.m_scalingstep = 0.003f;
            animator.startAnimation();

            MouseUtilitiesAnimation animatorParent = gameObject.GetComponent<MouseUtilitiesAnimation>();
            animatorParent.m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
            animatorParent.m_eventAnimationFinished += callbackAnimationMenuFinished;
            animatorParent.m_debug = m_debug;
            animatorParent.m_scalingEnd = new Vector3(0.001f, 0.001f, 0.001f);
            animatorParent.m_animationSpeed = 3.0f;
            animatorParent.m_scalingstep = 0.003f;
            animatorParent.startAnimation();

            if (m_objectStatus == InteractionState.InteractionStateParentHitNok)
            {
                animatorParent.m_positionEnd = m_hologramNok.hologram.transform.position;
                m_hologramNok.touched = true;
            }
            else if (m_objectStatus == InteractionState.InteractionStateParentHitOk)
            {
                animatorParent.m_positionEnd = m_hologramOk.hologram.transform.position;
                m_hologramOk.touched = true;
            }
            else if (m_objectStatus == InteractionState.InteractionStateParentHitHelp)
            {
                animatorParent.m_positionEnd = m_hologramHelp.hologram.transform.position;
                m_hologramHelp.touched = true;
            }

            m_objectStatus = InteractionState.InteractionStateMenuAnimationOngoing;
        }
        else if (m_objectStatus == InteractionState.InteractionStateMenuAnimationOngoing)
        {
            m_hologramOk.hologram.transform.position = m_hologramOk.positionOriginWorld;
            m_hologramNok.hologram.transform.position = m_hologramNok.positionOriginWorld;
            m_hologramHelp.hologram.transform.position = m_hologramHelp.positionOriginWorld;
        }
        else if ( m_objectStatus == InteractionState.InteractionStateMenuAnimationFinished)
        {
            // Animation is finished: inform the rest of the worl that the button has been hit and back to stand by mode
            if(m_hologramNok.touched)
            {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "InteractionStateMenuAnimationFinished - Nok event raised");
                m_utilitiesInteractionSwipesEventNok?.Invoke(this, EventArgs.Empty);
            }
            else if (m_hologramOk.touched)
            {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "InteractionStateMenuAnimationFinished - Ok event raised");
                m_utilitiesInteractionSwipesEventOk?.Invoke(this, EventArgs.Empty);
            }
            else if (m_hologramHelp.touched)
            {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "InteractionStateMenuAnimationFinished - Help event raised");

                m_utilitiesInteractionSwipesEventHelp?.Invoke(this, EventArgs.Empty);
            }
            else {
                m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Error, "The state InteractionStateMenuAnimationFinished should not be invoked if no hologram's menus touched bool are set to true");
            }

            resetHologramsPositions();
            m_objectStatus = InteractionState.InteractionStateParentStandBy;
        }
        else
        {
            m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "Update", MouseDebugMessagesManager.MessageLevel.Error, "At least one state is not managed. This will likely cause this part of the software to fail.");
        }
    }

    void displayInteractionHolograms(bool display)
    {
        m_hologramNok.hologram.SetActive(display);
        m_hologramOk.hologram.SetActive(display);
        m_hologramHelp.hologram.SetActive(display);
    }

    public void callbackParentIsMoved()
    {
        m_objectStatus = InteractionState.InteractionStateParentMoveStart;
    }

    public void callbackParentReleased()
    {
        m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "callbackParentReleased", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        //m_hologramParentMoveEnd = true;
        m_objectStatus = InteractionState.InteractionStateParentMoveEnd;
    }

    void setBillboardToGameObject(bool add)
    {
        if (add)
        {
            // Restore billboard
            if (gameObject.GetComponent<Billboard>() == null)
            {
                gameObject.AddComponent(typeof(Billboard));
                if (gameObject.GetComponent<Billboard>() != null)
                {
                    gameObject.GetComponent<Billboard>().PivotAxis = PivotAxis.Y;
                    m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "setBillboardToParent", MouseDebugMessagesManager.MessageLevel.Info, "Billboard component has been successfully added");
                }
            }
        }
        else
        {
            // Disable billboard
            if (gameObject.GetComponent<Billboard>() != null)
            {
                Destroy(gameObject.GetComponent<Billboard>());
                if (gameObject.GetComponent<Billboard>() == null)
                {
                    m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "setBillboardToParent", MouseDebugMessagesManager.MessageLevel.Info, "Billboard component has been successfully destroyed");
                }
            }
        }
    }

    GameObject instantiateHologramInstructions(string materialName, Vector3 positionLocal, Vector3 scalingLocal, Vector3 rotationLocal)
    {
        return instantiateHologramInstructions(materialName, positionLocal.x, positionLocal.y, positionLocal.z, scalingLocal.x, scalingLocal.y, scalingLocal.z, rotationLocal.x, rotationLocal.y, rotationLocal.z);
    }

    GameObject instantiateHologramInstructions(string materialName, float offsetX, float offsetY, float offsetZ, float scaleX, float scaleY, float scaleZ, float rotationX = 0.0f, float rotationY = 0.0f, float rotationZ = 0.0f)
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temp.transform.SetParent(gameObject.transform);
        temp.GetComponent<Renderer>().material = Resources.Load(materialName, typeof(Material)) as Material;
        temp.transform.localPosition = new Vector3(offsetX, offsetY, offsetZ);
        temp.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        temp.transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        temp.SetActive(false);

        return temp;
    }

    Vector3 convertRelativeToConcretePosition(HologramPositioning positionRelative)
    {
        Vector3 toReturn;

        if (positionRelative == HologramPositioning.Bottom)
        {
            toReturn = new Vector3(0, -1.5f, 0);
        }
        else if (positionRelative == HologramPositioning.Left)
        {
            toReturn = new Vector3(-1.5f, 0, 0);
        }
        else if (positionRelative == HologramPositioning.Right)
        {
            toReturn = new Vector3(1.5f, 0, 0);
        }
        else if (positionRelative == HologramPositioning.Top)
        {
            toReturn = new Vector3(0, 1.5f, 0);
        }
        else
        {
            toReturn = new Vector3(-1, -1, -1);
        }

        return toReturn;
    }

    Vector3 convertPositionRelativeToScaling(HologramPositioning positionRelative)
    {
        Vector3 toReturn;

        if (positionRelative == HologramPositioning.Bottom || positionRelative == HologramPositioning.Left || positionRelative == HologramPositioning.Right || positionRelative == HologramPositioning.Top)
        {
            toReturn = new Vector3(0.3f, 0.3f, 0.01f);
        }
        else
        {
            toReturn = new Vector3(-1, -1, -1);
        }

        return toReturn;
    }

    Vector3 convertPositionRelativeToRotation(HologramPositioning positionRelative)
    {
        Vector3 toReturn;

        if (positionRelative == HologramPositioning.Bottom)
        {
            toReturn = new Vector3(0, 0, 180);
        }
        else if (positionRelative == HologramPositioning.Left)
        {
            toReturn = new Vector3(0, 0, 180);
        }
        else if(positionRelative == HologramPositioning.Right)
        {
            toReturn = new Vector3(0, 0, 180);
        }
        else if (positionRelative == HologramPositioning.Top)
        {
            toReturn = new Vector3(0, 0, 180);
        }
        else
        {
            toReturn = new Vector3(-1, -1, -1);
        }

        return toReturn;
    }

    void resetHologramsPositions()
    {
        m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "resetHologramsPositions", MouseDebugMessagesManager.MessageLevel.Info, "Parent's position is: " + gameObject.transform.position.ToString());

        //Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
        Destroy(m_hologramHelp.hologram.GetComponent<MouseUtilitiesAnimation>());
        Destroy(m_hologramNok.hologram.GetComponent<MouseUtilitiesAnimation>());
        Destroy(m_hologramOk.hologram.GetComponent<MouseUtilitiesAnimation>());

        gameObject.transform.position = m_ParentOrigin;
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        m_hologramHelp.hologram.transform.localScale = convertPositionRelativeToScaling(m_hologramHelp.positionRelativeToParent);
        m_hologramHelp.hologram.transform.localPosition = convertRelativeToConcretePosition(m_hologramHelp.positionRelativeToParent);
        m_hologramNok.hologram.transform.localScale = convertPositionRelativeToScaling(m_hologramNok.positionRelativeToParent);
        m_hologramNok.hologram.transform.localPosition = convertRelativeToConcretePosition(m_hologramNok.positionRelativeToParent);
        m_hologramOk.hologram.transform.localScale = convertPositionRelativeToScaling(m_hologramOk.positionRelativeToParent);
        m_hologramOk.hologram.transform.localPosition = convertRelativeToConcretePosition(m_hologramOk.positionRelativeToParent);
    }

    void callbackAnimationParentFinished(object sender, EventArgs e)
    {
        m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "callbackAnimationParentFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_objectStatus = InteractionState.InteractionStateParentAnimationFinished;
    }

    void callbackAnimationMenuFinished(object sender, EventArgs e)
    {
        m_debug.displayMessage("MouseUtilitiesHologramInteractionSwipes", "callbackAnimationMenuFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_objectStatus = InteractionState.InteractionStateMenuAnimationFinished;
    }
}