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
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;
using TMPro;



public class MouseUserToDoList : MonoBehaviour //MouseAssistanceDialog
{

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseUtilitiesAdminMenu.Instance.addButton("Bring ToDo List window", callbackBringAgenda);

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void callbackBringAgenda()
    {
        gameObject.transform.position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z);
        gameObject.transform.LookAt(Camera.main.transform);
        gameObject.transform.Rotate(new Vector3(0, 1, 0), 180);
    }

    
}
