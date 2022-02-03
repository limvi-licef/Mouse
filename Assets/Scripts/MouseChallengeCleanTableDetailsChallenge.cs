using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChallengeCleanTableDetailsChallenge : MonoBehaviour
{
    public GameObject m_WindowMenu;
    //public GameObject m_HandMenu;
    public MouseDebugMessagesManager m_debug;


    // Start is called before the first frame update
    void Start()
    {
        //transform.LookAt(Camera.main.transform);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Vector3.Distance(Camera.main.transform.position, transform.position) > 1 )
        {
            transform.LookAt(Camera.main.transform);
        }*/
        
    }

    public void displayMenus (bool displayWindowMenu, bool displayHandMenu)
    {
        m_debug.displayMessage("MouseChallengeCleanTableDetailsChallenge", "displayMenus", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_WindowMenu.SetActive(displayWindowMenu);
        //m_HandMenu.SetActive(displayHandMenu);
    }

    private void OnEnable()
    {
        for (int i = 0; i < gameObject.transform.childCount; i ++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
