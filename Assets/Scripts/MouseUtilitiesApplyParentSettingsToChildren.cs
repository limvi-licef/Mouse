using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Settings are applied to the first level of children
 **/
public class MouseUtilitiesApplyParentSettingsToChildren : MonoBehaviour
{
    public bool m_showHideChildren;

    // Start is called before the first frame update
    void Start()
    {
        m_showHideChildren = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
