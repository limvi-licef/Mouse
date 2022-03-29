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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

/**
 * Generic component to handle animations. For now show and hide are supported, in place, to a given position and at a certain scaling.
 * */
public class MouseUtilitiesAnimation : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public Vector3 m_positionEnd;
    public Vector3 m_scalingEnd;
    public float m_animationSpeed = 4.0f;
    public float m_scalingstep = 0.05f;

    event EventHandler m_eventAnimationFinished;

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
                    // Animation is finished: trigger event
                    m_eventAnimationFinished?.Invoke(this, EventArgs.Empty);

                    m_startAnimation = false;
                }
            }
        }
    }

    public void startAnimation()
    {
        m_startAnimation = true;
    }

    public void animateDiseappearInPlace(MouseDebugMessagesManager debug, EventHandler eventHandler)
    {
        EventHandler[] temp = new EventHandler[1];

        temp[0] = eventHandler;

        animateDiseappearInPlace(debug, temp);
    }

    public void animateDiseappearInPlace(MouseDebugMessagesManager debug ,EventHandler[] eventHandlers)
    {
        m_debug = debug;

        m_positionEnd = gameObject.transform.position;
        m_scalingEnd = new Vector3(0f, 0f, 0f);
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;

        foreach (EventHandler e in eventHandlers) {
            m_eventAnimationFinished += e;
        }
            
        startAnimation();
    }

    public void animateAppearInPlace(MouseDebugMessagesManager debug, EventHandler e)
    {
        EventHandler[] eventHandlers = new EventHandler[] { e };

        animateAppearInPlace(debug, eventHandlers);
    }

    public void animateAppearInPlace(MouseDebugMessagesManager debug, EventHandler[] e)
    {
        animateAppearInPlaceToScaling(new Vector3(1.0f, 1.0f, 1.0f), debug, e);
    }

    public void animateAppearInPlaceToScaling(Vector3 targetScaling, MouseDebugMessagesManager debug, EventHandler eventHandler)
    {
        animateAppearInPlaceToScaling(targetScaling, debug, new EventHandler[] { eventHandler });
    }

    public void animateAppearInPlaceToScaling(Vector3 targetScaling, MouseDebugMessagesManager debug, EventHandler[] eventHandlers)
    {
        m_debug = debug;
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        m_positionEnd = gameObject.transform.position;
        m_scalingEnd = targetScaling;
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnScaling;
        foreach (EventHandler e in eventHandlers)
        {
            m_eventAnimationFinished += e;
        }

        gameObject.SetActive(true);

        startAnimation();
    }

    /**
     * In this function, the animation will start from a given position, and will bring it to its current position
     **/
    public void animateAppearFromPosition (Vector3 pos, MouseDebugMessagesManager debug, EventHandler e)
    {
        m_debug = debug;
        m_positionEnd = gameObject.transform.position;
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

    /**
    * In this function, the animation will start from the current position, and will bring it to the given position
    **/
    public void animateMoveToPosition(Vector3 posDest, MouseDebugMessagesManager debug, EventHandler e)
    {
        m_debug = debug;
        m_positionEnd = posDest;
        m_scalingEnd = gameObject.transform.localScale;
        m_triggerStopAnimation = MouseUtilitiesAnimation.ConditionStopAnimation.OnPositioning;
        m_eventAnimationFinished += e;

        gameObject.SetActive(true);

        startAnimation();
    }
}
