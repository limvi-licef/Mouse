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

public class MouseUserDemo : MonoBehaviour
{
    public MouseChallengeTakeOutGarbage m_challengeGarbage;
    bool m_challengeGarbageFirstLevelCalled = false;

    public MouseChallengeWateringThePlants m_challengeWatering;

    public MouseChallengeCleanTable m_challengeTable;

    MouseAssistanceBasic m_triggerGarbage;
    MouseAssistanceBasic m_triggerWateringPlants;
    MouseAssistanceBasic m_triggerCleanTable;

    // Start is called before the first frame update
    void Start()
    {
        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initializeScenario()
    {
        MouseInteractionSurface demo = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("User Demos", MouseUtilitiesAdminMenu.Panels.Obstacles, new Vector3(1.1f, 0.02f, 0.7f), "Mouse_Purple_Glowing", true, false, MouseUtilities.getEventHandlerEmpty(), transform);

        MouseAssistanceBasic demoSurface = MouseUtilitiesAssistancesFactory.Instance.createFlatSurface("Mouse_Cyan_Glowing", new Vector3(demo.getLocalPosition().x, demo.getLocalPosition().y + 0.02f, demo.getLocalPosition().z), demo.transform);
        demoSurface.setScale(new Vector3(demo.getLocalScale().x, demo.getLocalScale().y, demo.getLocalScale().z));
        demoSurface.show(MouseUtilities.getEventHandlerEmpty());

        m_triggerGarbage = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Garbage_Level1", demo.transform);
        m_triggerGarbage.setLocalPosition(new Vector3(-0.2f, m_triggerGarbage.getLocalPosition().y, m_triggerGarbage.getLocalPosition().z));
        m_triggerGarbage.show(MouseUtilities.getEventHandlerEmpty());
        m_triggerGarbage.s_touched += delegate (System.Object o, EventArgs e)
        {
            if (m_challengeGarbageFirstLevelCalled == false)
            {
                m_challengeGarbage.getInference19h().callbackOneMinuteTrigger();
                m_challengeGarbageFirstLevelCalled = true;
                m_triggerGarbage.setMaterialToChild("Mouse_Garbage_Level2");
            }
            else
            {
                m_challengeGarbage.getInference19h30().callbackOneMinuteTrigger();
                m_challengeGarbageFirstLevelCalled = false;
                m_triggerGarbage.setMaterialToChild("Mouse_Garbage_Level2_Pressed");
            }
        };

        m_triggerWateringPlants = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Flower", demo.transform);
        m_triggerWateringPlants.setLocalPosition(new Vector3(0f, m_triggerWateringPlants.getLocalPosition().y, m_triggerWateringPlants.getLocalPosition().z));
        m_triggerWateringPlants.show(MouseUtilities.getEventHandlerEmpty());
        m_triggerWateringPlants.s_touched += delegate (System.Object o, EventArgs e)
        {
            m_challengeWatering.getInference().callbackOneMinuteTrigger();
            m_triggerWateringPlants.setMaterialToChild("Mouse_Flower_Pressed");
        };
            

        m_triggerCleanTable = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Clean_Table", demo.transform);
        m_triggerCleanTable.setLocalPosition(new Vector3(0.2f, m_triggerCleanTable.getLocalPosition().y, m_triggerCleanTable.getLocalPosition().z));
        m_triggerCleanTable.show(MouseUtilities.getEventHandlerEmpty());
        m_triggerCleanTable.s_touched += delegate (System.Object o, EventArgs e)
        {
            m_challengeTable.getInference().callbackOneMinuteTrigger();
            m_triggerCleanTable.setMaterialToChild("Mouse_Clean_Table_Pressed");
        };

        MouseAssistanceDialog dialogInstructions = MouseUtilitiesAssistancesFactory.Instance.createDialogNoButton("", "Touchez un des boutons pour commencer un scénario. Le scénario commence 10 secondes après avoir touché le bouton.", demo.transform);
        //MouseAssistanceDialog dialogInstructions = MouseUtilitiesAssistancesFactory.Instance.createDialogTwoButtons("", "Touchez un des boutons pour commencer un scénario. Le scénario commence 10 secondes après avoir touché le bouton.", "Test bouton 1", MouseUtilities.getEventHandlerEmpty(), "Test button 2", MouseUtilities.getEventHandlerEmpty(), demo.transform);
        //dialogInstructions.setDescription("Touchez un des boutons pour commencer un scénario: sortir les poubelles, arroser les plantes, nettoyer la table.Le scénario commence 10 secondes après avoir touché le bouton.", 0.12f);
        //dialogInstructions.setDescription("Touchez un des boutons pour commencer un scénario. Le scénario commence 10 secondes après avoir touché le bouton.", 0.15f);
        dialogInstructions.m_adjustToHeight = false;
        dialogInstructions.transform.localPosition = new Vector3(0f, 0.5f, 0);
        dialogInstructions.show(MouseUtilities.getEventHandlerEmpty());
        

        m_challengeGarbage.s_challengeOnStandBy += callbackChallengeGarbageStandBy;
        m_challengeWatering.s_challengeOnStandBy += callbackChallengeWateringPlants;
        m_challengeTable.s_challengeOnStandBy += callbackChallengeCleanTable;
    }

    void callbackChallengeGarbageStandBy(System.Object o, EventArgs e)
    {
        m_triggerGarbage.setMaterialToChild("Mouse_Garbage_Level1");
        m_challengeGarbageFirstLevelCalled = false;
    }

    void callbackChallengeCleanTable(System.Object o, EventArgs e)
    {
        m_triggerCleanTable.setMaterialToChild("Mouse_Clean_Table");
    }
    
    void callbackChallengeWateringPlants(System.Object o, EventArgs e)
    {
        m_triggerWateringPlants.setMaterialToChild("Mouse_Flower");
    }
}
