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

public class MouseChallengeObjectDetectionTest : MouseChallengeAbstract
{
    public MouseUtilitiesContextualInferences m_inferenceManager;

    MouseUtilitiesInferenceObjectInInteractionSurface m_inferenceObjectDetected;

    MouseInteractionSurface m_storage;

    EventHandler s_inferenceObjectDetected;


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Scenario Test start");
        m_storage = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("Storage", default, new Vector3(0.4f, 0.4f, 0.4f), "Mouse_purple_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        
        //m_storage.GetComponent<Collider>().enabled = true;
        //m_storage.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        m_inferenceObjectDetected = new MouseUtilitiesInferenceObjectInInteractionSurface("Test",callbackDetected, "tv", m_storage);
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Scenario Test started");
        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initializeScenario()
    {
        m_inferenceManager.registerInference(m_inferenceObjectDetected);

        MouseUtilitiesAdminMenu.Instance.addSwitchButton("Storage ignore raycast", callbackIgnore);
        //inference
        MouseUtilitiesObjectInformation.Instance.s_objectDetectedInDictionary += callbackTouch; //lancer l'inférence si objet = tv ?

    }
    void callbackDetected(System.Object o, EventArgs e)
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectDetected);

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Callback object detected");
        

        s_inferenceObjectDetected?.Invoke(this, EventArgs.Empty);
    }
    
    
    void callbackTouch(System.Object o, EventArgs e)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "TOUUUUCH");
    }
    
    
    void callbackIgnore()
    {
        m_storage.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    
}
