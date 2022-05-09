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
    //public GameObject m_refCube;
    public MouseUtilitiesDisplayGraph m_graph;
    public MouseUtilitiesContextualInferences m_inferenceManager;
    //MouseUtilitiesManagerCubes m_plantsManager;

    Transform m_pointOfReferenceForPaths;

    List<Transform> m_plants;
    List<GameObject> m_plantsLines;

    MouseUtilitiesGradationAssistanceManager m_stateMachine;

    EventHandler s_inference19h00;
    MouseUtilitiesInferenceTime m_inference19h00;

    EventHandler s_dialogFirstUserFar;
    EventHandler s_dialogSecondUserFar;
    EventHandler s_dialogThirdUserFar;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_plants = new List<Transform>();
        m_plantsLines = new List<GameObject>();
        m_stateMachine = new MouseUtilitiesGradationAssistanceManager();

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
        // Object required
        m_pointOfReferenceForPaths = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface interactionSurfaceController = m_pointOfReferenceForPaths.GetComponent<MouseInteractionSurface>();
        interactionSurfaceController.setColor("Mouse_Purple_Glowing");
        interactionSurfaceController.setScaling(new Vector3(0.6f, 0.01f, 0.4f));
        m_pointOfReferenceForPaths.position = new Vector3(0.846999645f, 0.542999983f, 5.22099972f);
        interactionSurfaceController.setAdminButtons("plants");
        interactionSurfaceController.showInteractionSurfaceTable(true);
        interactionSurfaceController.setPreventResizeY(true);
        interactionSurfaceController.setObjectResizable(true);

        /*GameObject successView = Instantiate(m_refCube, m_pointOfReferenceForPaths);
        MouseAssistanceBasic successController = successView.GetComponent<MouseAssistanceBasic>();
        successController.setMaterialToChild("Mouse_Congratulation");*/
        MouseAssistanceBasic successController = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Congratulation", m_pointOfReferenceForPaths);

        // Setting 3 cubes to evaluate if the intermediate state thing works
        Transform plant1View = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface plant1Controller = plant1View.GetComponent<MouseInteractionSurface>();
        plant1Controller.setColor("Mouse_Yellow_Glowing");
        plant1Controller.setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        plant1View.position = new Vector3(3.31499958f, 0.347000003f, 5.22099972f);
        plant1Controller.setAdminButtons("plant 1");
        plant1Controller.showInteractionSurfaceTable(true);
        plant1Controller.setPreventResizeY(true);
        plant1Controller.setObjectResizable(true);

        Transform plant2View = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface plant2Controller = plant2View.GetComponent<MouseInteractionSurface>();
        plant2Controller.setColor("Mouse_Yellow_Glowing");
        plant2Controller.setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        plant2View.position = new Vector3(-1.38000059f, 0.35800001f, 1.05400014f);
        plant2Controller.setAdminButtons("plant 2");
        plant2Controller.showInteractionSurfaceTable(true);
        plant2Controller.setPreventResizeY(true);
        plant2Controller.setObjectResizable(true);
        
        Transform plant3View = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface plant3Controller = plant3View.GetComponent<MouseInteractionSurface>();
        plant3Controller.setColor("Mouse_Yellow_Glowing");
        plant3Controller.setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        plant3View.position = new Vector3(3.35499954f, 0.331999987f, 1.5990001f);
        plant3Controller.setAdminButtons("plant 3");
        plant3Controller.showInteractionSurfaceTable(true);
        plant3Controller.setPreventResizeY(true);
        plant3Controller.setObjectResizable(true);

        Transform faucetView = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface faucetController = faucetView.GetComponent<MouseInteractionSurface>();
        faucetController.setColor("Mouse_Cyan_Glowing");
        faucetController.setScaling(new Vector3(0.1f, 0.1f, 0.1f));
        faucetView.position = new Vector3(0.948000014f, 0.592000008f, 5.36800003f);
        faucetController.setAdminButtons("faucet");
        faucetController.showInteractionSurfaceTable(true);
        faucetController.setPreventResizeY(false);
        faucetController.setObjectResizable(true);

        // First dialog
        MouseAssistanceDialog dialogFirst = MouseUtilitiesAssistancesFactory.Instance.createDialogNoButton("", "Qu'est-ce qu'il est conseillé de faire en fin de journée quand il fait moins chaud?", m_pointOfReferenceForPaths);

        // Second dialog
        MouseAssistanceDialog dialogSecond = MouseUtilitiesAssistancesFactory.Instance.createDialogNoButton("", "Il n'y a pas que vous qui avez soif!", m_pointOfReferenceForPaths);

        // Third dialog
        MouseAssistanceDialog dialogThird = MouseUtilitiesAssistancesFactory.Instance.createDialogNoButton("", "Il est temps d'arroser vos plantes", m_pointOfReferenceForPaths);

        //// Inferences
        DateTime tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
        m_inference19h00 = new MouseUtilitiesInferenceTime("19h watering plants", tempTime, delegate(System.Object o, EventArgs e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

            m_inferenceManager.unregisterInference("19h watering plants");
            s_inference19h00?.Invoke(o, e);
        });
        m_inferenceManager.registerInference(m_inference19h00);

        /*MouseUtilitiesInferenceDistanceFromObject inferenceDistanceDialogFirst = new MouseUtilitiesInferenceDistanceFromObject("inferenceDistanceFirstDialog", delegate (System.Object o, EventArgs e)
        {
            m_inferenceManager.unregisterInference("inferenceDistanceFirstDialog");
            s_dialogFirstUserFar?.Invoke(o, e);
        }, dialogFirst.gameObject, 2.0f);
        m_inferenceManager.registerInference(inferenceDistanceDialogFirst);*/

        /*MouseUtilitiesInferenceDistanceFromObject inferenceDistanceDialogSecond = new MouseUtilitiesInferenceDistanceFromObject("inferenceDistanceSecondDialog", delegate (System.Object o, EventArgs e)
        {
            m_inferenceManager.unregisterInference("inferenceDistanceSecondDialog");
            s_dialogSecondUserFar?.Invoke(o, e);
        }, dialogSecond.gameObject, 2.0f);
        m_inferenceManager.registerInference(inferenceDistanceDialogSecond);*/
        /*MouseUtilitiesContextualInferencesFactory.Instance.createDistanceInferenceOneShot(m_inferenceManager, "inferenceDistanceSecondDialog", s_dialogSecondUserFar, dialogSecond.gameObject);*/

        /*MouseUtilitiesContextualInferencesFactory.Instance.createDistanceInferenceOneShot(m_inferenceManager, "inferenceDistanceThirdDialog", s_dialogThirdUserFar, dialogThird.gameObject);*/

        //// State machine
        MouseUtilitiesGradationAssistance sStandBy = m_stateMachine.addNewAssistanceGradation("StandBy");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceManager.registerInference(m_inference19h00);
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sDialogFirst = m_stateMachine.addNewAssistanceGradation("DialogFirst");
        sDialogFirst.setFunctionHideAndShow(dialogFirst);
        sDialogFirst.addFunctionShow(delegate (EventHandler e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "inferenceDistanceFirstDialog", s_dialogFirstUserFar, dialogSecond.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sDialogSecond = m_stateMachine.addNewAssistanceGradation("DialogSecond");
        sDialogSecond.setFunctionHideAndShow(dialogSecond);
        sDialogSecond.addFunctionShow(delegate (EventHandler e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "inferenceDistanceSecondDialog", s_dialogSecondUserFar, dialogSecond.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sDialogThird = m_stateMachine.addNewAssistanceGradation("DialogThird");
        sDialogThird.setFunctionHideAndShow(dialogThird);
        sDialogThird.addFunctionShow(delegate (EventHandler e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "inferenceDistanceThirdDialog", s_dialogThirdUserFar, dialogThird.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sPlant1 = m_stateMachine.addNewAssistanceGradation("Plant1");
        sPlant1.addFunctionShow(delegate (EventHandler e)
        {
            
        }, MouseUtilities.getEventHandlerEmpty());
        sPlant1.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        MouseUtilitiesGradationAssistance sPlant2 = m_stateMachine.addNewAssistanceGradation("Plant2");
        sPlant2.addFunctionShow(delegate (EventHandler e)
        {

        }, MouseUtilities.getEventHandlerEmpty());
        sPlant2.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        MouseUtilitiesGradationAssistance sPlant3 = m_stateMachine.addNewAssistanceGradation("Plant3");
        sPlant3.addFunctionShow(delegate (EventHandler e)
        {

        }, MouseUtilities.getEventHandlerEmpty());
        sPlant3.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sSuccess = m_stateMachine.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(successController);

        MouseUtilitiesGradationAssistanceIntermediateState sIntermediateWateringPlants = m_stateMachine.addIntermediateState("WateringPlants", sSuccess);

        // Set transitions and intermediate state
        s_inference19h00 += sStandBy.goToState(sDialogFirst);
        s_dialogFirstUserFar += sDialogFirst.goToState(sDialogSecond);
        s_dialogSecondUserFar += sDialogSecond.goToState(sDialogThird);

        faucetController.m_eventInteractionSurfaceTableTouched += sStandBy.goToState(sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogFirst.goToState(sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogSecond.goToState(sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogThird.goToState(sIntermediateWateringPlants);

        plant1Controller.m_eventInteractionSurfaceTableTouched += sIntermediateWateringPlants.addState(sPlant1);
        plant2Controller.m_eventInteractionSurfaceTableTouched += sIntermediateWateringPlants.addState(sPlant2);
        plant3Controller.m_eventInteractionSurfaceTableTouched += sIntermediateWateringPlants.addState(sPlant3);

        //sIntermediateWateringPlants.

        // Initial state
        m_stateMachine.setGradationInitial("StandBy");

        // Display graph
        m_graph.setManager(m_stateMachine);
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
