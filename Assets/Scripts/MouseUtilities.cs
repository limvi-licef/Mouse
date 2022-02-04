using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Settings are applied to the first level of children
 **/
public class MouseUtilities : MonoBehaviour
{
    public bool m_showHideChildren;
    public bool m_lookAtUser;

    // Start is called before the first frame update
    void Start()
    {
        m_showHideChildren = false;
        m_lookAtUser = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lookAtUser)
        {
            if (Vector3.Distance(Camera.main.transform.position, transform.position) > 1 )
            {
                gameObject.transform.LookAt(Camera.main.transform);
            }
        }
    }

    private void OnEnable()
    {
        if (m_showHideChildren)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        
    }

    private void OnDisable()
    {
        if (m_showHideChildren)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
    }
}
