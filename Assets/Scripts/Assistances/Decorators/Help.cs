using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/**
 * Very basic for now: when the show function is called, it will hide the assistance and display the help buttons
 * */
namespace MATCH
{
    namespace Assistances
    {
        namespace Decorators
        {
            public class Help : IAssistanceBasic
            {
                IAssistanceBasic AssistanceToDecorate;

                public Help (IAssistanceBasic assistanceToDecorate)
                {
                    AssistanceToDecorate = assistanceToDecorate;
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
                    AssistanceToDecorate.Hide(delegate (System.Object o, EventArgs e)
                    {
                        AssistanceToDecorate.ShowHelp(true);
                        AssistanceToDecorate.Show(callback);
                    });
                }

                public void ShowHelp(bool show)
                {
                    AssistanceToDecorate.ShowHelp(show);
                }
            }
        }
    }
}
