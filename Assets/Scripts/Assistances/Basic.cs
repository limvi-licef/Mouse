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

namespace MATCH
{
    namespace Assistances
    {
        public class Basic : MonoBehaviour, IAssistanceBasic
        {
            public Transform ChildView;

            Vector3 ChildScaleOrigin;

            /*MouseUtilitiesMutex m_mutexShow;
            MouseUtilitiesMutex m_mutexHide;*/

            public EventHandler s_touched;

            Dialog Help;

            public bool AdjustHeightOnShow { private get; set; }

            private void Awake()
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Awake called for object " + gameObject.name);

                // Initialize variables

                // Children
                ChildView = gameObject.transform.Find("Child");

                // Scale origin
                ChildScaleOrigin = ChildView.localScale;

                // Adding the touch event
                Utilities.HologramInteractions interactions = ChildView.GetComponent<Utilities.HologramInteractions>();
                if (interactions == null)
                {
                    interactions = ChildView.gameObject.AddComponent<Utilities.HologramInteractions>();
                }
                interactions.EventTouched += delegate (System.Object sender, EventArgs args)
                {
                    s_touched?.Invoke(sender, args);
                };
                //interactions.s

                AdjustHeightOnShow = true;

                // Help buttons
                if (transform.Find("ExclamationMarkButtons"))
                {
                    List<string> buttonsText = new List<string>();
                    buttonsText.Add("?");
                    List<EventHandler> buttonsCallback = new List<EventHandler>();
                    buttonsCallback.Add(CButtonHelp);
                    Help = Assistances.Factory.Instance.CreateButtons("", "", buttonsText, buttonsCallback, transform);
                    Help.gameObject.name = "ExclamationMarkButtons";
                    Help.GetTransform().localPosition = new Vector3(ChildView.localPosition.x, ChildView.localPosition.y - 0.3f, ChildView.localPosition.z);
                    Help.Hide(Utilities.Utility.GetEventHandlerEmpty());
                }
            }

            bool m_mutexShow = false;
            public void Show(EventHandler eventHandler)
            {
                if (m_mutexShow == false)
                {
                    m_mutexShow = true;

                    if (AdjustHeightOnShow)
                    {
                        MATCH.Utilities.Utility.AdjustObjectHeightToHeadHeight(transform);
                    }

                    MATCH.Utilities.Utility.AnimateAppearInPlace(ChildView.gameObject, ChildScaleOrigin, delegate (System.Object o, EventArgs e)
                    {
                        m_mutexShow = false;
                        eventHandler?.Invoke(this, EventArgs.Empty);
                    });
                }
            }

            bool m_mutexHide = false;
            public void Hide(EventHandler eventHandler)
            {
                if (m_mutexHide == false)
                {
                    m_mutexHide = true;

                    MATCH.Utilities.Utility.AnimateDisappearInPlace(ChildView.gameObject, ChildScaleOrigin, delegate (System.Object o, EventArgs e)
                    {
                        ChildView.gameObject.transform.localScale = ChildScaleOrigin;
                        m_mutexHide = false;
                        eventHandler?.Invoke(this, EventArgs.Empty);
                    });
                }
            }

            public void ShowHelp(bool show)
            {
                //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Displaying the help buttons");
                if (show)
                {
                    Help.Show(Utilities.Utility.GetEventHandlerEmpty());
                }
                else
                {
                    Help.Hide(Utilities.Utility.GetEventHandlerEmpty());
                }
            }

            public Transform GetTransform()
            {
                return ChildView;
            }

            public void SetMaterial(string materialName)
            {
                Renderer renderer = ChildView.GetComponent<Renderer>();
                if (renderer != null)
                {
                    //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Material set to " + materialName);
                    renderer.material = Resources.Load(materialName, typeof(Material)) as Material;
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "No renderer component for the child - no action will be done");
                }
            }

            /*public void SetAdjustHeightOnShow(bool enable)
            {
                m_adjustHeight = enable;
            }*/

            public void SetScale(float x, float y, float z)
            {
                SetScale(new Vector3(x, y, z));
                //m_childView.transform.localScale = new Vector3(x, y, z);
                //m_childScaleOrigin = m_childView.transform.localScale;
            }

            public void SetScale(Vector3 scale)
            {
                ChildView.transform.localScale = scale;
                ChildScaleOrigin = ChildView.transform.localScale;
            }

            public Vector3 GetScale()
            {
                return ChildView.transform.localScale;
            }

            public void SetLocalPosition(float x, float y, float z)
            {
                //m_childView.transform.localPosition = new Vector3(x, y, z);
                SetLocalPosition(new Vector3(x, y, z));
            }

            public void SetLocalPosition(Vector3 localPosition)
            {
                ChildView.transform.localPosition = localPosition;
            }

            public Vector3 GetLocalPosition()
            {
                return ChildView.transform.localPosition;
            }

            public void SetBillboard(bool enable)
            {
                ChildView.GetComponent<Billboard>().enabled = enable;
            }

            public void TriggerTouch()
            {
                s_touched?.Invoke(this, EventArgs.Empty);
            }

            public Transform GetChildTransform()
            {
                return ChildView.transform;
            }

            private void CButtonHelp(System.Object o, EventArgs e)
            {

            }
        }

    }
}

