/*Copyright 2022 Guillaume Spalla

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

public class MouseChallengeWateringThePlants : MonoBehaviour
{
    public MouseUtilitiesPathFinding m_pathFinderEngine;
    public Transform m_refInteractionSurface;
    //public GameObject m_refCube;
    public MouseUtilitiesDisplayGraph m_graph;
    public MouseUtilitiesContextualInferences m_inferenceManager;
    //MouseUtilitiesManagerCubes m_plantsManager;

    Transform m_pointOfReferenceForPaths;

    List<Transform> m_plantsView;
    List<EventHandler> m_plantsEventHandler;
    List<GameObject> m_plantsLines;
    List<MouseUtilitiesGradationAssistance> m_plantsStates;
    List<MouseInteractionSurface> m_plantsControllers;

    MouseUtilitiesGradationAssistanceManager m_stateMachineMain;
    MouseUtilitiesGradationAssistanceManager m_stateMachineAssistanceWateringPlants;

    EventHandler s_inference19h00;
    MouseUtilitiesInferenceTime m_inference19h00;

    EventHandler s_dialogFirstUserFar;
    EventHandler s_dialogSecondUserFar;
    EventHandler s_dialogThirdUserFar;
    EventHandler s_assistanceWaterNoPlantWatered;
    EventHandler s_assistanceWaterNeedHelp;
    EventHandler s_assistanceWaterNoNeedForHelp;

    int m_numberOfPlantsWatered = 0;

    MouseUtilitiesGradationAssistanceIntermediateState m_sIntermediateWateringPlants; // This particular state is a class variable as it is used in a callback

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_plantsView = new List<Transform>();
        m_plantsLines = new List<GameObject>();
        m_plantsEventHandler = new List<EventHandler>();
        m_stateMachineMain = new MouseUtilitiesGradationAssistanceManager();
        m_stateMachineAssistanceWateringPlants = new MouseUtilitiesGradationAssistanceManager();
        m_plantsStates = new List<MouseUtilitiesGradationAssistance>();
        m_plantsControllers = new List<MouseInteractionSurface>();

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
        m_plantsView.Add(Instantiate(m_refInteractionSurface, transform));

        int indexPlant = m_plantsView.Count - 1;

        string plantId = "Plant " + (m_plantsView.Count).ToString();

        m_plantsControllers.Add(m_plantsView.Last().GetComponent<MouseInteractionSurface>());
        m_plantsControllers.Last().setAdminButtons(plantId);
        m_plantsControllers.Last().setScaling(new Vector3(0.1f, 0.1f, 0.1f));
        m_plantsControllers.Last().setColor("Mouse_Yellow_Glowing");
        m_plantsControllers.Last().s_interactionSurfaceMoved += delegate (System.Object o, EventArgs e)
        { // Maybe this part will have to be updated , in the case I add a functionality to remove plants (which is unlokely but we never know).
            drawLineForPlant(indexPlant);
        };
        m_plantsControllers.Last().showInteractionSurfaceTable(true);
        m_plantsControllers.Last().setObjectResizable(true);
        

        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

        GameObject gameObjectForLine = new GameObject("Line " + (m_plantsView.Count).ToString());
        gameObjectForLine.transform.parent = transform;
        LineRenderer lineRenderer = gameObjectForLine.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.017f;
        lineRenderer.endWidth = 0.017f;
        lineRenderer.material = Resources.Load("Mouse_Green_Glowing", typeof(Material)) as Material;
        
        m_plantsLines.Add(gameObjectForLine);

        // Adding event handler to draw the line
        m_plantsEventHandler.Add(delegate (System.Object o, EventArgs e)
        {
            drawLineForPlant(indexPlant);
        });

        // Adding the information to the state machine and the interactions
        m_plantsStates.Add(m_stateMachineMain.addNewAssistanceGradation(plantId));
        m_plantsStates.Last().addFunctionShow(delegate (EventHandler e)
        {}, MouseUtilities.getEventHandlerEmpty());
        m_plantsStates.Last().setFunctionHide(delegate (EventHandler e)
        {e?.Invoke(this, EventArgs.Empty);}, MouseUtilities.getEventHandlerEmpty());

        m_plantsControllers.Last().m_eventInteractionSurfaceTableTouched += delegate (System.Object o, EventArgs e)
        {
            m_plantsControllers[indexPlant].setColor("Mouse_Green_Glowing");
            m_plantsLines[indexPlant].GetComponent<LineRenderer>().positionCount = 0;
            m_sIntermediateWateringPlants.addState(m_plantsStates[indexPlant]);
        };
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
        callbackAddPlant();
        callbackAddPlant();
        callbackAddPlant();
        MouseInteractionSurface plant1Controller = m_plantsView[0].GetComponent<MouseInteractionSurface>();
        MouseInteractionSurface plant2Controller = m_plantsView[1].GetComponent<MouseInteractionSurface>();
        MouseInteractionSurface plant3Controller = m_plantsView[2].GetComponent<MouseInteractionSurface>();

        //Transform plant1View = Instantiate(m_refInteractionSurface, transform);
        //MouseInteractionSurface plant1Controller = plant1View.GetComponent<MouseInteractionSurface>();
        plant1Controller.setColor("Mouse_Yellow_Glowing");
        plant1Controller.setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        m_plantsView[0].position = new Vector3(3.31499958f, 0.347000003f, 5.22099972f);
        plant1Controller.setAdminButtons("plant 1");
        plant1Controller.showInteractionSurfaceTable(true);
        plant1Controller.setPreventResizeY(true);
        plant1Controller.setObjectResizable(true);

        //Transform plant2View = Instantiate(m_refInteractionSurface, transform);
        //MouseInteractionSurface plant2Controller = plant2View.GetComponent<MouseInteractionSurface>();
        plant2Controller.setColor("Mouse_Yellow_Glowing");
        plant2Controller.setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        m_plantsView[1].position = new Vector3(-1.38000059f, 0.35800001f, 1.05400014f);
        plant2Controller.setAdminButtons("plant 2");
        plant2Controller.showInteractionSurfaceTable(true);
        plant2Controller.setPreventResizeY(true);
        plant2Controller.setObjectResizable(true);
        
        //Transform plant3View = Instantiate(m_refInteractionSurface, transform);
        //MouseInteractionSurface plant3Controller = plant3View.GetComponent<MouseInteractionSurface>();
        plant3Controller.setColor("Mouse_Yellow_Glowing");
        plant3Controller.setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        m_plantsView[2].position = new Vector3(3.35499954f, 0.331999987f, 1.5990001f);
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

        // Dialog to provide assistance to water the plants
        MouseAssistanceDialog dialogAssistanceWaterProposeHelp = MouseUtilitiesAssistancesFactory.Instance.createDialogTwoButtons("", "Besoin d'aide?", "Oui", delegate(System.Object o, EventArgs e) { s_assistanceWaterNeedHelp?.Invoke(o, e); }, "Non", delegate (System.Object o, EventArgs e) { s_assistanceWaterNoNeedForHelp?.Invoke(o, e); },  m_pointOfReferenceForPaths);

        MouseAssistanceDialog dialogAssistanceWaterHelp = MouseUtilitiesAssistancesFactory.Instance.createDialogTwoButtons("", "Voici les plantes qu'il vous reste à arroser. Si vous touchez une des plantes, un chemin au sol vous y guidera.", "Plante 1", m_plantsEventHandler[0],  "Plante 2", m_plantsEventHandler[1],  m_pointOfReferenceForPaths);

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

        //// Asssitance to water the plants state machine
        MouseUtilitiesGradationAssistance sAssistanceWaterPlantsStandBy = m_stateMachineAssistanceWateringPlants.addNewAssistanceGradation("standBy");
        sAssistanceWaterPlantsStandBy.addFunctionShow(delegate (EventHandler e)
        {}, MouseUtilities.getEventHandlerEmpty());
        sAssistanceWaterPlantsStandBy.setFunctionHide(delegate (EventHandler e)
        { e?.Invoke(this, EventArgs.Empty); }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sAssistanceWaterStart = m_stateMachineAssistanceWateringPlants.addNewAssistanceGradation("ready");
        sAssistanceWaterStart.addFunctionShow(delegate (EventHandler e)
        {
                                    MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceManager, "AssistanceWateringLeavingAndComing", callbackAssistanceWateringCheckIfPlantWatered, m_pointOfReferenceForPaths.gameObject);

        }, MouseUtilities.getEventHandlerEmpty());
        sAssistanceWaterStart.setFunctionHide(delegate (EventHandler e)
        {e?.Invoke(this, EventArgs.Empty);}, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sAssistanceWaterDialogFirst = m_stateMachineAssistanceWateringPlants.addNewAssistanceGradation("dialogFirst");
        sAssistanceWaterDialogFirst.setFunctionHideAndShow(dialogAssistanceWaterProposeHelp);

        MouseUtilitiesGradationAssistance sAssistanceWaterDialogSecond = m_stateMachineAssistanceWateringPlants.addNewAssistanceGradation("dialogSecond");
        sAssistanceWaterDialogSecond.setFunctionHideAndShow(dialogAssistanceWaterHelp);

        m_stateMachineAssistanceWateringPlants.setGradationInitial("standBy");
        //m_s

        //// Main State machine
        MouseUtilitiesGradationAssistance sStandBy = m_stateMachineMain.addNewAssistanceGradation("StandBy");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceManager.registerInference(m_inference19h00);
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sDialogFirst = m_stateMachineMain.addNewAssistanceGradation("DialogFirst");
        sDialogFirst.setFunctionHideAndShow(dialogFirst);
        sDialogFirst.addFunctionShow(delegate (EventHandler e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "inferenceDistanceFirstDialog", s_dialogFirstUserFar, dialogSecond.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sDialogSecond = m_stateMachineMain.addNewAssistanceGradation("DialogSecond");
        sDialogSecond.setFunctionHideAndShow(dialogSecond);
        sDialogSecond.addFunctionShow(delegate (EventHandler e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "inferenceDistanceSecondDialog", s_dialogSecondUserFar, dialogSecond.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sDialogThird = m_stateMachineMain.addNewAssistanceGradation("DialogThird");
        sDialogThird.setFunctionHideAndShow(dialogThird);
        sDialogThird.addFunctionShow(delegate (EventHandler e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "inferenceDistanceThirdDialog", s_dialogThirdUserFar, dialogThird.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sSuccess = m_stateMachineMain.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(successController);

        m_sIntermediateWateringPlants = m_stateMachineMain.addIntermediateState("WateringPlants", sSuccess);

        /*MouseUtilitiesGradationAssistance sPlant1 = m_stateMachineMain.addNewAssistanceGradation("Plant1");
        sPlant1.addFunctionShow(delegate (EventHandler e)
        {
        }, MouseUtilities.getEventHandlerEmpty());
        sPlant1.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());*/

        /*MouseUtilitiesGradationAssistance sPlant2 = m_stateMachineMain.addNewAssistanceGradation("Plant2");
        sPlant2.addFunctionShow(delegate (EventHandler e)
        {

        }, MouseUtilities.getEventHandlerEmpty());
        sPlant2.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());*/

       /* MouseUtilitiesGradationAssistance sPlant3 = m_stateMachineMain.addNewAssistanceGradation("Plant3");
        sPlant3.addFunctionShow(delegate (EventHandler e)
        {

        }, MouseUtilities.getEventHandlerEmpty());
        sPlant3.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());*/

        // Set transitions and intermediate state
        s_inference19h00 += sStandBy.goToState(sDialogFirst);
        s_dialogFirstUserFar += sDialogFirst.goToState(sDialogSecond);
        s_dialogSecondUserFar += sDialogSecond.goToState(sDialogThird);

        faucetController.m_eventInteractionSurfaceTableTouched += sStandBy.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogFirst.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogSecond.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogThird.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sAssistanceWaterPlantsStandBy.goToState(sAssistanceWaterStart); // Enabling the nested state machine

        /*plant1Controller.m_eventInteractionSurfaceTableTouched += delegate (System.Object o, EventArgs e)
        {
            plant1Controller.setColor("Mouse_Green_Glowing");
            m_sIntermediateWateringPlants.addState(sPlant1);
        };*/
        //plant2Controller.m_eventInteractionSurfaceTableTouched += m_sIntermediateWateringPlants.addState(sPlant2);
        //plant3Controller.m_eventInteractionSurfaceTableTouched += m_sIntermediateWateringPlants.addState(sPlant3);

        s_assistanceWaterNoPlantWatered += sAssistanceWaterStart.goToState(sAssistanceWaterDialogFirst);
        s_assistanceWaterNeedHelp += sAssistanceWaterDialogFirst.goToState(sAssistanceWaterDialogSecond);
        s_assistanceWaterNoNeedForHelp += sAssistanceWaterDialogFirst.goToState(sAssistanceWaterPlantsStandBy);
        m_sIntermediateWateringPlants.s_eventNextState += sAssistanceWaterDialogFirst.goToState(sAssistanceWaterPlantsStandBy);
        m_sIntermediateWateringPlants.s_eventNextState += sAssistanceWaterDialogSecond.goToState(sAssistanceWaterPlantsStandBy);


        // Initial state
        m_stateMachineMain.setGradationInitial("StandBy");

        // Display graph
        m_graph.setManager(m_stateMachineMain);

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Current state: " + m_stateMachineMain.getGradationCurrent());
    }

    void drawLineForPlant(int index)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        Vector3[] corners = m_pathFinderEngine.computePath(m_pointOfReferenceForPaths, m_plantsView[index].transform);

        LineRenderer lineRenderer = m_plantsLines[index].GetComponent<LineRenderer>();
        lineRenderer.positionCount = corners.Length;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 corner = corners[i];

            lineRenderer.SetPosition(i, corner);

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Corner : " + corner);
        }
    }

    /*
     * This callback is aimed to be called when the person left to water plants and comes back.
     * If the person did not water plants in the meantime, then an event is trigerred
     * */
    void callbackAssistanceWateringCheckIfPlantWatered(System.Object o, EventArgs e)
    {
        if (m_sIntermediateWateringPlants.getNbOfStatesWhoCalled() == m_numberOfPlantsWatered)
        { // Then no plants have been watered > trigger event to request additional assistance
            s_assistanceWaterNoPlantWatered?.Invoke(this, EventArgs.Empty);
        }
        else
        { // At least one plant has been watered > update the number of watered plants
            m_numberOfPlantsWatered = m_sIntermediateWateringPlants.getNbOfStatesWhoCalled();
                                                                                                            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceManager, "AssistanceWateringLeavingAndComing", callbackAssistanceWateringCheckIfPlantWatered, m_pointOfReferenceForPaths.gameObject);
        }
    }
}
