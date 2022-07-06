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

public class MouseChallengeRidTable : MouseChallengeAbstract
{
    public MouseUtilitiesContextualInferences m_inferenceManager;
    public MouseUtilitiesDisplayGraph m_graph;

    MouseUtilitiesGradationAssistanceManager m_gradationManager;

    MouseUtilitiesInferenceObjectInInteractionSurface m_inferenceObjectInDishWasher;
    MouseUtilitiesInferenceObjectOutInteractionSurface m_inferenceObjectOutPlateArea;
    MouseUtilitiesInferenceObjectInInteractionSurface m_inferenceObjectInPlateArea;

    MouseInteractionSurface m_dishwasher;
    MouseInteractionSurface m_platearea;

    EventHandler s_inferenceObjectDetectedInDishWasher;
    EventHandler s_inferenceObjectDetectedOutPlateArea;
    EventHandler s_inferenceObjectDetectedInPlateArea;


    private void Awake()
    {
        m_gradationManager = new MouseUtilitiesGradationAssistanceManager();
        setId("Débarrasser la table");
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseUtilitiesAdminMenu.Instance.addSwitchButton("Storage ignore raycast", callbackIgnore);
        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initializeScenario()
    {
        MouseScenarioManager.Instance.addScenario(this);

        //Surfaces
        m_platearea = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("PlateArea", default, new Vector3(0.4f, 0.2f, 0.4f), "Mouse_Green_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        m_platearea.setLocalPosition(new Vector3(0, 1, 0));
        m_dishwasher = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("DishWasher", default, new Vector3(0.4f, 0.4f, 0.4f), "Mouse_Purple_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        m_dishwasher.setLocalPosition(new Vector3(0, 1, 0));

        MouseAssistanceBasic successController = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Congratulation",m_dishwasher.transform);


        //Inferences
        m_inferenceObjectInPlateArea = new MouseUtilitiesInferenceObjectInInteractionSurface("In Plate Area", callbackDetectedInPlateArea, "frisbee", m_platearea); //frisbee = plate
        m_inferenceManager.registerInference(m_inferenceObjectInPlateArea);

        //*States*//

        //Stand by
        MouseUtilitiesGradationAssistance sStandBy = m_gradationManager.addNewAssistanceGradation("StandBy");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            
            m_inferenceObjectInPlateArea = new MouseUtilitiesInferenceObjectInInteractionSurface("In Plate Area", callbackDetectedInPlateArea, "frisbee", m_platearea);
            m_inferenceManager.registerInference(m_inferenceObjectInPlateArea);
            onChallengeStandBy();
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        //Plate on table
        MouseUtilitiesGradationAssistance sOnTable = m_gradationManager.addNewAssistanceGradation("Plate On Table");
        sOnTable.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceObjectOutPlateArea = new MouseUtilitiesInferenceObjectOutInteractionSurface("Out Plate Area", callbackDetectedOutPlateArea, "frisbee", m_platearea);
            m_inferenceManager.registerInference(m_inferenceObjectOutPlateArea);
        }, MouseUtilities.getEventHandlerEmpty());
        sOnTable.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        //Transport
        MouseUtilitiesGradationAssistance sTransport = m_gradationManager.addNewAssistanceGradation("Transport");
        sTransport.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceObjectInDishWasher = new MouseUtilitiesInferenceObjectInInteractionSurface("In Dish Washer", callbackDetectedInDishWasher, "frisbee", m_dishwasher);
            m_inferenceManager.registerInference(m_inferenceObjectInDishWasher);
        }, MouseUtilities.getEventHandlerEmpty());
        sTransport.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        //Success
        MouseUtilitiesGradationAssistance sSuccess = m_gradationManager.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(successController);
        sSuccess.addFunctionShow(delegate (EventHandler e)
        {
            onChallengeSuccess();
        }, MouseUtilities.getEventHandlerEmpty());



        //Connexions
        s_inferenceObjectDetectedInPlateArea += sStandBy.goToState(sOnTable);
        s_inferenceObjectDetectedOutPlateArea += sOnTable.goToState(sTransport);
        s_inferenceObjectDetectedInDishWasher += sTransport.goToState(sSuccess);
        successController.s_touched += sSuccess.goToState(sStandBy);

        m_gradationManager.setGradationInitial("StandBy");

        //Graph
        m_graph.setManager(m_gradationManager);

    }
    void callbackDetectedInPlateArea(System.Object o, EventArgs e) //Callback emitted when the object is in the plate area
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectInPlateArea);
        s_inferenceObjectDetectedInPlateArea?.Invoke(this, EventArgs.Empty);
    }

    void callbackDetectedInDishWasher(System.Object o, EventArgs e) //Callback emitted when the object is in the dish washer area
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectInDishWasher);
        s_inferenceObjectDetectedInDishWasher?.Invoke(this, EventArgs.Empty);
    }

    void callbackDetectedOutPlateArea(System.Object o, EventArgs e) //Callback emitted when the object is out of the plate area
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectOutPlateArea);
        s_inferenceObjectDetectedOutPlateArea?.Invoke(this, EventArgs.Empty);
    }
        
    void callbackIgnore() //callback emitted when the button is clicked : necessary for the proper functioning of the scenario
    {
        
        if (m_dishwasher.getInteractionSurface().gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            m_dishwasher.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Default");
            m_platearea.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            m_dishwasher.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            m_platearea.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

    }
    
}
