using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSurfaceToPopulateWithCubes : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public GameObject m_cubeToUseToPopulateSurface;

    // Start is called before the first frame update
    void Start()
    {
        
        m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Size of cube: x=" + gameObject.GetComponent<Renderer>().bounds.size.x.ToString() + " y=" + gameObject.GetComponent<Renderer>().bounds.size.y.ToString());

        float posX = 0.0f;
        float posY = 0.0f;
        for (posX = 0.0f; posX < 0.3f; posX += 0.1f)
        {
            for (posY = 0.0f; posY < 0.3f; posY += 0.1f)
            {
                GameObject temp = Instantiate(m_cubeToUseToPopulateSurface);
                temp.transform.SetParent(gameObject.transform.parent, false);
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                //temp.transform.SetPositionAndRotation(new Vector3(posX, posY), temp.transform.rotation);
                float posXP = gameObject.transform.position.x - gameObject.transform.localScale.x / 2.0f;
                float posYP = gameObject.transform.position.y - gameObject.transform.localScale.y / 2.0f;
                temp.transform.localPosition = new Vector3(posXP + posX + temp.transform.localScale.x / 2.0f, posYP + posY + temp.transform.localScale.y / 2.0f, gameObject.transform.position.z - 0.05f);
            }
        }
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
}
