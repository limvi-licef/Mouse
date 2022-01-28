using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Todo: Make this class more generic, i.e. by using the normal of the surface, and rotating the cube to use to populate the surface by the normal.
public class MouseChallengeCleanTable : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;
    public GameObject m_hologramToUseToPopulateSurface;
    public GameObject m_hologramToDisplayOnFinished;
    public GameObject m_inviteChallengeHologram;

    bool m_surfaceTouched; // Bool to detect the touch only once.

    Dictionary<Tuple<float, float>, Tuple<GameObject, bool>> m_cubesTouched;

    // Start is called before the first frame update
    void Start()
    {
        m_surfaceTouched = false;
        m_cubesTouched = new Dictionary<Tuple<float, float>, Tuple<GameObject, bool>>();

        // Sanity checks
        if (m_inviteChallengeHologram.GetComponent<MouseChallengeCleanTableInvite>() == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Start", MouseDebugMessagesManager.MessageLevel.Error, "The invite challenge hologram must have the MouseChallengeCubeInteractions component. The object will most likely crash because the implementation is not that safe.");
        }
    }

    void cubeTouched(object sender, EventArgs e)
    {
        MouseChallengeCleanTableHologramForSurfaceToClean tempCube = (MouseChallengeCleanTableHologramForSurfaceToClean)sender;

        //MouseCubeInteractions tempCube = temp.GetComponent<MouseCubeInteractions>();

        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "cubeTouched", MouseDebugMessagesManager.MessageLevel.Info, "Cube touched. Position: " + tempCube.transform.localPosition.x.ToString() + " " + tempCube.transform.localPosition.z.ToString());

        Tuple<float, float> tempTuple = new Tuple<float, float>(tempCube.transform.localPosition.x, tempCube.transform.localPosition.z);

        m_cubesTouched[tempTuple] = new Tuple<GameObject, bool>(m_cubesTouched[tempTuple].Item1, true);

        /*m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "cubeTouched", MouseDebugMessagesManager.MessageLevel.Info, "Dictionary status: ");

        foreach (KeyValuePair< Tuple<float, float>, bool> tempKeyValuePair in m_cubesTouched)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "cubeTouched", MouseDebugMessagesManager.MessageLevel.Info, tempKeyValuePair.ToString());
        }

        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "cubeTouched", MouseDebugMessagesManager.MessageLevel.Info, "Dictionary status: " + m_cubesTouched.ToString());*/

        checkIfSurfaceClean();
    }

    void inviteChallengeHologramTouched(object sender, EventArgs e)
    { // This function just relays the message by calling the appropriate function. Maybe all the code could be there.
        populateTablePanel();
    }

    public void populateTablePanel()
    {
        if (m_hologramToUseToPopulateSurface.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>() == null)
        {
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Error, "The GameObject used to populate the surface should contain the component: MouseChallengeCubeInteractions");
        }
        else
        {
            Vector3 goLocalPosition = gameObject.transform.localPosition;

            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Info, "Size of table panel: x=" + gameObject.GetComponent<Renderer>().bounds.size.x.ToString() + " y=" + gameObject.GetComponent<Renderer>().bounds.size.y.ToString());
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Info, "Local position of table panel: x =" + goLocalPosition.x.ToString() + " y =" + goLocalPosition.y.ToString() + " z =" + goLocalPosition.z.ToString());
            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "populateTablePanel", MouseDebugMessagesManager.MessageLevel.Info, "Position of table panel: x =" + gameObject.transform.position.x.ToString() + " y =" + gameObject.transform.position.y.ToString() + " z =" + gameObject.transform.position.z.ToString());

            float goScaleX = gameObject.transform.localScale.x;
            float goScaleY = gameObject.transform.localScale.y;
            float goScaleZ = gameObject.transform.localScale.z;

            float posX = 0.0f;
            float posZ = 0.0f;

            float incrementX = goScaleX / m_numberOfCubesToAddInColumn;
            float incrementZ = goScaleZ / m_numberOfCubesToAddInRow;

            m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Table panel position x=" + gameObject.transform.position.x.ToString() + " z=" + gameObject.transform.position.z.ToString());

            for (posX = 0.0f; posX < goScaleX; posX += /*0.1f*/ incrementX)
            {
                for (posZ = 0.0f; posZ < goScaleZ; posZ += /*0.1f*/ incrementZ)
                {
                    GameObject temp = Instantiate(m_hologramToUseToPopulateSurface);
                    temp.transform.SetParent(gameObject.transform.parent, false);
                    temp.transform.localPosition = Vector3.zero;
                    temp.transform.localScale = new Vector3(/*0.1f*/ incrementX, 0.01f, incrementZ);
                    //temp.transform.SetPositionAndRotation(new Vector3(posX, posY), temp.transform.rotation);
                    float posXP = posX + incrementX / 2.0f - 0.05f; /*goLocalPosition.x + goLocalPosition.x / 2.0f;*/
                    float posZP = posZ + incrementZ / 2.0f - 0.05f; /*goLocalPosition.z - goLocalPosition.z / 2.0f;*/

                    m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "Update", MouseDebugMessagesManager.MessageLevel.Info, "Position of the cube in x=" + posXP.ToString() + " z=" + posZP.ToString() + " | Size of the cube: x= " + temp.transform.localScale.x.ToString() +" z=" + temp.transform.localScale.z.ToString() );

                    temp.transform.localPosition = new Vector3(/*posXP + */posXP /*+ temp.transform.localScale.x / 2.0f*/, goLocalPosition.y + 0.05f, /*posZP +*/ posZP /*+ temp.transform.localScale.z / 2.0f*/);

                    MouseChallengeCleanTableHologramForSurfaceToClean cubeInteractions = temp.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>();
                    cubeInteractions.CubeTouchedEvent += cubeTouched;
                    m_cubesTouched.Add(new Tuple<float,float>(posXP, posZP), new Tuple<GameObject, bool>(temp, false));
                }
            }
            m_inviteChallengeHologram.SetActive(false);
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

    public void onTouch()
    {
        if (m_surfaceTouched == false) // Surface can be touched only once
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Called");
            m_inviteChallengeHologram.SetActive(true);
            MouseChallengeCleanTableInvite cubeInteractions = m_inviteChallengeHologram.GetComponent<MouseChallengeCleanTableInvite>();
            cubeInteractions.m_mouseChallengeCleanTableInviteHologramTouched += inviteChallengeHologramTouched; // Subsribe to the event so that the object is aware with the invite challenge hologram has been touched
            m_surfaceTouched = true;
        }
        
    }

    void checkIfSurfaceClean()
    {
        bool allCubesTouched = true; // By default weconsidered all cubes are touched. then we browse the values of the dictionary, and if we find a cube that has not been touched, then we set this boolean to false.

        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject,bool>> tempKeyValue in m_cubesTouched)
        {
            if (tempKeyValue.Value.Item2 == false)
            {
                allCubesTouched = false;
            }
        }

        if (allCubesTouched)
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "checkIfSurfaceClean", MouseDebugMessagesManager.MessageLevel.Info, "All cubes touched !!!!");
            closeChallenge();
            
        }
        else
        {
            m_debug.displayMessage("MouseSurfaceToPopulateWithCubes", "checkIfSurfaceClean", MouseDebugMessagesManager.MessageLevel.Info, "Still some work to do ...");
        }
    }

    void closeChallenge()
    {
        // Here we will remove the cubes from the table and display an hologram in the center of the table
        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> tempKeyValue in m_cubesTouched)
        {
            tempKeyValue.Value.Item1.SetActive(false);
        }

        m_hologramToDisplayOnFinished.SetActive(true);
    }
}
