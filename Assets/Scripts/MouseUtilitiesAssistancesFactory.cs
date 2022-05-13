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

public class MouseUtilitiesAssistancesFactory : MonoBehaviour
{
    private static MouseUtilitiesAssistancesFactory m_instance;
    public static MouseUtilitiesAssistancesFactory Instance { get { return m_instance; } }

    public MouseAssistanceDialog m_refDialogAssistance;
    public MouseAssistanceBasic m_refCube;
    public MouseAssistanceDialog m_refCheckListAssistance;
    public MouseInteractionSurface m_refInteractionSurface;

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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MouseAssistanceDialog createDialogNoButton(string title, string description, Transform parent)
    {
        Transform dialogView = Instantiate(m_refDialogAssistance.transform, parent);
        MouseAssistanceDialog dialogController = dialogView.GetComponent<MouseAssistanceDialog>();
        dialogController.setTitle(title);
        float sizeDescriptionText = -0.002f * description.Length + 0.38f;
        dialogController.setDescription(description, sizeDescriptionText);
        dialogController.enableBillboard(true);

        return dialogController;
    }

    public MouseAssistanceDialog createCheckListNoButton(string title, string description, Transform parent)
    {
        Transform dialogView = Instantiate(m_refCheckListAssistance.transform, parent);
        MouseAssistanceDialog dialogController = dialogView.GetComponent<MouseAssistanceDialog>();
        dialogController.setTitle(title);
        float sizeDescriptionText = -0.002f * description.Length + 0.38f;
        dialogController.setDescription(description, sizeDescriptionText);
        dialogController.enableBillboard(true);

        return dialogController;
    }

    public MouseAssistanceDialog createDialogTwoButtons(string title, string description, string textButton1, EventHandler callbackButton1, string textButton2, EventHandler callbackButton2, Transform parent)
    {
        Transform dialogView = Instantiate(m_refDialogAssistance.transform, parent);
        MouseAssistanceDialog dialogController = dialogView.GetComponent<MouseAssistanceDialog>();
        dialogController.setTitle(title);
        float sizeDescriptionText = -0.002f * description.Length + 0.38f;
        dialogController.setDescription(description, sizeDescriptionText);
        dialogController.enableBillboard(true);
        dialogController.addButton(textButton1, true);
        dialogController.addButton(textButton2, true);
        dialogController.m_buttonsController[0].s_buttonClicked += callbackButton1;
        dialogController.m_buttonsController[1].s_buttonClicked += callbackButton2;

        return dialogController;
    }

    public MouseAssistanceBasic createCube(string texture, Transform parent)
    {
        Transform cubeView = Instantiate(m_refCube.transform, parent);
        MouseAssistanceBasic cubeController = cubeView.GetComponent<MouseAssistanceBasic>();
        cubeController.setMaterialToChild(texture);

        return cubeController;
    }

    public MouseAssistanceBasic createCube(string texture, bool adjustHeight, Vector3 scale, Vector3 localPosition, bool enableBillboard, Transform parent)
    {
        MouseAssistanceBasic cube = createCube(texture, parent);
        cube.setAdjustHeightOnShow(false);
        cube.setMaterialToChild("Mouse_Yellow_Glowing");
        cube.setScale(scale);
        cube.setLocalPosition(localPosition);
        cube.setBillboard(enableBillboard);

        return cube;
    }

    public MouseInteractionSurface createInteractionSurface(string id, MouseUtilitiesAdminMenu.Panels panel, Vector3 scaling, string texture, bool show, bool resizable, EventHandler onMove, Transform parent)
    {
        Transform interactionSurface = Instantiate(m_refInteractionSurface.transform, parent);
        MouseInteractionSurface controller = interactionSurface.GetComponent<MouseInteractionSurface>();
        controller.setAdminButtons(id, MouseUtilitiesAdminMenu.Panels.Obstacles);
        controller.setScaling(scaling);
        controller.setColor(texture);
        controller.showInteractionSurfaceTable(show);
        controller.setObjectResizable(resizable);
        controller.s_interactionSurfaceMoved += onMove;

        return controller;
    }


}
