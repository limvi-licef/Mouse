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
    public GameObject m_refCube;
    public MouseUtilitiesDisplayGraph m_graphMain;
    public MouseUtilitiesDisplayGraph m_graphHelp;
    public MouseUtilitiesDisplayGraph m_graphPlants;
    public MouseUtilitiesContextualInferences m_inferenceManager;

    MouseAssistanceDialog m_dialogAssistanceWaterHelp; // Class variable as used in another function than the initialization of the scenario

    Transform m_pointOfReferenceForPaths;

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

    PlantsManager m_plantsManager;

    MouseUtilitiesGradationAssistanceIntermediateState m_sIntermediateWateringPlants; // This particular state is a class variable as it is used in a callback

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_stateMachineMain = new MouseUtilitiesGradationAssistanceManager();
        m_stateMachineAssistanceWateringPlants = new MouseUtilitiesGradationAssistanceManager();

        m_plantsManager = new PlantsManager(m_graphPlants, transform);

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
        int plantId = m_plantsManager.addPlant();
        Plant p = m_plantsManager.getPlant(plantId);
        p.s_moved += delegate (System.Object o, EventArgs e)
        { // Maybe this part will have to be updated , in the case I add a functionality to remove plants (which is unlikely but we never know).
            drawLineForPlant(plantId);
        };

        // Adding the information to the state machine and the interactions
        p.setState(m_stateMachineMain.addNewAssistanceGradation(p.getId()));
        p.getState().addFunctionShow(delegate (EventHandler e)
        { }, MouseUtilities.getEventHandlerEmpty());
        p.getState().setFunctionHide(delegate (EventHandler e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called for plant index " + plantId);

            p.getHighlight().hide(MouseUtilities.getEventHandlerEmpty());

            // Setting the color back to unwatered
            p.getHighlight().setMaterialToChild("Mouse_Yellow_Glowing");

            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        p.getInteractionSurface().m_eventInteractionSurfaceTableTouched += m_sIntermediateWateringPlants.addState(p.getState());
        p.getInteractionSurface().m_eventInteractionSurfaceTableTouched += p.gotoStateHighlightWatered(); // If we remain in the default attention state, it will just not go there, so should be safe

        // Add plant to the GUI
        m_dialogAssistanceWaterHelp.addButton("Plante " + (plantId + 1), false);
        m_dialogAssistanceWaterHelp.m_buttonsController.Last().s_buttonClicked += delegate (System.Object o, EventArgs e)
        {
            if (m_dialogAssistanceWaterHelp.m_buttonsController[plantId].isChecked() == false)
            {
                drawLineForPlant(plantId);
            }
        };
    }

    /**
     * The code of this function is a mess ... to be restructured
     * */
    void initializeScenario()
    {
        // Object required
        m_pointOfReferenceForPaths = Instantiate(m_refInteractionSurface, transform);
        MouseInteractionSurface interactionSurfaceController = m_pointOfReferenceForPaths.GetComponent<MouseInteractionSurface>();
        interactionSurfaceController.setColor("Mouse_Purple_Glowing");
        interactionSurfaceController.setScaling(new Vector3(0.6f, 0.01f, 0.4f));
        m_pointOfReferenceForPaths.position = new Vector3(0.846999645f, 0.542999983f, 5.22099972f);
        interactionSurfaceController.setAdminButtons("plants", MouseUtilitiesAdminMenu.Panels.Obstacles);
        interactionSurfaceController.showInteractionSurfaceTable(true);
        interactionSurfaceController.setPreventResizeY(true);
        interactionSurfaceController.setObjectResizable(true);

        MouseAssistanceBasic successController = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Congratulation", m_pointOfReferenceForPaths);

        MouseUtilitiesGradationAssistance sSuccess = m_stateMachineMain.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(successController);

        m_sIntermediateWateringPlants = m_stateMachineMain.addIntermediateState("WateringPlants", sSuccess);

        m_dialogAssistanceWaterHelp = MouseUtilitiesAssistancesFactory.Instance.createCheckListNoButton("", "Voici les plantes qu'il vous reste à arroser. Si vous touchez une des plantes, un chemin au sol vous y guidera.", m_pointOfReferenceForPaths);

        // Setting 3 cubes to evaluate if the intermediate state thing works
        callbackAddPlant();
        callbackAddPlant();
        callbackAddPlant();

        // The next lines are to fit with the "virtual room" stuff
        Plant plant1 = m_plantsManager.getPlant(0);
        Plant plant2 = m_plantsManager.getPlant(1);
        Plant plant3 = m_plantsManager.getPlant(2);

        plant1.getInteractionSurface().setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        plant1.getHighlight().setScale(plant1.getInteractionSurface().getInteractionSurface().localScale.x, plant1.getHighlight().getChildTransform().localScale.y, plant1.getInteractionSurface().getInteractionSurface().localScale.z);
        plant1.getInteractionSurface().transform.position = new Vector3(3.31499958f, 0.347000003f, 5.22099972f);
        plant2.getInteractionSurface().setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        plant2.getHighlight().setScale(plant2.getInteractionSurface().getInteractionSurface().localScale.x, plant2.getHighlight().getChildTransform().localScale.y, plant2.getInteractionSurface().getInteractionSurface().localScale.z);
        plant2.getInteractionSurface().transform.position = new Vector3(-1.38000059f, 0.35800001f, 1.05400014f);
        plant3.getInteractionSurface().setScaling(new Vector3(0.3f, 0.02f, 0.3f));
        plant3.getHighlight().setScale(plant3.getInteractionSurface().getInteractionSurface().localScale.x, plant3.getHighlight().getChildTransform().localScale.y, plant3.getInteractionSurface().getInteractionSurface().localScale.z);
        plant3.getInteractionSurface().transform.position = new Vector3(3.35499954f, 0.331999987f, 1.5990001f);

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

        //// Inferences
        DateTime tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
        m_inference19h00 = new MouseUtilitiesInferenceTime("19h watering plants", tempTime, delegate(System.Object o, EventArgs e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

            m_inferenceManager.unregisterInference("19h watering plants");
            s_inference19h00?.Invoke(o, e);
        });
        m_inferenceManager.registerInference(m_inference19h00);

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
        sAssistanceWaterDialogSecond.setFunctionHideAndShow(m_dialogAssistanceWaterHelp);

        m_stateMachineAssistanceWateringPlants.setGradationInitial("standBy");

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

        // Set transitions and intermediate state
        s_inference19h00 += sStandBy.goToState(sDialogFirst);
        s_dialogFirstUserFar += sDialogFirst.goToState(sDialogSecond);
        s_dialogSecondUserFar += sDialogSecond.goToState(sDialogThird);

        faucetController.m_eventInteractionSurfaceTableTouched += sStandBy.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogFirst.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogSecond.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sDialogThird.goToState(m_sIntermediateWateringPlants);
        faucetController.m_eventInteractionSurfaceTableTouched += sAssistanceWaterPlantsStandBy.goToState(sAssistanceWaterStart); // Enabling the nested state machine

        s_assistanceWaterNoPlantWatered += sAssistanceWaterStart.goToState(sAssistanceWaterDialogFirst);
        foreach (EventHandler handler in m_plantsManager.gotoStateHighlight(/*s_assistanceWaterNeedHelp*/))
        {
            s_assistanceWaterNeedHelp += handler;
        }

        s_assistanceWaterNeedHelp += delegate (System.Object o, EventArgs e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called - preparing lines display for the plants");

            int indexPlant = 0;
            for (indexPlant = 0; indexPlant < m_plantsManager.getNbPlants(); indexPlant++)
            {
                LineRenderer line = m_plantsManager.getLineRenderer(indexPlant);
                Plant plant = m_plantsManager.getPlant(indexPlant);

                MouseAssistanceButton plantButton = m_dialogAssistanceWaterHelp.m_buttonsController[indexPlant];
                
                plantButton.checkButton(m_sIntermediateWateringPlants.checkStateCalled(plant.getState()));

                plant.getInteractionSurface().m_eventInteractionSurfaceTableTouched += plant.gotoStateHighlightWatered();
                plant.getInteractionSurface().m_eventInteractionSurfaceTableTouched += delegate (System.Object oo, EventArgs ee)
                {
                    MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called for plant index " + indexPlant);

                    line.positionCount = 0;
                    plantButton.checkButton(true);
                };
            }
        };
        s_assistanceWaterNeedHelp += sAssistanceWaterDialogFirst.goToState(sAssistanceWaterDialogSecond);
        s_assistanceWaterNoNeedForHelp += sAssistanceWaterDialogFirst.goToState(sAssistanceWaterPlantsStandBy);
        
        m_sIntermediateWateringPlants.s_eventNextState += sAssistanceWaterDialogFirst.goToState(sAssistanceWaterPlantsStandBy);
        m_sIntermediateWateringPlants.s_eventNextState += sAssistanceWaterDialogSecond.goToState(sAssistanceWaterPlantsStandBy);
        foreach (EventHandler handler in m_plantsManager.gotoStateDefaultFromHighlightWatered())
        {
            m_sIntermediateWateringPlants.s_eventNextState += handler;
        }
        m_sIntermediateWateringPlants.s_eventNextState += sAssistanceWaterStart.goToState(sAssistanceWaterPlantsStandBy);
        successController.s_touched += sSuccess.goToState(sStandBy);
        
        // Initial state
        m_stateMachineMain.setGradationInitial("StandBy");

        // Display graphs
        m_graphMain.setManager(m_stateMachineMain);
        m_graphHelp.setManager(m_stateMachineAssistanceWateringPlants);
        m_plantsManager.displayGraphLastPlant();

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Current state: " + m_stateMachineMain.getGradationCurrent());
    }

    void drawLineForPlant(int index)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        Plant p = m_plantsManager.getPlant(index);

        Vector3[] corners = m_pathFinderEngine.computePath(m_pointOfReferenceForPaths, p.getInteractionSurface().transform);

        LineRenderer lineRenderer = m_plantsManager.getLineRenderer(index);
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

public class PlantsManager
{
    List<Plant> m_plants;
    List<LineRenderer> m_plantsLines;
    Transform m_parent;
    MouseUtilitiesDisplayGraph m_graph;

    public PlantsManager(MouseUtilitiesDisplayGraph graph, Transform parent)
    {
        m_plants = new List<Plant>();
        m_plantsLines = new List<LineRenderer>();

        m_graph = graph;

        m_parent = parent;
    }

    /**
     * Returns the id of the plant
     * */
    public int addPlant()
    {

        string plantId = "Plante " + (m_plants.Count+1).ToString(); //+1 because the new plant is not yet added to the list of plants
        Plant plant = new Plant(plantId, m_parent); 
        m_plants.Add(plant);
        int indexPlant = m_plants.Count - 1;

        GameObject gameObjectForLine = new GameObject("Line for " + plantId);
        gameObjectForLine.transform.parent = m_parent;
        LineRenderer lineRenderer = gameObjectForLine.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.017f;
        lineRenderer.endWidth = 0.017f;
        lineRenderer.material = Resources.Load("Mouse_Green_Glowing", typeof(Material)) as Material;
        lineRenderer.positionCount = 0;

        m_plantsLines.Add(lineRenderer);
        
        return indexPlant;
    }

    public Plant getPlant(int index)
    {
        return m_plants[index];
    }

    public LineRenderer getLineRenderer(int index)
    {
        return m_plantsLines[index];
    }

    public int getNbPlants()
    {
        return m_plants.Count;
    }

    public List<EventHandler>  gotoStateHighlight()
    {
        List<EventHandler> handlers = new List<EventHandler>();

        foreach (Plant p in m_plants)
        {
            handlers.Add(p.gotoStateHighlight());
        }

        return handlers;
    }

    public List<EventHandler> gotoStateDefaultFromHighlightWatered()
    {
        List<EventHandler> handlers = new List<EventHandler>();

        foreach (Plant p in m_plants)
        {
            handlers.Add(p.gotoStateDefaultFromHighlightWatered());
        }

        return handlers;
    }

    public void displayGraphLastPlant()
    {
        m_plants.Last().displayGraph(m_graph);
    }
}

public class Plant
{
    MouseUtilitiesGradationAssistanceManager m_grabAttentionManager;

    MouseInteractionSurface m_interactionSurface;
    MouseAssistanceBasic m_highlight;

    MouseUtilitiesGradationAssistance m_state; // State to be used in the main state machine

    string m_id;

    public EventHandler s_moved;
    public EventHandler s_scaled;

    public Plant(string id,Transform parent)
    {
        // Variables
        m_grabAttentionManager = new MouseUtilitiesGradationAssistanceManager();

        m_id = id;

        m_interactionSurface = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface(id, MouseUtilitiesAdminMenu.Panels.Obstacles, new Vector3(0.1f, 0.1f, 0.1f), "Mouse_Yellow_Glowing", true, true, delegate (System.Object o, EventArgs e)
        {
            s_moved?.Invoke(this, EventArgs.Empty);
        }, parent);

        m_highlight = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Yellow_Glowing", false, new Vector3(m_interactionSurface.getInteractionSurface().localScale.x, 0.6f, m_interactionSurface.getInteractionSurface().localScale.z), new Vector3(0, -0.35f, 0), false, m_interactionSurface.transform);

        m_interactionSurface.s_interactionSurfaceScaled += delegate
        {
            m_highlight.setScale(m_interactionSurface.getInteractionSurface().localScale.x,
                m_highlight.getChildTransform().localScale.y,
                m_interactionSurface.getInteractionSurface().localScale.z);
        };

        MouseUtilitiesGradationAssistance sDefault = m_grabAttentionManager.addNewAssistanceGradation("default");
        MouseUtilitiesGradationAssistance sHighlight = m_grabAttentionManager.addNewAssistanceGradation("highlight");
        MouseUtilitiesGradationAssistance sHighlightWatered = m_grabAttentionManager.addNewAssistanceGradation("highlightWatered");

        sDefault.addFunctionShow(delegate (EventHandler e)
        {
        }, MouseUtilities.getEventHandlerEmpty());
        sDefault.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        sHighlight.addFunctionShow(delegate (EventHandler e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Plant " + m_id + " is going to be highlighted");

            m_highlight.show(MouseUtilities.getEventHandlerEmpty());
        }, MouseUtilities.getEventHandlerEmpty());
        sHighlight.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        sHighlightWatered.addFunctionShow(delegate (EventHandler e)
        {
            m_highlight.setMaterialToChild("Mouse_Green_Glowing");
        }, MouseUtilities.getEventHandlerEmpty());
        sHighlightWatered.setFunctionHide(delegate (EventHandler e)
        {
            m_highlight.setMaterialToChild("Mouse_Yellow_Glowing");
            m_highlight.hide(MouseUtilities.getEventHandlerEmpty());

            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        m_grabAttentionManager.setGradationInitial("default");s
    }

    public string getId ()
    {
        return m_id;
    }

    public MouseInteractionSurface getInteractionSurface()
    {
        return m_interactionSurface;
    }

    public MouseAssistanceBasic getHighlight()
    {
        return m_highlight;
    }

    public EventHandler gotoStateHighlight()
    {
        return getState("default").goToState(getState("highlight"));
    }

    public EventHandler gotoStateHighlightWatered()
    {
        return getState("highlight").goToState(getState("highlightWatered"));
    }

    public EventHandler gotoStateDefaultFromHighlight()
    {
        return getState("default").goToState(getState("highlight"));
    }

    public EventHandler gotoStateDefaultFromHighlightWatered()
    {
        return getState("highlightWatered").goToState(getState("default"));
    }

    MouseUtilitiesGradationAssistance getState(string id)
    {
        return (MouseUtilitiesGradationAssistance)m_grabAttentionManager.getAssistance(id);
    }

    public void setState(MouseUtilitiesGradationAssistance state)
    {
        m_state = state;
    }

    public MouseUtilitiesGradationAssistance getState()
    {
        return m_state;
    }

    public void displayGraph(MouseUtilitiesDisplayGraph graph)
    {
        graph.setManager(m_grabAttentionManager);
    }
}