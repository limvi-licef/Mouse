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
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using System.Reflection;
using System;
using System.Linq;

public class MouseUtilitiesAdminMenu : MonoBehaviour
{
    public enum Panels
    {
        Default = 0,
        Obstacles = 1
    };

    bool m_menuShown;

    public bool m_menuStatic = false;

    //string m_hologramRagInteractionSurfaceMaterialName;

    private static MouseUtilitiesAdminMenu _instance;

    public static MouseUtilitiesAdminMenu Instance { get { return _instance; } }

    public GameObject m_refButtonSwitch;
    public GameObject m_refButton;

    List<GameObject> m_buttons;
    Dictionary<Panels,Transform> m_panels;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Initialize variables
            m_buttons = new List<GameObject>();
            m_panels = new Dictionary<Panels, Transform>();

            // Get children
            m_panels.Add(Panels.Default,gameObject.transform.Find("PanelDefault").Find("ButtonParent").transform);
            m_panels.Add(Panels.Obstacles, gameObject.transform.Find("PanelObstacles").Find("ButtonParent").transform);

            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_menuShown = false; // By default, the menu is hidden

        switchStaticOrMovingMenu();

        // Add the buttons to manage this menu
        addSwitchButton("Static/Mobile menu", callbackSwitchStaticOrMovingMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addSwitchButton(string text, UnityEngine.Events.UnityAction callback, Panels panel = Panels.Default)
    {
        m_buttons.Add(Instantiate(m_refButtonSwitch, m_panels[panel]));
        m_buttons.Last().GetComponent<Interactable>().GetReceiver<InteractableOnPressReceiver>().OnPress.AddListener(callback);
        m_buttons.Last().GetComponent<Interactable>().GetReceiver<InteractableOnPressReceiver>().InteractionFilter = 0;
        m_buttons.Last().transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>().SetText(text);
        m_panels[panel].GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public void addButton(string text, UnityEngine.Events.UnityAction callback, Panels panel = Panels.Default)
    {
        m_buttons.Add(Instantiate(m_refButton, m_panels[panel]));

        m_buttons.Last().GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;
        m_buttons.Last().GetComponent<ButtonConfigHelper>().SeeItSayItLabelEnabled = false;
        m_buttons.Last().GetComponent<Interactable>().GetReceiver<InteractableOnPressReceiver>().OnPress.AddListener(callback);
        m_buttons.Last().GetComponent<Interactable>().GetReceiver<InteractableOnPressReceiver>().InteractionFilter = 0;
        m_buttons.Last().transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>().SetText(text);
        m_panels[panel].GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public void callbackCubeTouched()
    {
        m_menuShown = !m_menuShown;

       for (int i = 0; i < gameObject.transform.childCount; i ++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(m_menuShown);
        }
    }

    public void callbackSwitchStaticOrMovingMenu()
    {
        m_menuStatic = !m_menuStatic;

        switchStaticOrMovingMenu();
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
