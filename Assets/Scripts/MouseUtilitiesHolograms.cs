using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Settings are applied to the first level of children
 **/
public class MouseUtilitiesHolograms : MonoBehaviour
{
    public bool m_showHideChildren = false;
    public bool m_lookAtUser = false;

    public bool m_useHeadHeightForPlacement = false; // Means that when the hologram becomes active, the hologram's height is adjusted to head's height

    bool m_headHeightAdjusted;

    // Start is called before the first frame update
    void Start()
    {
        m_headHeightAdjusted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lookAtUser)
        {
            if (Vector3.Distance(Camera.main.transform.position, transform.position) > 1)
            {
                gameObject.transform.LookAt(Camera.main.transform);
            }
        }

        if ( m_useHeadHeightForPlacement )
        {
            // The adjustment is made only the first frame when the hologram is made active
            if (gameObject.activeSelf && m_headHeightAdjusted == false)
            {
                m_headHeightAdjusted = true;
                gameObject.transform.localPosition = new Vector3(0, gameObject.transform.InverseTransformPoint(Camera.main.transform.position).y, 0);
            }
            else if (gameObject.activeSelf == false && m_headHeightAdjusted == true)
            {
                m_headHeightAdjusted = false;
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

    /*
 * The position is given in parameter, as the "reference" position is the responsibility of the parent gameobject (most likely the one that is the chief orchestra, i.e. the responsible of the challenge)
 * */
    public void resetHologram(Vector3 position)
    {
        // Deactivate the window
        gameObject.SetActive(false);

        // Setting back the scaling and position to 1.0
        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        gameObject.transform.localPosition = position;

        Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
    }
}
