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

        MouseAssistanceBasic triggerGarbage = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Garbage_Level1", demo.transform);
        triggerGarbage.setLocalPosition(new Vector3(-0.2f, triggerGarbage.getLocalPosition().y, triggerGarbage.getLocalPosition().z));
        triggerGarbage.show(MouseUtilities.getEventHandlerEmpty());
        triggerGarbage.s_touched += delegate (System.Object o, EventArgs e)
        {
            if (m_challengeGarbageFirstLevelCalled == false)
            {
                m_challengeGarbage.getInference19h().callbackOneMinuteTrigger();
                m_challengeGarbageFirstLevelCalled = true;
                triggerGarbage.setMaterialToChild("Mouse_Garbage_Level2");
            }
            else
            {
                m_challengeGarbage.getInference19h30().callbackOneMinuteTrigger();
                m_challengeGarbageFirstLevelCalled = false;
                triggerGarbage.setMaterialToChild("Mouse_Garbage_Level1");
            }
        };

        MouseAssistanceBasic triggerWateringPlants = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Flower", demo.transform);
        triggerWateringPlants.setLocalPosition(new Vector3(0f, triggerWateringPlants.getLocalPosition().y, triggerWateringPlants.getLocalPosition().z));
        triggerWateringPlants.show(MouseUtilities.getEventHandlerEmpty());
        triggerWateringPlants.s_touched += delegate (System.Object o, EventArgs e)
        {
            m_challengeWatering.getInference().callbackOneMinuteTrigger();
        };
            

        MouseAssistanceBasic triggerCleanTable = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Clean_Table", demo.transform);
        triggerCleanTable.setLocalPosition(new Vector3(0.2f, triggerCleanTable.getLocalPosition().y, triggerCleanTable.getLocalPosition().z));
        triggerCleanTable.show(MouseUtilities.getEventHandlerEmpty());
        triggerCleanTable.s_touched += delegate (System.Object o, EventArgs e)
        {
            m_challengeTable.getInference().callbackOneMinuteTrigger();
        };

        
    }
}
