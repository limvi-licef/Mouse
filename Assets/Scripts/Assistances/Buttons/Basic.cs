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
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;

/**
 * To be used with button having the "Interactable" MRTK component.
 * */
namespace MATCH
{
    namespace Assistances
    {
        namespace Buttons
        {
            public class Basic : Button
            {
                bool m_checked = false;

                public override void hide(EventHandler e)
                {
                    throw new NotImplementedException();
                }

                public override void show(EventHandler e)
                {
                    throw new NotImplementedException();
                }

                private void Awake()
                {
                    Interactable interactions = gameObject.GetComponent<Interactable>();
                    interactions.AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(delegate () {
                        onButtonClicked();
                    });
                }

                // Start is called before the first frame update
                void Start()
                {

                }

                // Update is called once per frame
                void Update()
                {

                }

                public void checkButton(bool check)
                {
                    m_checked = check;

                    if (m_checked)
                    {
                        transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_green_glowing", typeof(Material)) as Material;
                    }
                    else
                    {
                        transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_HolographicBackPlate", typeof(Material)) as Material;
                    }
                }

                public bool isChecked()
                {
                    return m_checked;
                }

                public void callbackSetButtonBackgroundCyan(System.Object o, EventArgs e)
                {
                    transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_Cyan_Glowing", typeof(Material)) as Material;
                }

                public void callbackSetButtonBackgroundGreen(System.Object o, EventArgs e)
                {
                    transform.Find("BackPlate").Find("Quad").GetComponent<Renderer>().material = Resources.Load("Mouse_Green_Glowing", typeof(Material)) as Material;
                }
            }

        }
    }
}

