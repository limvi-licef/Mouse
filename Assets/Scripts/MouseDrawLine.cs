using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.UI;
//using Microsoft.MixedReality.Toolkit.Input;

public class MouseDrawLine : MonoBehaviour
{
    public MouseDebugMessagesManager m_debugMessages;

    public GameObject mouseLine;
    GameObject currentLine;

    LineRenderer lineRenderer;

    List<Vector2> fingerPositions;

    //public Interactable interactable;

    // Start is called before the first frame update
    void Start()
    {
        currentLine = null;
        
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
    }

    void createLine()
    {
        currentLine = Instantiate(mouseLine, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();

        fingerPositions.Clear();

        fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

    }

    void updateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
    }

    public void onTouch()
    {
        m_debugMessages.displayMessage("MouseDrawLine", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
        Debug.Log("Helloooooooooooolilol");

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
        m_debugMessages.displayMessage("MouseDrawLine", "onFocus", MouseDebugMessagesManager.MessageLevel.Info, "Object focused");
        Debug.Log("HelloooooooooooolilolFicus");
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
