using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class MouseLineToObject : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public GameObject m_HologramTarget;
    public int m_numPoints = 1000;

    LineRenderer m_line;

    bool m_drawLine;
    //public float m_animationSpeed = 1.0f;
    //float m_delayBetweenTwoPoints = 0.1f;

    private float m_timerWaitTime = 0.006f;
    private float m_timer = 0.0f;
    float m_drawWithAnimationT; // when drawing with animation
    Vector3 m_drawWithAnimationStartingPoint;
    Vector3 m_drawWithAnimationMidPoint;
    Vector3 m_drawWithAnimationEndPoint;

    event EventHandler m_eventProcessFinished;

    private void Awake()
    {
        m_line = gameObject.GetComponent<LineRenderer>();
        m_drawLine = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_drawLine)
        {
            m_timer += Time.deltaTime;

            // Check if we have reached beyond 2 seconds.
            // Subtracting two is more accurate over time than resetting to zero.
            if (m_timer > m_timerWaitTime)
            {
                // Draw a point
                m_drawWithAnimationT += 1.0f / (float)m_numPoints;

                m_line.positionCount++;
                m_line.SetPosition(m_line.positionCount - 1, calculateQuadraticBezierPoint(m_drawWithAnimationT, m_drawWithAnimationStartingPoint, m_drawWithAnimationMidPoint, m_drawWithAnimationEndPoint));

                // Remove the recorded 2 seconds.
                m_timer = m_timer - m_timerWaitTime;
                //Time.timeScale = scrollBar;

                // Stoping the process when the line is fully drawn
                if (m_drawWithAnimationT >= 1.0f)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Finished drawing the line");
                    m_drawLine = false;

                    m_eventProcessFinished?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public void show (EventHandler eventHandler)
    {
        if (gameObject.activeSelf == false)
        {

            Vector3 endPoint = gameObject.transform.position;
            Vector3 startPoint = m_HologramTarget.transform.position;
            Vector3 midPoint = (startPoint + endPoint) / 2;
            midPoint.y += 2.0f;

            gameObject.SetActive(true);

            /*if (withAnimation == false)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Drawing line without animation - Starting point: " + startPoint.ToString() + " Mid point: " + midPoint.ToString() + " End point: " + endPoint.ToString());

                drawQuadraticCurve(startPoint, midPoint, endPoint);
            }
            else
            {*/
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Drawing line with animation - Starting point: " + startPoint.ToString() + " Mid point: " + midPoint.ToString() + " End point: " + endPoint.ToString());

            m_drawLine = true;

            m_drawWithAnimationT = 0.0f;
            m_drawWithAnimationStartingPoint = startPoint;
            m_drawWithAnimationMidPoint = midPoint;
            m_drawWithAnimationEndPoint = endPoint;

            m_line.SetPosition(0, startPoint);

            m_eventProcessFinished += eventHandler;
            //}
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Line already shown - nothing to do");
        }

    }

    void drawQuadraticCurve(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint)
    {
        float t = 0.0f;

        m_line.SetPosition(0, startPoint);
        

        for (int i = 0; i < m_numPoints; i ++)
        {
            t = (float)i / (float)m_numPoints;

            m_line.positionCount++;
            m_line.SetPosition(m_line.positionCount - 1, calculateQuadraticBezierPoint(t, startPoint, midPoint, endPoint));
        }
    }

    // Source: https://www.youtube.com/watch?v=Xwj8_z9OrFw
    Vector3 calculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)2P0 + 2(1-t)tP1 + t2P2 , 0 < t < 1
        Vector3 toReturn;

        toReturn = (1.0f - t) * (1.0f - t) * p0+2*(1-t)*t*p1+t*t*p2;

        /*m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "New point on the line: " + toReturn.ToString());*/

        return toReturn;
    }

    public void hide (EventHandler eventHandler) // Does not work with animations
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Hiding line - setting position counter to 0, so that the points will be overwritten next time it is displayed");

        /*Vector3[] temp = new Vector3[1];
        temp[0] = new Vector3(0, 0, 0);
            
        m_line.SetPositions(temp);*/
        m_line.positionCount = 1;

        //m_line.Res

        m_line.gameObject.SetActive(false);

        eventHandler?.Invoke(this, EventArgs.Empty);
    }
}
