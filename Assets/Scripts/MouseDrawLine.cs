using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.UI;
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

    //HandTrackingInputEventData m_eventToCreateNewLine;

     //public Interactable interactable;

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
        /*if(Input.)
        {
            createLine();
        }
        if (0)
        {
            Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count -1]) > .1f)
            {
                updateLine(tempFingerPos);
            }
        }*/

        if (m_status == States.CreateNewLine)
        {
            
            //throw new System.NotImplementedException();


            //createLine(xinplane, zinplane);
            //m_lines.Add(Instantiate(m_linePrefab, Vector3.zero, Quaternion.identity));

            //GameObject temp = Instantiate(m_linePrefab);
            //m_lines.Add(temp);

            //LineRenderer lineRenderer = m_lines.Last().GetComponent<LineRenderer>();

            //Vector3 posLine = new Vector3(xinplane, gameObject.transform.position.y + (float)0.02, zinplane);

            //lineRenderer.SetPosition(0, posLine);

            m_status = States.NotInitialized;
        }

        
    }

    void createLine(float posx, float posz)
    {
        //m_lines.Add(Instantiate(m_linePrefab, Vector3.zero, Quaternion.identity));
        Vector3 posLine = new Vector3(posx, gameObject.transform.position.y + m_lineYOffset, posz);

        /*float tiltAroundZ = 90.0f;
        Quaternion target = Quaternion.Euler(0, 0, tiltAroundZ);*/

        //m_lines.Add(Instantiate(m_linePrefab,Vector3.zero, Quaternion.identity));
        m_lines.Add(Instantiate(m_linePrefab, gameObject.transform, true ));
        //GameObject temp = Instantiate(m_linePrefab);


        LineRenderer lineRenderer = m_lines.Last().GetComponent<LineRenderer>();

        //lineRenderer.transform.Rotate(Vector3.left, 90.0f);

        
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

    public void onTouch()
    {
       m_debugMessages.displayMessage("MouseDrawLine", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
        //Debug.Log("Helloooooooooooolilol");

        /*IMixedRealityHand hand = gameObject.GetComponent<IMixedRealityHand>();

        if (hand != null)
        {
            m_debugMessages.displayMessage("MouseDrawLine", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Hand detected!");
        }
        else
        {
            m_debugMessages.displayMessage("MouseDrawLine", "onTouch", MouseDebugMessagesManager.MessageLevel.Warning, "Hand NOT detected ...");
        }*/

        /*NearInteractionTouchableUnityUI temp = gameObject.GetComponent<NearInteractionTouchableUnityUI>();
        m_debugMessages.displayMessage("MouseDrawLine", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, temp.LocalPressDirection.ToString());
        */
        //if ()

        /*if (currentLine == null)
        {
            createLine();
        }
        else
        {

        }*/
    }

    public void onFocus()
    {
        /*m_debugMessages.displayMessage("MouseDrawLine", "onFocus", MouseDebugMessagesManager.MessageLevel.Info, "Object focused");
        Debug.Log("HelloooooooooooolilolFicus");*/
    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        float xorigin = gameObject.transform.position.x - renderer.bounds.size.x / (float)2.0;
        float zorigin = gameObject.transform.position.z - renderer.bounds.size.z / (float)2.0;


        //m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Witdh: " + renderer.bounds.size.x.ToString() + " Height: " + renderer.bounds.size.z.ToString());

        float xinplane = eventData.InputData.x - xorigin;
        float zinplane = eventData.InputData.z - zorigin;

        //m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, xinplane.ToString() + " " + zinplane.ToString());

        //createLine(xinplane, zinplane);

        createLine(eventData.InputData.x, eventData.InputData.z);

        // Set the object status to "Create a new line" so that the next "update" function call will be requested to create a new line.
        //m_eventToCreateNewLine = eventData;
        //m_status = States.CreateNewLine;
    }

    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchCompleted", MouseDebugMessagesManager.MessageLevel.Info, "");
        //throw new System.NotImplementedException();
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        //m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchUpdated", MouseDebugMessagesManager.MessageLevel.Info, "");
        //throw new System.NotImplementedException();

        Renderer renderer = gameObject.GetComponent<Renderer>();
        float xorigin = gameObject.transform.position.x - renderer.bounds.size.x / (float)2.0;
        float zorigin = gameObject.transform.position.z - renderer.bounds.size.z / (float)2.0;


        //m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchUpdated", MouseDebugMessagesManager.MessageLevel.Info, "Witdh: " + renderer.bounds.size.x.ToString() + " Height: " + renderer.bounds.size.z.ToString());

        float xinplane = eventData.InputData.x - xorigin;
        float zinplane = eventData.InputData.z - zorigin;

        //m_debugMessages.displayMessage("MouseDrawLine", "IMixedRealityTouchHandler.OnTouchUpdated", MouseDebugMessagesManager.MessageLevel.Info, xinplane.ToString() + " " + zinplane.ToString());
        //throw new System.NotImplementedException();

        //addPointToCurrentLine(xinplane, zinplane);

        addPointToCurrentLine(eventData.InputData.x, eventData.InputData.z);
    }

    /*public void onTouched(UnityEngine.Events.UnityEvent ev)
    {
        m_debugMessages.displayMessage("MouseDrawLine", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");

        if (currentLine == null)
        {
            createLine();
        }

    }*/
}
