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
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;

/**
 * This class manages a basic show / hide for a child that should be named ... "child" (I am a creative person)
 * An event is emitted if the nested hologram is touched.
 * Appears / disappears in place. I told you it was basic.
 * */
public class MouseAssistanceBasic : MouseAssistanceAbstract
{
    public Transform m_childView;

    Vector3 m_childScaleOrigin;

    /*MouseUtilitiesMutex m_mutexShow;
    MouseUtilitiesMutex m_mutexHide;*/

    public event EventHandler s_touched;

    bool m_adjustHeight;

    private void Awake()
    {
        // Initialize variables

        // Children
        m_childView = gameObject.transform.Find("Child");

        // Scale origin
        m_childScaleOrigin = m_childView.localScale;

        // Adding the touch event
        MouseUtilitiesHologramInteractions interactions = m_childView.GetComponent<MouseUtilitiesHologramInteractions>();
        if (interactions == null)
        {
            interactions = m_childView.gameObject.AddComponent<MouseUtilitiesHologramInteractions>();
        }
        interactions.s_touched += delegate (System.Object sender, EventArgs args)
        {
            s_touched?.Invoke(sender, args);
        };

        m_adjustHeight = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool m_mutexShow = false;
    public override void show(EventHandler eventHandler)
    {
        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            if (m_adjustHeight)
            {
                MouseUtilities.adjustObjectHeightToHeadHeight(transform);
            }   

            MouseUtilities.animateAppearInPlace(m_childView.gameObject, m_childScaleOrigin, delegate (System.Object o, EventArgs e)
            {
                m_mutexShow = false;
                eventHandler?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    bool m_mutexHide = false;
    public override void hide(EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true;

            MouseUtilities.animateDisappearInPlace(m_childView.gameObject, m_childScaleOrigin, delegate (System.Object o, EventArgs e)
            {
                m_childView.gameObject.transform.localScale = m_childScaleOrigin;
                m_mutexHide = false;
                eventHandler?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    public void setMaterialToChild(string materialName)
    {
        Renderer renderer = m_childView.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = Resources.Load(materialName, typeof(Material)) as Material;
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No renderer component for the child - no action will be done");
        }
    }

    public void setAdjustHeightOnShow(bool enable)
    {
        m_adjustHeight = enable;
    }

    public void setScale(float x, float y, float z)
    {
        m_childView.transform.localScale = new Vector3(x, y, z);
        m_childScaleOrigin = m_childView.transform.localScale;
    }

    public void setLocalPosition(float x, float y, float z)
    {
        m_childView.transform.localPosition = new Vector3(x, y, z);
    }

    public void setBillboard(bool enable)
    {
        m_childView.GetComponent<Billboard>().enabled = enable;
    }

    public void triggerTouch()
    {
        s_touched?.Invoke(this, EventArgs.Empty);
    }
}
