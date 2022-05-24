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

using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using TMPro;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;

public class MouseAssistanceDialog : MouseAssistanceAbstract
{
    Transform m_buttonsParentView;
    Transform m_refButtonView;
    Transform m_titleView;
    Transform m_descriptionView;
    Transform m_backgroundView;
    List<Transform> m_buttonsView;
    public List<MouseAssistanceButton> m_buttonsController;

    Vector3 m_buttonsParentScalingOriginal;
    Vector3 m_backgoundScalingOriginal;
    Vector3 m_titleScalingOriginal;
    Vector3 m_descriptionScalingOriginal;
    List<Vector3> m_buttonsScalingOriginal;

    private void Awake()
    {
        // Instantiate variables
        m_buttonsView = new List<Transform>();
        m_buttonsController = new List<MouseAssistanceButton>();
        m_buttonsScalingOriginal = new List<Vector3>();  

        // Children
        m_buttonsParentView = transform.Find("ButtonParent");
        m_refButtonView = m_buttonsParentView.Find("Button");
        m_titleView = transform.Find("TitleText");
        m_descriptionView = gameObject.transform.Find("DescriptionText");
        m_backgroundView = gameObject.transform.Find("ContentBackPlate");

        // Initialize some values of the children
        m_buttonsParentScalingOriginal = m_buttonsParentView.localScale;
        m_backgoundScalingOriginal = m_backgroundView.localScale;
        m_titleScalingOriginal = m_titleView.localScale;
        m_descriptionScalingOriginal = m_descriptionView.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTitle(string text, float fontSize = -1.0f)
    {
        TextMeshPro tmp = m_titleView.GetComponent<TextMeshPro>();

        setTextToTextMeshProComponent(tmp, text, fontSize);
    }

    public void setDescription(string text, float fontSize = -1.0f)
    {
        TextMeshPro tmp = m_descriptionView.GetComponent<TextMeshPro>();

        setTextToTextMeshProComponent(tmp, text, fontSize);       
    }

    void setTextToTextMeshProComponent(TextMeshPro component, string text, float fontSize)
    {
        if (fontSize > 0.0f)
        {
            component.fontSize = fontSize;
        }

        component.SetText(text);
    }

    /**
     * If fontSize < 0.0f, means keep the default value of the button's size. Hence the default value.
     * */
    public void addButton(string text/*, EventHandler eventHandler*/, bool autoScaling, float fontSize = -1.0f)
    {
        // Instantiate the button
        Transform newButton = Instantiate(m_refButtonView, m_buttonsParentView);
        ButtonConfigHelper configHelper = newButton.GetComponent<ButtonConfigHelper>();
        configHelper.MainLabelText = text;

        // Get the text mesh pro component to set the fontsize
        if (fontSize > 0.0f)
        {
            TextMeshPro tmp = newButton.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>();
            tmp.fontSize = fontSize;
        }

        // Store the button
        m_buttonsView.Add(newButton);
        MouseAssistanceButton tempButtonController = newButton.GetComponent<MouseAssistanceButton>();
        m_buttonsController.Add(tempButtonController); // Only for the ease of use, nothing special here.

        // Locate button
        float scalingx = 1.0f;
        if (autoScaling)
        {
            scalingx = 1.0f / (float)(m_buttonsView.Count());
        }
        

        foreach (Transform b in m_buttonsView)
        {
            b.localScale = new Vector3(scalingx, b.localScale.y, b.localScale.z);
        }        

        // Store button scaling
        m_buttonsScalingOriginal.Add(m_buttonsView.Last().localScale);

        // Enable button
        m_buttonsView.Last().gameObject.SetActive(true);

        m_buttonsParentView.GetComponent<GridObjectCollection>().UpdateCollection();
    }

    bool m_mutexHide = false;
    public override void hide(EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true; 

            MouseUtilities.animateDisappearInPlace(m_titleView.gameObject, m_titleScalingOriginal, delegate 
            {
                m_mutexHide = false; 
                eventHandler?.Invoke(this, EventArgs.Empty);
            });

            MouseUtilities.animateDisappearInPlace(m_descriptionView.gameObject, m_descriptionScalingOriginal);

            MouseUtilities.animateDisappearInPlace(m_buttonsParentView.gameObject, m_buttonsParentScalingOriginal);

            MouseUtilities.animateDisappearInPlace(m_backgroundView.gameObject, m_backgoundScalingOriginal);
        }
    }

    bool m_mutexShow = false;
    public override void show (EventHandler eventHandler)
    {
        //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexShow == false)
        {
            m_mutexShow = true; 

            MouseUtilities.adjustObjectHeightToHeadHeight(transform);

            MouseUtilities.animateAppearInPlace(m_backgroundView.gameObject, m_backgoundScalingOriginal, delegate {
                //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Background shown");

                MouseUtilities.animateAppearInPlace(m_titleView.gameObject);
                MouseUtilities.animateAppearInPlace(m_buttonsParentView.gameObject);
                MouseUtilities.animateAppearInPlace(m_descriptionView.gameObject);

                m_mutexShow = false;
                eventHandler?.Invoke(this, EventArgs.Empty);

            });
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked, nothing will happen");
        }
    }

    /**
     * This function changes the color - you still have the responsibility to disable the callback if required
     * */
    public void checkButton(MouseAssistanceButton button, bool check)
    {
        //m_states[currentState.m_currentState.getId()].transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_Cyan_Glowing", typeof(Material)) as Material;
        button.checkButton(check);
    }

    public void enableBillboard(bool enable)
    {
        gameObject.GetComponent<Billboard>().enabled = enable;
    }
}
