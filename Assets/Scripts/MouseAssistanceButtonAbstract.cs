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
 * Abstract class to handles assistances showing a text and a button with a text. 
 * */
public abstract class MouseAssistanceButtonAbstract : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public event EventHandler s_buttonClicked;

    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    protected void onButtonClicked ()
    {
        s_buttonClicked?.Invoke(this, EventArgs.Empty);
    }

    public abstract void show(EventHandler e);

    public abstract void hide(EventHandler e);
}
