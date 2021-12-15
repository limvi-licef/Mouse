using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Todo: Make this class more generic, i.e. by using the normal of the surface, and rotating the cube to use to populate the surface by the normal.
public class MousePopulateSurfaceTableWithCubes : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public GameObject m_cubeToUseToPopulateSurface;
    public GameObject m_goToDisplayOnClick;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    public void populateTablePanel()
    {
        Vector3 goLocalPosition = gameObject.transform.localPosition;

        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Size of table panel: x=" + gameObject.GetComponent<Renderer>().bounds.size.x.ToString() + " y=" + gameObject.GetComponent<Renderer>().bounds.size.y.ToString());
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Local position of table panel: x =" + goLocalPosition.x.ToString() + " y =" + goLocalPosition.y.ToString() + " z =" + goLocalPosition.z.ToString());
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Position of table panel: x =" + gameObject.transform.position.x.ToString() + " y =" + gameObject.transform.position.y.ToString() + " z =" + gameObject.transform.position.z.ToString());

        float goScaleX = gameObject.transform.localScale.x;
        float goScaleY = gameObject.transform.localScale.y;
        float goScaleZ = gameObject.transform.localScale.z;

        float posX = 0.0f;
        float posZ = 0.0f;

        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Table panel position x=" + gameObject.transform.position.x.ToString() + " z=" + gameObject.transform.position.z.ToString());

        for (posX = 0.0f; posX < goScaleX; posX += 0.1f)
        {
            for (posZ = 0.0f; posZ < goScaleZ; posZ += 0.1f)
            {
                GameObject temp = Instantiate(m_cubeToUseToPopulateSurface);
                temp.transform.SetParent(gameObject.transform.parent, false);
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localScale = new Vector3(0.1f, 0.01f, 0.1f);
                //temp.transform.SetPositionAndRotation(new Vector3(posX, posY), temp.transform.rotation);
                float posXP = goLocalPosition.x - goLocalPosition.x / 2.0f;
                float posZP = goLocalPosition.z - goLocalPosition.z / 2.0f;

                m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Position of the cube in x=" + posXP.ToString() + " z=" + posZP.ToString());

                temp.transform.localPosition = new Vector3(/*posXP + */posX /*+ temp.transform.localScale.x / 2.0f*/, goLocalPosition.y + 0.05f, /*posZP +*/ posZ /*+ temp.transform.localScale.z / 2.0f*/);
            }
        }
        m_goToDisplayOnClick.SetActive(false);
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

    public void onClick()
    {
        m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_goToDisplayOnClick.SetActive(true);
    }
}
