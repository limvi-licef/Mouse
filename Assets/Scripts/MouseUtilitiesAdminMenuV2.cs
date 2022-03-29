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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using TMPro;
using System.Reflection;

public class MouseUtilitiesAdminMenuV2 : MonoBehaviour
{
    bool m_menuShown;
    public MouseChallengeCleanTableV2 m_challengeCleanTable;
    public MouseDebugMessagesManager m_debug;
    public GameObject m_hologramInteractionSurface;
    public GameObject m_hologramRagInteractionSurface;
    bool m_positioningInteractionSurfaceEnabled;
    bool m_positioningRagInteractionSurfaceEnabled;
    public GameObject m_hologramDebug;
    public GameObject m_MRTK;

    public bool m_menuStatic = false;

    string m_hologramRagInteractionSurfaceMaterialName;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_menuShown = false; // By default, the menu is hidden
        m_positioningInteractionSurfaceEnabled = true; // Enabled by default
        m_positioningRagInteractionSurfaceEnabled = true;

        m_hologramRagInteractionSurfaceMaterialName = m_hologramRagInteractionSurface.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)","");

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Material name: " + m_hologramRagInteractionSurfaceMaterialName);

        // Check if the occlusion is enabled
        MixedRealityToolkit mrtk = m_MRTK.GetComponent<MixedRealityToolkit>();

        if (MouseUtilities.IsEditorSimulator() == false && MouseUtilities.IsEditorGameView() == false)
        { // Means running in the Hololens, so adjusting some parameters
            m_menuStatic = false;
        }

        switchStaticOrMovingMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callbackCubeTouched()
    {
        m_debug.displayMessage("MouseUtilitiesAdminMenu", "callbackCubeTouched", MouseDebugMessagesManager.MessageLevel.Info, "Cube touched");
        m_menuShown = !m_menuShown;

       for (int i = 0; i < gameObject.transform.childCount; i ++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(m_menuShown);
        }
    }

    public void callbackResetChallengeCleanTable()
    {
        m_challengeCleanTable.resetChallenge();
    }

    public void callbackSwitchPositioningInteractionSurface()
    {
        string materialName = "";

        if (m_positioningInteractionSurfaceEnabled)
        {
            m_positioningInteractionSurfaceEnabled = false;
            materialName = "Mouse_White_Transparent";
        }
        else
        {
            m_positioningInteractionSurfaceEnabled = true;
            materialName = "Mouse_Cyan_Glowing";
        }

        m_hologramInteractionSurface.GetComponent<Renderer>().material = Resources.Load(materialName, typeof(Material)) as Material;
        m_hologramInteractionSurface.GetComponent<BoundsControl>().enabled = m_positioningInteractionSurfaceEnabled;
        m_hologramInteractionSurface.GetComponent<TapToPlace>().enabled = m_positioningInteractionSurfaceEnabled;
        m_hologramInteractionSurface.GetComponent<MeshRenderer>().enabled = m_positioningInteractionSurfaceEnabled;
    }

    public void callbackSwitchPositioningRagInteractionSurface()
    {
        string materialName = "";

        if (m_positioningRagInteractionSurfaceEnabled)
        {
            m_positioningRagInteractionSurfaceEnabled = false;
            materialName = "Mouse_White_Transparent";
        }
        else
        {
            m_positioningRagInteractionSurfaceEnabled = true;
            materialName = m_hologramRagInteractionSurfaceMaterialName; //"Mouse_Orange_Glowing";
            
        }

        m_hologramRagInteractionSurface.GetComponent<Renderer>().material = Resources.Load(materialName, typeof(Material)) as Material;
        m_hologramRagInteractionSurface.GetComponent<BoundsControl>().enabled = m_positioningRagInteractionSurfaceEnabled;
        m_hologramRagInteractionSurface.GetComponent<TapToPlace>().enabled = m_positioningRagInteractionSurfaceEnabled;
        m_hologramRagInteractionSurface.GetComponent<MeshRenderer>().enabled = m_positioningRagInteractionSurfaceEnabled;
    }

    public void callbackBringInteractionSurface()
    {
        m_debug.displayMessage("MouseUtilitiesAdminMenu", "callbackBringInteractionSurface", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramInteractionSurface.transform.position = new Vector3(Camera.main.transform.position.x + 1.5f, Camera.main.transform.position.y - 0.5f, Camera.main.transform.position.z);
    }

    public void callbackBringRagInteractionSurface()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramRagInteractionSurface.transform.position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y - 0.5f, Camera.main.transform.position.z);
    }

    public void callbackSwitchStaticOrMovingMenu()
    {
        m_menuStatic = !m_menuStatic;

        switchStaticOrMovingMenu();
    }

    public void callbackDebugSwitchDisplay()
    {
        m_hologramDebug.SetActive(!m_hologramDebug.activeSelf);
    }

    public void callbackDebugBringWindow()
    {
        m_hologramDebug.transform.position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z);
        m_hologramDebug.transform.LookAt(Camera.main.transform);
    }

    public void callbackDebugClearWindow()
    {
        TextMeshPro temp = m_hologramDebug.GetComponent<TextMeshPro>();
        temp.SetText("");
    }

    public void callbackDebugDisplayDebugInWindow()
    {
        m_hologramDebug.GetComponent<MouseDebugMessagesManager>().m_displayOnConsole = false;
    }


    public void switchStaticOrMovingMenu()
    {
        string materialName;

        gameObject.GetComponent<RadialView>().enabled = !m_menuStatic; // Menu static == RadialView must be disabled

        if (gameObject.GetComponent<RadialView>().enabled)
        {
            materialName = "Mouse_Orange_Glowing";
        }
        else
        {
            materialName = "Mouse_Purple_Glowing";
        }

        gameObject.GetComponent<Renderer>().material = Resources.Load(materialName, typeof(Material)) as Material;
    }
}
