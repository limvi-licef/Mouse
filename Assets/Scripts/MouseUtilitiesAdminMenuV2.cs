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
    public GameObject m_hologramDebug;
    public GameObject m_MRTK;

    public bool m_menuStatic = false;

    // Start is called before the first frame update
    void Start()
    {
        m_menuShown = false; // By default, the menu is hidden
        m_positioningInteractionSurfaceEnabled = true; // Enabled by default

        // Check if the occlusion is enabled
        MixedRealityToolkit mrtk = m_MRTK.GetComponent<MixedRealityToolkit>();
        // mrtk.GetService<MixedRealitySpatialAwarenessSystem>().ConfigurationProfile;

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

    public void callbackBringInteractionSurface()
    {
        m_debug.displayMessage("MouseUtilitiesAdminMenu", "callbackBringInteractionSurface", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramInteractionSurface.transform.position = new Vector3(Camera.main.transform.position.x + 1.5f, Camera.main.transform.position.y - 0.5f, Camera.main.transform.position.z);
    }

    public void callbackBringRagInteractionSurface()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");
        m_hologramInteractionSurface.transform.position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y - 0.5f, Camera.main.transform.position.z);
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