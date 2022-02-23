using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using System.Linq;

public class MouseDrawLine : MonoBehaviour, IMixedRealityTouchHandler
{
    public MouseDebugMessagesManager m_debugMessages;

    public GameObject m_linePrefab;

    List<GameObject> m_lines;

    float m_lineYOffset;

    enum States
    {
        CreateNewLine,
        AddPointToCurrentLine,
        NotInitialized
    }

    States m_status;

    // Start is called before the first frame update
    void Start()
    {
        m_status = States.NotInitialized;
        m_lines = new List<GameObject>();
        m_lineYOffset = 0.07f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_status == States.CreateNewLine)
        {
            m_status = States.NotInitialized;
        }

        
    }

    void createLine(float posx, float posz)
    {
        Vector3 posLine = new Vector3(posx, gameObject.transform.position.y + m_lineYOffset, posz);

        m_lines.Add(Instantiate(m_linePrefab, gameObject.transform, true ));

        LineRenderer lineRenderer = m_lines.Last().GetComponent<LineRenderer>();      
        lineRenderer.SetPosition(0, posLine);
        lineRenderer.SetPosition(1, posLine);

        m_debugMessages.displayMessage("MouseDrawLine", "createLine", MouseDebugMessagesManager.MessageLevel.Info, "Number of lines in the list: " + m_lines.Count.ToString() + " Index position: " + lineRenderer.positionCount.ToString());

    }

    void addPointToCurrentLine(float posx, float posz)
    {
        LineRenderer lineRenderer = m_lines.Last().GetComponent<LineRenderer>();
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(posx, gameObject.transform.position.y + m_lineYOffset, posz));

        m_debugMessages.displayMessage("MouseDrawLine", "addPointToCurrentLine", MouseDebugMessagesManager.MessageLevel.Info, "Number of lines in the list: " + m_lines.Count.ToString() + " Index position: " + lineRenderer.positionCount.ToString());
    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        float xorigin = gameObject.transform.position.x - renderer.bounds.size.x / (float)2.0;
        float zorigin = gameObject.transform.position.z - renderer.bounds.size.z / (float)2.0;

        float xinplane = eventData.InputData.x - xorigin;
        float zinplane = eventData.InputData.z - zorigin;

        createLine(eventData.InputData.x, eventData.InputData.z);
    }

    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchCompleted", MouseDebugMessagesManager.MessageLevel.Info, "");
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        float xorigin = gameObject.transform.position.x - renderer.bounds.size.x / (float)2.0;
        float zorigin = gameObject.transform.position.z - renderer.bounds.size.z / (float)2.0;

        float xinplane = eventData.InputData.x - xorigin;
        float zinplane = eventData.InputData.z - zorigin;

        addPointToCurrentLine(eventData.InputData.x, eventData.InputData.z);
    }
}
