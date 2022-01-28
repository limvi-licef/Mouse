using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit;
using System;


public class MouseChallengeCleanTableInvite : MonoBehaviour
{
    public MouseDebugMessagesManager m_debugMessages;
    public event EventHandler m_mouseChallengeCleanTableInviteHologramTouched;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void  onTouch()
    {
        m_debugMessages.displayMessage("MouseChallengeCubeInteractions", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
        m_mouseChallengeCleanTableInviteHologramTouched?.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {

    }
}