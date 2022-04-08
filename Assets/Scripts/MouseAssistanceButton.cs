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
public class MouseAssistanceButton : MouseAssistanceButtonAbstract
{
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
            //eventHandler?.Invoke(this, EventArgs.Empty);
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
}
