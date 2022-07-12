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
    namespace Assistances
    {
        public class Factory : MonoBehaviour
        {
            private static Factory m_instance;
            public static Factory Instance { get { return m_instance; } }

            public Dialog m_refDialogAssistance;
            public Basic m_refCube;
            public Dialog m_refCheckListAssistance;
            public Assistances.InteractionSurface m_refInteractionSurface;
            public ProcessingSurface m_refSurfaceToProcess;
            public Dialog m_refToDoListAssistance;

            private void Awake()
            {
                if (m_instance != null && m_instance != this)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    m_instance = this;
                }
            }

            // Start is called before the first frame update
           /* void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }*/

            public Dialog CreateDialogNoButton(string title, string description, Transform parent)
            {
                Transform dialogView = Instantiate(m_refDialogAssistance.transform, parent);
                Dialog dialogController = dialogView.GetComponent<Dialog>();
                dialogController.setTitle(title);
                //float sizeDescriptionText = -0.002f * description.Length + 0.38f;
                float sizeDescriptionText = -0.00047619f * description.Length + 0.205714286f;
                dialogController.setDescription(description, sizeDescriptionText);
                dialogController.enableBillboard(true);

                return dialogController;
            }

            public Dialog CreateCheckListNoButton(string title, string description, Transform parent)
            {
                Transform dialogView = Instantiate(m_refCheckListAssistance.transform, parent);
                Dialog dialogController = dialogView.GetComponent<Dialog>();
                dialogController.setTitle(title);
                float sizeDescriptionText = -0.00047619f * description.Length + 0.205714286f;
                dialogController.setDescription(description, sizeDescriptionText);
                dialogController.enableBillboard(true);

                return dialogController;
            }

            public Dialog CreateDialogTwoButtons(string title, string description, string textButton1, EventHandler callbackButton1, string textButton2, EventHandler callbackButton2, Transform parent)
            {
                //Transform dialogView = Instantiate(m_refDialogAssistance.transform, parent);
                //MouseAssistanceDialog dialogController = dialogView.GetComponent<MouseAssistanceDialog>();
                //dialogController.setTitle(title);
                //float sizeDescriptionText = -0.002f * description.Length + 0.38f;
                //dialogController.setDescription(description, sizeDescriptionText);
                //dialogController.enableBillboard(true);
                Dialog dialogController = CreateDialogNoButton(title, description, parent);

                float sizeDescriptionText = -0.016666667f * textButton1.Length + 0.366666667f;
                dialogController.addButton(textButton1, true, sizeDescriptionText);
                sizeDescriptionText = -0.016666667f * textButton2.Length + 0.366666667f;
                dialogController.addButton(textButton2, true, sizeDescriptionText);
                dialogController.m_buttonsController[0].s_buttonClicked += callbackButton1;
                dialogController.m_buttonsController[1].s_buttonClicked += callbackButton2;

                return dialogController;
            }

            public Dialog CreateDialogThreeButtons(string title, string description, string textButton1, EventHandler callbackButton1, string textButton2, EventHandler callbackButton2, string textButton3, EventHandler callbackButton3, Transform parent)
            {
                //Transform dialogView = Instantiate(m_refDialogAssistance.transform, parent);
                //MouseAssistanceDialog dialogController = dialogView.GetComponent<MouseAssistanceDialog>();
                //dialogController.setTitle(title);
                //float sizeDescriptionText = -0.002f * description.Length + 0.38f;
                //dialogController.setDescription(description, sizeDescriptionText);
                //dialogController.enableBillboard(true);
                Dialog dialogController = CreateDialogNoButton(title, description, parent);
                dialogController.addButton(textButton1, true);
                dialogController.addButton(textButton2, true);
                dialogController.addButton(textButton3, true);
                dialogController.m_buttonsController[0].s_buttonClicked += callbackButton1;
                dialogController.m_buttonsController[1].s_buttonClicked += callbackButton2;
                dialogController.m_buttonsController[2].s_buttonClicked += callbackButton3;

                return dialogController;
            }

            public Basic CreateCube(string texture, Transform parent)
            {
                Transform cubeView = Instantiate(m_refCube.transform, parent);
                Basic cubeController = cubeView.GetComponent<Basic>();
                cubeController.SetMaterialToChild(texture);

                return cubeController;
            }

            public Basic CreateCube(string texture, bool adjustHeight, Vector3 scale, Vector3 localPosition, bool enableBillboard, Transform parent)
            {
                Basic cube = CreateCube(texture, parent);
                //cube.SetAdjustHeightOnShow(adjustHeight);
                cube.AdjustHeightOnShow = adjustHeight;
                cube.SetScale(scale);
                cube.SetLocalPosition(localPosition);
                cube.SetBillboard(enableBillboard);

                return cube;
            }

            public Basic CreateFlatSurface(string texture, Vector3 localPosition, Transform parent)
            {
                Basic cube = CreateCube(texture, false, new Vector3(1.0f, 0.01f, 1.0f), localPosition, false, parent);

                return cube;
            }

            public Assistances.InteractionSurface CreateInteractionSurface(string id, AdminMenu.Panels panel, Vector3 scaling, string texture, bool show, bool resizable, EventHandler onMove, Transform parent)
            {
                Transform interactionSurface = Instantiate(m_refInteractionSurface.transform, parent);
                Assistances.InteractionSurface controller = interactionSurface.GetComponent<Assistances.InteractionSurface>();
                controller.SetAdminButtons(id, panel);
                controller.SetScaling(scaling);
                controller.SetColor(texture);
                controller.ShowInteractionSurfaceTable(show);
                controller.SetObjectResizable(resizable);
                controller.EventInteractionSurfaceMoved += onMove;

                return controller;
            }

            // CONTINUER LA AUSSI
            public ProcessingSurface CreateSurfaceToProcess(Transform parent)
            {
                Transform view = Instantiate(m_refSurfaceToProcess.transform, parent);

                ProcessingSurface controller = view.GetComponent<ProcessingSurface>();

                //MouseChallengeleanTableSurfaceToPopulateWithCubes
                return controller;
            }

            public Dialog CreateToDoList(string title, string description)
            {
                Transform dialogView = Instantiate(m_refToDoListAssistance.transform);
                Dialog dialogController = dialogView.GetComponent<Dialog>();
                dialogController.setTitle(title, 0.15f);
                float sizeDescriptionText = -0.00047619f * description.Length + 0.205714286f;
                dialogController.setDescription(description, sizeDescriptionText);
                dialogController.enableBillboard(false);

                return dialogController;
            }
        }

    }
}

