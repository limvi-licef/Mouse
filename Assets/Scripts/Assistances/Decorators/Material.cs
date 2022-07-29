using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/**
 * Material will be set when calling the "show" function
 * */
namespace MATCH
{
    namespace Assistances
    {
        namespace Decorators
        {
            public class Material : IAssistanceBasic
            {
                IAssistanceBasic AssistanceToDecorate;
                String MaterialName;

                public Material(IAssistanceBasic assistanceToDecorate, string materialName): base()
                {
                    AssistanceToDecorate = assistanceToDecorate;
                    MaterialName = materialName;
                }

                public Transform GetTransform()
                {
                    return AssistanceToDecorate.GetTransform();
                }

                public void Hide(EventHandler callback)
                {
                    AssistanceToDecorate.Hide(callback);
                }

                public void SetMaterial(string materialName)
                {
                    AssistanceToDecorate.SetMaterial(materialName);
                }

                public void Show(EventHandler callback)
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Decorated assistance is going to be shown");

                    if (AssistanceToDecorate.GetTransform().gameObject.activeSelf == false)
                    {
                        AssistanceToDecorate.SetMaterial(MaterialName);
                        AssistanceToDecorate.Show(callback);
                    }
                    else
                    {
                        AssistanceToDecorate.Hide(delegate (System.Object o, EventArgs e)
                        {
                            AssistanceToDecorate.SetMaterial(MaterialName);
                            AssistanceToDecorate.Show(callback);
                        });
                    }
                }

                public void ShowHelp(bool show)
                {
                    AssistanceToDecorate.ShowHelp(show);
                }
            }
        }
    }
}