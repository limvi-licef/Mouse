using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

public class MouseChallengeWateringThePlants : MonoBehaviour
{
    public MouseUtilitiesPathFinding m_pathFinderEngine;
    public Transform m_refInteractionSurface;
    //MouseUtilitiesManagerCubes m_plantsManager;

    Transform m_pointOfReferenceForPaths;

    List<Transform> m_plants;
    List<GameObject> m_plantsLines;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_plants = new List<Transform>();
        m_plantsLines = new List<GameObject>();

        // Get children and components
        //m_plantsManager = GetComponent<MouseUtilitiesManagerCubes>();

        // Add buttons to admin menu
        MouseUtilitiesAdminMenu.Instance.addButton("Add plant", callbackAddPlant);



        // Initialize scenario
        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackAddPlant()
    {
        //m_plantsManager.addCube("Plant " + (m_plantsManager.getCubes().Count + 1).ToString(), new Vector3(0.1f, 0.1f, 0.1f), new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z), "Mouse_Orange_Glowing", false, true, transform);
        Transform plantView = Instantiate(m_refInteractionSurface, transform);

        m_plants.Add(plantView);
        int indexPlant = m_plants.Count - 1;

        MouseInteractionSurface plantController = plantView.GetComponent<MouseInteractionSurface>();
        plantController.setAdminButtons("Plant " + (m_plants.Count).ToString());
        plantController.setScaling(new Vector3(0.1f, 0.1f, 0.1f));
        plantController.setColor("Mouse_Orange_Glowing");
        plantController.s_interactionSurfaceMoved += delegate (System.Object o, EventArgs e)
        { // Maybe this part will have to be updated , in the case I add a functionality to remove plants (which is unlokely but we never know).
            drawLineForPlant(indexPlant);
        };
        plantController.showInteractionSurfaceTable(true);
        plantController.setObjectResizable(true);
        

        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

        GameObject gameObjectForLine = new GameObject("Line " + (m_plants.Count).ToString());
        gameObjectForLine.transform.parent = transform;
        LineRenderer lineRenderer = gameObjectForLine.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.017f;
        lineRenderer.endWidth = 0.017f;
        lineRenderer.material = Resources.Load("Mouse_Green_Glowing", typeof(Material)) as Material;

        
        m_plantsLines.Add(gameObjectForLine);
    }

    void initializeScenario()
    {
        m_pointOfReferenceForPaths = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface interactionSurfaceController = m_pointOfReferenceForPaths.GetComponent<MouseInteractionSurface>();
        interactionSurfaceController.setColor("Mouse_Purple_Glowing");
        //interactionSurfaceController.setScaling(new Vector3(0.1f, 0.02f, 0.1f));
        interactionSurfaceController.setAdminButtons("plants");
        interactionSurfaceController.setPreventResizeY(true);
        interactionSurfaceController.setObjectResizable(true);
        interactionSurfaceController.showInteractionSurfaceTable(true);
    }

    void drawLineForPlant(int index)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        Vector3[] corners = m_pathFinderEngine.computePath(m_pointOfReferenceForPaths, m_plants[index].transform);

        LineRenderer lineRenderer = m_plantsLines[index].GetComponent<LineRenderer>();
        lineRenderer.positionCount = corners.Length;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 corner = corners[i];

            lineRenderer.SetPosition(i, corner);

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Corner : " + corner);
        }
    }
}
