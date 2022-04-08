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

/**
 * This class allows to tune some initialization parameters following if the compilation is done in the editor or for the Hololens
 * */
public class MouseUtilitiesGlobalInitializer : MonoBehaviour
{
    public MouseUtilitiesAdminMenu m_adminMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (MouseUtilities.IsEditorSimulator() || MouseUtilities.IsEditorGameView())
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Editor simulator");
        }
        else
        { // Means running in the Hololens, so adjusting some parameters
            MouseDebugMessagesManager.Instance.m_displayOnConsole = false;
            m_adminMenu.m_menuStatic = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
