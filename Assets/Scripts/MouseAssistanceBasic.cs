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
public class MouseAssistanceBasic : MonoBehaviour
{
    public Transform m_childView;

    Vector3 m_childScaleOrigin;

    MouseUtilitiesMutex m_mutexShow;
    MouseUtilitiesMutex m_mutexHide;

    public event EventHandler s_touched;

    private void Awake()
    {
        // Initialize variables
        m_mutexShow = new MouseUtilitiesMutex();
        m_mutexHide = new MouseUtilitiesMutex();

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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void show(EventHandler eventHandler)
    {
        if (m_mutexShow.isLocked() == false)
        {
            m_mutexShow.lockMutex();

            MouseUtilities.adjustObjectHeightToHeadHeight(transform);

            EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e) {
                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexShow.unlockMutex();
            }), eventHandler };

            m_childView.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(m_childScaleOrigin, temp);
        }
    }

    public void hide(EventHandler eventHandler)
    {
        if (m_mutexHide.isLocked() == false)
        {
            m_mutexHide.lockMutex();

            EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e) {
                m_childView.gameObject.transform.localScale = m_childScaleOrigin;
                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
                m_childView.gameObject.SetActive(false);
                   m_mutexHide.unlockMutex();
            }), eventHandler };

            m_childView.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(temp);
        }
    }
}
