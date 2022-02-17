using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseUtilitiesAnimation : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public Vector3 m_positionEnd;
    public Vector3 m_scalingEnd;
    public float m_animationSpeed = 4.0f;
    public float m_scalingstep = 0.05f;

    public event EventHandler m_eventAnimationFinished;

    bool m_startAnimation = false;

    bool m_scalingGrow;

    public enum ConditionStopAnimation
    {
        // Gérer les cas où l'animation doit être arrêtée par le scaling ou par la position
        OnPositioning = 0,
        OnScaling =  1
    }

    public ConditionStopAnimation m_triggerStopAnimation = ConditionStopAnimation.OnPositioning;

    // Start is called before the first frame update
    void Start()
    {
        /*m_debug.displayMessage("MouseUtilitiesAnimation", "Start", MouseDebugMessagesManager.MessageLevel.Info, "Called");*/

        // To decide if the animation should scale up or down. By default, scaling down.
        if (gameObject.transform.localScale.x < m_scalingEnd.x )
        {
            m_scalingGrow = true;
        }
        else
        {
            m_scalingGrow = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_startAnimation)
        {
            float step = m_animationSpeed * Time.deltaTime;

            if (Vector3.Distance(gameObject.transform.position, m_positionEnd) > 0.001f)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, m_positionEnd, step);
            }

            if ( m_scalingGrow && gameObject.transform.localScale.x < m_scalingEnd.x )
            {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + m_scalingstep, gameObject.transform.localScale.y + m_scalingstep, gameObject.transform.localScale.z + m_scalingstep);
            }
            else if (m_scalingGrow == false && gameObject.transform.localScale.x > m_scalingEnd.x)
            {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - m_scalingstep, gameObject.transform.localScale.y - m_scalingstep, gameObject.transform.localScale.z - m_scalingstep);
            }

            if ( m_triggerStopAnimation == ConditionStopAnimation.OnPositioning )
            {
                if (Vector3.Distance(gameObject.transform.position, m_positionEnd) < 0.001f)
                {
                    //m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Animation finished - with position");
                    // Animation is finished: trigger event
                    m_eventAnimationFinished?.Invoke(this, EventArgs.Empty);

                    m_startAnimation = false;
                }
            }
            else if (m_triggerStopAnimation == ConditionStopAnimation.OnScaling)
            {
                if ((m_scalingGrow && gameObject.transform.localScale.x >= m_scalingEnd.x) ||
                    m_scalingGrow == false && gameObject.transform.localScale.x <= m_scalingEnd.x)
                {
                    //m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Animation finished - with scaling");

                    // Animation is finished: trigger event
                    m_eventAnimationFinished?.Invoke(this, EventArgs.Empty);

                    m_startAnimation = false;
                }
            }

            //m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "New position: " + gameObject.transform.position.ToString());
        }
    }

    public void startAnimation()
    {
        /*m_debug.displayMessage("MouseUtilitiesAnimation", "startAnimation", MouseDebugMessagesManager.MessageLevel.Info, "Called");*/

        m_startAnimation = true;
    }

    public void animateDiseappearInPlace(MouseDebugMessagesManager debug ,EventHandler e)
    {
        m_debug = debug;
        m_positionEnd = gameObject.transform.position;//gameObject.transform.TransformPoint(new Vector3(0, 0.6f, 0));
        m_scalingEnd = new Vector3(0f, 0f, 0f);
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
        m_eventAnimationFinished += e;
        startAnimation();
    }

    public void animateAppearInPlace(MouseDebugMessagesManager debug, EventHandler e)
    {
        /*m_debug = debug;
        m_animationSpeed = 1.0f;
        m_scalingstep = 0.01f;
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        m_positionEnd = gameObject.transform.position;//gameObject.transform.TransformPoint(new Vector3(0, 0.6f, 0));
        m_scalingEnd = new Vector3(1.0f, 1.0f, 1.0f);
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
        m_eventAnimationFinished += e;

        gameObject.SetActive(true);
        startAnimation();*/

        animateAppearInPlaceToScaling(new Vector3(1.0f, 1.0f, 1.0f), debug, e);
    }

    public void animateAppearInPlaceToScaling(Vector3 targetScaling, MouseDebugMessagesManager debug, EventHandler e)
    {
        m_debug = debug;
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        m_positionEnd = gameObject.transform.position;//gameObject.transform.TransformPoint(new Vector3(0, 0.6f, 0));
        m_scalingEnd = targetScaling;
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
        m_eventAnimationFinished += e;

        gameObject.SetActive(true);
        startAnimation();
    }

    /**
     * In this function, the animation will start from a given position, and will bring it to its current position
     **/
    public void animateAppearFromPosition (Vector3 pos, MouseDebugMessagesManager debug, EventHandler e)
    {
        m_debug = debug;
        m_positionEnd = gameObject.transform.position;//gameObject.transform.TransformPoint(new Vector3(0, 0.6f, 0));
        m_scalingEnd = new Vector3(1.0f, 1.0f, 1.0f);
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
        m_eventAnimationFinished += e;

        gameObject.transform.position = pos; // Moving the object to the starting position
        gameObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f); // Set scaling to 0 before starting

        gameObject.SetActive(true);

        startAnimation();
    }

    /**
 * In this function, the animation will start from the object's current position, and will diseppear gradually to the target position given as input parameter
 **/
    public void animateDiseappearToPosition(Vector3 pos, MouseDebugMessagesManager debug, EventHandler e)
    {
        m_debug = debug;
        m_positionEnd = pos;
        m_scalingEnd = new Vector3(0f, 0f, 0f);
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
        m_eventAnimationFinished += e;

        startAnimation();
    }
}
