using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public class MouseChallengeCleanTableSurfaceToPopulateWithCubes : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow = 5;
    public int m_numberOfCubesToAddInColumn = 4;

    public GameObject m_hologramToUseToPopulateSurface;

    public event EventHandler m_eventSurfaceCleaned;

    Dictionary<Tuple<float, float>, Tuple<GameObject, bool>> m_cubesTouched;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the variables
        m_cubesTouched = new Dictionary<Tuple<float, float>, Tuple<GameObject, bool>>();

        // Sanity checks
        if (m_hologramToUseToPopulateSurface.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>() == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "The m_hologramToUseToPopulateSurface object should have a MouseChallengeCleanTableHologramForSurfaceToClean component");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showInteractionCubesTablePanel(EventHandler eventHandler)
    {
        bool populateTable = false;

        if (m_cubesTouched.Count > 0)
        {
            KeyValuePair< Tuple<float, float>, Tuple<GameObject, bool>> cube = m_cubesTouched.First();
            if (cube.Value.Item1.activeSelf == false)
            {
                populateTable = true;
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "At leat one cube is already displayed, so nothing to do");
                populateTable = false;
            }
        }
        else
        {
            populateTable = true;
        }
        
        /*if (m_hologramToUseToPopulateSurface.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>() == null)
        {
            
        }
        else
        {
           
        }*/

        if (populateTable)
        {
            Vector3 goLocalPosition = gameObject.transform.localPosition;

            float goScaleX = 1.0f;
            float goScaleZ = 1.0f;

            float posX = 0.0f;
            float posZ = 0.0f;

            float incrementX = goScaleX / m_numberOfCubesToAddInColumn;
            float incrementZ = goScaleZ / m_numberOfCubesToAddInRow;

            //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Table panel position x=" + gameObject.transform.position.x.ToString() + " z=" + gameObject.transform.position.z.ToString() + " increment: x = " + incrementX.ToString() + " z = " + incrementZ.ToString());

            for (posX = 0.0f; posX < goScaleX; posX += incrementX)
            {
                for (posZ = 0.0f; posZ < goScaleZ; posZ += incrementZ)
                {
                    GameObject temp = Instantiate(m_hologramToUseToPopulateSurface);
                    temp.transform.SetParent(gameObject.transform, false);
                    temp.transform.localPosition = Vector3.zero;
                    temp.transform.localScale = new Vector3(incrementX, 0.01f, incrementZ);
                    float posXP = posX - goScaleX / 2.0f + temp.transform.localScale.x / 2.0f;
                    float posZP = posZ - goScaleZ / 2.0f + temp.transform.localScale.z / 2.0f;

                    //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Position of the cube in x=" + posXP.ToString() + " z=" + posZP.ToString() + " | Size of the cube: x= " + temp.transform.localScale.x.ToString() + " z=" + temp.transform.localScale.z.ToString());

                    temp.transform.localPosition = new Vector3(posXP, goLocalPosition.y + 3.0f, posZP);

                    MouseChallengeCleanTableHologramForSurfaceToClean cubeInteractions = temp.GetComponent<MouseChallengeCleanTableHologramForSurfaceToClean>();
                    cubeInteractions.CubeTouchedEvent += cubeTouched;
                    m_cubesTouched.Add(new Tuple<float, float>(posXP, posZP), new Tuple<GameObject, bool>(temp, false));
                    temp.SetActive(true); // Hidden by default
                }
            }

            eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }

    void cubeTouched(object sender, EventArgs e)
    {
        MouseChallengeCleanTableHologramForSurfaceToClean tempCube = (MouseChallengeCleanTableHologramForSurfaceToClean)sender;

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube touched. Position: " + tempCube.transform.localPosition.x.ToString() + " " + tempCube.transform.localPosition.z.ToString());

        Tuple<float, float> tempTuple = new Tuple<float, float>(tempCube.transform.localPosition.x, tempCube.transform.localPosition.z);

        m_cubesTouched[tempTuple] = new Tuple<GameObject, bool>(m_cubesTouched[tempTuple].Item1, true);

        checkIfSurfaceClean();
    }

    void checkIfSurfaceClean()
    {
        bool allCubesTouched = true; // By default weconsidered all cubes are touched. then we browse the values of the dictionary, and if we find a cube that has not been touched, then we set this boolean to false.

        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> tempKeyValue in m_cubesTouched)
        {
            if (tempKeyValue.Value.Item2 == false)
            {
                allCubesTouched = false;
            }
        }

        if (allCubesTouched)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "All cubes touched !!!!");
            m_eventSurfaceCleaned?.Invoke(this, EventArgs.Empty);

        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Still some work to do ...");
        }
    }

    public void resetCubesStates(EventHandler eventHandler)
    {
        /*bool hideCubes = false;

        if (m_cubesTouched.Count > 0)
        {
            KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> cube = m_cubesTouched.First();
            if (cube.Value.Item1.activeSelf == true)
            {
                hideCubes = true;
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "At least one cube is already hidden, so nothing to do");
                hideCubes = false;
            }
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No cubes to delete - nothing to do");
            hideCubes = false;
        }

        if (hideCubes == true)
        {
            // Here we will remove the cubes from the table and display an hologram in the center of the table
            foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> tempKeyValue in m_cubesTouched)
            {
                //tempKeyValue.Value.Item1.SetActive(false);
                Destroy(tempKeyValue.Value.Item1);
            }

            eventHandler?.Invoke(this, EventArgs.Empty);
        }*/

        // Here we will remove the cubes from the table and display an hologram in the center of the table
        foreach (KeyValuePair<Tuple<float, float>, Tuple<GameObject, bool>> tempKeyValue in m_cubesTouched)
        {
            //tempKeyValue.Value.Item1.SetActive(false);
            Destroy(tempKeyValue.Value.Item1);
        }

        m_cubesTouched.Clear();

        eventHandler?.Invoke(this, EventArgs.Empty);
    }
}
