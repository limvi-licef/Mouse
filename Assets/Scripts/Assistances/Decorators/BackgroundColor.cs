using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MATCH
{
    namespace Assistances
    {
        namespace Decorators
        {
            public class BackgroundColor : IAssistancePanel
            {
                IAssistancePanel PanelToDecorate;
                string BackgroundColorDecorated;

                public BackgroundColor(IAssistancePanel panelToDecorate, string backgroundColor)
                {
                    PanelToDecorate = panelToDecorate;
                    BackgroundColorDecorated = backgroundColor;
                }

                public void Hide(EventHandler callback)
                {
                    PanelToDecorate.Hide(callback);
                }

                public void Show(EventHandler callback)
                {
                    PanelToDecorate.SetBackgroundColor(BackgroundColorDecorated);
                    PanelToDecorate.Show(callback);
                }

                public void ShowHelp(bool show)
                {
                    PanelToDecorate.ShowHelp(show);
                }

                public void SetBackgroundColor(string colorName)
                {
                    PanelToDecorate.SetBackgroundColor(colorName);
                }

                public void SetEdgeColor(string colorName)
                {
                    PanelToDecorate.SetEdgeColor(colorName);
                }

                public void SetEdgeThickness(float thickness)
                {
                    PanelToDecorate.SetEdgeThickness(thickness);
                }

                public void EnableWeavingHand(bool enable)
                {
                    PanelToDecorate.EnableWeavingHand(enable);
                }

                public Transform GetTransform()
                {
                    return PanelToDecorate.GetTransform();
                }
            }
        }
    }
}


