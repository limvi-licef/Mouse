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

namespace MATCH
{
    public class UserDemo : MonoBehaviour
    {
        public Scenarios.TakeOutGarbage m_challengeGarbage;
        bool m_challengeGarbageFirstLevelCalled = false;

        public Scenarios.WateringThePlants m_challengeWatering;

        public Scenarios.DustingTheTable m_challengeTable;

        Assistances.Basic m_triggerGarbage;
        Assistances.Basic m_triggerWateringPlants;
        Assistances.Basic m_triggerCleanTable;

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
            Assistances.InteractionSurface demo = Assistances.Factory.Instance.CreateInteractionSurface("User Demos", AdminMenu.Panels.Obstacles, new Vector3(1.1f, 0.02f, 0.7f), "Mouse_Purple_Glowing", true, false, Utilities.Utility.GetEventHandlerEmpty(), transform);

            Assistances.Basic demoSurface = Assistances.Factory.Instance.CreateFlatSurface("Mouse_Cyan_Glowing", new Vector3(demo.GetLocalPosition().x, demo.GetLocalPosition().y + 0.02f, demo.GetLocalPosition().z), demo.transform);
            demoSurface.SetScale(new Vector3(demo.GetLocalScale().x, demo.GetLocalScale().y, demo.GetLocalScale().z));
            demoSurface.Show(Utilities.Utility.GetEventHandlerEmpty());

            m_triggerGarbage = Assistances.Factory.Instance.CreateCube("Mouse_Garbage_Level1", demo.transform);
            m_triggerGarbage.SetLocalPosition(new Vector3(-0.2f, m_triggerGarbage.GetLocalPosition().y, m_triggerGarbage.GetLocalPosition().z));
            m_triggerGarbage.Show(Utilities.Utility.GetEventHandlerEmpty());
            m_triggerGarbage.s_touched += delegate (System.Object o, EventArgs e)
            {
                if (m_challengeGarbageFirstLevelCalled == false)
                {
                    m_challengeGarbage.GetInference19h().CallbackOneMinuteTrigger();
                    m_challengeGarbageFirstLevelCalled = true;
                    m_triggerGarbage.SetMaterialToChild("Mouse_Garbage_Level2");
                }
                else
                {
                    m_challengeGarbage.GetInference19h30().CallbackOneMinuteTrigger();
                    m_challengeGarbageFirstLevelCalled = false;
                    m_triggerGarbage.SetMaterialToChild("Mouse_Garbage_Level2_Pressed");
                }
            };

            m_triggerWateringPlants = Assistances.Factory.Instance.CreateCube("Mouse_Flower", demo.transform);
            m_triggerWateringPlants.SetLocalPosition(new Vector3(0f, m_triggerWateringPlants.GetLocalPosition().y, m_triggerWateringPlants.GetLocalPosition().z));
            m_triggerWateringPlants.Show(Utilities.Utility.GetEventHandlerEmpty());
            m_triggerWateringPlants.s_touched += delegate (System.Object o, EventArgs e)
            {
                m_challengeWatering.GetInference().CallbackOneMinuteTrigger();
                m_triggerWateringPlants.SetMaterialToChild("Mouse_Flower_Pressed");
            };


            m_triggerCleanTable = Assistances.Factory.Instance.CreateCube("Mouse_Clean_Table", demo.transform);
            m_triggerCleanTable.SetLocalPosition(new Vector3(0.2f, m_triggerCleanTable.GetLocalPosition().y, m_triggerCleanTable.GetLocalPosition().z));
            m_triggerCleanTable.Show(Utilities.Utility.GetEventHandlerEmpty());
            m_triggerCleanTable.s_touched += delegate (System.Object o, EventArgs e)
            {
                m_challengeTable.GetInference().CallbackOneMinuteTrigger();
                m_triggerCleanTable.SetMaterialToChild("Mouse_Clean_Table_Pressed");
            };

            Assistances.Dialog dialogInstructions = Assistances.Factory.Instance.CreateDialogNoButton("", "Touchez un des boutons pour commencer un scénario. Le scénario commence 10 secondes après avoir touché le bouton.", demo.transform);
            //MouseAssistanceDialog dialogInstructions = MouseUtilitiesAssistancesFactory.Instance.createDialogTwoButtons("", "Touchez un des boutons pour commencer un scénario. Le scénario commence 10 secondes après avoir touché le bouton.", "Test bouton 1", MouseUtilities.getEventHandlerEmpty(), "Test button 2", MouseUtilities.getEventHandlerEmpty(), demo.transform);
            //dialogInstructions.setDescription("Touchez un des boutons pour commencer un scénario: sortir les poubelles, arroser les plantes, nettoyer la table.Le scénario commence 10 secondes après avoir touché le bouton.", 0.12f);
            //dialogInstructions.setDescription("Touchez un des boutons pour commencer un scénario. Le scénario commence 10 secondes après avoir touché le bouton.", 0.15f);
            dialogInstructions.m_adjustToHeight = false;
            dialogInstructions.transform.localPosition = new Vector3(0f, 0.5f, 0);
            dialogInstructions.Show(Utilities.Utility.GetEventHandlerEmpty());


            m_challengeGarbage.s_challengeOnStandBy += callbackChallengeGarbageStandBy;
            m_challengeWatering.s_challengeOnStandBy += callbackChallengeWateringPlants;
            m_challengeTable.s_challengeOnStandBy += callbackChallengeCleanTable;
        }

        void callbackChallengeGarbageStandBy(System.Object o, EventArgs e)
        {
            m_triggerGarbage.SetMaterialToChild("Mouse_Garbage_Level1");
            m_challengeGarbageFirstLevelCalled = false;
        }

        void callbackChallengeCleanTable(System.Object o, EventArgs e)
        {
            m_triggerCleanTable.SetMaterialToChild("Mouse_Clean_Table");
        }

        void callbackChallengeWateringPlants(System.Object o, EventArgs e)
        {
            m_triggerWateringPlants.SetMaterialToChild("Mouse_Flower");
        }
    }

}

