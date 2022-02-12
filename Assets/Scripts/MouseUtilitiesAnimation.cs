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
    public float m_scalingstep = 0.003f;

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
        m_debug.displayMessage("MouseUtilitiesAnimation", "Start", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        //m_startAnimation = false;
        //m_triggerStopAnimation = ConditionStopAnimation.OnPositioning;

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
        /*m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Called " + gameObject.transform.parent.name);*/

        if (m_startAnimation)
        {
            /*m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Animation started");*/

            float step = m_animationSpeed * Time.deltaTime;

            if (Vector3.Distance(gameObject.transform.position, m_positionEnd) > 0.001f)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, m_positionEnd, step);
               /* m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Position: " + gameObject.transform.position.ToString());*/
            }

            if ( m_scalingGrow && gameObject.transform.localScale.x < m_scalingEnd.x )
            {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + m_scalingstep, gameObject.transform.localScale.y + m_scalingstep, gameObject.transform.localScale.z + m_scalingstep);
                /*m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Scaling updated grow: " + gameObject.transform.localScale.ToString());*/
            }
            else if (m_scalingGrow == false && gameObject.transform.localScale.x > m_scalingEnd.x)
            {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - m_scalingstep, gameObject.transform.localScale.y - m_scalingstep, gameObject.transform.localScale.z - m_scalingstep);
                /*m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Scaling updated shrink: " + gameObject.transform.localScale.ToString());*/
            }

            if ( m_triggerStopAnimation == ConditionStopAnimation.OnPositioning )
            {
                if (Vector3.Distance(gameObject.transform.position, m_positionEnd) < 0.001f)
                {
                    m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Animation finished - with position");
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
                    m_debug.displayMessage("MouseUtilitiesAnimation", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Animation finished - with scaling");

                    // Animation is finished: trigger event
                    m_eventAnimationFinished?.Invoke(this, EventArgs.Empty);

                    m_startAnimation = false;
                }
            }
        }
    }

    public void startAnimation()
    {
        m_debug.displayMessage("MouseUtilitiesAnimation", "startAnimation", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_startAnimation = true;
    }
}
